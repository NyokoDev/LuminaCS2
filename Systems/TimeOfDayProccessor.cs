namespace Lumina.Systems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Colossal.Serialization.Entities;
    using Game;
    using Game.Simulation;
    using LuminaMod.XML;
    using Unity.Entities;
    using UnityEngine;

    /// <summary>
    /// Handles time of day.
    /// </summary>
    internal partial class TimeOfDayProccessor : SystemBase
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
        public PlanetarySystem PlanetarySystem;

        /// <summary>
        /// Called when the system is created.
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();
            PlanetarySystem = World.GetExistingSystemManaged<PlanetarySystem>();
        }

        /// <summary>
        /// OnUpdate method helper.
        /// </summary>
        protected override void OnUpdate()
        {
            if (Locked)
            {
                PlanetarySystem.overrideTime = true;
                PlanetarySystem.time = Mathf.Lerp(PlanetarySystem.time, TimeFloat, 1.5f * UnityEngine.Time.deltaTime);
            }
        }
    }
}
