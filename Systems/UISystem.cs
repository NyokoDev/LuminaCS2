using Colossal.UI.Binding;
using Game.UI;
using Game.UI.InGame;
using Lumina.UI;
using Lumina.XML;
using LuminaMod.XML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using static UnityEngine.Rendering.DebugUI;

namespace Lumina.Systems
{
    internal partial class UISystem : UISystemBase
    {
        public bool Visible { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            CreateBindings();


        }



        /// <summary>
        /// Creates bindings for the mod.
        /// </summary>
        private void CreateBindings()
        {
           
            Checkboxes();
            ColorAdjustments();

            WhiteBalance();
            WhiteBalanceCheckboxes();

            StartShadowsMidtonesHighlights();
            ShadowsMidtonesHighlightsCheckboxes();


          

            //LegacyUI
            AddBinding(new TriggerBinding(Mod.MOD_UI, "OpenLegacyUI", OpenLegacyUI));
        }

        private void ShadowsMidtonesHighlightsCheckboxes()
        {
            AddBinding(new TriggerBinding(Mod.MOD_UI, "SetShadowsCheckbox", SetShadowsCheckbox));
            AddUpdateBinding(new GetterValueBinding<bool>(Mod.MOD_UI, "GetShadowsCheckbox", () => GetShadowsCheckbox()));

            AddBinding(new TriggerBinding(Mod.MOD_UI, "SetMidtonesCheckbox", SetMidtonesCheckbox));
            AddUpdateBinding(new GetterValueBinding<bool>(Mod.MOD_UI, "GetMidtonesCheckbox", () => GetMidtonesCheckbox()));

            AddBinding(new TriggerBinding(Mod.MOD_UI, "SetHighlightsCheckbox", SetHighlightsCheckbox));
            AddUpdateBinding(new GetterValueBinding<bool>(Mod.MOD_UI, "GetHighlightsCheckbox", () => GetHighlightsCheckbox()));




        }

        private bool GetHighlightsCheckbox()
        {
            return GlobalVariables.Instance.HighlightsActive;
        }

        private void SetHighlightsCheckbox()
        {
            GlobalVariables.Instance.HighlightsActive = !GlobalVariables.Instance.HighlightsActive;
        }

        private void SetMidtonesCheckbox()
        {
            GlobalVariables.Instance.MidtonesActive = !GlobalVariables.Instance.MidtonesActive;
        }

        private bool GetMidtonesCheckbox()
        {
            return GlobalVariables.Instance.MidtonesActive;
        }

        private bool GetShadowsCheckbox()
        {
            return GlobalVariables.Instance.ShadowsActive;
        }

        private void SetShadowsCheckbox()
        {
            GlobalVariables.Instance.ShadowsActive = !GlobalVariables.Instance.ShadowsActive;
        }

        private void StartShadowsMidtonesHighlights()
        {

            AddUpdateBinding(new GetterValueBinding<float>(Mod.MOD_UI, "GetShadows", () => GetShadows()));
            AddUpdateBinding(new GetterValueBinding<float>(Mod.MOD_UI, "GetMidtones", () => GetMidtones()));
            AddUpdateBinding(new GetterValueBinding<float>(Mod.MOD_UI, "GetHighlights", () => GetHighlights()));

            AddBinding(new TriggerBinding<float>(Mod.MOD_UI, "SetShadows", SetShadows));
            AddBinding(new TriggerBinding<float>(Mod.MOD_UI, "SetMidtones", SetMidtones));
            AddBinding(new TriggerBinding<float>(Mod.MOD_UI, "SetHighlights", SetHighlights));
        }

        private void WhiteBalance()
        {
            AddUpdateBinding(new GetterValueBinding<float>(Mod.MOD_UI, "GetTemperature", () => GetTemperature()));
            AddUpdateBinding(new GetterValueBinding<float>(Mod.MOD_UI, "GetTint", () => GetTint()));

            AddBinding(new TriggerBinding<float>(Mod.MOD_UI, "SetTemperature", SetTemperature));
            AddBinding(new TriggerBinding<float>(Mod.MOD_UI, "SetTint", SetTint));
        }

        private void WhiteBalanceCheckboxes()
        {
            AddBinding(new TriggerBinding(Mod.MOD_UI, "SetTempCheckbox", SetTempCheckbox));
            AddUpdateBinding(new GetterValueBinding<bool>(Mod.MOD_UI, "GetTempCheckbox", () => GetTempCheckbox()));

            AddBinding(new TriggerBinding(Mod.MOD_UI, "SetTintCheckbox", SetTintCheckbox));
            AddUpdateBinding(new GetterValueBinding<bool>(Mod.MOD_UI, "GetTintCheckbox", () => GetTintCheckbox()));


        }

        private bool GetTintCheckbox()
        {
            return GlobalVariables.Instance.TintActive;
        }

        private void SetTintCheckbox()
        {
            GlobalVariables.Instance.TintActive = !GlobalVariables.Instance.TintActive;
        }

        private bool GetTempCheckbox()
        {
            return GlobalVariables.Instance.TemperatureActive;
        }

        private void SetTempCheckbox()
        {
            GlobalVariables.Instance.TemperatureActive = !GlobalVariables.Instance.TemperatureActive;
        }

