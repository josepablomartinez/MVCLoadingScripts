using System;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class LoadingImageMapTool : BaseMapTool
    {
        private Uri _imageUri;
        private int _width;
        private int _height;

        public LoadingImageMapTool()
        {
        }

       
        [JsonMember(MemberName = "uri")]
        public Uri ImageUri
        {
            get
            {
                return _imageUri;
            }
            set
            {
                _imageUri = value;
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
                Validators.CheckValueIsGreaterOrEqualToZero(value, "Width");
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
                Validators.CheckValueIsGreaterOrEqualToZero(value, "Height");
                _height = value;
            }
        }
    }
}
