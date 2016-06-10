using System;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class MarkerValueItem
    {
        private string _itemValue;
        private PointMarkerStyle _defaultMarkerStyle;
        private BaseMarkerStyle _customMarkerStyle;

        public MarkerValueItem()
            : this(String.Empty)
        { }

        public MarkerValueItem(string value)
            : this(value, null)
        { }

        public MarkerValueItem(string value, BaseMarkerStyle customMarkerStyle)
        {
            this._itemValue = value;
            this._customMarkerStyle = customMarkerStyle;
        }

        public string Value
        {
            get { return _itemValue; }
            set { _itemValue = value; }
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
