using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace demo
{
    public sealed partial class MainWindow : Window
    {
        private DispatcherTimer _timer;
        private ScreenCaptureHelper _captureHelper;
        private CommandExecutor _commandExecutor;

        public MainWindow()
        {
            this.InitializeComponent();
            _captureHelper = new ScreenCaptureHelper();
            _commandExecutor = new CommandExecutor();
            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, WindowEventArgs e)
        {
            GlobalHookHelper.Stop();
        }

        private void StartRecordingEvents_Click(object sender, RoutedEventArgs e)
        {
            GlobalHookHelper.Start();
        }

        private void StartScreenCapture_Click(object sender, RoutedEventArgs e)
        {
            StartCapture();
        }

        private async void ExecuteCommandsFromCSV_Click(object sender, RoutedEventArgs e)
        {
            var filePath = @"D:\jianting\key_mouse_events.csv";
            await _commandExecutor.ExecuteCommandsFromCSV(filePath);
        }

        private void StartCapture()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(2);
            _timer.Tick += async (s, e) => await CaptureScreen();
            _timer.Start();
        }

        private async Task CaptureScreen()
        {
            await _captureHelper.CaptureScreenAsync();
        }
    }
}
