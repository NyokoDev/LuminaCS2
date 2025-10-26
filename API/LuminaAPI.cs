using UnityEngine;

namespace LuminaMod.API
{
    /// <summary>
    /// Exposes Lumina GlobalVariables via a discoverable GameObject.
    /// Automatically creates itself on load to be accessible at runtime.
    /// </summary>
    [DefaultExecutionOrder(-1000)] // Ensures this runs early
    public class LuminaAPI : MonoBehaviour
    {
        public const string API_OBJECT_NAME = "LuminaAPI";

        /// <summary>
        /// Public reference to GlobalVariables for external mods.
        /// </summary>
        public LuminaMod.XML.GlobalVariables Data { get; private set; }

        // Singleton instance
        private static LuminaAPI instance;

        /// <summary>
        /// Ensure the object exists automatically when the mod loads.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EnsureExists()
        {
            if (instance != null) return;

            // Try to find existing GameObject first
            var existingGO = GameObject.Find(API_OBJECT_NAME);
            if (existingGO != null)
            {
                instance = existingGO.GetComponent<LuminaAPI>();
                if (instance == null)
                {
                    instance = existingGO.AddComponent<LuminaAPI>();
                }
                return;
            }

            // Create the GameObject and attach this component
            var go = new GameObject(API_OBJECT_NAME);
            instance = go.AddComponent<LuminaAPI>();
            DontDestroyOnLoad(go);
        }

        private void Awake()
        {
            // Prevent duplicates
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            Data = LuminaMod.XML.GlobalVariables.Instance;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Example method showing how a consumer could read the data at runtime.
        /// </summary>
        public static void ExampleConsumerRead()
        {
            var luminaGO = GameObject.Find(API_OBJECT_NAME);
            if (luminaGO != null)
            {
                var api = luminaGO.GetComponent<LuminaAPI>();
                if (api != null && api.Data != null)
                {
                    Debug.Log("Current LUT: " + api.Data.LUTName);
                    Debug.Log("Post Exposure: " + api.Data.PostExposure);
                }
            }
            else
            {
                Debug.LogWarning("LuminaAPI GameObject not found.");
            }
        }
    }
}
