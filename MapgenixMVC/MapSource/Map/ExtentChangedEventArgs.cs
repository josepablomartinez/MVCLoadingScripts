using System;
using Mapgenix.Shapes;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class ExtentChangedEventArgs : EventArgs
    {
        private RectangleShape _currentExtent;
        private double _currentScale;

        public ExtentChangedEventArgs()
            : this(null, Double.NaN)
        { }

      
        public ExtentChangedEventArgs(RectangleShape currentExtent, double currentScale)
        {
            this._currentExtent = currentExtent;
            this._currentScale = currentScale;
        }

        public RectangleShape CurrentExtent
        {
            get { return _currentExtent; }
            set { _currentExtent = value; }
        }

        public double CurrentScale
        {
            get { return _currentScale; }
            set { _currentScale = value; }
        }
    }
}
