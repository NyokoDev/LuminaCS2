using Colossal;
using Lumina;
using Lumina.XML;
using System.Collections.Generic;


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
    public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts) => new Dictionary<string, string>
        {
            { setting.GetSettingsLocaleID(), "Lumina" },
            { setting.GetOptionTabLocaleID(Setting.KSection), "BASIC" },
            { setting.GetOptionGroupLocaleID(Setting.KButtonGroup), "Buttons" },
            { setting.GetOptionGroupLocaleID(Setting.KToggleGroup), $"Lumina {GlobalPaths.Version}" },
            { setting.GetOptionGroupLocaleID(Setting.KSliderGroup), "Feature Toggles" },
            { setting.GetOptionGroupLocaleID(Setting.KDropdownGroup), "Dropdowns" },
            { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Save automatically" },
            { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LuminaByNyoko)), $"Lumina {GlobalPaths.Version} by Nyoko" },
            { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Fetch all packages on restart" },
            { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Determines whether to fetch all packages (Cubemaps, LUTs) on restart. When enabled, Lumina will reload all packages every time the game restarts, ensuring that the latest versions are used and cleaning up any textures that didn’t load properly." },
            { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Saves your settings automatically when the UI is opened or closed." },
            { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Guides)), "Lumina Docs" },
            { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Guides)), "Opens Lumina guides for information and documentation in a new tab." },
{ setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Support)), "Obtain Support" },
{ setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Support)), "Experiencing issues? Click this button to create a ZIP of your logs and settings, opens the folder to find it, it automatically opens our discord invite so you can join our support channel." },

            { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Donate)), "Donate" },
            { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Donate)), "Opens the PayPal donation wizard in a new tab. Thanks for considering supporting me!" },
            { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Open Saved Settings File" },
            { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Opens the saved settings file in a new tab." },
            { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Use Time Of Day Slider" },
            { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Shows a time of day slider to switch between day and night. Disabled by default if Weather Plus or another time-changing mode is detected." },
            { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OtherSettings)), "More options can be adjusted directly in the in-game interface." },

            { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Latitude and Longitude Adjustments" },
            { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Enable this to allow manual latitude and longitude changes through the in-game interface." },
              {setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "MetroFramework Enabled" },
        {setting.GetOptionDescLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Enables the MetroFramework for Lumina, allowing for a more modern UI experience. This is enabled by default. Full Screen Windowed option must be enabled for best results." },

        };

    public void Unload()
    {
    }
}
