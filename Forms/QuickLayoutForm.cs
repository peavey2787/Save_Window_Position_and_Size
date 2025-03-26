using Save_Window_Position_and_Size.Classes;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;

namespace Save_Window_Position_and_Size.Forms
{
    public partial class QuickLayoutForm : Form
    {
        private WindowManager windowManager;
        private int layoutIndex;
        private bool initializing = true;

        internal QuickLayoutForm(WindowManager windowManager, int index, Icon formIcon)
        {
            InitializeComponent();

            this.windowManager = windowManager;
            this.layoutIndex = index;
            this.Icon = formIcon;
            this.Text = string.Format(Constants.UI.QuickLayoutTitleFormat, index);

            // Configure form properties to stay minimized but visible in taskbar
            this.ShowInTaskbar = true;
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.StartPosition = FormStartPosition.Manual;
            this.MinimizeBox = true;
            this.MaximizeBox = false;
            this.ControlBox = true; // Allow close box to appear in taskbar
            this.Opacity = 0; // Make form transparent

            // Position on-screen but outside visible area
            int x = Screen.PrimaryScreen.WorkingArea.Right - 10;
            int y = Screen.PrimaryScreen.WorkingArea.Bottom - 10;
            this.Location = new Point(x, y);
            this.Size = new Size(1, 1); // Minimal size

            // Handle form events
            this.Load += QuickLayoutForm_Load;
            this.Activated += QuickLayoutForm_Activated;
            this.FormClosing += QuickLayoutForm_FormClosing;
            this.Shown += QuickLayoutForm_Shown;
            this.SizeChanged += QuickLayoutForm_SizeChanged;
        }

        private void QuickLayoutForm_Shown(object sender, EventArgs e)
        {
            // First show the form normally to ensure it registers with the taskbar
            this.Opacity = 0;
            this.Show();
            this.WindowState = FormWindowState.Normal;

            // Then minimize it
            this.WindowState = FormWindowState.Minimized;
        }

        private void QuickLayoutForm_SizeChanged(object sender, EventArgs e)
        {
            // If form is somehow restored, make sure it stays minimized
            if (this.WindowState != FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void QuickLayoutForm_Load(object sender, EventArgs e)
        {
            // Capture current layout when the form is first created
            if (initializing)
            {
                CaptureLayout();
                initializing = false;
            }
        }

        private void QuickLayoutForm_Activated(object sender, EventArgs e)
        {
            // Make sure the form stays minimized
            if (this.WindowState != FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void QuickLayoutForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Simply hide the form if user tries to close it, unless application is shutting down
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.WindowState = FormWindowState.Minimized;
            }
        }

        // Override Show method to ensure form is shown and then minimized
        public new void Show()
        {
            base.Show();
            // First show normally, then minimize
            this.WindowState = FormWindowState.Normal;
            Application.DoEvents(); // Process UI events
            this.WindowState = FormWindowState.Minimized;
        }

        // Override ShowDialog to prevent showing as dialog
        public new DialogResult ShowDialog()
        {
            Show();
            return DialogResult.None;
        }

        private void CaptureLayout()
        {
            try
            {
                // Make sure the window stays minimized
                this.WindowState = FormWindowState.Minimized;

                // Get all currently visible windows using the window manager
                var windows = windowManager.GetVisibleRunningApps();

                // Filter out our own window if it's somehow included
                windows = windows.Where(w => w.hWnd != this.Handle).ToList();

                // Set AutoPosition to true for all windows
                foreach (var window in windows)
                {
                    window.AutoPosition = true;
                }

                // Save the layout using the window manager
                windowManager.SaveQuickLayout(layoutIndex, windows);
            }
            catch
            {
                // Silently handle any exceptions
            }
        }

        private void ApplyLayout()
        {
            try
            {
                // First minimize all visible windows except our own
                var currentWindows = windowManager.GetVisibleRunningApps();
                foreach (var window in currentWindows)
                {
                    IntPtr hWnd = window.hWnd;
                    if (hWnd != IntPtr.Zero && hWnd != this.Handle)
                    {
                        InteractWithWindow.MinimizeWindow(hWnd);
                    }
                }

                // Add a small delay to allow windows to minimize
                System.Threading.Thread.Sleep(100);

                // Now restore the saved layout
                windowManager.RestoreQuickLayout(layoutIndex);

                // Make sure this form stays minimized
                this.WindowState = FormWindowState.Minimized;
            }
            catch
            {
                // Silently handle any exceptions
            }
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_RESTORE = 0xF120;

            if (m.Msg == WM_SYSCOMMAND && m.WParam.ToInt32() == SC_RESTORE)
            {
                // User explicitly clicked the taskbar icon to restore the window
                // Execute ApplyLayout before the form gets restored
                ApplyLayout();

                // We'll still let the base process the message
                // but we'll immediately minimize afterward
                base.WndProc(ref m);

                // Immediately minimize the form again
                this.WindowState = FormWindowState.Minimized;
                return;
            }

            // For all other messages, process normally
            base.WndProc(ref m);
        }
    }
}