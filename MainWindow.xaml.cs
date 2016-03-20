using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Minecraft_Server_Control_Panel
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Avatar> Users;
        ServerControl Controller;
        private ObservableCollection<string> oc;
        private ScrollViewer sv;

        public MainWindow()
        {
            InitializeComponent();
            Users = new List<Avatar>();
            Controller = new ServerControl(this);
        }

        private void RibbonLoaded(object sender, RoutedEventArgs e)
        {
            Grid child = VisualTreeHelper.GetChild((DependencyObject)sender, 0) as Grid;
            if (child != null)
            {
                child.RowDefinitions[0].Height = new GridLength(0);
            }
        }

        public void ButtonSetStart()
        {
            ButtonStart.IsEnabled = false;
            ButtonStop.IsEnabled = true;
        }
        public void ButtonSetStop()
        {
            ButtonStart.IsEnabled = true;
            ButtonStop.IsEnabled = false;
            ButtonRestart.IsEnabled = false;
        }

        public void ConsoleChanged(string text)
        {
            if (Regex.IsMatch(text, @"^\[[0-9]{2}:[0-9]{2}:[0-9]{2}\][A-Za-z0-9 \[\]/]+INFO\]: [a-zA-Z0-9]+\[/[0-9\.]+:[0-9]+\] logged in with entity id [0-9]+ at \(.+\)$"))
            {
                Regex reg = new Regex(@"^\[[0-9]{2}:[0-9]{2}:[0-9]{2}\][A-Za-z0-9 \[\]/]+INFO\]: (?<ID>[a-zA-Z0-9]+)\[/(?<IP>[0-9\.]+):[0-9]+\] logged in with entity id [0-9]+ at \(.+\)$");
                Match m = reg.Match(text);
                Users.Add(new Avatar(m.Groups["ID"].Value));
                UserList.ItemsSource = Users;
            }
            if (Regex.IsMatch(text, @"^\[[0-9]{2}:[0-9]{2}:[0-9]{2}\][A-Za-z0-9 \[\]/]+INFO\]: [a-zA-Z0-9]+ left the game$"))
            {
                Regex reg = new Regex(@"^\[[0-9]{2}:[0-9]{2}:[0-9]{2}\][A-Za-z0-9 \[\]/]+INFO\]: (?<ID>[a-zA-Z0-9]+?) left the game$");
                Match m = reg.Match(text);
                Users.Remove(new Avatar(m.Groups["ID"].Value));
                UserList.ItemsSource = Users;
            }
            Log.Items.Add(text);
            var border = VisualTreeHelper.GetChild(UserList, 0) as Border;
            if (border != null)
            {
                var ListScroll = border.Child as ScrollViewer;
                if(ListScroll != null)
                {
                    ListScroll.ScrollToEnd();
                }
            }
        }
        public void PerformanceUpdate(string Cpu, string Mem, string VMem)
        {
            PerformanceCPU.Text = "CPU: " + Cpu;
            PerformanceRAM.Text = "物理メモリ: " + Mem;
            PerformanceVRAM.Text = "仮想メモリ: " + VMem;
        }

        void StartServer(object sender, RoutedEventArgs e)
        {
            Log.Items.Clear();
            Users.Clear();
        }
        void StopServer(object sender, RoutedEventArgs e)
        {
            ButtonStop.IsEnabled = false;
            ButtonRestart.IsEnabled = false;
            Controller.ConsoleWrite("stop");
        }
        void RestartServer(object sender, RoutedEventArgs e)
        {

        }

        void SendConsole(object sender, RoutedEventArgs e)
        {
            Controller.ConsoleWrite(ConsoleSender.Text);
        }
        void TextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (ConsoleSender.Text == "stop")
                {
                    StopServer(new object[]{}, new RoutedEventArgs());
                    ConsoleSender.Text = "";
                }
                else
                {
                    SendConsole(new object[]{}, new RoutedEventArgs());
                }
            }
        }
    }
}
