using System;
using System.Collections.ObjectModel;
using Mapgenix.Shapes;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class ContextMenuItem : IJsonSerializable, IRequireId
    {
        private string _id;
        private string _innerHtml;
        private string _cssClass;
        private string _hoverCssClass;
        private string _onClientClick;
        private Collection<string> _handlerMethodNames;
        private GeoKeyedCollection<ContextMenuItem> _menuItems;
      
        public event EventHandler<ContextMenuItemClickEventArgs> Click;

        public ContextMenuItem()
            : this(string.Empty, String.Empty, string.Empty)
        {
        }

        public ContextMenuItem(string innerHtml)
            : this(innerHtml, String.Empty, string.Empty)
        { }

       
        public ContextMenuItem(string innerHtml, string cssClass)
            : this(innerHtml, cssClass, string.Empty)
        {
        }

       
        public ContextMenuItem(string innerHtml, string cssClass, string hoverCssClass)
            : this(innerHtml, cssClass, hoverCssClass, Guid.NewGuid().ToString())
        {
        }

      
        public ContextMenuItem(string innerHtml, string cssClass, string hoverCssClass, string id)
        {
            this._id = id;
           
            this.InnerHtml = innerHtml;
            this.CssClass = cssClass;
            this.HoverCssClass = hoverCssClass;
            this._menuItems = new GeoKeyedCollection<ContextMenuItem>();
        }

       
        [JsonMember(MemberName = "items")]
        public GeoKeyedCollection<ContextMenuItem> MenuItems
        {
            get { return this._menuItems; }
        }

        [JsonMember(MemberName = "id")]
        public string Id
        {
            get { return this._id; }
        }
       
        [JsonMember(MemberName = "css")]
        public string CssClass
        {
            get { return _cssClass; }
            set { _cssClass = value; }
        }

        
        [JsonMember(MemberName = "hoverCss")]
        public string HoverCssClass
        {
            get { return _hoverCssClass; }
            set { _hoverCssClass = value; }
        }

      
        [JsonMember(MemberName = "clientClick")]
        public string OnClientClick
        {
            get { return _onClientClick; }
            set { _onClientClick = value; }
        }

        [JsonMember(MemberName = "html")]
        public String InnerHtml
        {
            get { return _innerHtml; }
            set { _innerHtml = value; }
        }

       
        [JsonMember(MemberName = "click")]
        protected bool HasClickEvent
        {
            get
            {
                if (Click != null || (_handlerMethodNames != null && _handlerMethodNames.Count > 0))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        internal void DetachClickEvent()
        {
            if (this.Click != null)
            {
                _handlerMethodNames = new Collection<string>();
                Delegate[] delegateList = this.Click.GetInvocationList();
                foreach (Delegate handler in delegateList)
                {
                    _handlerMethodNames.Add(handler.Method.Name);
                }
                this.Click = null;
            }

            if (this._menuItems.Count > 0)
            {
                foreach (ContextMenuItem item in this._menuItems)
                {
                    item.DetachClickEvent();
                }
            }
        }

        internal void RewireClickEvent(object target)
        {
            if (_handlerMethodNames != null)
            {
                foreach (string methodName in _handlerMethodNames)
                {
                    this.Click += (EventHandler<ContextMenuItemClickEventArgs>)Delegate.CreateDelegate(typeof(EventHandler<ContextMenuItemClickEventArgs>), target, methodName);
                }
            }

            if (this._menuItems.Count > 0)
            {
                foreach (ContextMenuItem item in this._menuItems)
                {
                    item.RewireClickEvent(target);
                }
            }
        }

        internal ContextMenuItem CloneShallow()
        {
            return (ContextMenuItem)this.MemberwiseClone();
        }

        internal ContextMenuItem FindChildMenuItemByFullId(string fullId)
        {
            if (this._menuItems.Contains(fullId) == true)
            {
                return this._menuItems[fullId];
            }
            else
            {
                string currentItemId = fullId.Split('!')[0];
                int index = fullId.IndexOf('!');
                string subFullId = fullId.Substring(index + 1);

                return this._menuItems[currentItemId].FindChildMenuItemByFullId(subFullId);
            }
        }

        #region IPostBackEventHandler Members

        internal void RaisePostBackEvent(double lontitude, double latitude)
        {
            if (HasClickEvent)
            {
                OnClick(new ContextMenuItemClickEventArgs(new PointShape(lontitude, latitude)));
            }
        }

       
        protected virtual void OnClick(ContextMenuItemClickEventArgs e)
        {
            if (HasClickEvent)
            {
                Click(this, e);
            }
        }

        #endregion

        #region IJsonSerializable Members

        public string ToJson()
        {
            return JsonConverter.ConvertObjectToJson(this);
        }

        #endregion
    }
}
