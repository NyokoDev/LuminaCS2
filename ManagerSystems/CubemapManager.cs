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
        /// Updates cubemap on update to prevent LightingSystem volume to reset their volume causing it to dissappear.
        /// </summary>
        protected override void OnUpdate()
        {
            if (GlobalVariables.Instance.HDRISkyEnabled)
            {
                RenderEffectsSystem.AdjustAngularDiameter();
                RenderEffectsSystem.ApplyCubemap();
            }
        }
    }
}