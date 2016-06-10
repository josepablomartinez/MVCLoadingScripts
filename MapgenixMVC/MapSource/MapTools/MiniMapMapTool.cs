using System;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class MiniMapMapTool : BaseMapTool
    {
        public MiniMapMapTool()
        {
            MaximizeRatio = 24;
            MinimizeRatio = 8;
        }
        
        [JsonMember(MemberName = "maxRatio")]
        public int MaximizeRatio { get; set; }

        [JsonMember(MemberName = "minRatio")]
        public int MinimizeRatio { get; set; }
    }
}
