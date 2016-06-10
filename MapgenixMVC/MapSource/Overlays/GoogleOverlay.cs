using System;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class GoogleOverlay : BaseOverlay
    {
        private Uri _javaScriptLibraryUri;
        private GoogleMapType _googleMapType;

        public GoogleOverlay()
            : this(Guid.NewGuid().ToString(), GoogleMapType.Normal)
        { }

       
        public GoogleOverlay(string id)
            : this(id, GoogleMapType.Normal)
        { }

       
        public GoogleOverlay(string id, GoogleMapType googleMapType)
            : base(id, true)
        {
            //Validators.CheckGoogleMapTypeIsValid(googleMapType);

            this._googleMapType = googleMapType;
            this._javaScriptLibraryUri = new Uri("http://maps.google.com/maps?file=api&v=2&key=ABQIAAAAoxK_HcqphMsnUQHEwLwHlRSavkNJi0NVTgm4UDidoiIU5dUJpRQW88FufPCp0aTPraxZgZFAIUHn3Q");
        }

        
        public Uri JavaScriptLibraryUri
        {
            get
            {
                return _javaScriptLibraryUri;
            }
            set
            {
                //Validators.CheckParameterIsNotNull(value, "JavaScriptLibraryUri");
                _javaScriptLibraryUri = value;
            }
        }

       
        [JsonMember(MemberName = "type")]
        public GoogleMapType GoogleMapType
        {
            get
            {
                return _googleMapType;
            }
            set
            {
                //Validators.CheckGoogleMapTypeIsValid(value);
                _googleMapType = value;
            }
        }

       
        [JsonMember(MemberName = "otype")]
        protected override string OverlayType
        {
            get
            {
                return "Google";
            }
        }

       
        //protected override void RegisterJavaScriptLibraryCore(System.Web.UI.Page page)
        //{
        //    MapResourceHelper.RegisterJavaScriptLibrary(page, "Google", JavaScriptLibraryUri);
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
