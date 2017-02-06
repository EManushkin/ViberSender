using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Automation;
using System.Windows.Forms;

namespace ViberSender2017
{
    internal static class WinApi
    {
        public static uint WM_MOUSEMOVE = 512;
        public static uint WM_LBUTTONDOWN = 513;
        public static uint MK_LBUTTON = 1;
        public static uint WM_LBUTTONUP = 514;
        public static uint WM_CHAR = 258;
        public static uint WM_MOUSEACTIVATE = 33;
        public static uint WM_KEYDOWN = 256;
        public static uint VK_RETURN = 13;
        public static uint WM_KEYUP = 257;
        public static uint VK_BACK = 8;
        public static uint WM_CLOSE = 16;
        public static uint WM_PASTE = 770;
        public static uint VK_CONTROL = 17;
        public static uint VK_LCONTROL = 162;
        public static uint WM_CLEAR = 771;
        private static AutomationElement button = (AutomationElement)null;
        private static AutomationElement number_edit = (AutomationElement)null;
        private static Random r = new Random();
        private static IntPtr hwnd;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string lclassName, string windowTitle);

        public static IntPtr MakeLParam(int LoWord, int HiWord)
        {
            return (IntPtr)(HiWord << 16 | LoWord & (int)ushort.MaxValue);
        }

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        public static bool StartWork()
        {
            WinApi.SendMessage(WinApi.FindWindow("Qt5QWindowIcon", "Viber"), WinApi.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            WinApi.hwnd = IntPtr.Zero;
            WinApi.hwnd = WinApi.FindWindow("Qt5QWindowOwnDCIcon", (string)null);
            while (WinApi.hwnd == IntPtr.Zero)
                WinApi.hwnd = WinApi.FindWindow("Qt5QWindowOwnDCIcon", (string)null);
            Thread.Sleep(500);
            StringBuilder lpString = new StringBuilder(WinApi.GetWindowTextLength(WinApi.hwnd) + 1);
            WinApi.GetWindowText(WinApi.hwnd, lpString, lpString.Capacity);
            lpString.ToString();
            if (lpString.ToString() == "Viber")
                return false;
            WinApi.SetWindowPos(WinApi.hwnd, IntPtr.Zero, 0, 0, 800, 600, 64U);
            return true;
        }

        public static void ClickNumber()
        {
            do
            {
                if (WinApi.FindWindow("Qt5QWindowIcon", "Viber") != IntPtr.Zero)
                {
                    WinApi.SendMessage(WinApi.FindWindow("Qt5QWindowIcon", "Viber"), WinApi.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                    Thread.Sleep(100);
                }
                WinApi.SetForegroundWindow(WinApi.hwnd);
                WinApi.SendCtrlhotKey('D');
            }
            while (!WinApi.WaitSubject(257, 133));
        }

        public static void EnterNumber(string number)
        {
            do
            {
                if (WinApi.FindWindow("Qt5QWindowIcon", "Viber") != IntPtr.Zero)
                {
                    WinApi.SendMessage(WinApi.FindWindow("Qt5QWindowIcon", "Viber"), WinApi.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                    Thread.Sleep(100);
                }
                WinApi.SendMessage(WinApi.hwnd, WinApi.WM_MOUSEMOVE, IntPtr.Zero, WinApi.MakeLParam(264, 134));
                WinApi.SendMessage(WinApi.hwnd, WinApi.WM_LBUTTONDOWN, (IntPtr)((long)WinApi.MK_LBUTTON), WinApi.MakeLParam(264, 134));
                WinApi.SendMessage(WinApi.hwnd, WinApi.WM_LBUTTONUP, IntPtr.Zero, WinApi.MakeLParam(254, 134));
                Clipboard.SetText(number);
                WinApi.SetForegroundWindow(WinApi.hwnd);
                WinApi.SendCtrlhotKey('A');
                WinApi.SendCtrlhotKey('V');
                Thread.Sleep(100);
            }
            while (!WinApi.WaitSubject(288, 165));
        }

        public static void ClickMessage()
        {
            do
            {
                if (WinApi.FindWindow("Qt5QWindowIcon", "Viber") != IntPtr.Zero)
                {
                    WinApi.SendMessage(WinApi.FindWindow("Qt5QWindowIcon", "Viber"), WinApi.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                    Thread.Sleep(200);
                }
                WinApi.SendMessage(WinApi.hwnd, WinApi.WM_MOUSEMOVE, IntPtr.Zero, WinApi.MakeLParam(199, 477));
                Thread.Sleep(50);
                WinApi.SendMessage(WinApi.hwnd, WinApi.WM_LBUTTONDOWN, (IntPtr)((long)WinApi.MK_LBUTTON), WinApi.MakeLParam(199, 477));
                WinApi.SendMessage(WinApi.hwnd, WinApi.WM_LBUTTONUP, IntPtr.Zero, WinApi.MakeLParam(199, 477));
            }
            while (!WinApi.WaitSubject(20, 104));
        }

        public static bool SendMsg(string text, string path = null, bool flag = false)
        {
            if (WinApi.FindWindow("Qt5QWindowIcon", "Viber") != IntPtr.Zero)
            {
                WinApi.SendMessage(WinApi.FindWindow("Qt5QWindowIcon", "Viber"), WinApi.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                Thread.Sleep(100);
            }
            if (flag)
                WinApi.SendFile(path);
            if (!string.IsNullOrEmpty(text))
                Clipboard.SetText(text);
            WinApi.SetForegroundWindow(WinApi.hwnd);
            Thread.Sleep(50);
            WinApi.SendMessage(WinApi.hwnd, WinApi.WM_MOUSEMOVE, (IntPtr)((long)WinApi.MK_LBUTTON), WinApi.MakeLParam(467, 546));
            WinApi.SendMessage(WinApi.hwnd, WinApi.WM_LBUTTONDOWN, (IntPtr)((long)WinApi.MK_LBUTTON), WinApi.MakeLParam(467, 546));
            WinApi.SendMessage(WinApi.hwnd, WinApi.WM_LBUTTONUP, IntPtr.Zero, WinApi.MakeLParam(467, 546));
            WinApi.SendCtrlhotKey('V');
            Thread.Sleep(50);
            if (WinApi.FindWindow("Qt5QWindowIcon", "Viber") != IntPtr.Zero)
            {
                WinApi.SendMessage(WinApi.FindWindow("Qt5QWindowIcon", "Viber"), WinApi.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                Thread.Sleep(100);
            }
            WinApi.SendMessage(WinApi.hwnd, WinApi.WM_KEYDOWN, (IntPtr)((long)WinApi.VK_RETURN), (IntPtr)((long)(uint)((int)WinApi.MapVirtualKey(WinApi.VK_RETURN, 0U) << 16 | 1)));
            Thread.Sleep(100);
            IntPtr window = WinApi.FindWindow("Qt5QWindowIcon", "Viber");
            if (!(window != IntPtr.Zero))
                return true;
            WinApi.SendMessage(window, WinApi.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            return false;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        private static void SendCtrlhotKey(char key)
        {
            WinApi.keybd_event((byte)17, (byte)0, 0U, 0);
            WinApi.keybd_event((byte)key, (byte)0, 0U, 0);
            WinApi.keybd_event((byte)key, (byte)0, 2U, 0);
            WinApi.keybd_event((byte)17, (byte)0, 2U, 0);
        }

        private static void SendFile(string path)
        {
            string[] files = Directory.GetFiles(path);
            path = files[WinApi.r.Next(0, files.Length)];
            Thread.Sleep(300);
            IntPtr window1 = WinApi.FindWindow("Qt5QWindowOwnDCIcon", (string)null);
            WinApi.SendMessage(window1, WinApi.WM_MOUSEMOVE, IntPtr.Zero, WinApi.MakeLParam(335, 544));
            Thread.Sleep(100);
            WinApi.SendMessage(window1, WinApi.WM_LBUTTONDOWN, (IntPtr)((long)WinApi.MK_LBUTTON), WinApi.MakeLParam(335, 544));
            Thread.Sleep(50);
            WinApi.SendMessage(window1, WinApi.WM_LBUTTONUP, IntPtr.Zero, WinApi.MakeLParam(335, 544));
            Thread.Sleep(500);
            IntPtr window2 = WinApi.FindWindow((string)null, "Отправить файл");
            if (window2 == IntPtr.Zero)
                window2 = WinApi.FindWindow((string)null, "Send a File");
            while (window2 != IntPtr.Zero)
            {
                IntPtr windowEx = WinApi.FindWindowEx(WinApi.FindWindowEx(WinApi.FindWindowEx(window2, IntPtr.Zero, "ComboBoxEx32", ""), IntPtr.Zero, "ComboBox", ""), IntPtr.Zero, "Edit", "");
                Clipboard.SetText(path);
                WinApi.SendMessage(windowEx, WinApi.WM_CLEAR, IntPtr.Zero, IntPtr.Zero);
                WinApi.SendMessage(windowEx, WinApi.WM_PASTE, IntPtr.Zero, IntPtr.Zero);
                WinApi.SendMessage(WinApi.FindWindowEx(window2, IntPtr.Zero, "Button", (string)null), 245U, IntPtr.Zero, IntPtr.Zero);
                Thread.Sleep(100);
                window2 = WinApi.FindWindow((string)null, "Отправить файл");
                if (window2 == IntPtr.Zero)
                    window2 = WinApi.FindWindow((string)null, "Send a File");
                if (window2 != IntPtr.Zero)
                {
                    WinApi.SetForegroundWindow(window2);
                    SendKeys.SendWait("~");
                }
            }
        }

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        private static extern int SendMessage4(IntPtr hwndControl, uint Msg, int wParam, int lParam);

        private static int GetTextBoxTextLength(IntPtr hTextBox)
        {
            uint Msg = 14;
            return WinApi.SendMessage4(hTextBox, Msg, 0, 0);
        }

        public static bool WaitSubject(int x, int y)
        {
            Rectangle bounds1 = Screen.PrimaryScreen.Bounds;
            int width1 = bounds1.Width;
            bounds1 = Screen.PrimaryScreen.Bounds;
            int height1 = bounds1.Height;
            Bitmap bitmap = new Bitmap(width1, height1);
            Graphics graphics = Graphics.FromImage((Image)bitmap);
            graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
            byte r = bitmap.GetPixel(x, y).R;
            int num = 0;
            while ((int)bitmap.GetPixel(x, y).R > 250)
            {
                graphics.Dispose();
                bitmap.Dispose();
                Rectangle bounds2 = Screen.PrimaryScreen.Bounds;
                int width2 = bounds2.Width;
                bounds2 = Screen.PrimaryScreen.Bounds;
                int height2 = bounds2.Height;
                bitmap = new Bitmap(width2, height2);
                graphics = Graphics.FromImage((Image)bitmap);
                graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
                Thread.Sleep(100);
                ++num;
                if (num > 10)
                    return false;
            }
            graphics.Dispose();
            bitmap.Dispose();
            return true;
        }
    }
}
