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
        internal static class AppSettingsConstants
        {
            internal const string SavedWindowsKey = "SavedWindows";
            internal const string SelectedProfileKey = "SelectedProfile";
            internal const string IgnoreListKey = "IgnoreList";
            internal const string RefreshTimeKey = "RefreshTime";
            internal const string HighlighterEnabledKey = "HighlighterEnabled";
            internal const string HighlighterColorKey = "HighlighterColor";
            internal const string SkipConfirmationKey = "SkipConfirmation";
            internal const string MinimizeOtherWindowsKey = "MinimizeOtherWindows";
            internal const string QuickLayoutsKey = "QuickLayouts";
            internal const string AutoStartTimerKey = "AutoStartTimer";
            internal const string StartAppsIfNotRunningKey = "StartAppsIfNotRunning";
        }

        // Default values
        internal static class Defaults
        {
            internal const int MaxProfiles = 5;
            internal const int DefaultRefreshTime = 1;
            internal const int DefaultRandomIdMin = 300;
            internal const int DefaultRandomIdMax = 32034;
            internal const int TimerIntervalMs = 1000;
            internal const int MaxQuickLayouts = 25;

            // Window Highlighter defaults
            internal const int HighlighterBorderThickness = 3;
            internal const int HighlighterBlinkIntervalMs = 500;
            internal const int HighlighterPositionUpdateIntervalMs = 100;
            internal const int HighlighterRedColor = 255;
            internal const int HighlighterGreenColor = 0;
            internal const int HighlighterBlueColor = 0;
            internal const int InnerHighlighterBorderThickness = 1;
            internal const int HighlighterDurationMs = 3000; // 3 seconds

            internal const string ProfilesFolderName = "Profiles";
            internal const string QuickLaunchFolderName = "QuickLaunch";
            internal const string DefaultProfileName = "Default";
            internal const string LastUsedProfileKey = "LastUsedProfile";
            internal const string SettingsFileName = "settings.json";
            internal const string ConfigManagerAppSettingsKey = "appSettings";

            internal const int RefreshTimerInterval = 1000;  // 1 second
            internal const int AutoSaveInterval = 30000;     // 30 seconds
        }

        // Process names
        internal static class ProcessNames
        {
            internal const string FileExplorer = "File Explorer";
        }

        // UI strings
        internal static class UI
        {
            internal const string FileExplorerPrefix = "FileExplorer: ";
            internal const string UnnamedWindowFormat = "Unnamed Window ({0})";
            internal const string DefaultProfileNameFormat = "Profile {0}";
            internal const string QuickLayoutTitleFormat = "{0} Quick Layout";
        }
    }
}