using System;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class MouseCoordinateMapTool : BaseMapTool
    {
        private MouseCoordinateType _mousePositionType;
        public MouseCoordinateMapTool()
        {
            _mousePositionType = MouseCoordinateType.LongitudeLatitude;
        }

        [JsonMember(MemberName = "showType")]
        public MouseCoordinateType MouseCoordinateType
        {
            get
            {
                return _mousePositionType;
            }
            set
            {
                _mousePositionType = value;
            }
        }
    }
}
