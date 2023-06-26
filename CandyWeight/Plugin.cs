// -----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CandyWeight
{
    using System;
    using CandyWeight.Patches;
    using Exiled.API.Features;
    using HarmonyLib;

    /// <inheritdoc />
    public class Plugin : Plugin<Config>
    {
        private Harmony harmony;

        /// <summary>
        /// Gets a static instance of the <see cref="Plugin"/> class.
        /// </summary>
        public static Plugin Instance { get; private set; }

        /// <inheritdoc/>
        public override string Author => "Build";

        /// <inheritdoc/>
        public override string Name => "CandyWeight";

        /// <inheritdoc/>
        public override string Prefix => "CandyWeight";

        /// <inheritdoc/>
        public override Version Version { get; } = new(3, 0, 1);

        /// <inheritdoc/>
        public override Version RequiredExiledVersion { get; } = new(7, 0, 0);

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Instance = this;
            harmony = new Harmony($"pinkCandyWeight.{DateTime.UtcNow.Ticks}");
            SpawnChanceWeight.Patch(harmony);
            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            harmony?.UnpatchAll(harmony.Id);
            harmony = null;
            Instance = null;
            base.OnDisabled();
        }
    }
}