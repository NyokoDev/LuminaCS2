using Colossal.UI.Binding;
using Game.UI;
using Lumina.UI;
using Lumina.XML;
using LuminaMod.XML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

namespace Lumina.Systems
{
    internal partial class UISystem : UISystemBase
    {
        public bool Visible { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();

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

            AddUpdateBinding(new GetterValueBinding<float>(Mod.MOD_UI, "GetTemperature", () => GetTemperature()));
            AddUpdateBinding(new GetterValueBinding<float>(Mod.MOD_UI, "GetTint", () => GetTint()));

            AddBinding(new TriggerBinding<float>(Mod.MOD_UI, "SetTemperature", SetTemperature));
            AddBinding(new TriggerBinding<float>(Mod.MOD_UI, "SetTint", SetTint));


            AddBinding(new TriggerBinding(Mod.MOD_UI, "Save", SaveToFile));
            AddBinding(new TriggerBinding(Mod.MOD_UI, "ResetLuminaSettings", Reset));


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
