// <copyright file="Mod.cs" company="NyokoDev">
// Copyright (c) NyokoDev. All rights reserved.
// </copyright>

namespace Lumina
{
    using Colossal.Logging;
    using Game;
    using Game.Modding;
    using Game.PSI;
    using Game.SceneFlow;
    using Game.UI.Localization;
    using HarmonyLib;
    using Lumina.Locale;
    using Lumina.ManagerSystems;
    using Lumina.Systems;
    using Lumina.XML;
    using LuminaMod.XML;
    using MetroFramework.Forms;
    using System;
    using System.IO;
    using System.Net;
    using System.Windows.Forms;
    using UnityEngine;
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
        private Harmony? harmony;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

        /// <inheritdoc/>
        public void OnLoad(UpdateSystem updateSystem)
        {
            // Initialize Harmony for patching
            this.harmony = new Harmony($"{nameof(Lumina)}.{nameof(Mod)}");
            this.harmony.PatchAll(typeof(Mod).Assembly);
            Mod.Log.Info("Patching completed successfully.");
            Log.Info(nameof(OnLoad));

            // Log mod asset location if found
            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
            {
                Log.Info($"Mod asset location: {asset.path}");
            }

            // Initialize and register mod settings
            this.setting = new Setting(this);
            this.setting.RegisterInOptionsUI();
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(setting));

            // Load translations.
            Localization.LoadTranslations(null, Log);

            CheckVersion();
            // Load global settings
            GlobalVariables.LoadFromFile(GlobalPaths.GlobalModSavingPath);

            updateSystem.UpdateAfter<PreSystem>(SystemUpdatePhase.UIUpdate);



            // Update system after RenderEffectsSystem but before culling
            updateSystem.UpdateAfter<RenderEffectsSystem>(SystemUpdatePhase.PreCulling);

            updateSystem.UpdateAt<UISystem>(SystemUpdatePhase.UIUpdate);

            updateSystem.UpdateAt<TimeOfDayProcessor>(SystemUpdatePhase.GameSimulation);

            updateSystem.UpdateAfter<CubemapUpdateSystem>(SystemUpdatePhase.GameSimulation);

            updateSystem.UpdateAfter<CustomSunManager>(SystemUpdatePhase.GameSimulation);


            SendNotification();
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

                Setting.ShowModernMessageBox(errorMsg);
                ToastNotification.ShowToast(errorMsg);


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
                        Mod.Log.Info("The version is up to date. Current: " + currentVersion);
                    }
                    else
                    {
                        string message = $"New version available! Current: {currentVersion} | Latest: {latestVersion}";
                        Mod.Log.Info(message);

                        Setting.ShowModernMessageBox(message);
                        ToastNotification.ShowToast(message);
                    }
                }
            }
            catch (Exception ex)
            {
                Mod.Log.Error("Error checking version: " + ex.Message);
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

            this.harmony?.UnpatchAll($"{nameof(Lumina)}.{nameof(Mod)}");

            if (this.setting != null)
            {
                this.setting.UnregisterInOptionsUI();
                this.setting = null;
            }
        }
    }
}