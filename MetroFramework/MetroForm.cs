using MetroFramework;
using MetroFramework.Forms;
using System;
using System.Drawing;
using System.Windows.Forms;

public class ToastNotification : MetroForm
{
    private MetroFramework.Controls.MetroLabel messageLabel;
    private MetroFramework.Controls.MetroButton okButton;

    public ToastNotification(string message, int maxWidth = 400)
    {
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Resizable = false;
        this.TopMost = true;
        this.StartPosition = FormStartPosition.Manual;
        this.Theme = MetroThemeStyle.Dark;
        this.Style = MetroColorStyle.Blue;

        Font metroFont = MetroFonts.DefaultBold(9f);

        using (var g = this.CreateGraphics())
        {
            var proposedSize = new Size(maxWidth - 40, 0);
            var textSize = TextRenderer.MeasureText(g, message, metroFont, proposedSize, TextFormatFlags.WordBreak);

            int formWidth = textSize.Width + 40;
            int formHeight = textSize.Height + 80;

            this.Width = formWidth;
            this.Height = formHeight;

            var screen = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(screen.Right - this.Width - 20, screen.Bottom - this.Height - 50);

            messageLabel = new MetroFramework.Controls.MetroLabel
            {
                Text = message,
                AutoSize = false,
                Width = formWidth - 40,
                Height = textSize.Height + 10,
                Location = new Point(20, 20),
                ForeColor = Color.White,
                Font = metroFont,
                TextAlign = ContentAlignment.MiddleLeft,
                Theme = MetroThemeStyle.Dark,
                Style = MetroColorStyle.Blue,
                BackColor = Color.FromArgb(40, 40, 40),
            };
            this.Controls.Add(messageLabel);

            okButton = new MetroFramework.Controls.MetroButton
            {
                Text = "OK",
                Width = 80,
                Height = 30,
                Location = new Point((this.Width - 80) / 2, messageLabel.Bottom + 10),
                Theme = MetroThemeStyle.Dark,
                Style = MetroColorStyle.Blue,
            };
            okButton.Click += (s, e) => this.Close();
            this.Controls.Add(okButton);
        }
    }

    public static void ShowToast(string message)
    {
        var toast = new ToastNotification(message);
        toast.Show();
    }
}
