using Colossal;
using Lumina;
using Lumina.XML;
using System.Collections.Generic;

/// <summary>
/// Fonte di localizzazione per l'italiano.
/// </summary>
public class LocaleIT : IDictionarySource
{
    private readonly Setting setting;

    /// <summary>
    /// Inizializza una nuova istanza della classe <see cref="LocaleIT"/>.
    /// </summary>
    /// <param name="setting">Parametro di configurazione.</param>
    public LocaleIT(Setting setting)
    {
        this.setting = setting;
    }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts) => new Dictionary<string, string>
    {
        { setting.GetSettingsLocaleID(), "Lumina" },
        { setting.GetOptionTabLocaleID(Setting.KSection), "BASE" },
        { setting.GetOptionGroupLocaleID(Setting.KButtonGroup), "Pulsanti" },
        { setting.GetOptionGroupLocaleID(Setting.KToggleGroup), $"Lumina {GlobalPaths.Version}" },
        { setting.GetOptionGroupLocaleID(Setting.KSliderGroup), "Interruttori Funzioni" },
        { setting.GetOptionGroupLocaleID(Setting.KDropdownGroup), "Menu a tendina" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Salva automaticamente" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LuminaByNyoko)), $"Lumina {GlobalPaths.Version} di Nyoko" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Ricarica tutti i pacchetti al riavvio" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Determina se ricaricare tutti i pacchetti (Cubemaps, LUT) al riavvio. Se abilitato, Lumina ricaricherà tutti i pacchetti ogni volta che il gioco si riavvia, assicurandosi che vengano usate le versioni più recenti e pulendo eventuali texture non caricate correttamente." },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Salva le impostazioni automaticamente all'apertura o chiusura dell'interfaccia utente." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Guides)), "Documentazione Lumina" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Guides)), "Apre le guide Lumina per informazioni e documentazione in una nuova scheda." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Support)), "Ottieni Supporto" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Support)), "Hai problemi? Clicca questo pulsante per creare un archivio ZIP dei tuoi log e impostazioni, apre la cartella per trovarlo, e apre automaticamente il nostro invito Discord per unirti al canale di supporto." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Donate)), "Dona" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Donate)), "Apre la procedura guidata di donazione PayPal in una nuova scheda. Grazie per aver considerato di supportarmi!" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Apri file impostazioni salvate" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Apre il file delle impostazioni salvate in una nuova scheda." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Usa cursore Ora del Giorno" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Mostra un cursore per passare tra giorno e notte. Disabilitato di default se viene rilevato Weather Plus o altra modalità di cambio tempo." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OtherSettings)), "Altre opzioni possono essere modificate direttamente nell'interfaccia in-game." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Regolazioni Latitudine e Longitudine" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Abilita questa opzione per consentire modifiche manuali di latitudine e longitudine tramite l'interfaccia in-game." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Metro Framework" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Abilita MetroFramework per Lumina, offrendo un'esperienza UI più moderna. È abilitato di default. L'opzione Finestra a schermo intero deve essere attivata per risultati ottimali." },

        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "Abilita Lumina Volume" },
{ setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "Necessario per il corretto funzionamento di Lumina. Attiva un volume Unity HDRP che applica override personalizzati di illuminazione e post-processing. Questo permette agli effetti visivi avanzati di Lumina, come la correzione del colore e il tone mapping, di funzionare correttamente nel gioco." },


    };

    public void Unload()
    {
    }
}
