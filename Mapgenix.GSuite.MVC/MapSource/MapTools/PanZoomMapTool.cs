//using Mapgenix.GSuite.Web;
using System;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class PanZoomMapTool : BaseMapTool
    {
        private bool _isGlobeButtonEnabled;

        public PanZoomMapTool()
        {
            _isGlobeButtonEnabled = true;
        }

        [JsonMember(MemberName = "zoomWorldIcon")]
        public bool IsGlobeButtonEnabled
        {
            get { return _isGlobeButtonEnabled; }
            set { _isGlobeButtonEnabled = value; }
        }
    }
}
