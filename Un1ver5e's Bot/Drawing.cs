using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;

namespace Un1ver5e.Bot
{
    public static class Drawing
    {
        public static Stream GetBeerQuery(string authorAvatarURL, string respondentAvatarURL)
        {
            Bitmap beerQ = new Bitmap(1536, 512);
            Graphics graphics = Graphics.FromImage(beerQ);
            graphics.Clear(Color.White);
            using (WebClient client = new WebClient())
            {
                Stream author = client.OpenRead(authorAvatarURL);
                Stream respond = client.OpenRead(respondentAvatarURL);
                graphics.DrawImage(Image.FromStream(author), new Rectangle(0, 0, 512, 512));
                graphics.DrawImage(Image.FromStream(respond), new Rectangle(1024, 0, 512, 512));
                graphics.DrawImage(Image.FromFile(Service.Generals.BotFilesPath + "\\Gallery\\BeerAsk.png"), new Rectangle(512, 0, 512, 512));
            }
            var stream = new MemoryStream();
            beerQ.Save(stream, ImageFormat.Jpeg);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
        public static Stream GetBeerYes(string authorAvatarURL, string respondentAvatarURL)
        {
            Bitmap beerQ = new Bitmap(1536, 512);
            Graphics graphics = Graphics.FromImage(beerQ);
            graphics.Clear(Color.White);
            using (WebClient client = new WebClient())
            {
                Stream author = client.OpenRead(authorAvatarURL);
                Stream respond = client.OpenRead(respondentAvatarURL);
                graphics.DrawImage(Image.FromStream(author), new Rectangle(0, 0, 512, 512));
                graphics.DrawImage(Image.FromStream(respond), new Rectangle(1024, 0, 512, 512));
                graphics.DrawImage(Image.FromFile(Service.Generals.BotFilesPath + "\\Gallery\\BeerYes.png"), new Rectangle(512, 0, 512, 512));
            }
            var stream = new MemoryStream();
            beerQ.Save(stream, ImageFormat.Jpeg);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
        public static Stream GetBeerNo(string authorAvatarURL, string respondentAvatarURL)
        {
            Bitmap beerQ = new Bitmap(1536, 512);
            Graphics graphics = Graphics.FromImage(beerQ);
            graphics.Clear(Color.White);
            using (WebClient client = new WebClient())
            {
                Stream author = client.OpenRead(authorAvatarURL);
                Stream respond = client.OpenRead(respondentAvatarURL);
                graphics.DrawImage(Image.FromStream(author), new Rectangle(0, 0, 512, 512));
                graphics.DrawImage(Image.FromStream(respond), new Rectangle(1024, 0, 512, 512));
                graphics.DrawImage(Image.FromFile(Service.Generals.BotFilesPath + "\\Gallery\\BeerNo.png"), new Rectangle(512, 0, 512, 512));
            }
            var stream = new MemoryStream();
            beerQ.Save(stream, ImageFormat.Jpeg);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }



        public static Stream GetSmokeQuery(string authorAvatarURL, string respondentAvatarURL)
        {
            Bitmap beerQ = new Bitmap(1536, 512);
            Graphics graphics = Graphics.FromImage(beerQ);
            graphics.Clear(Color.White);
            using (WebClient client = new WebClient())
            {
                Stream author = client.OpenRead(authorAvatarURL);
                Stream respond = client.OpenRead(respondentAvatarURL);
                graphics.DrawImage(Image.FromStream(author), new Rectangle(0, 0, 512, 512));
                graphics.DrawImage(Image.FromStream(respond), new Rectangle(1024, 0, 512, 512));
                graphics.DrawImage(Image.FromFile(Service.Generals.BotFilesPath + "\\Gallery\\SmokeAsk.png"), new Rectangle(512, 0, 512, 512));
            }
            var stream = new MemoryStream();
            beerQ.Save(stream, ImageFormat.Jpeg);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
        public static Stream GetSmokeYes(string authorAvatarURL, string respondentAvatarURL)
        {
            Bitmap beerQ = new Bitmap(1536, 512);
            Graphics graphics = Graphics.FromImage(beerQ);
            graphics.Clear(Color.White);
            using (WebClient client = new WebClient())
            {
                Stream author = client.OpenRead(authorAvatarURL);
                Stream respond = client.OpenRead(respondentAvatarURL);
                graphics.DrawImage(Image.FromStream(author), new Rectangle(0, 0, 512, 512));
                graphics.DrawImage(Image.FromStream(respond), new Rectangle(1024, 0, 512, 512));
                graphics.DrawImage(Image.FromFile(Service.Generals.BotFilesPath + "\\Gallery\\SmokeYes.png"), new Rectangle(512, 0, 512, 512));
            }
            var stream = new MemoryStream();
            beerQ.Save(stream, ImageFormat.Jpeg);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
        public static Stream GetSmokeNo(string authorAvatarURL, string respondentAvatarURL)
        {
            Bitmap beerQ = new Bitmap(1536, 512);
            Graphics graphics = Graphics.FromImage(beerQ);
            graphics.Clear(Color.White);
            using (WebClient client = new WebClient())
            {
                Stream author = client.OpenRead(authorAvatarURL);
                Stream respond = client.OpenRead(respondentAvatarURL);
                graphics.DrawImage(Image.FromStream(author), new Rectangle(0, 0, 512, 512));
                graphics.DrawImage(Image.FromStream(respond), new Rectangle(1024, 0, 512, 512));
                graphics.DrawImage(Image.FromFile(Service.Generals.BotFilesPath + "\\Gallery\\SmokeNo.png"), new Rectangle(512, 0, 512, 512));
            }
            var stream = new MemoryStream();
            beerQ.Save(stream, ImageFormat.Jpeg);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}
