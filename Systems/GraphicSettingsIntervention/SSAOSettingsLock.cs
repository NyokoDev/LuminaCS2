using System;
using System.Reflection;
using HarmonyLib;
using LuminaMod.XML;

namespace Lumina.Systems
{
    internal static class SSAOSettingsLock
    {
        private static Type s_SSAOType;

        internal static void Install(Harmony harmony)
        {
            try
            {
                s_SSAOType = AccessTools.TypeByName("Game.Settings.SSAOQualitySettings");
                if (s_SSAOType == null)
                {
                    Mod.Log.Warn("[SSAO] Native SSAO settings type not found.");
                    return;
                }

                var postfix = new HarmonyMethod(typeof(SSAOSettingsLock), nameof(ForceDisabled))
                {
                    priority = Priority.Last
                };

                var options = AccessTools.Method(s_SSAOType, "IsOptionsDisabled", Type.EmptyTypes);
                var quality = AccessTools.Method(s_SSAOType, "IsOptionFullyDisabled", Type.EmptyTypes);

                if (IsValidCondition(options))
                    harmony.Patch(options, postfix: postfix);
                else
                    Mod.Log.Warn("[SSAO] Native advanced SSAO disable condition not found.");

                if (quality != options && IsValidCondition(quality))
                    harmony.Patch(quality, postfix: postfix);
                else
                    Mod.Log.Warn("[SSAO] Native SSAO quality selector disable condition not found.");

                Mod.Log.Info($"[SSAO] Native settings lock: advanced={IsValidCondition(options)}, quality={IsValidCondition(quality)}.");
            }
            catch (Exception exception)
            {
                Mod.Log.Warn($"[SSAO] Native settings lock unavailable: {exception.Message}");
            }
        }

        private static bool IsValidCondition(MethodInfo method)
        {
            return method != null && !method.IsStatic && !method.IsAbstract
                && method.ReturnType == typeof(bool);
        }

        private static void ForceDisabled(object __instance, ref bool __result)
        {
            if (GlobalVariables.Instance.LuminaVolumeEnabled
                && s_SSAOType != null && s_SSAOType.IsInstanceOfType(__instance))
                __result = true;
        }
    }
}
