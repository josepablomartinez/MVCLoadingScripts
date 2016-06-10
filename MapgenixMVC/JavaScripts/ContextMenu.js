var ContextMenu = Class.Create();
var ContextMenuItem = Class.Create();
Class.Extent(ContextMenu.prototype, {
    id: null,
    div: null,
    items: null,
    lonlat: null,
    target: null,
    mapUniqueId: null,
    activeSubMenu: null,
    initialize: function(json, lonlat, target, mapUniqueId) {
        this.id = json.id;
        this.lonlat = lonlat;
        this.target = target;
        this.mapUniqueId = mapUniqueId;
        this.div = NewDomNode('DIV');
        this.div.id = this.id;
        if (CheckCssName(json.css)) {
            this.div.className = json.css;
        } else {
            this.setDefaultStyle();
        }
        this.div.style.position = 'absolute';
        this.div.style.width = json.w == 0 ? '100px' : (json.w + 'px');
        this.div.style.zIndex = 100000;
        this.div.style.display = 'none';
        this.div.oncontextmenu = function() { return false; };
    },
    addItem: function(itemJson) {
        var item = new ContextMenuItem(itemJson);
        item.parent = this;
        item.div.parentId = this.id;
        item.div.lonlat = this.lonlat;
        item.div.target = this.target;
        item.div.uid = this.mapUniqueId;

        if (itemJson.fullId) {
            item.div.fullId = itemJson.fullId;
        }
        else {
            item.div.fullId = itemJson.id;
        }

        this.div.appendChild(item.div);
    },
    display: function(visible) {
        if (visible) {
            this.div.style.display = 'block';
        } else {
            if (this.div.parentNode) {
                this.div.parentNode.removeChild(this.div);
               
                if (this.activeSubMenu) {
                    this.activeSubMenu.display(false);
                    this.activeSubMenu = null;
                }
            }
        }
    },
    setDefaultStyle: function() {
        this.div.style.padding = '1px';
        this.div.style.backgroundColor = 'white';
        this.div.style.border = '#cccccc 1px solid';
        this.div.style.borderRight = '#333333 1px solid';
        this.div.style.borderBottom = '#333333 1px solid';
        this.div.style.lineHeight = '18px';
    },
    moveTo: function(x, y) {
        var screenWidth = parseInt(document.body.clientWidth);
        var screenHeight = parseInt(document.body.clientHeight);

        if (!window.navigator.userAgent.indexOf("MSIE") < 1) {
            screenWidth = parseInt(document.documentElement.clientWidth);
            screenHeight = parseInt(document.documentElement.clientHeight);
        }
        var ctxWidth = parseInt(this.div.style.width);
        if (x + ctxWidth > screenWidth) {
            x -= ctxWidth;
        }

        var ctxHeight = this.div.children.length * 20;
        if (y + ctxHeight > screenHeight) {
            y -= ctxHeight;
        }

        this.div.style.left = x + 'px';
        this.div.style.top = y + 'px';
    }
});

Class.Extent(ContextMenuItem.prototype, {
    id: null,
    div: null,
    parent: null,
    initialize: function(json) {
        this.id = json.id;
        this.div = NewDomNode('DIV');
        this.div.id = this.id;
        this.div.innerHTML = json.html;
        this.div.menuItem = this;

        if (CheckCssName(json.css)) {
            this.div.className = json.css;
        } else {
            if (json.items && json.items.length > 0) {
                this.div.className = 'menuItemContainingSubItems';
            }
            this.setDefaultStyle();
        }

        this.div.normalCss = json.css;
        this.div.hoverCss = json.hoverCss;
        this.div.onmouseover = function() {
            if (CheckCssName(this.hoverCss)) {
                this.className = this.hoverCss;
            } else {
                this.style.backgroundColor = '#99ffff';
            }

            if (this.menuItem.parent.activeSubMenu) {

                this.menuItem.parent.activeSubMenu.display(false);
            }

            if (json.items && json.items.length > 0) {

                var subMenuJson = { id: this.id + "!" + "subMenu", w: 0 };
                var subContextMenu = new ContextMenu(subMenuJson, this.menuItem.parent.lonlat, this.menuItem.parent.target, this.menuItem.parent.mapUniqueId);
                for (var i = 0; i < json.items.length; i++) {
                    json.items[i].fullId = this.fullId ? this.fullId + "!" + json.items[i].id : this.id + "!" + json.items[i].id;
                    subContextMenu.addItem(json.items[i]);
                }
                var left = this.menuItem.parent.div.style.left.replace("px", "");
                var top = this.menuItem.parent.div.style.top.replace("px", "");
                subContextMenu.moveTo(parseInt(left) + this.offsetWidth + 3, parseInt(top) + this.offsetTop - 2);
                subContextMenu.display(true);
                document.getElementById(subContextMenu.mapUniqueId).parentNode.appendChild(subContextMenu.div);
                this.menuItem.parent.activeSubMenu = subContextMenu;
            }
        };
        this.div.onmouseout = function() {
            if (CheckCssName(this.normalCss)) {
                this.className = this.normalCss;
            } else {
                this.style.backgroundColor = 'white';
            }
        };

        if (json.click && json.click != 'undefined') {
            this.div.onclick = function(evt) {
                __doPostBack(this.uid, 'CONTEXTMENUCLICK^' + this.lonlat.lon + '^' + this.lonlat.lat + '^' + this.target + '^' + this.fullId + '^' + this.parentId);
            }
        } else if (json.clientClick && json.clientClick != 'undefined') {
            eval('var click = ' + json.clientClick);
            this.div.onclick = click;
        }
    },
    setDefaultStyle: function() {
        this.div.style.paddingLeft = '4px';
        this.div.style.paddingTop = '4px';
        this.div.style.fontSize = '10px';
        this.div.style.fontFamily = 'verdana';
        this.div.style.color = '#0000cc';
        this.div.style.cursor = 'pointer';
        this.div.style.backgroundColor = 'white';
    }
});

function CheckCssName(css) {
    if (css && css != 'undefined' && css != '') {
        return true;
    } else return false;
}
