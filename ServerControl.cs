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
        // 動的クラスの宣言
        MainWindow Dad;
        Utility Util;
        Config Conf;
        Process Server;

        // 変数の宣言
        public bool ServerRunning { get; private set; }
        public bool StartComplete { get; private set; }
        public int ServerProcessID { get; private set; }
        public PerformanceCounter ServerProcessCPU { get; private set; }

        public ServerControl(MainWindow me)
        {
            // 変数および動的クラスの初期化
            Dad = me;
            StartComplete = false;
            Util = new Utility(Dad);
            Conf = new Config(Dad);
        }

        public void ServerStart()
        {
            // 起動ボタンを無効化し、停止・再起動ボタンを有効化
            Dad.ButtonSetStart();

            // Serverディレクトリをチェックし、なかったら作成
            App.CheckDirectory(@"\Server");
            // JavaPath設定が空か
            if (Properties.Settings.Default.JavaPath == "")
            {
                Dad.ButtonSetStop();
                Util.ShowMessage("Javaの絶対パスが指定されていません。", MessageBoxImage.Error, MessageBoxButton.OK);
                Server = null;
                Conf.ShowDialog();
                return;
            }
            // jarファイルが存在するか
            if (App.CheckFile(@"\Server\server.jar"))
            {
                Dad.ButtonSetStop();
                Util.ShowMessage("サーバーファイルがありません。設定からインポートしてください。", MessageBoxImage.Error, MessageBoxButton.OK);
                Server = null;
                Conf.ShowDialog();
                return;
            }

            // 作業ディレクトリをServerフォルダに移動
            Environment.CurrentDirectory = App.ProgramDirectory + @"\Server";

            // 新しいプロセスを作成し、Javaのパスと引数を設定
            Server = new Process();
            Server.StartInfo = new ProcessStartInfo(Properties.Settings.Default.JavaPath);
            Server.StartInfo.Arguments = Properties.Settings.Default.Options + " -jar server.jar nogui";

            // 各種プロセス設定
            Server.StartInfo.UseShellExecute = false;
            Server.StartInfo.RedirectStandardInput = true;
            Server.StartInfo.RedirectStandardOutput = true;
            Server.StartInfo.RedirectStandardError = true;
            Server.StartInfo.CreateNoWindow = true;
            Server.OutputDataReceived += ConsoleRead;
            Server.ErrorDataReceived += ConsoleRead;

            // サーバーを起動してみる
            try { Server.Start(); }
            // Win32Exceptionが発生した（Javaの場所が正しくない）なら
            catch (System.ComponentModel.Win32Exception)
            {
                Dad.ButtonSetStop();
                Util.ShowMessage("Javaの場所を正しく指定してください。", MessageBoxImage.Error, MessageBoxButton.OK);
                Server = null;
                Conf.ShowDialog();
                return;
            }

            // ステータスバーを変更
            Dad.Status.Content = "サーバーを起動しています。";

            // コンソールの読み取りを開始
            Server.BeginOutputReadLine();
            Server.BeginErrorReadLine();

            // PCのCPU/メモリ/vメモリ使用率測定の設定
            ServerProcessID = Server.Id;
            ServerProcessCPU = new PerformanceCounter("Process", "% Processor Time", LookupInstance(ServerProcessID));

            // サーバーが起動中であることを変数に格納
            ServerRunning = true;

            // サーバー停止とパフォーマンスの監視を開始
            AsyncServerStop();
            AsyncServerPerformance();
        }

        /// <summary>
        /// サーバー停止を監視
        /// </summary>
        async private void AsyncServerStop()
        {
            await Task.Run(() => { Server.WaitForExit(); });
            Dad.ButtonSetStop();
            ServerRunning = false;
            StartComplete = false;
            Server = null;
        }
        /// <summary>
        /// CPU、メモリ、仮想メモリの使用量を監視
        /// </summary>
        async private void AsyncServerPerformance()
        {
            await Task.Run(() => //非同期処理
            {
                while (ServerRunning)
                {
                    Dad.PerformanceUpdate(GetServerCPU() + "%", GetServerRAM() + "MB", GetServerVRAM() + "MB");
                    Thread.Sleep(Properties.Settings.Default.StatusRefresh);
                }
            });
            Dad.PerformanceUpdate("0%", "0MB", "0MB"); // パフォーマンスの更新
        }

        /// <summary>
        /// CPU使用率を手に入れる
        /// </summary>
        /// <returns>CPU使用率</returns>
        public string GetServerCPU()
        {
            try { return (Math.Floor(((ServerProcessCPU.NextValue() / Environment.ProcessorCount) * 10)) / 10).ToString(); }
            catch { return "0"; }
        }
        /// <summary>
        /// メモリ使用量を手に入れる
        /// </summary>
        /// <returns>メモリ使用量</returns>
        public string GetServerRAM()
        {
            try { return (Server.WorkingSet64 / 1024 / 1024).ToString(); }
            catch { return "0"; }
        }
        /// <summary>
        /// 仮想メモリ使用量を手に入れる
        /// </summary>
        /// <returns>仮想メモリ使用量</returns>
        public string GetServerVRAM()
        {
            try { return (Server.VirtualMemorySize64 / 1024 / 1024).ToString(); }
            catch { return "0"; }
        }

        /// <summary>
        /// インスタンスを探して手に入れる
        /// </summary>
        /// <param name="pid">プロセスID</param>
        /// <returns>インスタンス名を返す。</returns>
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

        /// <summary>
        /// 新しいログを手に入れた時
        /// </summary>
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
        /// <summary>
        /// コンソールに書き込む（コマンド実行）
        /// </summary>
        /// <param name="text">コマンドー</param>
        public void ConsoleWrite(string text)
        {
            Server.StandardInput.WriteLine(text);
        }

        /// <summary>
        /// 強　制　終　了
        /// </summary>
        public void ServerStopForce()
        {
            Server.Kill();
        }
    }
}
