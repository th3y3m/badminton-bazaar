using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Services.Helper
{

    public class ImageService
    {

        public static byte[] GenerateImage(string text)
        {
            int width = 200;
            int height = 100;
            var bitmap = new Bitmap(width, height);
            var graphics = Graphics.FromImage(bitmap);

            graphics.Clear(Color.White);
            var font = new Font("Arial", 20, FontStyle.Bold, GraphicsUnit.Pixel);
            var textColor = Color.Black;
            var textBrush = new SolidBrush(textColor);

            graphics.DrawString(text, font, textBrush, new PointF(10, 40));

            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }

}
