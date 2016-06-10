using System;
using Mapgenix.Shapes;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class Marker : IRequireId, IJsonSerializable
    {
        private ContextMenu _contextMenu;
        private CustomPopup _popup;
        private int _popupDelay;
        private PointShape _position;
        private WebImage _webImage;
        private float _opacity;
        private string _id;
        private bool _isVisible;

        private const string DefaultIconPath = "/theme/default/img/marker_blue.gif";

        public Marker()
            : this(new PointShape(), new WebImage(DefaultIconPath))
        {
        }

        public Marker(WebImage webImage)
            : this(new PointShape(), webImage)
        {
        }

        public Marker(double x, double y)
            : this(new PointShape(x, y), new WebImage(DefaultIconPath))
        {
        }

        public Marker(double x, double y, WebImage webImage)
            : this(new PointShape(x, y), webImage)
        {
        }

        public Marker(PointShape position)
            : this(position, new WebImage(DefaultIconPath))
        {
        }

        public Marker(PointShape position, WebImage webImage)
        {
            this.Id = Guid.NewGuid().ToString();
            this.WebImage = webImage;
            this.Position = position;
            this._popup = new CustomPopup();
            this._popup.IsVisible = false;
            this._popupDelay = 500;
            this._isVisible = true;
            this._opacity = 100;
        }

        [JsonMember(MemberName = "contextMenu")]
        public ContextMenu ContextMenu
        {
            get
            {
                return _contextMenu;
            }
            set
            {
                _contextMenu = value;
            }
        }

        [JsonMember(MemberName = "popup")]
        public CustomPopup Popup
        {
            get
            {
                return _popup;
            }
            set
            {
                _popup = value;
            }
        }

        [JsonMember(MemberName = "popupdelay")]
        public int PopupDelay
        {
            get
            {
                return _popupDelay;
            }
            set
            {
                _popupDelay = value;
            }
        }

        [JsonMember(MemberName = "lonlat")]
        public PointShape Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
            }
        }

        [JsonMember(MemberName = "img")]
        public WebImage WebImage
        {
            get
            {
                return _webImage;
            }
            set
            {
                _webImage = value;
            }
        }

        [JsonMember(MemberName = "opacity")]
        public float Opacity
        {
            get
            {
                return _opacity;
            }
            set
            {
                _opacity = value;
            }
        }

        [JsonMember(MemberName = "id")]
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        [JsonMember(MemberName = "visible")]
        public bool IsVisible
        {
            get { return _isVisible; }
            set { _isVisible = value; }
        }

        #region IJsonSerializable Members

        public string ToJson()
        {
            return JsonConverter.ConvertObjectToJson(this);
        }

        #endregion
    }
}
