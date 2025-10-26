// <copyright file="Mod.cs" company="NyokoDev">
// Copyright (c) NyokoDev. All rights reserved.
// </copyright>

namespace Lumina
{
    using Colossal.Logging;
    using Game;
    using Game.Modding;
    using Game.Prefabs;
    using Game.PSI;
    using Game.Rendering;
    using Game.SceneFlow;
    using Game.Simulation;
    using Game.UI;
    using Game.UI.Localization;
    using HarmonyLib;
    using Lumina.API;
    using Lumina.Locale;
    using Lumina.ManagerSystems;
    using Lumina.Metro;
    using Lumina.Patches;
    using Lumina.Systems;
    using Lumina.XML;
    using LuminaMod.XML;
    using MetroFramework.Forms;
    using RoadWearAdjuster.Systems;
    using System;
    using System.IO;
    using System.Net;
    using System.Windows.Forms;
    using Unity.Entities;
    using UnityEngine;
    using Localization = Locale.Localization;
    using Version = Game.Version;

    /// <summary>
    /// Main mod class.
    /// </summary>
    public class Mod : IMod
    {
        /// <summary>
        /// ModUI to call from React.
        /// </summary>
        public const string MODUI = "Lumina";

        /// <summary>
        /// Main log function.
        /// </summary>
        public static ILog Log = LogManager.GetLogger($"{nameof(Lumina)}.{nameof(Mod)}").SetShowsErrorsInUI(false);
        private Setting setting;
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public static Harmony? harmony;


        public static string ModPath { get; set; }
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

        /// <inheritdoc/>
        public void OnLoad(UpdateSystem updateSystem)
        {
            // Load global settings
            GlobalVariables.EnsureSettingsFileExists(GlobalPaths.GlobalModSavingPath);
            GlobalVariables.LoadFromFile(GlobalPaths.GlobalModSavingPath);

            harmony = new Harmony($"{nameof(Lumina)}.{nameof(Mod)}");

            RunTranspilerPatch();


            // Always patch PhotoModeRenderSystem.OnUpdate postfix
            var photoModeOriginal = AccessTools.Method(typeof(PhotoModeRenderSystem), "OnUpdate");
            var postfixPhotoMode = AccessTools.Method(typeof(PhotoModeRenderSystemPatch), "Postfix");

            harmony.Patch(
                photoModeOriginal,
                postfix: postfixPhotoMode != null ? new HarmonyMethod(postfixPhotoMode) : null
            );
        
    
    Mod.Log.Info("Patching completed successfully.");



            Log.Info(nameof(OnLoad));

            // Log mod asset location if found
            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
            {
                ModPath = asset.path;
                Log.Info($"Mod asset location: {asset.path}");
            }

            // Initialize and register mod settings
            this.setting = new Setting(this);
            this.setting.RegisterInOptionsUI();




            // Load translations.
            LoadSettingsTranslations();
            Localization.LoadTranslations(null, Log);

            CheckVersion();


            updateSystem.UpdateAfter<PreSystem>(SystemUpdatePhase.UIUpdate);


            // Update system after RenderEffectsSystem but before culling
            updateSystem.UpdateAfter<RenderEffectsSystem>(SystemUpdatePhase.PreCulling);

            /// Update system after DisableWaterSystem to ensure water rendering is managed correctly
            updateSystem.UpdateAfter<DisableWaterSystem>(SystemUpdatePhase.MainLoop);

            updateSystem.UpdateAt<UISystem>(SystemUpdatePhase.UIUpdate);

            updateSystem.UpdateAt<TimeOfDayProcessor>(SystemUpdatePhase.GameSimulation);

            updateSystem.UpdateAfter<CubemapUpdateSystem>(SystemUpdatePhase.GameSimulation);

            updateSystem.UpdateAfter<ReplaceRoadWearSystem>(SystemUpdatePhase.GameSimulation);

            updateSystem.UpdateAfter<CustomSunManager>(SystemUpdatePhase.GameSimulation);


            SendNotification();
        }


        public static void RunTranspilerPatch()
        {
            var originalOnUpdate = AccessTools.Method(typeof(PlanetarySystem), "OnUpdate");
            var transpiler = AccessTools.Method(typeof(PlanetarySystem_OnUpdate_Patch), "Transpiler");

            var originalOnCreate = AccessTools.Method(typeof(PlanetarySystem), "OnCreate");
            var prefixOnCreate = AccessTools.Method(typeof(OverrideTime_OnUpdate_Patch), "Prefix");

            if (GlobalVariables.Instance.LatLongEnabled)
            {
                harmony.Patch(
                    originalOnUpdate,
                    transpiler: transpiler != null ? new HarmonyMethod(transpiler) : null
                );

                harmony.Patch(
                    originalOnCreate,
                    prefix: prefixOnCreate != null ? new HarmonyMethod(prefixOnCreate) : null
                );
            }
            else
            {
                // Use harmony ID string instead of MethodInfo
                harmony.Unpatch(originalOnUpdate, HarmonyPatchType.Transpiler, "Lumina.Mod");
                harmony.Unpatch(originalOnCreate, HarmonyPatchType.Prefix, "Lumina.Mod");
            }
        }


