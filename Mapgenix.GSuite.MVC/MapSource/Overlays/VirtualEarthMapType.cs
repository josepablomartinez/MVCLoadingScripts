namespace Mapgenix.GSuite.Mvc
{
    public enum VirtualEarthMapType
    {
        [JsonMember(MemberName = "VEMapStyle.Road")]
        Road = 0,
        [JsonMember(MemberName = "VEMapStyle.Shaded")]
        Shaded = 1,
        [JsonMember(MemberName = "VEMapStyle.Aerial")]
        Aerial = 2,
        [JsonMember(MemberName = "VEMapStyle.Hybrid")]
        Hybrid = 3,
        [JsonMember(MemberName = "VEMapStyle.Oblique")]
        Oblique = 4,
        [JsonMember(MemberName = "VEMapStyle.Birdseye")]
        Birdseye = 5,
        [JsonMember(MemberName = "VEMapStyle.BirdseyeHybrid")]
        BirdseyeHybrid = 6
    }
}
