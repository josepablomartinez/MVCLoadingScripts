using System;
using Mapgenix.Shapes;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class MapClickedEventArgs : EventArgs
    {
        public MapClickedEventArgs()
            : this(new PointShape())
        { }

        public MapClickedEventArgs(PointShape position)
        {
            Position = position;
        }

        public PointShape Position { get; set; }
    }
}
