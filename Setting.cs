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


        [SettingsUISection(kSection, kToggleGroup)]
        [SettingsUISlider(min = -500f, max = 1000f, step = 1f, unit = "percentage", scaleDragVolume = true)]
        public float PostExposure
        {
            get { return GlobalVariables.Instance.PostExposure; }
            set
            {
                GlobalVariables.Instance.PostExposure = value;
                SaveToFileIn();
            }
        }

        [SettingsUISection(kSection, kToggleGroup)]
        [SettingsUISlider(min = -10000f, max = 10000f, step = 1f, unit = "integer", scaleDragVolume = true)]
        public float Contrast
        {
            get { return GlobalVariables.Instance.Contrast; }
            set
            {
                GlobalVariables.Instance.Contrast = value;
                SaveToFileIn();
            }
        }

        [SettingsUISection(kSection, kToggleGroup)]
        [SettingsUISlider(min = -10000f, max = 10000f, step = 1f, unit = "integer", scaleDragVolume = true)]
        public float HueShift
        {
            get { return GlobalVariables.Instance.hueShift; }
            set
            {
                GlobalVariables.Instance.hueShift = value;
                SaveToFileIn();
            }
        }

        [SettingsUISection(kSection, kToggleGroup)]
        [SettingsUISlider(min = -10000f, max = 10000f, step = 1f, unit = "integer", scaleDragVolume = true)]
        public float Saturation
        {
            get { return GlobalVariables.Instance.Saturation; }
            set
            {
                GlobalVariables.Instance.Saturation = value;
                SaveToFileIn();
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
        public bool Support
        {
            set
            {
                OpenDiscordInvite();

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

                    Process.Start(settingsFilePath);
                }
                else
                {
                    Console.WriteLine("File not found: " + settingsFilePath);
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
        { m_Setting.GetOptionGroupLocaleID(Setting.kToggleGroup), "Color Adjustments" },
        { m_Setting.GetOptionGroupLocaleID(Setting.kSliderGroup), "Sliders" },
        { m_Setting.GetOptionGroupLocaleID(Setting.kDropdownGroup), "Dropdowns" },

        { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Button)), "Toggle IMGUR button visibility" },
        { m_Setting.GetOptionDescLocaleID(nameof(Setting.Button)), "Shows or hides the ingame IMGUR button." },


        { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PostExposure)), "Post Exposure" },
        { m_Setting.GetOptionDescLocaleID(nameof(Setting.PostExposure)), "Adjusts the brightness of the image just before color grading, in EV." },

       { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Support)), "Support" },
       { m_Setting.GetOptionDescLocaleID(nameof(Setting.Support)), "Opens Support discord server in new tab." },

        { m_Setting.GetOptionLabelLocaleID(nameof(Setting.OpenLocationButton)), "Open file" },
       { m_Setting.GetOptionDescLocaleID(nameof(Setting.OpenLocationButton)), "Opens file in new tab." },

        { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Contrast)), "Contrast" },
        { m_Setting.GetOptionDescLocaleID(nameof(Setting.Contrast)), "Contrast refers to the difference in luminance or color that makes an object (or its representation in an image or display) distinguishable. It is the difference between the darkest and lightest areas in an image. Adjusting contrast can make an image appear more vibrant and dynamic by increasing the difference between light and dark areas, or it can be reduced to create a softer, more subdued look." },

        { m_Setting.GetOptionLabelLocaleID(nameof(Setting.HueShift)), "Hue Shift" },
        { m_Setting.GetOptionDescLocaleID(nameof(Setting.HueShift)), "Hue shift, also known as hue adjustment, refers to the modification of the colors in an image along the color spectrum. It involves changing the overall tint of the image by shifting the hues of its individual colors. This adjustment can be used to fine-tune the color balance or to create artistic effects by subtly altering the overall color tone of an image." },

        { m_Setting.GetOptionLabelLocaleID(nameof(Setting.Saturation)), "Saturation" },
        { m_Setting.GetOptionDescLocaleID(nameof(Setting.Saturation)), "Saturation refers to the intensity or purity of colors in an image. Increasing saturation enhances the vividness and richness of colors, making them more vibrant and striking. Conversely, decreasing saturation desaturates the colors, resulting in a more muted or grayscale appearance. Saturation adjustment allows for precise control over the color intensity in an image, enabling photographers and artists to achieve the desired visual impact." },

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
