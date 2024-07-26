using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace demo
{
    public class CommandExecutor
    {
        public async Task ExecuteCommandsFromCSV(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Debug.WriteLine("CSV file does not exist");
                    return;
                }

                var commands = await LoadCommandsAsync(filePath);
                Debug.WriteLine($"Loaded {commands.Count} commands");

                await ExecuteCommands(commands);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error executing commands: {ex.Message}");
            }
        }

        private async Task<List<CommandRecord>> LoadCommandsAsync(string filePath)
        {
            var commands = new List<CommandRecord>();

            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    string line;
                    bool isFirstLine = true;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        // Skip the header line
                        if (isFirstLine)
                        {
                            isFirstLine = false;
                            continue;
                        }

                        var values = line.Split(',');
                        if (values.Length >= 7)
                        {
                            commands.Add(new CommandRecord
                            {
                                EventType = values[0],
                                Timestamp = DateTime.Parse(values[1], null, DateTimeStyles.RoundtripKind),
                                KeyCode = values[2],
                                KeyChar = values[3],
                                Button = values[4],
                                X = string.IsNullOrEmpty(values[5]) ? 0 : int.Parse(values[5]),
                                Y = string.IsNullOrEmpty(values[6]) ? 0 : int.Parse(values[6])
                            });
                        }
                    }
                }
                Debug.WriteLine("CSV file loaded successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading CSV file: {ex.Message}");
            }

            return commands;
        }

        private async Task ExecuteCommands(List<CommandRecord> commands)
        {
            DateTime? previousTimestamp = null;

            foreach (var command in commands)
            {
                if (previousTimestamp.HasValue)
                {
                    var delay = command.Timestamp - previousTimestamp.Value;
                    if (delay.TotalMilliseconds > 0)
                    {
                        await Task.Delay(delay);
                    }
                }

                if (command.EventType.Contains("Key"))
                {
                    ExecuteKeyPress(command);
                }
                else if (command.EventType.Contains("Mouse"))
                {
                    ExecuteMouseClick(command);
                }

                previousTimestamp = command.Timestamp;
            }
        }

        [DllImport("user32.dll")]
        static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetCursorPos(int X, int Y);

        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            public int type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct InputUnion
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;
            [FieldOffset(0)]
            public KEYBDINPUT ki;
            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct HARDWAREINPUT
        {
            public uint uMsg;
            public ushort wParamL;
            public ushort wParamH;
        }

        private void ExecuteKeyPress(CommandRecord command)
        {
            try
            {
                ushort vk = Convert.ToUInt16(command.KeyCode);

                INPUT[] inputs = new INPUT[1];
                inputs[0].type = 1; // Keyboard input
                inputs[0].u.ki.wVk = vk;
                inputs[0].u.ki.wScan = 0;
                inputs[0].u.ki.time = 0;
                inputs[0].u.ki.dwExtraInfo = IntPtr.Zero;

                if (command.EventType == "KeyDown")
                {
                    inputs[0].u.ki.dwFlags = 0; // Key down
                }
                else if (command.EventType == "KeyUp")
                {
                    inputs[0].u.ki.dwFlags = 2; // Key up
                }

                uint result = SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
                if (result == 0)
                {
                    Debug.WriteLine($"Failed to execute {command.EventType} for key {command.KeyChar}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error executing key press: {ex.Message}");
            }
        }

        private void ExecuteMouseClick(CommandRecord command)
        {
            try
            {
                SetCursorPos(command.X, command.Y);

                uint buttonFlagDown = command.Button == "Left" ? 0x0002U : (command.Button == "Right" ? 0x0008U : 0x0020U); // 0x0020 for middle button
                uint buttonFlagUp = buttonFlagDown << 1;

                INPUT[] inputs = new INPUT[1];
                inputs[0].type = 0; // Mouse input
                inputs[0].u.mi.dx = 0;
                inputs[0].u.mi.dy = 0;
                inputs[0].u.mi.mouseData = 0;
                inputs[0].u.mi.time = 0;
                inputs[0].u.mi.dwExtraInfo = IntPtr.Zero;

                if (command.EventType == "MouseDown")
                {
                    inputs[0].u.mi.dwFlags = buttonFlagDown; // Mouse down
                }
                else if (command.EventType == "MouseUp")
                {
                    inputs[0].u.mi.dwFlags = buttonFlagUp; // Mouse up
                }

                uint result = SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
                if (result == 0)
                {
                    Debug.WriteLine($"Failed to execute {command.EventType} at ({command.X}, {command.Y}) with {command.Button}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error executing mouse click: {ex.Message}");
            }
        }
    }

    public class CommandRecord
    {
        public string EventType { get; set; }
        public DateTime Timestamp { get; set; }
        public string KeyCode { get; set; }
        public string KeyChar { get; set; }
        public string Button { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
