namespace Lumina.Systems
{
    using Game;
    using Game.Assets;
    using Game.Prefabs;
    using Game.Rendering;
    using Game.SceneFlow;
    using Game.Simulation;
    using Game.UI;
    using Game.UI.Menu;
    using JetBrains.Annotations;
    using Lumina.Systems.TextureHelper;
    using Lumina.UI;
    using Lumina.XML;
    using LuminaMod.XML;
    using System;
    using System.Collections.Generic;
    using System.Drawing.Text;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.Experimental.Rendering;
    using UnityEngine.Profiling;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.HighDefinition;
    using static UnityEngine.Rendering.DebugUI;
    using static UnityEngine.Rendering.HighDefinition.CameraSettings;
    using static UnityEngine.Rendering.HighDefinition.VolumetricClouds;
    using static UnityEngine.Rendering.HighDefinition.WindParameter;
    using dialog = Game.UI.MessageDialog;
    using Version = Game.Version;

    /// <summary>
    /// Starts UNITY HDRP Volume and render effects.
    /// </summary>
    internal partial class RenderEffectsSystem : GameSystemBase
    {
        public static string lutFilePath = Path.Combine(GlobalPaths.LuminaLUTSDirectory, GlobalVariables.Instance.LUTName + ".cube");
        public static string[] LutFiles;
        public static string[] CubemapFiles;
        bool m_SetupDone = false;
        public static Volume LuminaVolume;
        private VolumeProfile m_Profile;

        public RenderEffectsSystem PPS;
        public ColorAdjustments m_ColorAdjustments;
        
        public ContactShadows m_ContactShadows;
        public static Tonemapping m_Tonemapping;
        private WhiteBalance m_WhiteBalance;
        private ShadowsMidtonesHighlights m_ShadowsMidtonesHighlights;
        public VolumetricClouds m_VolumetricClouds;
        public static string LutName_Example;
        public static string ToneMappingMode;
        public static PhysicallyBasedSky LightingPhysicallyBasedSky;

        private UnityEngine.Rendering.HighDefinition.ColorAdjustments colorAdjustments;
        private PhotoModeRenderSystem PhotoModeRenderSystem;

        public static bool InitializedVolume = false;
        public static bool LUTSValidated = false;
        public static bool PanelInitialized = false;

        public static bool Panel = true;

        public bool LUTloaded = false;
        private VisualEnvironment m_Sky;
        private HDRISky hdriSky;


        /// <summary>
        /// Cubemap file path.
        /// </summary>
        public static string cubemapFilePath = Path.Combine(GlobalPaths.LuminaHDRIDirectory, GlobalVariables.Instance.CubemapName + ".png");

        /// <summary>
        /// Cubemap.
        /// </summary>
        public static Cubemap GlobalCubemap;
        public static ScreenSpaceAmbientOcclusion m_AmbientOcclusion;

        /// <summary>
        /// Gets or sets a value indicating whether Tonemapping mode is External.
        /// </summary>
        public static bool IsExternalMode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Tonemapping mode is Custom.
        /// </summary>
        public static bool IsCustomMode { get; set; }
        public GlobalIllumination m_GlobalIllumination { get; private set; }

        /// <summary>
        /// Called when the system is created.
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();

            CheckVersion();

            GlobalVariables.LoadFromFile(GlobalPaths.GlobalModSavingPath);
            CheckForMods();



            CheckGPU();
            ConvertToHDRP();
            InitializeCubemap();
            GetPrivateFieldm_PhysicallyBasedSky();
            InitializeAmbientOcclusion();

        }

        private void InitializeAmbientOcclusion()
        {
            // Null safety
            if (LuminaVolume == null)
                return;

            if (LuminaVolume.profile == null)
                return;

            var profile = LuminaVolume.profile;

            // Add only if missing
            if (!profile.TryGet(out m_AmbientOcclusion))
                m_AmbientOcclusion = profile.Add<ScreenSpaceAmbientOcclusion>();

            if (m_AmbientOcclusion == null)
                return;

            // Enable / disable component
            m_AmbientOcclusion.active = GlobalVariables.Instance.IsScreenSpaceAmbientOcclusion;

            // Proper HDRP overrides (ALWAYS use Override)
            m_AmbientOcclusion.intensity.Override(GlobalVariables.Instance.AmbientOcclusionIntensity);
            m_AmbientOcclusion.maximumRadiusInPixels = (GlobalVariables.Instance.AmbientOcclusionMaxRadiusInPixels);

            m_AmbientOcclusion.radius.Override(GlobalVariables.Instance.AmbientOcclusionRadius);
            m_AmbientOcclusion.stepCount = (GlobalVariables.Instance.AmbientOcclusionStepCount);

            m_AmbientOcclusion.temporalAccumulation.Override(GlobalVariables.Instance.AmbientOcclusionTemporalAccumulation);
            m_AmbientOcclusion.spatialBilateralAggressiveness.Override(GlobalVariables.Instance.AmbientOcclusionBilateralAggressiveness);
            m_AmbientOcclusion.ghostingReduction.Override(GlobalVariables.Instance.AmbientOcclusionGhostingReduction);

            m_AmbientOcclusion.fullResolution = (GlobalVariables.Instance.AmbientOcclusionFullResolution);
            m_AmbientOcclusion.directLightingStrength.Override(GlobalVariables.Instance.AmbientOcclusionDirectLightingStrength);
        }

