using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mapgenix.Shapes;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class FeatureSourceMarkerOverlay : BaseMarkerOverlay
    {
        private FeatureSource.BaseFeatureSource _featureSource;
        private MarkerZoomLevelSet _zoomLevelSet;
        private int _filterGridSize;

        public FeatureSourceMarkerOverlay()
            : this(new Guid().ToString(), null)
        { }

       
        public FeatureSourceMarkerOverlay(string id)
            : this(id, null)
        { }

     
        public FeatureSourceMarkerOverlay(string id, FeatureSource.BaseFeatureSource featureSource)
            : base(id)
        {
            this._featureSource = featureSource;
            this._zoomLevelSet = new MarkerZoomLevelSet();
        }

        
        public FeatureSource.BaseFeatureSource FeatureSource
        {
            get
            {
                return _featureSource;
            }
            set
            {
                _featureSource = value;
            }
        }

       
        public MarkerZoomLevelSet ZoomLevelSet
        {
            get
            {
                return _zoomLevelSet;
            }
        }

       
        [JsonMember(MemberName = "gridSize")]
        public int SuppressingGridSize
        {
            get { return _filterGridSize; }
            set { _filterGridSize = value; }
        }

        protected override Collection<Marker> GetMarkersCore(RectangleShape worldExtent, int currentZoomLevelId)
        {
            Collection<Marker> returnMarkers = new Collection<Marker>();

            if (_featureSource != null)
            {
                MarkerZoomLevel zoomLevel = ZoomLevelSet.GetZoomLevelForDrawing(currentZoomLevelId);
                if (zoomLevel != null)
                {
                    _featureSource.Open();
                    Collection<Feature> features = _featureSource.GetFeaturesInsideBoundingBox(worldExtent, ReturningColumnsType.AllColumns);
                    _featureSource.Close();

                    returnMarkers = zoomLevel.GetMarkers(features);

                    returnMarkers = FilterMarkerWithClusterMarkerStyle(worldExtent, returnMarkers, zoomLevel);
                }
            }

            return returnMarkers;
        }

        private static Collection<Marker> FilterMarkerWithClusterMarkerStyle(RectangleShape worldExtent, Collection<Marker> returnMarkers, MarkerZoomLevel zoomLevel)
        {
            Collection<Marker> shouldDisplay = new Collection<Marker>();
            bool isClustered = false;
            if (zoomLevel.CustomMarkerStyle != null)
            {
                if (typeof(ClusterMarkerStyle).IsInstanceOfType(zoomLevel.CustomMarkerStyle))
                {
                    isClustered = true;
                    ClusterMarkerStyle clusterMarkerStyle = zoomLevel.CustomMarkerStyle as ClusterMarkerStyle;

                    if (clusterMarkerStyle.DistanceInPixel == 0 || clusterMarkerStyle.MapWidth == 0 || clusterMarkerStyle.MapHeight == 0)
                    {
                        return returnMarkers;
                    }

                    MarkerManager markerManager = new MarkerManager(worldExtent, clusterMarkerStyle.MapWidth, clusterMarkerStyle.MapHeight, clusterMarkerStyle.DistanceInPixel, returnMarkers);
                    List<int> filteredMarkerIndexes = markerManager.Cluster();

                    if (returnMarkers.Count != 0)
                    {
                        foreach (int index in filteredMarkerIndexes)
                        {
                            shouldDisplay.Add(returnMarkers[index]);
                        }
                    }
                }
            }
            if (!isClustered)
            {
                shouldDisplay = returnMarkers;
            }

            return shouldDisplay;
        }

        internal void DetachContextMenuClickEvents()
        {
            this._zoomLevelSet.DetachContextMenuClickEvents();
        }

        internal void RewireContextMenuClickEvents(object target)
        {
            this._zoomLevelSet.RewireContextMenuClickEvents(target);
        }
    }
}
