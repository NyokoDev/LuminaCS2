using Colossal;
using Lumina;
using Lumina.XML;
using System.Collections.Generic;

/// <summary>
/// 한국어 로케일 소스입니다.
/// </summary>
public class LocaleKO : IDictionarySource
{
    private readonly Setting setting;

    /// <summary>
    /// <see cref="LocaleKO"/> 클래스의 새 인스턴스를 초기화합니다.
    /// </summary>
    /// <param name="setting">설정 파라미터.</param>
    public LocaleKO(Setting setting)
    {
        this.setting = setting;
    }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts) => new Dictionary<string, string>
    {
        { setting.GetSettingsLocaleID(), "루미나" },
        { setting.GetOptionTabLocaleID(Setting.KSection), "기본" },
        { setting.GetOptionGroupLocaleID(Setting.KButtonGroup), "버튼" },
        { setting.GetOptionGroupLocaleID(Setting.KToggleGroup), $"Lumina {GlobalPaths.Version}" },
        { setting.GetOptionGroupLocaleID(Setting.KSliderGroup), "기능 토글" },
        { setting.GetOptionGroupLocaleID(Setting.KDropdownGroup), "드롭다운" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "자동 저장" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LuminaByNyoko)), $"Lumina {GlobalPaths.Version} by Nyoko" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "재시작 시 모든 패키지 다시 불러오기" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "재시작 시 모든 패키지(큐브맵, LUT)를 다시 불러올지 여부를 결정합니다. 활성화하면 게임이 재시작될 때마다 모든 패키지를 다시 불러와 최신 버전을 사용하며, 제대로 로드되지 않은 텍스처를 정리합니다." },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "UI를 열거나 닫을 때 설정을 자동으로 저장합니다." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Guides)), "Lumina 문서" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Guides)), "새 탭에서 Lumina 가이드와 문서를 엽니다." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Support)), "지원 받기" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Support)), "문제가 있나요? 이 버튼을 클릭하면 로그와 설정 ZIP이 생성되고, 해당 폴더가 열리며, 자동으로 디스코드 초대가 열려 지원 채널에 참여할 수 있습니다." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Donate)), "기부하기" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Donate)), "PayPal 기부 위자드를 새 탭에서 엽니다. 지원해 주셔서 감사합니다!" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "저장된 설정 파일 열기" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "저장된 설정 파일을 새 탭에서 엽니다." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "시간대 슬라이더 사용" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "낮과 밤을 전환하는 시간대 슬라이더를 표시합니다. Weather Plus 또는 다른 시간 변경 모드가 감지되면 기본적으로 비활성화됩니다." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OtherSettings)), "기타 옵션은 게임 내 인터페이스에서 직접 조정할 수 있습니다." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "위도 및 경도 조정" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "게임 내 인터페이스를 통해 위도와 경도를 수동으로 변경할 수 있도록 활성화합니다." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Metro Framework" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Lumina에 MetroFramework를 활성화하여 더 현대적인 UI 경험을 제공합니다. 기본적으로 활성화되어 있습니다. 최상의 결과를 위해 전체 화면 창 모드를 사용해야 합니다." },


        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "Lumina 볼륨 활성화" },
{ setting.GetOptionDescLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "Lumina가 정상적으로 작동하려면 필요합니다. 사용자 지정 조명 및 후처리 오버라이드를 적용하는 Unity HDRP 볼륨을 활성화합니다. 이를 통해 색상 보정 및 톤 매핑과 같은 Lumina의 고급 시각 효과가 게임 내에서 제대로 작동합니다." },

{setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.EnableDebugLogging)), "디버그 로그 활성화" },
{setting.GetOptionDescLocaleID(nameof(Lumina.Setting.EnableDebugLogging)), "문제 해결을 위해 자세한 디버그 로그를 활성화합니다. 이로 인해 더 상세한 로그 파일이 생성되어 문제를 파악하는 데 도움이 될 수 있지만, 성능에 약간의 영향을 줄 수 있습니다." },
{setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.EnablePerformanceMode)), "성능 모드" },
{setting.GetOptionDescLocaleID(nameof(Lumina.Setting.EnablePerformanceMode)), "활성화하면 Lumina는 게임이 완전히 로드될 때까지 지형, 조명, 수면 시스템의 로드를 지연합니다. 이를 통해 메인 메뉴에서 지연이 줄어들고 게임 시작 시 더 부드러운 경험을 제공합니다. 단, 로딩 중에는 약간의 프레임 저하가 발생할 수 있습니다. 충돌이나 불안정이 발생하면 이 옵션을 비활성화하는 것을 고려하세요." },

    };

    public void Unload()
    {
    }
}
