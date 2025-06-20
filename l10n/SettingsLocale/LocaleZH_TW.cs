using Colossal;
using Lumina;
using Lumina.XML;
using System.Collections.Generic;

/// <summary>
/// 繁體中文語言資源。
/// </summary>
public class LocaleZHTW : IDictionarySource
{
    private readonly Setting setting;

    /// <summary>
    /// 初始化 <see cref="LocaleZHTW"/> 類的新實例。
    /// </summary>
    /// <param name="setting">設定參數。</param>
    public LocaleZHTW(Setting setting)
    {
        this.setting = setting;
    }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts) => new Dictionary<string, string>
    {
        { setting.GetSettingsLocaleID(), "Lumina" },
        { setting.GetOptionTabLocaleID(Setting.KSection), "基礎" },
        { setting.GetOptionGroupLocaleID(Setting.KButtonGroup), "按鈕" },
        { setting.GetOptionGroupLocaleID(Setting.KToggleGroup), $"Lumina {GlobalPaths.Version}" },
        { setting.GetOptionGroupLocaleID(Setting.KSliderGroup), "功能切換" },
        { setting.GetOptionGroupLocaleID(Setting.KDropdownGroup), "下拉選單" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "自動儲存" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LuminaByNyoko)), $"Lumina {GlobalPaths.Version} 由 Nyoko 製作" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "重啟時重新載入所有套件" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "決定是否在重啟時取得所有套件（立方體貼圖、LUT）。啟用後，每次遊戲重啟時 Lumina 將重新載入所有套件，確保使用最新版本並清理未正確載入的貼圖。" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "當介面開啟或關閉時自動儲存設定。" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Guides)), "Lumina 文件" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Guides)), "在新分頁開啟 Lumina 指南，查看資訊與文件。" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Support)), "取得支援" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Support)), "遇到問題？點擊此按鈕建立您的日誌與設定壓縮檔，開啟資料夾找到檔案，並自動開啟我們的 Discord 邀請連結，方便加入支援頻道。" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Donate)), "捐款" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Donate)), "在新分頁開啟 PayPal 捐款精靈。感謝您的支持！" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "打開已儲存設定檔" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "在新分頁打開已儲存的設定檔。" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "使用時間滑桿" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "顯示一個時間滑桿，切換白天與夜晚。若偵測到 Weather Plus 或其他時間變更模組，預設關閉此功能。" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OtherSettings)), "更多選項可直接於遊戲介面調整。" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "緯度與經度調整" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "啟用此選項以允許透過遊戲介面手動調整緯度與經度。" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Metro Framework" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "啟用 MetroFramework 以提供更現代的 UI 體驗。預設啟用。建議開啟全螢幕無邊視窗模式以獲得最佳效果。" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "啟用 Lumina 體積效果" },
{ setting.GetOptionDescLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "此功能為 Lumina 正常運作所必需。它會啟用 Unity HDRP 的體積效果，並套用自訂的光照與後製處理設定。這可讓 Lumina 的進階視覺效果（如色彩校正與色調對映）在遊戲中正確執行。" },

 

        {setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.UseRoadTextures)), "使用道路紋理" },
        {setting.GetOptionDescLocaleID(nameof(Lumina.Setting.UseRoadTextures)), "啟用自訂道路紋理並調整亮度、不透明度與平滑度。" }

    };

    public void Unload()
    {
    }
}
