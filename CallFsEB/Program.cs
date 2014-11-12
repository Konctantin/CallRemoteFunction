using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CallFsEB
{
    public static class Console
    {
        static Form1 form;
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(form = new Form1());
        }

        public static void Clear()
        {
            form.tbConsole.Clear();
        }

        public static void WriteLine()
        {
            form.tbConsole.AppendText("\r\n");
        }

        public static void WriteLine(string str)
        {
            form.tbConsole.AppendText(str + "\r\n");
        }

        public static void WriteLine(string format, params object[] args)
        {
            form.tbConsole.AppendText(string.Format(format + "\r\n", args));
        }
    }
}
