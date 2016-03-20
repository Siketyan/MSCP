using System;
using System.Windows.Media.Imaging;
using System.Net;
using System.IO;
using System.Windows.Media;

namespace Minecraft_Server_Control_Panel
{
    class Avatar
    {
        public string name { get; set; }
        public int size { get; set; }
        public ImageSource image { get; set; }

        public Avatar(string name)
        {
            this.name = name;
            var image = new BitmapImage();
            int BytesToRead = 100;
            int size = Properties.Settings.Default.AvatarSize;

            WebRequest request = WebRequest.Create(new Uri("https://minotar.net/helm/" + name + "/" + size + ".png", UriKind.Absolute));
            request.Timeout = -1;
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            BinaryReader reader = new BinaryReader(responseStream);
            MemoryStream memoryStream = new MemoryStream();

            byte[] bytebuffer = new byte[BytesToRead];
            int bytesRead = reader.Read(bytebuffer, 0, BytesToRead);

            while (bytesRead > 0)
            {
                memoryStream.Write(bytebuffer, 0, bytesRead);
                bytesRead = reader.Read(bytebuffer, 0, BytesToRead);
            }

            image.BeginInit();
            memoryStream.Seek(0, SeekOrigin.Begin);

            image.StreamSource = memoryStream;
            image.EndInit();

            this.image = image;
        }
    }
}
