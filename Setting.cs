// <copyright file="Setting.cs" company="NyokoDev">
// Copyright (c) NyokoDev. All rights reserved.
// </copyright>

namespace Lumina
{
    using Colossal;
    using Colossal.IO.AssetDatabase;
    using Colossal.IO.AssetDatabase.Internal;
    using Game.Modding;
    using Game.Prefabs;
    using Game.PSI;
    using Game.SceneFlow;
    using Game.Settings;
    using Game.Simulation;
    using Game.UI;
    using Game.UI.InGame;
    using Game.UI.Localization;
    using Game.UI.Widgets;
    using Lumina.Metro;
    using Lumina.Roads;
    using Lumina.Systems;
    using Lumina.Systems.SimulationRefresh;
    using Lumina.UI;
    using Lumina.XML;
    using LuminaMod.XML;
    using RoadWearAdjuster.Systems;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;
    using Unity.Entities;
    using UnityEngine;
    using static UnityEngine.Rendering.DebugUI;
    using Version = Game.Version;


    /// <summary>
    /// Main settings file.
    /// </summary>
    [FileLocation(nameof(Lumina))]
    [SettingsUIGroupOrder(KButtonGroup, KToggleGroup, KSliderGroup, KDropdownGroup)]
    [SettingsUIShowGroupName(KButtonGroup, KToggleGroup, KSliderGroup, KDropdownGroup)]
    public class Setting : ModSetting
    {

       
    
        

        /// <summary>
        /// Main section.
        /// </summary>
        public
        const string KSection = "Main";

        /// <summary>
        /// Main button group.
        /// </summary>
        public
        const string KButtonGroup = "Button";

        /// <summary>
        /// Main toggle group.
        /// </summary>
        public
        const string KToggleGroup = "Toggle";

        /// <summary>
        /// Main slider group.
        /// </summary>
        public
        const string KSliderGroup = "Slider";

        /// <summary>
        /// Main dropdown group.
        /// </summary>
        public
        const string KDropdownGroup = "Dropdown";
        private static SynchronizationContext _unityContext;


        /// <summary>
        /// Initializes a new instance of the <see cref="Setting"/> class.
        /// </summary>
        /// <param name="mod">Mod.</param>
        public Setting(IMod mod)
            : base(mod)
        {
        }

        [SettingsUISection(KSection, KToggleGroup)]
        [SettingsUIMultilineText("coui://ui-mods/Icons/Lumina.svg")]
        public string LuminaByNyoko => string.Empty;


        [SettingsUISection(KSection, KToggleGroup)]
        public bool EnableLuminaVolume
        {
            get => GlobalVariables.Instance.LuminaVolumeEnabled;
            set
            {
                GlobalVariables.Instance.LuminaVolumeEnabled = value;
            }
        }


        [SettingsUISection(KSection, KToggleGroup)]
        public bool ReloadAllPackagesOnRestart
        {
            get => GlobalVariables.Instance.ReloadAllPackagesOnRestart;
            set
            {
                GlobalVariables.Instance.ReloadAllPackagesOnRestart = value;
            }
        }


        [SettingsUISection(KSection, KToggleGroup)]
        public bool SaveAutomatically
        {
            get => GlobalVariables.Instance.SaveAutomatically;
            set
            {
                GlobalVariables.Instance.SaveAutomatically = value;
            }
        }



        /// <summary>
        /// Sets a value indicating whether the location should be launched.
        /// </summary>
        [SettingsUIButton]
        [SettingsUISection(KSection, KToggleGroup)]
        public bool OpenLocationButton
        {
            set
            {
                this.OpenLocation();
            }
        }

        /// <summary>
        /// Sets a value indicating whether it opens the Github Guides center for Lumina.
        /// </summary>
        [SettingsUIButton]
        [SettingsUISection(KSection, KToggleGroup)]
        [SettingsUIMultilineText("coui://ui-mods/Icons/Lumina.svg")]
        public bool Guides
        {
            set
            {
                this.OpenGuides();
            }
        }

        /// <summary>
        /// Sets a value indicating whether the discord invite should be opened.
        /// </summary>
        [SettingsUIButton]
        [SettingsUISection(KSection, KToggleGroup)]
        public bool Support
        {
            set
            {
                System.Threading.Tasks.Task.Run(() => ZipAndOpenDiscordInvite());
            }

        }

