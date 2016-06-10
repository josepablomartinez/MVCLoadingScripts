namespace Mapgenix.GSuite.Mvc
{

    public enum MeasureUnitType {

        [JsonMember(MemberName = "metric")]
        Metric = 0,
       
        [JsonMember(MemberName = "geographic")]
        Geographic = 1,

        [JsonMember(MemberName = "english")]
        English = 2,
    }
}
