using System;
using Mapgenix.Shapes;

namespace Mapgenix.GSuite.Mvc
{
    public class MarkerDraggedEventArgs : EventArgs
    {
        private string _markerId;
        private PointShape _markerPosition;

        public MarkerDraggedEventArgs()
            : this(String.Empty, null)
        { }

       
        public MarkerDraggedEventArgs(string markerId, PointShape markerPosition)
        {
            this._markerId = markerId;
            this._markerPosition = markerPosition;
        }

        public string MarkerId
        {
            get { return _markerId; }
            set { _markerId = value; }
        }

        public PointShape MarkerPosition
        {
            get { return _markerPosition; }
            set { _markerPosition = value; }
        }
    }
}
