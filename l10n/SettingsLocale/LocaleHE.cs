using Colossal;
using Lumina;
using Lumina.XML;
using System.Collections.Generic;

/// <summary>
/// מקור תרגום לעברית.
/// </summary>
public class LocaleHE : IDictionarySource
{
    private readonly Setting setting;

    /// <summary>
    /// יוצר מופע חדש של המחלקה <see cref="LocaleHE"/>.
    /// </summary>
    /// <param name="setting">פרמטר הגדרות.</param>
    public LocaleHE(Setting setting)
    {
        this.setting = setting;
    }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts) => new Dictionary<string, string>
    {
        { setting.GetSettingsLocaleID(), "לומינה" },
        { setting.GetOptionTabLocaleID(Setting.KSection), "בסיסי" },
        { setting.GetOptionGroupLocaleID(Setting.KButtonGroup), "כפתורים" },
        { setting.GetOptionGroupLocaleID(Setting.KToggleGroup), $"לומינה {GlobalPaths.Version}" },
        { setting.GetOptionGroupLocaleID(Setting.KSliderGroup), "הפעלות תכונה" },
        { setting.GetOptionGroupLocaleID(Setting.KDropdownGroup), "תפריטים נפתחים" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "שמור אוטומטית" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LuminaByNyoko)), $"לומינה {GlobalPaths.Version} מאת Nyoko" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "טען את כל החבילות מחדש באתחול" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "קובע אם יש לטעון את כל החבילות (Cubemaps, LUTs) מחדש בעת אתחול. כשהאפשרות פעילה, לומינה תטען את כל החבילות מחדש בכל אתחול המשחק, ותבטיח שימוש בגרסאות העדכניות ביותר ותנקה טקסטורות שלא נטענו כראוי." },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "שומר את ההגדרות שלך אוטומטית בעת פתיחה או סגירה של הממשק." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Guides)), "מדריכי לומינה" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Guides)), "פותח מדריכי לומינה למידע ותיעוד בכרטיסיה חדשה." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Support)), "קבל תמיכה" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Support)), "נתקל בבעיות? לחץ על כפתור זה כדי ליצור קובץ ZIP של הלוגים וההגדרות שלך, יפתח את התיקייה למציאתו, ויפתח אוטומטית את הזמנת דיסקורד שלנו כדי שתוכל להצטרף לערוץ התמיכה." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Donate)), "תרום" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Donate)), "פותח את אשף התרומות של PayPal בכרטיסיה חדשה. תודה על התמיכה!" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "פתח את קובץ ההגדרות השמור" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "פותח את קובץ ההגדרות השמור בכרטיסיה חדשה." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "השתמש במחליק זמן היום" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "מציג מחליק זמן יום למעבר בין יום ולילה. כבוי כברירת מחדל אם מזוהה Weather Plus או מצב שינוי זמן אחר." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OtherSettings)), "אפשרויות נוספות ניתנות לשינוי ישירות בממשק המשחק." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "התאמות קו רוחב וקו אורך" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "אפשר זאת כדי לאפשר שינוי ידני של קווי רוחב ואורך דרך ממשק המשחק." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "מסגרת Metro" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "מאפשרת את מסגרת Metro ללומינה, המציעה חווית ממשק מודרנית יותר. מופעל כברירת מחדל. יש להפעיל את אפשרות חלון מלא כדי לקבל תוצאות מיטביות." },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "הפעלת נפח Lumina" },
{ setting.GetOptionDescLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "נדרש כדי שלומינה תעבוד כראוי. מפעיל נפח Unity HDRP עם התאמות אישיות של תאורה ועיבוד פוסט. זה מאפשר לאפקטים חזותיים מתקדמים של Lumina, כמו תיקון צבע ומיפוי גוונים, לפעול כראוי בתוך המשחק." },



    };

    public void Unload()
    {
    }
}
