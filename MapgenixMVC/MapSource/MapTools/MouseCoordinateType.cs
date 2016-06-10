namespace Mapgenix.GSuite.Mvc
{
    public enum MouseCoordinateType
    {
        [JsonMember(MemberName = "lonlat")]
        LongitudeLatitude = 0,

        [JsonMember(MemberName = "latlon")]
        LatitudeLongitude = 1,

        [JsonMember(MemberName = "dms")]
        DegreesMinutesSeconds = 2
    }
}
