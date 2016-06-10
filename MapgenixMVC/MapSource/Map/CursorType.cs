namespace Mapgenix.GSuite.Mvc
{
    public enum CursorType
    {
        [JsonMember(MemberName = "default")]
        Default = 0,
        [JsonMember]
        Crosshair = 1,
        [JsonMember]
        Help = 2,
        [JsonMember]
        Move = 3,
        [JsonMember(MemberName = "e-resize")]
        EResize = 4,
        [JsonMember(MemberName = "n-resize")]
        NResize = 5,
        [JsonMember(MemberName = "ne-resize")]
        NeResize = 6,
        [JsonMember(MemberName = "nw-resize")]
        NwResize = 7,
        [JsonMember]
        Pointer = 8,
        [JsonMember]
        Progress = 9,
        [JsonMember]
        Text = 10,
        [JsonMember(MemberName = "s-resize")]
        SResize = 11,
        [JsonMember(MemberName = "se-resize")]
        SeResize = 12,
        [JsonMember(MemberName = "sw-resize")]
        SwResize = 13,
        [JsonMember(MemberName = "w-resize")]
        WResize = 14,
        [JsonMember]
        Wait = 15,
        [JsonMember]
        Custom = 16
    }
}
