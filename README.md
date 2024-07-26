### README.md

```markdown
# Demo Project

This project demonstrates the following functionalities:
1. Automatic screen capturing.
2. Recording mouse and keyboard events and saving them to a CSV file.
3. Reading and executing recorded mouse and keyboard events from a CSV file.

## Project Structure

```
demo
├── App.xaml
├── App.xaml.cs
├── MainWindow.xaml
├── MainWindow.xaml.cs
├── ScreenCaptureHelper.cs
├── GlobalHookHelper.cs
└── CommandExecutor.cs
```

## Features

### 1. Automatic Screen Capturing

The `ScreenCaptureHelper` class handles capturing the screen every 2 seconds and saving the screenshots to a specified directory.

### 2. Recording Mouse and Keyboard Events

The `GlobalHookHelper` class sets up global hooks to capture mouse and keyboard events. These events are logged into a CSV file with their respective timestamps.

### 3. Executing Commands from CSV

The `CommandExecutor` class reads the recorded events from the CSV file and simulates the same mouse and keyboard actions.

## Usage

### Prerequisites

- .NET 6.0 SDK or later
- Visual Studio 2022 or later

### Setup

1. **Clone the repository:**
   ```bash
   git clone https://github.com/your-repo/demo.git
   cd demo
   ```

2. **Open the project in Visual Studio:**
   Open `demo.sln` in Visual Studio.

3. **Install the required NuGet packages:**
   - `CsvHelper` (latest version)
   - `InputSimulatorPlus` (version 1.0.7)
   - `Microsoft.Windows.SDK.BuildTools` (latest version)
   - `Microsoft.WindowsAppSDK` (latest version)
   - `System.Drawing.Common` (latest version)
   - `System.Windows.Extensions` (latest version)

### Running the Project

1. **Start Recording Events:**
   Click the "Start Recording Events" button to begin logging mouse and keyboard events to `D:\jianting\key_mouse_events.csv`.

2. **Start Screen Capture:**
   Click the "Start Screen Capture" button to start capturing the screen every 2 seconds. Screenshots are saved to `D:\jianting`.

3. **Execute Commands from CSV:**
   Click the "Execute Commands from CSV" button to read the events from `D:\jianting\key_mouse_events.csv` and simulate the recorded mouse and keyboard actions.

### Directory Structure

- `D:\jianting`:
  - Contains the screenshots captured by the application.
  - Contains the `key_mouse_events.csv` file which logs the recorded events.

## Code Explanation

### ScreenCaptureHelper.cs

Handles screen capturing using Windows API to capture the entire screen and save the image to a specified directory.

### GlobalHookHelper.cs

Sets up global hooks to monitor and log mouse and keyboard events. These events are saved to a CSV file with relevant details such as event type, timestamp, key code, key character, button, and coordinates.

### CommandExecutor.cs

Reads the logged events from the CSV file and simulates the mouse and keyboard actions using Windows API.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

```

This README file provides a comprehensive overview of the `demo` project, detailing its functionalities, setup instructions, usage, and code explanations. It can be directly copied into a `README.md` file in your project repository.
