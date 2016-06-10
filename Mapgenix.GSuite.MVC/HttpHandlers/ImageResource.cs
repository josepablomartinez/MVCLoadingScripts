using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Web;
using Mapgenix.Canvas;

namespace Mapgenix.GSuite.Mvc
{
    internal class ImageResource : IHttpHandler
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            OutputIcon(context);
        }

        private static void OutputIcon(HttpContext context)
        {
            string iconPath = context.Server.UrlDecode(context.Request.QueryString["path"]);

            if (!String.IsNullOrEmpty(iconPath))
            {
                iconPath = context.Server.MapPath(iconPath);
            }

            string iconFormat = context.Request.QueryString["format"];
            string innerText = context.Request.QueryString["text"];
            string color = context.Request.QueryString["color"];
            string bgColor = context.Request.QueryString["bgcolor"];
            int iconWidth = Int32.Parse(context.Request.QueryString["width"], CultureInfo.InvariantCulture);
            int iconHeight = Int32.Parse(context.Request.QueryString["height"], CultureInfo.InvariantCulture);
            double angle = double.Parse(context.Request.QueryString["angle"], CultureInfo.InvariantCulture);
            float fontSize = float.Parse(context.Request.QueryString["fontsize"], CultureInfo.InvariantCulture);
            float x = float.Parse(context.Request.QueryString["x"], CultureInfo.InvariantCulture);
            float y = float.Parse(context.Request.QueryString["y"], CultureInfo.InvariantCulture);
            DrawingFontStyles fontStyle = (DrawingFontStyles)Int32.Parse(context.Request.QueryString["fontstyle"], CultureInfo.InvariantCulture);

            int bmpWidth = iconWidth;
            int bmpHeight = iconHeight;
            Bitmap bmp = new Bitmap(iconPath);
            if (iconWidth.Equals(0))
            {
                iconWidth = bmp.Width;
            }
            if (iconHeight.Equals(0))
            {
                iconHeight = bmp.Height;
            }

            DrawingRectangleF recF = new DrawingRectangleF();
            if (!string.IsNullOrEmpty(innerText))
            {
                recF = MeasureText(innerText, fontSize, fontStyle);
                double rotateRadient = angle * Math.PI / 180;
                double radient = recF.Width >= recF.Height ? recF.Width : recF.Height;
                if (angle - 0.0f > float.Epsilon)
                {
                    bmpWidth += Convert.ToInt32(radient * Math.Abs(Math.Cos(rotateRadient)));
                }
                else if (recF.Width > iconWidth)
                {
                    bmpWidth = (int)recF.Width;
                }

                if (angle - 0.0f > float.Epsilon)
                {
                    bmpHeight += Convert.ToInt32(radient * Math.Abs(Math.Sin(rotateRadient)));
                }
                else if (recF.Height > iconHeight)
                {
                    bmpHeight = (int)recF.Height;
                }
            }

            Bitmap iconBitMap = new Bitmap(bmpWidth + (int)Math.Ceiling(x), bmpHeight + (int)Math.Ceiling(y));
            Graphics g = Graphics.FromImage(iconBitMap);
            g.TextRenderingHint = TextRenderingHint.AntiAlias;

            if (String.IsNullOrEmpty(iconPath))
            {
                int tempIconWidth = (iconWidth == 0 ? 32 : iconWidth);
                int tempIconHeight = (iconHeight == 0 ? 32 : iconHeight);

                DrawImage(new Bitmap(tempIconWidth, tempIconHeight), g, iconBitMap, iconWidth, iconHeight, innerText, angle, fontSize, fontStyle, color, 0, 0, x, y, recF, bgColor);
            }
            else
            {
                DrawImage(bmp, g, iconBitMap, bmp.Width, bmp.Height, innerText, angle, fontSize, fontStyle, color, 0, 0, x, y, recF, bgColor);
            }

            MemoryStream iconStream = new MemoryStream();
            try
            {
                iconBitMap.Save(iconStream, ImageFormat.Png);
                context.Response.ContentType = iconFormat;
                context.Response.BinaryWrite(iconStream.GetBuffer());

            }
            finally
            {
                iconStream.Close();
                g.Dispose();
                iconBitMap.Dispose();
            }
        }

        private static DrawingRectangleF MeasureText(string text, float fontSize, DrawingFontStyles fontStyle)
        {
            Bitmap bitmap = null;
            Graphics graphics = null;

            SizeF size;

            try
            {
                bitmap = new Bitmap(1, 1);
                graphics = Graphics.FromImage(bitmap);

                FontStyle style = GetFontStyleFromDrawingFontStyles(fontStyle);
                size = graphics.MeasureString(text, new Font("ARIAL", fontSize, style));
                if (size.Width == 0 && size.Height != 0 && text.Length != 0)
                {
                    size.Width = 1;
                }
            }
            finally
            {
                if (graphics != null) { graphics.Dispose(); }
                if (bitmap != null) { bitmap.Dispose(); }
            }

            return new DrawingRectangleF(size.Width / 2, size.Height / 2, size.Width, size.Height);
        }

        private static void DrawImage(Bitmap bitmapToDraw, Graphics graphics, Bitmap source, int iconWidth, int iconHeight, string text, double rotateAngle, float fontSize, DrawingFontStyles fontStyle, string fontColor, float positionX, float positionY, float offsetX, float offsetY, DrawingRectangleF textRectangle, string textBgcolor)
        {
            float widthInScreen = bitmapToDraw.Width;
            float heightInScreen = bitmapToDraw.Height;
            if (iconWidth - 0.0f < float.Epsilon)
            {
                iconWidth = (int)widthInScreen;
            }
            if (iconHeight - 0.0f < float.Epsilon)
            {
                iconHeight = (int)heightInScreen;
            }
            Color color = ColorTranslator.FromHtml(fontColor);

            if (rotateAngle - 0.0f > float.Epsilon)
            {
                graphics.TranslateTransform((float)bitmapToDraw.Width / 2, (float)bitmapToDraw.Height / 2);
            }

            graphics.RotateTransform((float)rotateAngle);
            if (rotateAngle - 0.0f > float.Epsilon)
            {
                graphics.TranslateTransform(-(float)bitmapToDraw.Width / 2, -(float)bitmapToDraw.Height / 2);
            }
            graphics.DrawImage(bitmapToDraw, new Rectangle(0, 0, (int)(bitmapToDraw.Width), (int)(bitmapToDraw.Height)), new Rectangle(0, 0, bitmapToDraw.Width, bitmapToDraw.Height), GraphicsUnit.Pixel);
            if (!String.IsNullOrEmpty(text))
            {
                FontStyle style = GetFontStyleFromDrawingFontStyles(fontStyle);
                graphics.FillRectangle(new SolidBrush(ColorTranslator.FromHtml(textBgcolor)), new RectangleF(textRectangle.CenterX - textRectangle.Width / 2 + offsetX, textRectangle.CenterY - textRectangle.Height / 2 + offsetY, textRectangle.Width, textRectangle.Height));
                graphics.DrawString(text, new Font("ARIAL", fontSize, style), new SolidBrush(color), offsetX, offsetY);
            }
        }

        private static FontStyle GetFontStyleFromDrawingFontStyles(DrawingFontStyles drawingFontStyle)
        {
            FontStyle fontStyle;
            switch (drawingFontStyle)
            {
                case DrawingFontStyles.Bold:
                    fontStyle = FontStyle.Bold;
                    break;
                case DrawingFontStyles.Italic:
                    fontStyle = FontStyle.Italic;
                    break;
                case DrawingFontStyles.Strikeout:
                    fontStyle = FontStyle.Strikeout;
                    break;
                case DrawingFontStyles.Underline:
                    fontStyle = FontStyle.Underline;
                    break;
                default:
                    fontStyle = FontStyle.Regular;
                    break;
            }
            return fontStyle;
        }
    }
}
