using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CallFsEB
{
    public partial class Form1 : Form
    {
        [DllImport("kernel32", SetLastError = true)]
        static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, MemoryProtection flNewProtect, out MemoryProtection lpflOldProtect);

        const int offset_def7 = 0x1230; // 7 args
        const int offset_def3 = 0x11D0; // 3 args

        delegate long Run();

        unsafe void Calc()
        {
            var t = 0xAADDFFEECCAAFFL;
            var list = new List<byte>();

            // push t
            // pop rax
            // ret
            list.Add(0x68);
            list.AddRange(BitConverter.GetBytes(t));
            list.Add(0x58);
            list.Add(0xC3);

            fixed (byte* pointer = list.ToArray())
            {
                MemoryProtection protection;
                if (!VirtualProtectEx(Process.GetCurrentProcess().Handle, (IntPtr)pointer, list.Count, MemoryProtection.ExecuteReadWrite, out protection))
                    throw new Win32Exception();

                var val = (Marshal.GetDelegateForFunctionPointer((IntPtr)pointer, typeof(Run)) as Run)();

                MessageBox.Show(val.ToString("X"));
                
                VirtualProtectEx(Process.GetCurrentProcess().Handle, (IntPtr)pointer, list.Count, protection, out protection);
            }
        }

        class ProcessEntry
        {
            public Process Process;
            public string Name
            {
                get { return string.Format("[{0}] {1}", Process.Id, Process.ProcessName); }
            }
        }

        public Form1()
        {
            InitializeComponent();

           // Calc();

            tbFuncAddress.Text = "0x0036B10"; // FrameScript::ExecuteBuffer
            this.Text = "Test injection \"sizeof(void*) = " + IntPtr.Size + "\"";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(1000);
            cbProcess.Items.Clear();

            foreach (var process in Process.GetProcessesByName("wow-64")
                .Concat(Process.GetProcessesByName("wow")))
            {
                cbProcess.Items.Add(new ProcessEntry { Process = process });
            }

            if (cbProcess.Items.Count > 0)
            {
                cbProcess.SelectedIndex = 0;
            }
            else
            {
                foreach (var process in Process.GetProcesses())
                {
                    cbProcess.Items.Add(new ProcessEntry { Process = process });
                }
            }
        }

        private void bInject_Click(object sender, EventArgs e)
        {
            if (cbProcess.SelectedIndex == -1)
                return;

            Console.Clear();

            var process = ((ProcessEntry)cbProcess.SelectedItem).Process;
            var memory  = new ProcessMemory(process);


            var src  = memory.WriteCString(tbCode.Text);
            var path = memory.WriteCString(tbPath.Text);
            var code = memory.Alloc(0x100);

            Console.WriteLine("Code [alloc]: 0x{0:X16}", code);

            try
            {
                var build = process.MainModule.FileVersionInfo.FilePrivatePart;
                long func = offset_def3;

                // Если это не тестовое приложение
                if (build != 0)
                {
                    Console.WriteLine("Build: " + build);
                    var a = tbFuncAddress.Text.Substring(2);
                    func = long.Parse(a, NumberStyles.AllowHexSpecifier);
                }
                else
                {
                    memory.Call(code, memory.Rebase(offset_def7), src.ToInt64(), path.ToInt64(), 0, 4, 5, 0xffddaaeeffaa, 7);
                }

                Console.WriteLine("Func address [no rebase]: 0x{0:X16}", func);
                Console.WriteLine("Base addr: 0x{0:X16}", memory.Process.MainModule.BaseAddress.ToInt64());

                memory.Call(code, memory.Rebase(func), src.ToInt64(), path.ToInt64(), 0);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                memory.Free(src);
                memory.Free(path);
                memory.Free(code);
            }
        }
    }
}
