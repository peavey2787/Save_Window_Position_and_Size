# Save Window Position and Size - Complete Documentation

## 1. Application Overview

Save Window Position and Size is a Windows utility designed to save and restore window positions and sizes for any application. It helps users maintain consistent window layouts across application restarts and system reboots.

### Key Features

- **Profile Support**: Create and manage multiple window layout profiles
- **Auto-Positioning**: Automatically restore windows to saved positions at customizable intervals
- **Always-on-Top Option**: Keep important windows visible at all times
- **Visual Highlighting**: Temporarily highlight windows for easier identification
- **System Tray Integration**: Quick access to functionality from the system tray
- **Ignore List**: Exclude specific windows from management
- **Percentage-Based Sizing**: Option to use percentage values instead of pixels (experimental)

### Target Users

This application is ideal for:
- Power users who work with multiple applications simultaneously
- Users with multi-monitor setups who want to maintain specific window arrangements
- Developers who need consistent workspace layouts
- Anyone who frequently adjusts window positions manually

## 2. User Guide

### Installation

The application is portable and requires no formal installation process. Simply download and run the executable.

### Basic Usage

1. **Starting the Application**: Run the executable file `Save_Window_Position_and_Size.exe`
2. **Accessing the Interface**: The application starts in the system tray. Double-click the tray icon to open the main interface

### Saving Window Layouts

1. Position your application windows as desired
2. Click the **Refresh All Apps** button (↻) to update the running applications list
3. Select an application from the list
4. Review and adjust position/size settings if needed
5. Click the **Save** button to add the window to your active profile

### Quick Capture of All Windows

1. Right-click the system tray icon
2. Select **Capture Current Layout**
3. All visible windows will be saved to the current profile

### Restoring Windows

#### Manual Restoration
1. Select a saved window from the Saved Apps list
2. Click the **Restore** button
3. The window will be moved to its saved position and size

#### Automatic Restoration
1. Enable the **Auto Position** checkbox for windows you want automatically maintained
2. Set the desired time interval (in minutes) in the **Update Timer Interval** field
3. The application will periodically check and restore windows to their saved positions

### Managing Profiles

1. Select a profile from the dropdown in the top-right corner
2. The application maintains up to 5 distinct profiles
3. Switch profiles using the dropdown or the tray icon's **Switch Profile** menu

### Using the Ignore List

1. Click the **Ignore List** button (🚫)
2. Double-click "Add item..." to add new entries
3. Select an entry and press Delete to remove it
4. Windows matching these entries will not be affected by the application

## 3. System Architecture

### High-Level Architecture

The application follows a simple layered architecture:

1. **Presentation Layer**: Windows Forms UI components (Form1, IgnoreForm)
2. **Business Logic Layer**: Core functionality classes (WindowManager, IgnoreListManager, etc.)
3. **Data Access Layer**: Configuration and persistence (AppSettings)

### Key Components

- **Form1**: Main application interface
- **WindowManager**: Manages window profiles and operations
- **IgnoreListManager**: Handles the list of windows to be ignored
- **WindowHighlighter**: Provides visual feedback by highlighting windows
- **InteractWithWindow**: Contains native Windows API calls for window manipulation
- **AppSettings**: Handles application configuration persistence

### Data Flow

1. User interactions are captured through the UI
2. UI calls business logic components to perform operations
3. Business logic uses Windows API to interact with system windows
4. Configuration is persisted using AppSettings

## 4. Technical Documentation

### Class Structure

#### Core Classes

- **Window**: Represents a window with its properties
  - Properties: hWnd, WindowPosAndSize, DisplayName, ProcessName, etc.
  - Methods for cloning, validation, and string representation

- **WindowPosAndSize**: Contains position and size information
  - Properties: X, Y, Width, Height

- **Profile**: Contains a collection of windows for a specific layout
  - Properties: Name, Windows (List<Window>)
  - Methods for cloning and window management

- **ProfileCollection**: Manages multiple profiles
  - Properties: Profiles, SelectedProfile, SelectedProfileIndex

- **WindowManager**: Manages window saving, loading, and profile management
  - Methods for adding/removing windows, switching profiles, restoring windows

- **IgnoreListManager**: Manages the list of ignored windows
  - Methods for adding/removing from the list, checking if a window should be ignored

- **WindowHighlighter**: Provides visual highlighting of windows
  - Uses Windows API to draw borders around windows

