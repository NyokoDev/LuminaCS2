using UnityEngine;

namespace LuminaMod.API
{
    /// <summary>
    /// Exposes Lumina GlobalVariables to other mods via a discoverable GameObject.
    /// </summary>
    public class LuminaAPI : MonoBehaviour
    {
        public const string API_OBJECT_NAME = "LuminaAPI";

        /// <summary>
        /// Runtime GlobalVariables reference.
        /// </summary>
        public LuminaMod.XML.GlobalVariables Data
            => LuminaMod.XML.GlobalVariables.Instance;

        private void Awake()
        {
            // Prevent duplicates
            if (GameObject.Find(API_OBJECT_NAME) != null && gameObject.name != API_OBJECT_NAME)
            {
                Destroy(gameObject);
                return;
            }

            gameObject.name = API_OBJECT_NAME;
            DontDestroyOnLoad(gameObject);
        }
    }
}
