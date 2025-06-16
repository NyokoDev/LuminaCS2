using Colossal;
using Lumina;
using Lumina.XML;
using System.Collections.Generic;

/// <summary>
/// Lokalisierungsquelle für Deutsch.
/// </summary>
public class LocaleDE : IDictionarySource
{
    private readonly Setting setting;

    /// <summary>
    /// Erstellt eine neue Instanz der Klasse <see cref="LocaleDE"/>.
    /// </summary>
    /// <param name="setting">Einstellungen.</param>
    public LocaleDE(Setting setting)
    {
        this.setting = setting;
    }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts) => new Dictionary<string, string>
    {
        { setting.GetSettingsLocaleID(), "Lumina" },
        { setting.GetOptionTabLocaleID(Setting.KSection), "BASIS" },
        { setting.GetOptionGroupLocaleID(Setting.KButtonGroup), "Schaltflächen" },
        { setting.GetOptionGroupLocaleID(Setting.KToggleGroup), $"Lumina {GlobalPaths.Version}" },
        { setting.GetOptionGroupLocaleID(Setting.KSliderGroup), "Feature Umschalter" },
        { setting.GetOptionGroupLocaleID(Setting.KDropdownGroup), "Dropdowns" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Automatisch speichern" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LuminaByNyoko)), $"Lumina {GlobalPaths.Version} von Nyoko" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Alle Pakete beim Neustart neu laden" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Bestimmt, ob alle Pakete (Cubemaps, LUTs) beim Neustart geladen werden. Wenn aktiviert, lädt Lumina alle Pakete bei jedem Spielstart neu, um sicherzustellen, dass die neuesten Versionen verwendet werden und nicht richtig geladene Texturen bereinigt werden." },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Speichert deine Einstellungen automatisch, wenn die Benutzeroberfläche geöffnet oder geschlossen wird." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Guides)), "Lumina-Anleitungen" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Guides)), "Öffnet Lumina-Anleitungen für Informationen und Dokumentation in einem neuen Tab." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Support)), "Unterstützung erhalten" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Support)), "Probleme? Klicke diesen Button, um ein ZIP mit deinen Protokollen und Einstellungen zu erstellen, öffnet den Ordner zum Auffinden, und öffnet automatisch unsere Discord-Einladung, damit du unserem Support-Kanal beitreten kannst." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Donate)), "Spenden" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Donate)), "Öffnet den PayPal-Spendenassistenten in einem neuen Tab. Danke, dass du meine Arbeit unterstützen möchtest!" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Gespeicherte Einstellungsdatei öffnen" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Öffnet die gespeicherte Einstellungsdatei in einem neuen Tab." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Zeit des Tages Schieberegler verwenden" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Zeigt einen Schieberegler zur Umschaltung zwischen Tag und Nacht. Standardmäßig deaktiviert, wenn Weather Plus oder ein anderes Zeitänderungs-Mod erkannt wird." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OtherSettings)), "Weitere Optionen können direkt im Spielmenü angepasst werden." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Anpassungen von Breiten- und Längengraden" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Aktiviere dies, um manuelle Änderungen von Breiten- und Längengraden über die Spieloberfläche zu erlauben." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Metro Framework" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Aktiviert das MetroFramework für Lumina, um eine modernere Benutzeroberfläche zu ermöglichen. Standardmäßig aktiviert. Für beste Ergebnisse sollte die Vollbild-Fenstermodusoption aktiviert sein." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "Lumina Volume aktivieren" },
{ setting.GetOptionDescLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "Erforderlich, damit Lumina korrekt funktioniert. Aktiviert ein Unity HDRP-Volume, das benutzerdefinierte Beleuchtungs- und Post-Processing-Überschreibungen anwendet. Dadurch können Luminas erweiterte visuelle Effekte wie Farbkorrektur und Tonemapping im Spiel korrekt dargestellt werden." },

    };

    public void Unload()
    {
    }
}
