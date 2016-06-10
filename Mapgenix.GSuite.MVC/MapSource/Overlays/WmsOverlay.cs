using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class WmsOverlay : BaseOverlay
    {
        private Dictionary<string, string> _parameters;
        private int _tileWidth;
        private int _tileHeight;
        private TransitionEffect _transitionEffect;
        private TileType _tileType;
        private Collection<Uri> _serverUris;
        private WebImageFormat _webImageFormat;
        private WrapDatelineMode _wrapDateLine;

        public WmsOverlay()
            : this(Guid.NewGuid().ToString(), new Collection<Uri>(), TileType.MultipleTile)
        { }

        public WmsOverlay(string id)
            : this(id, new Collection<Uri>(), TileType.MultipleTile)
        { }

        public WmsOverlay(string id, Uri serverUri)
            : this(id, new Collection<Uri>() { serverUri }, TileType.MultipleTile)
        { }

        public WmsOverlay(string id, Uri serverUri, TileType tileType)
            : this(id, new Collection<Uri>() { serverUri }, tileType)
        { }

        public WmsOverlay(string id, IEnumerable<Uri> serverUris, TileType tileType)
            : base(id, true)
        {
            this._serverUris = new Collection<Uri>();
            foreach (Uri serverUri in serverUris)
            {
                this._serverUris.Add(serverUri);
            }

            this._tileType = tileType;

            this._tileWidth = 256;
            this._tileHeight = 256;
            this._transitionEffect = TransitionEffect.Stretching;
            this._webImageFormat = WebImageFormat.Png;
            this._parameters = new Dictionary<string, string>();
            this._wrapDateLine = WrapDatelineMode.None;
        }

        public void SetBaseEpsgProjection(string epsgProjection)
        {
            Validators.CheckOverlayEpsgProjectionGetAndSetValid(IsBaseOverlay);
            Projection = epsgProjection;
        }

        public string GetBaseEpsgProjection()
        {
            Validators.CheckOverlayEpsgProjectionGetAndSetValid(IsBaseOverlay);
            return Projection;
        }

        [JsonMember(MemberName = "parameters")]
        public Dictionary<string, string> Parameters
        {
            get
            {
                return _parameters;
            }
        }

       [JsonMember(MemberName = "SRS")]
        protected string Projection
        {
            get;
            set;
        }

        [JsonMember(MemberName = "format")]
        public WebImageFormat WebImageFormat
        {
            get
            {
                return _webImageFormat;
            }
            set
            {
                _webImageFormat = value;
            }
        }

        [JsonMember(MemberName = "w")]
        public int TileWidth
        {
            get
            {
                return _tileWidth;
            }
            set
            {
                _tileWidth = value;
            }
        }
       
        [JsonMember(MemberName = "h")]
        public int TileHeight
        {
            get
            {
                return _tileHeight;
            }
            set
            {
                _tileHeight = value;
            }
        }

        [JsonMember(MemberName = "transitionEffect")]
        public TransitionEffect TransitionEffect
        {
            get
            {
                return _transitionEffect;
            }
            set
            {
                _transitionEffect = value;
            }
        }

       
        [JsonMember(MemberName = "singleTile")]
        public TileType TileType
        {
            get
            {
                return _tileType;
            }
            set
            {
                _tileType = value;
            }
        }

      
        [JsonMember(MemberName = "uris")]
        public Collection<Uri> ServerUris
        {
            get
            {
                return _serverUris;
            }
        }

        [JsonMember(MemberName = "otype")]
        protected override string OverlayType
        {
            get
            {
                return "WMS";
            }
        }

    
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
    }
}
