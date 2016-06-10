namespace Mapgenix.GSuite.Mvc
{
    public enum WebImageFormat
    {
        [JsonMember(MemberName = "image/png")]
        Png = 0,
        [JsonMember(MemberName = "image/jpeg")]
        Jpeg = 1,
    }
}
