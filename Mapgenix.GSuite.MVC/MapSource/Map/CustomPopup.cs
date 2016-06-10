using System;
using Mapgenix.Shapes;
using Mapgenix.Canvas;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class CustomPopup : BasePopup
    {
        private GeoColor _backgroundColor;
        private GeoColor _borderColor;
        private int _borderWidth;
        private string _contentHtml;
        private string _backgroundImageVirtualPath;

        public CustomPopup()
            : this(Guid.NewGuid().ToString())
        { }

       
        public CustomPopup(string id)
            : this(id, new PointShape())
        { }

       
        public CustomPopup(string id, PointShape position)
            : this(id, position, string.Empty)
        { }

       
        public CustomPopup(string id, PointShape position, string contentHtml)
            : this(id, position, contentHtml, 180, 150)
        { }

       
        public CustomPopup(string id, PointShape position, string contentHtml, int width, int height)
            : this(id, position, contentHtml, width, height, false)
        { }

       
        public CustomPopup(string id, PointShape position, string contentHtml, int width, int height, bool autoPan)
            : this(id, position, contentHtml, width, height, autoPan, false)
        { }

       
        public CustomPopup(string id, PointShape position, string contentHtml, int width, int height, bool autoPan, bool hasCloseButton)
            : this(id, position, contentHtml, width, height, autoPan, hasCloseButton, 0)
        { }

        
        public CustomPopup(string id, PointShape position, string contentHtml, int width, int height, bool autoPan, bool hasCloseButton, int borderWidth)
            : base(id, position, width, height, autoPan, hasCloseButton)
        {
            Validators.CheckValueIsGreaterOrEqualToZero(borderWidth, "borderWidth");

            this._contentHtml = contentHtml;
            this._borderWidth = borderWidth;
            this._backgroundColor = GeoColor.StandardColors.White;
            this._borderColor = GeoColor.StandardColors.Black;
        }

        #region Properties

       
        [JsonMember(MemberName = "bgcolor")]
        public GeoColor BackgroundColor
        {
            get
            {
                return _backgroundColor;
            }
            set
            {
                Validators.CheckParameterIsNotNull(value, "BackgroundColor");
                _backgroundColor = value;
            }
        }

        
        [JsonMember(MemberName = "bdcolor")]
        public GeoColor BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; }
        }

       
        [JsonMember(MemberName = "bw")]
        public int BorderWidth
        {
            get
            {
                return _borderWidth;
            }
            set
            {
                _borderWidth = value;
            }
        }

       
        [JsonMember(MemberName = "html")]
        public string ContentHtml
        {
            get
            {
                return _contentHtml;
            }
            set
            {
                _contentHtml = value;
            }
        }

       
        [JsonMember(MemberName = "backImg")]
        public string BackgroundImageVirtualPath
        {
            get { return _backgroundImageVirtualPath; }
            set { _backgroundImageVirtualPath = value; }
        }

        [JsonMember(MemberName = "popupType")]
        protected override string PopupType
        {
            get { return "NormalPopup"; }
        }
        #endregion
    }
}
