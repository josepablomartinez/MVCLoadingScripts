using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.SessionState;
using Mapgenix.Canvas;
using Mapgenix.Layers;
using Mapgenix.Shapes;

namespace Mapgenix.GSuite.Mvc
{
    public class SingleThreadTileResource : IHttpHandler, IReadOnlySessionState
    {
        private const int MessageCountOfLine = 50;

        private string _boundingBox;
        private RectangleShape _tileExtent;
        private string _pageName;
        private string _clientId;
        private string _cacheId;
        private string _layerOverlayId;
        private double _scale;
        private int _zoom;

        private LayerOverlay _layerOverlay;
        private GeographyUnit _mapUnit;
        private BaseGeoBrush _backgroundFillBrush;

        private ClientCache _clientCache;
        private ServerCache _serverCache;
        private string _imageFormat;
        private int _jpegQuality;
        private int _tileWidth;
        private int _tileHeight;

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            ProcessRequestCore(context);
        }

        protected virtual void ProcessRequestCore(HttpContext context)
        {
            SetEnvironmentFromQueryString(context);
            SetEnvironmentFromSession(context);
            GenerateAndOutputTileImage(context);
        }
        private void SetEnvironmentFromQueryString(HttpContext context)
        {
            _boundingBox = context.Request.QueryString["BBOX"];
            _tileExtent = RectangleConverter.ConvertStringToRectangle(_boundingBox);
            _pageName = context.Server.UrlDecode(context.Request.QueryString["PageName"]);
            _clientId = context.Server.UrlDecode(context.Request.QueryString["ClientId"]);
            _cacheId = context.Server.UrlDecode(context.Request.QueryString["CacheId"]);
            _layerOverlayId = context.Server.UrlDecode(context.Request.QueryString["OverlayId"]);
            _tileWidth = Int32.Parse(context.Request.QueryString["WIDTH"], CultureInfo.InvariantCulture);
            _tileHeight = Int32.Parse(context.Request.QueryString["HEIGHT"], CultureInfo.InvariantCulture);
            _zoom = Int32.Parse(context.Request.QueryString["ZOOM"], CultureInfo.InvariantCulture);
        }

        private void SetEnvironmentFromSession(HttpContext context)
        {
            if (!string.IsNullOrEmpty(_clientId))
            {
                Collection<double> clientZoomLevelScales = (Collection<double>)context.Session[_pageName + _clientId + "ClientZoomLevelScales"];
                GeoKeyedCollection<BaseOverlay> overlays = (GeoKeyedCollection<BaseOverlay>)context.Session[_pageName + _clientId + "Overlays"];
                LayerOverlay backgroundOverlay = context.Session[_pageName + _clientId + "BackgroundOverlay"] as LayerOverlay;
                LayerOverlay staticOverlay = (LayerOverlay)context.Session[_pageName + _clientId + "StaticOverlay"];
                LayerOverlay dynamicOverlay = (LayerOverlay)context.Session[_pageName + _clientId + "DynamicOverlay"];
                _mapUnit = (GeographyUnit)context.Session[_pageName + _clientId + "MapUnit"];
                BackgroundLayer mapBackground = (BackgroundLayer)context.Session[_pageName + _clientId + "MapBackground"];

                _scale = clientZoomLevelScales[_zoom];

                if (mapBackground != null)
                {
                    _backgroundFillBrush = mapBackground.BackgroundBrush;
                }

                if (_layerOverlay == null && overlays != null && overlays.Count > 0)
                {
                    if (overlays.Contains(_layerOverlayId))
                    {
                        _layerOverlay = overlays[_layerOverlayId] as LayerOverlay;
                    }
                }
                if (_layerOverlay == null && backgroundOverlay != null && _layerOverlayId.Equals(backgroundOverlay.Id, StringComparison.Ordinal))
                {
                    _layerOverlay = backgroundOverlay;
                }
                if (_layerOverlay == null && staticOverlay != null && _layerOverlayId.Equals(staticOverlay.Id, StringComparison.Ordinal))
                {
                    _layerOverlay = staticOverlay;
                }
                if (_layerOverlay == null && dynamicOverlay != null && _layerOverlayId.Equals(dynamicOverlay.Id, StringComparison.Ordinal))
                {
                    _layerOverlay = dynamicOverlay;
                }

                if (_layerOverlay != null)
                {
                    _clientCache = _layerOverlay.ClientCache;
                    _serverCache = _layerOverlay.ServerCache;
                    _imageFormat = "image/" + _layerOverlay.WebImageFormat.ToString().ToUpperInvariant();
                    _jpegQuality = _layerOverlay.JpegQuality;

                    if (!string.IsNullOrEmpty(_cacheId))
                    {
                        context.Response.Cache.SetExpires(DateTime.Now.AddSeconds(_clientCache.Duration.TotalSeconds));
                        context.Response.Cache.SetCacheability(HttpCacheability.Public);
                    }
                    else
                    {
                        context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    }
                }
            }
        }

