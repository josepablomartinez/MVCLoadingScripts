using System;
using Mapgenix.Shapes;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class CloudPopup : BasePopup
    {
        private string _contentHtml;

        public CloudPopup()
            : this(Guid.NewGuid().ToString())
        { }

        public CloudPopup(string id)
            : this(id, new PointShape())
        { }

      
        public CloudPopup(string id, PointShape position)
            : this(id, position, string.Empty)
        { }

        
        public CloudPopup(string id, PointShape position, string contentHtml)
            : this(id, position, contentHtml, 180, 150)
        { }

       
        public CloudPopup(string id, PointShape position, string contentHtml, int width, int height)
            : this(id, position, contentHtml, width, height, false)
        { }

       
        public CloudPopup(string id, PointShape position, string contentHtml, int width, int height, bool autoPan)
            : this(id, position, contentHtml, width, height, autoPan, false)
        { }

       
        public CloudPopup(string id, PointShape position, string contentHtml, int width, int height, bool autoPan, bool hasCloseButton)
            : base(id, position, width, height, autoPan, hasCloseButton)
        {
            this._contentHtml = contentHtml;
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

      
        [JsonMember(MemberName = "popupType")]
        protected override string PopupType
        {
            get { return "CloudPopup"; }
        }
    }
}
