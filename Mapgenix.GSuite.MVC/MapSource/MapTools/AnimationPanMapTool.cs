using System;

namespace Mapgenix.GSuite.Mvc
{

    [Serializable]
    public class AnimationPanMapTool : BaseMapTool {
        private string _onClientClick;

        public AnimationPanMapTool()
            : base(false) {
            Delay = 300;
            _onClientClick = String.Empty;
        }

        [JsonMember(MemberName = "delay")]
        public int Delay { get; set; }

        
        [JsonMember(MemberName = "onClientClick")]
        public string OnClientClick {
            get {
                return _onClientClick;
            }
            set {
                _onClientClick = value;
            }
        }
    }
}
