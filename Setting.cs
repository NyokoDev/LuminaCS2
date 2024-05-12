using Lumina.UI;
using LuminaMod.XML;
using Colossal;
using Colossal.IO.AssetDatabase;
using Colossal.IO.AssetDatabase.Internal;
using Game.Modding;
using Game.Prefabs;
using Game.Settings;
using Game.UI;
using Game.UI.Widgets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Lumina.XML;

namespace Lumina
{
    [FileLocation(nameof(Lumina))]
    [SettingsUIGroupOrder(kButtonGroup, kToggleGroup, kSliderGroup, kDropdownGroup)]
    [SettingsUIShowGroupName(kButtonGroup, kToggleGroup, kSliderGroup, kDropdownGroup)]
    public class Setting : ModSetting
    {
        public const string kSection = "Main";

        public const string kButtonGroup = "Button";
        public const string kToggleGroup = "Toggle";
        public const string kSliderGroup = "Slider";
        public const string kDropdownGroup = "Dropdown";

        public Setting(IMod mod) : base(mod)
        {

        }

        [SettingsUISection(kSection, kToggleGroup)]
        public bool Button
        {
            get
            {
                return SliderPanel.buttonVisible;
            }
            set
            {
                SliderPanel.buttonVisible = value;
            }
        }



        [SettingsUIButton]
        [SettingsUISection(kSection, kToggleGroup)]
        public bool OpenLocationButton
        {
            set
            {
                OpenLocation();

            }
        }

        [SettingsUIButton]
        [SettingsUISection(kSection, kToggleGroup)]
        public bool Guides
        {
            set
            {
                OpenGuides();

            }
        }



        [SettingsUIButton]
        [SettingsUISection(kSection, kToggleGroup)]
        public bool Support
        {
            set
            {
                OpenDiscordInvite();

            }
        }

        [SettingsUIButton]
        [SettingsUISection(kSection, kToggleGroup)]
        public bool Donate
        {
            set
            {
                OpenPaypal();

            }
        }

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


        public void SaveToFileIn()
        {
            File.Create(GlobalPaths.GlobalModSavingPath);
            GlobalVariables.SaveToFile(GlobalPaths.GlobalModSavingPath);
        }

        public override void SetDefaults()
        {
           
        }

        public enum SomeEnum
        {
            Value1,
            Value2,
            Value3,
        }
    }

    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;
        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }
        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
    {
        { m_Setting.GetSettingsLocaleID(), "Lumina" },
        { m_Setting.GetOptionTabLocaleID(Setting.kSection), "Main" },

        { m_Setting.GetOptionGroupLocaleID(Setting.kButtonGroup), "Buttons" },
        { m_Setting.GetOptionGroupLocaleID(Setting.kToggleGroup), "Lumina "  + GlobalPaths.Version},
        { m_Setting.GetOptionGroupLocaleID(Setting.kSliderGroup), "Sliders" },
        { m_Setting.GetOptionGroupLocaleID(Setting.kDropdownGroup), "Dropdowns" },

        { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Button)), "Toggle IMGUR button visibility" },
        { m_Setting.GetOptionDescLocaleID(nameof(Setting.Button)), "Shows or hides the ingame IMGUR button." },

          { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Guides)), "Lumina Guides" },
       { m_Setting.GetOptionDescLocaleID(nameof(Setting.Guides)), "Opens Lumina guides for information and documentation in a new tab." },

       { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Support)), "Discord Server" },
       { m_Setting.GetOptionDescLocaleID(nameof(Setting.Support)), "Opens Support discord server in new tab." },

              { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Donate)), "Donate" },
       { m_Setting.GetOptionDescLocaleID(nameof(Setting.Donate)), "Opens Paypal donation wizard in new tab. Thanks for considering supporting me!" },
{
    m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLocationButton)),
    "Open Saved Settings File"
},
{
    m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLocationButton)),
    "Opens the saved settings file in a new tab."
},


        { m_Setting.GetEnumValueLocaleID(Setting.SomeEnum.Value1), "Value 1" },
        { m_Setting.GetEnumValueLocaleID(Setting.SomeEnum.Value2), "Value 2" },
        { m_Setting.GetEnumValueLocaleID(Setting.SomeEnum.Value3), "Value 3" },
    };
        }


        public void Unload()
        {

        }
    }
}
