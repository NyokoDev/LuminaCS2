using Game.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace Lumina.Systems.SimulationRefresh
{
    internal partial class RefreshSimulationSystem : SystemBase
    {

        public SimulationSystem? SimulationSystem;

        /// <summary>
        /// Called when the system is created.
        /// </summary>
        protected override void OnCreate()
        {
            base.OnCreate();
 
        }
        public void RefreshSimulation()
        {
            ResumeSimulation();
            }
        
        

        public void ResumeSimulation()
        {
// Pending implementation that resumes the simulation due to Metro Framework apparently breaking the simulation after a notification is sent.
        }

        /// <summary>
        /// Called every frame to update the system.
        /// </summary>
        protected override void OnUpdate()
        {
            // Logic to refresh the simulation can be added here.
            // This could involve resetting certain states, reloading data, etc.
        }
    }
    
}

