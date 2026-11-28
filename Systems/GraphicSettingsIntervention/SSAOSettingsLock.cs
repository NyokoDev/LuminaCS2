using System;
using System.Reflection;
using Game.UI.Localization;
using HarmonyLib;
using LuminaMod.XML;

namespace Lumina.Systems
{
    internal static class SSAOSettingsLock
    {
        private static Type s_SSAOType;

        private static bool OwnsSSAO => GlobalVariables.Instance.LuminaVolumeEnabled;

        internal static void Install(Harmony harmony)
        {
            try
            {
                s_SSAOType = AccessTools.TypeByName("Game.Settings.SSAOQualitySettings");
                var itemType = AccessTools.TypeByName("Game.UI.Menu.AutomaticSettings+SettingItemData");
                if (s_SSAOType == null || itemType == null)
                {
                    Mod.Log.Warn("[SSAO] Native settings types not found.");
                    return;
                }

                var constructor = Array.Find(itemType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance),
                    candidate =>
                    {
                        var parameters = candidate.GetParameters();
                        return parameters.Length == 4 && parameters[3].ParameterType == typeof(string);
                    });

                if (constructor == null)
                {
                    Mod.Log.Warn("[SSAO] SettingItemData constructor not found.");
                    return;
                }

                harmony.Patch(constructor, postfix: new HarmonyMethod(typeof(SSAOSettingsLock), nameof(ConfigureItem))
                {
                    priority = Priority.Last
                });

                Mod.Log.Info("[SSAO] Native SSAO settings item lock and ownership label installed.");
            }
            catch (Exception exception)
            {
                Mod.Log.Warn($"[SSAO] Native settings lock unavailable: {exception.Message}");
            }
        }

        private static void ConfigureItem(object __instance)
        {
            try
            {
                var type = __instance.GetType();
                var setting = AccessTools.Property(type, "setting")?.GetValue(__instance);
                if (setting == null || !s_SSAOType.IsInstanceOfType(setting))
                    return;

                var property = AccessTools.Property(type, "property")?.GetValue(__instance);
                if (property == null)
                    return;

                var name = AccessTools.Property(property.GetType(), "name")?.GetValue(property) as string;
                if (name != "Level" && name != "maxPixelRadius" && name != "fullscreen" && name != "stepCount")
                    return;

                var disable = AccessTools.Property(type, "disableAction");
                var originalDisable = disable.GetValue(__instance) as Func<bool>;
                disable.SetValue(__instance, (Func<bool>)(() => OwnsSSAO || (originalDisable?.Invoke() ?? false)));

                var description = AccessTools.Property(type, "description");
                var descriptionAction = AccessTools.Property(type, "descriptionAction");
                var originalDescription = (LocalizedString)description.GetValue(__instance);
                var originalDescriptionAction = descriptionAction.GetValue(__instance) as Func<LocalizedString>;
                descriptionAction.SetValue(__instance, (Func<LocalizedString>)(() => OwnsSSAO
                    ? LocalizedString.Value("Managed by Lumina. Use Lumina's SSAO panel to adjust these settings.")
                    : originalDescriptionAction?.Invoke() ?? originalDescription));

                if (name == "Level")
                {
                    var displayName = AccessTools.Property(type, "displayName");
                    var displayNameAction = AccessTools.Property(type, "dispayNameAction");
                    var originalName = (LocalizedString)displayName.GetValue(__instance);
                    var originalNameAction = displayNameAction.GetValue(__instance) as Func<LocalizedString>;
                    displayNameAction.SetValue(__instance, (Func<LocalizedString>)(() => OwnsSSAO
                        ? LocalizedString.Value("Ambient Occlusion Quality — Managed by Lumina")
                        : originalNameAction?.Invoke() ?? originalName));
                }

                Mod.Log.Info($"[SSAO] Native settings item configured: {name}");
            }
            catch (Exception exception)
            {
                Mod.Log.Warn($"[SSAO] Could not configure native settings item: {exception.Message}");
            }
        }
    }
}
