using Colossal;
using Lumina;
using Lumina.XML;
using System.Collections.Generic;

/// <summary>
/// Lokalisaatiolähde suomeksi.
/// </summary>
public class LocaleFI : IDictionarySource
{
    private readonly Setting setting;

    /// <summary>
    /// Luo uuden <see cref="LocaleFI"/>-instanssin.
    /// </summary>
    /// <param name="setting">Asetusparametri.</param>
    public LocaleFI(Setting setting)
    {
        this.setting = setting;
    }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts) => new Dictionary<string, string>
    {
        { setting.GetSettingsLocaleID(), "Lumina" },
        { setting.GetOptionTabLocaleID(Setting.KSection), "PERUS" },
        { setting.GetOptionGroupLocaleID(Setting.KButtonGroup), "Painikkeet" },
        { setting.GetOptionGroupLocaleID(Setting.KToggleGroup), $"Lumina {GlobalPaths.Version}" },
        { setting.GetOptionGroupLocaleID(Setting.KSliderGroup), "Toiminnon kytkimet" },
        { setting.GetOptionGroupLocaleID(Setting.KDropdownGroup), "Valikot" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Tallenna automaattisesti" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LuminaByNyoko)), $"Lumina {GlobalPaths.Version} tekijältä Nyoko" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Lataa kaikki paketit uudelleen käynnistyksen yhteydessä" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Määrää, ladataanko kaikki paketit (kuutemapit, LUT:t) uudelleen pelin käynnistyessä. Kun päällä, Lumina lataa kaikki paketit uudelleen jokaisella käynnistyksellä varmistaen uusimmat versiot ja korjaten mahdolliset latausvirheet." },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Tallentaa asetukset automaattisesti, kun käyttöliittymä avataan tai suljetaan." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Guides)), "Lumina-ohjeet" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Guides)), "Avaa Lumina-oppaat tiedoille ja dokumentaatiolle uudessa välilehdessä." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Support)), "Hanki tukea" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Support)), "Koetko ongelmia? Klikkaa tätä painiketta luodaksesi ZIP-tiedoston lokeistasi ja asetuksistasi, avaa kansio tiedoston löytämistä varten ja avaa automaattisesti Discord-kutsumme, jotta voit liittyä tukikanavalle." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Donate)), "Lahjoita" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Donate)), "Avaa PayPal-lahjoitusikkunan uudessa välilehdessä. Kiitos, että harkitset tukemista!" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Avaa tallennettu asetustiedosto" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Avaa tallennetun asetustiedoston uudessa välilehdessä." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Käytä päivänajan liukusäädintä" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Näyttää päivänajan liukusäätimen päivän ja yön vaihtamiseen. Oletuksena pois päältä, jos Weather Plus tai muu ajan muuttava moodi on käytössä." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OtherSettings)), "Lisäasetuksia voi muuttaa suoraan pelin käyttöliittymässä." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Leveyspiirin ja pituuspiirin säädöt" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Ota käyttöön manuaaliset leveyspiirin ja pituuspiirin säädöt pelin käyttöliittymän kautta." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Metro Framework" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Ota MetroFramework käyttöön Luminalle modernimman käyttöliittymän kokemuksen saamiseksi. Tämä on oletuksena päällä. Paras tulos saavutetaan, kun koko näytön ikkuna on käytössä." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "Ota Lumina Volume käyttöön" },
{ setting.GetOptionDescLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "Vaaditaan Lumina-toimintojen oikeaan toimintaan. Aktivoi Unity HDRP -volyymin, joka käyttää mukautettuja valaistus- ja jälkikäsittelyasetuksia. Tämä mahdollistaa Lumina:n edistyneet visuaaliset efektit, kuten värikorjauksen ja sävyjen kartoituksen, toimimaan oikein pelissä." },


    };

    public void Unload()
    {
    }
}
