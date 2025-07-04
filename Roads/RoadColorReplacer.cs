using Lumina.Systems;
using RoadWearAdjuster.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace Lumina.Roads
{
    internal partial class RoadColorReplacer : SystemBase
    {
        ReplaceRoadWearSystem textureReplacer;

        public static void UpdateColors()
        {
            ReplaceRoadWearSystem.UpdateColors();
        }
        protected override void OnUpdate()
        {
            
        }
    }
}
