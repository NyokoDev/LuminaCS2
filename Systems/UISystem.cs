namespace Lumina.Systems
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Colossal.UI.Binding;
    using Game.UI;
    using Game.UI.InGame;
    using Lumina.Systems.Presets;
    using Lumina.Systems.Tonemapping_Custom;
    using Lumina.UI;
    using Lumina.XML;
    using LuminaMod.XML;
    using UnityEngine;
    using UnityEngine.Experimental.Rendering;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.HighDefinition;
    using static UnityEngine.Rendering.DebugUI;

    internal partial class UISystem : ExtendedUISystemBase
    {
        public bool Visible { get; set; }
        public string CubemapName;
        public bool UsingHDRSky = GlobalVariables.Instance.HDRISkyEnabled;

        private ValueBindingHelper<string[]> LutArrayExtended;
        private ValueBindingHelper<string[]> CubemapArrayExtended;
        private ValueBindingHelper<string> LutName;

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            base.OnCreate();
            InitializeLutName();
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
            PlanetarySettings();




            //LegacyUI
            AddBinding(new TriggerBinding(Mod.MODUI, "OpenLegacyUI", OpenLegacyUI));
            AddBinding(new TriggerBinding(Mod.MODUI, "ImportLuminaPreset", PresetManagement.ExecuteImport));
            AddBinding(new TriggerBinding(Mod.MODUI, "ExportLuminaPreset", PresetManagement.ExportLuminaPreset));
            AddBinding(new TriggerBinding<string>(Mod.MODUI, "UpdatePresetName", PresetManagement.UpdatePresetName));

            AddBinding(new TriggerBinding(Mod.MODUI, "OpenPresetFolder", PresetManagement.OpenPresetFolder));

            //Tonemapping
            AddBinding(new TriggerBinding<string>(Mod.MODUI, "UpdateLUTName", UpdateLUTName));
            AddUpdateBinding(new GetterValueBinding<string>(Mod.MODUI, "TonemappingMode", GetTonemappingMode));
            AddBinding(new TriggerBinding<float>(Mod.MODUI, "SetTonemappingMode", SetTonemappingMode));


            AddUpdateBinding(new GetterValueBinding<string>(Mod.MODUI, "LUTName", GetLUTName));

            AddUpdateBinding(new GetterValueBinding<float>(Mod.MODUI, "LUTValue", () => LUTValue()));
            AddBinding(new TriggerBinding<float>(Mod.MODUI, "HandleLUTContribution", HandleLUTContribution));
            AddBinding(new TriggerBinding(Mod.MODUI, "OpenLUTFolder", OpenLUTFolder));
            AddBinding(new TriggerBinding(Mod.MODUI, "UpdateLUT", UpdateLUT));

            // Texture Format
            AddUpdateBinding(new GetterValueBinding<string>(Mod.MODUI, "TextureFormat", GetTextureFormatMode));





            //Time of day
            AddUpdateBinding(new GetterValueBinding<bool>(Mod.MODUI, "TimeFloatIsActive", () => TimeFloatIsActive()));
            AddUpdateBinding(new GetterValueBinding<float>(Mod.MODUI, "TimeFloatValue", () => TimeFloatValue()));
            AddBinding(new TriggerBinding<float>(Mod.MODUI, "HandleTimeFloatValue", HandleTimeFloatValue));
            // Array
            LutArrayExtended = CreateBinding("LUTArray", LUTArray());
            CubemapArrayExtended = CreateBinding("CubemapArrayExtended", CubemapArrayExtendedReturn());

            //Cubemaps
            AddUpdateBinding(new GetterValueBinding<string>(Mod.MODUI, "CubemapName", ReturnCubemapName));

            //TonemappingExternalMode
            AddUpdateBinding(new GetterValueBinding<bool>(Mod.MODUI, "IsExternal", () => IsExternalMode()));

            //TonemappingCustomMode
            AddUpdateBinding(new GetterValueBinding<bool>(Mod.MODUI, "IsCustom", () => IsCustomMode()));


            AddUpdateBinding(new GetterValueBinding<bool>(Mod.MODUI, "IsToeStrengthActive", () => IsToeStrengthActive()));
            AddUpdateBinding(new GetterValueBinding<float>(Mod.MODUI, "ToeStrengthValue", () => GetToeStrengthValue()));
            AddBinding(new TriggerBinding(Mod.MODUI, "SetToeStrengthActive", SetToeStrengthActive));
            AddBinding(new TriggerBinding<float>(Mod.MODUI, "HandleToeStrengthActive", HandleToeStrengthActive));

            AddUpdateBinding(new GetterValueBinding<bool>(Mod.MODUI, "IsToeLengthActive", () => IsToeLengthActive()));
            AddUpdateBinding(new GetterValueBinding<float>(Mod.MODUI, "ToeLengthValue", () => ToeLengthValue()));
            AddBinding(new TriggerBinding(Mod.MODUI, "SetToeLengthActive", SetToeLengthActive));
            AddBinding(new TriggerBinding<float>(Mod.MODUI, "HandleToeLengthActive", HandleToeLengthActive));

            AddUpdateBinding(new GetterValueBinding<bool>(Mod.MODUI, "IsShoulderStrengthActive", () => IsShoulderStrengthActive()));
            AddBinding(new TriggerBinding(Mod.MODUI, "SetShoulderStrengthActive", SetShoulderStrengthActive));
            AddUpdateBinding(new GetterValueBinding<float>(Mod.MODUI, "ShoulderStrengthValue", () => ShoulderStrengthValue()));
            AddBinding(new TriggerBinding<float>(Mod.MODUI, "handleShoulderStrength", handleShoulderStrength));

            AddUpdateBinding(new GetterValueBinding<bool>(Mod.MODUI, "IsHDRISkyEnabled", () => IsHDRISkyEnabled()));
            AddBinding(new TriggerBinding(Mod.MODUI, "SetHDRISkyEnabled", SetHDRISkyEnabled));

            AddBinding(new TriggerBinding<string>(Mod.MODUI, "UpdateCubemapName", UpdateCubemapName));


            AddBinding(new TriggerBinding(Mod.MODUI, "SaveAutomatically", SaveAutomatically));

            AddBinding(new TriggerBinding<float>(Mod.MODUI, "handleEmissionMultiplier", handleEmissionMultiplier));
            AddUpdateBinding(new GetterValueBinding<float>(Mod.MODUI, "EmissionMultiplier", () => GetEmissionMultiplier()));







        }

        private float GetEmissionMultiplier()
        {
            return GlobalVariables.Instance.spaceEmissionMultiplier;
        }

        private void handleEmissionMultiplier(float obj)
        {
            RenderEffectsSystem.handleEmissionMultiplier(obj);
        }

        private void UpdateCubemapName(string obj)
        {
            // Update GlobalVariables.Instance.CubemapName with the provided name
            GlobalVariables.Instance.CubemapName = obj;
            CubemapLoader.IncomingCubemap = obj;

            // Ensure the local CubemapName variable is also updated
            CubemapName = GlobalVariables.Instance.CubemapName;

            // Once the names are updated, call the method to update the cubemap
            RenderEffectsSystem.UpdateCubemap();
        }

        private void SetHDRISkyEnabled()
        {
            GlobalVariables.Instance.HDRISkyEnabled = !GlobalVariables.Instance.HDRISkyEnabled;
            UsingHDRSky = GlobalVariables.Instance.HDRISkyEnabled;
            RenderEffectsSystem.DisableCubemap();
        }

        private bool IsHDRISkyEnabled()
        {
            // Check if HDRI Sky is enabled in GlobalVariables
            if (GlobalVariables.Instance.HDRISkyEnabled == false)
            {
                UsingHDRSky = false;
            }
            else
            {
                UsingHDRSky = true;
            }

            // Return the value of UsingHDRSky
            return UsingHDRSky;
        }


        private string ReturnCubemapName()
        {
            // Update the GlobalVariables.Instance.CubemapName with the current CubemapName
            GlobalVariables.Instance.CubemapName = CubemapName;

            // Return the current CubemapName
            return CubemapName;
        }


        private string[] CubemapArrayExtendedReturn()
        {
            // Retrieve the LUT files array
            var lutFiles = RenderEffectsSystem.CubemapFiles;

            // Check if lutFiles is null or empty and update it with the directory files if necessary
            if (lutFiles == null || lutFiles.Length == 0)
            {
                Lumina.Mod.Log.Info("CubemapArrayExtendedReturn() is null or empty. Populating with files from the directory.");

                // Ensure the LUT directory exists
                if (!Directory.Exists(GlobalPaths.LuminaHDRIDirectory))
                {
                    Lumina.Mod.Log.Warn($"Cubemaps directory not found: {GlobalPaths.LuminaHDRIDirectory}. Creating directory...");
                    Directory.CreateDirectory(GlobalPaths.LuminaHDRIDirectory);
                }

                // Populate RenderEffectsSystem.LutFiles with files from the specified directory
                var filesWithFullPath = Directory.GetFiles(GlobalPaths.LuminaHDRIDirectory, "*.png");

                // Extract only the file names without the extension
                var fileNames = filesWithFullPath
                    .Select(filePath => Path.GetFileNameWithoutExtension(filePath))
                    .ToArray();

                // Update RenderEffectsSystem.LutFiles with only the file names
                RenderEffectsSystem.CubemapFiles = fileNames;

                Lumina.Mod.Log.Info(string.Join(", ", RenderEffectsSystem.CubemapFiles)); // Log the result for debugging
            }

            // Return the array
            return RenderEffectsSystem.CubemapFiles;
        }

        private void SaveAutomatically()
        {
            // Check if the save automatically flag is true
            if (GlobalVariables.Instance.SaveAutomatically)
            {
                // Save global variables to file
                try
                {
                    GlobalVariables.SaveToFile(GlobalPaths.GlobalModSavingPath);
                    Mod.Log.Info($"Global variables saved to file: {GlobalPaths.GlobalModSavingPath}");
                }
                catch (Exception ex)
                {
                    Mod.Log.Error($"Error saving global variables to file: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }


        private void handleShoulderStrength(float obj)
        {
            TonemappingCustomBindings.handleShoulderStrength(obj);
        }

        private float ShoulderStrengthValue()
        {
            return GlobalVariables.Instance.shoulderStrengthValue;
        }

        private void SetShoulderStrengthActive()
        {
            TonemappingCustomBindings.SetShoulderStrengthActive();
        }

        private bool IsShoulderStrengthActive()
        {
            return GlobalVariables.Instance.shoulderStrengthActive;
        }

        private void HandleToeLengthActive(float obj)
        {
            TonemappingCustomBindings.HandleToeLengthActive(obj);
        }

        private void SetToeLengthActive()
        {
            TonemappingCustomBindings.SetToeLengthActive();
        }

        private float ToeLengthValue()
        {
            return GlobalVariables.Instance.ToeLengthValue;
        }

        private bool IsToeLengthActive()
        {
            return GlobalVariables.Instance.ToeLengthActive;
        }

        private float GetToeStrengthValue()
        {
            return GlobalVariables.Instance.ToeStrengthValue;
        }

        private void HandleToeStrengthActive(float obj)
        {
            TonemappingCustomBindings.HandleToeStrengthActive(obj);
        }

        private void SetToeStrengthActive()
        {
            TonemappingCustomBindings.SetToeStrength();
        }

        private bool IsToeStrengthActive()
        {
            return TonemappingCustomBindings.IsToeStrengthActive;
        }

        private bool IsCustomMode()
        {
            return RenderEffectsSystem.IsCustomMode;
        }

        private bool IsExternalMode()
        {
            return RenderEffectsSystem.IsExternalMode;
        }


        private void PopulateLUTSArray()
        {
            try
            {
                AddBinding(new GetterValueBinding<string[]>(
                    Mod.MODUI,
                    "LUTArray",
                    () => LUTArray()
                ));
            }
            catch (Exception ex)
            {
                Lumina.Mod.Log.Info("[FAILURE] Failed to add or update binding for LUTArray: " + ex.Message);
            }

        }

        private void SendLUTName(string obj)
        {
            try
            {
                // Log the incoming value for debugging
                Lumina.Mod.Log.Info($"SendLUTName() called with value: {obj}");

                // Assign the value to RenderEffectsSystem.LutName_Example
                RenderEffectsSystem.LutName_Example = obj;

                // Log the successful update
                Lumina.Mod.Log.Info($"LutName_Example successfully updated to: {RenderEffectsSystem.LutName_Example}");
            }
            catch (Exception ex)
            {
                // Log the exception details for debugging
                Lumina.Mod.Log.Error($"An error occurred in SendLUTName(): {ex.Message}");
                Lumina.Mod.Log.Error($"Stack Trace: {ex.StackTrace}");
            }
        }

        public static void UpdateArray()
        {
            var lutFiles = RenderEffectsSystem.LutFiles;

            // Check if lutFiles is null and update it with the directory files if necessary
            if (lutFiles == null)
            {
                Lumina.Mod.Log.Info("LUTArray() returned null from RenderEffectsSystem.LutFiles. Populating with files from the directory.");

                // Populate RenderEffectsSystem.LutFiles with files from the specified directory
                var filesWithFullPath = Directory.GetFiles(GlobalPaths.LuminaLUTSDirectory, "*.cube");

                // Extract only the file names without the extension
                var fileNames = filesWithFullPath
                    .Select(filePath => Path.GetFileNameWithoutExtension(filePath))
                    .ToArray();

                // Update RenderEffectsSystem.LutFiles with only the file names
                RenderEffectsSystem.LutFiles = fileNames;

                Lumina.Mod.Log.Info(string.Join(", ", RenderEffectsSystem.LutFiles)); // Log the result for debugging
            }

            // Optionally, check if the array is empty and handle it if needed
            if (RenderEffectsSystem.LutFiles.Length == 0)
            {
                Lumina.Mod.Log.Info("LUTArray() returned an empty array from RenderEffectsSystem.LutFiles.");
            }
        }

        private string[] LUTArray()
        {

            // Retrieve the LUT files array
            var lutFiles = RenderEffectsSystem.LutFiles;

            // Check if lutFiles is null or empty and update it with the directory files if necessary
            if (lutFiles == null || lutFiles.Length == 0)
            {
                Lumina.Mod.Log.Info("LUTArray() is null or empty. Populating with files from the directory.");

                // Ensure the LUT directory exists
                if (!Directory.Exists(GlobalPaths.LuminaLUTSDirectory))
                {
                    Lumina.Mod.Log.Warn($"LUT directory not found: {GlobalPaths.LuminaLUTSDirectory}. Creating directory...");
                    Directory.CreateDirectory(GlobalPaths.LuminaLUTSDirectory);
                }

                // Populate RenderEffectsSystem.LutFiles with files from the specified directory
                var filesWithFullPath = Directory.GetFiles(GlobalPaths.LuminaLUTSDirectory, "*.cube");

                // Extract only the file names without the extension
                var fileNames = filesWithFullPath
                    .Select(filePath => Path.GetFileNameWithoutExtension(filePath))
                    .ToArray();

                // Update RenderEffectsSystem.LutFiles with only the file names
                RenderEffectsSystem.LutFiles = fileNames;

                Lumina.Mod.Log.Info(string.Join(", ", RenderEffectsSystem.LutFiles)); // Log the result for debugging
            }

            // Return the array
            return RenderEffectsSystem.LutFiles;
        }




        private string GetLUTName()
        {
            
            return RenderEffectsSystem.LutName_Example;
        }

        private void InitializeLutName()
        {
            RenderEffectsSystem.LutName_Example = GlobalVariables.Instance.LUTName;
            GlobalVariables.Instance.CubemapName = "";
        }

        private void SetTonemappingMode(float obj)
        {
          RenderEffectsSystem.SetTonemappingMode(obj);
        }

        private string GetTonemappingMode()
        {
            return RenderEffectsSystem.ToneMappingMode;
        }

        private string GetTextureFormatMode()
        {
            return GraphicsFormat.R16G16B16A16_SFloat.ToString();
        }



        private bool TimeFloatIsActive()
        {
            return TimeOfDayProcessor.Locked;
        }

        private void HandleTimeFloatValue(float obj)
        {
            TimeOfDayProcessor.TimeFloat = obj;
        }

        private float TimeFloatValue()
        {
            return TimeOfDayProcessor.TimeFloat;
        }

        private void OpenLUTFolder()
        {
            string luminaLUTSDirectory = GlobalPaths.LuminaLUTSDirectory;
            Process.Start(luminaLUTSDirectory);
        }

        private void UpdateLUTName(string obj)
        {
            // Log the incoming value
            Lumina.Mod.Log.Info($"[DEBUG] UpdateLUTName called with obj: {obj}");

            // Check if obj is null or empty
            if (string.IsNullOrEmpty(obj))
            {
                Lumina.Mod.Log.Info("[DEBUG] UpdateLUTName received an empty or null value.");
            }
            else
            {
                // Log the values before assignment
                Lumina.Mod.Log.Info($"[DEBUG] Setting RenderEffectsSystem.LutName_Example to: {obj}");
                Lumina.Mod.Log.Info($"[DEBUG] Setting GlobalVariables.Instance.LUTName to: {obj}");
            }

            try
            {
                // Assign the values
                RenderEffectsSystem.LutName_Example = obj;
                GlobalVariables.Instance.LUTName = obj;

                // Confirm successful assignment
                Lumina.Mod.Log.Info($"[DEBUG] Successfully updated LUT names: RenderEffectsSystem.LutName_Example = {RenderEffectsSystem.LutName_Example}, GlobalVariables.Instance.LUTName = {GlobalVariables.Instance.LUTName}");
            }
            catch (Exception ex)
            {
                // Log any exceptions
                Lumina.Mod.Log.Info($"[ERROR] Exception occurred in UpdateLUTName: {ex.Message}\n{ex.StackTrace}");
            }
        }
        private void UpdateLUT()
        {
            RenderEffectsSystem.UpdateLUT();
        }

        private void HandleLUTContribution(float obj)
        {
            GlobalVariables.Instance.LUTContribution = obj;
        }

        private float LUTValue()
        {
            return GlobalVariables.Instance.LUTContribution;
        }

        private void PlanetarySettings()
        {
            AddBinding(new TriggerBinding<float>(Mod.MODUI, "SetLatitude", PlanetarySettingsMerger.SetLatitude));
            AddUpdateBinding(new GetterValueBinding<float>(Mod.MODUI, "LatitudeValue", () => PlanetarySettingsMerger.LatitudeValue()));

            AddBinding(new TriggerBinding<float>(Mod.MODUI, "SetLongitude", PlanetarySettingsMerger.SetLongitude));
            AddUpdateBinding(new GetterValueBinding<float>(Mod.MODUI, "LongitudeValue", () => PlanetarySettingsMerger.LongitudeValue()));
        }

        private void ShadowsMidtonesHighlightsCheckboxes()
        {
            AddBinding(new TriggerBinding(Mod.MODUI, "SetShadowsCheckbox", SetShadowsCheckbox));
            AddUpdateBinding(new GetterValueBinding<bool>(Mod.MODUI, "GetShadowsCheckbox", () => GetShadowsCheckbox()));

            AddBinding(new TriggerBinding(Mod.MODUI, "SetMidtonesCheckbox", SetMidtonesCheckbox));
            AddUpdateBinding(new GetterValueBinding<bool>(Mod.MODUI, "GetMidtonesCheckbox", () => GetMidtonesCheckbox()));

            AddBinding(new TriggerBinding(Mod.MODUI, "SetHighlightsCheckbox", SetHighlightsCheckbox));
            AddUpdateBinding(new GetterValueBinding<bool>(Mod.MODUI, "GetHighlightsCheckbox", () => GetHighlightsCheckbox()));




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

            AddUpdateBinding(new GetterValueBinding<float>(Mod.MODUI, "GetShadows", () => GetShadows()));
            AddUpdateBinding(new GetterValueBinding<float>(Mod.MODUI, "GetMidtones", () => GetMidtones()));
            AddUpdateBinding(new GetterValueBinding<float>(Mod.MODUI, "GetHighlights", () => GetHighlights()));

            AddBinding(new TriggerBinding<float>(Mod.MODUI, "SetShadows", SetShadows));
            AddBinding(new TriggerBinding<float>(Mod.MODUI, "SetMidtones", SetMidtones));
            AddBinding(new TriggerBinding<float>(Mod.MODUI, "SetHighlights", SetHighlights));
        }

        private void WhiteBalance()
        {
            AddUpdateBinding(new GetterValueBinding<float>(Mod.MODUI, "GetTemperature", () => GetTemperature()));
            AddUpdateBinding(new GetterValueBinding<float>(Mod.MODUI, "GetTint", () => GetTint()));

            AddBinding(new TriggerBinding<float>(Mod.MODUI, "SetTemperature", SetTemperature));
            AddBinding(new TriggerBinding<float>(Mod.MODUI, "SetTint", SetTint));
        }

        private void WhiteBalanceCheckboxes()
        {
            AddBinding(new TriggerBinding(Mod.MODUI, "SetTempCheckbox", SetTempCheckbox));
            AddUpdateBinding(new GetterValueBinding<bool>(Mod.MODUI, "GetTempCheckbox", () => GetTempCheckbox()));

            AddBinding(new TriggerBinding(Mod.MODUI, "SetTintCheckbox", SetTintCheckbox));
            AddUpdateBinding(new GetterValueBinding<bool>(Mod.MODUI, "GetTintCheckbox", () => GetTintCheckbox()));


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
            AddBinding(new TriggerBinding(Mod.MODUI, "MIT_EnableToggle", SliderPanel.Toggle));
            AddBinding(new TriggerBinding(Mod.MODUI, "LUM_SendToLumina", SliderPanel.SendToLumina));

            AddBinding(new TriggerBinding<float>(Mod.MODUI, "SetPostExposure", SetPostExposure));
            AddUpdateBinding(new GetterValueBinding<float>(Mod.MODUI, "PostExposure", () => GetSetPostExposure()));

            AddBinding(new TriggerBinding<float>(Mod.MODUI, "SetContrast", SetContrast));
            AddUpdateBinding(new GetterValueBinding<float>(Mod.MODUI, "GetContrast", () => GetContrast()));

            AddUpdateBinding(new GetterValueBinding<float>(Mod.MODUI, "GetHueShift", () => GetHueShift()));
            AddBinding(new TriggerBinding<float>(Mod.MODUI, "SetHueShift", SetHueShift));

            AddUpdateBinding(new GetterValueBinding<float>(Mod.MODUI, "GetSaturation", () => GetSaturation()));
            AddBinding(new TriggerBinding<float>(Mod.MODUI, "SetSaturation", SetSaturation));

            AddBinding(new TriggerBinding(Mod.MODUI, "Save", SaveToFile));
            AddBinding(new TriggerBinding(Mod.MODUI, "ResetLuminaSettings", Reset));
        }

        private void Checkboxes()
        {
            AddBinding(new TriggerBinding(Mod.MODUI, "SetPostExposureCheckbox", SetPostExposureCheckbox));
            AddUpdateBinding(new GetterValueBinding<bool>(Mod.MODUI, "GetPostExposureCheckbox", () => GetPostExposureCheckbox()));


            AddBinding(new TriggerBinding(Mod.MODUI, "SetcontrastCheckbox", SetcontrastCheckbox));
            AddUpdateBinding(new GetterValueBinding<bool>(Mod.MODUI, "GetcontrastCheckbox", () => GetcontrastCheckbox()));

            AddBinding(new TriggerBinding(Mod.MODUI, "SethueshiftCheckbox", SethueshiftCheckbox));
            AddUpdateBinding(new GetterValueBinding<bool>(Mod.MODUI, "GethueshiftCheckbox", () => GethueshiftCheckbox()));

            AddBinding(new TriggerBinding(Mod.MODUI, "SetsaturationCheckbox", SetsaturationCheckbox));
            AddUpdateBinding(new GetterValueBinding<bool>(Mod.MODUI, "GetsaturationCheckbox", () => GetsaturationCheckbox()));
        }

        private bool GetsaturationCheckbox()
        {
            return GlobalVariables.Instance.SaturationActive;
        }

        private void SetsaturationCheckbox()
        {
            GlobalVariables.Instance.SaturationActive = !GlobalVariables.Instance.SaturationActive;
        }

        private bool GethueshiftCheckbox()
        {
            return GlobalVariables.Instance.HueShiftActive;
        }

        private void SethueshiftCheckbox()
        {
           GlobalVariables.Instance.HueShiftActive = !GlobalVariables.Instance.HueShiftActive;
        }

        private bool GetcontrastCheckbox()
        {
            return GlobalVariables.Instance.ContrastActive;
        }

        private void SetcontrastCheckbox()
        {
            GlobalVariables.Instance.ContrastActive = !GlobalVariables.Instance.ContrastActive;
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
            GlobalVariables.Instance.HueShift = obj; 
        }

        private float GetHueShift()
        {
            return GlobalVariables.Instance.HueShift;
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
