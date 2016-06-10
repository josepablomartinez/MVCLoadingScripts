namespace Mapgenix.GSuite.Mvc
{
    public enum TransitionEffect
    {
        [JsonMember(MemberName = "")]
        None = 0,
        [JsonMember(MemberName = "resize")]
        Stretching = 1,
    }
}
