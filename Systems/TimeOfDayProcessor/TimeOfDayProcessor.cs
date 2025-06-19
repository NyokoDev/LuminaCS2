using Colossal.Serialization.Entities;
using Game;
using Game.Common;
using Game.Rendering;
using Game.Simulation;
using LuminaMod.XML;
using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Lumina.Systems
{
    internal partial class TimeOfDayProcessor : GameSystemBase
    {
        public static bool Locked;
        public static float TimeFloat;
        public static bool ChangingTime = GlobalVariables.Instance.ChangingTime;

        public SimulationSystem SimulationSystem;
        private EntityQuery _timeDataQuery;
        private bool isLerping = false;
        private bool offsetSet = false; // Track if we set offset on this lock

        protected override void OnCreate()
        {
            base.OnCreate();
           SimulationSystem = World.GetExistingSystemManaged<SimulationSystem>();
        _timeDataQuery = GetEntityQuery(ComponentType.ReadWrite<TimeData>());
            RequireForUpdate(_timeDataQuery);
        }

        const int kTicksPerDay = 262144;

        private bool timeChanged = false;

        protected override void OnUpdate()
        {
            if (_timeDataQuery.IsEmpty)
                return;

            var entity = _timeDataQuery.GetSingletonEntity();
            var data = EntityManager.GetComponentData<TimeData>(entity);

            int currentTicks = (int)(SimulationSystem.frameIndex - data.m_FirstFrame);
            int desiredTicks = Mathf.RoundToInt(TimeFloat / 24f * kTicksPerDay);

            if (ChangingTime)
            {
                // Smoothly interpolate towards the target TimeFloat
                float currentTime = currentTicks / (float)kTicksPerDay * 24f;
                float lerpSpeed = 2f * World.Time.DeltaTime; // Increase this for faster transitions

                float newTime = Mathf.Lerp(currentTime, TimeFloat, lerpSpeed);
                int newDesiredTicks = Mathf.RoundToInt(newTime / 24f * kTicksPerDay);
                data.TimeOffset = newDesiredTicks - currentTicks;
                EntityManager.SetComponentData(entity, data);

                // Stop changing if close enough
                if (Mathf.Abs(newTime - TimeFloat) < 0.01f)
                {
                    ChangingTime = false;
                    Locked = true;
                }
            }
            else if (Locked && !offsetSet)
            {
                // Set once when lock activates
                data.TimeOffset = desiredTicks - currentTicks;
                EntityManager.SetComponentData(entity, data);
                offsetSet = true;
            }
            else if (!Locked && data.TimeOffset != 0)
            {
                // Reset when unlocking
                data.TimeOffset = 0;
                EntityManager.SetComponentData(entity, data);
                offsetSet = false;
            }
        }



        public void StartLerp(float newTime)
        {
            TimeFloat = newTime;
            ChangingTime = true;
            Locked = false; // Locked becomes true only when lerp finishes
            offsetSet = false;
        }

        public void ReleaseLock()
        {
            Locked = false;
            offsetSet = false;
            // Keep current TimeOffset as-is so simulation time continues smoothly
        }
    }
}