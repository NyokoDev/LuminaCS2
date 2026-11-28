using System;
using System.Runtime.CompilerServices;
using Game.UI.Localization;
using Game.UI.Widgets;
using HarmonyLib;
using LuminaMod.XML;

namespace Lumina.Systems
{
    internal static class SSAOSettingsLock
    {
        private static bool s_Installed;
        private static readonly ConditionalWeakTable<Widget, object> s_Configured = new();
        private static bool OwnsSSAO => s_Installed && GlobalVariables.Instance.LuminaVolumeEnabled;

        internal static void Install(Harmony harmony)
        {
            try
            {
                var target = AccessTools.Method(typeof(Widget), "UpdateBase", Type.EmptyTypes);
                if (target == null)
                {
                    Mod.Log.Warn("[SSAO] Widget.UpdateBase not found.");
                    return;
                }

                harmony.Patch(target, prefix: new HarmonyMethod(typeof(SSAOSettingsLock), nameof(ConfigureWidget))
                {
                    priority = Priority.First
                });
                s_Installed = true;
                Mod.Log.Info("[SSAO] Native SSAO widget update hook installed.");
            }
            catch (Exception exception)
            {
                Mod.Log.Warn($"[SSAO] Native settings lock unavailable: {exception.Message}");
            }
        }

        internal static void Release()
        {
            s_Installed = false;
        }

        private static bool EndsWithPath(string path, string name)
        {
            return string.Equals(path, name, StringComparison.Ordinal)
                || path.EndsWith("." + name, StringComparison.Ordinal);
        }

        private static void ConfigureWidget(Widget __instance)
        {
            if (!s_Installed || !(__instance is NamedWidget widget) || s_Configured.TryGetValue(widget, out _))
                return;

            string path = widget.path.ToString();
            string id = widget.displayName.id ?? string.Empty;
            if (string.IsNullOrEmpty(path))
                return;

            bool quality = EndsWithPath(path, "SSAOQualitySettings")
                || EndsWithPath(path, "SSAOQualitySettings.Level")
                || id == "Options.OPTION[SSAOQualitySettings]";
            bool advanced = EndsWithPath(path, "SSAOQualitySettings.maxPixelRadius")
                || EndsWithPath(path, "SSAOQualitySettings.fullscreen")
                || EndsWithPath(path, "SSAOQualitySettings.stepCount");
            if (!quality && !advanced)
                return;

            try
            {
                var originalDisabled = widget.disabled;
                var originalDescription = widget.description;
                var originalDescriptionAction = widget.descriptionAction;
                widget.disabled = () => OwnsSSAO || (originalDisabled?.Invoke() ?? false);
                widget.descriptionAction = () => OwnsSSAO
                    ? LocalizedString.Value("Managed by Lumina. Use Lumina's SSAO panel to adjust these settings.")
                    : originalDescriptionAction?.Invoke() ?? originalDescription;

                if (quality)
                {
                    var originalName = widget.displayName;
                    var originalNameAction = widget.displayNameAction;
                    widget.displayNameAction = () => OwnsSSAO
                        ? LocalizedString.Value("Ambient Occlusion Quality (Managed by Lumina)")
                        : originalNameAction?.Invoke() ?? originalName;
                }

                s_Configured.Add(widget, new object());
                Mod.Log.Info($"[SSAO] Native SSAO widget configured: {path}, id={id}, quality={quality}");
            }
            catch (Exception exception)
            {
                Mod.Log.Warn($"[SSAO] Native SSAO widget configuration failed: {exception.Message}");
                s_Configured.Add(widget, new object());
            }
        }
    }
}
