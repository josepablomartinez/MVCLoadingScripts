using System;
using System.Collections.ObjectModel;
using Mapgenix.Shapes;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class SimpleMarkerOverlay : BaseMarkerOverlay
    {
        private GeoKeyedCollection<Marker> _markers;
        private MarkerDragMode _dragMode;
        private Collection<string> _draggedEventHandlerNames;

        public event EventHandler<MarkerDraggedEventArgs> MarkerDragged;

        protected void OnMarkerDragged(MarkerDraggedEventArgs e)
        {
            EventHandler<MarkerDraggedEventArgs> handler = MarkerDragged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        
        public SimpleMarkerOverlay()
            : this(Guid.NewGuid().ToString())
        { }

       
        public SimpleMarkerOverlay(string id)
            : base(id)
        { }


        [JsonMember(MemberName = "markers")]
        public GeoKeyedCollection<Marker> Markers
        {
            get
            {
                if (_markers == null)
                {
                    _markers = new GeoKeyedCollection<Marker>();
                }
                return _markers;
            }
        }

       [JsonMember(MemberName = "dragMode")]
        public MarkerDragMode DragMode
        {
            get
            {
                return _dragMode;
            }
            set
            {
                _dragMode = value;
            }
        }

        protected override Collection<Marker> GetMarkersCore(RectangleShape worldExtent, int currentZoomLevelId)
        {
            Collection<Marker> returnMarkers = new Collection<Marker>();
            if (_markers != null)
            {
                foreach (Marker marker in _markers)
                {
                    if (worldExtent.Contains(marker.Position))
                    {
                        returnMarkers.Add(marker);
                    }
                }
            }
            return returnMarkers;
        }

        internal void RaiseMarkerDraggedEvent(MarkerDraggedEventArgs e)
        {
            OnMarkerDragged(e);
        }

        internal void DetachMarkerDraggedEvent()
        {
            if (MarkerDragged != null)
            {
                _draggedEventHandlerNames = new Collection<string>();
                Delegate[] handlers = MarkerDragged.GetInvocationList();
                foreach(Delegate handler in handlers)
                {
                    _draggedEventHandlerNames.Add(handler.Method.Name);
                }

                MarkerDragged = null;
            }
        }

        internal void RewireMarkerDraggedEvent(object target)
        {
            if (_draggedEventHandlerNames != null)
            {
                foreach (string methodName in _draggedEventHandlerNames)
                {
                    MarkerDragged += (EventHandler<MarkerDraggedEventArgs>)Delegate.CreateDelegate(typeof(EventHandler<MarkerDraggedEventArgs>), target, methodName);
                }
            }
        }

        internal void DetachContextMenuClickEvents()
        {
            foreach (Marker marker in Markers)
            {
                if (marker.ContextMenu != null)
                {
                    marker.ContextMenu.DetachItemsClickEvent();
                }
            }
        }

        internal void RewireContextMenuClickEvents(object target)
        {
            foreach (Marker marker in Markers)
            {
                if (marker.ContextMenu != null)
                {
                    marker.ContextMenu.RewireItemsClickEvent(target);
                }
            }
        }

        [JsonMember(MemberName = "otype")]
        protected override string OverlayType
        {
            get
            {
                return "SIMPLEMARKERS";
            }
        }

        
        [JsonMember(MemberName = "draggedEvent")]
        protected bool HasMarkerDraggedEvent
        {
            get
            {
                return (MarkerDragged != null || (_draggedEventHandlerNames != null && _draggedEventHandlerNames.Count > 0));
            }
        }
    }
}
