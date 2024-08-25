namespace Lumina.Systems
{
    using System;
    using System.Collections.Generic;
    using System.Drawing.Text;
    using System.Reflection;
    using Colossal;
    using Game.Rendering;
    using Game.Rendering.CinematicCamera;
    using Game.Simulation;
    using Game.UI.Widgets;
    using HarmonyLib;
    using Lumina.UI;
    using LuminaMod.XML;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.HighDefinition;
    using static Game.Rendering.Debug.RenderPrefabRenderer;

    /// <summary>
    /// Extracts properties from Photo Mode.
    /// </summary>
    public partial class PhotoModeExtractor : SystemBase
    {

        /// <summary>
        /// ColorAdjustments properties from Photo Mode.
        /// </summary>
        ///
        public static ColorAdjustments SavedColorAdjustments;

        /// <summary>
        /// Extracts ColorAdjustments from Photo Mode.
        /// </summary>
        public static void ExtractColorAdjustments()
        {
            SliderPanel.slider1Value = SavedColorAdjustments.postExposure.value;
            SliderPanel.slider2Value = SavedColorAdjustments.contrast.value;
            SliderPanel.slider3Value = SavedColorAdjustments.hueShift.value;
            SliderPanel.slider4Value = SavedColorAdjustments.saturation.value;

            GlobalVariables.Instance.PostExposure = SavedColorAdjustments.postExposure.value;
            GlobalVariables.Instance.Contrast = SavedColorAdjustments.contrast.value;
            GlobalVariables.Instance.HueShift = SavedColorAdjustments.hueShift.value;
            GlobalVariables.Instance.Saturation = SavedColorAdjustments.saturation.value;
        }

        /// <summary>
        /// OnUpdate method.
        /// </summary>
        protected override void OnUpdate()
        {
        }
    }
}
