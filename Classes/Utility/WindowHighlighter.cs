using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace Save_Window_Position_and_Size.Classes
{
    /// <summary>
    /// Handles drawing a blinking highlight around a window
    /// </summary>
    internal class WindowHighlighter
    {
        // P/Invoke declarations
        [DllImport("user32.dll")]
        private static extern int GetWindowRect(IntPtr hwnd, out RECT rect);

        [DllImport("user32.dll")]
        private static extern int GetClientRect(IntPtr hwnd, out RECT rect);

        [DllImport("user32.dll")]
        private static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        internal static extern IntPtr WindowFromPoint(POINT Point);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr hWnd, System.Text.StringBuilder lpClassName, int nMaxCount);

        [DllImport("dwmapi.dll")]
        private static extern int DwmGetWindowAttribute(IntPtr hwnd, int dwAttribute, out RECT pvAttribute, int cbAttribute);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        // Constants
        private const int DWMWA_EXTENDED_FRAME_BOUNDS = 9;

        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            internal int Left;
            internal int Top;
            internal int Right;
            internal int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct POINT
        {
            internal int X;
            internal int Y;
        }

        private HighlighterForm _highlighterForm;
        private readonly Timer _blinkTimer = new Timer();
        private readonly Timer _autoStopTimer = new Timer(); // Auto stop timer
        private bool _outerBorderVisible = true;
        private IntPtr _highlightedWindow = IntPtr.Zero;
        private bool _isBlinking = false;
        private int _borderWidth = 3;
        private int _innerBorderWidth = 1;
        private Color _borderColor = Color.Red;
        private Color _innerBorderColor = Color.Red;
        private int _highlightDurationMs = 3000; // Default 3 seconds
        private Rectangle _windowRect;

        /// <summary>
        /// Gets or sets the border width in pixels
        /// </summary>
        internal int BorderWidth
        {
            get { return _borderWidth; }
            set { _borderWidth = value; }
        }

        /// <summary>
        /// Gets or sets the inner border thickness
        /// </summary>
        internal int InnerBorderThickness
        {
            get { return _innerBorderWidth; }
            set { _innerBorderWidth = value; }
        }

        /// <summary>
        /// Gets or sets the border color
        /// </summary>
        internal Color BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; }
        }

        /// <summary>
        /// Gets or sets the inner border color
        /// </summary>
        internal Color InnerBorderColor
        {
            get { return _innerBorderColor; }
            set { _innerBorderColor = value; }
        }

        /// <summary>
        /// Gets or sets the highlight duration in milliseconds
        /// </summary>
        internal int HighlightDurationMs
        {
            get { return _highlightDurationMs; }
            set { _highlightDurationMs = value; }
        }

        internal WindowHighlighter()
        {
            // Configure blink timer
            _blinkTimer.Interval = 500;
            _blinkTimer.Tick += BlinkTimer_Tick;

            // Configure auto stop timer
            _autoStopTimer.Tick += AutoStopTimer_Tick;
            _autoStopTimer.Interval = _highlightDurationMs;
        }

        /// <summary>
        /// Highlights a window
        /// </summary>
        internal void HighlightWindow(IntPtr hWnd)
        {
            try
            {
                if (hWnd == IntPtr.Zero)
                {
                    return;
                }

                // Stop any existing highlighting
                StopHighlighting();

                _highlightedWindow = hWnd;
                _windowRect = GetPreciseWindowRect(hWnd);

                // Validate window size
                if (_windowRect.Width <= 10 || _windowRect.Height <= 10 ||
                    _windowRect.Width > SystemInformation.VirtualScreen.Width ||
                    _windowRect.Height > SystemInformation.VirtualScreen.Height)
                {
                    return;
                }

                // Create the highlighter form
                _highlighterForm = new HighlighterForm(this);
                _highlighterForm.Show();

                // Start blinking
                StartBlinking();

                // Start auto-stop timer
                _autoStopTimer.Interval = Math.Max(500, _highlightDurationMs); // Ensure minimum duration
                _autoStopTimer.Start();
            }
            catch (Exception)
            {
                StopHighlighting(); // Clean up in case of partial initialization
            }
        }

        /// <summary>
        /// Gets more accurate window dimensions using various techniques
        /// </summary>
        private Rectangle GetPreciseWindowRect(IntPtr hWnd)
        {
            try
            {
                // Get the class name of the window
                var classNameBuilder = new System.Text.StringBuilder(256);
                GetClassName(hWnd, classNameBuilder, classNameBuilder.Capacity);
                string className = classNameBuilder.ToString();

                // Get process info for special handling
                uint processId;
                string processName = "";
                GetWindowThreadProcessId(hWnd, out processId);

                try
                {
                    using (Process process = Process.GetProcessById((int)processId))
                    {
                        processName = process.ProcessName.ToLower();
                    }
                }
                catch
                {
                    // Process may have exited
                }

                // Default rectangle using standard method
                Rectangle standardRect;
                RECT rect;
                GetWindowRect(hWnd, out rect);
                standardRect = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);

                // Special handling for console windows which need exact borders
                if (className == "ConsoleWindowClass")
                {
                    // Console windows work best with standard GetWindowRect
                    return standardRect;
                }

                // Try to use DWM for more accurate borders (Windows Vista and later)
                if (Environment.OSVersion.Version.Major >= 6) // Vista or later
                {
                    try
                    {
                        RECT dwmRect;
                        int result = DwmGetWindowAttribute(hWnd, DWMWA_EXTENDED_FRAME_BOUNDS, out dwmRect, Marshal.SizeOf(typeof(RECT)));

                        if (result == 0) // Success
                        {
                            var dwmRectangle = new Rectangle(
                                dwmRect.Left,
                                dwmRect.Top,
                                dwmRect.Right - dwmRect.Left,
                                dwmRect.Bottom - dwmRect.Top
                            );

                            // Validate that the DWM rectangle is reasonable
                            if (dwmRectangle.Width > 0 && dwmRectangle.Height > 0 &&
                                dwmRectangle.Width <= Screen.PrimaryScreen.WorkingArea.Width &&
                                dwmRectangle.Height <= Screen.PrimaryScreen.WorkingArea.Height)
                            {
                                return dwmRectangle;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // Silently handle exception
                    }
                }

                // If we're working with certain applications that might have weird chrome/shadows
                // Try a different approach using client rectangle with adjustments
                if (processName.Contains("chrome") || processName.Contains("edge") || processName.Contains("firefox"))
                {
                    try
                    {
                        RECT clientRect;
                        GetClientRect(hWnd, out clientRect);

                        // Convert client coordinates to screen coordinates
                        POINT clientPoint = new POINT { X = 0, Y = 0 };
                        ClientToScreen(hWnd, ref clientPoint);

                        // Calculate the border difference
                        int borderWidth = (standardRect.Width - (clientRect.Right - clientRect.Left)) / 2;
                        int borderHeight = standardRect.Height - (clientRect.Bottom - clientRect.Top) - borderWidth;

                        // Create a rectangle based on the client area with adjustments for borders
                        return new Rectangle(
                            clientPoint.X - borderWidth,
                            clientPoint.Y - borderWidth,
                            (clientRect.Right - clientRect.Left) + (borderWidth * 2),
                            (clientRect.Bottom - clientRect.Top) + borderWidth + borderHeight
                        );
                    }
                    catch (Exception)
                    {
                        // Silently handle exception
                    }
                }

                // If all else fails, use the standard window rectangle
                return standardRect;
            }
            catch (Exception)
            {
                // Last resort: standard GetWindowRect
                RECT rect;
                GetWindowRect(hWnd, out rect);
                return new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
            }
        }

        /// <summary>
        /// Start the blinking effect
        /// </summary>
        private void StartBlinking()
        {
            if (!_isBlinking)
            {
                _isBlinking = true;
                _outerBorderVisible = true;
                _highlighterForm?.Invalidate();
                _blinkTimer.Start();
            }
        }

        /// <summary>
        /// Stop the blinking effect
        /// </summary>
        private void StopBlinking()
        {
            if (_isBlinking)
            {
                _isBlinking = false;
                _blinkTimer.Stop();
            }
        }

        /// <summary>
        /// Stop the highlighter
        /// </summary>
        internal void StopHighlighting()
        {
            try
            {
                StopBlinking();
                _autoStopTimer.Stop();

                if (_highlighterForm != null && !_highlighterForm.IsDisposed)
                {
                    _highlighterForm.Close();
                    _highlighterForm.Dispose();
                    _highlighterForm = null;
                }

                _highlightedWindow = IntPtr.Zero;
            }
            catch (Exception)
            {
                // Reset state even if cleanup fails
                _highlighterForm = null;
                _highlightedWindow = IntPtr.Zero;
            }
        }

        internal void Dispose()
        {
            StopHighlighting();
            _blinkTimer.Dispose();
            _autoStopTimer.Dispose();
        }

        /// <summary>
        /// Get the handle of the window under the cursor
        /// </summary>
        internal static IntPtr GetWindowUnderCursor()
        {
            POINT cursorPos;
            GetCursorPos(out cursorPos);
            return WindowFromPoint(cursorPos);
        }

        /// <summary>
        /// Timer tick handler to create blinking effect
        /// </summary>
        private void BlinkTimer_Tick(object sender, EventArgs e)
        {
            _outerBorderVisible = !_outerBorderVisible;
            _highlighterForm?.Invalidate();
        }

        /// <summary>
        /// Auto-stop timer handler to automatically stop highlighting after a duration
        /// </summary>
        private void AutoStopTimer_Tick(object sender, EventArgs e)
        {
            // Auto-stop after the timeout
            StopHighlighting();
        }

        /// <summary>
        /// Transparent form used to draw borders around a window
        /// </summary>
        private class HighlighterForm : Form
        {
            private readonly WindowHighlighter _owner;

            internal HighlighterForm(WindowHighlighter owner)
            {
                _owner = owner ?? throw new ArgumentNullException(nameof(owner));

                try
                {
                    // Configure form
                    this.FormBorderStyle = FormBorderStyle.None;
                    this.ShowInTaskbar = false;
                    this.StartPosition = FormStartPosition.Manual;
                    this.TopMost = true;
                    this.BackColor = Color.Magenta; // Will be made transparent
                    this.TransparencyKey = Color.Magenta;

                    // Set location and size to cover the target window plus space for outer border
                    int padding = Math.Max(_owner._borderWidth, _owner._innerBorderWidth) * 2;

                    // Ensure valid form size and position
                    int left = _owner._windowRect.Left - padding;
                    int top = _owner._windowRect.Top - padding;
                    int width = Math.Min(SystemInformation.VirtualScreen.Width, _owner._windowRect.Width + (padding * 2));
                    int height = Math.Min(SystemInformation.VirtualScreen.Height, _owner._windowRect.Height + (padding * 2));

                    this.Location = new Point(left, top);
                    this.Size = new Size(width, height);

                    // Handle paint event to draw borders
                    this.Paint += HighlighterForm_Paint;
                }
                catch (Exception)
                {
                    throw; // Re-throw to allow parent to handle cleanup
                }
            }

            private void HighlighterForm_Paint(object sender, PaintEventArgs e)
            {
                try
                {
                    // Get graphics object with high quality settings
                    Graphics g = e.Graphics;
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    // Calculate border rectangles relative to form
                    int outerPadding = _owner._borderWidth;
                    int innerPadding = _owner._innerBorderWidth;

                    // Coordinates of the target window relative to this form
                    int windowX = outerPadding;
                    int windowY = outerPadding;
                    int windowWidth = Math.Max(1, this.Width - (outerPadding * 2));
                    int windowHeight = Math.Max(1, this.Height - (outerPadding * 2));

                    // Draw outer border if visible
                    if (_owner._outerBorderVisible && _owner._borderWidth > 0)
                    {
                        using (Pen outerPen = new Pen(_owner._borderColor, _owner._borderWidth))
                        {
                            // Draw outer border
                            Rectangle outerRect = new Rectangle(
                                outerPadding / 2,
                                outerPadding / 2,
                                Math.Max(1, this.Width - outerPadding),
                                Math.Max(1, this.Height - outerPadding));
                            g.DrawRectangle(outerPen, outerRect);
                        }
                    }

                    // Draw inner border (always visible)
                    if (_owner._innerBorderWidth > 0)
                    {
                        using (Pen innerPen = new Pen(_owner._innerBorderColor, _owner._innerBorderWidth))
                        {
                            // Draw inner border
                            Rectangle innerRect = new Rectangle(
                                windowX,
                                windowY,
                                windowWidth,
                                windowHeight);
                            g.DrawRectangle(innerPen, innerRect);
                        }
                    }
                }
                catch (Exception)
                {
                    // Continue execution - don't crash on paint errors
                }
            }
        }
    }
}