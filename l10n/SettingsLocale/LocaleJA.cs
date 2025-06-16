using Colossal;
using Lumina;
using Lumina.XML;
using System.Collections.Generic;

/// <summary>
/// 日本語用ロケールソース。
/// </summary>
public class LocaleJA : IDictionarySource
{
    private readonly Setting setting;

    /// <summary>
    /// <see cref="LocaleJA"/> クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="setting">設定パラメータ。</param>
    public LocaleJA(Setting setting)
    {
        this.setting = setting;
    }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts) => new Dictionary<string, string>
    {
        { setting.GetSettingsLocaleID(), "ルミナ" },
        { setting.GetOptionTabLocaleID(Setting.KSection), "基本" },
        { setting.GetOptionGroupLocaleID(Setting.KButtonGroup), "ボタン" },
        { setting.GetOptionGroupLocaleID(Setting.KToggleGroup), $"Lumina {GlobalPaths.Version}" },
        { setting.GetOptionGroupLocaleID(Setting.KSliderGroup), "機能トグル" },
        { setting.GetOptionGroupLocaleID(Setting.KDropdownGroup), "ドロップダウン" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "自動保存" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LuminaByNyoko)), $"Lumina {GlobalPaths.Version} by Nyoko" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "再起動時にすべてのパッケージを再取得" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "再起動時にすべてのパッケージ（キューブマップ、LUT）を取得するかどうかを決定します。有効にすると、ゲーム再起動時に常に最新のパッケージを読み込み、正しく読み込まれなかったテクスチャをクリアします。" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "UIを開閉した際に設定を自動で保存します。" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Guides)), "Lumina ドキュメント" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Guides)), "新しいタブでLuminaの情報とドキュメントを開きます。" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Support)), "サポートを受ける" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Support)), "問題が発生していますか？このボタンをクリックするとログと設定のZIPが作成され、フォルダーが開きます。自動でDiscord招待も開き、サポートチャンネルに参加できます。" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Donate)), "寄付する" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Donate)), "PayPalの寄付ウィザードを新しいタブで開きます。サポートしていただけると嬉しいです！" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "保存済み設定ファイルを開く" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "保存された設定ファイルを新しいタブで開きます。" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "時間帯スライダーを使用する" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "昼夜を切り替える時間帯スライダーを表示します。Weather Plusや他の時間変更モードが検出された場合はデフォルトで無効になります。" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OtherSettings)), "その他のオプションはゲーム内インターフェイスで直接調整できます。" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "緯度・経度の調整" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "ゲーム内インターフェイスから緯度と経度を手動で変更できるようにします。" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Metro Framework" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "LuminaのMetroFrameworkを有効にし、よりモダンなUI体験を提供します。デフォルトで有効です。ベストな結果のためにはフルスクリーンウィンドウモードを有効にしてください。" },

        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "Luminaボリュームを有効にする" },
{ setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "Luminaが正しく動作するために必要です。カスタムのライティングおよびポストプロセスのオーバーライドを適用するUnity HDRPボリュームを有効にします。これにより、色補正やトーンマッピングなど、Luminaの高度な視覚効果がゲーム内で正常に機能します。" },


    };

    public void Unload()
    {
    }
}
