using UnityEngine;
using System;
using System.Reflection;

namespace LuminaMod.API
{
    /// <summary>
    /// Exposes Lumina GlobalVariables via a discoverable GameObject.
    /// Automatically creates itself on Awake to be accessible at runtime.
    /// </summary>
    [DefaultExecutionOrder(-1000)] // Ensures this runs early
    public class LuminaAPI : MonoBehaviour
    {
        public const string API_OBJECT_NAME = "LuminaAPI";

        /// <summary>
        /// Public reference to GlobalVariables for external mods.
        /// </summary>
        public object Data { get; private set; }

        // Singleton instance
        private static LuminaAPI instance;

        private void Awake()
        {
            // Ensure singleton
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            // Ensure GameObject is named correctly and persistent
            gameObject.name = API_OBJECT_NAME;
            DontDestroyOnLoad(gameObject);

            // Dynamically find GlobalVariables in main build via reflection
            Type gvType = Type.GetType("LuminaMod.XML.GlobalVariables, LuminaMod"); // Replace "LuminaMod" with actual assembly
            if (gvType != null)
            {
                PropertyInfo instanceProp = gvType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                if (instanceProp != null)
                    Data = instanceProp.GetValue(null);
            }
        }

        /// <summary>
        /// Example method showing how a consumer could read the data at runtime dynamically.
        /// </summary>
        public static void ExampleConsumerRead()
        {
            var luminaGO = GameObject.Find(API_OBJECT_NAME);
            if (luminaGO == null)
            {
                Debug.LogWarning("LuminaAPI GameObject not found.");
                return;
            }

            Type luminaType = Type.GetType("LuminaMod.API.LuminaAPI, LuminaMod"); // Replace assembly name
            if (luminaType == null)
            {
                Debug.LogWarning("LuminaAPI type not found.");
                return;
            }

            Component apiComponent = luminaGO.GetComponent(luminaType);
            if (apiComponent == null)
            {
                Debug.LogWarning("LuminaAPI component not found on GameObject.");
                return;
            }

            PropertyInfo dataProp = luminaType.GetProperty("Data", BindingFlags.Public | BindingFlags.Instance);
            object data = dataProp?.GetValue(apiComponent);
            if (data == null)
            {
                Debug.LogWarning("GlobalVariables Data is null.");
                return;
            }

            // Example dynamic access
            PropertyInfo lutProp = data.GetType().GetProperty("LUTName");
            PropertyInfo exposureProp = data.GetType().GetProperty("PostExposure");

            if (lutProp != null) Debug.Log("Current LUT: " + lutProp.GetValue(data));
            if (exposureProp != null) Debug.Log("Post Exposure: " + exposureProp.GetValue(data));
        }

        /// <summary>
        /// Ensures a LuminaAPI instance exists in the scene.
        /// Call this from other mods if needed.
        /// </summary>
        public static void EnsureExists()
        {
            if (instance != null) return;

            var existingGO = GameObject.Find(API_OBJECT_NAME);
            if (existingGO != null)
            {
                instance = existingGO.GetComponent<LuminaAPI>();
                if (instance == null)
                    instance = existingGO.AddComponent<LuminaAPI>();
                return;
            }

            var go = new GameObject(API_OBJECT_NAME);
            instance = go.AddComponent<LuminaAPI>();
        }
    }
}
