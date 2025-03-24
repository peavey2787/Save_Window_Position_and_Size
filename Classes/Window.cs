using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Save_Window_Position_and_Size.Classes
{
    internal class Window
    {
        public IntPtr hWnd { get; set; }
        public WindowPosAndSize WindowPosAndSize { get; set; }
        public bool KeepOnTop { get; set; }
        public bool AutoPosition { get; set; }
        public int Id { get; set; }
        public string ProcessName { get; set; }
        public string TitleName { get; set; }
        public string DisplayName { get; set; }
        public bool IsFileExplorer { get; set; }
        public bool UsePercentages { get; set; }

        internal Window()
        {
            WindowPosAndSize = new WindowPosAndSize();
            ProcessName = string.Empty;
            TitleName = string.Empty;
            DisplayName = string.Empty;
        }

        public Window(IntPtr hWnd, string windowTitle, string displayName)
        {
            this.hWnd = hWnd;
            this.TitleName = windowTitle;
            this.DisplayName = displayName;
        }

        /// <summary>
        /// Creates a deep copy of this window
        /// </summary>
        public Window Clone()
        {
            return new Window
            {
                AutoPosition = this.AutoPosition,
                DisplayName = this.DisplayName,
                hWnd = this.hWnd,
                Id = this.Id,
                IsFileExplorer = this.IsFileExplorer,
                KeepOnTop = this.KeepOnTop,
                ProcessName = this.ProcessName,
                TitleName = this.TitleName,
                UsePercentages = this.UsePercentages,
                WindowPosAndSize = new WindowPosAndSize
                {
                    X = this.WindowPosAndSize.X,
                    Y = this.WindowPosAndSize.Y,
                    Width = this.WindowPosAndSize.Width,
                    Height = this.WindowPosAndSize.Height,
                    IsPercentageBased = this.WindowPosAndSize.IsPercentageBased
                }
            };
        }

        /// <summary>
        /// Checks if the window is valid
        /// </summary>
        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(DisplayName) && string.IsNullOrWhiteSpace(TitleName) && string.IsNullOrWhiteSpace(ProcessName))
                return false;

            // For regular windows, check if we have a valid handle
            if (!IsFileExplorer && hWnd == IntPtr.Zero)
                return false;

            // For file explorers, check if we have a title
            if (IsFileExplorer && string.IsNullOrWhiteSpace(TitleName))
                return false;

            return true;
        }

        /// <summary>
        /// Creates a window with a random ID
        /// </summary>
        public static Window CreateWithRandomId(Random random)
        {
            var window = new Window
            {
                Id = random.Next(300, 32034)
            };
            return window;
        }

        /// <summary>
        /// Updates this window's position and size based on another window
        /// </summary>
        public void UpdatePositionAndSize(Window source)
        {
            if (source == null || source.WindowPosAndSize == null)
                return;

            WindowPosAndSize.X = source.WindowPosAndSize.X;
            WindowPosAndSize.Y = source.WindowPosAndSize.Y;
            WindowPosAndSize.Width = source.WindowPosAndSize.Width;
            WindowPosAndSize.Height = source.WindowPosAndSize.Height;
        }

        /// <summary>
        /// Ensures the window has a valid display name
        /// </summary>
        public void EnsureValidDisplayName()
        {
            if (string.IsNullOrWhiteSpace(DisplayName))
            {
                if (!string.IsNullOrWhiteSpace(TitleName))
                    DisplayName = TitleName;
                else if (!string.IsNullOrWhiteSpace(ProcessName))
                    DisplayName = ProcessName;
                else
                    DisplayName = $"Window {Id}";
            }
        }

        public override string ToString()
        {
            try
            {
                // Always ensure we have a valid display name
                EnsureValidDisplayName();

                if (IsFileExplorer)
                    return $"{DisplayName} (Explorer)";

                return DisplayName;
            }
            catch
            {
                // Fallback to a safe representation if anything fails
                return $"Window #{Id}";
            }
        }

        /// <summary>
        /// Validates window position and size, correcting any invalid values
        /// </summary>
        public void ValidatePositionAndSize()
        {
            if (WindowPosAndSize == null)
                WindowPosAndSize = new WindowPosAndSize();

            // Ensure width and height are positive
            if (WindowPosAndSize.Width <= 0)
                WindowPosAndSize.Width = 100;

            if (WindowPosAndSize.Height <= 0)
                WindowPosAndSize.Height = 100;

            // Ensure position is within reasonable bounds
            if (WindowPosAndSize.X < -10000 || WindowPosAndSize.X > 10000)
                WindowPosAndSize.X = 0;

            if (WindowPosAndSize.Y < -10000 || WindowPosAndSize.Y > 10000)
                WindowPosAndSize.Y = 0;
        }

        public WindowPosAndSize GetWindowPosAndSize()
        {
            var windowPosAndSize = new WindowPosAndSize();
            if (IsFileExplorer)
            {
                windowPosAndSize = InteractWithWindow.GetFileExplorerPosAndSize(TitleName);
            }
            else
            {
                windowPosAndSize = InteractWithWindow.GetWindowPositionAndSize(hWnd);
            }
            WindowPosAndSize = windowPosAndSize ?? new WindowPosAndSize();
            return WindowPosAndSize;
        }
    }
}
