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
    using Game.UI;
    using Game.UI.InGame;
    using Game.UI.Localization;
    using Game.UI.Widgets;
    using Lumina.Systems;
    using Lumina.UI;
    using Lumina.XML;
    using LuminaMod.XML;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.IO;
    using System.IO.Compression;
    using System.Threading;
    using System.Windows.Forms;
    using UnityEngine;
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

        [SettingsUISection(KSection, KSliderGroup)]
        public bool LatitudeAndLongitudeAdjustments
        {
            get => GlobalVariables.Instance.LatLongEnabled;
            set
            {
                GlobalVariables.Instance.LatLongEnabled = value;
            }
        }

        [SettingsUISection(KSection, KSliderGroup)]
        public bool UseTimeOfDaySlider
        {
            get => TimeOfDayProcessor.Locked;
            set
            {
                TimeOfDayProcessor.Locked = value;
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
            set => ZipAndOpenDiscordInvite();
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
                    filesToZip.AddRange(Directory.GetFiles(logsDirectory, "*.*", SearchOption.AllDirectories));
                else
                    Lumina.Mod.Log.Info($"Logs directory not found: {logsDirectory}");

                if (File.Exists(settingsPath))
                    filesToZip.Add(settingsPath);
                else
                    Lumina.Mod.Log.Info($"Settings file not found: {settingsPath}");

                if (filesToZip.Count == 0)
                {
                    ShowModernMessageBox("No log or settings files found to zip.");
                    return;
                }

                string tempDir = GlobalPaths.AssemblyDirectory;
                string zipPattern = "LuminaLogs_*.zip";
                foreach (var oldZip in Directory.GetFiles(tempDir, zipPattern))
                {
                    try { File.Delete(oldZip); } catch { }
                }

                string zipPath = Path.Combine(tempDir, $"LuminaLogs_{DateTime.Now:yyyyMMdd_HHmmss}.zip");
                using (var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
                {
                    foreach (var file in filesToZip)
                    {
                        zip.CreateEntryFromFile(file, Path.GetFileName(file));
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

                    var infoEntry = zip.CreateEntry("Lumina_System.txt");
                    using (var writer = new StreamWriter(infoEntry.Open()))
                    {
                        writer.Write(infoContent);
                    }
                }

                if (!File.Exists(zipPath))
                {
                    ShowModernMessageBox("Failed to create the zip file.");
                    return;
                }

                Lumina.Mod.Log.Info($"Zip file created: {zipPath}");
                Process.Start("explorer.exe", $"/select,\"{zipPath}\"");

                ShowModernMessageBox("A ZIP file with your Lumina logs and settings has been created and opened in File Explorer. Please upload this file in the #support channel on Discord. The Discord invite link has been opened in your browser to help you join. Thank you for supporting Lumina!");


                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://discord.gg/NgYaXXdFnY",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Lumina.Mod.Log.Info("An error occurred: " + ex.Message);
                ShowModernMessageBox($"An error occurred: {ex.Message}");
            }
        }

        public static void ShowModernMessageBox(string message)
        {
            // Escape single quotes for PowerShell
            string escapedMessage = message.Replace("'", "''");

            Process.Start(new ProcessStartInfo
            {
                FileName = "powershell",
                Arguments = $"-Command \"Add-Type -AssemblyName PresentationFramework;[System.Windows.MessageBox]::Show('{escapedMessage}', 'Lumina Support')\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            });
        }



        private string EscapeForPowerShell(string input)
        {
            return input.Replace("'", "''");
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