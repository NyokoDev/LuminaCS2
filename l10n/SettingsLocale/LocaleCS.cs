using Colossal;
using Lumina;
using Lumina.XML;
using System.Collections.Generic;

/// <summary>
/// Zdroj lokalizace pro češtinu.
/// </summary>
public class LocaleCS : IDictionarySource
{
    private readonly Setting setting;

    /// <summary>
    /// Inicializuje novou instanci třídy <see cref="LocaleCS"/>.
    /// </summary>
    /// <param name="setting">Parametr nastavení.</param>
    public LocaleCS(Setting setting)
    {
        this.setting = setting;
    }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts) => new Dictionary<string, string>
    {
        { setting.GetSettingsLocaleID(), "Lumina" },
        { setting.GetOptionTabLocaleID(Setting.KSection), "ZÁKLADNÍ" },
        { setting.GetOptionGroupLocaleID(Setting.KButtonGroup), "Tlačítka" },
        { setting.GetOptionGroupLocaleID(Setting.KToggleGroup), $"Lumina {GlobalPaths.Version}" },
        { setting.GetOptionGroupLocaleID(Setting.KSliderGroup), "Přepínače funkcí" },
        { setting.GetOptionGroupLocaleID(Setting.KDropdownGroup), "Rozbalovací seznamy" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Automaticky ukládat" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LuminaByNyoko)), $"Lumina {GlobalPaths.Version} od Nyoko" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Načíst všechny balíčky při restartu" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Určuje, zda se při restartu načtou všechny balíčky (Cubemapy, LUTy). Pokud je povoleno, Lumina při každém restartu hry načte všechny balíčky znovu, čímž zajistí použití nejnovějších verzí a vyčistí případné špatně načtené textury." },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Nastavení se automaticky uloží při otevření nebo zavření uživatelského rozhraní." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Guides)), "Lumina návody" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Guides)), "Otevře návody Lumina pro informace a dokumentaci v nové záložce." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Support)), "Získat podporu" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Support)), "Máte problémy? Kliknutím na toto tlačítko vytvoříte ZIP s vašimi protokoly a nastaveními, otevře složku, kde ho najdete, a automaticky otevře pozvánku na náš Discord, kde se můžete připojit na náš podpůrný kanál." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Donate)), "Přispět" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Donate)), "Otevře průvodce darováním PayPal v nové záložce. Děkuji, že uvažujete o podpoře mé práce!" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Otevřít soubor s uloženými nastaveními" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Otevře soubor s uloženými nastaveními v nové záložce." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Použít posuvník času dne" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Zobrazí posuvník času dne pro přepínání mezi dnem a nocí. Ve výchozím nastavení je deaktivováno, pokud je detekován Weather Plus nebo jiný režim měnící čas." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OtherSettings)), "Další možnosti lze upravit přímo v herním rozhraní." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Úpravy zeměpisné šířky a délky" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Povolte tuto možnost pro manuální změny zeměpisné šířky a délky přes herní rozhraní." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Metro Framework" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Povolí MetroFramework pro Lumina, což umožňuje modernější uživatelské rozhraní. Ve výchozím nastavení povoleno. Pro nejlepší výsledky je třeba povolit režim okna na celou obrazovku." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "Povolit Lumina Volume" },
{ setting.GetOptionDescLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "Vyžadováno pro správné fungování Lumina. Aktivuje Unity HDRP volume, které aplikuje vlastní nastavení osvětlení a post-processingu. Díky tomu mohou pokročilé vizuální efekty Lumina, jako korekce barev a tonální mapování, správně fungovat ve hře." },

{setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.EnableDebugLogging)), "Povolit ladicí protokolování" },
{setting.GetOptionDescLocaleID(nameof(Lumina.Setting.EnableDebugLogging)), "Povolí podrobné ladicí protokoly pro řešení problémů. To vytvoří obsáhlejší soubory protokolů, které mohou pomoci při identifikaci problémů, ale může mírně ovlivnit výkon." },
{setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.EnablePerformanceMode)), "Režim výkonu" },
{setting.GetOptionDescLocaleID(nameof(Lumina.Setting.EnablePerformanceMode)), "Po aktivaci Lumina odloží načítání systémů terénu, osvětlení a vody, dokud nebude hra plně načtena. To může pomoci snížit zpoždění v hlavním menu a zajistit plynulejší spuštění hry. Během načítání však může dojít k mírnému poklesu snímkové frekvence. Pokud zažíváte pády nebo nestabilitu, zvažte deaktivaci této volby." },

    };

    public void Unload()
    {
    }
}
