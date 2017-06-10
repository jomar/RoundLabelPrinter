using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RoundLabelPrinter
{
    public class Entry
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int Count { get; set; }
        public float Abv { get; set; }
        public float ImageOpacity { get; set; }
        public Color TextColor1 { get; set; }
        public Color TextColor2 { get; set; }
        public Color TextColor3 { get; set; }

        [XmlIgnore]
        public Image Image { get; set; }

        public string ImageBase64Encoded
        {
            get
            {
                if (Image != null)
                    return ImageToBase64(Image, System.Drawing.Imaging.ImageFormat.Png);
                else
                    return null;
            }

            set
            {
                if (!string.IsNullOrEmpty(value))
                    Image = Base64ToImage(value);
                else
                    Image = null;
            }
        }

        private string ImageToBase64(Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();

                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }

        private Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
