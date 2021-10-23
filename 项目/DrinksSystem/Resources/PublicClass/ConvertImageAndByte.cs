using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DrinksSystem.Resources.PublicClass
{
    class ConvertImageAndByte : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                byte[] binaryimagedata = value as byte[];
                if (binaryimagedata == null) return "";
                using (Stream imageStreamSource = new MemoryStream(binaryimagedata, false))
                {
                    JpegBitmapDecoder jpeDecoder = new JpegBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                    ImageSource imageSource = jpeDecoder.Frames[0];
                    return imageSource;
                }
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";
            string path = value.ToString().Substring(8, value.ToString().Length - 8);
            System.Drawing.Bitmap bitmap;
            BitmapSource bmp = new BitmapImage(new Uri(path, UriKind.Absolute));
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();

                enc.Frames.Add(BitmapFrame.Create(bmp));
                enc.Save(outStream);
                bitmap = new System.Drawing.Bitmap(outStream);
            }
            System.Drawing.Bitmap bm = new System.Drawing.Bitmap(bitmap);

            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            bm.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] imgBytes = stream.ToArray();
            stream.Close();
            return imgBytes;
        }
    }
}
