using Colossal;
using Lumina;
using Lumina.XML;
using System.Collections.Generic;

/// <summary>
/// Lokale bron voor Nederlands.
/// </summary>
public class LocaleNL : IDictionarySource
{
    private readonly Setting setting;

    /// <summary>
    /// Initialiseert een nieuwe instantie van de <see cref="LocaleNL"/> klasse.
    /// </summary>
    /// <param name="setting">Instellingenparameter.</param>
    public LocaleNL(Setting setting)
    {
        this.setting = setting;
    }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts) => new Dictionary<string, string>
    {
        { setting.GetSettingsLocaleID(), "Lumina" },
        { setting.GetOptionTabLocaleID(Setting.KSection), "BASIS" },
        { setting.GetOptionGroupLocaleID(Setting.KButtonGroup), "Knoppen" },
        { setting.GetOptionGroupLocaleID(Setting.KToggleGroup), $"Lumina {GlobalPaths.Version}" },
        { setting.GetOptionGroupLocaleID(Setting.KSliderGroup), "Functie schakelaars" },
        { setting.GetOptionGroupLocaleID(Setting.KDropdownGroup), "Keuzelijsten" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Automatisch opslaan" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LuminaByNyoko)), $"Lumina {GlobalPaths.Version} door Nyoko" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Laad alle pakketten opnieuw bij herstart" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Bepaalt of alle pakketten (Cubemaps, LUTs) bij herstart opnieuw geladen worden. Als ingeschakeld zal Lumina bij elke herstart alle pakketten opnieuw laden om de nieuwste versies te gebruiken en eventuele niet correct geladen textures op te schonen." },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Slaat je instellingen automatisch op bij openen of sluiten van de UI." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Guides)), "Lumina Documentatie" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Guides)), "Opent Lumina handleidingen en documentatie in een nieuw tabblad." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Support)), "Ondersteuning krijgen" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Support)), "Problemen? Klik op deze knop om een ZIP-bestand van je logs en instellingen aan te maken, de map te openen en automatisch onze Discord-uitnodiging te openen zodat je onze ondersteuningskanalen kunt betreden." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Donate)), "Doneren" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Donate)), "Opent de PayPal-donatie wizard in een nieuw tabblad. Bedankt voor je overweging om te steunen!" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Open opgeslagen instellingenbestand" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Opent het opgeslagen instellingenbestand in een nieuw tabblad." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Gebruik tijd van dag schuifregelaar" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Toont een schuifregelaar om te wisselen tussen dag en nacht. Standaard uitgeschakeld als Weather Plus of een andere tijdswisselmodus gedetecteerd wordt." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OtherSettings)), "Meer opties kunnen direct in de in-game interface aangepast worden." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Aanpassingen van breedte- en lengtegraad" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Activeer dit om handmatige aanpassingen van breedte- en lengtegraad via de in-game interface toe te staan." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Metro Framework" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Activeert het MetroFramework voor Lumina, voor een modernere UI-ervaring. Dit is standaard ingeschakeld. Voor de beste resultaten moet de volledige scherm-venstermodus aanstaan." },
    };

    public void Unload()
    {
    }
}
