using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Save_Window_Position_and_Size.Classes
{
    /// <summary>
    /// Contains application-wide constants
    /// </summary>
    internal static class Constants
    {
        // AppSettings keys
        public static class AppSettingsConstants
        {
            public const string SavedWindowsKey = "SavedWindows";
            public const string SelectedProfileKey = "SelectedProfile";
            public const string IgnoreListKey = "IgnoreList";
            public const string RefreshTimeKey = "RefreshTime";
            public const string HighlighterEnabledKey = "HighlighterEnabled";
            public const string HighlighterColorKey = "HighlighterColor";
            public const string SkipConfirmationKey = "SkipConfirmation";
            public const string MinimizeOtherWindowsKey = "MinimizeOtherWindows";
        }

        // Default values
        public static class Defaults
        {
            public const int MaxProfiles = 5;
            public const int DefaultRefreshTime = 1;
            public const int DefaultRandomIdMin = 300;
            public const int DefaultRandomIdMax = 32034;
            public const int TimerIntervalMs = 1000;

            // Window Highlighter defaults
            public const int HighlighterBorderThickness = 3;
            public const int HighlighterBlinkIntervalMs = 500;
            public const int HighlighterPositionUpdateIntervalMs = 100;
            public const int HighlighterRedColor = 255;
            public const int HighlighterGreenColor = 0;
            public const int HighlighterBlueColor = 0;
            public const int InnerHighlighterBorderThickness = 1;
            public const int HighlighterDurationMs = 3000; // 3 seconds

            public const string ProfilesFolderName = "Profiles";
            public const string DefaultProfileName = "Default";
            public const string LastUsedProfileKey = "LastUsedProfile";
            public const string SettingsFileName = "settings.json";

            public const int RefreshTimerInterval = 1000;  // 1 second
            public const int AutoSaveInterval = 30000;     // 30 seconds
        }

        // Process names
        public static class ProcessNames
        {
            public const string FileExplorer = "File Explorer";
        }

        // UI strings
        public static class UI
        {
            public const string FileExplorerPrefix = "FileExplorer: ";
            public const string UnnamedWindowFormat = "Unnamed Window ({0})";
            public const string DefaultProfileNameFormat = "Profile {0}";
        }
    }
}