// -----------------------------------------------------------------------
// <copyright file="SpawnChanceWeight.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CandyWeight.Patches
{
#pragma warning disable SA1118
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using HarmonyLib;
    using InventorySystem.Items.Usables.Scp330;
    using NorthwoodLib.Pools;
    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CandyPink.SpawnChanceWeight"/> to set the weight of the pink candy.
    /// </summary>
    internal static class SpawnChanceWeight
    {
        /// <summary>
        /// Patches all classes that inherit from <see cref="ICandy"/> with <see cref="Transpiler"/>.
        /// </summary>
        /// <param name="harmony">The harmony instance to patch with.</param>
        internal static void Patch(Harmony harmony)
        {
            Type candyType = typeof(ICandy);
            HarmonyMethod transpiler = new HarmonyMethod(typeof(SpawnChanceWeight).GetMethod(nameof(Transpiler), BindingFlags.NonPublic | BindingFlags.Static));
            foreach (Type type in candyType.Assembly.GetTypes())
            {
                if (type.GetInterfaces().Contains(candyType) && type.GetProperty("SpawnChanceWeight", BindingFlags.Public | BindingFlags.Instance) is PropertyInfo property)
                    harmony.Patch(property.GetMethod, transpiler: transpiler);
            }
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label skipModificationLabel = generator.DefineLabel();

            LocalBuilder weight = generator.DeclareLocal(typeof(float));
            LocalBuilder modifiedWeight = generator.DeclareLocal(typeof(float));

            const int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldc_R4) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Stloc_S, weight.LocalIndex),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Plugin), nameof(Plugin.Instance))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Plugin), nameof(Plugin.Config))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Config), nameof(Config.Weights))),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ICandy), nameof(ICandy.Kind))),
                new CodeInstruction(OpCodes.Ldloca_S, modifiedWeight.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Dictionary<CandyKindID, float>), nameof(Dictionary<CandyKindID, float>.TryGetValue))),
                new CodeInstruction(OpCodes.Brfalse_S, skipModificationLabel),
                new CodeInstruction(OpCodes.Ldloc_S, modifiedWeight.LocalIndex),
                new CodeInstruction(OpCodes.Stloc_S, weight.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, weight.LocalIndex).WithLabels(skipModificationLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}