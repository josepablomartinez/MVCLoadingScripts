using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Web;
using System.Web.SessionState;
using Mapgenix.Shapes;

namespace Mapgenix.GSuite.Mvc
{
    internal class MarkerPopupResource : IHttpAsyncHandler, IReadOnlySessionState
    {
        private BaseMarkerOverlay _markerOverlay;
        private string _markerId;
        private string _overlayId;
        private string _pageName;
        private string _clientId;
        private int _currentZoomId;

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
            _overlayId = context.Request.QueryString["oid"];
            _markerId = context.Request.QueryString["id"];
            _pageName = context.Server.UrlDecode(context.Request.QueryString["PageName"]);
            _clientId = context.Server.UrlDecode(context.Request.QueryString["ClientId"]);
            _currentZoomId = int.Parse(context.Request.QueryString["zoom"], CultureInfo.InvariantCulture);

            SetEnvironmentFromSession(context);
            GenerateAndOutputMarkersJson(context);
        }

        private void SetEnvironmentFromSession(HttpContext context)
        {
            if (!string.IsNullOrEmpty(_pageName) && !string.IsNullOrEmpty(_clientId))
            {
                GeoKeyedCollection<BaseOverlay> overlays = (GeoKeyedCollection<BaseOverlay>)context.Session[_pageName + _clientId + "Overlays"];
                if (overlays != null && overlays.Count > 0)
                {
                    if (overlays.Contains(_overlayId))
                    {
                        _markerOverlay = overlays[_overlayId] as BaseMarkerOverlay;
                    }
                }
            }
        }

        private void GenerateAndOutputMarkersJson(HttpContext context)
        {
            string markerJson = "";
            if (!String.IsNullOrEmpty(_markerId))
            {
                MarkerZoomLevel zoomLevel = null;
                Feature feature = new Feature();
                string id = _markerId.Split('_')[0];
                if (_markerOverlay.GetType() == typeof(InMemoryMarkerOverlay))
                {
                    InMemoryMarkerOverlay inmemoryMarkerOverlay = _markerOverlay as InMemoryMarkerOverlay;
                    inmemoryMarkerOverlay.FeatureSource.Open();
                    feature = inmemoryMarkerOverlay.FeatureSource.GetFeatureById(id, ReturningColumnsType.AllColumns);
                    zoomLevel = inmemoryMarkerOverlay.ZoomLevelSet.GetZoomLevelForDrawing(_currentZoomId);
                }
                else if (_markerOverlay.GetType() == typeof(FeatureSourceMarkerOverlay))
                {
                    FeatureSourceMarkerOverlay featureSourceMarkerOverlay = _markerOverlay as FeatureSourceMarkerOverlay;
                    featureSourceMarkerOverlay.FeatureSource.Open();
                    feature = featureSourceMarkerOverlay.FeatureSource.GetFeatureById(id, ReturningColumnsType.AllColumns);
                    zoomLevel = featureSourceMarkerOverlay.ZoomLevelSet.GetZoomLevelForDrawing(_currentZoomId);
                }

                if (feature != null && zoomLevel != null)
                {
                    Collection<Feature> features = new Collection<Feature>() { feature };
                    Collection<Marker> markers = zoomLevel.CustomMarkerStyle.GetMarkers(features);

                    foreach (Marker item in markers)
                    {
                        if (item.Id == _markerId)
                        {
                            markerJson = item.Popup.ToJson();
                            break;
                        }
                    }
                }
            }

            context.Response.Clear();
            context.Response.Write(markerJson);
            context.ApplicationInstance.CompleteRequest();
        }
    }
}
