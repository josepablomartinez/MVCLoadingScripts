using System;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class MarkerOverlayClickEventArgs : EventArgs
    {
        private string _featureId;
        private string _overlayId;

        public MarkerOverlayClickEventArgs()
            : this(Guid.NewGuid().ToString(), String.Empty)
        {
        }

        public MarkerOverlayClickEventArgs(string overlayId, string featureId)
        {
            this._featureId = featureId;
            this._overlayId = overlayId;
        }

        public string FeatureId
        {
            get
            {
                return _featureId;
            }
            set
            {
                _featureId = value;
            }
        }

      
        public string OverlayId
        {
            get
            {
                return _overlayId;
            }
            set
            {
                _overlayId = value;
            }
        }
    }
}
