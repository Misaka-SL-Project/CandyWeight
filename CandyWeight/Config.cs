// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Build">
// Copyright (c) Build. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace CandyWeight
{
    using System.Collections.Generic;
    using Exiled.API.Interfaces;
    using InventorySystem.Items.Usables.Scp330;

    /// <inheritdoc />
    public class Config : IConfig
    {
        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the adjusted weights of the candies.
        /// </summary>
        public Dictionary<CandyKindID, float> Weights { get; set; } = new()
        {
            { CandyKindID.Pink, 1f },
        };
    }
}