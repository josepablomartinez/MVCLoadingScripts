using System;
using System.Collections.ObjectModel;
using Mapgenix.Shapes;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public abstract class BaseMarkerOverlay : BaseOverlay
    {
        private Collection<string> _handlerMethodNames;

        public event EventHandler<MarkerOverlayClickEventArgs> Click;

        protected BaseMarkerOverlay()
            : this(new Guid().ToString()) { }

        protected BaseMarkerOverlay(string id)
            : base(id, false)
        {
        }

        [JsonMember(MemberName = "isBaseLayer")]
        public override bool IsBaseOverlay
        {
            get
            {
                return base.IsBaseOverlay;
            }
            set
            {
                if (value)
                {
                    //Validators.NotSupport(Mvc.Properties.ExceptionDescription.MarkerLayerNotSupportBaseLayer);
                }
                base.IsBaseOverlay = value;
            }
        }

        [JsonMember(MemberName = "otype")]
        protected override string OverlayType
        {
            get
            {
                return "MARKERS";
            }
        }

        [JsonMember(MemberName = "click")]
        internal protected bool HasClickEvent
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

        [JsonMember(MemberName = "isDefault")]
        internal protected bool IsDefault { get; set; }


        public Collection<Marker> GetMarkers(RectangleShape worldExtent, int currentZoomLevelId)
        {
            return GetMarkersCore(worldExtent, currentZoomLevelId);
        }

        protected abstract Collection<Marker> GetMarkersCore(RectangleShape worldExtent, int currentZoomLevelId);

        internal void RaiseClickEvent(string overlayId, string featureId)
        {
            OnClick(new MarkerOverlayClickEventArgs(overlayId, featureId));
        }

        protected virtual void OnClick(MarkerOverlayClickEventArgs e)
        {
            if (HasClickEvent)
            {
                Click(this, e);
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
        }

        internal void RewireClickEvent(object target)
        {
            if (_handlerMethodNames != null)
            {
                foreach (string methodName in _handlerMethodNames)
                {
                    Click += (EventHandler<MarkerOverlayClickEventArgs>)Delegate.CreateDelegate(typeof(EventHandler<MarkerOverlayClickEventArgs>), target, methodName);
                }
            }
        }
    }
}
