using System;

namespace Mapgenix.GSuite.Mvc
{
    [Serializable]
    public class ContextMenu : IJsonSerializable, IRequireId
    {
        private GeoKeyedCollection<ContextMenuItem> _menuItems;

        public ContextMenu()
            : this(Guid.NewGuid().ToString(), 0, String.Empty)
        { }

        public ContextMenu(string id, int width)
            : this(id, width, String.Empty)
        { }

        public ContextMenu(string id, int width, string cssClass)
        {
            Id = id;
            CssClass = cssClass;
            Width = width;
            _menuItems = new GeoKeyedCollection<ContextMenuItem>();
        }

        [JsonMember(MemberName = "id")]
        public string Id { get; set; }

        [JsonMember(MemberName = "w")]
        public int Width { get; set; }

        [JsonMember(MemberName = "css")]
        public string CssClass { get; set; }

       
        [JsonMember(MemberName = "items")]
        public GeoKeyedCollection<ContextMenuItem> MenuItems
        {
            get
            {
                return _menuItems;
            }
        }

        internal void DetachItemsClickEvent()
        {
            foreach (ContextMenuItem menuItem in MenuItems)
            {
                menuItem.DetachClickEvent();
            }
        }

        internal void RewireItemsClickEvent(object target)
        {
            foreach (ContextMenuItem menuItem in MenuItems)
            {
                menuItem.RewireClickEvent(target);
            }
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

        public ContextMenu CloneDeep()
        {
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.CssClass = this.CssClass;
            contextMenu.Id = this.Id;
            contextMenu.Width = this.Width;
            contextMenu._menuItems = new GeoKeyedCollection<ContextMenuItem>();
            foreach (ContextMenuItem item in MenuItems)
            {
                contextMenu.MenuItems.Add(item.CloneShallow());
            }

            return contextMenu;
        }

        #region IJsonSerializable Members

        public string ToJson()
        {
            return JsonConverter.ConvertObjectToJson(this);
        }

        #endregion
    }
}
