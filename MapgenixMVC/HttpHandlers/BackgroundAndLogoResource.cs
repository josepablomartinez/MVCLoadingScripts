using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.SessionState;
using Mapgenix.Canvas;
using Mapgenix.Layers;
using Mapgenix.Shapes;

namespace Mapgenix.GSuite.Mvc
{
    internal class BackgroundAndLogoResource : IHttpHandler, IReadOnlySessionState
    {
        private const string LogoResourceName = "Mapgenix.GSuite.Mvc.Resources.PoweredByMapgenix.png";
        private const string Background = "bg";
        private const string Logo = "logo";
        private string _type;

        public BackgroundAndLogoResource()
            : this(String.Empty)
        { }

        public BackgroundAndLogoResource(string type)
        {
            this._type = type;
        }

        public bool IsReusable
        {
            get { return true; }
        }

        //public void ProcessRequest(HttpContext context)
        //{
        //    if (_type.Equals(Background, StringComparison.OrdinalIgnoreCase))
        //    {
        //        OutputBackground(context);
        //    }
        //    else if (_type.Equals(Logo, StringComparison.OrdinalIgnoreCase))
        //    {
        //        OutputLogo(context);
        //    }
        //}

        //private static void OutputBackground(HttpContext context)
        //{
        //    string imageFormat = context.Request.QueryString["Format"];
        //    int imageQuality = int.Parse(context.Request.QueryString["Quality"], CultureInfo.InvariantCulture);

        //    RectangleShape extent = new RectangleShape(-180, 90, 180, -90);

        //    using (Bitmap bitmap = new Bitmap(256, 256))
        //    {
        //        GdiPlusGeoCanvas canvas = new GdiPlusGeoCanvas();
        //        canvas.DrawingQuality = DrawingQuality.HighSpeed;
        //        canvas.BeginDrawing(bitmap, extent, GeographyUnit.DecimalDegree);


        //        string pageName = context.Request.QueryString["PageName"];
        //        string clientId = context.Request.QueryString["ClientId"];
        //        string cacheId = context.Request.QueryString["CacheId"];

        //        BackgroundLayer mapBackground = (BackgroundLayer)context.Session[String.Concat(pageName, clientId, cacheId, "MapBackground")];

        //        if (mapBackground != null)
        //        {
        //            canvas.Clear(mapBackground.BackgroundBrush);
        //        }

        //        canvas.EndDrawing();

        //        context.Response.Clear();
        //        context.Response.Cache.SetExpires(DateTime.Now.AddDays(1.0));
        //        context.Response.Cache.SetCacheability(HttpCacheability.Public);
        //        context.Response.ContentType = imageFormat;
        //        context.Response.BinaryWrite(MapResourceHelper.ConvertImageFormat(bitmap, imageFormat, imageQuality));
        //        context.ApplicationInstance.CompleteRequest();
        //    }
        //}

        private void OutputLogo(HttpContext context)
        {
            Stream logoStream = GetType().Assembly.GetManifestResourceStream(LogoResourceName);

            byte[] buffer = new byte[logoStream.Length];
            logoStream.Read(buffer, 0, (int)logoStream.Length);
            logoStream.Close();

            context.Response.Cache.SetExpires(DateTime.Now.AddSeconds(1800));
            context.Response.Cache.SetCacheability(HttpCacheability.Public);
            context.Response.ContentType = "image/png";
            context.Response.BinaryWrite(buffer);
        }
    }
}
