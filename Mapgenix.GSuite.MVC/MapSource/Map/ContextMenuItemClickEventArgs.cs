using System;
using Mapgenix.Shapes;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class ContextMenuItemClickEventArgs : EventArgs
    {
        private PointShape _location;

        public ContextMenuItemClickEventArgs()
            : this(new PointShape())
        { }

      
        public ContextMenuItemClickEventArgs(PointShape location)
        {
            this._location = location;
        }

       
        public PointShape Location
        {
            get { return _location; }
            set { _location = value; }
        }
    }
}
