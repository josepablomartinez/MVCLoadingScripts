using System;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class BaseOverlayChangedEventArgs : EventArgs
    {
        private string _previousBaseOverlayId;
        private string _currentBaseOverlayId;

        public BaseOverlayChangedEventArgs()
            : this(string.Empty, string.Empty)
        {
        }

        public BaseOverlayChangedEventArgs(string previousBaseOverlayId, string currentBaseOverlayId)
        {
            this._previousBaseOverlayId = previousBaseOverlayId;
            this._currentBaseOverlayId = currentBaseOverlayId;
        }

        public string PreviousBaseOverlayId
        {
            get
            {
                return _previousBaseOverlayId;
            }
            set
            {
                _previousBaseOverlayId = value;
            }
        }

        public string CurrentBaseOverlayId
        {
            get
            {
                return _currentBaseOverlayId;
            }
            set
            {
                _currentBaseOverlayId = value;
            }
        }
    }
}
