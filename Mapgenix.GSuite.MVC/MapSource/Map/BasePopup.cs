using System;
using Mapgenix.Shapes;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public abstract class BasePopup : IRequireId, IJsonSerializable
    {
        private string _id;
        private PointShape _position;
        private bool _autoPan;
        private bool _hasCloseButton;
        private bool _autoSize;
        private int _width;
        private int _height;
        private bool _isVisible;
        private float _opacity;
        private int _offsetXInPixels;
        private int _offsetYInPixels;

        protected BasePopup()
            : this(Guid.NewGuid().ToString())
        { }

        protected BasePopup(string id)
            : this(id, new PointShape())
        { }

       
        protected BasePopup(string id, PointShape position)
            : this(id, position, 180, 150, false)
        { }

       
        protected BasePopup(string id, PointShape position, int width, int height)
            : this(id, position, width, height, false)
        { }

        
        protected BasePopup(string id, PointShape position, int width, int height, bool autoPan)
            : this(id, position, width, height, autoPan, false)
        { }

      
        protected BasePopup(string id, PointShape position, int width, int height, bool autoPan, bool hasCloseButton)
        {
            Validators.CheckValueIsGreaterOrEqualToZero(width, "width");
            Validators.CheckValueIsGreaterOrEqualToZero(height, "height");

            this._id = id;
            this._position = position;
            this._autoPan = autoPan;
            this._hasCloseButton = hasCloseButton;
            this._width = width;
            this._height = height;
            this._isVisible = true;
            this._opacity = 1;
        }

        [JsonMember(MemberName = "id")]
        public string Id
        {
            get { return this._id; }
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
                Validators.CheckParameterIsNotNull(value, "Position");
                _position = value;
            }
        }

        [JsonMember(MemberName = "w")]
        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
            }
        }

        [JsonMember(MemberName = "h")]
        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
            }
        }

        [JsonMember(MemberName = "visibility")]
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                _isVisible = value;
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

       
        [JsonMember(MemberName = "autosize")]
        public bool AutoSize
        {
            get
            {
                return _autoSize;
            }
            set
            {
                _autoSize = value;
            }
        }

        [JsonMember(MemberName = "autopan")]
        public bool AutoPan
        {
            get
            {
                return _autoPan;
            }
            set
            {
                _autoPan = value;
            }
        }

        [JsonMember(MemberName = "closable")]
        public bool HasCloseButton
        {
            get
            {
                return _hasCloseButton;
            }
            set
            {
                _hasCloseButton = value;
            }
        }

        [JsonMember(MemberName = "ox")]
        public int OffsetXInPixels
        {
            get
            {
                return _offsetXInPixels;
            }
            set
            {
                _offsetXInPixels = value;
            }
        }

       
        [JsonMember(MemberName = "oy")]
        public int OffsetYInPixels
        {
            get
            {
                return _offsetYInPixels;
            }
            set
            {
                _offsetYInPixels = value;
            }
        }

       
        protected abstract string PopupType
        {
            get;
        }

        public virtual BasePopup CloneShallow()
        {
            return (BasePopup)this.MemberwiseClone();
        }

        #region IJsonSerializable Members

        public virtual string ToJson()
        {
            return JsonConverter.ConvertObjectToJson(this);
        }

        #endregion
    }
}
