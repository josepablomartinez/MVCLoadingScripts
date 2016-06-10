using System;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class MarkerClassBreak
    {
        private double _breakValue;
        private PointMarkerStyle _defaultMarkerStyle;
        private BaseMarkerStyle _customMarkerStyle;

        public MarkerClassBreak()
            : this(Double.MinValue)
        { }

        public MarkerClassBreak(double value)
        {
            this._breakValue = value;
        }

        public double Value
        {
            get { return _breakValue; }
            set { _breakValue = value; }
        }

        public PointMarkerStyle DefaultMarkerStyle
        {
            get
            {
                if (_defaultMarkerStyle == null)
                {
                    _defaultMarkerStyle = new PointMarkerStyle();
                }
                return _defaultMarkerStyle;
            }
            set
            {
                Validators.CheckParameterIsNotNull(value, "DefaultMarkerStyle");
                _defaultMarkerStyle = value;
            }
        }

        public BaseMarkerStyle CustomMarkerStyle
        {
            get
            {
                return _customMarkerStyle;
            }
            set
            {
                _customMarkerStyle = value;
            }
        }
    }
}
