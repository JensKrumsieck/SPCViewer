using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace SPCViewer.WPF.Extension
{
    /// <summary>
    /// Native Methods from WinAPI to Send and Get Messages between instances
    /// https://www.codeproject.com/Articles/1224031/Passing-Parameters-to-a-Running-Application-in-WPF
    /// </summary>
    public static class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

        private const int WM_COPYDATA = 0x004A;

        [StructLayout(LayoutKind.Sequential)]
        private struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpData;
        }

        public static string GetMessage(int message, IntPtr lParam)
        {
            if (message != WM_COPYDATA) return null;
            try
            {
                var data = Marshal.PtrToStructure<COPYDATASTRUCT>(lParam);
                var result = (string)data.lpData.Clone();
                return result;
            }
            catch
            {
                return null;
            }
        }

        public static void SendMessage(IntPtr hwnd, string message)
        {
            var messageBytes = Encoding.Unicode.GetBytes(message);
            var data = new COPYDATASTRUCT
            {
                dwData = IntPtr.Zero,
                lpData = message,
                cbData = messageBytes.Length + 1 /* +1 because of \0 string termination */
            };

            if (SendMessage(hwnd, WM_COPYDATA, IntPtr.Zero, ref data) != 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
}
