using System;
using System.Collections.Generic;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class ArcGisServerRestOverlay : BaseOverlay
    {
        private Dictionary<string, string> _parameters;
        private int _tileWidth;
        private int _tileHeight;
        private TransitionEffect _transitionEffect;
        private TileType _tileType;
        private Uri _serverUri;

        public ArcGisServerRestOverlay()
            : this(Guid.NewGuid().ToString(), null, TileType.MultipleTile)
        { }

       
        public ArcGisServerRestOverlay(string id)
            : this(id, null, TileType.MultipleTile)
        { }

       
        public ArcGisServerRestOverlay(string id, Uri serverUri)
            : this(id, serverUri, TileType.MultipleTile)
        { }

        
        public ArcGisServerRestOverlay(string id, Uri serverUri, TileType tileType)
            : base(id, true)
        {
            this._serverUri =  serverUri;
            this._tileType = tileType;
            this._tileWidth = 256;
            this._tileHeight = 256;
            this._transitionEffect = TransitionEffect.Stretching;
            this._parameters = new Dictionary<string, string>();
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
        public string Projection
        {
            get;
            set;
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

        [JsonMember(MemberName = "uri")]
        public Uri ServerUri
        {
            get
            {
                return _serverUri;
            }
            set
            {
                 _serverUri=value;
            }
        }

       
        [JsonMember(MemberName = "otype")]
        protected override string OverlayType
        {
            get
            {
                return "ArcGIS";
            }
        }

       
        [JsonMember(MemberName = "tick")]
        protected override bool HasTickEvent
        {
            get
            {
                return false;
            }
        }
    }
}
