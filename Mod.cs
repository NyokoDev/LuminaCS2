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
    using System;
    using System.IO;
    using System.Net;
    using UnityEngine;

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
            string url = "https://raw.githubusercontent.com/NyokoDev/LuminaCS2/refs/heads/master/XML/version.txt"; // URL for the version file
            string currentVersion = GlobalPaths.Version; 

            try
            {
                // Create a WebClient to fetch the version from the URL
                using (WebClient client = new WebClient())
                {
                    // Download the version file content synchronously
                    string latestVersion = client.DownloadString(url).Trim(); // Fetch and trim the version string

                    // Compare the current version with the latest version
                    if (currentVersion == latestVersion)
                    {
                        Mod.Log.Info("The version is up to date. Current: " + currentVersion);
                    }
                    else
                    {
                        Mod.Log.Info("New version available! Current: " + currentVersion + " Latest: " + latestVersion);
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