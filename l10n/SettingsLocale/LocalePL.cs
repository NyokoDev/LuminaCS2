using Colossal;
using Lumina;
using Lumina.XML;
using System.Collections.Generic;

/// <summary>
/// Źródło lokalizacji dla języka polskiego.
/// </summary>
public class LocalePL : IDictionarySource
{
    private readonly Setting setting;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="LocalePL"/>.
    /// </summary>
    /// <param name="setting">Parametr ustawień.</param>
    public LocalePL(Setting setting)
    {
        this.setting = setting;
    }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts) => new Dictionary<string, string>
    {
        { setting.GetSettingsLocaleID(), "Lumina" },
        { setting.GetOptionTabLocaleID(Setting.KSection), "PODSTAWOWE" },
        { setting.GetOptionGroupLocaleID(Setting.KButtonGroup), "Przyciski" },
        { setting.GetOptionGroupLocaleID(Setting.KToggleGroup), $"Lumina {GlobalPaths.Version}" },
        { setting.GetOptionGroupLocaleID(Setting.KSliderGroup), "Przełączniki funkcji" },
        { setting.GetOptionGroupLocaleID(Setting.KDropdownGroup), "Listy rozwijane" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Automatyczne zapisywanie" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LuminaByNyoko)), $"Lumina {GlobalPaths.Version} autorstwa Nyoko" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Pobierz wszystkie pakiety po restarcie" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Określa, czy po restarcie pobrać wszystkie pakiety (Cubemapy, LUTy). Jeśli włączone, Lumina załaduje wszystkie pakiety przy każdym restarcie gry, aby zapewnić najnowsze wersje i usunąć tekstury, które nie załadowały się poprawnie." },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Automatycznie zapisuje ustawienia po otwarciu lub zamknięciu interfejsu użytkownika." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Guides)), "Dokumentacja Lumina" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Guides)), "Otwiera przewodniki Lumina z informacjami i dokumentacją w nowej karcie." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Support)), "Uzyskaj wsparcie" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Support)), "Masz problemy? Kliknij ten przycisk, aby utworzyć archiwum ZIP z logami i ustawieniami, otworzyć folder z plikiem oraz automatycznie otworzyć zaproszenie na Discord, aby dołączyć do kanału wsparcia." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Donate)), "Wspomóż" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Donate)), "Otwiera kreatora donacji PayPal w nowej karcie. Dziękuję za rozważenie wsparcia!" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Otwórz zapisany plik ustawień" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Otwiera zapisany plik ustawień w nowej karcie." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Użyj suwaka czasu dnia" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Wyświetla suwak czasu dnia do przełączania między dniem a nocą. Domyślnie wyłączony, jeśli wykryto Weather Plus lub inny tryb zmiany czasu." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OtherSettings)), "Więcej opcji można dostosować bezpośrednio w interfejsie gry." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Regulacja szerokości i długości geograficznej" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Włącz, aby umożliwić ręczne zmiany szerokości i długości geograficznej przez interfejs gry." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Metro Framework" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Włącza MetroFramework dla Lumina, zapewniając bardziej nowoczesny interfejs użytkownika. Domyślnie włączone. Dla najlepszych efektów włącz opcję okna na pełnym ekranie." },
    };

    public void Unload()
    {
    }
}
