using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.SessionState;
using Mapgenix.Shapes;

namespace Mapgenix.GSuite.Mvc
{
    internal class MarkerResource : IHttpAsyncHandler, IReadOnlySessionState
    {
        private string _extentString;
        private RectangleShape _extent;
        private int _currentZoomLevelId;
        private string _pageName;
        private string _clientId;
        private double _currentScale;
        private string[] _markerOverlayIds;
        private int _gridSize;
        private GeographyUnit _mapUnit;
        private Collection<BaseMarkerOverlay> _markerOverlays;

        public MarkerResource()
        {
            _markerOverlays = new Collection<BaseMarkerOverlay>();
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            ProcessRequestDelegate processRequestMethod = new ProcessRequestDelegate(ProcessRequest);
            return processRequestMethod.BeginInvoke(context, cb, extraData);
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            /* */
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            SetEnvironmentFromQueryString(context);
            SetEnvironmentFromSession(context);
            GenerateAndOutputMarkersJson(context);
        }

        //private void SetEnvironmentFromQueryString(HttpContext context)
        //{
        //    _extentString = context.Request.QueryString["extent"];
        //    _extent = RectangleConverter.ConvertStringToRectangle(_extentString);
        //    _currentZoomLevelId = Int32.Parse(context.Request.QueryString["level"], CultureInfo.InvariantCulture);
        //    _currentScale = double.Parse(context.Request.QueryString["scale"], CultureInfo.InvariantCulture);
        //    _gridSize = Int32.Parse(context.Request.QueryString["gridSize"], CultureInfo.InvariantCulture);

        //    _pageName = context.Server.UrlDecode(context.Request.QueryString["PageName"]);
        //    _clientId = context.Server.UrlDecode(context.Request.QueryString["ClientId"]);
        //    _markerOverlayIds = context.Server.UrlDecode(context.Request.QueryString["OverlayIds"]).Split(',');
        //}

        private void SetEnvironmentFromSession(HttpContext context)
        {
            _mapUnit = (GeographyUnit)context.Session[_pageName + _clientId + "MapUnit"];
            if (!string.IsNullOrEmpty(_pageName) && !string.IsNullOrEmpty(_clientId))
            {
                GeoKeyedCollection<BaseOverlay> overlays = (GeoKeyedCollection<BaseOverlay>)context.Session[_pageName + _clientId + "Overlays"];
                if (overlays != null && overlays.Count > 0)
                {
                    foreach (string markerOverlayId in _markerOverlayIds)
                    {
                        if (overlays.Contains(markerOverlayId))
                        {
                            _markerOverlays.Add((BaseMarkerOverlay)overlays[markerOverlayId]);
                        }
                    }
                }
                InMemoryMarkerOverlay markerOverlay = (InMemoryMarkerOverlay)context.Session[_pageName + _clientId + "MarkerOverlay"];
                if (markerOverlay != null)
                {
                    _markerOverlays.Add(markerOverlay);
                }
            }
        }

        private void GenerateAndOutputMarkersJson(HttpContext context)
        {
            StringBuilder markersJson = new StringBuilder();
            markersJson.Append("[");
            foreach (BaseMarkerOverlay markerOverlay in _markerOverlays)
            {
                if (CheckClusterStyleUsed(markerOverlay))
                {
                    markersJson.AppendFormat(@"{{""id"":""{0}"",""iscluster"":""{1}"", ""markers"":{2}}},", markerOverlay.Id, "1", GetMarkerOverlayJson(markerOverlay));
                }
                else
                {
                    markersJson.AppendFormat(@"{{""id"":""{0}"", ""markers"":{1}}},", markerOverlay.Id, GetMarkerOverlayJson(markerOverlay));
                }
            }
            if (markersJson.ToString().EndsWith(",", StringComparison.Ordinal))
            {
                markersJson.Remove(markersJson.Length - 1, 1);
            }
            markersJson.Append("]");

            context.Response.Clear();
            context.Response.Write(markersJson.ToString());
            context.ApplicationInstance.CompleteRequest();
        }

        private bool CheckClusterStyleUsed(BaseMarkerOverlay markerOverlay)
        {
            MarkerZoomLevel zoomLevel = null;
            if (markerOverlay.GetType() == typeof(InMemoryMarkerOverlay))
            {
                zoomLevel = ((InMemoryMarkerOverlay)markerOverlay).ZoomLevelSet.GetZoomLevelForDrawing(_currentZoomLevelId);
            }
            else if (markerOverlay.GetType() == typeof(FeatureSourceMarkerOverlay))
            {
                zoomLevel = ((FeatureSourceMarkerOverlay)markerOverlay).ZoomLevelSet.GetZoomLevelForDrawing(_currentZoomLevelId);
            }

            if (zoomLevel != null)
            {
                if (zoomLevel.CustomMarkerStyle != null)
                {
                    if (typeof(ClusterMarkerStyle).IsInstanceOfType(zoomLevel.CustomMarkerStyle))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private string GetMarkerOverlayJson(BaseMarkerOverlay markerOverlay)
        {
            Collection<Marker> markers = markerOverlay.GetMarkers(_extent, _currentZoomLevelId);
            Collection<Marker> filteredMarkers = null;

            if (_gridSize > 0)
                filteredMarkers = GetFilteredMarkers(_gridSize, markers, _extent, _currentScale);

            if (filteredMarkers == null)
                filteredMarkers = markers;

            Collection<IJsonSerializable> jsonMarkers = new Collection<IJsonSerializable>();
            foreach (Marker marker in filteredMarkers)
                jsonMarkers.Add((IJsonSerializable)marker);

            return JsonConverter.ConvertJsonCollectionToJson(jsonMarkers);
        }

        private Collection<Marker> GetFilteredMarkers(int gridSizeForFilter, Collection<Marker> markers, RectangleShape targetWorldExtent, double targetScale)
        {
            Collection<string> usedGrid = new Collection<string>();
            Collection<Marker> newMarkers = new Collection<Marker>();

            foreach (Marker marker in markers)
            {
                PointShape point = marker.Position;

                if (point != null)
                {
                    string rowCell = GetRowCellIndex(targetWorldExtent, point, gridSizeForFilter, targetScale);
                    if (!usedGrid.Contains(rowCell))
                    {
                        usedGrid.Add(rowCell);
                        newMarkers.Add(marker);
                    }
                }
            }

            return newMarkers;
        }

        private string GetRowCellIndex(RectangleShape worldExtent, PointShape point, int gridSizeForFilter, double targetScale)
        {
            double currentResolution = MapUtilities.GetResolutionFromScale(targetScale, _mapUnit);
            double intervalXInWorldCoordinate = gridSizeForFilter * currentResolution;
            double intervalYInWorldCoordinate = intervalXInWorldCoordinate;

            int leftIndex = (int)Math.Floor((point.X - worldExtent.LowerLeftPoint.X) / intervalXInWorldCoordinate);
            int topIndex = (int)Math.Floor((worldExtent.UpperLeftPoint.Y - point.Y) / intervalYInWorldCoordinate);
            return String.Format(CultureInfo.InvariantCulture, "{0},{1}", leftIndex, topIndex);
        }
    }
}