        private void CheckVersion()
        {
            string url = "https://raw.githubusercontent.com/NyokoDev/LuminaCS2/refs/heads/main/XML/version.txt";
            string unityversion = UnityEngine.Application.unityVersion;
            string currentVersion = GlobalPaths.Version;
            string gameVersion = Version.current.fullVersion;
            string supportedGameVersion = GlobalPaths.SupportedGameVersion;

            Mod.Log.Info($"Checking game version: {gameVersion}");

            if (gameVersion != supportedGameVersion)
            {
                string errorMsg = "[LUMINA] Unsupported game version: {gameVersion}. Supported version is {supportedGameVersion}.";
                string recommendation =
                    "Recommendations:\n" +
                    "- Update your game to the latest supported version.\n" +
                    "- Check for a newer version of the Lumina mod.\n" +
                    "- Visit the Lumina support or GitHub page for help.\n" +
                    "- Join the Discord for assistance: https://discord.gg/5gZgRNm29e";

                Mod.Log.Error($"{errorMsg}\n{recommendation}");

                {
                    GlobalPaths.SendMessage(errorMsg); // Show a message box with the version information
                }

                return;
            }

            Mod.Log.Info("Unity version " + unityversion);

            try
            {
                using (WebClient client = new WebClient())
                {
                    string latestVersion = client.DownloadString(url).Trim();

                    if (currentVersion == latestVersion)
                    {
                        Mod.Log.Info("Lumina is up to date with version: " + currentVersion);
                    }
                    else
                    {
                        string message = string.Format("Lumina new version available! Current: {0} | Latest: {1}", currentVersion, latestVersion);
                        GlobalPaths.SendMessage(message); // Show a message box with the version information
                        Mod.Log.Info(message);
                    }
                }
            }
            catch (Exception ex)
            {
                Mod.Log.Info("Error checking version: " + ex.Message);
            }
        }

        private void CheckForMods()
        {
            if (GlobalVariables.Instance.ViewTimeOfDaySlider &&
                CompatibilityHelper.CheckForIncompatibleMods("TimeWeatherAnarchy"))
            {
                // Disable the slider if incompatible mod is active
                GlobalVariables.Instance.ViewTimeOfDaySlider = false;

                var dialog = new SimpleMessageDialog(
                    "Incompatible mod detected: Time and Weather Anarchy.\n\n" +
                    "This mod conflicts with Lumina's Time of Day Slider.\n" +
                    "To prevent conflicts, the slider has been disabled.\n\n" +
                    "Please remove Time and Weather Anarchy to use the slider again."
                );
                GameManager.instance.userInterface.appBindings.ShowMessageDialog(dialog, null);
            }

            if (!GlobalVariables.Instance.UseRoadTextures) return;

            string[] incompatibleMods = { "RoadWearRemover", "RoadWearAdjuster" };
            foreach (var modName in incompatibleMods)
            {
                if (CompatibilityHelper.CheckForIncompatibleMods(modName))
                {
                    string errorMessage =
                        $"Incompatible mod detected: {modName}.\n" +
                        $"Both Lumina and {modName} modify road textures, which can cause visual glitches or conflicts.\n" +
                        "To ensure stability, Lumina's road texture system has been disabled.\n" +
                        $"Please remove {modName} to take full advantage of Lumina's features.";

#if DEBUG
                    Mod.Log.Info(errorMessage);
#endif
                    GlobalVariables.Instance.UseRoadTextures = false;

                    var dialog = new SimpleMessageDialog(errorMessage);
                    GameManager.instance.userInterface.appBindings.ShowMessageDialog(dialog, null);

                    break;
                }
            }
        }

