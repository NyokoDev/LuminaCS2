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
    using HarmonyLib;
    using Lumina.Locale;
    using Lumina.Systems;
    using Lumina.XML;
    using LuminaMod.XML;
    using System;
    using System.IO;
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
            GameManager.instance.localizationManager.AddSource("en-US", new Setting.LocaleEN(setting));

            // Load translations.
            Localization.LoadTranslations(null, Log);

            try
            {
                // Load global settings
                GlobalVariables.LoadFromFile(GlobalPaths.GlobalModSavingPath);
                CheckForNullLUTName();


                // Update system after RenderEffectsSystem but before culling
                updateSystem.UpdateAfter<RenderEffectsSystem>(SystemUpdatePhase.PreCulling);

                updateSystem.UpdateAt<UISystem>(SystemUpdatePhase.MainLoop);

                updateSystem.UpdateAt<TimeOfDayProcessor>(SystemUpdatePhase.GameSimulation);
            }
            catch
            {
                // Notify if failed to retrieve Lumina settings
                NotificationSystem.Push(
                  "lumina",
                  thumbnail: "https://i.imgur.com/C9fZDiA.png",
                  title: "Lumina",
                  text: $"Wasn't possible to retrieve settings.");
            }
        }




        private static void CheckForNullLUTName()
        {
            // Access the instance of GlobalVariables and check if LUTName is null or empty
            if (GlobalVariables.Instance == null)
            {
                Lumina.Mod.Log.Error("GlobalVariables.Instance is null.");
                return;
            }

            if (string.IsNullOrEmpty(GlobalVariables.Instance.LUTName))
            {
                // If LUTName is null or empty, assign it a default value
                GlobalVariables.Instance.LUTName = "None";
                Lumina.Mod.Log.Info("LUTName was null or empty, assigned default value 'None'.");



            }
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