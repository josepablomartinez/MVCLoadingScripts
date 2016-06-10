using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mapgenix.Shapes;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public abstract class BaseMarkerStyle
    {
        protected BaseMarkerStyle()
        { 
        }

        public abstract Collection<Marker> GetMarkers(IEnumerable<Feature> features);

        protected virtual GeoKeyedCollection<ContextMenu> GetContextMenusCore()
        {
            return new GeoKeyedCollection<ContextMenu>();
        }

        internal GeoKeyedCollection<ContextMenu> GetContextMenus()
        {
            return GetContextMenusCore();
        }
    }
}

