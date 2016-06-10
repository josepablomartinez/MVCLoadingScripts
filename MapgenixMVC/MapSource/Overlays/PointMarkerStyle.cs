using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Mapgenix.Shapes;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class PointMarkerStyle : BaseMarkerStyle
    {
        private CustomPopup _popup;
        private int _popupDelay;
        private ContextMenu _contextMenu;
        private WebImage _webImage;
        private float _opacity;

        public PointMarkerStyle()
            : this(new WebImage(string.Empty), null, null)
        {
        }

        public PointMarkerStyle(WebImage webImage)
            : this(webImage, null, null)
        {
        }

        public PointMarkerStyle(WebImage webImage, CustomPopup popup)
            : this(webImage, popup, null)
        { }

        public PointMarkerStyle(WebImage webImage, CustomPopup popup, ContextMenu contextMenu)
        {
            this._webImage = webImage;
            this._popup = popup;
            if (popup != null)
            {
                this._popup.IsVisible = false;
            }
            this._popupDelay = 500;
            this._contextMenu = contextMenu;
            this._opacity = 1;
        }

        public CustomPopup Popup
        {
            get
            {
                if (_popup == null)
                {
                    _popup = new CustomPopup();
                    _popup.IsVisible = false;
                }
                return _popup;
            }
            set
            {
                //Validators.CheckParameterIsNotNull(value, "Popup");
                _popup = value;
            }
        }

        public int PopupDelay
        {
            get
            {
                return _popupDelay;
            }
            set
            {
                _popupDelay = value;
            }
        }

        public WebImage WebImage
        {
            get
            {
                return _webImage;
            }
            set
            {
                //Validators.CheckParameterIsNotNull(value, "WebImage");
                _webImage = value;
            }
        }

        public ContextMenu ContextMenu
        {
            get
            {
                return _contextMenu;
            }
            set
            {
                //Validators.CheckParameterIsNotNull(value, "ContextMenu");
                _contextMenu = value;
            }
        }

        public float Opacity
        {
            get
            {
                return _opacity;
            }
            set
            {
                _opacity = value;
            }
        }

        protected override GeoKeyedCollection<ContextMenu> GetContextMenusCore()
        {
            GeoKeyedCollection<ContextMenu> contextMenus = new GeoKeyedCollection<ContextMenu>();
            if (_contextMenu != null)
            {
                contextMenus.Add(_contextMenu);
            }
            return contextMenus;
        }

        public override Collection<Marker> GetMarkers(IEnumerable<Feature> features)
        {
            Collection<Marker> returnMarkers = new Collection<Marker>();
            foreach (Feature feature in features)
            {
                if (feature.GetWellKnownType() == WellKnownType.Point)
                {
                    PointShape point = (PointShape)feature.GetShape();
                    Marker marker = CreateMarkerByPoint(point, feature.Id, feature.ColumnValues);
                    returnMarkers.Add(marker);
                }
                else if (feature.GetWellKnownType() == WellKnownType.Multipoint)
                {
                    MultipointShape multiPoint = (MultipointShape)feature.GetShape();
                    for (int i = 0; i < multiPoint.Points.Count; i++)
                    {
                        string markerId = feature.Id + "_" + (i + 1).ToString(CultureInfo.InvariantCulture);
                        Marker marker = CreateMarkerByPoint(multiPoint.Points[i], markerId, feature.ColumnValues);
                        returnMarkers.Add(marker);
                    }
                }
            }
            return returnMarkers;
        }

        private Marker CreateMarkerByPoint(PointShape point, string markerId, Dictionary<string, string> columnValues)
        {
            Marker marker = new Marker(point);
            marker.Id = markerId;
            marker.Popup = (CustomPopup)Popup.CloneShallow();
            marker.PopupDelay = PopupDelay;
            marker.WebImage = WebImage.CloneShallow();
            marker.Opacity = Opacity;

            if (ContextMenu != null)
            {
                marker.ContextMenu = ContextMenu.CloneDeep();
            }

            if (marker.Popup != null)
            {
                foreach (string key in columnValues.Keys)
                {
                    marker.Popup.ContentHtml = marker.Popup.ContentHtml.Replace("[#" + key + "#]", columnValues[key].ToString());
                }
            }

            if (marker.ContextMenu != null)
            {
                foreach (ContextMenuItem menuItem in marker.ContextMenu.MenuItems)
                {
                    foreach (string key in columnValues.Keys)
                    {
                        menuItem.InnerHtml = menuItem.InnerHtml.Replace("[#" + key + "#]", columnValues[key].ToString());
                    }
                }
            }

            if (!String.IsNullOrEmpty(marker.WebImage.Text))
            {
                foreach (string key in columnValues.Keys)
                {
                    marker.WebImage.Text = marker.WebImage.Text.Replace("[#" + key + "#]", columnValues[key].ToString());
                }
            }

            return marker;
        }
    }
}
