using Colossal;
using Lumina;
using Lumina.XML;
using System.Collections.Generic;

/// <summary>
/// Источник локализации для русского языка.
/// </summary>
public class LocaleRU : IDictionarySource
{
    private readonly Setting setting;

    /// <summary>
    /// Инициализирует новый экземпляр класса <see cref="LocaleRU"/>.
    /// </summary>
    /// <param name="setting">Параметр настройки.</param>
    public LocaleRU(Setting setting)
    {
        this.setting = setting;
    }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts) => new Dictionary<string, string>
    {
        { setting.GetSettingsLocaleID(), "Lumina" },
        { setting.GetOptionTabLocaleID(Setting.KSection), "ОСНОВНЫЕ" },
        { setting.GetOptionGroupLocaleID(Setting.KButtonGroup), "Кнопки" },
        { setting.GetOptionGroupLocaleID(Setting.KToggleGroup), $"Lumina {GlobalPaths.Version}" },
        { setting.GetOptionGroupLocaleID(Setting.KSliderGroup), "Переключатели функций" },
        { setting.GetOptionGroupLocaleID(Setting.KDropdownGroup), "Выпадающие списки" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Автосохранение" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LuminaByNyoko)), $"Lumina {GlobalPaths.Version} от Nyoko" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Загружать все пакеты при перезапуске" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "Определяет, следует ли загружать все пакеты (Кубемапы, LUT) при перезапуске. Если включено, Lumina будет загружать все пакеты каждый раз при запуске игры, гарантируя использование последних версий и очищая текстуры, которые не загрузились правильно." },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "Автоматически сохраняет настройки при открытии или закрытии интерфейса." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Guides)), "Документация Lumina" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Guides)), "Открывает руководства Lumina для информации и документации в новой вкладке." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Support)), "Получить поддержку" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Support)), "Проблемы? Нажмите эту кнопку, чтобы создать ZIP с вашими логами и настройками, открыть папку с ним, а также автоматически открыть приглашение в наш Discord для доступа к каналу поддержки." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Donate)), "Пожертвовать" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Donate)), "Открывает мастер пожертвований PayPal в новой вкладке. Спасибо за вашу поддержку!" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Открыть файл сохранённых настроек" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "Открывает файл сохранённых настроек в новой вкладке." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Использовать ползунок времени суток" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "Показывает ползунок времени суток для переключения между днём и ночью. По умолчанию отключён, если обнаружен Weather Plus или другой режим изменения времени." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OtherSettings)), "Больше настроек можно изменить напрямую в игровом интерфейсе." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Настройки широты и долготы" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "Включите, чтобы разрешить ручное изменение широты и долготы через игровой интерфейс." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Metro Framework" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "Включает MetroFramework для Lumina, обеспечивая более современный интерфейс. Включено по умолчанию. Рекомендуется включить полноэкранное окно для лучшей работы." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "Включить Lumina Volume" },
{ setting.GetOptionDescLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "Требуется для корректной работы Lumina. Включает Unity HDRP volume с пользовательскими настройками освещения и постобработки. Это позволяет продвинутым визуальным эффектам Lumina, таким как цветокоррекция и тональный маппинг, работать правильно в игре." },


    };

    public void Unload()
    {
    }
}
