using Colossal.Serialization.Entities;
using Game;
using Game.Simulation;
using LuminaMod.XML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Lumina.Systems
{
    partial class TimeOfDayProccessor : SystemBase
    {
        public PlanetarySystem _planetarySystem;
        public static bool Locked;
        public static float TimeFloat;
        protected override void OnCreate()
        {
            base.OnCreate();
            _planetarySystem = World.GetExistingSystemManaged<PlanetarySystem>();

        }
        protected override void OnUpdate()
        {
            if (Locked)
            {
                _planetarySystem.overrideTime = true;
                _planetarySystem.time = Mathf.Lerp(_planetarySystem.time, TimeFloat, 1.5f * UnityEngine.Time.deltaTime);
            }
        }
    }
}
