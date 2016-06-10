using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using Mapgenix.Canvas;
using Mapgenix.Shapes;
using Mapgenix.Utils;
using Mapgenix.Layers;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class LayerOverlay : BaseOverlay {
        private int _tileWidth;
        private int _tileHeight;
        private int _jpegQuality;
        private int _tileMargin;
        private bool _isMultithreadDisabled;
        private SafeCollection<BaseLayer> _layers;
        private TransitionEffect _transitionEffect;
        private TileType _tileType;
        private ServerCache _serverCache;
        private ClientCache _clientCache;
        private WebImageFormat _webImageFormat;
        private WrapDatelineMode _wrapDateLine;

        public LayerOverlay()
            : this(Guid.NewGuid().ToString(), true, TileType.MultipleTile) { }

        public LayerOverlay(string id)
            : this(id, true, TileType.MultipleTile) { }

        public LayerOverlay(string id, bool isBaseOverlay, TileType tileType)
            : base(id, isBaseOverlay) {
            //Validators.CheckTileTypeIsValid(tileType);

            this._tileType = tileType;
            this._tileHeight = 256;
            this._tileWidth = 256;
            this._jpegQuality = 100;
            this._webImageFormat = WebImageFormat.Png;
            this._transitionEffect = TransitionEffect.Stretching;
            this._serverCache = new ServerCache();
            this._clientCache = new ClientCache();
            this.ExtraParameter = DateTime.Now.ToString("ddhhmmssms", CultureInfo.InvariantCulture);
            this.Projection = "EPSG:4326";
            this._wrapDateLine = WrapDatelineMode.None;
        }

        public void SetBaseEpsgProjection(string epsgProjection) {
            //Validators.CheckOverlayEpsgProjectionGetAndSetValid(IsBaseOverlay);
            this.Projection = epsgProjection;
        }

        public string GetBaseEpsgProjection() {
            //Validators.CheckOverlayEpsgProjectionGetAndSetValid(IsBaseOverlay);
            return Projection;
        }

        [JsonMember(MemberName = "w")]
        public int TileWidth {
            get {
                return _tileWidth;
            }
            set {
                //Validators.CheckValueIsGreaterThanZero(value, "TileWidth");
                _tileWidth = value;
            }
        }

        [JsonMember(MemberName = "h")]
        public int TileHeight {
            get {
                return _tileHeight;
            }
            set {
                //Validators.CheckValueIsGreaterThanZero(value, "TileHeight");
                _tileHeight = value;
            }
        }

        [JsonMember(MemberName = "transitionEffect")]
        public TransitionEffect TransitionEffect {
            get {
                return _transitionEffect;
            }
            set {
                //Validators.CheckTransitionEffectIsValid(value);
                _transitionEffect = value;
            }
        }

        [JsonMember(MemberName = "singleTile")]
        public TileType TileType {
            get {
                return _tileType;
            }
            set {
                //Validators.CheckTileTypeIsValid(value);
                _tileType = value;
            }
        }

        [JsonMember(MemberName = "buffer")]
        public int TileMargin {
            get {
                return _tileMargin;
            }
            set {
                //Validators.CheckValueIsGreaterOrEqualToZero(value, "TileMargin");
                _tileMargin = value;
            }
        }

        public ServerCache ServerCache {
            get {
                return _serverCache;
            }
            set {
                //Validators.CheckParameterIsNotNull(value, "ServerCache");
                _serverCache = value;
            }
        }

        [JsonMember(MemberName = "clientCache")]
        public ClientCache ClientCache {
            get {
                return _clientCache;
            }
            set {
                //Validators.CheckParameterIsNotNull(value, "ClientCache");
                _clientCache = value;
            }
        }

        [JsonMember(MemberName = "imageFormat")]
        public WebImageFormat WebImageFormat {
            get {
                return _webImageFormat;
            }
            set {
                //Validators.CheckWebImageFormatIsValid(value);
                _webImageFormat = value;
            }
        }

        [JsonMember(MemberName = "jpegQuality")]
        public int JpegQuality {
            get {
                return _jpegQuality;
            }
            set {
                //Validators.CheckJpegCompressionQuality(value, "JpegQuality");
                _jpegQuality = value;
            }
        }

        [JsonMember(MemberName = "singleThread")]
        public bool IsMultiThreadDisabled {
            get { return _isMultithreadDisabled; }
            set { _isMultithreadDisabled = value; }
        }

        public SafeCollection<BaseLayer> Layers {
            get {
                if (_layers == null) {
                    _layers = new SafeCollection<BaseLayer>();
                }
                return _layers;
            }
        }

        public override bool IsVisible {
            get {
                return base.IsVisible;
            }
            set {
                base.IsVisible = value;
            }
        }

        [JsonMember(MemberName = "otype")]
        protected override string OverlayType {
            get {
                return "LAYER";
            }
        }

        [JsonMember(MemberName = "extra")]
        protected string ExtraParameter { get; set; }

        [JsonMember(MemberName = "projection")]
        protected string Projection { get; set; }

        [JsonMember(MemberName = "wrapDateLine")]
        public WrapDatelineMode WrapDateline
        {
            get
            {
                return _wrapDateLine;
            }
            set
            {
                _wrapDateLine = value;
            }
        }

        public RectangleShape GetBoundingBox() {
            RectangleShape rectangleShape = null;

            foreach (BaseLayer layer in _layers) {
                layer.Open();
                if (rectangleShape == null) {
                    rectangleShape = layer.GetBoundingBox();
                }
                else {
                    rectangleShape.ExpandToInclude(layer.GetBoundingBox());
                }
                layer.Close();
            }

            return rectangleShape;
        }

        public void Draw(GdiPlusGeoCanvas canvas, object nativeImage, RectangleShape canvasExtent, GeographyUnit mapUnit) {
            //Validators.CheckMapUnitIsValid(mapUnit);
            DrawCore(canvas, nativeImage, canvasExtent, mapUnit);
            
        }

        protected virtual void DrawCore(GdiPlusGeoCanvas canvas, object nativeImage, RectangleShape canvasExtent, GeographyUnit mapUnit) {
            Collection<SimpleCandidate> labeledFeaturesInLayers = new Collection<SimpleCandidate>();
            foreach (BaseLayer layer in Layers) {
                lock (layer) {
                    canvas.BeginDrawing(nativeImage, canvasExtent, mapUnit);
                    layer.Open();
                    layer.Draw(canvas, labeledFeaturesInLayers);
                    layer.Close();
                    canvas.EndDrawing();
                }
            }
        }

        public void Redraw() {
            this.ExtraParameter = DateTime.Now.ToString("ddhhmmssms", CultureInfo.InvariantCulture);
        }

        //public void GenerateCacheImages(ZoomLevel zoomLevel, RectangleShape cacheExtent, GeographyUnit mapUnit) {
        //    //Validators.CheckParameterIsNotNull(zoomLevel, "zoomLevel");
        //    GenerateCacheImages(zoomLevelel, cacheExtent, mapUnit);
        //}

        //public void GenerateCacheImages(double scale, RectangleShape cacheExtent, GeographyUnit mapUnit) {
        //    //Validators.CheckValueIsGreaterThanZero(scale, "scale");
        //    //Validators.CheckParameterIsNotNull(cacheExtent, "cacheExtent");

        //    if (string.IsNullOrEmpty(ServerCache.CacheDirectory)) {
        //        throw new InvalidOperationException("The ServerCache is not enabled on the LayerOverlay,please set the CacheDirectory on the ServerCache.");
        //    }

        //    TileMatrix tileMatrix = new TileMatrix(scale, _tileWidth, _tileHeight, mapUnit);
        //    FileNativeImageTileCache tileCache = new FileNativeImageTileCache(_serverCache.CacheDirectory, _serverCache.CacheId, ConvertToTileImageFormat(_webImageFormat), tileMatrix);
        //    tileCache.JpegQuality = Convert.ToInt16(_jpegQuality);

        //    RowColumnRange rowColumnRange = tileCache.TileMatrix.GetIntersectingRowColumnRange(cacheExtent);

        //    for (long row = rowColumnRange.MinRowIndex; row <= rowColumnRange.MaxRowIndex; row++) {
        //        for (long column = rowColumnRange.MinColumnIndex; column <= rowColumnRange.MaxColumnIndex; column++) {
        //            RectangleShape tileExtent = tileCache.TileMatrix.GetCell(row, column).BoundingBox;

        //            Bitmap bitmap = new Bitmap(TileWidth, TileHeight);
        //            try {
        //                GdiPlusGeoCanvas geoCanvas = new GdiPlusGeoCanvas();
        //                Draw(geoCanvas, bitmap, tileExtent, mapUnit);

        //                NativeImageTile tile = new NativeImageTile(tileExtent, scale);
        //                tile.NativeImage = MapResourceHelper.ConvertImageFormat(bitmap, _webImageFormat.ToString(), _jpegQuality);
        //                tileCache.SaveTile(tile);
        //            }
        //            finally {
        //                bitmap.Dispose();
        //            }
        //        }
        //    }
        //}

        private static TileImageFormat ConvertToTileImageFormat(WebImageFormat webImageFormat) {
            if (webImageFormat == WebImageFormat.Jpeg) {
                return TileImageFormat.Jpeg;
            }
            else {
                return TileImageFormat.Png;
            }
        }
    }
}
