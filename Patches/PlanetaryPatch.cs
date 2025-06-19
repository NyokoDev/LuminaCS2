using System.Collections.Generic;
using System.Reflection.Emit;
using Game.Simulation;
using HarmonyLib;
using LuminaMod.XML;

namespace Lumina.Patches
{
    [HarmonyPatch(typeof(PlanetarySystem), "OnUpdate")]
    public static class PlanetarySystem_OnUpdate_Patch
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);

            var getInstance = AccessTools.PropertyGetter(typeof(GlobalVariables), nameof(GlobalVariables.Instance));
            var getLatitude = AccessTools.PropertyGetter(typeof(GlobalVariables), nameof(GlobalVariables.Latitude));
            var getLongitude = AccessTools.PropertyGetter(typeof(GlobalVariables), nameof(GlobalVariables.Longitude));

            bool replacedLatitude = false;
            bool replacedLongitude = false;

            for (int i = 0; i < codes.Count; i++)
            {
                var code = codes[i];

                // Look for loading the local variable or loading the original field for latitude (local 0)
                if (!replacedLatitude &&
                    (code.opcode == OpCodes.Ldloc_0 ||
                    (code.opcode == OpCodes.Ldloc_S && code.operand is LocalBuilder lb1 && lb1.LocalIndex == 0)))
                {
                    // Replace this load with calls to GlobalVariables.Instance.Latitude
                    codes[i] = new CodeInstruction(OpCodes.Call, getInstance);
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, getLatitude));
                    replacedLatitude = true;
                }
                // Same for longitude (local 1)
                else if (!replacedLongitude &&
                    (code.opcode == OpCodes.Ldloc_1 ||
                    (code.opcode == OpCodes.Ldloc_S && code.operand is LocalBuilder lb2 && lb2.LocalIndex == 1)))
                {
                    codes[i] = new CodeInstruction(OpCodes.Call, getInstance);
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, getLongitude));
                    replacedLongitude = true;
                }
            }

            return codes;
        }
    }
}
