using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace DrinksSystem.Resources.PublicClass
{
    public class ByteConvertBitmapImage
    {
        //Byte转换为Iamge
        public static BitmapImage ByteArrayToBitmapImage(byte[] byteArray)
        {
            BitmapImage bmp = null;
            
            try
            {
                bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.StreamSource = new MemoryStream(byteArray);
                bmp.EndInit();
            }
            catch
            {
                bmp = null;
            }

            return bmp;
        }
        //Iamge转换为Byte
        public static byte[] BitmapImageToByteArray(BitmapImage bmp)
        {
            byte[] byteArray = null;

            try
            {
                Stream sMarket = bmp.StreamSource;

                if (sMarket != null && sMarket.Length > 0)
                {
                    //很重要，因为Position经常位于Stream的末尾，导致下面读取到的长度为0。 
                    sMarket.Position = 0;

                    using (BinaryReader br = new BinaryReader(sMarket))
                    {
                        byteArray = br.ReadBytes((int)sMarket.Length);
                    }
                }
            }
            catch
            {
                //other exception handling 
            }

            return byteArray;
        }
    }
}
