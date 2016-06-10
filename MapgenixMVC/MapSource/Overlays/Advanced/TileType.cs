namespace Mapgenix.GSuite.Mvc
{
    public enum TileType
    {
        [JsonMember(MemberName = "1")]
        SingleTile = 0,
       
        [JsonMember(MemberName = "0")]
        MultipleTile = 1,
    }
}