        /// <summary>
        /// Sets a value indicating whether it opens the donate paypal.
        /// </summary>
        [SettingsUIButton]
        [SettingsUISection(KSection, KToggleGroup)]
        public bool Donate
        {
            set
            {
                OpenPaypal();
            }
        }

        [SettingsUISection(KSection, KToggleGroup)]
        [SettingsUIMultilineText("coui://ui-mods/Icons/Lumina.svg")]
        public string OtherSettings => string.Empty;

        // Second page of settings

        [SettingsUISection(KSection, KSliderGroup)]
        public bool LatitudeAndLongitudeAdjustments
        {
            get => GlobalVariables.Instance.LatLongEnabled;
            set
            {
                GlobalVariables.Instance.LatLongEnabled = value;
                Mod.RunTranspilerPatch();
            }
        }


        [SettingsUISection(KSection, KSliderGroup)]
        public bool UseRoadTextures
        {
            get => GlobalVariables.Instance.UseRoadTextures;
            set
            {
                GlobalVariables.Instance.UseRoadTextures = value;
                var world = World.DefaultGameObjectInjectionWorld;         // or whichever is correct for you
                if (world == null) return;                                 // safety

                m_ReplaceRoadWearSystem ??=
                    world.GetExistingSystemManaged<ReplaceRoadWearSystem>();  // 🚫 not GetOrCreate
                m_ReplaceRoadWearSystem.RevertTextures(); // revert textures if they were already applied
                RoadColorReplacer.RestoreOriginalColors(); // restore original colors if they were already applied

                CheckForMods(); // check for incompatible mods
            }
        }

        private void CheckForMods()
        {

            if (!GlobalVariables.Instance.UseRoadTextures) return;

            string[] incompatibleMods = { "RoadWearRemover", "RoadWearAdjuster" };
            foreach (var modName in incompatibleMods)
            {
                if (CompatibilityHelper.CheckForIncompatibleMods(modName))
                {
                    string errorMessage =
                        $"Incompatible mod detected: {modName}.\n" +
                        $"Both Lumina and {modName} modify road textures, which can cause visual glitches or conflicts.\n" +
                        "To ensure stability, Lumina's road texture system has been disabled.\n" +
                        $"Please remove {modName} to take full advantage of Lumina's features.";

                    Mod.Log.Info(errorMessage);
                    GlobalVariables.Instance.UseRoadTextures = false;

                    var dialog = new SimpleMessageDialog(errorMessage);
                    GameManager.instance.userInterface.appBindings.ShowMessageDialog(dialog, null);

                    break;
                }
            }
        }




        [SettingsUISection(KSection, KSliderGroup)]
        public bool UseTimeOfDaySlider
        {
            get => GlobalVariables.Instance.ViewTimeOfDaySlider;
            set
            {
                GlobalVariables.Instance.ViewTimeOfDaySlider = value;
                CheckForTimeAndWeatherAnarchy(); // check for incompatible mods
            }
        }

