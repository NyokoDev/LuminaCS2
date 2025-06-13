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

namespace Lumina.Metro
{
    public class ToastNotification : MetroForm
    {
        private MetroLabel messageLabel;
        private MetroButton okButton;

        private const int ToastWidth = 480;
        private const int PaddingHorizontal = 40;
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
            string iconPath = GlobalPaths.GetIconPath();

            int iconSize = 32;
            int iconPaddingTop = 10;
            int spacingAfterIcon = 10;
            int toastWidth = ToastWidth;
            int cornerRadius = CornerRadius;

            PictureBox pictureBox = null;

            if (File.Exists(iconPath))
            {
                try
                {
                    this.Icon = new Icon(iconPath);

                    pictureBox = new PictureBox
                    {
                        Image = Image.FromFile(iconPath),
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Size = new Size(iconSize, iconSize),
                        Location = new Point((toastWidth - iconSize) / 2, iconPaddingTop),
                    };
                    this.Controls.Add(pictureBox);


                }
                catch (Exception ex)
                {
                    // Pending logging or error handling
                }
            }

            // Base style and setup
            FormBorderStyle = FormBorderStyle.None;
            ControlBox = false;
            Text = string.Empty;
            ShowInTaskbar = false;
            TopMost = true;
            StartPosition = FormStartPosition.Manual;
            BackColor = Color.FromArgb(40, 40, 40);
            Theme = MetroThemeStyle.Dark;
            Style = MetroColorStyle.Silver;

            Font font = MetroFonts.DefaultBold(12f);

            int labelTop = (pictureBox != null)
                ? pictureBox.Bottom + spacingAfterIcon
                : PaddingVertical;

            // Measure height of label for word-wrapped text
            Label dummy = new Label
            {
                AutoSize = false,
                Font = font,
                Text = message,
                MaximumSize = new Size(toastWidth - 2 * PaddingHorizontal - 10, 0),

            };
            Size measuredSize = TextRenderer.MeasureText(message, font, dummy.MaximumSize, TextFormatFlags.WordBreak | TextFormatFlags.Top);
            dummy.Size = measuredSize;


            int labelHeight = dummy.Height;

            int formHeight = labelTop + labelHeight + SpaceBetween + ButtonHeight + PaddingVertical;
            this.Width = toastWidth;
            this.Height = formHeight;

            // Rounded corners
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, cornerRadius, cornerRadius));

            // Center on screen
            Rectangle primaryScreenBounds = Screen.PrimaryScreen.WorkingArea;
            this.Location = new Point(
                primaryScreenBounds.Left + (primaryScreenBounds.Width - Width) / 2,
                primaryScreenBounds.Top + (primaryScreenBounds.Height - Height) / 2);

            // Message label
            messageLabel = new MetroLabel
            {
                Text = message,
                AutoSize = false,
                Width = toastWidth - 2 * PaddingHorizontal,
                Height = labelHeight,
                Location = new Point(PaddingHorizontal, labelTop),
                Font = font,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleCenter,
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

            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Resizable = false; // MetroFramework specific

            // Keep it topmost without stealing focus
            this.Load += (s, e) =>
            {
                SetWindowPos(this.Handle, HWND_TOPMOST,
                    this.Left, this.Top, this.Width, this.Height,
                    SWP_NOACTIVATE | SWP_SHOWWINDOW);
            };
        }

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

        protected override bool ShowWithoutActivation => true;

        // Static method to show the toast
        public static void ShowToast(string message)
        {

            var toast = new ToastNotification(message);
            toast.Show();
        }
    }
}
