using Colossal;
using Lumina;
using Lumina.XML;
using System.Collections.Generic;

/// <summary>
/// Türkçe için dil kaynağı.
/// </summary>
public class LocaleTR : IDictionarySource
{
    private readonly Setting setting;

    /// <summary>
    /// <see cref="LocaleTR"/> sınıfının yeni bir örneğini başlatır.
    /// </summary>
    /// <param name="setting">Ayar parametresi.</param>
    public LocaleTR(Setting setting)
    {
        this.setting = setting;
    }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts) => new Dictionary<string, string>
    {
        { setting.GetSettingsLocaleID(), "Lumina" },
        { setting.GetOptionTabLocaleID(Setting.KSection), "TEMEL" },
        { setting.GetOptionGroupLocaleID(Setting.KButtonGroup), "Düğmeler" },
        { setting.GetOptionGroupLocaleID(Setting.KToggleGroup), $"Lumina {GlobalPaths.Version}" },
        { setting.GetOptionGroupLocaleID(Setting.KSliderGroup), "Özellik Anahtarları" },
        { setting.GetOptionGroupLocaleID(Setting.KDropdownGroup), "Açılır Menüler" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Otomatik Kaydet" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LuminaByNyoko)), $"Lumina {GlobalPaths.Version} by Nyoko" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Yeniden Başlatmada Tüm Paketleri Yükle" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Yeniden başlatıldığında tüm paketlerin (Cubemaps, LUTs) yüklenip yüklenmeyeceğini belirler. Etkinleştirildiğinde, Lumina her oyun yeniden başlatıldığında tüm paketleri yeniden yükler, böylece en son sürümlerin kullanılması sağlanır ve düzgün yüklenmeyen dokular temizlenir." },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "UI açıldığında veya kapandığında ayarlarınızı otomatik olarak kaydeder." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Guides)), "Lumina Belgeleri" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Guides)), "Bilgi ve dokümantasyon için Lumina rehberlerini yeni bir sekmede açar." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Support)), "Destek Al" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Support)), "Sorun mu yaşıyorsunuz? Günlüklerinizin ve ayarlarınızın bir ZIP dosyasını oluşturmak için bu düğmeye tıklayın, dosyanın bulunduğu klasörü açar ve destek kanalımıza katılmanız için Discord davetimizi otomatik olarak açar." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Donate)), "Bağış Yap" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Donate)), "PayPal bağış sihirbazını yeni bir sekmede açar. Desteklemeyi düşündüğünüz için teşekkürler!" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Kaydedilen Ayar Dosyasını Aç" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Kaydedilen ayar dosyasını yeni bir sekmede açar." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Günün Zamanı Kaydırıcısını Kullan" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Gündüz ve gece arasında geçiş yapmak için bir zaman kaydırıcısı gösterir. Weather Plus veya başka bir zaman değiştirme modu algılanırsa varsayılan olarak devre dışı bırakılır." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OtherSettings)), "Daha fazla seçenek oyun içi arayüzden doğrudan ayarlanabilir." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Enlem ve Boylam Ayarları" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Oyun içi arayüzden manuel enlem ve boylam değişikliklerine izin vermek için bunu etkinleştirin." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Metro Framework" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Lumina için MetroFramework’ü etkinleştirir ve daha modern bir kullanıcı arayüzü deneyimi sağlar. Bu varsayılan olarak etkinleştirilmiştir. En iyi sonuçlar için Tam Ekran Pencere modunun etkin olması gerekir." },


        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "Lumina Hacmini Etkinleştir" },
{ setting.GetOptionDescLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "Lumina’nın düzgün çalışması için gereklidir. Özel aydınlatma ve post-processing ayarları uygulayan Unity HDRP hacmini etkinleştirir. Bu sayede Lumina’nın gelişmiş görsel efektleri, renk düzeltme ve ton eşleme gibi, oyunda doğru şekilde çalışır." },


    };

    public void Unload()
    {
    }
}
