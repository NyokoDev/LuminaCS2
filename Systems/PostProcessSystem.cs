using Game.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine;
using LuminaMod.XML;
using Lumina.UI;
using Game.Simulation;
using JetBrains.Annotations;
using Game.Assets;
using System.Drawing.Text;
using static UnityEngine.Rendering.HighDefinition.VolumetricClouds;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.Rendering.HighDefinition.WindParameter;
using System.IO;
using Lumina.XML;
using System.Runtime.InteropServices;

namespace Lumina.Systems
{

    internal partial class PostProcessSystem : SystemBase
    {
        public static string lutFilePath = Path.Combine(GlobalPaths.LuminaLUTSDirectory, GlobalVariables.Instance.LUTName + ".cube");
        public static string[] LutFiles;
        bool m_SetupDone = false;
        Volume LuminaVolume;
        private VolumeProfile m_Profile;

        public PostProcessSystem PPS;

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

        public static string TextureFormat;


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

            SceneFlowChecker.CheckForErrors();

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
            PostProcessSystem.ToneMappingMode = GlobalVariables.Instance.TonemappingMode.ToString();
            PostProcessSystem.TextureFormat = GlobalVariables.Instance.TextureFormat.ToString();
            CubeLutLoader.TextureFormat = GlobalVariables.Instance.TextureFormat;
        }



        public static void UpdateLUT()
        {
            try
            {
                var lutFilePath = Path.Combine(GlobalPaths.LuminaLUTSDirectory, LutName_Example + ".cube");

                // Ensure tonemapping is active and properly configured
                m_Tonemapping.active = true;
                m_Tonemapping.mode.overrideState = true;
                m_Tonemapping.mode.value = GlobalVariables.Instance.TonemappingMode;
                m_Tonemapping.lutTexture.overrideState = true;

                // Attempt to load the LUT texture from file
                Texture3D lutTexture = null;

                if (GlobalVariables.Instance.TextureFormat == UnityEngine.TextureFormat.RGBA64)
                {
                    lutTexture = CubeLutLoader.LoadLutFromFileRGBA64(lutFilePath);
                    UpdateLutTextureFormat(UnityEngine.TextureFormat.RGBA64);
                }
                else if (GlobalVariables.Instance.TextureFormat == UnityEngine.TextureFormat.RGBAHalf)
                {
                    lutTexture = CubeLutLoader.LoadLutFromFileRGBAHalf(lutFilePath);
                    UpdateLutTextureFormat(UnityEngine.TextureFormat.RGBAHalf);
                }

                if (lutTexture == null)
                {
                    Mod.Log.Error($"Failed to load LUT texture from file: {lutFilePath}");
                    return;
                }



                // Set the loaded LUT texture
                m_Tonemapping.lutTexture.value = lutTexture;
                m_Tonemapping.ValidateLUT();


                Mod.Log.Info($"LUT successfully set to: {lutFilePath}");

                GlobalVariables.Instance.LUTName = LutName_Example;

                // Save global variables to file, with error handling
                GlobalVariables.SaveToFile(GlobalPaths.GlobalModSavingPath);
            }
            catch (Exception ex)
            {
                Mod.Log.Error($"An error occurred while updating LUT: {ex.Message}\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Updates LUT texture format to desired.
        /// </summary>
        /// <param name="newFormat">New format.</param>
        public static void UpdateLutTextureFormat(TextureFormat newFormat)
        {
            // Ensure the LUT texture is not null
            if (m_Tonemapping.lutTexture.value != null)
            {
                // Create a new texture with the desired format
                Texture3D newLutTexture = ChangeTextureFormat((Texture3D)m_Tonemapping.lutTexture.value, newFormat);

                // Set the new texture
                m_Tonemapping.lutTexture.value = newLutTexture;

                // Validate the new LUT texture
                m_Tonemapping.ValidateLUT();

                Lumina.Mod.Log.Info($"LUT Texture format changed to {newFormat} and validated successfully.");
            }
            else
            {
                Lumina.Mod.Log.Info("LUT Texture is null, cannot change format.");
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

            m_Tonemapping.active = true;
            Mod.Log.Info("Tonemapping activated.");

            m_Tonemapping.mode.value = GlobalVariables.Instance.TonemappingMode;

            m_Tonemapping.lutTexture.overrideState = true;
            m_Tonemapping.mode.overrideState = true;
            Mod.Log.Info("Tonemapping mode set to" + GlobalVariables.Instance.TonemappingMode + " and overrideState enabled.");

            // Attempt to load the LUT texture from file
            Texture3D lutTexture = null;

            if (GlobalVariables.Instance.TextureFormat == UnityEngine.TextureFormat.RGBA64)
            {
                lutTexture = CubeLutLoader.LoadLutFromFileRGBA64(lutFilePath);
            }
            else if (GlobalVariables.Instance.TextureFormat == UnityEngine.TextureFormat.RGBAHalf)
            {
                lutTexture = CubeLutLoader.LoadLutFromFileRGBAHalf(lutFilePath);
            }

            if (lutTexture == null)
            {
                Mod.Log.Error($"Failed to load LUT texture from file: {lutFilePath}");
                return;
            }

            Mod.Log.Info("LUT texture loaded successfully.");

            if (LUTSValidated)
            {
                m_Tonemapping.lutTexture.value = lutTexture;
                Mod.Log.Info("LUT texture applied to the tonemapping component.");
            }
            else
            {
                Mod.Log.Info("LUT texture validation failed.");
                return;
            }

            m_Tonemapping.lutContribution.overrideState = true;
            m_Tonemapping.lutContribution.Override(GlobalVariables.Instance.LUTContribution);

            Mod.Log.Info("LUT contribution set with value: " + GlobalVariables.Instance.LUTContribution);

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
                    PostProcessSystem.LutFiles = Directory.GetFiles(GlobalPaths.LuminaLUTSDirectory);
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
            PostProcessSystem.UpdateTonemapping();
        }

        private static void UpdateTonemapping()
        {
            m_Tonemapping.mode.value = GlobalVariables.Instance.TonemappingMode;
        }

        /// <summary>
        /// Sets texture format to RGBA64 OR 32 based on user choice.
        /// </summary>
        /// <param name="obj">Texture format.</param>
        /// <exception cref="Exception">Throws an exception.</exception>
        internal static void SetTextureFormat(float obj)
        {
            TextureFormat mode;

            switch (obj)
            {
                case 0f:
                    mode = UnityEngine.TextureFormat.RGBA64;
                    break;
                case 1f:
                    mode = UnityEngine.TextureFormat.RGBAHalf;
                    break;
                default:
                    // Optionally, handle the default case (e.g., log a warning or set a fallback mode)
                    return;
            }

            // Ensure GlobalVariables.Instance is not null before setting the value
            if (GlobalVariables.Instance != null)
            {
                GlobalVariables.Instance.TextureFormat = mode;
                Mod.Log.Info("Texture format set to: " + mode);
            }
            else
            {
                throw new Exception("Null reference.");
            }
        }
    }
}
