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
    using Game.UI.Widgets;
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

        /// <summary>
        /// Gets or sets a value indicating whether the IMGUR button should be visible.
        /// </summary>
        [SettingsUISection(KSection, KToggleGroup)]
        public bool Button
        {
            get
            {
                return SliderPanel.buttonVisible;
            }
            set => SliderPanel.buttonVisible = value;
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

        /// <summary>
        /// Locale source for English.
        /// </summary>
        public class LocaleEN : IDictionarySource
        {
            private readonly Setting setting;

            /// <summary>
            /// Initializes a new instance of the <see cref="LocaleEN"/> class.
            /// </summary>
            /// <param name="setting">Setting parameter.</param>
            public LocaleEN(Setting setting)
            {
                this.setting = setting;
            }

            /// <inheritdoc/>
            public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
            {
                return new Dictionary<string, string>
                {
          {
            setting.GetSettingsLocaleID(), "Lumina"
          },
          {
            setting.GetOptionTabLocaleID(Setting.KSection),
            "Main"
          },
          {
            setting.GetOptionGroupLocaleID(Setting.KButtonGroup),
            "Buttons"
          },
          {
            setting.GetOptionGroupLocaleID(Setting.KToggleGroup),
            "Lumina " + GlobalPaths.Version
          },
          {
            setting.GetOptionGroupLocaleID(Setting.KSliderGroup),
            "Sliders"
          },
          {
            setting.GetOptionGroupLocaleID(Setting.KDropdownGroup),
            "Dropdowns"
          },
          {
            setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Button)),
            "Toggle IMGUR button visibility"
          },
          {
            setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Button)),
            "Shows or hides the ingame IMGUR button."
          },
          {
            setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Guides)),
            "Lumina Guides"
          },
          {
            setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Guides)),
            "Opens Lumina guides for information and documentation in a new tab."
          },
          {
            setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Support)),
            "Discord Server"
          },
          {
            setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Support)),
            "Opens Support discord server in new tab."
          },
          {
            setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Donate)),
            "Donate"
          },
          {
            setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Donate)),
            "Opens Paypal donation wizard in new tab. Thanks for considering supporting me!"
          },
          {
            setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OpenLocationButton)),
            "Open Saved Settings File"
          },
          {
            setting.GetOptionDescLocaleID(nameof(Lumina.Setting.OpenLocationButton)),
            "Opens the saved settings file in a new tab."
          },
                };
            }

            /// <inheritdoc/>
            public void Unload()
            {
            }
        }
    }
}