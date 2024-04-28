using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine.Rendering;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using Lumina;
using System.Threading;

namespace LuminaMod.XML
{
    [Serializable]
    public class GlobalVariables
    {
        /// <summary>
        /// PostExposure
        /// </summary>
        [XmlElement]
        public float PostExposure { get; set; }

        [XmlElement]
        public float Contrast { get; set; }

        [XmlElement]
        public float hueShift { get; set; }

        [XmlElement]
        public float Saturation { get; set; }

        [XmlElement]
        public float Longitude { get; set; }

        [XmlElement]
        public float Latitude { get; set; }

        [XmlElement]
        public float Temperature { get; set; }

        [XmlElement]
        public float Tint { get; set; }

#if DEBUG
        [XmlElement]
        public Vector4Parameter Shadows { get; set; }

        [XmlElement]
        public Vector4Parameter Midtones { get; set; }

        [XmlElement]
        public Vector4Parameter Highlights { get; set; }

        [XmlElement]
        public MinFloatParameter ShadowsStart { get; set; }

        [XmlElement]
        public MinFloatParameter ShadowsEnd { get; set; }

        [XmlElement]
        public MinFloatParameter highlightsStart { get; set; }

        [XmlElement]
        public MinFloatParameter highlightsEnd { get; set; }

#endif

        public static void SaveToFile(string filePath)
        {
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
                    Mod.log.Info($"Sharing violation encountered. Retrying in 1 second...");
                    Thread.Sleep(1000); // Wait for 1 second before retrying
                    retries++;
                }
                catch (Exception ex)
                {
                    Mod.log.Info($"Error saving GlobalVariables to file: {ex.Message}");
                    break; // Break out of the loop if an unexpected error occurs
                }
            }

            if (retries >= maxRetries)
            {
                Mod.log.Info($"Failed to save GlobalVariables to file after {maxRetries} retries.");
            }
        }


        public static GlobalVariables LoadFromFile(string filePath)
        {
            try
            {
                // Create an XmlSerializer for the GlobalVariables type.
                XmlSerializer serializer = new XmlSerializer(typeof(GlobalVariables));

                // Open the file for reading.
                using (TextReader reader = new StreamReader(filePath))
                {
                    // Deserialize the object from the file.
                    GlobalVariables loadedVariables = (GlobalVariables)serializer.Deserialize(reader);

                    // Set the loaded values to the corresponding properties.
                    GlobalVariables.Instance.PostExposure = loadedVariables.PostExposure;
                    GlobalVariables.Instance.Contrast = loadedVariables.Contrast;
                    GlobalVariables.Instance.hueShift = loadedVariables.hueShift;
                    GlobalVariables.Instance.Saturation = loadedVariables.Saturation;

                    GlobalVariables.Instance.Latitude = loadedVariables.Latitude;
                    GlobalVariables.Instance.Longitude = loadedVariables.Longitude; 

                    GlobalVariables.Instance.Temperature = loadedVariables.Temperature;
                    GlobalVariables.Instance.Tint = loadedVariables.Tint;

             




                    return loadedVariables;
                }
            }
            catch (Exception ex)
            {
                Mod.log.Info("Failed to load Lumina settings. Ensure that at least one setting is set.");
                return null;
            }
        }


        // Singleton pattern to ensure only one instance of GlobalVariables exists.
        private static GlobalVariables instance;
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