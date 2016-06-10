using System;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class OpenStreetMapOverlay : BaseOverlay
    {
        private int _tileWidth;
        private int _tileHeight;

        public OpenStreetMapOverlay()
            : base(Guid.NewGuid().ToString())
        {
        }

        public OpenStreetMapOverlay(string id)
            : base(id, true)
        {
            TileHeight = 256;
            TileWidth = 256;
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

        [JsonMember(MemberName = "otype")]
        protected override string OverlayType
        {
            get { return "osm"; }
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