        private void GenerateAndOutputTileImage(HttpContext context)
        {
            Bitmap bitmap = new Bitmap(_tileWidth, _tileHeight);
            byte[] tileBuffer = null;
            try
            {
                tileBuffer = GenerateTileImage(bitmap);
            }
            catch (Exception ex)
            {
                PrintExceptionMessage(bitmap, ex.Message);
                tileBuffer = MapResourceHelper.ConvertImageFormat(bitmap, _imageFormat, _jpegQuality);
            }
            finally
            {
                bitmap.Dispose();
                context.Response.Clear();
                context.Response.ContentType = _imageFormat.ToUpperInvariant();
                context.Response.BinaryWrite(tileBuffer);
                context.ApplicationInstance.CompleteRequest();
            }
        }

        private static void PrintExceptionMessage(Bitmap bmp, string message)
        {
            int origLength = message.Length;
            int splitNum = origLength / MessageCountOfLine;
            if (origLength % MessageCountOfLine > 0)
            {
                splitNum += 1;
            }

            StringBuilder splits = new StringBuilder();
            for (int index = 0; index < splitNum; index++)
            {
                int startPosition = index * MessageCountOfLine;
                int endPosition = startPosition + MessageCountOfLine;
                if (endPosition > origLength)
                {
                    endPosition = origLength;
                }

                string lineMessage = message.Substring(startPosition, (endPosition - startPosition)) + Environment.NewLine;
                splits.AppendFormat(CultureInfo.InvariantCulture, lineMessage);
            }

            Font warterMarkFont = new Font("Arial", 7);
            Color warterMarkColor = Color.FromArgb(255, 0, 0, 0);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                g.DrawString(splits.ToString(), warterMarkFont, new SolidBrush(warterMarkColor), 0, 0, StringFormat.GenericDefault);
            }
        }

        private byte[] GenerateTileImage(Bitmap bitmap)
        {
            byte[] tileBuffer = null;

            if (!string.IsNullOrEmpty(_layerOverlay.ServerCache.CacheDirectory))
            {

                TileMatrix tileMatrix = new TileMatrix(_scale, _tileWidth, _tileHeight, _mapUnit);
                FileNativeImageTileCache tileCache = new FileNativeImageTileCache(_serverCache.CacheDirectory, _serverCache.CacheId, ConvertImageFormat(_layerOverlay.WebImageFormat), tileMatrix);
                tileCache.JpegQuality = Convert.ToInt16(_jpegQuality);

                NativeImageTile tile = tileCache.GetTile(_tileExtent);

                if (tile.NativeImage != null)
                {
                    tileBuffer = tile.NativeImage;
                }
                else
                {
                    tileBuffer = DrawTile(bitmap);
                    tileCache.SaveTile(new NativeImageTile(tileBuffer, _tileExtent, _scale));
                }
            }
            else
            {
                tileBuffer = DrawTile(bitmap);
            }

            return tileBuffer;
        }

        private byte[] DrawTile(Bitmap bitmap)
        {
            GdiPlusGeoCanvas geoCanvas = new GdiPlusGeoCanvas();

            if (_backgroundFillBrush != null && _layerOverlay.IsBaseOverlay)
            {
                geoCanvas.BeginDrawing(bitmap, _tileExtent, _mapUnit);
                geoCanvas.Clear(_backgroundFillBrush);
                geoCanvas.EndDrawing();
            }
            _layerOverlay.Draw(geoCanvas, bitmap, _tileExtent, _mapUnit);
            return MapResourceHelper.ConvertImageFormat(bitmap, _imageFormat, _jpegQuality);
        }

        private static TileImageFormat ConvertImageFormat(WebImageFormat webImageFormat)
        {
            if (webImageFormat == WebImageFormat.Jpeg)
            {
                return TileImageFormat.Jpeg;
            }
            else
            {
                return TileImageFormat.Png;
            }
        }
    }
}
