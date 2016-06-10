using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mapgenix.Shapes;
using Mapgenix.FeatureSource;
using Mapgenix.Layers;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class InMemoryMarkerOverlay : BaseMarkerOverlay
    {
        private MarkerZoomLevelSet _zoomLevelSet;
        private InMemoryFeatureLayer _inMemoryFeatureLayer;
        private int _filterGridSize;



        public InMemoryMarkerOverlay(string id)
            : base(id)
        {
            
            _zoomLevelSet = new MarkerZoomLevelSet();
            _inMemoryFeatureLayer = FeatureLayerFactory.CreateInMemoryFeatureLayer();

        }

        public Collection<FeatureSourceColumn> Columns
        {
            get
            {
                return _inMemoryFeatureLayer.Columns;
            }
        }

        [JsonMember(MemberName = "gridSize")]
        public int SuppressingGridSize
        {
            get { return _filterGridSize; }
            set { _filterGridSize = value; }
        }

        public FeatureSource.BaseFeatureSource FeatureSource
        {
            get
            {
                return _inMemoryFeatureLayer.FeatureSource;
            }
        }

        public MarkerZoomLevelSet ZoomLevelSet
        {
            get
            {
                return _zoomLevelSet;
            }
        }

        protected override Collection<Marker> GetMarkersCore(RectangleShape worldExtent, int currentZoomLevelId)
        {
            Collection<Marker> returnMarkers = new Collection<Marker>();

            MarkerZoomLevel zoomLevel = ZoomLevelSet.GetZoomLevelForDrawing(currentZoomLevelId);
            if (zoomLevel != null)
            {
                _inMemoryFeatureLayer.Open();
               

                returnMarkers = zoomLevel.GetMarkers(_inMemoryFeatureLayer.FeatureSource.GetFeaturesInsideBoundingBox(worldExtent, ReturningColumnsType.AllColumns));
                _inMemoryFeatureLayer.Close();

                returnMarkers = FilterMarkerWithClusterMarkerStyle(worldExtent, returnMarkers, zoomLevel);

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

        private Collection<string> GenerateReturnColumns(MarkerZoomLevel zoomLevel)
        {
            Collection<string> returnColumns = new Collection<string>();
            foreach (FeatureSourceColumn item in _inMemoryFeatureLayer.Columns)
            {
                string key = item.ColumnName;
                if (zoomLevel.CustomMarkerStyle != null)
                {
                    if (zoomLevel.CustomMarkerStyle.GetType() == typeof(ValueMarkerStyle))
                    {
                        ValueMarkerStyle markerStyle = zoomLevel.CustomMarkerStyle as ValueMarkerStyle;
                        if (!returnColumns.Contains(markerStyle.ColumnName))
                        {
                            returnColumns.Add(markerStyle.ColumnName);
                        }
                        foreach (MarkerValueItem valueItem in markerStyle.ValueItems)
                        {
                            PointMarkerStyle itemStyle = valueItem.CustomMarkerStyle as PointMarkerStyle;
                            if (itemStyle == null)
                            {
                                itemStyle = valueItem.DefaultMarkerStyle as PointMarkerStyle;
                            }
                            FetchFeatureColumnsUsed(itemStyle, returnColumns, key);
                        }
                    }
                    else if (zoomLevel.CustomMarkerStyle.GetType() == typeof(ClassBreakMarkerStyle))
                    {
                        ClassBreakMarkerStyle markerStyle = zoomLevel.CustomMarkerStyle as ClassBreakMarkerStyle;
                        if (!returnColumns.Contains(markerStyle.ColumnName))
                        {
                            returnColumns.Add(markerStyle.ColumnName);
                        }
                        foreach (MarkerClassBreak classBreakItem in markerStyle.ClassBreaks)
                        {
                            PointMarkerStyle itemStyle = classBreakItem.CustomMarkerStyle as PointMarkerStyle;
                            if (itemStyle == null)
                            {
                                itemStyle = classBreakItem.DefaultMarkerStyle as PointMarkerStyle;
                            }
                            FetchFeatureColumnsUsed(itemStyle, returnColumns, key);
                        }
                    }
                }
                else
                {
                    FetchFeatureColumnsUsed(zoomLevel.DefaultMarkerStyle, returnColumns, key);
                }
            }
            return returnColumns;
        }

        private static void FetchFeatureColumnsUsed(PointMarkerStyle defaultMarkerStyle, Collection<string> returnColumns, string columnName)
        {
            if (defaultMarkerStyle != null)
            {
                if (defaultMarkerStyle.Popup.ContentHtml.Contains("[#" + columnName + "#]") && !returnColumns.Contains(columnName))
                {
                    returnColumns.Add(columnName);
                }
            }
            if (defaultMarkerStyle.ContextMenu != null)
            {
                foreach (ContextMenuItem menuItem in defaultMarkerStyle.ContextMenu.MenuItems)
                {
                    if (menuItem.InnerHtml.Contains("[#" + columnName + "#]") && !returnColumns.Contains(columnName))
                    {
                        returnColumns.Add(columnName);
                    }
                }
            }
            if (defaultMarkerStyle.WebImage.Text.Contains("[#" + columnName + "#]") && !returnColumns.Contains(columnName))
            {
                returnColumns.Add(columnName);
            }
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
