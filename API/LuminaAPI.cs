using UnityEngine;

namespace LuminaMod.API
{
    /// <summary>
    /// Exposes Lumina GlobalVariables via a discoverable GameObject.
    /// Automatically creates itself if missing.
    /// </summary>
    public class LuminaAPI : MonoBehaviour
    {
        public const string API_OBJECT_NAME = "LuminaAPI";

        /// <summary>
        /// Public reference to GlobalVariables for external mods.
        /// </summary>
        public LuminaMod.XML.GlobalVariables Data;

        private void Awake()
        {
            // Singleton pattern: prevent duplicates
            var existing = GameObject.Find(API_OBJECT_NAME);
            if (existing != null && existing != gameObject)
            {
                Destroy(gameObject);
                return;
            }

            gameObject.name = API_OBJECT_NAME;
            DontDestroyOnLoad(gameObject);

            // Set runtime reference
            Data = LuminaMod.XML.GlobalVariables.Instance;
        }

        /// <summary>
        /// Ensures the LuminaAPI GameObject exists in the scene and returns its component.
        /// </summary>
        public static LuminaAPI EnsureExists()
        {
            var go = GameObject.Find(API_OBJECT_NAME);
            if (go != null)
            {
                var api = go.GetComponent<LuminaAPI>();
                if (api != null) return api;
                return go.AddComponent<LuminaAPI>();
            }

            // Create new GameObject if missing
            var newGO = new GameObject(API_OBJECT_NAME);
            var newAPI = newGO.AddComponent<LuminaAPI>();
            DontDestroyOnLoad(newGO);
            return newAPI;
        }

        /// <summary>
        /// Example consumer read method.
        /// </summary>
        public static void ExampleConsumerRead()
        {
            var api = EnsureExists();
            if (api.Data != null)
            {
                Lumina.Mod.Log.Info("API Test: Current LUT: " + api.Data.LUTName);
                Lumina.Mod.Log.Info("API Test: " + api.Data.PostExposure);
            }
            else
            {
                Lumina.Mod.Log.Info("Lumina GlobalVariables not available.");
            }
        }
    }
}
