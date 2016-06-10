using System;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class LogoMapTool : BaseMapTool
    {
        private Uri _imageUri;

        public LogoMapTool()
            : base(true)
        { }

        
        [JsonMember(MemberName = "url")]
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
    }
}
