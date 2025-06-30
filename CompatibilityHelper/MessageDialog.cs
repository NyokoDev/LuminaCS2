using System.Collections.Generic;
using Game.UI;
using Game.UI.Localization;

public class SimpleMessageDialog : MessageDialog
{
    public SimpleMessageDialog(string message)
        : base(
            title: MakeSimpleLocalizedString("LUMINA"),
            message: MakeSimpleLocalizedString(message),
            confirmAction: MakeSimpleLocalizedString("OK")
        )
    {
    }

    private static LocalizedString MakeSimpleLocalizedString(string text)
    {
        return new LocalizedString(text, text, null);
    }
}
