using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace Minecraft_Server_Control_Panel
{
    class Utility
    {
        private MainWindow Dad;
        
        public Utility(MainWindow me)
        {
            Dad = me;
        }

        public void ShowMessage(string Message, MessageBoxImage Icon, MessageBoxButton Button)
        {
            var Dialog = new MessageBoxEx();
            Dialog.Message = Message;
            Dialog.Button = Button;
            Dialog.Image = Icon;
            Dialog.Owner = Dad;
            Dialog.ShowDialog();
        }
    }
}
