using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mapgenix.Shapes;

namespace Mapgenix.GSuite.Mvc
{
    public class ClusterMarkerStyle : BaseMarkerStyle
    {
        private double _distance;
        private BaseMarkerStyle _markerStyle;
        private double _mapWidth;

        public ClusterMarkerStyle()
            : this(0d)
        {
        }

        internal ClusterMarkerStyle(double distance)
            : this(distance, 0, 0)
        {
        }

       
        public ClusterMarkerStyle(double distance, double mapWidth, double mapHeight)
        {
            this._distance = distance;
            this._mapWidth = mapWidth;
        }

        
        public double DistanceInPixel
        {
            get { return _distance; }
            set { _distance = value; }
        }

        public BaseMarkerStyle MarkerStyle
        {
            get { return _markerStyle; }
            set { _markerStyle = value; }
        }

       
        public double MapWidth
        {
            get { return _mapWidth; }
            set { _mapWidth = value; }
        }

        public double MapHeight
        {
            get { return _mapWidth; }
            set { _mapWidth = value; }
        }

       
        public override Collection<Marker> GetMarkers(IEnumerable<Feature> features)
        {
            Collection<Marker> markers = new Collection<Marker>();
            if (MarkerStyle != null)
            {
                markers = MarkerStyle.GetMarkers(features);
            }

            return markers;
        }
    }
}
