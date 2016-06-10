using System;
using System.Collections.ObjectModel;
using Mapgenix.Shapes;
using Mapgenix.Canvas;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class HighlightFeatureOverlay : BaseFeatureOverlay
    {
        private FeatureOverlayStyle _highlightStyle;
        private string _onClientClick;
        private ContextMenu _contextMenu;
        private Collection<string> _handlerMethodNames;

        public event EventHandler<HighlightFeatureOverlayClickEventArgs> Click;

        internal HighlightFeatureOverlay(string id)
            : base(id, false)
        {
            this.IsVisibleInOverlaySwitcher = false;
            _highlightStyle = new FeatureOverlayStyle(new GeoColor(80, GeoColor.FromHtml("#ff3300")), new GeoColor(160, GeoColor.FromHtml("#ff3300")), 1);
        }

        [JsonMember(MemberName = "highlightStyle")]
        public FeatureOverlayStyle HighlightStyle
        {
            get
            {
                return _highlightStyle;
            }
            set
            {
                _highlightStyle = value;
            }
        }

        [JsonMember(MemberName = "contextMenu")]
        public ContextMenu ContextMenu
        {
            get
            {
                return _contextMenu;
            }
            set
            {
                _contextMenu = value;
            }
        }

        [JsonMember(MemberName = "onClientClick")]
        public string OnClientClick
        {
            get
            {
                return _onClientClick;
            }
            set
            {
                _onClientClick = value;
            }
        }

        [JsonMember(MemberName = "click")]
        protected bool HasClickEvent
        {
            get 
            {
                if (Click != null || (_handlerMethodNames != null && _handlerMethodNames.Count > 0))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        protected virtual void OnClick(HighlightFeatureOverlayClickEventArgs e)
        {
            if (this.Click != null)
            {
                Click(this, e);
            }
        }

        internal void RaiseClickEvent(double longitude, double latitude, string featureId)
        {
            foreach(Feature feature in Features)
            {
                if (feature.Id == featureId)
                {
                    HighlightFeatureOverlayClickEventArgs args = new HighlightFeatureOverlayClickEventArgs(longitude, latitude, feature);
                    OnClick(args);
                    break;
                }
            }
        }

        internal void DetachClickEvent()
        {
            if (this.Click != null)
            {
                _handlerMethodNames = new Collection<string>();
                Delegate[] delegateList = this.Click.GetInvocationList();
                foreach (Delegate handler in delegateList)
                {
                    _handlerMethodNames.Add(handler.Method.Name);
                }
                this.Click = null;
            }
            else
            {
                if (_handlerMethodNames != null) { _handlerMethodNames.Clear(); }
            }
        }

        internal void RewireClickEvent(object target)
        {
            if (_handlerMethodNames != null)
            {
                foreach (string methodName in _handlerMethodNames)
                {
                    Click += (EventHandler<HighlightFeatureOverlayClickEventArgs>)Delegate.CreateDelegate(typeof(EventHandler<HighlightFeatureOverlayClickEventArgs>), target, methodName);
                }
            }
        }

        internal void DetachContextMenuClickEvents()
        {
            if (_contextMenu != null)
            {
                _contextMenu.DetachItemsClickEvent();
            }
        }

        internal void RewireContextMenuClickEvents(object target)
        {
            if (_contextMenu != null)
            {
                _contextMenu.RewireItemsClickEvent(target);
            }
        }
    }
}
