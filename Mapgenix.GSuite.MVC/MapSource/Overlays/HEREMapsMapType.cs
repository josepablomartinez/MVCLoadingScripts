namespace Mapgenix.GSuite.Mvc
{
    public enum HEREMapsMapType
    {
        [JsonMember(MemberName = "YAHOO_MAP_REG")]
        Regular = 0,
        [JsonMember(MemberName = "YAHOO_MAP_SAT")]
        Satellite = 1,
        [JsonMember(MemberName = "YAHOO_MAP_HYB")]
        Hybrid = 2
    }
}
