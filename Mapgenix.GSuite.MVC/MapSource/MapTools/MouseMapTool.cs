using System;
//using Mapgenix.GSuite.Web.MapSource.MapTools;

namespace Mapgenix.GSuite.Mvc
{

    [Serializable]
    public class MouseMapTool : BaseMapTool {
        private bool _isMouseWheelDisabled;
        private TrackZoomMaskType _trackZoomMaskType;

        public MouseMapTool()
            : base(true) {
            _trackZoomMaskType = TrackZoomMaskType.Shift;
        }

        [JsonMember(MemberName = "wheelDisabled")]
        public bool IsMouseWheelDisabled {
            get { return _isMouseWheelDisabled; }
            set { _isMouseWheelDisabled = value; }
        }

        [JsonMember(MemberName = "zoomBoxKeyMask")]
        public TrackZoomMaskType TrackZoomMaskType {
            get { return _trackZoomMaskType; }
            set { _trackZoomMaskType = value; }
        }
    }
}
