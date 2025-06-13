using MetroFramework;
using MetroFramework.Components;
using MetroFramework.Controls;
using MetroFramework.Forms;
using System;
using System.Drawing;
using System.Windows.Forms;

public class MainSupportForm : MetroForm
{
    public static MainSupportForm Instance { get; private set; }

    public MainSupportForm()
    {
        Instance = this;

        // Style setup
        var styleManager = new MetroStyleManager();
        styleManager.Owner = this;
        styleManager.Theme = MetroThemeStyle.Dark;
        styleManager.Style = MetroColorStyle.Blue;
        this.StyleManager = styleManager;
        this.Theme = MetroThemeStyle.Dark;
        this.Style = MetroColorStyle.Blue;
    }

    public void ShowToastNotification(string message)
    {
        if (this.InvokeRequired)
        {
            this.Invoke(new Action(() => ShowToastNotification(message)));
            return;
        }

        var toast = new MetroLabel
        {
            Text = message,
            Theme = MetroThemeStyle.Dark,
            Style = MetroColorStyle.Blue,
            AutoSize = true,
            FontWeight = MetroLabelWeight.Bold,
            BackColor = Color.FromArgb(40, 40, 40),
            ForeColor = Color.White,
            Padding = new Padding(10),
        };

        // Position after setting AutoSize (width/height known)
        toast.Location = new Point((this.ClientSize.Width - toast.Width) / 2, this.ClientSize.Height - toast.Height - 20);
        toast.Anchor = AnchorStyles.Bottom;

        this.Controls.Add(toast);
        toast.BringToFront();
        toast.Invalidate();
        this.Invalidate();
    }
}