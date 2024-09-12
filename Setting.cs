// <copyright file="Setting.cs" company="NyokoDev">
// Copyright (c) NyokoDev. All rights reserved.
// </copyright>

namespace Lumina
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using Colossal;
    using Colossal.IO.AssetDatabase;
    using Colossal.IO.AssetDatabase.Internal;
    using Game.Modding;
    using Game.Prefabs;
    using Game.Settings;
    using Game.UI;
    using Game.UI.InGame;
    using Game.UI.Widgets;
    using Lumina.Systems;
    using Lumina.UI;
    using Lumina.XML;
    using LuminaMod.XML;

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
        [SettingsUIMultilineText("https://svgshare.com/i/15rV.svg")]
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

        [SettingsUISection(KSection, KToggleGroup)]
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
            set => OpenDiscordInvite();
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
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        /// <summary>
        /// This opens the discord invite in a new tab.
        /// </summary>
        public void OpenDiscordInvite()
        {
            try
            {
                string discordInviteLink = "https://discord.gg/5JcaKwDBHn";

                // Use Process.Start to open the URL in the default web browser
                System.Diagnostics.Process.Start(discordInviteLink);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
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
                string discordInviteLink = "https://github.com/NyokoDev/LuminaCS2";

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