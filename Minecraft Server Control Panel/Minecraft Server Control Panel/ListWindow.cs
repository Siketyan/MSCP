using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Codeplex.Data;
using System.IO;
using System.Reflection;

namespace Minecraft_Server_Control_Panel
{
    public partial class ListWindow : Form
    {
        public ListWindow(string Type)
        {
            InitializeComponent();
            string ListType = Type;
            switch (ListType)
            {
                case "BAN":
                    Text = "BANリストの管理";
                    if (!File.Exists(Program.ProgramDirectory + @"\Server\banned-players.json"))
                    {
                        MessageBox.Show("まだBANリストデータが作成されていません。\n一度サーバーを起動してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Close();
                    }
                    else
                    {
                        var json = DynamicJson.Parse(File.ReadAllText(Program.ProgramDirectory + @"\Server\banned-players.json"));
                        foreach (var data in json)
                        {
                            Avatar[data.name] = new ClassAvatar();
                            UserList.Items.Add(data.name);
                            AsyncGetAvatar(data.name);
                            Avatar[data.name].ip_update(data.name);
                        }
                    }   
                    break;

                case "OP":
                    Text = "OPリストの管理";
                    if (!File.Exists(Program.ProgramDirectory + @"\Server\ops.json"))
                    {
                        MessageBox.Show("まだOPリストデータが作成されていません。\n一度サーバーを起動してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Close();
                    }
                    else
                    {
                        var json = DynamicJson.Parse(File.ReadAllText(Program.ProgramDirectory + @"\Server\ops.json"));
                        foreach (var data in json)
                        {
                            Avatar[data.name] = new ClassAvatar();
                            UserList.Items.Add(data.name);
                            AsyncGetAvatar(data.name);
                            Avatar[data.name].ip_update(data.name);
                        }
                    }
                    break;

                case "White":
                    Text = "ホワイトリストの管理";
                    if (!File.Exists(Program.ProgramDirectory + @"\Server\whitelist.json"))
                    {
                        MessageBox.Show("まだホワイトリストデータが作成されていません。\n一度サーバーを起動してください。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Close();
                    }
                    else
                    {
                        var json = DynamicJson.Parse(File.ReadAllText(Program.ProgramDirectory + @"\Server\whitelist.json"));
                        foreach (var data in json)
                        {
                            Avatar[data.name] = new ClassAvatar();
                            UserList.Items.Add(data.name);
                            AsyncGetAvatar(data.name);
                            Avatar[data.name].ip_update(data.name);
                        }
                    }
                    break;
            }
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
    }
}
