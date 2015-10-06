using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace CallFsEB
{
    public partial class Form1 : Form
    {
        // build 20490
        private Dictionary<string, long> addr_list = new Dictionary<string, long> {
            ["wow"]     = 0, // todo
            ["wow-64"]  = 0x3D0A0,
            ["wowfake"] = 0, // todo
        };

        class ProcessEntry
        {
            public Process Process;
            public long address;
            public string Name
            {
                get { return $"[{Process.Id}] {Process.ProcessName} func: 0x{address:X}"; }
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cbProcess.Items.Clear();
            foreach (var process in Process.GetProcesses())
            {
                var name = process.ProcessName?.ToLower();
                if (addr_list.ContainsKey(name))
                {
                    var addr = addr_list[name];
                    cbProcess.Items.Add(new ProcessEntry { Process = process, address = addr });
                }
            }

            if (cbProcess.Items.Count > 0)
                cbProcess.SelectedIndex = 0;
        }

        int counter = 0;
        private void bInject_Click(object sender, EventArgs e)
        {
            if (cbProcess.SelectedIndex == -1)
                return;
            Console.Clear();

            try
            {
                ++counter;
                var processEntry = ((ProcessEntry)cbProcess.SelectedItem);
                if (processEntry.address == 0L)
                    throw new Exception("Don't supported!");

                var memory = new ProcessMemory(processEntry.Process);
                var src    = memory.WriteCString(string.Format(tbParam1.Text, counter));
                var path   = memory.WriteCString(tbParam2.Text + " " + counter);
                var alloc  = memory.Alloc(0x1000);

                Console.WriteLine("Inject #{0}", counter);
                memory.Call(alloc, memory.Rebase(processEntry.address), src.ToInt64(), path.ToInt64(), 0);

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
