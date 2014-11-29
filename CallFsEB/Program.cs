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
            Action f = () => form.tbConsole.Clear();
            if (form.tbConsole.InvokeRequired)
                form.tbConsole.BeginInvoke(f);
            else
                f();
        }



        public static void WriteLine()
        {
            Action<string> f = (s) => form.tbConsole.AppendText(s);
            if (form.tbConsole.InvokeRequired)
                form.tbConsole.BeginInvoke(f, "\r\n");
            else
                f("\r\n");
        }

        public static void WriteLine(string str)
        {
            Action<string> f = (s) => form.tbConsole.AppendText(s);
            if (form.tbConsole.InvokeRequired)
                form.tbConsole.BeginInvoke(f, str + "\r\n");
            else
                f(str + "\r\n");
        }

        public static void WriteLine(string format, params object[] args)
        {
            Action<string> f = (s) => form.tbConsole.AppendText(s);
            if (form.tbConsole.InvokeRequired)
                form.tbConsole.BeginInvoke(f, string.Format(format + "\r\n", args));
            else
                f(string.Format(format + "\r\n", args));
        }
    }
}
