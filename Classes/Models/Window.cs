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
            this.Id = GenerateRandomId();
        }

        public Window(IntPtr hWnd, string windowTitle, string displayName)
        {
            WindowPosAndSize = new WindowPosAndSize();
            this.hWnd = hWnd;
            this.TitleName = windowTitle;
            this.DisplayName = displayName;
            this.ProcessName = string.Empty;
            this.Id = GenerateRandomId();
            EnsureValidDisplayName();
        }

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

        private static int GenerateRandomId()
        {
            Random _random = new Random();
            return _random.Next(Constants.Defaults.DefaultRandomIdMin, Constants.Defaults.DefaultRandomIdMax);
        }

        public void EnsureValidDisplayName()
        {
            if (string.IsNullOrWhiteSpace(DisplayName))
            {
                if (!string.IsNullOrWhiteSpace(TitleName))
                    DisplayName = TitleName;
                else if (!string.IsNullOrWhiteSpace(ProcessName))
                    DisplayName = ProcessName;
                else
                    DisplayName = string.Format(Constants.UI.UnnamedWindowFormat, Id);
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
    }
}
