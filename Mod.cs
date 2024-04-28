using Lumina.Systems;
using Colossal.IO.AssetDatabase;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.Rendering.CinematicCamera;
using Game.Rendering;
using Game.SceneFlow;
using Game.UI.InGame;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;

namespace Lumina
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(Lumina)}.{nameof(Mod)}").SetShowsErrorsInUI(false);
        private Setting m_Setting;
        private Harmony? _harmony;

        public void OnLoad(UpdateSystem updateSystem)
        {

            _harmony = new($"{nameof(Lumina)}.{nameof(Mod)}");
            _harmony.PatchAll(typeof(Mod).Assembly);
            Mod.log.Info("Ran PatchAll.");
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info($"Mod asset location {asset.path}");

            m_Setting = new Setting(this);
            m_Setting.RegisterInOptionsUI();
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(m_Setting));

            AssetDatabase.global.LoadSettings(nameof(Lumina), m_Setting, new Setting(this));


            updateSystem.UpdateAfter<PostProcessSystem>(SystemUpdatePhase.GameSimulation);
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));

            _harmony?.UnpatchAll($"{nameof(Lumina)}.{nameof(Mod)}");

            if (m_Setting != null)
            {
                m_Setting.UnregisterInOptionsUI();
                m_Setting = null;
            }
        }


            [HarmonyPatch(typeof(PhotoModeUISystem), nameof(PhotoModeUISystem.Activate))]
            [HarmonyPrefix]
            public static bool OverrideActivate(PhotoModeRenderSystem ___m_PhotoModeRenderSystem)
            {
                ___m_PhotoModeRenderSystem.Enable(true);
                return false;
            }

            [HarmonyPatch(typeof(PhotoModeRenderSystem), nameof(PhotoModeRenderSystem.DisableAllCameraProperties))]
            [HarmonyPrefix]
            public static bool DontDisableAllCameraPropertiesPlease(Game.Rendering.PhotoModeRenderSystem __instance)
            {
                foreach (KeyValuePair<string, PhotoModeProperty> photoModeProperty in __instance.photoModeProperties)
                {
                    Mod.log.Info($"Property Name: {photoModeProperty.Key}, Enabled: {photoModeProperty.Value.isEnabled}");
                    photoModeProperty.Value.setEnabled?.Invoke(obj: true);
                }

                return false;
            }
        }
    }
