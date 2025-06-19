using HarmonyLib;
using Game.Simulation;

namespace Lumina.Patches
{
    [HarmonyPatch(typeof(PlanetarySystem), "OnCreate")]
    public static class OverrideTime_OnUpdate_Patch
    {
        static void Prefix(PlanetarySystem __instance)
        {
            // Force overrideTime to false before OnUpdate
            __instance.overrideTime = false;
        }
    }
}
