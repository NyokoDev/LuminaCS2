using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using Unity.Entities;
using Game.Rendering;
using Lumina.UI;
using static Game.Rendering.Debug.RenderPrefabRenderer;
using System.Drawing.Text;
using UnityEngine.Rendering;
using Game.UI.Widgets;
using HarmonyLib;
using LuminaMod.XML;
using Game.Simulation;
using Game.Rendering.CinematicCamera;
using System.Collections.Generic;
using Colossal;

namespace Lumina.Systems
{
    public partial class PhotoModeExtractor : SystemBase
    {

        private PhotoModeRenderSystem photoModeRenderSystem = null;
        string[] EffectsMerged;

        private ColorAdjustments m_ColorAdjustments;
        private WhiteBalance m_WhiteBalance;
        private ShadowsMidtonesHighlights m_ShadowsMidtonesHighlights;
        private static PhotoModeExtractor instance;
        public string volumeTag = "Post Processing Settings"; // Tag assigned to your Volume GameObject

        private Volume PhotoModeVolume;

        public bool WhiteBalancedExtracted = false;


        /// <summary>
        /// Photo Mode ones.
        /// </summary>
        /// 
        public static ColorAdjustments SavedColorAdjustments;


        public static void ExtractColorAdjustments()
        {
            SliderPanel.slider1Value = SavedColorAdjustments.postExposure.value;
            SliderPanel.slider2Value = SavedColorAdjustments.contrast.value;
            SliderPanel.slider3Value = SavedColorAdjustments.hueShift.value;
            SliderPanel.slider4Value = SavedColorAdjustments.saturation.value;

            GlobalVariables.Instance.PostExposure = SavedColorAdjustments.postExposure.value;
            GlobalVariables.Instance.Contrast = SavedColorAdjustments.contrast.value;
            GlobalVariables.Instance.hueShift = SavedColorAdjustments.hueShift.value;
            GlobalVariables.Instance.Saturation = SavedColorAdjustments.saturation.value;
        }

        protected override void OnUpdate()
        {
        
        }
    }
}
