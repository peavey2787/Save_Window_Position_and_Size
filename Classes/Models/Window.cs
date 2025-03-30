using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Save_Window_Position_and_Size.Classes
{
    [Serializable]
    internal class Window
    {
        [JsonProperty]
        internal IntPtr hWnd { get; set; }

        [JsonProperty]
        internal WindowPosAndSize WindowPosAndSize { get; set; }

        [JsonProperty]
        internal bool KeepOnTop { get; set; }

        [JsonProperty]
        internal bool AutoPosition { get; set; }

        [JsonProperty]
        internal int Id { get; set; }

        [JsonProperty]
        internal string ProcessName { get; set; }

        [JsonProperty]
        internal string TitleName { get; set; }

        [JsonProperty]
        internal string DisplayName { get; set; }

        [JsonProperty]
        internal bool IsFileExplorer { get; set; }

        [JsonProperty]
        internal string FilePath { get; set; }

        [JsonProperty]
        internal bool UsePercentages { get; set; }

        internal Window()
        {
            WindowPosAndSize = new WindowPosAndSize();
            ProcessName = string.Empty;
            TitleName = string.Empty;
            DisplayName = string.Empty;
            FilePath = string.Empty;
            this.Id = GenerateRandomId();
        }

        internal Window(IntPtr hWnd, string windowTitle, string displayName)
        {
            WindowPosAndSize = new WindowPosAndSize();
            this.hWnd = hWnd;
            this.TitleName = windowTitle;
            this.DisplayName = displayName;
            this.ProcessName = string.Empty;
            this.FilePath = string.Empty;
            this.Id = GenerateRandomId();
            EnsureValidDisplayName();
        }

        internal Window Clone()
        {
            return new Window
            {
                AutoPosition = this.AutoPosition,
                DisplayName = this.DisplayName,
                hWnd = this.hWnd,
                Id = this.Id,
                IsFileExplorer = this.IsFileExplorer,
                FilePath = this.FilePath,
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

        internal bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(DisplayName) && string.IsNullOrWhiteSpace(TitleName) && string.IsNullOrWhiteSpace(ProcessName))
                return false;

            return true;
        }
        internal bool IsSameWindowWithoutHandle(Window window)
        {
            if (window.Id == this.Id) return true;

            // Check if adding a * to end of window title will cause a match
            if ((string.Equals(window.TitleName, this.TitleName, StringComparison.Ordinal) 
                || string.Equals(window.TitleName + "*", this.TitleName, StringComparison.Ordinal))
                && string.Equals(window.ProcessName, this.ProcessName, StringComparison.Ordinal))
            {
                return true;
            }
            
            return false;
        }
        internal bool IsSameWindowWithHandle(Window window)
        {
            if (window.Id == this.Id) return true;

            if (window.TitleName == this.TitleName && window.ProcessName == this.ProcessName && window.hWnd == this.hWnd)
                return true;

            return false;
        }
        private static int GenerateRandomId()
        {
            Random _random = new Random();
            return _random.Next(Constants.Defaults.DefaultRandomIdMin, Constants.Defaults.DefaultRandomIdMax);
        }

        internal void EnsureValidDisplayName()
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
