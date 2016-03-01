using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Minecraft_Server_Control_Panel
{
    public partial class Form1 : Form
    {
        const int ImageSpace = 3;

        Image CreateAvatar(Image image, int w, int h)
        {
            float fw = (float)w / (float)image.Width;
            float fh = (float)h / (float)image.Height;

            float scale = Math.Min(fw, fh);
            int nw = (int)(image.Width * scale);
            int nh = (int)(image.Height * scale);

            return new Bitmap(image, nw, nh);
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Init(object sender, EventArgs e)
        {

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

        private void UserListDraw(object sender, DrawItemEventArgs e)
        {
            if (e.Index == -1) return;
            e.DrawBackground();
            Image Avatar = (Image)listBox1.Items[e.Index];
            e.Graphics.DrawImage(Avatar,
                e.Bounds.X + (e.Bounds.Width - Avatar.Width) / 2,
                e.Bounds.Y + (e.Bounds.Height - Avatar.Height) / 2);
            e.Graphics.DrawString("文字", e.Font, Brushes.Red, 48, 16 + Avatar.Height);
            e.DrawFocusRectangle();
        }
    }
}
