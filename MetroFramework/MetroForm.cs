using Lumina;
using Lumina.XML;
using MetroFramework;
using MetroFramework.Controls;
using MetroFramework.Forms;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class ToastNotification : MetroForm
{
    private MetroLabel messageLabel;
    private MetroButton okButton;

    private const int ToastWidth = 480;
    private const int PaddingHorizontal = 24;
    private const int PaddingVertical = 24;
    private const int ButtonHeight = 45;
    private const int ButtonWidth = 120;
    private const int SpaceBetween = 20;
    private const int CornerRadius = 12;

    // For fullscreen overlay without stealing focus
    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    private const uint SWP_NOACTIVATE = 0x0010;
    private const uint SWP_SHOWWINDOW = 0x0040;

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
        int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
    private static extern IntPtr CreateRoundRectRgn(
        int nLeftRect, int nTopRect, int nRightRect, int nBottomRect,
        int nWidthEllipse, int nHeightEllipse);

    public ToastNotification(string message)
    {

        string Path = GlobalPaths.GetIconPath();
       

        if (File.Exists(Path))
        {
            try
            {
                this.Icon = new Icon(Path);
                Lumina.Mod.Log.Info("Toast icon loaded successfully.");
            }
            catch (Exception ex)
            {
                Lumina.Mod.Log.Warn("Failed to load toast icon: " + ex.Message);
            }
        }
        else
        {
            Lumina.Mod.Log.Warn("Toast icon file not found: " + Path);
        }


        // Base style and setup
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        TopMost = true;
        StartPosition = FormStartPosition.Manual;
        BackColor = Color.FromArgb(40, 40, 40);
        Theme = MetroThemeStyle.Dark;
        Style = MetroColorStyle.Blue;




        Font font = MetroFonts.DefaultBold(12f);

        // Create label for dynamic height calculation
        Label dummy = new Label
        {
            AutoSize = false,
            Font = font,
            Text = message,
            MaximumSize = new Size(ToastWidth - 2 * PaddingHorizontal, 0),
        };
        dummy.Size = dummy.PreferredSize;

        int labelHeight = dummy.Height;

        int formHeight = labelHeight + 2 * PaddingVertical + ButtonHeight + SpaceBetween;
        this.Width = ToastWidth;
        this.Height = formHeight;

        // Rounded corners
        this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, CornerRadius, CornerRadius));

        // Get the *primary* screen
        Rectangle primaryScreenBounds = Screen.PrimaryScreen.WorkingArea;
        this.Location = new Point(
            primaryScreenBounds.Left + (primaryScreenBounds.Width - Width) / 2,
            primaryScreenBounds.Top + (primaryScreenBounds.Height - Height) / 2);

        // Message Label
        messageLabel = new MetroLabel
        {
            Text = message,
            AutoSize = false,
            Width = ToastWidth - 2 * PaddingHorizontal,
            Height = labelHeight,
            Location = new Point(PaddingHorizontal, PaddingVertical),
            Font = font,
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleLeft,
            Theme = MetroThemeStyle.Dark,
            Style = MetroColorStyle.Blue,
            BackColor = Color.Transparent,
            UseMnemonic = false
        };
        Controls.Add(messageLabel);

        // OK button
        okButton = new MetroButton
        {
            Text = "OK",
            Width = ButtonWidth,
            Height = ButtonHeight,
            Location = new Point((Width - ButtonWidth) / 2, messageLabel.Bottom + SpaceBetween),
            Theme = MetroThemeStyle.Dark,
            Style = MetroColorStyle.Blue
        };
        okButton.Click += (s, e) => Close();
        Controls.Add(okButton);

        // Ensure no activation and topmost
        this.Load += (s, e) =>
        {
            SetWindowPos(this.Handle, HWND_TOPMOST,
                this.Left, this.Top, this.Width, this.Height,
                SWP_NOACTIVATE | SWP_SHOWWINDOW);
        };
    }

    // Keep fullscreen app active
    protected override CreateParams CreateParams
    {
        get
        {
            const int WS_EX_NOACTIVATE = 0x08000000;
            const int WS_EX_TOPMOST = 0x00000008;
            const int WS_EX_TOOLWINDOW = 0x00000080;

            var cp = base.CreateParams;
            cp.ExStyle |= WS_EX_NOACTIVATE | WS_EX_TOPMOST | WS_EX_TOOLWINDOW;
            return cp;
        }
    }

    // Static method to show the toast
    public static void ShowToast(string message)
    {
        var toast = new ToastNotification(message);
        toast.Show();
    }
}
