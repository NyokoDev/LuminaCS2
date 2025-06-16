using Colossal;
using Lumina;
using Lumina.XML;
using System.Collections.Generic;

/// <summary>
/// Lokalkälla för svenska.
/// </summary>
public class LocaleSV : IDictionarySource
{
    private readonly Setting setting;

    /// <summary>
    /// Skapar en ny instans av <see cref="LocaleSV"/>-klassen.
    /// </summary>
    /// <param name="setting">Inställningsparameter.</param>
    public LocaleSV(Setting setting)
    {
        this.setting = setting;
    }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts) => new Dictionary<string, string>
    {
        { setting.GetSettingsLocaleID(), "Lumina" },
        { setting.GetOptionTabLocaleID(Setting.KSection), "GRUNDLÄGGANDE" },
        { setting.GetOptionGroupLocaleID(Setting.KButtonGroup), "Knappar" },
        { setting.GetOptionGroupLocaleID(Setting.KToggleGroup), $"Lumina {GlobalPaths.Version}" },
        { setting.GetOptionGroupLocaleID(Setting.KSliderGroup), "Funktioner att slå på/av" },
        { setting.GetOptionGroupLocaleID(Setting.KDropdownGroup), "Rullgardinsmenyer" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Spara automatiskt" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LuminaByNyoko)), $"Lumina {GlobalPaths.Version} av Nyoko" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Hämta alla paket vid omstart" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Bestämmer om alla paket (Cubemaps, LUTs) ska hämtas vid omstart. När detta är aktiverat laddar Lumina om alla paket varje gång spelet startas om, vilket säkerställer att senaste versionerna används och rensar eventuella texturer som inte laddats korrekt." },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Sparar dina inställningar automatiskt när gränssnittet öppnas eller stängs." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Guides)), "Lumina-dokumentation" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Guides)), "Öppnar Luminas guider för information och dokumentation i en ny flik." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Support)), "Få support" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Support)), "Upplever du problem? Klicka på denna knapp för att skapa en ZIP-fil med dina loggar och inställningar, öppna mappen där den finns, och automatiskt öppna vår Discord-inbjudan så att du kan gå med i vår supportkanal." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Donate)), "Donera" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Donate)), "Öppnar PayPal-donationsguiden i en ny flik. Tack för att du överväger att stödja mig!" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Öppna sparad inställningsfil" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Öppnar den sparade inställningsfilen i en ny flik." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Använd tidsreglaget" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Visar ett tidsreglage för att växla mellan dag och natt. Som standard inaktiverat om Weather Plus eller annan tidsändrande mod upptäcks." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OtherSettings)), "Fler alternativ kan justeras direkt i spelets gränssnitt." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Justerar latitud och longitud" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Aktivera detta för att tillåta manuell justering av latitud och longitud via spelets gränssnitt." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Metro Framework" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Aktiverar MetroFramework för Lumina, vilket ger en mer modern UI-upplevelse. Detta är aktiverat som standard. Fullskärmsfönster måste vara aktiverat för bästa resultat." },

        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "Aktivera Lumina Volume" },
{ setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "Krävs för att Lumina ska fungera korrekt. Aktiverar ett Unity HDRP-volym som tillämpar anpassade belysnings- och efterbehandlingsinställningar. Detta gör att Luminas avancerade visuella effekter, såsom färgkorrigering och tonmappning, fungerar korrekt i spelet." },

    };

    public void Unload()
    {
    }
}
