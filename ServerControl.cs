using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Minecraft_Server_Control_Panel
{
    class ServerControl
    {
        MainWindow Dad;
        Process Server;
        public bool ServerRunning { get; private set; }
        public int ServerProcessID { get; private set; }
        public PerformanceCounter ServerProcessCPU { get; private set; }

        public ServerControl(MainWindow me)
        {
            Dad = me;
        }

        public void ServerStart()
        {
            Environment.CurrentDirectory = App.ProgramDirectory + @"\Server";
            Server = new Process();
            Server.StartInfo = new ProcessStartInfo(Properties.Settings.Default.JavaPath);
            Server.StartInfo.Arguments = "-Xms" + Properties.Settings.Default.Xms +
                " -Xmx" + Properties.Settings.Default.Xmx +
                " -jar server.jar nogui";
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
                MessageBox.Show("Javaの場所を正しく指定してください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                Server = null;
                return;
            }
            Server.BeginOutputReadLine();
            Server.BeginErrorReadLine();
            ServerProcessID = Server.Id;
            ServerProcessCPU = new PerformanceCounter("Process", "% Processor Time", LookupInstance(ServerProcessID));
            Dad.ButtonSetStart();
            ServerRunning = true;
            AsyncServerStop();
            AsyncServerPerformance();
        }

        async private void AsyncServerStop()
        {
            await Task.Run(() => { Server.WaitForExit(); });
            Dad.ButtonSetStop();
            ServerRunning = false;
            Server = null;
        }
        
        async private void AsyncServerPerformance()
        {
            await Task.Run(() =>
            {
                while (ServerRunning)
                {
                    Dad.PerformanceUpdate(GetServerCPU() + "%", GetServerRAM() + "MB", GetServerVRAM() + "MB");
                    Thread.Sleep(Properties.Settings.Default.StatusRefresh);
                }
            });
            Dad.PerformanceUpdate("0%", "0MB", "0MB");
        }

        public string GetServerCPU()
        {
            try { return (Math.Floor(((ServerProcessCPU.NextValue() / Environment.ProcessorCount) * 10)) / 10).ToString(); }
            catch { return "0"; }
        }

        public string GetServerRAM()
        {
            try { return (Server.WorkingSet64 / 1024 / 1024).ToString(); }
            catch { return "0"; }
        }

        public string GetServerVRAM()
        {
            try { return (Server.VirtualMemorySize64 / 1024 / 1024).ToString(); }
            catch { return "0"; }
        }

        private string LookupInstance(long pid)
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
                catch (InvalidOperationException) { }
            }
            return InstanceName;
        }

        private void ConsoleRead(object sender, DataReceivedEventArgs e)
        {
            Process p = (Process)sender;
            if (!string.IsNullOrEmpty(e.Data))
            {
                Dad.ConsoleChanged(e.Data);
            }
        }

        public void ConsoleWrite(string text)
        {
            Server.StandardInput.WriteLine(text);
        }

        public void ServerStopForce()
        {
            Server.Kill();
        }
    }
}
