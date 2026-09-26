# Grab Level Downloader

A desktop application for downloading custom levels from **Grab VR** with support for direct sideloading to VR headsets via ADB.

## Requirements

- **.NET 10.0** or later
- **Android Debug Bridge (ADB)** - For sideloading to headsets
- **Chromium/Edge** - Installed locally (used by Playwright)
- Windows OS (application built for Windows)

## Installation

1. Clone or download this repository

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the project:
   ```bash
   dotnet build --configuration Release
   ```

4. Run the application:
   ```bash
   dotnet run
   ```

   Or use the provided batch file:
   ```bash
   run.bat
   ```

## Usage

### Downloading a Level

1. **Obtain a Share Link** - Get a Grab VR level share link (format: `https://grabvr.quest/?level=<userId>:<timestamp>`)
2. **Paste in App** - Enter the link in the "Level Link" field
3. **Set Output Directory** - Choose where to save the `.level` file (defaults to current directory)
4. **Click Download** - Click the "Download" button to save locally

### Sideloading to Headset

1. **Connect Your Headset** - Ensure your VR headset is connected via USB and ADB is enabled
2. **Paste Share Link** - Enter the level link in the app
3. **Click "Download To Headset (ADB)"** - The app will:
   - Download the level file
   - Push it to your headset's Grab levels folder
   - Automatically clean up the local copy
4. **Check Logs** - Monitor the log window for success/error messages

## Dependencies

- **Raylib-cs** (8.1.0) - Graphics and windowing
- **rlImGui-cs** (3.2.0) - ImGui bindings for Raylib
- **ImGui.NET** (1.91.6.1) - Immediate-mode GUI
- **Microsoft.Playwright** (1.63.0) - Headless browser automation

## Project Structure

```
├── Program.cs              # Main application loop and UI
├── LevelDownloader.cs      # API interaction and level download logic
├── adb.cs                  # Android Debug Bridge wrapper
├── adb.exe                 # ADB executable
├── GrabLevelDownloader.csproj
└── run.bat                 # Batch script to run the app
```

## Troubleshooting

| Issue | Solution |
|-------|----------|
| **No connected headsets** | Enable ADB on your headset and connect via USB |
| **Download fails** | Ensure the share link is valid and properly formatted |
| **Chromium not found** | Install Microsoft Edge or ensure Chromium is available |
| **Permission denied (ADB)** | Run the app as Administrator or check ADB device permissions |
