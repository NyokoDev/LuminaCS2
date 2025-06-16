using Colossal;
using Lumina;
using Lumina.XML;
using System.Collections.Generic;

/// <summary>
/// Πηγή μεταφράσεων για τα Ελληνικά.
/// </summary>
public class LocaleEL : IDictionarySource
{
    private readonly Setting setting;

    /// <summary>
    /// Δημιουργεί ένα νέο στιγμιότυπο της κλάσης <see cref="LocaleEL"/>.
    /// </summary>
    /// <param name="setting">Ρυθμίσεις.</param>
    public LocaleEL(Setting setting)
    {
        this.setting = setting;
    }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts) => new Dictionary<string, string>
    {
        { setting.GetSettingsLocaleID(), "Lumina" },
        { setting.GetOptionTabLocaleID(Setting.KSection), "ΒΑΣΙΚΑ" },
        { setting.GetOptionGroupLocaleID(Setting.KButtonGroup), "Κουμπιά" },
        { setting.GetOptionGroupLocaleID(Setting.KToggleGroup), $"Lumina {GlobalPaths.Version}" },
        { setting.GetOptionGroupLocaleID(Setting.KSliderGroup), "Εναλλαγή λειτουργιών" },
        { setting.GetOptionGroupLocaleID(Setting.KDropdownGroup), "Αναπτυσσόμενα" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Αυτόματη αποθήκευση" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LuminaByNyoko)), $"Lumina {GlobalPaths.Version} από τον Nyoko" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Επαναφόρτωση όλων κατά την επανεκκίνηση" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Καθορίζει αν θα φορτώνονται όλα τα πακέτα (Cubemaps, LUTs) κατά την επανεκκίνηση. Εάν είναι ενεργό, το Lumina θα φορτώνει εκ νέου όλα τα πακέτα κάθε φορά που ξεκινάει το παιχνίδι, διασφαλίζοντας τις πιο πρόσφατες εκδόσεις και καθαρίζοντας τυχόν προβληματικές υφές." },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Αποθηκεύει τις ρυθμίσεις σας αυτόματα όταν ανοίγει ή κλείνει το UI." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Guides)), "Οδηγοί Lumina" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Guides)), "Ανοίγει τους οδηγούς Lumina για πληροφορίες και τεκμηρίωση σε νέα καρτέλα." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Support)), "Λήψη υποστήριξης" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Support)), "Αν αντιμετωπίζετε προβλήματα, κάντε κλικ σε αυτό το κουμπί για να δημιουργήσετε ένα ZIP με τα αρχεία καταγραφής και τις ρυθμίσεις σας. Θα ανοίξει ο φάκελος με το αρχείο και θα ανοίξει αυτόματα ο σύνδεσμος Discord για το κανάλι υποστήριξης." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Donate)), "Δωρεά" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Donate)), "Ανοίγει τον οδηγό δωρεάς μέσω PayPal σε νέα καρτέλα. Ευχαριστώ που σκέφτεστε να με υποστηρίξετε!" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Άνοιγμα αποθηκευμένου αρχείου ρυθμίσεων" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Ανοίγει το αποθηκευμένο αρχείο ρυθμίσεων σε νέα καρτέλα." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Χρήση ρυθμιστικού ώρας ημέρας" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Εμφανίζει ρυθμιστικό για εναλλαγή μεταξύ ημέρας και νύχτας. Απενεργοποιημένο από προεπιλογή αν εντοπιστεί το Weather Plus ή άλλο mod αλλαγής ώρας." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OtherSettings)), "Περισσότερες επιλογές μπορούν να ρυθμιστούν μέσα στο παιχνίδι." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Προσαρμογές γεωγραφικού πλάτους και μήκους" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Ενεργοποιήστε αυτήν την επιλογή για να επιτρέψετε χειροκίνητες αλλαγές γεωγραφικού πλάτους και μήκους από την in-game διεπαφή." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Metro Framework" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Ενεργοποιεί το MetroFramework στο Lumina για πιο σύγχρονη εμπειρία χρήσης. Ενεργοποιημένο από προεπιλογή. Συνιστάται χρήση σε λειτουργία παραθύρου πλήρους οθόνης για τα καλύτερα αποτελέσματα." },
       
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "Ενεργοποίηση Lumina Volume" },
{ setting.GetOptionDescLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "Απαραίτητο για τη σωστή λειτουργία του Lumina. Ενεργοποιεί ένα Unity HDRP volume που εφαρμόζει προσαρμοσμένες ρυθμίσεις φωτισμού και επεξεργασίας μετά την εικόνα. Αυτό επιτρέπει στα προηγμένα οπτικά εφέ του Lumina, όπως διόρθωση χρώματος και τονισμό τόνων, να λειτουργούν σωστά μέσα στο παιχνίδι." },


    };

    public void Unload()
    {
    }
}