        private void CheckGPU()
        {
            string gpuName = SystemInfo.graphicsDeviceName;
            string gpuLower = gpuName.ToLower();

            bool isRTX = gpuName.Contains("RTX");
            bool is40Series = gpuName.Contains("RTX 40");
            bool isGTX = gpuName.Contains("GTX");

            bool isAMD = gpuLower.Contains("radeon") || gpuLower.Contains("amd");
            bool isRDNA3 = gpuLower.Contains("rx 7") || gpuLower.Contains("rx 7900") || gpuLower.Contains("rx 7800");
            bool isIntel = gpuLower.Contains("intel");

            Mod.Log.Info($"🖥️ Detected GPU: {gpuName}");

            if (is40Series)
            {
                Mod.Log.Info("🚀 RTX 40-series GPU detected. DLSS is supported in CS2.");
            }
            else if (isRTX)
            {
                Mod.Log.Info("✅ RTX GPU detected. DLSS is available in CS2 — enable them in graphics settings.");
            }
            else if (isGTX)
            {
                Mod.Log.Info("🧓 GTX GPU detected. No DLSS or Reflex — you're on classic gear. Tweak Lumina for performance.");
            }
            else if (isRDNA3)
            {
                Mod.Log.Info("🔥 RDNA3 GPU detected (RX 7000 series). DLSS isn’t supported, but FSR may be available.");
            }
            else if (isAMD)
            {
                Mod.Log.Info("🟥 AMD GPU detected. No DLSS, but CS2 may support FSR depending on your driver and settings.");
            }
            else if (isIntel)
            {
                Mod.Log.Info("💀 Intel integrated GPU detected. Turn everything down — including your expectations.");
            }
            else
            {
                Mod.Log.Info("❓ Unknown GPU detected. Results may vary — keep Lumina settings on the safe side.");
            }
        }



        public static void InitializeCubemap()
        {
            if (GlobalVariables.Instance == null)
            {
                Mod.Log.Error("GlobalVariables.Instance is null! Aborting.");
                return;
            }

            if (string.IsNullOrEmpty(GlobalVariables.Instance.CubemapName) ||
                GlobalVariables.Instance.CubemapName.Equals("None", StringComparison.OrdinalIgnoreCase))
            {
                Mod.Log.Info("Cubemap name is none or empty. Ignoring.");
                return;
            }

            // Load the new cubemap
            if (string.IsNullOrEmpty(GlobalVariables.Instance.CubemapName) ||
    GlobalVariables.Instance.CubemapName == "None" ||
    GlobalVariables.Instance.CubemapName == "Select Cubemap")
            {
                Mod.Log.Info("Cubemap name is invalid. Skipping cubemap loading.");

                GlobalCubemap = null;
            }
            else
            {
                GlobalCubemap = CubemapLoader.LoadSavedCubemap();
            }


            if (GlobalCubemap == null)
            {
                Mod.Log.Error("CubemapLoader.LoadSavedCubemap() returned null! Aborting.");
                return;
            }

            RenderEffectsSystem.ApplyCubemap();
            Mod.Log.Info("Initialized cubemap successfully.");
        }




