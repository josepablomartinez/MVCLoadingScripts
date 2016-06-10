using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Web.UI;

namespace Mapgenix.GSuite.Mvc
{
    internal static class MapResourceHelper
    {
        internal static void RegisterJavaScriptLibrary(Page page, string key, Uri uri)
        {
            if (!page.ClientScript.IsClientScriptBlockRegistered(key))
            {
                page.ClientScript.RegisterClientScriptBlock(page.GetType(), key, String.Format(CultureInfo.InvariantCulture, "<script type='text/javascript' src='{0}'></script>", uri.AbsoluteUri));
            }
        }

        internal static string GetResourceScript(string localName, Type type)
        {
            using (Stream stream = type.Assembly.GetManifestResourceStream(localName))
            {
                StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }

        internal static string GetFileScript(string filePath)
        {
            using (StreamReader reader = File.OpenText(filePath))
            {
                return reader.ReadToEnd();
            }
        }

        internal static void CreateResourceFiles(Page page, Type type)
        {
            string iconPath = page.MapPath("~/theme/default/img/");
            string iconLogPath = iconPath + "log.txt";
            if (!File.Exists(iconLogPath))
            {
                if (!Directory.Exists(iconPath))
                {
                    Directory.CreateDirectory(iconPath);
                }

                ResourceManager manager = new ResourceManager(typeof(ThemeResource));
                PropertyInfo[] infors = typeof(ThemeResource).GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

                foreach (PropertyInfo info in infors)
                {
                    if (info.PropertyType.Name == "Bitmap")
                    {
                        Bitmap bitmap = (Bitmap)(manager.GetObject(info.Name));
                        try
                        {
                            bitmap.Save(String.Format(CultureInfo.InvariantCulture, "{0}{1}.gif", iconPath, info.Name));
                        }
                        finally
                        {
                            bitmap.Dispose();
                        }
                    }
                }

                StreamWriter styleWriter = new StreamWriter(page.MapPath("~/theme/default/style.css"));
                styleWriter.Write(GetResourceScript("Mapgenix.GSuite.Web.Resources.style.css", type));
                styleWriter.Close();

                StreamWriter logWriter = File.CreateText(iconLogPath);
                logWriter.WriteLine("Icon created at:{0}", DateTime.Now.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture));
                logWriter.Close();
            }
        }



        internal static byte[] ConvertImageFormat(Bitmap bitmap, string imageFormat, int imageQuality)
        {
            string simpleImageFormat = imageFormat.Substring(imageFormat.LastIndexOf('/') + 1).ToUpperInvariant();

            MemoryStream memoryStream = new MemoryStream();
            try
            {
                if (simpleImageFormat == "JPEG")
                {

                    EncoderParameter encoderParameter = new EncoderParameter(Encoder.Quality, imageQuality);
                    EncoderParameters encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = encoderParameter;
                    ImageCodecInfo jpegCodecInfo = GetEncoder(ImageFormat.Jpeg);

                    bitmap.Save(memoryStream, jpegCodecInfo, encoderParameters);
                }
                else if (simpleImageFormat == "PNG" || simpleImageFormat == "GIF" || simpleImageFormat == "BMP")
                {

                    bitmap.Save(memoryStream, GetImageFormatByString(imageFormat));
                }
                else
                {
                    Graphics g = Graphics.FromImage(bitmap);
                    g.Clear(Color.LightGray);
                    g.DrawString(string.Format(CultureInfo.InvariantCulture, "Image format {0} is not supported.", simpleImageFormat), new Font("verdana", 8f), new SolidBrush(Color.Black), 10f, 10f);
                    bitmap.Save(memoryStream, ImageFormat.Jpeg);
                    g.Dispose();
                }

                return memoryStream.GetBuffer();
            }
            finally
            {
                memoryStream.Close();
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private static ImageFormat GetImageFormatByString(string imageFormatString)
        {
            string simpleImageFormat = imageFormatString.Substring(imageFormatString.LastIndexOf('/') + 1);

            switch (simpleImageFormat.ToUpperInvariant())
            {
                case "BMP": return ImageFormat.Bmp;
                case "EMF": return ImageFormat.Emf;
                case "EXIF": return ImageFormat.Exif;
                case "GIF": return ImageFormat.Gif;
                case "ICON": return ImageFormat.Icon;
                case "JPEG": return ImageFormat.Jpeg;
                case "MEMOERYBMP": return ImageFormat.MemoryBmp;
                case "PNG": return ImageFormat.Png;
                case "TIFF": return ImageFormat.Tiff;
                case "WMF": return ImageFormat.Wmf;
                default: return ImageFormat.Png;
            }
        }
    }
}
