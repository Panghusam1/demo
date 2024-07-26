using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace demo
{
    public class ScreenCaptureHelper
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

        [DllImport("gdi32.dll")]
        private static extern IntPtr DeleteObject(IntPtr hObject);

        [DllImport("user32.dll")]
        private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        public async Task CaptureScreenAsync()
        {
            var desktopWnd = GetDesktopWindow();
            var desktopDC = GetWindowDC(desktopWnd);
            var memoryDC = CreateCompatibleDC(desktopDC);

            var width = GetSystemMetrics(0);
            var height = GetSystemMetrics(1);

            var bitmap = CreateCompatibleBitmap(desktopDC, width, height);
            var oldBitmap = SelectObject(memoryDC, bitmap);

            BitBlt(memoryDC, 0, 0, width, height, desktopDC, 0, 0, 0x00CC0020);

            SelectObject(memoryDC, oldBitmap);
            DeleteDC(memoryDC);
            ReleaseDC(desktopWnd, desktopDC);

            var bmp = Image.FromHbitmap(bitmap);
            DeleteObject(bitmap);

            var directoryPath = @"D:\jianting";
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var filePath = Path.Combine(directoryPath, $"Screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            bmp.Save(filePath, ImageFormat.Png);
        }

        [DllImport("gdi32.dll")]
        private static extern int DeleteDC(IntPtr hdc);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetSystemMetrics(int nIndex);
    }
}