- **AppSettings**: Static class for saving/loading application settings
  - Generic methods for serializing/deserializing complex objects

#### User Interface

- **Form1**: Main application form
  - Handles user interactions and coordinates with business logic
  - Contains UI elements for window management

- **IgnoreForm**: Manages the ignore list
  - Allows adding/removing entries from the ignore list

### Configuration

Settings are stored in the application configuration file and include:
- Saved window profiles
- Selected profile index
- Ignore list
- Refresh time interval
- Highlighter settings
- Skip confirmation flag

### Database Schema

The application does not use a traditional database but persists data through:
- .NET Configuration system
- JSON serialization for complex objects

### Code Flow Diagrams

#### Saving a Window
```
User selects window → Updates window properties → Clicks Save → 
WindowManager.AddOrUpdateWindow → Serializes profile → AppSettings.Save
```

#### Restoring a Window
```
User selects window → Clicks Restore → WindowManager retrieves window → 
InteractWithWindow.RestoreWindow → Gets window handle → 
Sets window position and size using Windows API
```

#### Auto-Positioning Flow
```
Timer tick → For each window with AutoPosition enabled → 
Find matching running window → Apply saved position and size → 
Update "always on top" state
```

## 5. External Dependencies

### Third-Party Libraries

- **Newtonsoft.Json**: Used for JSON serialization/deserialization (version 13.0.3)

### System Dependencies

- **.NET 6.0**: Application framework
- **Windows Forms**: UI framework
- **System.Configuration**: For application settings management
- **COM References**:
  - SHDocVw: For Internet Explorer/File Explorer window management
  - Shell32: For Windows shell integration

### Windows API Integration

The application uses P/Invoke to call native Windows API functions for:
- Retrieving window handles
- Getting/setting window positions and sizes
- Setting "always on top" state
- Getting window titles and process information
- Drawing window highlights

## 6. Application Settings

### cursorrules.json

This file defines rules for how the application should handle different windows:

```json
{
  "version": "1.0",
  "profiles": [
    {
      "name": "Default",
      "rules": [
        {
          "windowPattern": "*",
          "action": "save",
          "applyOnStartup": true
        }
      ]
    },
    // Additional profiles with specific rules
  ],
  "settings": {
    "refreshIntervalMinutes": 1,
    "skipConfirmation": false,
    "highlighterEnabled": true,
    "highlighterColor": "#FF0000",
    "highlighterDurationMs": 3000,
    "autoSaveIntervalSeconds": 30
  },
  "ignoreList": [
    "Task Manager",
    "Program Manager",
    "Settings"
  ]
}
```

### Rule Types

- **save**: Save the window's current position and size
- **position**: Move the window to a specific position and size
- **ignore**: Exclude the window from management

### Constants

The application uses several constants defined in `Constants.cs`:

- **AppSettingsConstants**: Keys for application settings
- **Defaults**: Default values for application settings
- **ProcessNames**: Special process name constants
- **UI**: UI-related constants

## 7. Extensibility

### Adding New Features

To extend the application with new features:

1. **New Window Properties**: Add properties to the Window class
2. **UI Controls**: Add corresponding controls to Form1
3. **Persistence**: Update serialization in WindowManager
4. **Windows API**: Add new P/Invoke methods in InteractWithWindow if needed

### Customizing Behavior

The application can be customized through:

1. **cursorrules.json**: Modify rules for window handling
2. **Constants.cs**: Adjust application constants
3. **Profiles**: Create custom profiles for different scenarios

## 8. Troubleshooting

### Common Issues

- **Window Not Found**: Occurs when a saved window can't be found among running applications
  - *Solution*: Make sure the application is running and has a visible window

- **Window Position Not Applying**: Some applications override window positioning
  - *Solution*: Try enabling "Keep on Top" option or increase refresh interval

- **File Explorer Windows**: Special handling is required for File Explorer windows
  - *Solution*: Application uses SHDocVw COM component to manage these windows

- **High CPU Usage**: Can occur if refresh interval is too short
  - *Solution*: Increase the refresh interval to reduce frequency of checks

### Logging

The application uses Debug.WriteLine for logging. To capture logs:
1. Run the application with a debugger attached
2. View output in the Debug window

## 9. Performance Considerations

### Resource Usage

