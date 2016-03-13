using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minecraft_Server_Control_Panel
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Init(object sender, EventArgs e)
        {

        }

        public void ButtonSetStart()
        {
            RibbonPanelStart.Enabled = false;
            RibbonPanelStop.Enabled = true;
            RibbonPanelRestart.Enabled = true;
        }

        public void ButtonSetStop()
        {
            RibbonPanelStart.Enabled = true;
            RibbonPanelStop.Enabled = false;
            RibbonPanelRestart.Enabled = false;
        }

        private void RibbonPanelMouseIn(object sender, EventArgs e)
        {
            ((Panel)sender).BackColor = Color.DeepSkyBlue;
        }

        private void RibbonPanelMouseOut(object sender, EventArgs e)
        {
            ((Panel)sender).BackColor = Color.FromArgb(246, 246, 246);
        }

        private void RibbonImageMouseIn(object sender, EventArgs e)
        {
            string PanelName = ((PictureBox)sender).Name.Replace("Image", "Panel");
            Control[] Controls = this.Controls.Find(PanelName, true);
            for (int i = 0; i < Controls.Length; i++)
            {
                if (Controls[i] is Panel)
                {
                    ((Panel)Controls[i]).BackColor = Color.DeepSkyBlue;
                }
            }
        }

        private void RibbonImageMouseOut(object sender, EventArgs e)
        {
            string PanelName = ((PictureBox)sender).Name.Replace("Image", "Panel");
            Control[] Controls = this.Controls.Find(PanelName, true);
            for (int i = 0; i < Controls.Length; i++)
            {
                if (Controls[i] is Panel)
                {
                    ((Panel)Controls[i]).BackColor = Color.FromArgb(246, 246, 246);
                }
            }
        }

        private void RibbonLabelMouseIn(object sender, EventArgs e)
        {
            string LabelName = ((Label)sender).Name.Replace("Label", "Panel");
            Control[] Controls = this.Controls.Find(LabelName, true);
            for (int i = 0; i < Controls.Length; i++)
            {
                if (Controls[i] is Panel)
                {
                    ((Panel)Controls[i]).BackColor = Color.DeepSkyBlue;
                }
            }
        }

        private void RibbonLabelMouseOut(object sender, EventArgs e)
        {
            string LabelName = ((Label)sender).Name.Replace("Label", "Panel");
            Control[] Controls = this.Controls.Find(LabelName, true);
            for (int i = 0; i < Controls.Length; i++)
            {
                if (Controls[i] is Panel)
                {
                    ((Panel)Controls[i]).BackColor = Color.FromArgb(246, 246, 246);
                }
            }
        }

        private void RibbonTabChange(object sender, EventArgs e)
        {
            RibbonTabServer.BackColor = Color.White; RibbonPageServer.Visible = false;
            RibbonTabPlayer.BackColor = Color.White; RibbonPagePlayer.Visible = false;
            RibbonTabTools.BackColor = Color.White; RibbonPageServer.Visible = false;
            ((Label)sender).BackColor = Color.FromArgb(246, 246, 246);
            string PageName = ((Label)sender).Name.Replace("Tab", "Page");
            Control[] Controls = this.Controls.Find(PageName, true);
            for (int i = 0; i < Controls.Length; i++)
            {
                if (Controls[i] is Panel)
                {
                    ((Panel)Controls[i]).Visible = true;
                }
            }
        }

        public void ConsoleChanged(string text)
        {
            if (Regex.IsMatch(text, @"^\[[0-9]{2}:[0-9]{2}:[0-9]{2}\][A-Za-z0-9 \[\]/]+INFO\]: [a-zA-Z0-9]+\[/[0-9\.]+:[0-9]+\] logged in with entity id [0-9]+ at \(.+\)$"))
            {
                Regex reg = new Regex(@"^\[[0-9]{2}:[0-9]{2}:[0-9]{2}\][A-Za-z0-9 \[\]/]+INFO\]: (?<ID>[a-zA-Z0-9]+)\[/(?<IP>[0-9\.]+):[0-9]+\] logged in with entity id [0-9]+ at \(.+\)$");
                Match m = reg.Match(text);
                Avatar[m.Groups["ID"].Value] = new ClassAvatar();
                UserList.Items.Add(m.Groups["ID"].Value);
                AsyncGetAvatar(m.Groups["ID"].Value);
                Avatar[m.Groups["ID"].Value].ip_update(m.Groups["IP"].Value);
            }
            if (Regex.IsMatch(text, @"^\[[0-9]{2}:[0-9]{2}:[0-9]{2}\][A-Za-z0-9 \[\]/]+INFO\]: [a-zA-Z0-9]+ left the game$"))
            {
                Regex reg = new Regex(@"^\[[0-9]{2}:[0-9]{2}:[0-9]{2}\][A-Za-z0-9 \[\]/]+INFO\]: (?<ID>[a-zA-Z0-9]+?) left the game$");
                Match m = reg.Match(text);
                UserList.Items.Remove(m.Groups["ID"].Value);
                Avatar.Remove(m.Groups["ID"].Value);
            }
            Console.Items.Add(text);
            Console.TopIndex = Console.Items.Count - 1;
        }

        private class ClassAvatar
        {
            public bool is_enable { get; private set; } = false;
            public byte[] data { get; private set; }
            public string ip { get; private set; } = "";
            public bool data_update(byte[] input_data)
            {
                if (!is_enable)
                {
                    data = input_data;
                    is_enable = true;
                    return true;
                }
                return false;
            }
            public bool ip_update(string input_data)
            {
                if (ip == "")
                {
                    ip = input_data;
                    return true;
                }
                return false;
            }
        }

        Dictionary<string, ClassAvatar> Avatar = new Dictionary<string, ClassAvatar>();

        async private void AsyncGetAvatar(string id)
        {
            //string filename = Program.directory_path + @"\common\image_cache\" + id + @".png";
            Uri u = new Uri(@"https://minotar.net/avatar/" + id + @"/32.png");
            await Task.Run(() =>
            {
                using (System.Net.WebClient wc = new System.Net.WebClient())
                {
                    wc.Headers.Add("User-Agent", "Minecraft Server Control Panel");
                    Avatar[id].data_update(wc.DownloadData(u));
                }
            });
            this.UserList.Refresh();
        }

        private void UserListDraw(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            if (e.Index > -1)
            {
                Brush b = null;

                if ((e.State & DrawItemState.Selected) != DrawItemState.Selected)
                {
                    b = new SolidBrush(Color.Black);
                }
                else
                {
                    b = new SolidBrush(e.ForeColor);
                }

                string id = ((ListBox)sender).Items[e.Index].ToString();
                string ip = "";
                Image img = null;
                if (Avatar[id].is_enable)
                {
                    using (MemoryStream ms = new MemoryStream(Avatar[id].data))
                    {
                        img = Image.FromStream(ms);
                    }
                    ip += (" - " + Avatar[id].ip);
                }
                else
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    using (Stream s = assembly.GetManifestResourceStream("MinecraftServerManagementSystem.resource.head.png"))
                    {
                        img = Image.FromStream(s);
                    }
                    ip += (" - ***.***.***.***");
                }
                e.Graphics.DrawImage(img, 0, e.Bounds.Bottom - 32, img.Width, img.Height);

                e.Graphics.DrawString(id, e.Font, b, 37, e.Bounds.Bottom - 31);
                e.Graphics.DrawString(ip, e.Font, b, 37, e.Bounds.Bottom - 12);

                Pen p = new Pen(Color.Black, 1);
                e.Graphics.DrawRectangle(p, -1, e.Bounds.Bottom - 33, e.Bounds.Size.Width + 1, e.Bounds.Size.Height);


                b.Dispose();
            }

            e.DrawFocusRectangle();
        }

        public void PerformanceUpdate(string cpu, string mem, string vmem)
        {
            LabelPerformance.Text = "CPU: " + cpu + Environment.NewLine + "メモリ: " + mem + Environment.NewLine + "仮想メモリ: " + vmem;
        }

        private void ServerStart(object sender, EventArgs e)
        {
            Console.Items.Clear();
            ServerControl.ServerStart();
        }

        private void ServerStop(object sender, EventArgs e)
        {
            RibbonPanelStop.Enabled = false;
            RibbonPanelRestart.Enabled = false;
            ServerControl.ConsoleWrite("stop");
        }

        private void ServerRestart(object sender, EventArgs e)
        {

        }

        private void ConsoleSend(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (CommandSender.Text == "stop")
                {
                    ServerStop(new object[]{}, new KeyEventArgs(Keys.Enter));
                }
            }
        }

        int SelectedPlayer = -1;

        private void UserListSelected(object sender, EventArgs e)
        {
            Point p = Cursor.Position;
            if (UserList.SelectedIndex != -1) SelectedPlayer = UserList.SelectedIndex;
            UserList.SelectedIndex = -1;
        }

        private void OpenBANList(object sender, EventArgs e)
        {
            ListWindow Window = new ListWindow("BAN");
            Window.Show(this);
        }

        private void OpenOPList(object sender, EventArgs e)
        {
            ListWindow Window = new ListWindow("OP");
            Window.Show(this);
        }

        private void OpenWhiteList(object sender, EventArgs e)
        {
            ListWindow Window = new ListWindow("White");
            Window.Show(this);
        }

        private void OpenConfig(object sender, EventArgs e)
        {

        }

        private void FormClose(object sender, FormClosingEventArgs e)
        {
            if (ServerControl.ServerRunning)
            {
                if (MessageBox.Show("サーバーが起動中です。サーバーを強制終了してMSCPを終了しますか？", 
                    "強制終了の確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    ServerControl.ServerStopForce();
                }
            }
        }
    }
}
