namespace Mapgenix.GSuite.Mvc
{
    public enum TrackMode
    {
        [JsonMember(MemberName = "None")]
        None = 0,
        [JsonMember(MemberName = "Point")]
        Point = 1,
        [JsonMember(MemberName = "Line")]
        Line = 2,
        [JsonMember(MemberName = "Polygon")]
        Polygon = 3,
        [JsonMember(MemberName = "Rectangle")]
        Rectangle = 4,
        [JsonMember(MemberName = "Square")]
        Square = 5,
        [JsonMember(MemberName = "Circle")]
        Circle = 6,
        [JsonMember(MemberName = "Ellipse")]
        Ellipse = 7,
        [JsonMember(MemberName = "Modify")]
        Edit = 8
    }
}
