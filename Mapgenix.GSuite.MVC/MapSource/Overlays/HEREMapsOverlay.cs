using System;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class HEREMapsOverlay : BaseOverlay
    {
        private Uri _javaScriptLibraryUri;
        private HEREMapsMapType _yahooMapType;

        public HEREMapsOverlay()
            : this(Guid.NewGuid().ToString())
        { }

        
        public HEREMapsOverlay(string id)
            : this(id, HEREMapsMapType.Regular)
        { }

        
        public HEREMapsOverlay(string id, HEREMapsMapType yahooMapType)
            : base(id, true)
        {
            this._yahooMapType = yahooMapType;
            this._javaScriptLibraryUri = new Uri("http://api.maps.yahoo.com/ajaxymap?v=3.8&appid=YD-eQRpTl0_JX2E95l_xAFs5UwZUlNQhhn7lj1H");
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
        public HEREMapsMapType HEREMapsMapType
        {
            get
            {
                return _yahooMapType;
            }
            set
            {
                _yahooMapType = value;
            }
        }

        [JsonMember(MemberName = "otype")]
        protected override string OverlayType
        {
            get
            {
                return "HEREMaps";
            }
        }

        protected override void RegisterJavaScriptLibraryCore(System.Web.UI.Page page)
        {
            MapResourceHelper.RegisterJavaScriptLibrary(page, "HEREMaps", JavaScriptLibraryUri);
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