        private void CheckForTimeAndWeatherAnarchy()
        {
            if (GlobalVariables.Instance.ViewTimeOfDaySlider &&
                CompatibilityHelper.CheckForIncompatibleMods("TimeWeatherAnarchy"))
            {
                // Disable the slider if incompatible mod is active
                GlobalVariables.Instance.ViewTimeOfDaySlider = false;

                var dialog = new SimpleMessageDialog(
                    "Incompatible mod detected: Time and Weather Anarchy.\n\n" +
                    "This mod conflicts with Lumina's Time of Day Slider.\n" +
                    "To prevent conflicts, the slider has been disabled.\n\n" +
                    "Please remove Time and Weather Anarchy to use the slider again."
                );
                GameManager.instance.userInterface.appBindings.ShowMessageDialog(dialog, null);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Metro Framework is enabled. Always disabled.
        /// </summary>
        [SettingsUISection(KSection, KSliderGroup)]
        [SettingsUIDisableByCondition(typeof(Setting), nameof(Setting.AlwaysDisableMetro))]
        public bool MetroFrameworkEnabled
        {
            get => GlobalVariables.Instance.MetroEnabled;
            set
            {
                GlobalVariables.Instance.MetroEnabled = value;
                Eureka();
            }
        }

        public ReplaceRoadWearSystem m_ReplaceRoadWearSystem { get; private set; }

        public static bool AlwaysDisableMetro()
        {
            // Always return true to disable MetroFramework due to a bug in the current game version that prevents the game from working properly with MetroFramework enabled.
            return true;
        }

        /// <summary>
        /// This method is called when the MetroFrameworkEnabled setting is changed.
        /// </summary>
        private void Eureka()
        {
            bool value = GlobalVariables.Instance.MetroEnabled;

            string status = value ? "enabled" : "disabled";
            string message = $"MetroFramework is now {status}. Please restart the game for changes to take effect. For best results use FullScreenWindowed if enabled.";
            {
                ShowModernMessageBox(message);
            }
        }


        /// <summary>
        /// Opens the folder location.
        /// </summary>
        public void OpenLocation()
        {
            try
            {
                string settingsFilePath = GlobalPaths.GlobalModSavingPath;

                // Check if the file exists
                if (File.Exists(settingsFilePath))
                {
                    GlobalVariables.SaveToFile(GlobalPaths.GlobalModSavingPath); // Save first
                    Process.Start(settingsFilePath);
                }
                else
                {
                    GlobalVariables.SaveToFile(GlobalPaths.GlobalModSavingPath);
                    Process.Start(settingsFilePath);
                }
            }
            catch (Exception ex)
            {
                Mod.Log.Info("An error occurred: " + ex.Message);
            }
        }

        /// <summary>
        /// Zips the necesssary log files and opens the Discord invite link.
        /// </summary>
        public void ZipAndOpenDiscordInvite()
        {
            try
            {
                var filesToZip = new List<string>();

                string logsDirectory = GlobalPaths.logsassemblyPDXDirectory;
                string settingsPath = GlobalPaths.GlobalModSavingPath;

                if (Directory.Exists(logsDirectory))
                {
                    try
                    {
                        filesToZip.AddRange(Directory.GetFiles(logsDirectory, "*.*", SearchOption.AllDirectories));
                    }
                    catch (Exception ex)
                    {
                        Lumina.Mod.Log.Info($"Failed to collect logs: {ex.Message}");
                    }
                }
                else
                {
                    Lumina.Mod.Log.Info($"Logs directory not found: {logsDirectory}");
                }

                if (File.Exists(settingsPath))
                {
                    filesToZip.Add(settingsPath);
                }
                else
                {
                    Lumina.Mod.Log.Info($"Settings file not found: {settingsPath}");
                }

                if (filesToZip.Count == 0)
                {
                    ShowModernMessageBox("No log or settings files found to zip.");
                    return;
                }

                string tempDir = GlobalPaths.AssemblyDirectory;
                string zipPattern = "LuminaLogs_*.zip";

                foreach (var oldZip in Directory.GetFiles(tempDir, zipPattern))
                {
                    try
                    {
                        File.Delete(oldZip);
                    }
                    catch (Exception ex)
                    {
                        Lumina.Mod.Log.Info($"Failed to delete old zip file {oldZip}: {ex.Message}");
                    }
                }

                string zipPath = Path.Combine(tempDir, $"LuminaLogs_{DateTime.Now:yyyyMMdd_HHmmss}.zip");

                using (var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                {
                    foreach (var file in filesToZip)
                    {
                        try
                        {
                            zip.CreateEntryFromFile(file, Path.GetFileName(file));
                        }
                        catch (Exception ex)
                        {
                            Lumina.Mod.Log.Info($"Failed to zip {file}: {ex.Message}");
                        }
                    }

                    string screenResolution = $"{UnityEngine.Screen.currentResolution.width}x{UnityEngine.Screen.currentResolution.height}";
                    string gpuName = SystemInfo.graphicsDeviceName;
                    string ram = $"{SystemInfo.systemMemorySize} MB";
                    string cpuName = SystemInfo.processorType;

                    string infoContent =
                        $"Lumina Support Info{Environment.NewLine}" +
                        $"Date: {DateTime.Now}{Environment.NewLine}" +
                        $"Mod Version: {GlobalPaths.Version}{Environment.NewLine}" +
                        $"Game Version: {Version.current.fullVersion}{Environment.NewLine}" +
                        $"OS: {SystemInfo.operatingSystem}{Environment.NewLine}" +
                        $"Screen Resolution: {screenResolution}{Environment.NewLine}" +
                        $"GPU: {gpuName}{Environment.NewLine}" +
                        $"RAM: {ram}{Environment.NewLine}" +
                        $"CPU: {cpuName}{Environment.NewLine}";

                    try
                    {
                        var infoEntry = zip.CreateEntry("Lumina_System.txt");
                        using (var writer = new StreamWriter(infoEntry.Open()))
                        {
                            writer.Write(infoContent);
                        }
                    }
                    catch (Exception ex)
                    {
                        Lumina.Mod.Log.Info("Failed to write system info to zip: " + ex.Message);
                    }
                }

                if (!File.Exists(zipPath))
                {
                    ShowModernMessageBox("Failed to create the zip file.");
                    return;
                }

                Lumina.Mod.Log.Info($"Zip file created: {zipPath}");

                try
                {
                    Process.Start("explorer.exe", $"/select,\"{zipPath}\"");
                }
                catch (Exception ex)
                {
                    Lumina.Mod.Log.Info("Failed to open File Explorer: " + ex.Message);
                }

                ShowModernMessageBox("A ZIP file with your Lumina logs and settings has been created and opened in File Explorer. Please upload this file in the #support channel on Discord. The Discord invite link has been opened in your browser to help you join. Thank you for supporting Lumina!");

                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://discord.gg/NgYaXXdFnY",
                    });
                }
                catch (Exception ex)
                {
                    Lumina.Mod.Log.Info("Failed to open Discord invite link: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Lumina.Mod.Log.Info("Unexpected error in ZipAndOpenDiscordInvite: " + ex.Message);
                ShowModernMessageBox("An error occurred: " + ex.Message);
            }
        }



        /// <summary>
        /// Shows a toast (Metro enabled) or logs (Metro disabled) from ANY thread.
        /// </summary>
        public static void ShowModernMessageBox(string v)
        {
            // If we haven’t captured the main context yet, do it now.
            // The first call is normally from the main Unity thread.
            if (_unityContext == null)
            {
                _unityContext = SynchronizationContext.Current;
            }

            // The work we actually want to perform on the main thread:
            void DoMessage()
            {
                if (GlobalVariables.Instance.MetroEnabled)
                {
                    ToastNotification.ShowToast("An error occurred: " + v);
                }
                else
                {
                    Lumina.Mod.Log.Info(v);
                }
            }

            // Are we already on the Unity main thread?
            if (SynchronizationContext.Current == _unityContext)
            {
                DoMessage();              // Yes → just run it
            }
            else if (_unityContext != null)
            {
                _unityContext.Post(_ => DoMessage(), null);   // No → marshal to main thread
            }
            else
            {
                // Fallback: context unknown (shouldn’t happen after first call)
                Lumina.Mod.Log.Info("Could not capture Unity context; message logged:\n" + v);
            }
        }

        



        /// <summary>
        /// This opens the paypal website in a new tab.
        /// </summary>
        public void OpenPaypal()
        {
            try
            {
                string discordInviteLink = "https://paypal.me/nyokodev";

                // Use Process.Start to open the URL in the default web browser
                System.Diagnostics.Process.Start(discordInviteLink);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        /// <summary>
        /// Opens the GitHub Guides website.
        /// </summary>
        public void OpenGuides()
        {
            try
            {
                string discordInviteLink = "https://skylinx.gitbook.io/lumina";

                // Use Process.Start to open the URL in the default web browser
                System.Diagnostics.Process.Start(discordInviteLink);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        /// <summary>
        /// Override apply button to make our own implementation.
        /// </summary>
        public override void Apply()
        {
            base.Apply();
            GlobalVariables.SaveToFile(GlobalPaths.GlobalModSavingPath);
        }

        /// <summary>
        /// This creates the file first, then saves it.
        /// </summary>
        public void SaveToFileIn()
        {
            File.Create(GlobalPaths.GlobalModSavingPath);
            GlobalVariables.SaveToFile(GlobalPaths.GlobalModSavingPath);
        }

        /// <summary>
        /// Does nothing, but required.
        /// </summary>
        public override void SetDefaults()
        {
        }

        public void Unload()
        {

        }
    }
}