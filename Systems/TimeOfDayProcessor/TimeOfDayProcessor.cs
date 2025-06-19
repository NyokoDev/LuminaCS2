namespace Lumina.Systems
{
    using Colossal.Serialization.Entities;
    using Game;
    using Game.Rendering;
    using Game.Simulation;
    using LuminaMod.XML;
    using System;
    using Unity.Entities;
    using UnityEngine;

    /// <summary>
    /// Handles time of day.
    /// </summary>
    internal partial class TimeOfDayProcessor : SystemBase
    {
        /// <summary>
        /// Determines if the user allows Time of Day to be locked.
        /// </summary>
        public static bool Locked;

        /// <summary>
        /// Float for the time to be changed.
        /// </summary>
        public static float TimeFloat;

        /// <summary>
        /// Planetary system instance.
        /// </summary>
        public PlanetarySystem? PlanetarySystem;

        /// <summary>
        /// Called when the system is created.
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();



            DisableWater(); // Disable water rendering system in debug mode to avoid lag while testing UI.



            Locked = GlobalVariables.Instance.TimeOfDayLocked; // Get the initial value from global variables.

            PlanetarySystem = World.GetExistingSystemManaged<PlanetarySystem>();
            if (PlanetarySystem == null)
            {
                Lumina.Mod.Log.Info("[Lumina] TimeOfDayProcessor: PlanetarySystem not found. Time of day features will be disabled.");
            }
        }


        private void DisableWater()
        {
            WaterRenderSystem waterRenderSystem = World.GetExistingSystemManaged<WaterRenderSystem>();
            if (waterRenderSystem != null)
            {
                waterRenderSystem.Enabled = false;
                Mod.Log.Info("WaterRenderSystem disabled successfully.");
            }
            else
            {
                Mod.Log.Info("WaterRenderSystem not found, skipping disable.");
            }
        }


        /// <summary>
        /// Called every frame to update the system.
        /// </summary>
        protected override void OnUpdate()
        {
            if (Locked)
            {
                PlanetarySystem.overrideTime = true;
                float lerpSpeed = Mathf.Max(1.5f * UnityEngine.Time.deltaTime, 0.001f);
                PlanetarySystem.time = Mathf.Lerp(PlanetarySystem.time, TimeFloat, lerpSpeed);
            }
        }
    }
}
