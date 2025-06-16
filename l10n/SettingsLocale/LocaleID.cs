using Colossal;
using Lumina;
using Lumina.XML;
using System.Collections.Generic;

/// <summary>
/// Sumber locale untuk Bahasa Indonesia.
/// </summary>
public class LocaleID : IDictionarySource
{
    private readonly Setting setting;

    /// <summary>
    /// Membuat instance baru dari kelas <see cref="LocaleID"/>.
    /// </summary>
    /// <param name="setting">Parameter setting.</param>
    public LocaleID(Setting setting)
    {
        this.setting = setting;
    }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts) => new Dictionary<string, string>
    {
        { setting.GetSettingsLocaleID(), "Lumina" },
        { setting.GetOptionTabLocaleID(Setting.KSection), "DASAR" },
        { setting.GetOptionGroupLocaleID(Setting.KButtonGroup), "Tombol" },
        { setting.GetOptionGroupLocaleID(Setting.KToggleGroup), $"Lumina {GlobalPaths.Version}" },
        { setting.GetOptionGroupLocaleID(Setting.KSliderGroup), "Pengalih Fitur" },
        { setting.GetOptionGroupLocaleID(Setting.KDropdownGroup), "Dropdown" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Simpan otomatis" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LuminaByNyoko)), $"Lumina {GlobalPaths.Version} oleh Nyoko" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Muat ulang semua paket saat restart" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Menentukan apakah semua paket (Cubemaps, LUTs) dimuat ulang saat restart. Jika diaktifkan, Lumina akan memuat ulang semua paket setiap kali game dimulai ulang, memastikan versi terbaru digunakan dan membersihkan tekstur yang tidak dimuat dengan benar." },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Menyimpan pengaturan secara otomatis saat UI dibuka atau ditutup." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Guides)), "Dokumentasi Lumina" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Guides)), "Membuka panduan Lumina untuk informasi dan dokumentasi di tab baru." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Support)), "Dapatkan Dukungan" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Support)), "Mengalami masalah? Klik tombol ini untuk membuat ZIP dari log dan pengaturan Anda, membuka folder untuk menemukannya, dan secara otomatis membuka undangan Discord kami sehingga Anda dapat bergabung dengan saluran dukungan kami." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Donate)), "Donasi" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Donate)), "Membuka wizard donasi PayPal di tab baru. Terima kasih telah mempertimbangkan untuk mendukung saya!" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Buka File Pengaturan yang Disimpan" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Membuka file pengaturan yang disimpan di tab baru." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Gunakan Slider Waktu Hari" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Menampilkan slider waktu hari untuk beralih antara siang dan malam. Dinonaktifkan secara default jika Weather Plus atau mode pengubah waktu lain terdeteksi." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OtherSettings)), "Opsi lainnya dapat disesuaikan langsung di antarmuka dalam game." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Penyesuaian Lintang dan Bujur" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Aktifkan ini untuk mengizinkan perubahan lintang dan bujur secara manual melalui antarmuka dalam game." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Metro Framework" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Mengaktifkan MetroFramework untuk Lumina, memungkinkan pengalaman UI yang lebih modern. Ini diaktifkan secara default. Opsi Jendela Layar Penuh harus diaktifkan untuk hasil terbaik." },

        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "Aktifkan Volume Lumina" },
{ setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "Diperlukan agar Lumina berfungsi dengan benar. Mengaktifkan volume Unity HDRP yang menerapkan pengaturan pencahayaan dan post-processing khusus. Ini memungkinkan efek visual canggih Lumina, seperti koreksi warna dan tone mapping, bekerja dengan baik dalam permainan." },


    };

    public void Unload()
    {
    }
}
