using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace GrepolisBot2
{
    class CaptchaHandler
    {
        public static string imageToBase64(Image p_Image, System.Drawing.Imaging.ImageFormat p_Format)
        {
            using (MemoryStream l_Ms = new MemoryStream())
            {
                // Convert Image to byte[]
                p_Image.Save(l_Ms, p_Format);
                byte[] l_ImageBytes = l_Ms.ToArray();

                // Convert byte[] to Base64 String
                string l_Base64String = Convert.ToBase64String(l_ImageBytes, 0, l_ImageBytes.Length);
                return l_Base64String;
            }
        }
    }
}
