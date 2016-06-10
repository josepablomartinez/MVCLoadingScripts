using System;
using System.Web.UI;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public abstract class BaseOverlay : IRequireId, IJsonSerializable
    {
        private string _id;
        private string _name;
        private bool _isVisible;
        private float _opacity;
        private bool _isBaseOverlay;
        private bool _isVisibleInOverlaySwitcher;

        private TimeSpan _autoRefreshInterval;
        private double _autoRefreshMilliseconds;

        protected BaseOverlay()
            : this(Guid.NewGuid().ToString(), true)
        { }

       
        protected BaseOverlay(string id)
            : this(id, true)
        {
        }

        protected BaseOverlay(string id, bool isBaseOverlay)
        {
            //Validators.CheckParameterIsNotNullOrEmpty(id, "id");

            this._id = id;
            this._name = id;
            this._isBaseOverlay = isBaseOverlay;
            this._isVisible = true;
            this._isVisibleInOverlaySwitcher = true;
            this._opacity = 1;
        }

       
        public event EventHandler<EventArgs> Tick;

       
        [JsonMember(MemberName = "visibility")]
        public virtual bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                _isVisible = value;
            }
        }

       
        [JsonMember(MemberName = "name")]
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

       
        [JsonMember(MemberName = "opacity")]
        public float Opacity
        {
            get
            {
                return _opacity;
            }
            set
            {
                _opacity = value;
            }
        }

        [JsonMember(MemberName = "displayInLayerSwitcher")]
        public bool IsVisibleInOverlaySwitcher
        {
            get
            {
                return _isVisibleInOverlaySwitcher;
            }
            set
            {
                _isVisibleInOverlaySwitcher = value;
            }
        }

        [JsonMember(MemberName = "isBaseLayer")]
        public virtual bool IsBaseOverlay
        {
            get
            {
                return _isBaseOverlay;
            }
            set
            {
                _isBaseOverlay = value;
            }
        }

        [JsonMember(MemberName = "Id")]
        public string Id
        {
            get
            {
                return _id;
            }
        }

       
        [JsonMember(MemberName = "classType")]
        protected abstract string OverlayType
        {
            get;
        }

        internal void RegisterJavaScriptLibrary(Page page)
        {
            RegisterJavaScriptLibraryCore(page);
        }

       
        protected virtual void RegisterJavaScriptLibraryCore(Page page)
        { }

      
        [JsonMember(MemberName = "tick")]
        protected virtual bool HasTickEvent
        {
            get
            {
                return (Tick != null);
            }
        }

        public TimeSpan AutoRefreshInterval
        {
            get
            {
                return _autoRefreshInterval;
            }
            set
            {
                _autoRefreshInterval = value;
                AutoRefreshMilliseconds = _autoRefreshInterval.TotalMilliseconds;
            }
        }


        [JsonMember(MemberName = "AutoRefreshMilliseconds")]
        internal double AutoRefreshMilliseconds
        {
            get
            {
                return _autoRefreshMilliseconds;
            }
            set
            {
                _autoRefreshMilliseconds = value;
            }
        }

        #region IJsonSerializable Members

       
        public virtual string ToJson()
        {
            return JsonConverter.ConvertObjectToJson(this);
        }

        #endregion

        internal virtual void RaiseTickEvent(EventArgs e)
        {
            OnTick(e);
        }

        protected virtual void OnTick(EventArgs e)
        {
            EventHandler<EventArgs> handler = Tick;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