        private void LoadSettingsTranslations()
        {
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(setting));
            GameManager.instance.localizationManager.AddSource("es-ES", new LocaleES(setting));
            GameManager.instance.localizationManager.AddSource("fr-FR", new LocaleFR(setting));
            GameManager.instance.localizationManager.AddSource("de-DE", new LocaleDE(setting));
            GameManager.instance.localizationManager.AddSource("it-IT", new LocaleIT(setting));
            GameManager.instance.localizationManager.AddSource("pt-BR", new LocalePT(setting));
            GameManager.instance.localizationManager.AddSource("ru-RU", new LocaleRU(setting));
            GameManager.instance.localizationManager.AddSource("zh-HANS", new LocaleZH(setting));
            GameManager.instance.localizationManager.AddSource("zh-HANT", new LocaleZHTW(setting));
            GameManager.instance.localizationManager.AddSource("ja-JP", new LocaleJA(setting));
            GameManager.instance.localizationManager.AddSource("ko-KR", new LocaleKO(setting));
            GameManager.instance.localizationManager.AddSource("pl-PL", new LocalePL(setting));
            GameManager.instance.localizationManager.AddSource("nl-NL", new LocaleNL(setting));
            GameManager.instance.localizationManager.AddSource("sv-SE", new LocaleSV(setting));
            GameManager.instance.localizationManager.AddSource("tr-TR", new LocaleTR(setting));
            GameManager.instance.localizationManager.AddSource("cs-CZ", new LocaleCS(setting));
            GameManager.instance.localizationManager.AddSource("fi-FI", new LocaleFI(setting));
            GameManager.instance.localizationManager.AddSource("el-GR", new LocaleEL(setting));
            GameManager.instance.localizationManager.AddSource("he-IL", new LocaleHE(setting));
            GameManager.instance.localizationManager.AddSource("uk-UA", new LocaleUK(setting));
            GameManager.instance.localizationManager.AddSource("hi-IN", new LocaleHI(setting));
            GameManager.instance.localizationManager.AddSource("id-ID", new LocaleID(setting));
            GameManager.instance.localizationManager.AddSource("vi-VN", new LocaleVI(setting));
            GameManager.instance.localizationManager.AddSource("th-TH", new LocaleTH(setting));
        }

        private void CheckVersion()
        {
            string url = "https://raw.githubusercontent.com/NyokoDev/LuminaCS2/refs/heads/master/XML/version.txt";
            string unityversion = UnityEngine.Application.unityVersion;
            string currentVersion = GlobalPaths.Version;
            string gameVersion = Version.current.fullVersion;
            string supportedGameVersion = GlobalPaths.SupportedGameVersion;

            Mod.Log.Info($"Checking game version: {gameVersion}");

            if (gameVersion != supportedGameVersion)
            {
                string errorMsg = $"[LUMINA] Unsupported game version: {gameVersion}. Supported version is {supportedGameVersion}.";
                string recommendation =
                    "Recommendations:\n" +
                    "- Update your game to the latest supported version.\n" +
                    "- Check for a newer version of the Lumina mod.\n" +
                    "- Visit the Lumina support or GitHub page for help.\n" +
                    "- Join the Discord for assistance: https://discord.gg/5gZgRNm29e";

                Mod.Log.Error($"{errorMsg}\n{recommendation}");

                {
                    GlobalPaths.SendMessage(errorMsg); // Show a message box with the version information
                }

                return;
            }

            Mod.Log.Info("Unity version " + unityversion);

            try
            {
                using (WebClient client = new WebClient())
                {
                    string latestVersion = client.DownloadString(url).Trim();

                    if (currentVersion == latestVersion)
                    {
                        Mod.Log.Info("Lumina is up to date with version: " + currentVersion);
                    }
                    else
                    {
                        string message = string.Format("Lumina new version available! Current: {0} | Latest: {1}", currentVersion, latestVersion);
                        GlobalPaths.SendMessage(message); // Show a message box with the version information
                        Mod.Log.Info(message);
                    }
                }
            }
            catch (Exception ex)
            {
                Mod.Log.Info("Error checking version: " + ex.Message);
            }
        }
        private void SendNotification()
        {
            // Notify if failed to retrieve Lumina settings
            NotificationSystem.Push(
                identifier: "lumina",
                thumbnail: "https://i.imgur.com/6KKpq5g.jpeg",
                progress: 100, // 50% complete
                title: (LocalizedString)"Lumina",  // Assuming LocalizedString conversion
                text: (LocalizedString)"Loaded succesfully.",  // Assuming LocalizedString conversion
                onClicked: () =>
                {
                    // Remove the notification when it is clicked
                    NotificationSystem.Pop("lumina");
                });
            NotificationSystem.Pop("lumina");

        }

        /// <inheritdoc/>
        public void OnDispose()
        {
            Log.Info(nameof(this.OnDispose));

            harmony?.UnpatchAll($"{nameof(Lumina)}.{nameof(Mod)}");

            if (this.setting != null)
            {
                this.setting.UnregisterInOptionsUI();
                this.setting = null;
            }
        }
    }
}