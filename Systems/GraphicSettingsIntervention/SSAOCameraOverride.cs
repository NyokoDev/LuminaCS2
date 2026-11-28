using System;
using System.Linq;
using HarmonyLib;
using LuminaMod.XML;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace Lumina.Systems
{
    internal static class SSAOCameraOverride
    {
        internal static void Install(Harmony harmony)
        {
            try
            {
                var target = AccessTools.GetDeclaredMethods(typeof(HDCamera))
                    .FirstOrDefault(method =>
                    {
                        var parameters = method.GetParameters();
                        return method.Name == "Update" && parameters.Length > 0
                            && parameters[0].ParameterType == typeof(FrameSettings);
                    });

                if (target == null)
                {
                    Mod.Log.Warn("[SSAO] HDCamera.Update not found; using volume overrides only.");
                    return;
                }

                var prefix = new HarmonyMethod(typeof(SSAOCameraOverride), nameof(Prefix))
                {
                    priority = Priority.Last
                };
                var postfix = new HarmonyMethod(typeof(SSAOCameraOverride), nameof(Postfix))
                {
                    priority = Priority.Last
                };

                harmony.Patch(target, prefix: prefix, postfix: postfix);
                Mod.Log.Info("[SSAO] Lumina camera frame settings and resolved volume overrides installed.");
            }
            catch (Exception exception)
            {
                Mod.Log.Warn($"[SSAO] Camera override unavailable: {exception.Message}");
            }
        }

        private static bool OwnsCamera(HDCamera camera)
        {
            var volume = RenderEffectsSystem.LuminaVolume;
            return volume != null && volume.isActiveAndEnabled
                && GlobalVariables.Instance.LuminaVolumeEnabled
                && camera != null && camera.camera != null
                && camera.camera.cameraType == CameraType.Game;
        }

        private static void Prefix(HDCamera __instance, ref FrameSettings __0)
        {
            if (!OwnsCamera(__instance))
                return;

            var asset = GraphicsSettings.currentRenderPipeline as HDRenderPipelineAsset;
            bool supported = asset != null && asset.currentPlatformRenderPipelineSettings.supportSSAO
                && __0.IsEnabled(FrameSettingsField.OpaqueObjects);
            __0.SetEnabled(FrameSettingsField.SSAO, supported && GlobalVariables.Instance.IsScreenSpaceAmbientOcclusion);
        }

        private static void Postfix(HDCamera __instance)
        {
            if (!OwnsCamera(__instance) || __instance.volumeStack == null)
                return;

            var ao = __instance.volumeStack.GetComponent<ScreenSpaceAmbientOcclusion>();
            RenderEffectsSystem.ApplyAmbientOcclusion(ao);
        }
    }
}
