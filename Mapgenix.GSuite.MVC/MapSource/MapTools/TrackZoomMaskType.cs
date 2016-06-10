//using Mapgenix.GSuite.Mvc;

namespace Mapgenix.GSuite.Mvc
{

    public enum TrackZoomMaskType {

        [JsonMember(MemberName="0")]
        None,

        [JsonMember(MemberName = "1")]
        Shift,

        [JsonMember(MemberName = "2")]
        Ctrl,

        [JsonMember(MemberName = "4")]
        Alt,
    }
}
