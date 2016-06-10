namespace Mapgenix.GSuite.Mvc
{
    public enum GoogleMapType
    {
        [JsonMember(MemberName = "G_NORMAL_MAP")]
        Normal = 0,
        [JsonMember(MemberName = "G_SATELLITE_MAP")]
        Satellite = 1,
        [JsonMember(MemberName = "G_HYBRID_MAP")]
        Hybrid = 2,
        [JsonMember(MemberName = "G_PHYSICAL_MAP")]
        Physical = 3
    }
}
