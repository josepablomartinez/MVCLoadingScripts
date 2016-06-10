using System;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class VirtualEarthOverlay : BaseOverlay
    {
        private Uri _javaScriptLibraryUri;
        private VirtualEarthMapType _virtualEarthMapType;

       
        public VirtualEarthOverlay()
            : this(Guid.NewGuid().ToString())
        { }

      
        public VirtualEarthOverlay(string id)
            : this(id, VirtualEarthMapType.Road)
        { }

      
        public VirtualEarthOverlay(string id, VirtualEarthMapType virtualEarthMapType)
            : base(id, true)
        {
            this._virtualEarthMapType = virtualEarthMapType;
            this._javaScriptLibraryUri = new Uri("http://dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=6.2");
        }

        
        public Uri JavaScriptLibraryUri
        {
            get
            {
                return _javaScriptLibraryUri;
            }
            set
            {
                _javaScriptLibraryUri = value;
            }
        }

      
        [JsonMember(MemberName = "type")]
        public VirtualEarthMapType VirtualEarthMapType
        {
            get
            {
                return _virtualEarthMapType;
            }
            set
            {
                _virtualEarthMapType = value;
            }
        }

        [JsonMember(MemberName = "otype")]
        protected override string OverlayType
        {
            get
            {
                return "VirtualEarth";
            }
        }

        //protected override void RegisterJavaScriptLibraryCore(System.Web.UI.Page page)
        //{
        //    MapResourceHelper.RegisterJavaScriptLibrary(page, "VirtualEarth", JavaScriptLibraryUri);
        //}

      
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
