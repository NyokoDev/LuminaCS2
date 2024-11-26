// <copyright file="CubemapManager.cs" company="NyokoDev">
// Copyright (c) NyokoDev. All rights reserved.
// </copyright>

namespace Lumina.ManagerSystems
{
    using Lumina.Systems;
    using LuminaMod.XML;
    using Unity.Entities;
    using UnityEngine;

    /// <summary>
    /// Cubemap update system.
    /// </summary>
    internal partial class CubemapUpdateSystem : SystemBase
    {
        /// <summary>
        /// Called when the system is created.
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();
            Mod.Log.Info("CubemapUpdateSystem initialized.");
        }

        /// <summary>
        /// </summary>
        protected override void OnUpdate()
        {
        }
    }
}