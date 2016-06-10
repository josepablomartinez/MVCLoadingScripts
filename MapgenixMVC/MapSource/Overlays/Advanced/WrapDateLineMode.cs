namespace Mapgenix.GSuite.Mvc
{
    public enum WrapDatelineMode
    {
        [JsonMember(MemberName = "0")]
        None = 0,
        [JsonMember(MemberName = "1")]
        WrapDateline = 1
    }
}
