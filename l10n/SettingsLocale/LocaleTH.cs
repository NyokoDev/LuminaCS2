using Colossal;
using Lumina;
using Lumina.XML;
using System.Collections.Generic;

/// <summary>
/// แหล่งข้อมูลภาษาสำหรับภาษาไทย
/// </summary>
public class LocaleTH : IDictionarySource
{
    private readonly Setting setting;

    /// <summary>
    /// สร้างอินสแตนซ์ใหม่ของคลาส <see cref="LocaleTH"/>
    /// </summary>
    /// <param name="setting">พารามิเตอร์การตั้งค่า</param>
    public LocaleTH(Setting setting)
    {
        this.setting = setting;
    }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts) => new Dictionary<string, string>
    {
        { setting.GetSettingsLocaleID(), "Lumina" },
        { setting.GetOptionTabLocaleID(Setting.KSection), "พื้นฐาน" },
        { setting.GetOptionGroupLocaleID(Setting.KButtonGroup), "ปุ่ม" },
        { setting.GetOptionGroupLocaleID(Setting.KToggleGroup), $"Lumina {GlobalPaths.Version}" },
        { setting.GetOptionGroupLocaleID(Setting.KSliderGroup), "สลับเปิด-ปิดฟีเจอร์" },
        { setting.GetOptionGroupLocaleID(Setting.KDropdownGroup), "รายการแบบเลื่อนลง" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "บันทึกอัตโนมัติ" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LuminaByNyoko)), $"Lumina {GlobalPaths.Version} โดย Nyoko" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "โหลดแพ็กเกจทั้งหมดเมื่อเริ่มเกมใหม่" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "กำหนดว่าจะโหลดแพ็กเกจทั้งหมด (Cubemaps, LUTs) เมื่อเริ่มเกมใหม่หรือไม่ เมื่อเปิดใช้งาน Lumina จะโหลดแพ็กเกจทั้งหมดทุกครั้งที่เริ่มเกมใหม่ เพื่อให้แน่ใจว่าใช้เวอร์ชันล่าสุดและล้างข้อมูลพื้นผิวที่โหลดไม่สมบูรณ์" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "บันทึกการตั้งค่าของคุณโดยอัตโนมัติเมื่อเปิดหรือปิด UI" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Guides)), "เอกสาร Lumina" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Guides)), "เปิดเอกสารแนะนำการใช้งาน Lumina ในแท็บใหม่" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Support)), "ขอรับการสนับสนุน" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Support)), "พบปัญหาใช่ไหม? กดปุ่มนี้เพื่อสร้างไฟล์ ZIP ของบันทึกและการตั้งค่าของคุณ เปิดโฟลเดอร์ที่เก็บ และเปิดเชิญ Discord โดยอัตโนมัติเพื่อเข้าร่วมช่องสนับสนุนของเรา" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Donate)), "บริจาค" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Donate)), "เปิดหน้าต่างบริจาค PayPal ในแท็บใหม่ ขอบคุณที่สนับสนุนผม!" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "เปิดไฟล์ตั้งค่าที่บันทึกไว้" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "เปิดไฟล์ตั้งค่าที่บันทึกไว้ในแท็บใหม่" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "ใช้แถบเลื่อนเวลาของวัน" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "แสดงแถบเลื่อนเพื่อสลับระหว่างกลางวันและกลางคืน ปิดใช้งานโดยค่าเริ่มต้นถ้าตรวจพบ Weather Plus หรือโหมดเปลี่ยนเวลาอื่น" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OtherSettings)), "ตัวเลือกเพิ่มเติมสามารถปรับได้โดยตรงในอินเทอร์เฟซเกม" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "ปรับละติจูดและลองจิจูด" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "เปิดใช้งานเพื่ออนุญาตให้ปรับละติจูดและลองจิจูดด้วยตนเองผ่านอินเทอร์เฟซในเกม" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Metro Framework" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "เปิดใช้งาน MetroFramework สำหรับ Lumina เพื่อประสบการณ์ UI ที่ทันสมัยยิ่งขึ้น เปิดใช้งานโดยค่าเริ่มต้น ต้องเปิดใช้งาน Full Screen Windowed เพื่อผลลัพธ์ที่ดีที่สุด" },
    };

    public void Unload()
    {
    }
}
