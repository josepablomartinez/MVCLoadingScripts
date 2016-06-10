using System;
using Mapgenix.Shapes;

namespace Mapgenix.GSuite.Mvc
{
   
    [Serializable]
    public class HighlightFeatureOverlayClickEventArgs : EventArgs
    {
        private PointShape _location;
        private Feature _clickedFeature;

       
        public HighlightFeatureOverlayClickEventArgs()
            : this(new PointShape(0, 0), new Feature())
        {
        }

      
        public HighlightFeatureOverlayClickEventArgs(double x, double y, Feature clickedFeature)
            : this(new PointShape(x, y), clickedFeature)
        {
        }

       
        public HighlightFeatureOverlayClickEventArgs(PointShape location, Feature clickedFeature)
        {
            this._location = location;
            this._clickedFeature = clickedFeature;
        }

        public PointShape Location
        {
            get { return _location; }
            set { _location = value; }
        }

       
        public Feature ClickedFeature
        {
            get { return _clickedFeature; }
            set { _clickedFeature = value; }
        }
    }
}
