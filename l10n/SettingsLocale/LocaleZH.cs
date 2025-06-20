using Colossal;
using Lumina;
using Lumina.XML;
using System.Collections.Generic;

/// <summary>
/// 简体中文语言资源。
/// </summary>
public class LocaleZH : IDictionarySource
{
    private readonly Setting setting;

    /// <summary>
    /// 初始化 <see cref="LocaleZH"/> 类的新实例。
    /// </summary>
    /// <param name="setting">配置参数。</param>
    public LocaleZH(Setting setting)
    {
        this.setting = setting;
    }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts) => new Dictionary<string, string>
    {
        { setting.GetSettingsLocaleID(), "Lumina" },
        { setting.GetOptionTabLocaleID(Setting.KSection), "基础" },
        { setting.GetOptionGroupLocaleID(Setting.KButtonGroup), "按钮" },
        { setting.GetOptionGroupLocaleID(Setting.KToggleGroup), $"Lumina {GlobalPaths.Version}" },
        { setting.GetOptionGroupLocaleID(Setting.KSliderGroup), "功能开关" },
        { setting.GetOptionGroupLocaleID(Setting.KDropdownGroup), "下拉菜单" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "自动保存" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LuminaByNyoko)), $"Lumina {GlobalPaths.Version}，由 Nyoko 制作" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "重启时重新加载所有包" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "决定是否在重启时获取所有包（立方体贴图、LUT）。启用后，每次游戏重启时 Lumina 会重新加载所有包，确保使用最新版本，并清理未正确加载的纹理。" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "当界面打开或关闭时自动保存设置。" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Guides)), "Lumina 文档" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Guides)), "在新标签页打开 Lumina 指南，查看信息和文档。" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Support)), "获取支持" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Support)), "遇到问题？点击此按钮创建日志和设置的压缩包，打开文件夹找到它，并自动打开我们的 Discord 邀请链接，方便加入支持频道。" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Donate)), "捐赠" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Donate)), "在新标签页打开 PayPal 捐赠向导。感谢您的支持！" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "打开已保存的设置文件" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "在新标签页打开已保存的设置文件。" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "使用时间滑块" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "显示一个时间滑块，可在白天和夜晚之间切换。如果检测到 Weather Plus 或其他时间变化模式，默认禁用此功能。" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OtherSettings)), "更多选项可在游戏内界面直接调整。" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "经纬度调整" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "启用后，可通过游戏内界面手动调整经纬度。" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Metro 框架" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "启用 MetroFramework 以提供更现代的 UI 体验。默认启用。建议启用全屏无边窗口模式以获得最佳效果。" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "启用 Lumina 体积效果" },
{ setting.GetOptionDescLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "Lumina 正常运行所必需。启用 Unity HDRP 体积效果，应用自定义的光照和后期处理设置。这样可以使 Lumina 的高级视觉效果（如色彩校正和色调映射）在游戏中正确发挥作用。" },




        {setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.UseRoadTextures)), "使用道路纹理" },
        {setting.GetOptionDescLocaleID(nameof(Lumina.Setting.UseRoadTextures)), "启用自定义道路纹理，并可调整亮度、不透明度和平滑度。" }

    };

    public void Unload()
    {
    }
}