- **Memory**: The application has a small memory footprint
- **CPU**: Minimal CPU usage except during window scanning/repositioning
- **Disk**: Minimal disk I/O for settings persistence

### Optimization

- **Window Scanning**: Only active windows are scanned
- **Ignore List**: Reduces unnecessary processing
- **Timer Interval**: Configurable to balance responsiveness vs. resource usage

## 10. Security Considerations

### Data Storage

- Application settings are stored in the application configuration file
- No sensitive information is collected or stored

### Windows API Usage

- Uses standard Windows API calls for window management
- No elevation or special permissions required
- No network communication

## 11. Future Development

### Planned Features

- Multi-monitor support with display identification
- More sophisticated window matching (process + title)
- Rule-based window management
- Keyboard shortcuts for common actions
- Import/export profiles
- Fully functioning percentage-based positioning

### Known Limitations

- Percentage-based positioning is experimental
- Some applications may override window positioning
- Limited to 5 profiles
- No support for minimized/maximized state
- Limited handling of special windows (like Windows Explorer)

---

## Appendix A: Class Reference

### Window Class
```csharp
class Window
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
    
    // Methods
    public Window Clone()
    public bool IsValid()
    public override string ToString()
    internal void UpdatePositionAndSize(Window saved)
}
```

### WindowManager Class
```csharp
class WindowManager
{
    // Properties
    private ProfileCollection profileCollection;
    
    // Methods
    public List<Window> GetCurrentProfileWindows()
    public int GetCurrentProfileIndex()
    public List<Window> SwitchToProfile(int profileIndex)
    public List<string> GetAllProfileNames()
    public bool RenameCurrentProfile(string newName)
    public bool AddOrUpdateWindow(Window window)
    public bool RemoveWindow(Window window)
    public Window GetWindowById(int id)
    public void RestoreAllWindows(IgnoreListManager ignoreListManager)
    public WindowPosAndSize GetSavedWindowPosAndSize(Window window)
    public void SaveChanges()
}
```

### IgnoreListManager Class
```csharp
class IgnoreListManager
{
    // Properties
    private List<string> _ignoreList;
    public List<string> IgnoreList { get; }
    
    // Methods
    public void LoadIgnoreList()
    public void SaveIgnoreList()
    public bool AddToIgnoreList(string windowTitle)
    public bool RemoveFromIgnoreList(string windowTitle)
    public void UpdateIgnoreList(List<string> newList)
    public bool ShouldIgnoreWindow(Window window)
    public bool ShouldIgnoreWindow(string windowTitle)
    public void ClearIgnoreList()
    public List<string> GetIgnoreList()
    public string ExtractWindowTitle(string selectedText)
    public void SetIgnoreList(List<string> list)
}
```

### AppSettings Class
```csharp
static class AppSettings
{
    // Methods
    public static void Save(string key, string value)
    public static string Load(string key)
}

static class AppSettings<T>
{
    // Methods
    public static void Save(string key, T value)
    public static T Load(string key)
}
```

## Appendix B: UI Component Reference

### Main Form (Form1)
- **Controls**: ListBoxes, TextBoxes, Buttons, CheckBoxes, etc.
- **Events**: Load, FormClosing, button clicks, selection changes, etc.
- **Timers**: refreshTimer for periodic updates

### IgnoreForm
- **Controls**: ListBox, Button, Label
- **Events**: Load, FormClosing, button click, key press, etc.

## Appendix C: Windows API Integration

### Key Windows API Functions Used

- **FindWindow**: Finds a window by class name and window name
- **GetWindowRect**: Gets the dimensions of a window
- **SetWindowPos**: Changes the position and size of a window
- **GetWindowText**: Gets the text of a window
- **EnumWindows**: Enumerates all top-level windows
- **IsWindowVisible**: Checks if a window is visible
- **GetForegroundWindow**: Gets the foreground window
- **SetForegroundWindow**: Brings a window to the foreground
- **SetWindowsHookEx**: Sets a hook for window events (used in highlighting)
- **DwmGetWindowAttribute**: Gets extended window bounds for DWM-managed windows

## Appendix D: Change Log

### Version 1.0
- Initial release
- Basic window position/size saving and restoring
- System tray integration
- Profile support
- Ignore list
- Window highlighting

### Version 1.1
- Added "Use Percentages" option (experimental)
- Improved window matching logic
- Enhanced ignore list functionality
- Added window highlighting
- Fixed bugs with File Explorer windows 