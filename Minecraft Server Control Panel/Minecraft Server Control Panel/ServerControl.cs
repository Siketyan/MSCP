using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minecraft_Server_Control_Panel
{
    static class ServerControl
    {
        static private Process Server;
        static public bool ServerRunning { get; private set; }
        static public int ServerProcessID { get; private set; }
        static public PerformanceCounter ServerProcessCPU { get; private set; }

        static public void ServerStart()
        {
            Environment.CurrentDirectory = Program.ProgramDirectory + @"\Server";
            Server = new Process();
            Server.StartInfo = new ProcessStartInfo(Properties.Settings.Default.JavaPath);
            Server.StartInfo.Arguments = "-Xms" + Properties.Settings.Default.Xms + 
                " -Xmx" + Properties.Settings.Default.Xmx + 
                " -jar \"" + Properties.Settings.Default.JarPath + "\" nogui";
            Server.StartInfo.UseShellExecute = false;
            Server.StartInfo.RedirectStandardInput = true;
            Server.StartInfo.RedirectStandardOutput = true;
            Server.StartInfo.RedirectStandardError = true;
            Server.StartInfo.CreateNoWindow = true;
            Server.OutputDataReceived += ConsoleRead;
            Server.ErrorDataReceived += ConsoleRead;
            try { Server.Start(); }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show("Javaの場所を正しく指定してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Server = null;
                return;
            }
            Server.BeginOutputReadLine();
            Server.BeginErrorReadLine();
            ServerProcessID = Server.Id;
            ServerProcessCPU = new PerformanceCounter("Process", "% Processor Time", LookupInstance(ServerProcessID));
            Program.App.ButtonSetStart();
            ServerRunning = true;
            AsyncServerStop();
            AsyncServerPerformance();
        }

        static async private void AsyncServerStop()
        {
            await Task.Run(() => { Server.WaitForExit(); });
            Program.App.ButtonSetStop();
            ServerRunning = false;
            Server = null;
        }

        private delegate void DelegateServerPerformance(string cpu, string mem, string vmem);
        static private DelegateServerPerformance ServerPerformanceDLG = new DelegateServerPerformance(Program.App.PerformanceUpdate);
        static async private void AsyncServerPerformance()
        {
            await Task.Run(() =>
            {
                while (ServerRunning)
                {
                    Program.App.Invoke(ServerPerformanceDLG, new object[] { GetServerCPU() + " %", GetServerRAM() + " MB", GetServerVRAM() + " MB" });
                    Thread.Sleep(1000);
                }
            });
            Program.App.Invoke(ServerPerformanceDLG, new object[] { "0 %", "0 MB", "0 MB" });
        }

        static public string GetServerCPU()
        {
            try { return (Math.Floor(((ServerProcessCPU.NextValue() / Environment.ProcessorCount) * 10)) / 10).ToString(); }
            catch { return "0"; }
        }

        static public string GetServerRAM()
        {
            try { return (Server.WorkingSet64 / 1024 / 1024).ToString(); }
            catch { return "0"; }
        }

        static public string GetServerVRAM()
        {
            try { return (Server.VirtualMemorySize64 / 1024 / 1024).ToString(); }
            catch { return "0"; }
        }

        static private string LookupInstance(long pid)
        {
            string InstanceName = "";
            var category = new PerformanceCounterCategory("Process");
            var interfaces = category.GetInstanceNames();

            foreach (var instance in interfaces)
            {
                var c = new PerformanceCounter("Process", "ID Process", instance);

                try
                {
                    if (pid == c.RawValue)
                    {
                        InstanceName = instance.ToString();
                        return InstanceName;
                    }
                }
                catch (InvalidOperationException) {}
            }
            return InstanceName;
        }

        private delegate void ConsoleReadDelegate(string text);
        static private ConsoleReadDelegate ConsoleReadDLG = new ConsoleReadDelegate(Program.App.ConsoleChanged);

        static private void ConsoleRead(object sender, DataReceivedEventArgs e)
        {
            Process p = (Process)sender;
            if (!string.IsNullOrEmpty(e.Data))
            {
                Program.App.Invoke(ConsoleReadDLG, new object[] { e.Data });
            }
        }

        static public void ConsoleWrite(string text)
        {
            Server.StandardInput.WriteLine(text);
        }

        static public void server_stop_force()
        {
            Server.Kill();
        }
    }
}
