using System;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class PanZoomBarMapTool : BaseMapTool
    {
        private bool _isGlobeButtonEnabled;

        public PanZoomBarMapTool()
            : base(true)
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