        /// <summary>
        /// Logs current LUT log size.
        /// </summary>
        private static void LogSizeAssureCorrectMode()
        {
            // Assuming you already have a reference to the current HDRenderPipelineAsset
            HDRenderPipelineAsset currentAsset = GraphicsSettings.currentRenderPipeline as HDRenderPipelineAsset;

            if (currentAsset != null)
            {
                int lutSize = currentAsset.currentPlatformRenderPipelineSettings.postProcessSettings.lutSize;
                Mod.Log.Info($"Current LUT size: {lutSize}");
            }
            else
            {
                Mod.Log.Info("Failed to retrieve the current HDRenderPipelineAsset.");
            }

            // Check and set Tonemapping mode
            if (m_Tonemapping.mode.value == TonemappingMode.External)
            {
                GlobalVariables.Instance.TonemappingMode = TonemappingMode.External;
                IsExternalMode = true;
                IsCustomMode = false;
            }
            else if (m_Tonemapping.mode.value == TonemappingMode.Custom)
            {
                GlobalVariables.Instance.TonemappingMode = TonemappingMode.Custom;
                IsCustomMode = true;
                IsExternalMode = false;
            }
            else
            {
                // If neither condition is met, set both to false
                IsExternalMode = false;
                IsCustomMode = false;
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

            UpdateNames();

            if (!LUTloaded)
            {
                TonemappingLUT();
                LUTloaded = true;
            }

            ColorAdjustments();
            WhiteBalance();
            ShadowsMidTonesHighlights();
            ContactShadowsUpdate();
            OriginalShadows();
            AmbientOcclusionUpdate();
        }

        private void AmbientOcclusionUpdate()
        {
            if (m_AmbientOcclusion == null)
                return;

            // Enable / Disable
            m_AmbientOcclusion.active = GlobalVariables.Instance.IsScreenSpaceAmbientOcclusion;

            // Only update parameters if enabled (optional but cleaner)
            if (!m_AmbientOcclusion.active)
                return;

            // Always use Override for VolumeParameters
            m_AmbientOcclusion.intensity.Override(
                GlobalVariables.Instance.AmbientOcclusionIntensity);

            m_AmbientOcclusion.maximumRadiusInPixels =
                GlobalVariables.Instance.AmbientOcclusionMaxRadiusInPixels;

            m_AmbientOcclusion.radius.Override(
                GlobalVariables.Instance.AmbientOcclusionRadius);

            m_AmbientOcclusion.stepCount =
                GlobalVariables.Instance.AmbientOcclusionStepCount;

            m_AmbientOcclusion.temporalAccumulation.Override(
                GlobalVariables.Instance.AmbientOcclusionTemporalAccumulation);

            m_AmbientOcclusion.spatialBilateralAggressiveness.Override(
                GlobalVariables.Instance.AmbientOcclusionBilateralAggressiveness);

            m_AmbientOcclusion.ghostingReduction.Override(
                GlobalVariables.Instance.AmbientOcclusionGhostingReduction);

            m_AmbientOcclusion.fullResolution =
                GlobalVariables.Instance.AmbientOcclusionFullResolution;

            m_AmbientOcclusion.directLightingStrength.Override(
                GlobalVariables.Instance.AmbientOcclusionDirectLightingStrength);
        }

        private void OriginalShadows()
        {
      
        }

        private void ContactShadowsUpdate()
        {
            m_ContactShadows.active = GlobalVariables.Instance.IsContactShadows;
            m_ContactShadows.length.Override(GlobalVariables.Instance.ContactShadowsLength);
            m_ContactShadows.maxDistance.Override(GlobalVariables.Instance.ContactShadowsMaxDistance);
            m_ContactShadows.minDistance.Override(GlobalVariables.Instance.ContactShadowsMinDistance);
            m_ContactShadows.thicknessScale.Override(GlobalVariables.Instance.ContactShadowsThicknessScale);
            m_ContactShadows.distanceScaleFactor.Override(GlobalVariables.Instance.ContactShadowsDistanceScaleFactor);
            m_ContactShadows.enable.Override(GlobalVariables.Instance.IsContactShadows);
            m_ContactShadows.opacity.Override(GlobalVariables.Instance.ContactShadowsOpacity);
            m_ContactShadows.rayBias.Override(GlobalVariables.Instance.ContactShadowsRayBias);
            m_ContactShadows.fadeDistance.Override(GlobalVariables.Instance.ContactShadowsFadeDistance);
            m_ContactShadows.fadeInDistance.Override(GlobalVariables.Instance.ContactShadowsFadeInDistance);
        }

        /// <summary>
        /// Disables the HDRI Sky and reverts to the default lighting sky configuration.
        /// </summary>
        public static void DisableCubemap()
        {
            try
            {
                // Enable the LightingPhysicallyBasedSky
                LightingPhysicallyBasedSky.active = true;
                DisableVisualEnvironment();
                Mod.Log.Info("LightingPhysicallyBasedSky enabled successfully.");
            }
            catch (Exception ex)
            {
                // Log any unexpected errors
                Mod.Log.Error($"Failed to enable LightingPhysicallyBasedSky: {ex.Message}");
            }
        }

        /// <summary>
        /// Removes the VisualEnvironment and HDRISky overrides from the LuminaVolume.
        /// </summary>
        private static void DisableVisualEnvironment()
        {
            // Ensure the LuminaVolume is available
            Volume luminaVolume = RenderEffectsSystem.LuminaVolume;
            if (luminaVolume == null)
            {
                Mod.Log.Error("LuminaVolume not found. Unable to remove VisualEnvironment and HDRISky.");
                return;
            }

            // Get the VolumeProfile
            VolumeProfile profile = luminaVolume.profile;
            if (profile == null)
            {
                Mod.Log.Error("No VolumeProfile assigned to LuminaVolume.");
                return;
            }

            // Attempt to remove the VisualEnvironment override
            if (profile.TryGet(out VisualEnvironment visualEnvironment))
            {
                profile.Remove<VisualEnvironment>();
                Mod.Log.Info("VisualEnvironment override removed successfully.");
            }
            else
            {
                Mod.Log.Info("VisualEnvironment override not found in the VolumeProfile.");
            }

            // Attempt to remove the HDRISky override
            if (profile.TryGet(out HDRISky hdriSky))
            {
                profile.Remove<HDRISky>();
                Mod.Log.Info("HDRISky override removed successfully.");
            }
            else
            {
                Mod.Log.Info("HDRISky override not found in the VolumeProfile.");
            }
        }


        public static void ApplyCubemap()
        {
            // Ensure GlobalCubemap is not null
            if (GlobalCubemap == null)
            {
                Mod.Log.Info("GlobalCubemap is null. Skipping cubemap application.");
                return;
            }

            // Get the active Volume and ensure it's not null
            Volume volume = RenderEffectsSystem.LuminaVolume;
            if (volume == null)
            {
                Mod.Log.Info("No Volume found in the scene. Add a Volume to configure HDRI Sky.");
                return;
            }

            // Fetch or create the VolumeProfile
            VolumeProfile profile = volume.profile;
            if (!profile)
            {
                Mod.Log.Info("No VolumeProfile assigned to the Volume.");
                return;
            }

            // Ensure HDRI Sky and Visual Environment overrides are present
            if (!profile.TryGet(out HDRISky hdriSky))
            {
                hdriSky = profile.Add<HDRISky>();
                hdriSky.active = true;
            }
            if (!profile.TryGet(out VisualEnvironment visualEnvironment))
            {
                visualEnvironment = profile.Add<VisualEnvironment>();
                visualEnvironment.active = true;
            }

            // Set HDRI Sky as the active sky type
            visualEnvironment.skyType.Override((int)SkyType.HDRI);

            // Assign the HDRI Cubemap
            hdriSky.hdriSky.overrideState = true;
            hdriSky.hdriSky.Override(GlobalCubemap);
            Mod.Log.Info("Applied Cubemap " + GlobalVariables.Instance.CubemapName);
            // Adjust brightness by setting the exposure
            float desiredExposure = 7000f;
            hdriSky.skyIntensityMode.Override(SkyIntensityMode.Lux);
            hdriSky.desiredLuxValue.Override(desiredExposure);
            GlobalVariables.Instance.HDRISkyEnabled = true;
            Mod.Log.Info("Added Visual Environment HDRI Sky component.");
        }

        public static void UpdateCubemap()
        {
            // Load the new cubemap
            GlobalCubemap = CubemapLoader.LoadCubemap();

            RenderEffectsSystem.ApplyCubemap();

            // Log that the cubemap has been initialized successfully
            Mod.Log.Info("Initialized cubemap successfully.");
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
                GC.Collect();

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
                m_Tonemapping.lutContribution.overrideState = GlobalVariables.Instance.LUTContributionOverrideState;
                m_Tonemapping.lutContribution.Override(GlobalVariables.Instance.LUTContribution);
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

                // Create a new Texture3D object to hold the LUT
                Texture3D newLutTexture = null;
                Mod.Log.Info($"Loading LUT with format: {GraphicsFormat.R16G16B16A16_SFloat}");

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
                    Mod.Log.Info($"Failed to load LUT texture from: {lutFilePath}");
                }

                // Update global variables
                GlobalVariables.Instance.LUTName = LutName_Example;
                Mod.Log.Info("Updated GlobalVariables.Instance.LUTName");



            }
            catch (Exception ex)
            {
                Mod.Log.Error($"An error occurred while updating LUT: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void TonemappingLUT()
        {
            Mod.Log.Info("Starting TonemappingLUT process.");

            LogSizeAssureCorrectMode();
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

            // Attempt to load the new LUT texture from file
            Texture3D lutTexture = null;
            lutTexture = CubeLutLoader.LoadLutFromFile(lutFilePath);
            if (lutTexture == null)
            {
                Mod.Log.Info($"Failed to load LUT texture from file: {lutFilePath}");
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
            m_Tonemapping.toeStrength.overrideState = GlobalVariables.Instance.ToeStrengthActive;
            m_Tonemapping.toeStrength.Override(GlobalVariables.Instance.ToeStrengthValue);
            m_Tonemapping.lutContribution.Override(GlobalVariables.Instance.LUTContribution);
            Mod.Log.Info("LUT contribution set with value: " + GlobalVariables.Instance.LUTContribution);

            // Validate and log the result
            bool isLUTValid = m_Tonemapping.ValidateLUT();

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
            var resourceNamespace = "Lumina.LUTS";

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
            colorAdjustments = m_ColorAdjustments;
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


        private void GetPrivateFieldm_PhysicallyBasedSky()
        {
            // Use reflection to get the private m_PhysicallyBasedSky field from LightingSystem
            Type lightingSystemType = typeof(LightingSystem);
            FieldInfo physicallyBasedSkyFieldInfo = lightingSystemType.GetField("m_PhysicallyBasedSky", BindingFlags.NonPublic | BindingFlags.Instance);

            if (physicallyBasedSkyFieldInfo != null)
            {
                LightingSystem lightingSystem = World.GetExistingSystemManaged<LightingSystem>();
                if (lightingSystem != null)
                {
                    // Get the value of the private field
                    var physicallyBasedSky = physicallyBasedSkyFieldInfo.GetValue(lightingSystem);

                    if (physicallyBasedSky != null)
                    {
                        // Set the value to the field in this script
                        LightingPhysicallyBasedSky = (UnityEngine.Rendering.HighDefinition.PhysicallyBasedSky)physicallyBasedSky;
                        if (GlobalVariables.Instance.HDRISkyEnabled)
                        {
                            ApplyCubemap();
                        }
                        else
                        {
                            Mod.Log.Info("HDRI Sky disabled. Space emission texture not applied.");
                        }

                        Mod.Log.Info("Successfully retrieved and assigned m_PhysicallyBasedSky.");
                    }
                    else
                    {
                        Mod.Log.Info("m_PhysicallyBasedSky field is null.");
                    }
                }
                else
                {
                    Mod.Log.Info("LightingSystem instance is null.");
                }
            }
            else
            {
                Mod.Log.Info("Field m_PhysicallyBasedSky not found.");
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
        /// Initializes Lumina's volume into the Scene.
        /// </summary>
        public void ConvertToHDRP()
        {
            if (!m_SetupDone)
            {
                CheckForOtherVolumes();

                // Create new Global Volume GameObject
                GameObject globalVolume = new GameObject("Lumina");


                globalVolume.AddComponent<SliderPanel>(); // Workaround for Photo Mode Retrieval - Send to Lumina button
                UnityEngine.Object.DontDestroyOnLoad(globalVolume);

                // Add Volume component
                LuminaVolume = globalVolume.AddComponent<Volume>();
                LuminaVolume.priority = 1980f;
                Mod.Log.Info("[LUMINA] Priority set to 1980.");

                if (GlobalVariables.Instance.LuminaVolumeEnabled)
                {
                    LuminaVolume.enabled = true;
                }
                else
                {
                    LuminaVolume.enabled = false;
                }

                LuminaVolume.name = "Lumina";

                // Access the Volume Profile
                m_Profile = LuminaVolume.profile;

#if DEBUG
                // Add Volumetric Clouds
                SetUpVolumetricClouds();
#endif

                // Add Tonemapping
                m_ColorAdjustments = m_Profile.Add<ColorAdjustments>();
                m_ShadowsMidtonesHighlights = m_Profile.Add<ShadowsMidtonesHighlights>(); // Shadows, midtones, highlights
                m_Tonemapping = m_Profile.Add<Tonemapping>();
                m_WhiteBalance = m_Profile.Add<WhiteBalance>();


                m_WhiteBalance.temperature.Override(GlobalVariables.Instance.Temperature);
                m_WhiteBalance.tint.Override(GlobalVariables.Instance.Tint);

                m_ShadowsMidtonesHighlights.shadows.Override(new Vector4(GlobalVariables.Instance.Shadows, GlobalVariables.Instance.Shadows, GlobalVariables.Instance.Shadows, GlobalVariables.Instance.Shadows));
                m_ShadowsMidtonesHighlights.midtones.Override(new Vector4(GlobalVariables.Instance.Midtones, GlobalVariables.Instance.Midtones, GlobalVariables.Instance.Midtones, GlobalVariables.Instance.Midtones));


                // GlobalIllumination
                m_GlobalIllumination = m_Profile.Add<GlobalIllumination>();
                m_GlobalIllumination.active = GlobalVariables.Instance.IsSSGIInterventionEnabled;
                m_GlobalIllumination.enable.Override(GlobalVariables.Instance.IsSSGIInterventionEnabled);


                // Contact Shadows

                m_ContactShadows = m_Profile.Add<ContactShadows>();
                m_ContactShadows.active = GlobalVariables.Instance.IsContactShadows;
                m_ContactShadows.length.Override(GlobalVariables.Instance.ContactShadowsLength);
                m_ContactShadows.maxDistance.Override(GlobalVariables.Instance.ContactShadowsMaxDistance);
                m_ContactShadows.minDistance.Override(GlobalVariables.Instance.ContactShadowsMinDistance);
                m_ContactShadows.thicknessScale.Override(GlobalVariables.Instance.ContactShadowsThicknessScale);
                m_ContactShadows.distanceScaleFactor.Override(GlobalVariables.Instance.ContactShadowsDistanceScaleFactor);
                m_ContactShadows.enable.Override(GlobalVariables.Instance.IsContactShadows);
                m_ContactShadows.opacity.Override(GlobalVariables.Instance.ContactShadowsOpacity);
                m_ContactShadows.rayBias.Override(GlobalVariables.Instance.ContactShadowsRayBias);
                m_ContactShadows.fadeDistance.Override(GlobalVariables.Instance.ContactShadowsFadeDistance);
                m_ContactShadows.fadeInDistance.Override(GlobalVariables.Instance.ContactShadowsFadeInDistance);
           

              

                foreach (var component in m_Profile.components)
                {
                    if (component == null)
                        continue;

                    Mod.Log.Info($"[LUMINA] ================================");
                    Mod.Log.Info($"[LUMINA] Component: {component.GetType().FullName}");
                    Mod.Log.Info($"[LUMINA] Active: {component.active}");

                }


                // Finalize Volume
                m_SetupDone = true;
                Mod.Log.Info("[LUMINA] Successfully added HDRP volume.");

            }
        }
        
    


        private void CheckForOtherVolumes()
        {
            try
            {
                var allVolumes = UnityEngine.Object.FindObjectsOfType<UnityEngine.Rendering.Volume>();

                if (allVolumes == null || allVolumes.Length == 0)
                {
                    Mod.Log.Info("[LUMINA] No HDRP Volumes found in the scene.");
                    return;
                }

                Mod.Log.Info($"[LUMINA] Found {allVolumes.Length} HDRP Volume(s) in the scene:");

                foreach (var volume in allVolumes)
                {
                    string name = volume.gameObject.name;
                    float priority = volume.priority;
                    bool isGlobal = volume.isGlobal;
                    int componentCount = volume.profile?.components?.Count ?? 0;

                    // Print hierarchy path
                    string path = GetHierarchyPath(volume.transform);
                    Mod.Log.Info($"[LUMINA] Volume: '{name}', Priority: {priority}, Global: {isGlobal}, Components: {componentCount}, Path: {path}");
                }

                // Now handle duplicates
                var grouped = allVolumes.GroupBy(v => v.gameObject.name);

                foreach (var group in grouped)
                {
                    var list = group.ToList();
                    if (list.Count <= 1)
                        continue;

                    var reference = list[0];

                    for (int i = 1; i < list.Count; i++)
                    {
                        var candidate = list[i];

                        if (reference == null || candidate == null)
                            continue;

                        if (reference.priority != candidate.priority || reference.isGlobal != candidate.isGlobal)
                            continue;

                        var compsA = reference.profile?.components;
                        var compsB = candidate.profile?.components;

                        if (compsA == null || compsB == null || compsA.Count != compsB.Count)
                            continue;

                        // Optional: deeper check for exact types can go here

                        string dupPath = GetHierarchyPath(candidate.transform);
                        Mod.Log.Info($"[LUMINA] Deleting duplicate volume '{candidate.gameObject.name}' at path: {dupPath}");
                        UnityEngine.Object.Destroy(candidate.gameObject);
                    }
                }
            }
            catch (Exception ex)
            {
                Mod.Log.Error($"[LUMINA] Error while checking HDRP volumes: {ex}");
            }

            // Local function to avoid clutter
            string GetHierarchyPath(Transform t)
            {
                if (t == null) return "[null]";
                string path = t.name;
                while (t.parent != null)
                {
                    t = t.parent;
                    path = t.name + "/" + path;
                }
                return path;
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
                     Mod.Log.Info($"Unknown tonemapping mode value: {obj}");
                    return;
            }

            // Set the mode in the global variables
            GlobalVariables.Instance.TonemappingMode = mode;
            RenderEffectsSystem.UpdateTonemapping();
        }

        private static void UpdateTonemapping()
        {
            m_Tonemapping.mode.value = GlobalVariables.Instance.TonemappingMode;

            IsExternalMode = GlobalVariables.Instance.TonemappingMode == TonemappingMode.External;
            IsCustomMode = GlobalVariables.Instance.TonemappingMode == TonemappingMode.Custom;

        }

        internal static void ToggleToeStrength()
        {
            bool newState = !GlobalVariables.Instance.ToeStrengthActive;
            GlobalVariables.Instance.ToeStrengthActive = newState;
            m_Tonemapping.toeStrength.overrideState = newState;
        }

        internal static void SetToeStrengthValue()
        {
            m_Tonemapping.toeStrength.Override(GlobalVariables.Instance.ToeStrengthValue);
        }

        internal static void SetTonemappingCustomModeProperties()
        {
            if (m_Tonemapping.mode.value == TonemappingMode.Custom)
            {
                m_Tonemapping.toeStrength.overrideState = GlobalVariables.Instance.ToeStrengthActive;
                m_Tonemapping.toeStrength.Override(GlobalVariables.Instance.ToeStrengthValue);

                m_Tonemapping.toeLength.overrideState = GlobalVariables.Instance.ToeLengthActive;
                m_Tonemapping.toeLength.Override(GlobalVariables.Instance.ToeLengthValue);

                m_Tonemapping.shoulderStrength.overrideState = GlobalVariables.Instance.shoulderStrengthActive;
                m_Tonemapping.shoulderStrength.Override(GlobalVariables.Instance.shoulderStrengthValue);
                m_Tonemapping.shoulderLength.overrideState = GlobalVariables.Instance.shoulderLengthActive;
                m_Tonemapping.shoulderAngle.overrideState = GlobalVariables.Instance.shoulderAngleActive;
                m_Tonemapping.shoulderAngle.Override(GlobalVariables.Instance.shoulderAngleValue);

                m_Tonemapping.gamma.overrideState = GlobalVariables.Instance.TonemappingGammaActive;
                m_Tonemapping.gamma.Override(GlobalVariables.Instance.TonemappingGammaValue);
            }

        }

        internal static void SetToeLengthValue()
        {
            m_Tonemapping.toeLength.Override(GlobalVariables.Instance.ToeLengthValue);
        }

        internal static void ToggleToeLength()
        {
            m_Tonemapping.toeStrength.overrideState = !m_Tonemapping.toeStrength.overrideState;
        }

        internal static void SetShoulderStrengthActive()
        {
            // Toggle the overrideState of shoulderStrength
            m_Tonemapping.shoulderStrength.overrideState = !m_Tonemapping.shoulderStrength.overrideState;

            // Update GlobalVariables.Instance.shoulderStrengthActive based on the new state
            GlobalVariables.Instance.shoulderStrengthActive = m_Tonemapping.shoulderStrength.overrideState;
        }


        internal static void handleShoulderStrength()
        {
            m_Tonemapping.shoulderStrength.Override(GlobalVariables.Instance.shoulderStrengthValue);
        }

        internal static void handleEmissionMultiplier(float obj)
        {
            GlobalVariables.Instance.spaceEmissionMultiplier = obj;
            LightingPhysicallyBasedSky.spaceEmissionMultiplier.Override(obj);
        }

        /// <summary>
        /// Adjusts light.
        /// </summary>
        internal static void AdjustAngularDiameter()
        {
            // Find the game object named "SunLight"
            GameObject sunLightObject = GameObject.Find("SunLight");

            if (sunLightObject != null)
            {
                Light light = sunLightObject.GetComponent<Light>();

                if (light != null && light.type == UnityEngine.LightType.Directional)
                {
                    HDAdditionalLightData hdLightData = light.GetComponent<HDAdditionalLightData>();

                    if (hdLightData != null)
                    {
                        hdLightData.angularDiameter = GlobalVariables.Instance.AngularDiameter != 0f ? GlobalVariables.Instance.AngularDiameter : 0f;
                        hdLightData.intensity = GlobalVariables.Instance.SunIntensity != 0f ? GlobalVariables.Instance.SunIntensity : 0f;
                        hdLightData.flareSize = GlobalVariables.Instance.SunFlareSize != 0f ? GlobalVariables.Instance.SunFlareSize : 0f;
                    }
                    else
                    {
                         Mod.Log.Info($"Sun Light found but no HDAdditionalLightData: {light.gameObject.name}");
                    }
                }
                else
                {
                     Mod.Log.Info($"GameObject 'SunLight' is not a directional light.");
                }
            }
            else
            {
                 Mod.Log.Info("Sun Light (game object) not found in the scene.");
            }
        }
    }
}