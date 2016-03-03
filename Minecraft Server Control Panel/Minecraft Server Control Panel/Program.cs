using System;
using System.IO;
using System.Windows.Forms;

namespace Minecraft_Server_Control_Panel
{
    static class Program
    {
        static private System.Reflection.Assembly MyAssembly = System.Reflection.Assembly.GetEntryAssembly();
        static public string ProgramDirectory = Directory.GetParent(MyAssembly.Location).FullName;
        public static Form1 App;

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            App = new Form1();
            CheckDirectory(@"\Server");
            Application.Run(App);
        }

        static public void CheckDirectory(string path)
        {
            if (!Directory.Exists(ProgramDirectory + path)) Directory.CreateDirectory(ProgramDirectory + path);
        }
    }
}
