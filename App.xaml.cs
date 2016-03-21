using System;
using System.IO;
using System.Windows;

namespace Minecraft_Server_Control_Panel
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        static private System.Reflection.Assembly MyAssembly = System.Reflection.Assembly.GetEntryAssembly();
        static public string ProgramDirectory = Directory.GetParent(MyAssembly.Location).FullName;
        
        static public void CheckDirectory(string path)
        {
            if (!Directory.Exists(ProgramDirectory + path)) Directory.CreateDirectory(ProgramDirectory + path);
        }
        static public bool CheckFile(string path)
        {
            if (!File.Exists(ProgramDirectory + path)) return false;
            return true;
        }
    }
}
