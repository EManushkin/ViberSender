using System;
using System.Windows.Forms;

namespace ViberSender2017
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run((Form)new MainForm());
        }
    }
}
