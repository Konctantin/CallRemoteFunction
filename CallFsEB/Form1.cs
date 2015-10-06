using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CallFsEB
{
    public partial class Form1 : Form
    {
        const int offset_def7 = 0x1230; // 7 args
        const int offset_def3 = 0x3D0A0; // 3 args

        int counter = 0;

        private long FuncOffset
        {
            get { return long.Parse(tbFuncAddress.Text.Substring(2), NumberStyles.AllowHexSpecifier); }
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

            tbFuncAddress.Text = "0x003D0A0"; // FrameScript::ExecuteBuffer
            Text = "Test injection \"sizeof(void*) = " + IntPtr.Size + "\"";
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

            Console.WriteLine("sizeof(CONTEXT) = " + Marshal.SizeOf(typeof(CONTEXT)));
        }

        private void bInject_Click(object sender, EventArgs e)
        {
            if (cbProcess.SelectedIndex == -1) return;
            Console.Clear();

            try
            {
                ++counter;
                var process = ((ProcessEntry)cbProcess.SelectedItem).Process;
                var memory = new ProcessMemory(process);

                var build = process.MainModule.FileVersionInfo.FilePrivatePart;
                var func = build == 0 ? offset_def3 : FuncOffset;

                var src = memory.WriteCString(string.Format(tbParam1.Text, counter));
                var path = memory.WriteCString(tbParam2.Text + " " + counter);
                var alloc = memory.Alloc(0x1000);

                Console.WriteLine("Inject #{0}", counter);
                memory.Call(alloc, memory.Rebase(func), src.ToInt64(), path.ToInt64(), 0);

                // не освобождаем дескрипторы, необходимо для отладки процесса.
                //memory.Free(src);
                //memory.Free(path);
                //memory.Free(alloc);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
