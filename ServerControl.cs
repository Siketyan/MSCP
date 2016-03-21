using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Minecraft_Server_Control_Panel
{
    class ServerControl
    {
        MainWindow Dad;
        Utility Util;
        Process Server;
        public bool ServerRunning { get; private set; }
        public bool StartComplete { get; private set; }
        public int ServerProcessID { get; private set; }
        public PerformanceCounter ServerProcessCPU { get; private set; }

        public ServerControl(MainWindow me)
        {
            Dad = me;
            StartComplete = false;
            Util = new Utility(Dad);
        }

        public void ServerStart()
        {
            Dad.ButtonSetStart();
            App.CheckDirectory(@"\Server");
            if (Properties.Settings.Default.JavaPath == "" ||
                Properties.Settings.Default.Xmx == "" ||
                Properties.Settings.Default.Xms == "")
            {
                Dad.ButtonSetStop();
                Util.ShowMessage("空のJava設定が存在します。Javaの場所、最大/最小メモリ使用量を正しく指定してください。", MessageBoxImage.Error, MessageBoxButton.OK);
                Server = null;
                //TODO: Configウィンドウを開く
                return;
            }
            if (App.CheckFile(@"\Server\server.jar"))
            {
                Dad.ButtonSetStop();
                Util.ShowMessage("サーバーファイルがありません。設定からインポートしてください。", MessageBoxImage.Error, MessageBoxButton.OK);
                Server = null;
                //TODO: Configウィンドウを開く
                return;
            }
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
                Dad.ButtonSetStop();
                Util.ShowMessage("Javaの場所を正しく指定してください。", MessageBoxImage.Error, MessageBoxButton.OK);
                Server = null;
                //TODO: 設定ウィンドウを開く
                return;
            }

            Dad.Status.Content = "サーバーを起動しています。";

            Server.BeginOutputReadLine();
            Server.BeginErrorReadLine();

            ServerProcessID = Server.Id;
            ServerProcessCPU = new PerformanceCounter("Process", "% Processor Time", LookupInstance(ServerProcessID));

            ServerRunning = true;

            AsyncServerStop();
            AsyncServerPerformance();
        }

        async private void AsyncServerStop()
        {
            await Task.Run(() => { Server.WaitForExit(); });
            Dad.ButtonSetStop();
            ServerRunning = false;
            StartComplete = false;
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
                if (Regex.IsMatch(e.Data, @"^\[[0-9]{2}:[0-9]{2}:[0-9]{2}\][A-Za-z0-9 \[\]/]+INFO\]: Done ([0-9]{2}.[0-9]{3})! For help, type " + "\"help\" or \"?\"$"))
                {
                    Regex reg = new Regex(@"^\[[0-9]{2}:[0-9]{2}:[0-9]{2}\][A-Za-z0-9 \[\]/]+INFO\]: Done \((?<Sec1>[0-9]{2})\.(?<Sec2>[0-9]{3})\)! For help, type " + "\"help\" or \"?\"$");
                    Match m = reg.Match(e.Data);
                    StartComplete = true;
                    Dad.Status.Content = "サーバーが起動しました。 (" + m.Groups["Sec1"].Value + "." + m.Groups["Sec2"].Value + "秒)";
                }
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
