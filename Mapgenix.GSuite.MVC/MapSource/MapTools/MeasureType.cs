namespace Mapgenix.GSuite.Mvc
{

    public enum MeasureType {

        [JsonMember(MemberName = "None")]
        None = 0,

        [JsonMember(MemberName = "PathMeasure")]
        Line = 1,

        [JsonMember(MemberName = "PolygonMeasure")]
        Area = 2,
    }
}
