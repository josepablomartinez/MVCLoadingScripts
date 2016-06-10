namespace Mapgenix.GSuite.Mvc
{

    public enum MarkerDragMode
    {
        [JsonMember(MemberName = "none")]
        None = 0,
       
        [JsonMember(MemberName = "drag")]
        Drag = 1
    }
}
