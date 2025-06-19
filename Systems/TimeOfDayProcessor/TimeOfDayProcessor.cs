using Colossal.Serialization.Entities;
using Game;
using Game.Common;
using Game.Rendering;
using Game.Simulation;
using HarmonyLib;
using Lumina.Patches;
using LuminaMod.XML;
using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Lumina.Systems
{
    internal partial class TimeOfDayProcessor : SystemBase
    {
        public static bool Locked;
        public static float TimeFloat;
        public static bool ChangingTime;

        public TimeSystem TimeSystem;
        public PlanetarySystem PlanetarySystem;
        public SimulationSystem SimulationSystem;
        public RenderingSystem RenderingSystem;

        public bool AllDone { get; private set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            SimulationSystem = World.GetExistingSystemManaged<SimulationSystem>();
            PlanetarySystem = World.GetExistingSystemManaged<PlanetarySystem>();
            RenderingSystem = World.GetExistingSystemManaged<RenderingSystem>();
        }

        protected override void OnUpdate()
        {
            ChangingTime = GlobalVariables.Instance.ChangingTime;

            if (!GlobalVariables.Instance.ViewTimeOfDaySlider)
                return;

            if (ChangingTime)
            {
                PlanetarySystem.overrideTime = true;

                // Smoothly interpolate towards target time
                float lerpSpeed = math.max(0.0005f * UnityEngine.Time.captureDeltaTime, 0.001f);
                PlanetarySystem.time = math.lerp(PlanetarySystem.time, TimeFloat, lerpSpeed);

                PlanetarySystem.minute = (int)math.lerp(PlanetarySystem.time, TimeFloat, lerpSpeed);
                PlanetarySystem.hour = (int)math.lerp(PlanetarySystem.time, TimeFloat, lerpSpeed);

                // If we're close enough, snap to the target and mark as done
                if (math.abs(PlanetarySystem.time - TimeFloat) < 0.001f)
                {
                    PlanetarySystem.time = TimeFloat;
              
                    PlanetarySystem.minute = (int)math.lerp(PlanetarySystem.time, TimeFloat, lerpSpeed);
                    PlanetarySystem.hour = (int)math.lerp(PlanetarySystem.time, TimeFloat, lerpSpeed);
                    AllDone = true;
                }

                if (AllDone)
                {
                    PlanetarySystem.overrideTime = false;
                    AllDone = false;
                    GlobalVariables.Instance.ChangingTime = false;
                }

                Lumina.Mod.Log.Info($"Smoothly changing time to {TimeFloat:F2}");
            }
            else if (Locked)
            {
                float lerpSpeed = math.max(0.0005f * UnityEngine.Time.captureDeltaTime, 0.001f);
                PlanetarySystem.overrideTime = true;
                PlanetarySystem.time = TimeFloat;
                PlanetarySystem.minute = (int)math.lerp(PlanetarySystem.time, TimeFloat, lerpSpeed);
                PlanetarySystem.hour = (int)math.lerp(PlanetarySystem.time, TimeFloat, lerpSpeed);
            }
            else
            {
                PlanetarySystem.overrideTime = false;
            }
        }
    }
}
