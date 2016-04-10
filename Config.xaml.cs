using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Minecraft_Server_Control_Panel
{
    /// <summary>
    /// Config.xaml の相互作用ロジック
    /// </summary>
    public partial class Config : Window
    {
        // 使用する動的クラスを宣言
        MainWindow Dad;
        Utility Util;

        // 使用する変数を宣言
        Properties.Settings Conf;

        public Config(MainWindow me)
        {
            // ウィンドウを形成
            InitializeComponent();

            // 動的クラスを初期化
            Dad = me;
            Util = new Utility(Dad);

            // 設定をConf変数に格納
            Conf = Properties.Settings.Default;
        }

        void Init(object sender, RoutedEventArgs e)
        {
            // 現在の設定を読み出し
            JavaPath.Text = Properties.Settings.Default.JavaPath;
            Options.Text = Properties.Settings.Default.Options;

            if (Properties.Settings.Default.Version != "")
            {
                string type = (Properties.Settings.Default.IsBukkit) ? "Bukkit " : "Minecraft ";
                Current.Content = "現在のバージョン: " + type + Conf.Version;
            }

            NotifyLog.IsChecked = Conf.NotifyLog;
            NotifyJoin.IsChecked = Conf.NotifyJoin;
            NotifyLeave.IsChecked = Conf.NotifyLeave;
        }
        
        void JavaOpen(object sender, RoutedEventArgs e)
        {
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.DefaultExt = "java.exe";
            if (Dialog.ShowDialog() == true) JavaPath.Text = Dialog.FileName;
        }
        void JarOpen(object sender, RoutedEventArgs e)
        {
            OpenFileDialog Dialog = new OpenFileDialog();
            Dialog.DefaultExt = "*.jar";
            if (Dialog.ShowDialog() == true) JarPath.Text = Dialog.FileName;
        }

        void Save(object sender, RoutedEventArgs e)
        {
            // 各種プロパティを設定
            Conf.JavaPath = JavaPath.Text;
            Conf.Options = Options.Text;
            Conf.NotifyLog = (bool)NotifyLog.IsChecked;
            Conf.NotifyJoin = (bool)NotifyJoin.IsChecked;
            Conf.NotifyLeave = (bool)NotifyLeave.IsChecked;

            // 設定を保存して閉じる
            Conf.Save();
            Close();
        }
        void Cancel(object sender, RoutedEventArgs e)
        {
            // ウィンドウを閉じる
            Close();
        }
    }
}
