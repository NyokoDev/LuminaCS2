﻿namespace Lumina.Systems
{
    using System;
    using System.Collections.Generic;
    using System.Drawing.Text;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;
    using Game.Assets;
    using Game.Rendering;
    using Game.Simulation;
    using JetBrains.Annotations;
    using Lumina.UI;
    using Lumina.XML;
    using LuminaMod.XML;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.HighDefinition;
    using static UnityEngine.Rendering.DebugUI;
    using static UnityEngine.Rendering.HighDefinition.VolumetricClouds;
    using static UnityEngine.Rendering.HighDefinition.WindParameter;

    /// <summary>
    /// Starts UNITY HDRP Volume and render effects.
    /// </summary>
    internal partial class RenderEffectsSystem : SystemBase
    {
        public static string lutFilePath = Path.Combine(GlobalPaths.LuminaLUTSDirectory, GlobalVariables.Instance.LUTName + ".cube");
        public static string[] LutFiles;
        bool m_SetupDone = false;
        Volume LuminaVolume;
        private VolumeProfile m_Profile;

        public RenderEffectsSystem PPS;

        public Exposure m_Exposure;
        public Vignette m_Vignette;
        public ColorAdjustments m_ColorAdjustments;
        public static Tonemapping m_Tonemapping;
        private WhiteBalance m_WhiteBalance;
        private ShadowsMidtonesHighlights m_ShadowsMidtonesHighlights;
        public VolumetricClouds m_VolumetricClouds;
        public static string LutName_Example;
        public static string ToneMappingMode;

        private UnityEngine.Rendering.HighDefinition.ColorAdjustments colorAdjustments;
        private PhotoModeRenderSystem PhotoModeRenderSystem;

        public static bool InitializedVolume = false;
        public static bool LUTSValidated = false;
        public static bool PanelInitialized = false;

        public static bool Panel = true;

        public bool LUTloaded = false;

        public static TextureFormat TextureFormat { get; set; } = TextureFormat.RGBAHalf;


        /// <summary>
        /// Logs current LUT log size.
        /// </summary>
        private static void LogSize()
        {
            // Assuming you already have a reference to the current HDRenderPipelineAsset
            HDRenderPipelineAsset currentAsset = GraphicsSettings.currentRenderPipeline as HDRenderPipelineAsset;

            if (currentAsset != null)
            {
                int lutSize = currentAsset.currentPlatformRenderPipelineSettings.postProcessSettings.lutSize;
                Lumina.Mod.Log.Info($"Current LUT size: {lutSize}");
            }
            else
            {
                Lumina.Mod.Log.Info("Failed to retrieve the current HDRenderPipelineAsset.");
            }
        }

        /// <summary>
        /// OnUpdate method.
        /// </summary>
        protected override void OnUpdate()
        {

            if (GlobalVariables.Instance.SceneFlowCheckerEnabled)
            {
                SceneFlowChecker.CheckForErrors();
            }

            if (Panel)
            {
                GameObject newGameObject = new GameObject();
                newGameObject.AddComponent<SliderPanel>();
                PanelInitialized = true;
                Panel = false;
            }

            // Start everything else
            PlanetarySettings();

            UpdateNames();

            if (!LUTloaded)
            {
                TonemappingLUT();
                LUTloaded = true;
            }

            ColorAdjustments();
            WhiteBalance();
            ShadowsMidTonesHighlights();
        }

        private void UpdateNames()
        {
            RenderEffectsSystem.ToneMappingMode = GlobalVariables.Instance.TonemappingMode.ToString();
        }


        /// <summary>
        /// Updates the LUT (Look-Up Table) used in tonemapping by loading a new LUT texture.
        /// from the specified file path. The method ensures that the existing LUT is cleared.
        /// and resources are properly unloaded before applying the new LUT texture.
        /// </summary>
        /// <remarks>
        /// This method handles different texture formats specified in the global settings and
        /// attempts to load the LUT accordingly. It also updates global variables with the new LUT name
        /// and saves these settings to a file. If an error occurs during the process, it is logged for debugging purposes.
        /// </remarks>
        /// <exception cref="Exception">Thrown if there is an error during the LUT update process.</exception>
        public static void UpdateLUT()
        {
            try
            {
                var lutFilePath = Path.Combine(GlobalPaths.LuminaLUTSDirectory, LutName_Example + ".cube");

                Mod.Log.Info($"Attempting to update LUT from file: {lutFilePath}");

                // Ensure tonemapping is active and properly configured
                if (m_Tonemapping == null)
                {
                    Mod.Log.Error("Tonemapping instance is null.");
                    return;
                }

                m_Tonemapping.active = true;
                m_Tonemapping.mode.overrideState = true;
                m_Tonemapping.mode.value = GlobalVariables.Instance.TonemappingMode;
                m_Tonemapping.lutTexture.overrideState = true;
                Mod.Log.Info($"Tonemapping mode set to: {m_Tonemapping.mode.value}");

                // Find and delete the existing texture if it already exists
                var existingTexture = Resources.FindObjectsOfTypeAll<Texture3D>().FirstOrDefault(t => t.name == RenderEffectsSystem.LutName_Example);
                if (existingTexture != null)
                {
                    Mod.Log.Info("Destroying existing Texture3D: " + existingTexture.name);
                    UnityEngine.Object.DestroyImmediate(existingTexture);
                }
                else
                {
                    Mod.Log.Info("No existing Texture3D to destroy.");
                }

                // Force resource unloading to ensure that no residual data remains
                Mod.Log.Info("Forcing resource unloading.");
                Resources.UnloadUnusedAssets();

                // Create a new Texture3D object to hold the LUT
                Texture3D newLutTexture = null;
                Mod.Log.Info($"Loading LUT with format: {GlobalVariables.Instance.TextureFormat}");

                // Load the new LUT texture from the file based on the format
                try
                {
                    newLutTexture = CubeLutLoader.LoadLutFromFile(lutFilePath);
                }
                catch (Exception ex)
                {
                    Mod.Log.Error($"Error loading LUT from file: {ex.Message}\n{ex.StackTrace}");
                }

                // Ensure that the newly created LUT texture is not null
                if (newLutTexture != null)
                {
                    // Assign a name to the new texture
                    newLutTexture.name = RenderEffectsSystem.LutName_Example;
                    Mod.Log.Info("Loaded new Texture3D: " + newLutTexture.name);

                    // Set the newly loaded LUT texture
                    m_Tonemapping.lutTexture.value = newLutTexture;
                    Mod.Log.Info("New LUT texture assigned.");
                    m_Tonemapping.ValidateLUT();
                    Mod.Log.Info($"LUT successfully set to: {lutFilePath}");
                }
                else
                {
                    Mod.Log.Error($"Failed to load LUT texture from: {lutFilePath}");
                }

                // Update global variables
                GlobalVariables.Instance.LUTName = LutName_Example;
                Mod.Log.Info("Updated GlobalVariables.Instance.LUTName");

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
            catch (Exception ex)
            {
                Mod.Log.Error($"An error occurred while updating LUT: {ex.Message}\n{ex.StackTrace}");
            }
        }



        private static Texture3D ChangeTextureFormat(Texture3D sourceTexture, TextureFormat newFormat)
        {
            if (sourceTexture == null)
            {
                Lumina.Mod.Log.Info("Source texture is null.");
                return null;
            }

            // Create a new texture with the new format
            Texture3D newTexture = new Texture3D(sourceTexture.width, sourceTexture.height, sourceTexture.depth, newFormat, false);
            newTexture.SetPixels(sourceTexture.GetPixels());
            newTexture.Apply();

            return newTexture;
        }

        private void TonemappingLUT()
        {
            Mod.Log.Info("Starting TonemappingLUT process.");

            ValidateLUTSDirectory();

            if (m_Tonemapping == null)
            {
                Mod.Log.Info("Tonemapping component is null.");
                return;
            }
            Mod.Log.Info("Tonemapping component found.");

            if (string.IsNullOrEmpty(lutFilePath))
            {
                Mod.Log.Info("LUT file path is null or empty.");
                return;
            }
            Mod.Log.Info("LUT file path is valid: " + lutFilePath);

            // Ensure tonemapping is active and set up correctly
            m_Tonemapping.active = true;
            m_Tonemapping.mode.value = GlobalVariables.Instance.TonemappingMode;
            m_Tonemapping.mode.overrideState = true;
            m_Tonemapping.lutTexture.overrideState = true;
            Mod.Log.Info("Tonemapping activated with mode: " + GlobalVariables.Instance.TonemappingMode);

            // Clear any existing LUT texture
            if (m_Tonemapping.lutTexture.value != null)
            {
                UnityEngine.Object.DestroyImmediate(m_Tonemapping.lutTexture.value);
                m_Tonemapping.lutTexture.value = null;
                Mod.Log.Info("Tonemapping LUT: Previous LUT texture cleared.");
            }

            Resources.UnloadUnusedAssets();

            // Attempt to load the new LUT texture from file
            Texture3D lutTexture = null;
            lutTexture = CubeLutLoader.LoadLutFromFile(lutFilePath);
            if (lutTexture == null)
            {
                Mod.Log.Error($"Failed to load LUT texture from file: {lutFilePath}");
                return;
            }

            Mod.Log.Info("LUT texture loaded successfully.");

            if (LUTSValidated)
            {
                // Set the name for initial texture management
                lutTexture.name = RenderEffectsSystem.LutName_Example;
                m_Tonemapping.lutTexture.value = lutTexture;
                Mod.Log.Info("LUT texture applied to the tonemapping component with name: " + lutTexture.name);
            }
            else
            {
                Mod.Log.Info("LUT texture validation failed.");
                return;
            }

            // Set LUT contribution
            m_Tonemapping.lutContribution.overrideState = true;
            m_Tonemapping.lutContribution.Override(GlobalVariables.Instance.LUTContribution);
            Mod.Log.Info("LUT contribution set with value: " + GlobalVariables.Instance.LUTContribution);

            // Validate and log the result
            bool isLUTValid = m_Tonemapping.ValidateLUT();
            LogSize();
            Mod.Log.Info("LUT validation result: " + isLUTValid);

            if (!isLUTValid)
            {
                Mod.Log.Info("Final LUT validation failed after assignment.");
            }
        }


        /// <summary>
        /// Validates and ensures predetermined LUTs are available in ModsData.
        /// </summary>
        public void ValidateLUTSDirectory()
        {
            try
            {
                // Ensure the directory path for LUTs is valid
                string directoryPath = GlobalPaths.LuminaLUTSDirectory;

                if (string.IsNullOrWhiteSpace(directoryPath))
                {
                    throw new InvalidOperationException("The directory path for LUTs is not set.");
                }

                // Create the directory if it doesn't exist
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                    RenderEffectsSystem.LutFiles = Directory.GetFiles(GlobalPaths.LuminaLUTSDirectory);
                }

                // Copy all embedded resources
                CopyAllEmbeddedResourcesToDirectory(directoryPath);
            }
            catch (Exception ex)
            {
                Mod.Log.Error($"Failed to validate or create the LUTs directory: {ex.Message}");
            }
        }

        private void CopyAllEmbeddedResourcesToDirectory(string directoryPath)
        {
            var assembly = GetType().Assembly;
            var resourceNamespace = "Lumina.LUTS"; // Replace with your actual namespace

            // Get all resource names
            var resourceNames = assembly.GetManifestResourceNames();

            foreach (var resourceName in resourceNames)
            {
                // Check if the resource belongs to the correct namespace
                if (resourceName.StartsWith(resourceNamespace))
                {
                    using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
                    {
                        if (resourceStream == null)
                        {
                            Mod.Log.Error($"Embedded resource '{resourceName}' not found.");
                            continue;
                        }

                        // Determine the destination path
                        var relativePath = resourceName.Substring(resourceNamespace.Length + 1); // Remove namespace prefix
                        var destinationPath = Path.Combine(directoryPath, relativePath);

                        // Create the directory if it doesn't exist
                        var destinationDirectory = Path.GetDirectoryName(destinationPath);
                        if (!Directory.Exists(destinationDirectory))
                        {
                            Directory.CreateDirectory(destinationDirectory);
                        }

                        // Copy the resource to the destination
                        using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
                        {
                            resourceStream.CopyTo(fileStream);
                        }

                        LUTSValidated = true;
                    }
                }
            }
        }



        private void ColorAdjustments()
        {
            // Use reflection to get the private ColorAdjustments field from LightingSystem
            Type lightingSystemType = typeof(LightingSystem);
            FieldInfo colorAdjustmentsField = lightingSystemType.GetField("m_ColorAdjustments", BindingFlags.NonPublic | BindingFlags.Instance);

            if (colorAdjustmentsField != null)
            {
                LightingSystem lightingSystem = World.GetExistingSystemManaged<LightingSystem>();
                colorAdjustments = (UnityEngine.Rendering.HighDefinition.ColorAdjustments)colorAdjustmentsField.GetValue(lightingSystem);

                if (colorAdjustments != null)
                {
                    // Set the exposure value to 0 using Override method
                    colorAdjustments.postExposure.Override(GlobalVariables.Instance.PostExposure);
                    colorAdjustments.postExposure.overrideState = GlobalVariables.Instance.PostExposureActive;


                    colorAdjustments.contrast.Override(GlobalVariables.Instance.Contrast);
                    colorAdjustments.contrast.overrideState = GlobalVariables.Instance.ContrastActive;

                    colorAdjustments.hueShift.Override(GlobalVariables.Instance.HueShift);
                    colorAdjustments.hueShift.overrideState = GlobalVariables.Instance.HueShiftActive;

                    colorAdjustments.saturation.Override(GlobalVariables.Instance.Saturation);
                    colorAdjustments.saturation.overrideState = GlobalVariables.Instance.SaturationActive;
                }
            }
        }

        private void WhiteBalance()
        {
            m_WhiteBalance.temperature.Override(GlobalVariables.Instance.Temperature);
            m_WhiteBalance.temperature.overrideState = GlobalVariables.Instance.TemperatureActive;
            m_WhiteBalance.tint.Override(GlobalVariables.Instance.Tint);
            m_WhiteBalance.tint.overrideState = GlobalVariables.Instance.TintActive;
        }

        private void ShadowsMidTonesHighlights()
        {
            m_ShadowsMidtonesHighlights.shadows.Override(new Vector4(GlobalVariables.Instance.Shadows, GlobalVariables.Instance.Shadows, GlobalVariables.Instance.Shadows, GlobalVariables.Instance.Shadows));
            m_ShadowsMidtonesHighlights.shadows.overrideState = GlobalVariables.Instance.ShadowsActive;
            m_ShadowsMidtonesHighlights.midtones.Override(new Vector4(GlobalVariables.Instance.Midtones, GlobalVariables.Instance.Midtones, GlobalVariables.Instance.Midtones, GlobalVariables.Instance.Midtones));
            m_ShadowsMidtonesHighlights.midtones.overrideState = GlobalVariables.Instance.MidtonesActive;
            m_ShadowsMidtonesHighlights.highlights.Override(new Vector4(GlobalVariables.Instance.Highlights, GlobalVariables.Instance.Highlights, GlobalVariables.Instance.Highlights, GlobalVariables.Instance.Highlights));
            m_ShadowsMidtonesHighlights.highlights.overrideState = GlobalVariables.Instance.HighlightsActive;
        }

        /// <summary>
        /// Called when the system is created.
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();
            ConvertToHDRP();
        }

        /// <summary>
        /// Initializes Lumina's volume into the Scene.
        /// </summary>
        public void ConvertToHDRP()
        {
            if (!m_SetupDone)
            {
                // Create new Global Volume GameObject
                GameObject globalVolume = new GameObject("Lumina");
                UnityEngine.Object.DontDestroyOnLoad(globalVolume);

                // Add Volume component
                LuminaVolume = globalVolume.AddComponent<Volume>();
                LuminaVolume.priority = 1980f;
                LuminaVolume.enabled = true;

                // Access the Volume Profile
                m_Profile = LuminaVolume.profile;

#if DEBUG
                // Add Volumetric Clouds
                SetUpVolumetricClouds();
#endif

                // Add Tonemapping
                m_Tonemapping = m_Profile.Add<Tonemapping>();

                // Add and configure White Balance effect
                m_WhiteBalance = m_Profile.Add<WhiteBalance>();
                m_WhiteBalance.active = true;
                m_WhiteBalance.temperature.Override(GlobalVariables.Instance.Temperature);
                m_WhiteBalance.tint.Override(GlobalVariables.Instance.Tint);

                // Add and configure Color Adjustments effect
                m_ColorAdjustments = m_Profile.Add<ColorAdjustments>();
                m_ColorAdjustments.colorFilter.Override(new Color(1f, 1f, 1f));

                m_SetupDone = true;

                m_ShadowsMidtonesHighlights = m_Profile.Add<ShadowsMidtonesHighlights>(); // Shadows, midtones, highlights
                m_ShadowsMidtonesHighlights.active = true;
                m_ShadowsMidtonesHighlights.shadows.Override(new Vector4(GlobalVariables.Instance.Shadows, GlobalVariables.Instance.Shadows, GlobalVariables.Instance.Shadows, GlobalVariables.Instance.Shadows));
                m_ShadowsMidtonesHighlights.midtones.Override(new Vector4(GlobalVariables.Instance.Midtones, GlobalVariables.Instance.Midtones, GlobalVariables.Instance.Midtones, GlobalVariables.Instance.Midtones));

                Mod.Log.Info("[LUMINA] Successfully added HDRP volume.");
            }
        }
#if DEBUG
        public void SetUpVolumetricClouds()
        {
            m_VolumetricClouds = m_Profile.Add<VolumetricClouds>();

            // Enable/Disable the volumetric clouds effect
            m_VolumetricClouds.enable.value = VolumetricCloudsData.vmcactive;

            // Renders the volumetric clouds effect pre or post transparent
            m_VolumetricClouds.renderHook.value = VolumetricCloudsData.cloudhook;

            // Controls whether clouds are local or part of the skybox
            m_VolumetricClouds.localClouds.Override(VolumetricCloudsData.localclouds);
            Mod.Log.Info("Override");

            // Controls the curvature of the cloud volume which defines the distance at which the clouds intersect with the horizon
            m_VolumetricClouds.earthCurvature.value = VolumetricCloudsData.earthCurvature;
            Mod.Log.Info("earthcurvature");

            // Tiling (x,y) of the cloud map
            m_VolumetricClouds.cloudTiling.value = VolumetricCloudsData.cloudtiling;

            // Offset (x,y) of the cloud map
            m_VolumetricClouds.cloudOffset.value = VolumetricCloudsData.cloudOffset;

            // Controls the altitude of the bottom of the volumetric clouds volume in meters
            m_VolumetricClouds.bottomAltitude.value = VolumetricCloudsData.bottomAltitude;

            // Controls the size of the volumetric clouds volume in meters
            m_VolumetricClouds.altitudeRange.value = VolumetricCloudsData.altitudeRange;

            // Controls the mode in which the clouds fade in when close to the camera's near plane
            m_VolumetricClouds.fadeInMode.value = VolumetricCloudsData.fadeInMode;

            // Controls the minimal distance at which clouds start appearing
            m_VolumetricClouds.fadeInStart.value = VolumetricCloudsData.fadeInStart;

            // Controls the distance that it takes for the clouds to reach their complete density
            m_VolumetricClouds.fadeInDistance.value = VolumetricCloudsData.fadeInDistance;

            // Controls the number of steps when evaluating the clouds' transmittance
            m_VolumetricClouds.numPrimarySteps.value = VolumetricCloudsData.numPrimarySteps;

            // Controls the number of steps when evaluating the clouds' lighting
            m_VolumetricClouds.numLightSteps.value = VolumetricCloudsData.numLightSteps;

            // Specifies the cloud map - Coverage (R), Rain (G), Type (B)
            m_VolumetricClouds.cloudMap.value = VolumetricCloudsData.cloudMap;

            // Specifies the lookup table for the clouds - Profile Coverage (R), Erosion (G), Ambient Occlusion (B)
            m_VolumetricClouds.cloudLut.value = VolumetricCloudsData.cloudLut;

            // Specifies the cloud control Mode: Simple, Advanced or Manual
            m_VolumetricClouds.cloudControl.value = VolumetricCloudsData.cloudControl;

            // Specifies the lower cloud layer distribution in the advanced mode
            m_VolumetricClouds.cumulusMap.value = VolumetricCloudsData.cumulusMap;

            // Overrides the coverage of the lower cloud layer specified in the cumulus map in the advanced mode
            m_VolumetricClouds.cumulusMapMultiplier.value = 1f;

            // Specifies the higher cloud layer distribution in the advanced mode
            m_VolumetricClouds.altoStratusMap.value = VolumetricCloudsData.altoStratusMap;

            // Overrides the coverage of the higher cloud layer specified in the alto stratus map in the advanced mode
            m_VolumetricClouds.altoStratusMapMultiplier.value = 1f;

            // Specifies the anvil shaped clouds distribution in the advanced mode
            m_VolumetricClouds.cumulonimbusMap.value = VolumetricCloudsData.cumulonimbusMap;

            // Overrides the coverage of the anvil shaped clouds specified in the cumulonimbus map in the advanced mode
            m_VolumetricClouds.cumulonimbusMapMultiplier.value = 1f;

            // Specifies the rain distribution in the advanced mode
            m_VolumetricClouds.rainMap.value = VolumetricCloudsData.rainMap;

            // Specifies the internal texture resolution used for the cloud map in the advanced mode
            m_VolumetricClouds.cloudMapResolution.value = VolumetricCloudsData.cloudMapResolution;

            // Controls the density (Y axis) of the volumetric clouds as a function of the height (X Axis) inside the cloud volume
            m_VolumetricClouds.densityCurve.value = VolumetricCloudsData.densityCurve;

            // Controls the erosion (Y axis) of the volumetric clouds as a function of the height (X Axis) inside the cloud volume
            m_VolumetricClouds.erosionCurve.value = VolumetricCloudsData.erosionCurve;

            // Controls the ambient occlusion (Y axis) of the volumetric clouds as a function of the height (X Axis) inside the cloud volume
            m_VolumetricClouds.ambientOcclusionCurve.value = VolumetricCloudsData.ambientOcclusionCurve;

            // Specifies the tint of the cloud scattering color
            m_VolumetricClouds.scatteringTint.value = VolumetricCloudsData.scatteringTint;

            // Controls the amount of local scattering in the clouds
            m_VolumetricClouds.powderEffectIntensity.value = VolumetricCloudsData.powderEffectIntensity;

            // Controls the amount of multi-scattering inside the cloud
            m_VolumetricClouds.multiScattering.value = VolumetricCloudsData.multiScattering;

            // Controls the global density of the cloud volume
            m_VolumetricClouds.densityMultiplier.value = VolumetricCloudsData.densityMultiplier;

            // Controls the larger noise passing through the cloud coverage
            m_VolumetricClouds.shapeFactor.value = VolumetricCloudsData.shapeFactor;

            // Controls the size of the larger noise passing through the cloud coverage
            m_VolumetricClouds.shapeScale.value = VolumetricCloudsData.shapeScale;

            // Controls the world space offset applied when evaluating the larger noise passing through the cloud coverage
            m_VolumetricClouds.shapeOffset.value = VolumetricCloudsData.shapeOffset;

            // Controls the smaller noise on the edge of the clouds
            m_VolumetricClouds.erosionFactor.value = VolumetricCloudsData.erosionFactor;

            // Controls the size of the smaller noise passing through the cloud coverage
            m_VolumetricClouds.erosionScale.value = VolumetricCloudsData.erosionScale;

            // Controls the type of noise used to generate the smaller noise passing through the cloud coverage
            m_VolumetricClouds.erosionNoiseType.value = VolumetricCloudsData.erosionNoiseType;

            // Controls the influence of the light probes on the cloud volume
            m_VolumetricClouds.ambientLightProbeDimmer.value = VolumetricCloudsData.ambientLightProbeDimmer;

            // Controls the influence of the sun light on the cloud volume
            m_VolumetricClouds.sunLightDimmer.value = VolumetricCloudsData.sunLightDimmer;

            // Controls how much Erosion Factor is taken into account when computing ambient occlusion
            m_VolumetricClouds.erosionOcclusion.value = VolumetricCloudsData.erosionOcclusion;

            // Sets the global horizontal wind speed in kilometers per hour
            m_VolumetricClouds.globalWindSpeed.value = VolumetricCloudsData.globalWindSpeed;

            // Controls the orientation of the wind relative to the X world vector
            m_VolumetricClouds.orientation.value = VolumetricCloudsData.orientation;

            // Controls the intensity of the wind-based altitude distortion of the clouds
            m_VolumetricClouds.altitudeDistortion.value = VolumetricCloudsData.altitudeDistortion;

            // Controls the multiplier to the speed of the cloud map
            m_VolumetricClouds.cloudMapSpeedMultiplier.value = VolumetricCloudsData.cloudMapSpeedMultiplier;

            // Controls the multiplier to the speed of the larger cloud shapes
            m_VolumetricClouds.shapeSpeedMultiplier.value = VolumetricCloudsData.shapeSpeedMultiplier;

            // Controls the multiplier to the speed of the erosion cloud shapes
            m_VolumetricClouds.erosionSpeedMultiplier.value = VolumetricCloudsData.erosionSpeedMultiplier;

            // Controls the vertical wind speed of the larger cloud shapes
            m_VolumetricClouds.verticalShapeWindSpeed.value = VolumetricCloudsData.verticalShapeWindSpeed;

            // Controls the vertical wind speed of the erosion cloud shapes
            m_VolumetricClouds.verticalErosionWindSpeed.value = VolumetricCloudsData.verticalErosionWindSpeed;

            // Temporal accumulation increases the visual quality of clouds by decreasing the noise
            m_VolumetricClouds.temporalAccumulationFactor.value = VolumetricCloudsData.temporalAccumulationFactor;

            // Enable/Disable the volumetric clouds ghosting reduction
            m_VolumetricClouds.ghostingReduction.value = VolumetricCloudsData.ghostingReduction;

            // Specifies the strength of the perceptual blending for the volumetric clouds
            m_VolumetricClouds.perceptualBlending.value = VolumetricCloudsData.perceptualBlending;

            // Enable/Disable the volumetric clouds shadow
            m_VolumetricClouds.shadows.value = VolumetricCloudsData.shadows;

            // Specifies the resolution of the volumetric clouds shadow map
            m_VolumetricClouds.shadowResolution.value = VolumetricCloudsData.shadowResolution;

            // Controls the vertical offset applied to compute the volumetric clouds shadow in meters
            m_VolumetricClouds.shadowPlaneHeightOffset.value = VolumetricCloudsData.shadowPlaneHeightOffset;

            // Sets the size of the area covered by shadow around the camera
            m_VolumetricClouds.shadowDistance.value = VolumetricCloudsData.shadowDistance;

            // Controls the opacity of the volumetric clouds shadow
            m_VolumetricClouds.shadowOpacity.value = VolumetricCloudsData.shadowOpacity;

            // Controls the shadow opacity when outside the area covered by the volumetric clouds shadow
            m_VolumetricClouds.shadowOpacityFallback.value = VolumetricCloudsData.shadowOpacityFallback;
        }
#endif


        private void PlanetarySettings()
        {
            try
            {
#if DEBUG
                Mod.Log.Info("Entered PlanetarySettings");
#endif
                LightingSystem lightingSystemInstance = World.GetExistingSystemManaged<LightingSystem>();
                if (lightingSystemInstance != null)
                {
                    Type lightingSystemType = typeof(LightingSystem);
                    FieldInfo planetarySystemField = lightingSystemType.GetField("m_PlanetarySystem", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (planetarySystemField != null)
                    {
                        PlanetarySystem planetarySystemInstance = (PlanetarySystem)planetarySystemField.GetValue(lightingSystemInstance);

                        if (planetarySystemInstance != null)
                        {


                            Type planetarySystemType = typeof(PlanetarySystem);
                            FieldInfo latitudeField = planetarySystemType.GetField("m_Latitude", BindingFlags.NonPublic | BindingFlags.Instance);
                            FieldInfo longitudeField = planetarySystemType.GetField("m_Longitude", BindingFlags.NonPublic | BindingFlags.Instance);

                            if (latitudeField != null && longitudeField != null)
                            {
                                float newLatitude = GlobalVariables.Instance.Latitude;
                                float newLongitude = GlobalVariables.Instance.Longitude;

                                latitudeField.SetValue(planetarySystemInstance, newLatitude);
                                longitudeField.SetValue(planetarySystemInstance, newLongitude);


                            }
                            else
                            {
#if DEBUG
                                Mod.Log.Info("Latitude or longitude field not found.");
#endif
                            }
                        }
                        else
                        {
#if DEBUG
                            Mod.Log.Info("PlanetarySystemInstance is null.");
#endif
                        }
                    }
                    else
                    {
#if DEBUG
                        Mod.Log.Info("m_PlanetarySystem field not found in LightingSystem.");
#endif
                    }
                }
                else
                {
#if DEBUG
                    Mod.Log.Info("LightingSystemInstance is null.");
#endif
                }
            }
            catch (Exception ex)
            {
#if DEBUG
                Mod.Log.Info("An error occurred: " + ex.Message);
#endif
            }
        }

        /// <summary>
        /// Sets tonemapping mode from dropdown.
        /// </summary>
        /// <param name="obj">Tonemapping mode.</param>
        internal static void SetTonemappingMode(float obj)
        {
            // Default to an invalid mode or a fallback mode if the float doesn't match any known mode
            TonemappingMode mode = TonemappingMode.External;

            // Check the input float and set the appropriate TonemappingMode
            switch (obj)
            {
                case 0f: // Represents "None"
                    mode = TonemappingMode.None;
                    break;
                case 1f: // Represents "External"
                    mode = TonemappingMode.External;
                    break;
                case 2f: // Represents "Custom"
                    mode = TonemappingMode.Custom;
                    break;
                case 3f: // Represents "Neutral"
                    mode = TonemappingMode.Neutral;
                    break;
                case 4f: // Represents "ACES"
                    mode = TonemappingMode.ACES;
                    break;
                default:
                    // Handle unknown mode or log an error
                    Lumina.Mod.Log.Info($"Unknown tonemapping mode value: {obj}");
                    return;
            }

            // Set the mode in the global variables
            GlobalVariables.Instance.TonemappingMode = mode;
            RenderEffectsSystem.UpdateTonemapping();
        }

        private static void UpdateTonemapping()
        {
            m_Tonemapping.mode.value = GlobalVariables.Instance.TonemappingMode;
        }
    }
}