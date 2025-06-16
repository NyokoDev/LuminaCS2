using Colossal;
using Lumina;
using Lumina.XML;
using System.Collections.Generic;

/// <summary>
/// Nguồn ngôn ngữ cho tiếng Việt.
/// </summary>
public class LocaleVI : IDictionarySource
{
    private readonly Setting setting;

    /// <summary>
    /// Khởi tạo một thể hiện mới của <see cref="LocaleVI"/>.
    /// </summary>
    /// <param name="setting">Tham số thiết lập.</param>
    public LocaleVI(Setting setting)
    {
        this.setting = setting;
    }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts) => new Dictionary<string, string>
{
    { setting.GetSettingsLocaleID(), "Lumina" },
    { setting.GetOptionTabLocaleID(Setting.KSection), "CƠ BẢN" },
    { setting.GetOptionGroupLocaleID(Setting.KButtonGroup), "Nút bấm" },
    { setting.GetOptionGroupLocaleID(Setting.KToggleGroup), $"Lumina {GlobalPaths.Version}" },
    { setting.GetOptionGroupLocaleID(Setting.KSliderGroup), "Chuyển đổi tính năng" },
    { setting.GetOptionGroupLocaleID(Setting.KDropdownGroup), "Danh sách thả xuống" },

    { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Tự động lưu" },
    { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LuminaByNyoko)), $"Lumina {GlobalPaths.Version} bởi Nyoko" },
    { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Tải lại tất cả gói khi khởi động lại" },
    { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Quyết định có lấy lại tất cả các gói (Cubemaps, LUTs) khi khởi động lại không. Khi bật, Lumina sẽ tải lại tất cả các gói mỗi lần game khởi động lại, đảm bảo sử dụng phiên bản mới nhất và làm sạch các texture tải không thành công." },
    { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Tự động lưu cài đặt khi giao diện người dùng được mở hoặc đóng." },

    { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Guides)), "Tài liệu Lumina" },
    { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Guides)), "Mở các hướng dẫn Lumina để xem thông tin và tài liệu trong tab mới." },

    { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Support)), "Nhận hỗ trợ" },
    { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Support)), "Gặp vấn đề? Nhấn nút này để tạo file ZIP chứa nhật ký và cài đặt của bạn, mở thư mục để tìm, và tự động mở liên kết Discord của chúng tôi để bạn có thể tham gia kênh hỗ trợ." },

    { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Donate)), "Ủng hộ" },
    { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Donate)), "Mở trình hỗ trợ quyên góp PayPal trong tab mới. Cảm ơn bạn đã cân nhắc ủng hộ tôi!" },

    { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Mở file cài đặt đã lưu" },
    { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Mở file cài đặt đã lưu trong tab mới." },

    { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Sử dụng thanh trượt thời gian trong ngày" },
    { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Hiển thị thanh trượt thời gian trong ngày để chuyển đổi giữa ngày và đêm. Mặc định tắt nếu phát hiện Weather Plus hoặc chế độ thay đổi thời gian khác." },

    { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OtherSettings)), "Có thể điều chỉnh nhiều tùy chọn hơn trực tiếp trong giao diện game." },

    { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Điều chỉnh vĩ độ và kinh độ" },
    { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Bật để cho phép thay đổi vĩ độ và kinh độ thủ công qua giao diện trong game." },

    { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Metro Framework" },
    { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Bật MetroFramework cho Lumina, cho trải nghiệm giao diện người dùng hiện đại hơn. Mặc định bật. Tùy chọn cửa sổ toàn màn hình phải bật để có kết quả tốt nhất." },
};

    public void Unload()
    {
    }
}
