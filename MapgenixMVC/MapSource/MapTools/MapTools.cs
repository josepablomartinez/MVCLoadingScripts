using System;
//using Mapgenix.GSuite.Web.MapSource.MapTools;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class MapTools : IJsonSerializable
    {
        private OverlaySwitcherMapTool _overlaySwitcher;
        private MouseMapTool _mouseDefaults;
        private MouseCoordinateMapTool _mousePosition;
        private MiniMapMapTool _miniMap;
        private PanZoomMapTool _panZoom;
        private PanZoomBarMapTool _panZoomBar;
        private ScaleLineMapTool _scaleLine;
        private LogoMapTool _logo;
        private KeyboardMapTool _keyboardDefaults;
        private LoadingImageMapTool _loadingImage;
        private AnimationPanMapTool _animationPanMapTool;
        private MeasureMapTool _measureMapTool;
       
        public MapTools()
        {
        }

        [JsonMember(MemberName = "Measure")]
        public MeasureMapTool MeasureMapTool
        {
            get
            {
                if (_measureMapTool == null)
                {
                    _measureMapTool = new MeasureMapTool();
                }
                return _measureMapTool;
            }
        }

       
        [JsonMember(MemberName = "AnimationPan")]
        public AnimationPanMapTool AnimationPanMapTool
        {
            get
            {
                if (_animationPanMapTool == null)
                {
                    _animationPanMapTool = new AnimationPanMapTool();
                }
                return _animationPanMapTool;
            }
        }

        [JsonMember(MemberName = "LayerSwitcher")]
        public OverlaySwitcherMapTool OverlaySwitcher
        {
            get
            {
                if (_overlaySwitcher == null)
                {
                    _overlaySwitcher = new OverlaySwitcherMapTool();
                }
                return _overlaySwitcher;
            }
        }

        [JsonMember(MemberName = "Navigation")]
        public MouseMapTool MouseMapTool
        {
            get
            {
                if (_mouseDefaults == null)
                {
                    _mouseDefaults = new MouseMapTool();
                }
                return _mouseDefaults;
            }
        }

        [JsonMember(MemberName = "MousePosition")]
        public MouseCoordinateMapTool MouseCoordinate
        {
            get
            {
                if (_mousePosition == null)
                {
                    _mousePosition = new MouseCoordinateMapTool();
                }
                return _mousePosition;
            }
        }

        [JsonMember(MemberName = "OverviewMap")]
        public MiniMapMapTool MiniMap
        {
            get
            {
                if (_miniMap == null)
                {
                    _miniMap = new MiniMapMapTool();
                }
                return _miniMap;
            }
        }

        [JsonMember(MemberName = "PanZoom")]
        public PanZoomMapTool PanZoom
        {
            get
            {
                if (_panZoom == null)
                {
                    _panZoom = new PanZoomMapTool();
                }
                return _panZoom;
            }
        }

        [JsonMember(MemberName = "PanZoomBar")]
        public PanZoomBarMapTool PanZoomBar
        {
            get
            {
                if (_panZoomBar == null)
                {
                    _panZoomBar = new PanZoomBarMapTool();
                }
                return _panZoomBar;
            }
        }

        [JsonMember(MemberName = "ScaleLine")]
        public ScaleLineMapTool ScaleLine
        {
            get
            {
                if (_scaleLine == null)
                {
                    _scaleLine = new ScaleLineMapTool();
                }
                return _scaleLine;
            }
        }

        [JsonMember(MemberName = "Logo")]
        public LogoMapTool Logo
        {
            get
            {
                if (_logo == null)
                {
                    _logo = new LogoMapTool();
                }
                return _logo;
            }
        }

        [JsonMember(MemberName = "KeyboardDefaults")]
        public KeyboardMapTool KeyboardMapTool
        {
            get
            {
                if (_keyboardDefaults == null)
                {
                    _keyboardDefaults = new KeyboardMapTool();
                }
                return _keyboardDefaults;
            }
        }

        [JsonMember(MemberName = "LoadingImage")]
        public LoadingImageMapTool LoadingImage
        {
            get
            {
                if (_loadingImage == null)
                {
                    _loadingImage = new LoadingImageMapTool();
                }
                return _loadingImage;
            }
        }

        #region IJsonSerializable Members

        
        public string ToJson()
        {
            return JsonConverter.ConvertObjectToJson(this);
        }

        #endregion
    }
}
