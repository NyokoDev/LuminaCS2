using Colossal;
using Lumina;
using Lumina.XML;
using System.Collections.Generic;

/// <summary>
/// हिंदी के लिए लोकल सोर्स।
/// </summary>
public class LocaleHI : IDictionarySource
{
    private readonly Setting setting;

    /// <summary>
    /// <see cref="LocaleHI"/> क्लास का नया इंस्टेंस बनाता है।
    /// </summary>
    /// <param name="setting">सेटिंग पैरामीटर।</param>
    public LocaleHI(Setting setting)
    {
        this.setting = setting;
    }

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts) => new Dictionary<string, string>
    {
        { setting.GetSettingsLocaleID(), "लूमिना" },
        { setting.GetOptionTabLocaleID(Setting.KSection), "बेसिक" },
        { setting.GetOptionGroupLocaleID(Setting.KButtonGroup), "बटन" },
        { setting.GetOptionGroupLocaleID(Setting.KToggleGroup), $"लूमिना {GlobalPaths.Version}" },
        { setting.GetOptionGroupLocaleID(Setting.KSliderGroup), "फीचर टॉगल" },
        { setting.GetOptionGroupLocaleID(Setting.KDropdownGroup), "ड्रॉपडाउन" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "स्वचालित रूप से सहेजें" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LuminaByNyoko)), $"लूमिना {GlobalPaths.Version} द्वारा Nyoko" },
        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "रिस्टार्ट पर सभी पैकेज फिर से लोड करें" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.ReloadAllPackagesOnRestart)), "निर्धारित करता है कि क्या रिस्टार्ट पर सभी पैकेज (क्यूबमैप्स, LUTs) फिर से लोड किए जाएं। सक्षम होने पर, लूमिना हर गेम रिस्टार्ट पर सभी पैकेज फिर से लोड करेगा, यह सुनिश्चित करते हुए कि नवीनतम संस्करण उपयोग हो रहे हैं और कोई भी टेक्सचर जो सही से लोड नहीं हुआ, साफ़ हो जाएगा।" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.SaveAutomatically)), "जब UI खुलता या बंद होता है तो आपकी सेटिंग्स स्वतः सहेजता है।" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Guides)), "लूमिना दस्तावेज़" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Guides)), "जानकारी और दस्तावेज़ीकरण के लिए लूमिना गाइड्स को एक नए टैब में खोलता है।" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Support)), "सहायता प्राप्त करें" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Support)), "समस्याओं का सामना कर रहे हैं? इस बटन पर क्लिक करें ताकि आपकी लॉग्स और सेटिंग्स का ZIP बनाया जा सके, फ़ोल्डर खुल जाएगा ताकि आप इसे पा सकें, और यह हमारे डिसकॉड इनवाइट को स्वतः खोलेगा ताकि आप सपोर्ट चैनल में जुड़ सकें।" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.Donate)), "दान करें" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.Donate)), "PayPal डोनेशन विज़ार्ड को एक नए टैब में खोलता है। समर्थन के लिए धन्यवाद!" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "सहेजी गई सेटिंग्स फ़ाइल खोलें" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.OpenLocationButton)), "सहेजी गई सेटिंग्स फ़ाइल को एक नए टैब में खोलता है।" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "दिन के समय स्लाइडर का उपयोग करें" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.UseTimeOfDaySlider)), "दिन और रात के बीच स्विच करने के लिए एक दिन के समय का स्लाइडर दिखाता है। यदि Weather Plus या कोई अन्य समय परिवर्तन मोड सक्रिय है तो डिफ़ॉल्ट रूप से अक्षम।" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.OtherSettings)), "और विकल्प सीधे इन-गेम इंटरफ़ेस में समायोजित किए जा सकते हैं।" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "अक्षांश और देशांतर समायोजन" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.LatitudeAndLongitudeAdjustments)), "इन-गेम इंटरफ़ेस के माध्यम से अक्षांश और देशांतर को मैन्युअल रूप से बदलने की अनुमति देता है।" },

        { setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "मेट्रो फ्रेमवर्क" },
        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.MetroFrameworkEnabled)), "लूमिना के लिए मेट्रो फ्रेमवर्क सक्षम करता है, जो एक अधिक आधुनिक UI अनुभव प्रदान करता है। यह डिफ़ॉल्ट रूप से सक्षम है। सर्वोत्तम परिणामों के लिए फुल स्क्रीन विंडो विकल्प सक्षम होना चाहिए।" },

        { setting.GetOptionDescLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "Lumina वॉल्यूम सक्षम करें" },
{ setting.GetOptionLabelLocaleID(nameof(Lumina.Setting.EnableLuminaVolume)),
  "Lumina को सही ढंग से काम करने के लिए आवश्यक। यह Unity HDRP वॉल्यूम को सक्षम करता है जो कस्टम लाइटिंग और पोस्ट-प्रोसेसिंग ओवरराइड्स लागू करता है। इससे Lumina के उन्नत विज़ुअल इफेक्ट्स, जैसे रंग सुधार और टोन मैपिंग, खेल में सही तरीके से काम करते हैं।" },


    };

    public void Unload()
    {
    }
}
