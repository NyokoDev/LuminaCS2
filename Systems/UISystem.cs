namespace Lumina.Systems
{
    using Colossal.UI.Binding;
    using Game;
    using Game.Simulation;
    using Game.UI;
    using Game.UI.InGame;
    using Lumina.Systems.Presets;
    using Lumina.Systems.Tonemapping_Custom;
    using Lumina.UI;
    using Lumina.XML;
    using LuminaMod.XML;
    using MetroFramework.Controls;
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.Experimental.Rendering;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.HighDefinition;
    using static UnityEngine.Rendering.DebugUI;

    internal partial class UISystem : ExtendedUISystemBase
    {

        public PlanetarySystem m_PlanetarySystem;
        public bool Visible { get; set; }
        public string CubemapName { get; set;}
        public bool UsingHDRSky = GlobalVariables.Instance.HDRISkyEnabled;

        private ValueBindingHelper<string[]> LutArrayExtended;
        private ValueBindingHelper<string[]> CubemapArrayExtended;
        private ValueBindingHelper<string> LutName;
        private bool customSunEnabled { get; set; }
        private float SunDiameterValue { get; set; }
        public float SunIntensityValue { get; set; }
        public float SunFlareSizeValue { get; set; }

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            base.OnCreate();
            InitializeLutName();
            CreateBindings();
            m_PlanetarySystem = World.GetExistingSystemManaged<PlanetarySystem>();
        }

        /// <summary>
        /// Creates bindings for the mod.
        /// </summary>
        private void CreateBindings()
        {
            UseLuminaVolume();
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

            AddUpdateBinding(new GetterValueBinding<float>(Mod.MODUI, "GetLutContributionValue", () => GetLutContributionValue()));
            AddBinding(new TriggerBinding<float>(Mod.MODUI, "HandleLUTContribution", HandleLUTContribution));
            AddBinding(new TriggerBinding(Mod.MODUI, "OpenLUTFolder", OpenLUTFolder));
            AddBinding(new TriggerBinding(Mod.MODUI, "UpdateLUT", UpdateLUT));

            // Texture Format
            AddUpdateBinding(new GetterValueBinding<string>(Mod.MODUI, "TextureFormat", GetTextureFormatMode));



            AddBinding(new TriggerBinding(Mod.MODUI, "Save", SaveToFile));
            AddBinding(new TriggerBinding(Mod.MODUI, "ResetLuminaSettings", Reset));


            //Time of day
            AddUpdateBinding(new GetterValueBinding<bool>(Mod.MODUI, "TimeFloatIsActive", () => TimeFloatIsActive()));
            AddUpdateBinding(new GetterValueBinding<float>(Mod.MODUI, "TimeFloatValue", () => TimeFloatValue()));
            AddBinding(new TriggerBinding<float>(Mod.MODUI, "HandleTimeFloatValue", HandleTimeFloatValue));
            // Array
            LutArrayExtended = CreateBinding("LUTArray", LUTArray());
            CubemapArrayExtended = CreateBinding("CubemapArrayExtended", CubemapArrayExtendedReturn());

            //Cubemaps
            AddUpdateBinding(new GetterValueBinding<string>(Mod.MODUI, "CubemapName", ReturnCubemapName));

            // Time Lock Status
           
             AddUpdateBinding(new GetterValueBinding<bool>(Mod.MODUI, "TimeLockStatus", () => TimeLockStatus()));

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

            AddUpdateBinding(new GetterValueBinding<bool>(Mod.MODUI, "IsCustomSunEnabled", () => IsCustomSunEnabled()));
            AddBinding(new TriggerBinding(Mod.MODUI, "SetCustomSunEnabled", SetCustomSunEnabled));


            AddUpdateBinding(new GetterValueBinding<float>(Mod.MODUI, "SunDiameter", () => SunDiameter()));
            AddBinding(new TriggerBinding<float>(Mod.MODUI, "handleAngularDiameter", handleAngularDiameter));

            AddUpdateBinding(new GetterValueBinding<float>(Mod.MODUI, "SunIntensity", () => SunIntensity()));
            AddBinding(new TriggerBinding<float>(Mod.MODUI, "handleSunIntensity", handleSunIntensity));

            AddUpdateBinding(new GetterValueBinding<float>(Mod.MODUI, "SunFlareSize", () => SunFlareSize()));
            AddBinding(new TriggerBinding<float>(Mod.MODUI, "handleSunFlareSize", handleSunFlareSize));


            // UI Update
            AddBinding(new TriggerBinding(Mod.MODUI, "UpdateUIElements", UpdateUIElements));

            AddBinding(new TriggerBinding(Mod.MODUI, "UploadLUTFileDialog", OpenLUTFileDialog));

            AddBinding(new TriggerBinding(Mod.MODUI, "LockTime", HandleTimeLocked));


        }

        /// <summary>
        /// Returns the current time lock status.
        /// </summary>
        /// <returns></returns>
        private bool TimeLockStatus()
        {
            return GlobalVariables.Instance.TimeOfDayLocked;
        }

        private void UseLuminaVolume()
        {
            AddUpdateBinding(new GetterValueBinding<bool>(Mod.MODUI, "UseLuminaVolume", () => CheckIfLumina()));
            AddBinding(new TriggerBinding(Mod.MODUI, "RestartLuminaVolume", RestartLuminaVolume));
        }

        private bool CheckIfLumina()
        {
            return RenderEffectsSystem.LuminaVolume.enabled;
        }
        private void RestartLuminaVolume()
        {
            if (RenderEffectsSystem.LuminaVolume != null)
            {
                // Toggle the enabled state and the global variable together
                bool newState = !RenderEffectsSystem.LuminaVolume.enabled;
                RenderEffectsSystem.LuminaVolume.enabled = newState;
                GlobalVariables.Instance.LuminaVolumeEnabled = newState;
            }
            else
            {
                Mod.Log.Error("[LUMINA] LuminaVolume is null. Unable to restart LuminaVolume.");
            }
        }


        public void OpenLUTFileDialog()
        {
            string filePath = ModernFileDialog.ShowDialog("Select a .cube LUT File", "LUT Cube Files", "*.cube");
            if (!string.IsNullOrEmpty(filePath))
            {
                Lumina.Mod.Log.Info($"Selected LUT path: {filePath}");

                try
                {
                    var lutTexture = CubeLutLoader.LoadLutFromFile(filePath);
                    if (lutTexture != null)
                    {
                        string lutName = System.IO.Path.GetFileNameWithoutExtension(filePath);
                        RenderEffectsSystem.LutName_Example = lutName;
                        GlobalVariables.Instance.LUTName = lutName;
                        RenderEffectsSystem.m_Tonemapping.lutTexture.value = lutTexture; // Set directly (we already created the texture)

                        Lumina.Mod.Log.Info("LUT successfully loaded and applied.");
                    }
                    else
                    {
                        Lumina.Mod.Log.Info("Failed to load LUT texture.");
                    }
                }
                catch (Exception ex)
                {
                    Lumina.Mod.Log.Info($"Exception during LUT loading: {ex.Message}");
                }
            }
            else
            {
                Lumina.Mod.Log.Info("User cancelled file selection or closed dialog.");
            }
        }



        private float GetLutContributionValue()
        {
            float lutContribution = GlobalVariables.Instance.LUTContribution;

            // Check if the value is a valid number (not NaN) and return it
            if (!float.IsNaN(lutContribution))
            {
                return lutContribution;
            }

            // Return a default value if it's not a valid number
            return 1f;
        }



        private void UpdateUIElements()
        {
            CubemapArrayExtendedReturn();
            LUTArray();
        }

        private void handleSunFlareSize(float obj)
        {
            SunFlareSizeValue = obj;
            GlobalVariables.Instance.SunFlareSize = SunFlareSizeValue;
        }


            private float SunFlareSize()
        {
            SunFlareSizeValue = GlobalVariables.Instance.SunFlareSize;
            return SunFlareSizeValue;
        }

        private void handleSunIntensity(float obj)
        {
            SunIntensityValue = obj;
            GlobalVariables.Instance.SunIntensity = SunIntensityValue;
        }

        private float SunIntensity()
        {
                SunIntensityValue = GlobalVariables.Instance.SunIntensity;
                return SunIntensityValue;
        }

        private void handleAngularDiameter(float obj)
        {
            SunDiameterValue = obj;
            GlobalVariables.Instance.AngularDiameter = SunDiameterValue;
        }

        private float SunDiameter()
        {
            SunDiameterValue = GlobalVariables.Instance.AngularDiameter;
            return SunDiameterValue;
        }


        private void SetCustomSunEnabled()
        {
            GlobalVariables.Instance.CustomSunEnabled = !GlobalVariables.Instance.CustomSunEnabled;
            customSunEnabled = GlobalVariables.Instance.CustomSunEnabled;
            Mod.Log.Info("Set CustomSun to " + customSunEnabled.ToString());
        }

        private bool IsCustomSunEnabled()
        {
            // Check if CustomSunEnabled is enabled in GlobalVariables
            if (GlobalVariables.Instance.CustomSunEnabled == false)
            {
                customSunEnabled = false;
            }
            else
            {
                customSunEnabled = true;
            }

            // Return the value of CustomSunEnabled
            return customSunEnabled;
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

        /// <summary>
        /// Toggles the state of the HDRI Sky and applies or disables the cubemap accordingly.
        /// </summary>
        private void SetHDRISkyEnabled()
        {
            // Toggle the HDRI Sky state
            bool isEnabled = !GlobalVariables.Instance.HDRISkyEnabled;
            GlobalVariables.Instance.HDRISkyEnabled = isEnabled;
            UsingHDRSky = isEnabled;

            // Update the render effects based on the new state
            if (isEnabled)
            {
                RenderEffectsSystem.ApplyCubemap();
            }
            else
            {
                RenderEffectsSystem.DisableCubemap();
            }
        }



        private bool IsHDRISkyEnabled()
        {
            // Return the value of UsingHDRSky
            return GlobalVariables.Instance.HDRISkyEnabled;
        }

        private string ReturnCubemapName()
        {
            // Ensure CubemapName is set, use "Select Cubemap" if not
            CubemapName = GlobalVariables.Instance.CubemapName;

            // Always return the current CubemapName
            return CubemapName;
        }



        public static string[] CubemapArrayExtendedReturn()
        {
            try
            {
                // Ensure CubemapFiles is not null before updating
                RenderEffectsSystem.CubemapFiles = UIDropdownUpdate.UpdateCubemapDropdown() ?? Array.Empty<string>();

                // Check if CubemapFiles is still null or empty after the update
                if (RenderEffectsSystem.CubemapFiles == null || RenderEffectsSystem.CubemapFiles.Length == 0)
                {
                    // Log a warning if no cubemap files are found
                    Mod.Log.Info("No cubemap files found.");
                    return new string[] { "None" }; // Return "None" instead of an empty array
                }

                // Return the updated array
                return RenderEffectsSystem.CubemapFiles;
            }
            catch (Exception ex)
            {
                // Log the error
                Mod.Log.Error($"Error in CubemapArrayExtendedReturn: {ex.Message}");
                return new string[] { "None" }; // Return "None" as a fallback
            }
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
                 Mod.Log.Info("[FAILURE] Failed to add or update binding for LUTArray: " + ex.Message);
            }

        }

        private void SendLUTName(string obj)
        {
            try
            {
                // Log the incoming value for debugging
                 Mod.Log.Info($"SendLUTName() called with value: {obj}");

                // Assign the value to RenderEffectsSystem.LutName_Example
                RenderEffectsSystem.LutName_Example = obj;

                // Log the successful update
                 Mod.Log.Info($"LutName_Example successfully updated to: {RenderEffectsSystem.LutName_Example}");
            }
            catch (Exception ex)
            {
                // Log the exception details for debugging
                Mod.Log.Error($"An error occurred in SendLUTName(): {ex.Message}");
                Mod.Log.Error($"Stack Trace: {ex.StackTrace}");
            }
        }

        public static void UpdateArray()
        {
            var lutFiles = RenderEffectsSystem.LutFiles;

            // Check if lutFiles is null and update it with the directory files if necessary
            if (lutFiles == null)
            {
                 Mod.Log.Info("LUTArray() returned null from RenderEffectsSystem.LutFiles. Populating with files from the directory.");

                // Populate RenderEffectsSystem.LutFiles with files from the specified directory
                var filesWithFullPath = Directory.GetFiles(GlobalPaths.LuminaLUTSDirectory, "*.cube");

                // Extract only the file names without the extension
                var fileNames = filesWithFullPath
                    .Select(filePath => Path.GetFileNameWithoutExtension(filePath))
                    .ToArray();

                // Update RenderEffectsSystem.LutFiles with only the file names
                RenderEffectsSystem.LutFiles = fileNames;

                 Mod.Log.Info(string.Join(", ", RenderEffectsSystem.LutFiles)); // Log the result for debugging
            }

            // Optionally, check if the array is empty and handle it if needed
            if (RenderEffectsSystem.LutFiles.Length == 0)
            {
                 Mod.Log.Info("LUTArray() returned an empty array from RenderEffectsSystem.LutFiles.");
            }
        }

        private string[] LUTArray()
        {
            // Retrieve the LUT files array
            var lutFiles = RenderEffectsSystem.LutFiles;

            // Check if lutFiles is null or empty and update it with the directory files if necessary
            if (lutFiles == null || lutFiles.Length == 0)
            {
                Mod.Log.Info("LUTArray() is null or empty. Populating with files from the directory.");

                // Ensure the LUT directory exists
                if (!Directory.Exists(GlobalPaths.LuminaLUTSDirectory))
                {
                    Mod.Log.Info($"LUT directory not found: {GlobalPaths.LuminaLUTSDirectory}. Creating directory...");
                    Directory.CreateDirectory(GlobalPaths.LuminaLUTSDirectory);
                }

                // Populate RenderEffectsSystem.LutFiles with files from the specified directory
                var filesWithFullPath = Directory.GetFiles(GlobalPaths.LuminaLUTSDirectory, "*.cube");

                // Extract only the file names without the extension
                var fileNames = filesWithFullPath
                    .Select(filePath => Path.GetFileNameWithoutExtension(filePath))
                    .ToArray();

                // Check if RenderEffectsSystem.LutFiles is not null before updating it
                if (RenderEffectsSystem.LutFiles != null)
                {
                    RenderEffectsSystem.LutFiles = null;
                }

                // Update RenderEffectsSystem.LutFiles with only the file names
                RenderEffectsSystem.LutFiles = fileNames;

                Mod.Log.Info(string.Join(", ", RenderEffectsSystem.LutFiles)); // Log the result for debugging
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
            return GlobalVariables.Instance.ViewTimeOfDaySlider;
        }

        private bool IsTimeLocked()
        {
                       return TimeOfDayProcessor.Locked;
        }

        private void HandleTimeLocked()
        {
            TimeOfDayProcessor.Locked = !TimeOfDayProcessor.Locked;
            GlobalVariables.Instance.TimeOfDayLocked = TimeOfDayProcessor.Locked;
            Mod.Log.Info($"Time of Day locked state changed to: {TimeOfDayProcessor.Locked}");
        }

        private void HandleTimeFloatValue(float obj)
        {
            TimeOfDayProcessor.TimeFloat = obj;
            TimeOfDayProcessor.ChangingTime = true;
            WaitForChangingTime();
        }

        private async Task WaitForChangingTime()
        {
            while (Math.Abs(m_PlanetarySystem.time - TimeOfDayProcessor.TimeFloat) > 0.0001f)
            {
                await Task.Delay(16); // Wait one frame (~60fps)
            }

            TimeOfDayProcessor.ChangingTime = false;
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
             Mod.Log.Info($"[DEBUG] UpdateLUTName called with obj: {obj}");

            // Check if obj is null or empty
            if (string.IsNullOrEmpty(obj))
            {
                 Mod.Log.Info("[DEBUG] UpdateLUTName received an empty or null value.");
            }
            else
            {
                // Log the values before assignment
                 Mod.Log.Info($"[DEBUG] Setting RenderEffectsSystem.LutName_Example to: {obj}");
                 Mod.Log.Info($"[DEBUG] Setting GlobalVariables.Instance.LUTName to: {obj}");
            }

            try
            {
                // Assign the values
                RenderEffectsSystem.LutName_Example = obj;
                GlobalVariables.Instance.LUTName = obj;

                // Confirm successful assignment
                 Mod.Log.Info($"[DEBUG] Successfully updated LUT names: RenderEffectsSystem.LutName_Example = {RenderEffectsSystem.LutName_Example}, GlobalVariables.Instance.LUTName = {GlobalVariables.Instance.LUTName}");
            }
            catch (Exception ex)
            {
                // Log any exceptions
                 Mod.Log.Info($"[ERROR] Exception occurred in UpdateLUTName: {ex.Message}\n{ex.StackTrace}");
            }
        }
        private void UpdateLUT()
        {
            RenderEffectsSystem.UpdateLUT();
        }

        private void HandleLUTContribution(float obj)
        {
            GlobalVariables.Instance.LUTContribution = obj;
            UpdateLUTContribution();
        }

        private void UpdateLUTContribution()
        {
            if (RenderEffectsSystem.m_Tonemapping != null && GlobalVariables.Instance != null)
            {
                RenderEffectsSystem.m_Tonemapping.lutContribution.value = GlobalVariables.Instance.LUTContribution;
            }
            else
            {
                Lumina.Mod.Log.Info("RenderEffectsSystem.m_Tonemapping or GlobalVariables.Instance is null.");
            }
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
            SliderPanel.ResetSettings();
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
