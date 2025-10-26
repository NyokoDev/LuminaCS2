using LuminaMod.XML;
using Unity.Entities;
using UnityEngine;

namespace Lumina.API
{
    /// <summary>
    /// ECS system that exposes Lumina GlobalVariables via a DataContainer.
    /// Other systems can safely read from Data.
    /// </summary>
    internal partial class LuminaAPI : SystemBase
    {
        /// <summary>
        /// Container holding the GlobalVariables instance.
        /// </summary>
        public class DataContainer
        {
            /// <summary>
            /// The Lumina GlobalVariables instance.
            /// </summary>
            public GlobalVariables GlobalVariables { get; internal set; }
        }

        /// <summary>
        /// Exposed Data object that other systems can read safely.
        /// </summary>
        public DataContainer Data { get; private set; } = new DataContainer();

        /// <summary>
        /// Direct reference to the GlobalVariables instance.
        /// </summary>
        private GlobalVariables globalVariables;

        protected override void OnCreate()
        {
            base.OnCreate();

            // Initialize or load GlobalVariables
            globalVariables = GlobalVariables.Instance; // Assuming a singleton pattern
            if (globalVariables == null)
            {
                Debug.LogWarning("[LuminaAPISystem] GlobalVariables instance not found!");
            }

            Data.GlobalVariables = globalVariables;
        }

        protected override void OnUpdate()
        {
            // Refresh Data every frame if GlobalVariables could change
            Data.GlobalVariables = globalVariables;
        }
    }
}
