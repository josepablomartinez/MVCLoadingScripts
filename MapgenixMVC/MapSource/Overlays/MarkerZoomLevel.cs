using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mapgenix.Shapes;
using Mapgenix.Canvas;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class MarkerZoomLevel
    {
        private ApplyUntilZoomLevel _applyUntilZoomLevel;
        private PointMarkerStyle _defaultMarkerStyle;
        private BaseMarkerStyle _customMarkerStyle;

        public MarkerZoomLevel()
        {
        }

        internal bool HasStyleDefined
        {
            get 
            {
                if (_defaultMarkerStyle == null && _customMarkerStyle == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public ApplyUntilZoomLevel ApplyUntilZoomLevel
        {
            get { return _applyUntilZoomLevel; }
            set { _applyUntilZoomLevel = value; }
        }

        public PointMarkerStyle DefaultMarkerStyle
        {
            get
            {
                if (_defaultMarkerStyle == null)
                {
                    _defaultMarkerStyle = new PointMarkerStyle();
                }
                return _defaultMarkerStyle;
            }
        }

        public BaseMarkerStyle CustomMarkerStyle
        {
            get
            {
                return _customMarkerStyle;
            }
            set
            {
                _customMarkerStyle = value;
            }
        }

        public Collection<Marker> GetMarkers(IEnumerable<Feature> features)
        {
            Collection<Marker> returnMarkers = new Collection<Marker>();

            if (_customMarkerStyle != null)
            {
                returnMarkers = CustomMarkerStyle.GetMarkers(features);
            }
            else
            {
                returnMarkers = DefaultMarkerStyle.GetMarkers(features);
            }

            return returnMarkers;
        }

        protected virtual GeoKeyedCollection<ContextMenu> GetContextMenusCore()
        {
            GeoKeyedCollection<ContextMenu> contextMenus = new GeoKeyedCollection<ContextMenu>();
            
            if (CustomMarkerStyle != null)
            {
                foreach (ContextMenu contextMenu in CustomMarkerStyle.GetContextMenus())
                {
                    contextMenus.Add(contextMenu);
                }
            }
            else if (DefaultMarkerStyle.ContextMenu != null)
            {
                contextMenus.Add(DefaultMarkerStyle.ContextMenu);
            }

            return contextMenus;
        }

        internal GeoKeyedCollection<ContextMenu> GetContextMenus()
        {
            return GetContextMenusCore();
        }
    }
}
