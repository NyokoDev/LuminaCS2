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

        /// <summary>
        /// Gets or sets volumetric Clouds data.
        /// </summary>
        [XmlElement]
        public VolumetricCloudsData VolumetricCloudsData { get; set; }

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
        /// Loads from file.
        /// </summary>
        /// <param name="filePath">File path parameter.</param>
        /// <returns>Loaded variables.</returns>
        public static GlobalVariables LoadFromFile(string filePath)
        {
            try
            {
                // Ensure directory exists
                if (!Directory.Exists(GlobalPaths.AssemblyDirectory))
                {
                    Directory.CreateDirectory(GlobalPaths.AssemblyDirectory);
                }

                // Create an XmlSerializer for the GlobalVariables type.
                XmlSerializer serializer = new XmlSerializer(typeof(GlobalVariables));

                // Open the file for reading.
                using (TextReader reader = new StreamReader(filePath))
                {
                    // Deserialize the object from the file.
                    GlobalVariables loadedVariables = (GlobalVariables)serializer.Deserialize(reader);

                    // Set the loaded values to the corresponding properties, with default values if missing.
                    GlobalVariables.Instance.PostExposure = loadedVariables?.PostExposure ?? 0f;
                    GlobalVariables.Instance.PostExposureActive = loadedVariables?.PostExposureActive ?? false;
                    GlobalVariables.Instance.Contrast = loadedVariables?.Contrast ?? 0f;
                    GlobalVariables.Instance.ContrastActive = loadedVariables?.ContrastActive ?? false;
                    GlobalVariables.Instance.HueShift = loadedVariables?.HueShift ?? 0f;
                    GlobalVariables.Instance.HueShiftActive = loadedVariables?.HueShiftActive ?? false;
                    GlobalVariables.Instance.Saturation = loadedVariables?.Saturation ?? 0f;
                    GlobalVariables.Instance.SaturationActive = loadedVariables?.SaturationActive ?? false;

                    GlobalVariables.Instance.Latitude = loadedVariables?.Latitude ?? 0f;
                    GlobalVariables.Instance.Longitude = loadedVariables?.Longitude ?? 0f;

                    GlobalVariables.Instance.Temperature = loadedVariables?.Temperature ?? 0f;
                    GlobalVariables.Instance.TemperatureActive = loadedVariables?.TemperatureActive ?? false;
                    GlobalVariables.Instance.Tint = loadedVariables?.Tint ?? 0f;
                    GlobalVariables.Instance.TintActive = loadedVariables?.TintActive ?? false;

                    GlobalVariables.Instance.Shadows = loadedVariables?.Shadows ?? 0f;
                    GlobalVariables.Instance.ShadowsActive = loadedVariables?.ShadowsActive ?? false;
                    GlobalVariables.Instance.Midtones = loadedVariables?.Midtones ?? 0f;
                    GlobalVariables.Instance.MidtonesActive = loadedVariables?.MidtonesActive ?? false;
                    GlobalVariables.Instance.Highlights = loadedVariables?.Highlights ?? 0f;
                    GlobalVariables.Instance.HighlightsActive = loadedVariables?.HighlightsActive ?? false;

                    // Tonemapping
                    GlobalVariables.Instance.TonemappingMode = loadedVariables?.TonemappingMode ?? TonemappingMode.None;
                    GlobalVariables.Instance.LUTContribution = loadedVariables?.LUTContribution ?? 0f;
                    GlobalVariables.Instance.LUTName = loadedVariables?.LUTName ?? "None";
                    GlobalVariables.Instance.SceneFlowCheckerEnabled = loadedVariables?.SceneFlowCheckerEnabled ?? false;

                    GlobalVariables.Instance.ToeStrengthActive = loadedVariables?.ToeStrengthActive ?? false;
                    GlobalVariables.Instance.ToeStrengthValue = loadedVariables?.ToeStrengthValue ?? 0f;
                    GlobalVariables.Instance.ToeLengthActive = loadedVariables?.ToeLengthActive ?? false;
                    GlobalVariables.Instance.ToeLengthValue = loadedVariables?.ToeLengthValue ?? 0f;

                    GlobalVariables.Instance.shoulderStrengthActive = loadedVariables?.shoulderStrengthActive ?? false;
                    GlobalVariables.Instance.shoulderStrengthValue = loadedVariables?.shoulderStrengthValue ?? 0f;
                    GlobalVariables.Instance.shoulderLengthActive = loadedVariables?.shoulderLengthActive ?? false;
                    GlobalVariables.Instance.shoulderAngleActive = loadedVariables?.shoulderAngleActive ?? false;
                    GlobalVariables.Instance.shoulderAngleValue = loadedVariables?.shoulderAngleValue ?? 0f;

                    GlobalVariables.Instance.TonemappingGammaActive = loadedVariables?.TonemappingGammaActive ?? false;
                    GlobalVariables.Instance.TonemappingGammaValue = loadedVariables?.TonemappingGammaValue ?? 0f;
                    GlobalVariables.Instance.SaveAutomatically = loadedVariables?.SaveAutomatically ?? true;

                    return loadedVariables;
                }
            }
            catch (Exception)
            {
                Mod.Log.Info("Failed to load Lumina settings. Ensure that at least one setting is set.");
                NotificationSystem.Push(
                    "lumina",
                    thumbnail: "https://i.imgur.com/C9fZDiA.png",
                    title: "Lumina",
                    text: $"Please set a setting or verify the settings file.");

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