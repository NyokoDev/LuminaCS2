// <copyright file="GlobalVariables.cs" company="NyokoDev">
// Copyright (c) NyokoDev. All rights reserved.
// </copyright>

namespace LuminaMod.XML
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Xml.Serialization;
    using Game.Objects;
    using Game.PSI;
    using Game.Rendering;
    using Lumina;
    using Lumina.XML;
    using UnityEngine;
    using UnityEngine.Experimental.Rendering;
    using UnityEngine.Rendering;
    using UnityEngine.Rendering.HighDefinition;
    using Color = UnityEngine.Color;

    /// <summary>
    /// GlobalVariables class.
    /// </summary>
    [Serializable]
    public class GlobalVariables
    {
        /// <summary>
        /// Gets or sets postExposure.
        /// </summary>
        [XmlElement]
        public float PostExposure { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether postExposure is enabled.
        /// </summary>
        [XmlElement]
        public bool PostExposureActive { get; set; }

        /// <summary>
        /// Gets or sets contrast.
        /// </summary>
        [XmlElement]
        public float Contrast { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether contrast is enabled.
        /// </summary>
        [XmlElement]
        public bool ContrastActive { get; set; }

        /// <summary>
        /// Gets or sets hueshift.
        /// </summary>
        [XmlElement]
        public float HueShift { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether hueshift is enabled.
        /// </summary>
        [XmlElement]
        public bool HueShiftActive { get; set; }

        /// <summary>
        /// Gets or sets saturation.
        /// </summary>
        [XmlElement]
        public float Saturation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether saturation is enabled.
        /// </summary>
        [XmlElement]
        public bool SaturationActive { get; set; }

        /// <summary>
        /// Gets or sets longitude.
        /// </summary>
        [XmlElement]
        public float Longitude { get; set; }

        /// <summary>
        /// Gets or sets latitude.
        /// </summary>
        [XmlElement]
        public float Latitude { get; set; }

        /// <summary>
        /// Gets or sets temperature.
        /// </summary>
        [XmlElement]
        public float Temperature { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether temperature is enabled.
        /// </summary>
        [XmlElement]
        public bool TemperatureActive { get; set; }

        /// <summary>
        /// Gets or sets tint.
        /// </summary>
        [XmlElement]
        public float Tint { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether tint is enabled.
        /// </summary>
        [XmlElement]
        public bool TintActive { get; set; }

        /// <summary>
        /// Gets or sets shadows.
        /// </summary>
        [XmlElement]
        public float Shadows { get; set; } = 1f;

        /// <summary>
        /// Gets or sets a value indicating whether shadows is enabled.
        /// </summary>
        [XmlElement]
        public bool ShadowsActive { get; set; }

        /// <summary>
        /// Gets or sets midtones.
        /// </summary>
        [XmlElement]
        public float Midtones { get; set; } = 1f;

        /// <summary>
        /// Gets or sets a value indicating whether midtones is enabled.
        /// </summary>
        [XmlElement]
        public bool MidtonesActive { get; set; }

        /// <summary>
        /// Gets or sets highlights.
        /// </summary>
        [XmlElement]
        public float Highlights { get; set; } = 1f;

        /// <summary>
        /// Gets or sets a value indicating whether highlights is enabled.
        /// </summary>
        [XmlElement]
        public bool HighlightsActive { get; set; }

        [XmlElement]
        public float LUTContribution { get; set; } = 1f;

        [XmlElement]
        public bool LUTContributionOverrideState { get; set; } = true;

        [XmlElement]
        public string LUTName { get; set; }

        [XmlElement]
        public bool TimeOfDayLocked { get; set; }

        [XmlElement]
        public TonemappingMode TonemappingMode { get; set; } = TonemappingMode.None;

        [XmlElement]
        public bool SceneFlowCheckerEnabled { get; set; } = false;

        [XmlElement]
        public bool ToeStrengthActive { get; set; } = false;

        [XmlElement]
        public float ToeStrengthValue { get; set; } = 0f;

        [XmlElement]
        public bool ToeLengthActive { get; set; } = false;

        [XmlElement]
        public float ToeLengthValue { get; set; } = 0f;

        [XmlElement]
        public bool shoulderStrengthActive { get; set; } = false;

        [XmlElement]
        public float shoulderStrengthValue { get; set; } = 0f;

        [XmlElement]
        public bool shoulderLengthActive { get; set; } = false;

        [XmlElement]
        public bool shoulderAngleActive { get; set; } = false;

        [XmlElement]
        public float shoulderAngleValue { get; set; } = 0f;

        [XmlElement]
        public bool TonemappingGammaActive { get; set; } = false;

        [XmlElement]
        public float TonemappingGammaValue { get; set; } = 0f;

        [XmlElement]
        public bool SaveAutomatically { get; set; } = true;

        [XmlElement]
        public string CubemapName { get; set; }

        [XmlElement]
        public float spaceEmissionMultiplier { get; set; } = 1000f;

        [XmlElement]
        public bool HDRISkyEnabled { get; set; } = false;

        [XmlElement]
        public bool CustomSunEnabled { get; set; }

        [XmlElement]
        public float AngularDiameter { get; set; }

        [XmlElement]
        public float SunIntensity { get; set; }

        [XmlElement]
        public float SunFlareSize { get; set; }

        [XmlElement]
        public bool ReloadAllPackagesOnRestart { get; set; }

        [XmlElement]
        public bool LuminaVolumeEnabled { get; set; }

        [XmlElement]
        public bool LatLongEnabled { get; set; }

        [XmlElement]
        public bool MetroEnabled { get; set; }

        [XmlIgnore]
        public bool ChangingTime { get; set; }

        [XmlElement]
        public bool ViewTimeOfDaySlider { get; set; }

        [XmlElement]
        public float RoadTextureSmoothness { get; set; }

        [XmlElement]
        public float TextureBrightness { get; set; }

        [XmlElement]
        public float TextureOpacity { get; set; }

        [XmlElement]
        public bool UseRoadTextures { get; set; }

        [XmlElement]
        public Color PrimaryRoadColor { get; set; } = Color.white;

        [XmlElement]
        public Color SecondaryRoadColor { get; set; } = Color.white;

        [XmlElement]
        public bool EnableDebugLogs { get; set; } = false;

        [XmlElement]
        public bool SafeMode { get; set; } = false;

        [XmlElement]
        public bool PerformanceMode { get; set; } = false;

        [XmlElement]
        public bool IsSSGIInterventionEnabled { get; set; } = false;

        [XmlElement]
        public bool IsContactShadows { get; set; } = false;

        [XmlElement]
        public bool IsScreenSpaceAmbientOcclusion { get; set; } = false;

        [XmlElement]
        public int AmbientOcclusionMaxRadiusInPixels { get; set; }

        [XmlElement]
        public float AmbientOcclusionRadius { get;set; }

        [XmlElement]
        public int AmbientOcclusionStepCount { get;  set; }

        [XmlElement]
        public bool AmbientOcclusionTemporalAccumulation { get;set; }

        [XmlElement]
        public float AmbientOcclusionBilateralAggressiveness { get;  set; }

        [XmlElement]
        public float AmbientOcclusionGhostingReduction { get;  set; }

        [XmlElement]
        public bool AmbientOcclusionFullResolution { get; set; }

        [XmlElement]
        public float AmbientOcclusionDirectLightingStrength { get; set; }

        [XmlElement]
        public float AmbientOcclusionIntensity { get; set; }

        [XmlElement]
        public float AmbientOcclusionBlurSharpness { get; set; }

        [XmlElement]
        public bool AmbientOcclusionBilateralUpsample { get; set; }

        [XmlElement]
        public object AmbientOcclusionDirectionCount { get;  set; }

        [XmlElement]
        public float AmbientOcclusionSpecularOcclusion { get;  set; }

        [XmlElement]
        public bool AmbientOcclusionOccluderMotionRejection { get;  set; }

        [XmlElement]
        public bool AmbientOcclusionReceiverMotionRejection { get;  set; }

        [XmlElement]
        public LayerMask AmbientOcclusionLayerMask { get;  set; }

        [XmlElement]
        public bool AmbientOcclusionRayTracing { get;  set; }

        [XmlElement]
        public float AmbientOcclusionRayLength { get;  set; }

        [XmlElement]
        public int AmbientOcclusionSampleCount { get;  set; }

        [XmlElement]
        public bool AmbientOcclusionDenoise { get; set; }

        [XmlElement]
        public float AmbientOcclusionDenoiserRadius { get;  set; }






        // Contact Shadows
        [XmlElement] public float ContactShadowsLength { get; set; } = 1f;
        [XmlElement] public float ContactShadowsMaxDistance { get; set; } = 50f;
        [XmlElement] public float ContactShadowsMinDistance { get; set; } = 0.5f;
        [XmlElement] public float ContactShadowsThicknessScale { get; set; } = 1f;
        [XmlElement] public float ContactShadowsDistanceScaleFactor { get; set; } = 1f;
        [XmlElement] public float ContactShadowsOpacity { get; set; } = 1f;
        [XmlElement] public float ContactShadowsRayBias { get; set; } = 0.01f;
        [XmlElement] public float ContactShadowsFadeDistance { get; set; } = 20f;
        [XmlElement] public float ContactShadowsFadeInDistance { get; set; } = 0f;

        public static void EnsureSettingsFileExists(string filePath)
        {
            // Ensure directory exists
            if (!Directory.Exists(GlobalPaths.AssemblyDirectory))
            {
                Directory.CreateDirectory(GlobalPaths.AssemblyDirectory);
            }

            // Exit early if file already exists
            if (File.Exists(filePath))
            {
                return;
            }

            // Create and save default GlobalVariables instance
            GlobalVariables defaultVariables = new GlobalVariables();
            XmlSerializer serializer = new XmlSerializer(typeof(GlobalVariables));

            using (TextWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, defaultVariables);
            }

            Mod.Log.Info("Settings file not found. Created new file with default GlobalVariables.");
        }



        /// <summary>
        /// Serializes to a file.
        /// </summary>
        /// <param name="filePath">Filepath parameter.</param>
        public static void SaveToFile(string filePath)
        {
            if (!Directory.Exists(GlobalPaths.AssemblyDirectory))
            {
                Directory.CreateDirectory(GlobalPaths.AssemblyDirectory);
            }

            const int maxRetries = 3;
            int retries = 0;

            while (retries < maxRetries)
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(GlobalVariables));
                    using (TextWriter writer = new StreamWriter(filePath))
                    {
                        serializer.Serialize(writer, Instance);
                    }

                    break; // Break out of the loop if the write operation is successful
                }
                catch (IOException ex) when (ex.HResult == -2147024864) // Sharing violation error code
                {
                    // Log the error and retry after a short delay
                    Mod.Log.Info($"Sharing violation encountered. Retrying in 1 second...");
                    Thread.Sleep(1000); // Wait for 1 second before retrying
                    retries++;
                }
                catch (Exception ex)
                {
                    Mod.Log.Info($"Error saving settings to file: {ex.Message}");
                    NotificationSystem.Push(
    "lumina",
    thumbnail: "https://i.imgur.com/C9fZDiA.png",
    title: "Lumina",
    text: $"Saving settings failed. Error message:" + ex.Message.ToString());
                    break; // Break out of the loop if an unexpected error occurs
                }
            }

            if (retries >= maxRetries)
            {
                Mod.Log.Info($"Failed to save settings to file after {maxRetries} retries.");
            }
        }


        /// <summary>
        /// Load From File.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static GlobalVariables LoadFromFile(string filePath)
        {
            try
            {
                Directory.CreateDirectory(GlobalPaths.AssemblyDirectory);

                if (!File.Exists(filePath))
                {
                    Mod.Log.Info("Settings file not found. Returning default instance.");
                    return Instance; // Nothing to load
                }

                var serializer = new XmlSerializer(typeof(GlobalVariables));
                using var reader = new StreamReader(filePath);
                var loadedVariables = (GlobalVariables)serializer.Deserialize(reader);

                if (loadedVariables == null)
                {
                    Mod.Log.Info("Loaded settings were null. Using defaults.");
                    return Instance;
                }

                // Automatically copy all simple properties from loadedVariables to Instance
                // Reflection ensures maintainability: adding new XmlElements won't break this
                var properties = typeof(GlobalVariables).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var prop in properties)
                {
                    if (!prop.CanWrite) continue;

                    var value = prop.GetValue(loadedVariables);
                    if (value != null)
                    {
                        prop.SetValue(Instance, value);
                    }
                }

                Mod.Log.Info("Settings loaded successfully.");
                return loadedVariables;
            }
            catch (Exception ex)
            {
                Mod.Log.Info($"Error loading settings: {ex.Message}");
                if (ex.InnerException != null)
                    Mod.Log.Info($"Inner: {ex.InnerException.Message}");

                GlobalPaths.SendMessage("Error loading settings. Please check the log for more details.");
                return null;
            }
        }



        private static GlobalVariables instance;


        /// <summary>
        /// Gets singleton pattern to ensure only one instance of GlobalVariables exists.
        /// </summary>
        public static GlobalVariables Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GlobalVariables();
                }

                return instance;
            }
        }

    
    }
}