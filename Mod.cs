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
using LuminaMod.XML;
using Lumina.XML;
using Game.PSI;
using static System.Net.WebRequestMethods;

namespace Lumina
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(Lumina)}.{nameof(Mod)}").SetShowsErrorsInUI(false);
        private Setting m_Setting;
        private Harmony? _harmony;

        public void OnLoad(UpdateSystem updateSystem)
        {
            // Initialize Harmony for patching
            _harmony = new Harmony($"{nameof(Lumina)}.{nameof(Mod)}");
            _harmony.PatchAll(typeof(Mod).Assembly);
            Mod.log.Info("Patching completed successfully.");
            log.Info(nameof(OnLoad));

            // Log mod asset location if found
            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info($"Mod asset location: {asset.path}");

            // Initialize and register mod settings
            m_Setting = new Setting(this);
            m_Setting.RegisterInOptionsUI();
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(m_Setting));

            // Update system after PostProcessSystem but before culling
            updateSystem.UpdateAfter<PostProcessSystem>(SystemUpdatePhase.PreCulling);

            try
            {
                // Load global settings
                GlobalVariables.LoadFromFile(GlobalPaths.GlobalModSavingPath);
            }
            catch
            {
                // Notify if failed to retrieve Lumina settings
                NotificationSystem.Push(
        "mod-check",
        thumbnail: "https://i.imgur.com/C9fZDiA.png",
        title: "Lumina",
        text: $"Exception attempting to retrieve Lumina settings."
    );

            }
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
    }
}