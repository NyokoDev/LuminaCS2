// <copyright file="CubemapManager.cs" company="NyokoDev">
// Copyright (c) NyokoDev. All rights reserved.
// </copyright>

namespace Lumina
{
    using Lumina.Systems;
    using Unity.Entities;

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
            Lumina.Mod.Log.Info("CubemapUpdateSystem initialized.");
        }

        /// <summary>
        /// Updates cubemap on update to prevent LightingSystem volume to reset their volume causing it to dissappear.
        /// </summary>
        protected override void OnUpdate()
        {
            RenderEffectsSystem.ApplyCubemap();
        }
    }
}