        private void ColorAdjustments()
        {
            //AddBinding(_ShowUI = new(Mod.MOD_UI, "MIT_ShowUI", false));
            AddBinding(new TriggerBinding(Mod.MOD_UI, "MIT_EnableToggle", SliderPanel.Toggle));
            AddBinding(new TriggerBinding(Mod.MOD_UI, "LUM_SendToLumina", SliderPanel.SendToLumina));

            AddBinding(new TriggerBinding<float>(Mod.MOD_UI, "SetPostExposure", SetPostExposure));
            AddUpdateBinding(new GetterValueBinding<float>(Mod.MOD_UI, "PostExposure", () => GetSetPostExposure()));

            AddBinding(new TriggerBinding<float>(Mod.MOD_UI, "SetContrast", SetContrast));
            AddUpdateBinding(new GetterValueBinding<float>(Mod.MOD_UI, "GetContrast", () => GetContrast()));

            AddUpdateBinding(new GetterValueBinding<float>(Mod.MOD_UI, "GetHueShift", () => GetHueShift()));
            AddBinding(new TriggerBinding<float>(Mod.MOD_UI, "SetHueShift", SetHueShift));

            AddUpdateBinding(new GetterValueBinding<float>(Mod.MOD_UI, "GetSaturation", () => GetSaturation()));
            AddBinding(new TriggerBinding<float>(Mod.MOD_UI, "SetSaturation", SetSaturation));

            AddBinding(new TriggerBinding(Mod.MOD_UI, "Save", SaveToFile));
            AddBinding(new TriggerBinding(Mod.MOD_UI, "ResetLuminaSettings", Reset));
        }

        private void Checkboxes()
        {
            AddBinding(new TriggerBinding(Mod.MOD_UI, "SetPostExposureCheckbox", SetPostExposureCheckbox));
            AddUpdateBinding(new GetterValueBinding<bool>(Mod.MOD_UI, "GetPostExposureCheckbox", () => GetPostExposureCheckbox()));


            AddBinding(new TriggerBinding(Mod.MOD_UI, "SetcontrastCheckbox", SetcontrastCheckbox));
            AddUpdateBinding(new GetterValueBinding<bool>(Mod.MOD_UI, "GetcontrastCheckbox", () => GetcontrastCheckbox()));

            AddBinding(new TriggerBinding(Mod.MOD_UI, "SethueshiftCheckbox", SethueshiftCheckbox));
            AddUpdateBinding(new GetterValueBinding<bool>(Mod.MOD_UI, "GethueshiftCheckbox", () => GethueshiftCheckbox()));

            AddBinding(new TriggerBinding(Mod.MOD_UI, "SetsaturationCheckbox", SetsaturationCheckbox));
            AddUpdateBinding(new GetterValueBinding<bool>(Mod.MOD_UI, "GetsaturationCheckbox", () => GetsaturationCheckbox()));
        }

        private bool GetsaturationCheckbox()
        {
            return GlobalVariables.Instance.saturationActive;
        }

        private void SetsaturationCheckbox()
        {
            GlobalVariables.Instance.saturationActive = !GlobalVariables.Instance.saturationActive;
        }

        private bool GethueshiftCheckbox()
        {
            return GlobalVariables.Instance.hueShiftActive;
        }

        private void SethueshiftCheckbox()
        {
           GlobalVariables.Instance.hueShiftActive = !GlobalVariables.Instance.hueShiftActive;
        }

        private bool GetcontrastCheckbox()
        {
            return GlobalVariables.Instance.contrastActive;
        }

        private void SetcontrastCheckbox()
        {
            GlobalVariables.Instance.contrastActive = !GlobalVariables.Instance.contrastActive;
        }

        private bool GetPostExposureCheckbox()
        {
            return GlobalVariables.Instance.PostExposureActive;
        }

        private void SetPostExposureCheckbox()
        {
            GlobalVariables.Instance.PostExposureActive = !GlobalVariables.Instance.PostExposureActive;
        }

        private void OpenLegacyUI()
        {
            SliderPanel.panelVisible = true;
        }


        private float GetHighlights()
        {
            return GlobalVariables.Instance.Highlights;
        }

        private float GetMidtones()
        {
            return GlobalVariables.Instance.Midtones;
        }

        private float GetShadows()
        {
            return GlobalVariables.Instance.Shadows;
        }

        private void SetHighlights(float obj)
        {
            GlobalVariables.Instance.Highlights = obj;
        }

        private void SetMidtones(float obj)
        {
            GlobalVariables.Instance.Midtones = obj;
        }

        private void SetShadows(float obj)
        {
            GlobalVariables.Instance.Shadows = obj;
        }

        private void Reset()
        {
            SliderPanel.showResetDialog = true;
        }

        private void SaveToFile()
        {
            GlobalVariables.SaveToFile(GlobalPaths.GlobalModSavingPath);
        }

        private void SetTint(float obj)
        {
            GlobalVariables.Instance.Tint = obj;
        }

        private void SetTemperature(float obj)
        {
            GlobalVariables.Instance.Temperature = obj;
        }

        private float GetTint()
        {
            return GlobalVariables.Instance.Tint;
        }

        private float GetTemperature()
        {
            return GlobalVariables.Instance.Temperature;
        }

        private void SetSaturation(float obj)
        {
            GlobalVariables.Instance.Saturation = obj;
        }

        private float GetSaturation()
        {
            return GlobalVariables.Instance.Saturation;
        }

        private void SetHueShift(float obj)
        {
            GlobalVariables.Instance.hueShift = obj; 
        }

        private float GetHueShift()
        {
            return GlobalVariables.Instance.hueShift;
        }

        private float GetContrast()
        {
            return GlobalVariables.Instance.Contrast;
        }

        private void SetContrast(float obj)
        {
            GlobalVariables.Instance.Contrast = obj;
        }

        private void SetPostExposure(float obj)
        {
            GlobalVariables.Instance.PostExposure = obj;
        }

        private float GetSetPostExposure()
        {
            return GlobalVariables.Instance.PostExposure;
           
        }
    }
}
