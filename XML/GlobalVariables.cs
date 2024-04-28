using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine.Rendering;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

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



        public static void SaveToFile(string filePath)
        {
            try
            {
                // Create an XmlSerializer for the GlobalVariables type.
                XmlSerializer serializer = new XmlSerializer(typeof(GlobalVariables));

                // Create or open the file for writing.
                using (TextWriter writer = new StreamWriter(filePath))
                {
                    // Serialize the current static object to the file.
                    serializer.Serialize(writer, Instance);
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving GlobalVariables to file: {ex.Message}");
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

                    GlobalVariables.Instance.Shadows = loadedVariables.Shadows;
                    GlobalVariables.Instance.Midtones = loadedVariables.Midtones;
                    GlobalVariables.Instance.Highlights = loadedVariables.Highlights;
                    GlobalVariables.Instance.ShadowsStart = loadedVariables.ShadowsStart;
                    GlobalVariables.Instance.ShadowsEnd = loadedVariables.ShadowsEnd;
                    GlobalVariables.Instance.highlightsStart = loadedVariables.highlightsStart;
                    GlobalVariables.Instance.highlightsEnd = loadedVariables.highlightsEnd;




                    return loadedVariables;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to load Lumina settings. Ensure that at least one setting is set.");
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