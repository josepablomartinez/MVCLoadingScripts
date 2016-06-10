using System;
using Mapgenix.Canvas;

namespace Mapgenix.GSuite.Mvc
{

    [Serializable]
    public class OverlaySwitcherMapTool : BaseMapTool
    {
        private string _onClientBaseOverlayChanged;
        private string _baseOverlayTitle;
        private string _switchOverlayTitle;
        private GeoColor _backgroundColor;

        public OverlaySwitcherMapTool()
        {
            _onClientBaseOverlayChanged = String.Empty;
            _baseOverlayTitle = string.Empty;
            _switchOverlayTitle = string.Empty;
            _backgroundColor = GeoColor.FromHtml("#dddddd");
        }

       
        [JsonMember(MemberName = "onLayerChanged")]
        public string OnClientBaseOverlayChanged
        {
            get
            {
                return _onClientBaseOverlayChanged;
            }
            set
            {
                _onClientBaseOverlayChanged = value;
            }
        }

        [JsonMember(MemberName = "activeColor")]
        public GeoColor BackgroundColor
        {
            get
            {
                return _backgroundColor;
            }
            set
            {
                _backgroundColor = value;
            }
        }

        [JsonMember(MemberName = "baseTitle")]
        public string BaseOverlayTitle
        {
            get
            {
                return _baseOverlayTitle;
            }
            set
            {
                _baseOverlayTitle = value;
            }
        }

        [JsonMember(MemberName = "switchTitle")]
        public string DynamicOverlayTitle
        {
            get
            {
                return _switchOverlayTitle;
            }
            set
            {
                _switchOverlayTitle = value;
            }
        }
    }
}
