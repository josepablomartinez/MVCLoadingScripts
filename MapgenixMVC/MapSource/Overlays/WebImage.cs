using System;
using System.Globalization;
using System.Web;
using Mapgenix.Canvas;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class WebImage : IJsonSerializable
    {
        private string _imageVirtualPath;
        private int _imageWidth;
        private int _imageHeight;
        private float _rotationAngle;
        private string _text;
        private GeoFont _fontStyle;
        private GeoColor _fontColor;
        private GeoColor _textBackgroundColor;
        private float _textOffsetX;
        private float _textOffsetY;
        private float _imageOffsetX;
        private float _imageOffsetY;

        
        public WebImage()
            : this(String.Empty, 0, 0)
        { }

       
        public WebImage(string imageVirtualPath)
            : this(imageVirtualPath, 0, 0)
        {
        }

        public WebImage(int imageWidth, int imageHeight)
            : this(String.Empty, imageWidth, imageHeight)
        {
        }

        
        public WebImage(int imageWidth, int imageHeight, float imageOffsetX, float imageOffsetY)
            : this(String.Empty, imageWidth, imageHeight, imageOffsetX, imageOffsetY)
        {
        }

        
        public WebImage(string imageVirtualPath, int imageWidth, int imageHeight)
            : this(imageVirtualPath, imageWidth, imageHeight, 0, 0)
        {
        }

       
        public WebImage(string imageVirtualPath, int imageWidth, int imageHeight, float imageOffsetX, float imageOffsetY)
        {
            //Validators.CheckValueIsGreaterOrEqualToZero(imageWidth, "ImageWidth");
            //Validators.CheckValueIsGreaterOrEqualToZero(imageHeight, "ImageHeight");

            this._imageVirtualPath = imageVirtualPath;
            this._imageWidth = imageWidth;
            this._imageHeight = imageHeight;
            _fontStyle = new GeoFont("verdana", 10, DrawingFontStyles.Regular);
            _fontColor = GeoColor.StandardColors.Black;
            _textBackgroundColor = GeoColor.StandardColors.Transparent;

            this._text = String.Empty;
            this._imageOffsetX = imageOffsetX;
            this._imageOffsetY = imageOffsetY;
        }

        public string ImageVirtualPath
        {
            get { return _imageVirtualPath; }
            set { _imageVirtualPath = value; }
        }

        [JsonMember(MemberName = "w")]
        public int ImageWidth
        {
            get
            {
                return _imageWidth;
            }
            set
            {
                //Validators.CheckValueIsGreaterOrEqualToZero(value, "imageWidth");
                _imageWidth = value;
            }
        }

        [JsonMember(MemberName = "h")]
        public int ImageHeight
        {
            get
            {
                return _imageHeight;
            }
            set
            {
                //Validators.CheckValueIsGreaterOrEqualToZero(value, "imageHeight");
                _imageHeight = value;
            }
        }

        public float RotationAngle
        {
            get { return _rotationAngle; }
            set { _rotationAngle = value; }
        }

        public GeoFont FontStyle
        {
            get { return _fontStyle; }
            set { _fontStyle = value; }
        }

        public GeoColor FontColor
        {
            get { return _fontColor; }
            set { _fontColor = value; }
        }

        public GeoColor TextBackgroundColor
        {
            get { return _textBackgroundColor; }
            set { _textBackgroundColor = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public float TextOffsetX
        {
            get { return _textOffsetX; }
            set { _textOffsetX = value; }
        }

        public float TextOffsetY
        {
            get { return _textOffsetY; }
            set { _textOffsetY = value; }
        }

        [JsonMember(MemberName = "url")]
        internal protected string IconImagePath
        {
            get
            {
                string returnPath = ImageVirtualPath;
                if (RotationAngle != 0f || !String.IsNullOrEmpty(_text))
                {
                    returnPath = string.Format(CultureInfo.InvariantCulture, "icon_GeoResource.axd?path={0}&angle={1}&width={2}&height={3}&text={4}&format=image/{5}&fontsize={6}&color={7}&x={8}&y={9}&fontstyle={10}&bgcolor={11}",
                        HttpUtility.UrlEncode(_imageVirtualPath), _rotationAngle, _imageWidth, _imageHeight,
                        HttpUtility.UrlEncode(_text),
                        HttpUtility.UrlEncode(_imageVirtualPath.Substring(_imageVirtualPath.LastIndexOf('.') + 1)),
                        (int)_fontStyle.Size,
                        GeoColor.ToHtml(_fontColor),
                        _textOffsetX,
                        _textOffsetY,
                        (int)_fontStyle.Style,
                        GeoColor.ToHtml(_textBackgroundColor)
                        );
                }
                return returnPath;
            }
        }

        [JsonMember(MemberName = "ox")]
        public float ImageOffsetX
        {
            get { return _imageOffsetX; }
            set { _imageOffsetX = value; }
        }

        [JsonMember(MemberName = "oy")]
        public float ImageOffsetY
        {
            get { return _imageOffsetY; }
            set { _imageOffsetY = value; }
        }

        public virtual WebImage CloneShallow()
        {
            return (WebImage)this.MemberwiseClone();
        }

        #region IJsonSerializable Members

        public string ToJson()
        {
            return JsonConverter.ConvertObjectToJson(this);
        }

        #endregion
    }
}
