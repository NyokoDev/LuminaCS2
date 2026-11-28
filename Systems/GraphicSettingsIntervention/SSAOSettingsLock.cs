using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Colossal;
using HarmonyLib;
using LuminaMod.XML;

namespace Lumina.Systems
{
    internal static class SSAOSettingsLock
    {
        private static Type s_SSAOType;
        internal static readonly IDictionarySource OwnershipLocale = new OwnershipLocaleSource();

        private sealed class OwnershipLocaleSource : IDictionarySource
        {
            public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
            {
                if (s_SSAOType == null) yield break;
                var map = AccessTools.Field(s_SSAOType, "s_SettingsMap")?.GetValue(null) as IDictionary;
                if (map == null) yield break;
                object settings = null;
                foreach (DictionaryEntry entry in map)
                {
                    if (s_SSAOType.IsInstanceOfType(entry.Value)) { settings = entry.Value; break; }
                }
                if (settings == null) yield break;
                var label = AccessTools.Method(s_SSAOType, "GetOptionLabelLocaleID", new[] { typeof(string) });
                var desc = AccessTools.Method(s_SSAOType, "GetOptionDescLocaleID", new[] { typeof(string) });
                var property = s_SSAOType.GetProperty("Quality", BindingFlags.Public | BindingFlags.Instance)
                    ?? s_SSAOType.GetProperty("Level", BindingFlags.Public | BindingFlags.Instance);
                if (property == null) yield break;
                var labelKey = label?.Invoke(settings, new object[] { property.Name }) as string;
                var descKey = desc?.Invoke(settings, new object[] { property.Name }) as string;
                if (!string.IsNullOrEmpty(labelKey)) yield return new KeyValuePair<string, string>(labelKey, "Ambient Occlusion Quality (Managed by Lumina when enabled)");
                if (!string.IsNullOrEmpty(descKey)) yield return new KeyValuePair<string, string>(descKey, "When Lumina is enabled, Ambient Occlusion is managed by Lumina. Use Lumina's SSAO panel to adjust these settings. Disable Lumina's master volume to restore native controls.");
            }
            public void Unload() { }
        }

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
