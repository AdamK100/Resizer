using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace Resizer_App
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        private const int exifOrientationID = 0x112; //274

        public static void ExifRotate(Image img, Image img2)
        {
            if (!img.PropertyIdList.Contains(exifOrientationID))
                return;

            var prop = img.GetPropertyItem(exifOrientationID);
            int val = BitConverter.ToUInt16(prop.Value, 0);
            var rot = RotateFlipType.RotateNoneFlipNone;

            if (val == 3 || val == 4)
                rot = RotateFlipType.Rotate180FlipNone;
            else if (val == 5 || val == 6)
                rot = RotateFlipType.Rotate90FlipNone;
            else if (val == 7 || val == 8)
                rot = RotateFlipType.Rotate270FlipNone;

            if (val == 2 || val == 4 || val == 5 || val == 7)
                rot |= RotateFlipType.RotateNoneFlipX;

            if (rot != RotateFlipType.RotateNoneFlipNone)
                img2.RotateFlip(rot);
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            string dir = textBox1.Text;
            int width = 0;
            int height = 0;
            if (!Directory.Exists(dir + "Resized"))
            {
                Directory.CreateDirectory(dir + "Resized");
            }
            progressBar1.Maximum = Directory.GetFiles(dir).Length;
            foreach (string s in Directory.GetFiles(dir))
            {
                Image img = Image.FromFile(s);
                Image res;
                    if (comboBox1.Text == "480P")
                    {
                        height = 480;
                        width = 720;
                    }
                    if (comboBox1.Text == "720P")
                    {
                        height = 720;
                        width = 1080;
                    }
                    if (comboBox1.Text == "1080P")
                    {
                        height = 1080;
                        width = 1620;
                    }
                    res = ResizeImage(img, width, height);
                    ExifRotate(img, res);
                res.Save(dir + "Resized\\" + s.Split('\\')[s.Split('\\').Length - 1].Split('.')[0] + "l" + ".jpg", ImageFormat.Jpeg);
                progressBar1.Value += 1;
                img.Dispose();
                res.Dispose();
            }
        }
    }
}
