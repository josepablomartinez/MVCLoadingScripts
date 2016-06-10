OpenLayers.Layer.Markers = OpenLayers.Class(OpenLayers.Layer.Markers, {
    drawMarker: function(marker) {
        var px = this.map.getLayerPxFromLonLat(marker.lonlat);
        if (px == null) {
            marker.display(false);
        } else {
            var markerImg = marker.draw(px);

            var iconUrl = markerImg.childNodes[0].src;
            var iconWidth = parseInt(markerImg.style.width);
            var iconHeight = parseInt(markerImg.style.height);
            var iconZIndex = markerImg.childNodes[0].style.zIndex;

            if (marker.text && marker.text.length != 0) {
                var textMarker = document.createElement('DIV');

                textMarker.innerText = marker.text;

                textMarker.style.width = iconWidth;
                textMarker.style.height = iconHeight;
                textMarker.style.color = marker.color;
                textMarker.style.paddingTop = marker.paddingTop;
                textMarker.style.fontFamily = marker.fontFamily;
                textMarker.style.fontSize = marker.fontSize;
                textMarker.style.fontStyle = marker.fontStyle;
                textMarker.style.zIndex = iconZIndex + 1;
                textMarker.style.left = (iconWidth - marker.text.length * marker.fontSize) / 2 + 2;
                textMarker.style.position = 'absolute';
                textMarker.style.overflow = 'hidden';
                textMarker.style.cursor = 'normal';

                markerImg.appendChild(textMarker);
            }

            if (!marker.drawn) {
                this.div.appendChild(markerImg);
                marker.drawn = true;
            }
        }
    }
});

OpenLayers.Marker.GeoMarker = OpenLayers.Class(OpenLayers.Marker, {

    id: null,
    uniqueId: null,
    extra: null,
    text: '',
    paddingTop: 0,
    fontSize: 10,
    color: 'black',
    fontFamily: 'verdana',
    fontStyle: 'normal',

    popup: null,
    hoverText: null,
    popupX: null,
    popupY: null,
    popupVisible: null,
    popupWidth: null,
    popupHeight: null,
    popupHasCloseButton: null,
    popupAutoPan: null,
    popupDelay: null,
    popupHideTimer: null,

    
    initialize: function(id, lonlat, icon) {
        this.id = id;
        this.lonlat = lonlat;

        var newIcon = (icon) ? icon : OpenLayers.Marker.defaultIcon();
        if (this.icon == null) {
            this.icon = newIcon;
        } else {
            this.icon.url = newIcon.url;
            this.icon.size = newIcon.size;
            this.icon.offset = newIcon.offset;
            this.icon.calculateOffset = newIcon.calculateOffset;
        }
        this.events = new OpenLayers.Events(this, this.icon.imageDiv, null);
    },
    display: function(display) {
        this.icon.display(display);
    },
    initializePopup: function(map) {
        if (this.icon.size == null) {
            this.popup = new Popup(this.popupJson, null).element;
        } else {
            this.popup = new Popup(this.popupJson, this.icon).element;
        }
        this.popup.lonlat = this.lonlat;
        this.popup.isMarkerHoverPopup = true;
        this.popupVisible = this.popupJson.visibility;
        map.addPopup(this.popup);

        if (!this.popupVisible) {
            this.popup.hide();
        }

        if (!this.popupVisible) {
            this.popup.events.register('mouseover', this, function(evt) {
                clearTimeout(this.hideTimer);
            });

            this.popup.events.register('mouseout', this, function(evt) {
                this.hidePopup(map);
            });
            this.popup.hide();
        }
    },

    showPopup: function() {
        if (!this.popupVisible)
            this.showTimer = setTimeout(Function.createDelegate(this,
            function() {
                if (this.popup && this.popup.div) {
                    if (!this.popup.map) this.popup.map = this.map;
                    this.popup.show();

                    if (typeof (OnPopupShowing) != 'undefined' && OnPopupShowing != null) {
                        OnPopupShowing.call(this.popup, { isLeft: this.popup.left, isTop: this.popup.top });
                    }
                }
            }), this.popupDelay);
    },

    hidePopup: function() {
        if (!this.popupVisible) {
            if (this.showTimer) {
                window.clearTimeout(this.showTimer);
            }
            this.hideTimer = setTimeout(Function.createDelegate(this,
                function() {
                    if (this.popup && this.popup.div) {
                        if (!this.popup.map) this.popup.map = this.map;
                        this.popup.hide();
                    }
                }), 500);
        }
    },

    movePopup: function(px) {
        if (this.popup && this.popup.div) {
            if (!this.popup.map) this.popup.map = this.map;
            this.popup.lonlat = this.map.getLonLatFromLayerPx(px);
            this.popup.updatePosition();
        }
    },

    CLASS_NAME: "OpenLayers.Marker.GeoMarker"
});

OpenLayers.Control.LayerSwitcher = OpenLayers.Class(OpenLayers.Control.LayerSwitcher, {
    onLayerChanged: function(newLayer) { },

    onInputClick: function(e) {
        if (!this.inputElem.disabled) {
            if (this.inputElem.type == "radio") {
                this.inputElem.checked = true;
                this.layer.map.setBaseLayer(this.layer);
                if (this.layerSwitcher.onLayerChanged != "") {
                    this.layerSwitcher.onLayerChanged(this.layer.map.baseLayer.name);
                }
            } else {
                this.inputElem.checked = !this.inputElem.checked;
                this.layerSwitcher.updateMap();
            }
        }
        OpenLayers.Event.stop(e);
    }
});

OpenLayers.Control.MousePosition = OpenLayers.Class(OpenLayers.Control.MousePosition, {
    showType: 'lonlat',
    numdigits: 5,
    formatOutput: function(lonLat) {
        var digits = parseInt(this.numdigits);
        var newHtml = '';
        if (this.showType == 'dms') {
            newHtml = this.prefix
                + this.getDms(lonLat.lon.toFixed(digits))
                + ', '
                + this.getDms(lonLat.lat.toFixed(digits))
                + this.suffix;
        } else if (this.showType == 'lonlat') {
            newHtml =
                this.prefix +
                lonLat.lon.toFixed(digits) +
                this.separator +
                lonLat.lat.toFixed(digits) +
                this.suffix;
        } else if (this.showType == 'latlon') {
            newHtml =
                this.prefix +
                lonLat.lat.toFixed(digits) +
                this.separator +
                lonLat.lon.toFixed(digits) +
                this.suffix;
        }

        return newHtml;
    },

    getDms: function(value) {
        var absDdValue = Math.abs(value);

        var degrees;
        if (value >= 0) {
            degrees = Math.floor(value);
        } else {
            degrees = Math.ceil(value);
        }

        var tmpMinute = this.calculateDmsFactor(absDdValue);
        var tmpSecond = this.calculateDmsFactor(tmpMinute);

        return degrees + "&deg; " + parseInt(tmpMinute) + "' " + parseInt(tmpSecond) + "''";
    },

    calculateDmsFactor: function(value) {
        var decimals = 10000;
        return Math.round(((value - Math.floor(value)) * 60 * decimals)) / decimals;
    }
});

OpenLayers.Control.Logo = OpenLayers.Class(OpenLayers.Control.Attribution, {
    innerHTML: null,
    initialize: function(url) {
        var arVersion = navigator.appVersion.split("MSIE");
        var version = parseFloat(arVersion[1]);
        if (version == 6) {
            this.innerHTML = '<div style="height:26px; width:76px; filter:progid:DXImageTransform.Microsoft.AlphaImageLoader(src=\'' + url + '\', sizingMethod=\'scale\')"></div>'
        } else {
            this.innerHTML = '<img src="' + url + '" />';
        }
    },

    updateAttribution: function() {
        if (this.map && this.innerHTML) {
            this.div.style.right = "10px";
            this.div.style.bottom = "10px";
            this.div.innerHTML = this.innerHTML;
        }
    },

    CLASS_NAME: "OpenLayers.Control.Logo"
});

OpenLayers.Map = OpenLayers.Class(OpenLayers.Map, {
    minZoomLevel: 0,

    setCenter: function(lonlat, zoom, dragging, forceZoomChange) {
        if (zoom == null) {
            zoom = this.getZoom();
        }
        if (this.restrictedExtent) {
            var restrictedZoom = this.getZoomForExtent(this.restrictedExtent);
            if (zoom < restrictedZoom) {
                zoom = restrictedZoom;
            }
        }
        if (zoom < this.minZoomLevel) {
            zoom = this.minZoomLevel;
        }
        this.moveTo(lonlat, zoom, {
            'dragging': dragging,
            'forceZoomChange': forceZoomChange,
            'caller': 'setCenter'
        });
    },

    destroyShallow: function() {
        if (!this.unloadDestroy) {
            return false;
        }

        OpenLayers.Event.stopObserving(window, 'unload', this.unloadDestroy);
        this.unloadDestroy = null;

        if (this.updateSizeDestroy) {
            OpenLayers.Event.stopObserving(window, 'resize',
                                           this.updateSizeDestroy);
        } else {
            this.events.unregister("resize", this, this.updateSize);
        }

        this.paddingForPopups = null;

        if (this.eventListeners) {
            this.events.un(this.eventListeners);
            this.eventListeners = null;
        }
        this.events.destroy();
        this.events = null;
    }
});

OpenLayers.Layer.Markers.DragableMarkers = OpenLayers.Class(OpenLayers.Layer.Markers, {
    CLASS_NAME: "OpenLayers.Layer.Markers.DragMarkers",
    isDragging: false,
    selectedMarker: null,
    crossMarker: null,
    bouncingTimer: null,
    bouncingMarker: null,
    initialize: function(name, options) {
        OpenLayers.Layer.Markers.prototype.initialize.apply(this, arguments);
        this.events.addEventType("dragstart");
        this.events.addEventType("dragging");
        this.events.addEventType("dragend");

    },
    wireDragEvents: function() {
        if (this.dragMode == 'none') {
            return;
        }

        var size = new OpenLayers.Size(16, 16);
        var offset = new OpenLayers.Pixel(-(size.w / 2), -(size.h / 2));
        var icon = new OpenLayers.Icon(this.rootPath + 'theme/default/img/cross.gif', size, offset);
        this.crossMarker = new OpenLayers.Marker(new OpenLayers.LonLat(0, 0), icon);
        this.crossMarker.isShadow = true;
        this.addMarkerCore(this.crossMarker);
        this.crossMarker.display(false);
        this.events.includeXY = true;
        this.map.events.register('mousemove', this, function(e) {
            if (this.isDragging && this.selectedMarker != null) {
                var newXy = e.xy;
                var targetXy = this.map.getLayerPxFromViewPortPx(newXy);
                var markerXy = new OpenLayers.Pixel(targetXy.x, targetXy.y - 15);
                this.selectedMarker.moveTo(markerXy);
                this.crossMarker.moveTo(targetXy);
                this.crossMarker.display(true);

                if (this.selectedMarker.popup) {
                    this.selectedMarker.hidePopup();
                }
                this.events.triggerEvent("dragging", { evt: e, marker: this.selectedMarker });
            }
        });

        this.map.events.register('mouseup', this, function(evt) {
            this.isDragging = false;
            if (this.selectedMarker != null) {
                this.bouncingMarker = this.selectedMarker;
                var hoverPopup = this.selectedMarker.popup;
                this.selectedMarker = null;

                var bouncingHandler = function(e) {
                    var marker = this.marker;
                    var layer = this.layer;
                    var currentXy = layer.map.getLayerPxFromLonLat(marker.lonlat);
                    var targetXy = this.targetXY;
                    var timer = this.timer;
                    if (currentXy.y < targetXy.y) {
                        currentXy.y += 1;
                        marker.moveTo(currentXy);
                    } else {
                        marker.moveTo(targetXy);
                        this.layer.bouncingMarker = null;
                        window.clearInterval(layer.bouncingTimer);
                        layer.bouncingTimer = null;

                        if (this.layer.draggedEvent) {
                            var markerId = marker.id;
                            var lon = marker.lonlat.lon;
                            var lat = marker.lonlat.lat;
                            this.layer.map.isMarkerDragging = false;
                            __doPostBack(this.layer.map.uniqueId, 'MARKERDRAGGED^' + layer.id + '^' + markerId + '^' + lon + '^' + lat);
                        }
                        layer.events.triggerEvent("dragend", { targetXY: targetXy, marker: this.marker });
                    }
                }

                var targetXy = this.map.getLayerPxFromLonLat(this.crossMarker.lonlat);
                this.bouncingMarker.movePopup(targetXy);
                this.bouncingTimer = window.setInterval(OpenLayers.Function.bind(bouncingHandler, { marker: this.bouncingMarker, layer: this, targetXY: targetXy }), 5);
                this.crossMarker.display(false);
            }
        });
    },
    addMarker: function(marker) {
        this.addMarkerCore(marker);

        if (this.dragMode == 'drag') {
            marker.events.register('mousedown', { layer: this, selectedMarker: marker }, function(e) {
                this.layer.map.isMarkerDragging = true;
                this.layer.removeBouncingAnimation(this.layer);
                this.layer.isDragging = true;
                this.layer.selectedMarker = this.selectedMarker;
                this.layer.selectedMarker.map = this.layer.map;
                this.layer.crossMarker.map = this.layer.map;
                this.layer.crossMarker.moveTo(this.layer.getViewPortPxFromLonLat(this.selectedMarker.lonlat));
                this.layer.events.triggerEvent("dragstart", { evt: e, marker: this.layer.selectedMarker });
                OpenLayers.Event.stop(e);
            });
        }
    },
    addMarkerCore: function(marker) {
        OpenLayers.Layer.Markers.prototype.addMarker.apply(this, arguments);
    },
    removeBouncingAnimation: function(layer) {
        if (layer.bouncingTimer) {
            window.clearTimeout(layer.bouncingTimer);
        }

        if (layer.bouncingMarker) {
            layer.bouncingMarker = null;
        }
    }
});

OpenLayers.Tile.Image = OpenLayers.Class(OpenLayers.Tile.Image, {
    renderTile: function() {
        if (this.imgDiv == null) {
            this.initImgDiv();
        }

        this.imgDiv.viewRequestID = this.layer.map.viewRequestID;

        if (this.layer.url instanceof Array) {
            this.imgDiv.urls = this.layer.url.slice();
        }

        this.url = this.layer.getURL(this.bounds);

        if (this.url) {
            this.url += '&ZOOM=' + this.layer.map.getZoom();
        }

        OpenLayers.Util.modifyDOMElement(this.frame,
                                         null, this.position, this.size);

        var imageSize = this.layer.getImageSize();
        if (this.layerAlphaHack) {
            OpenLayers.Util.modifyAlphaImageDiv(this.imgDiv,
                    null, null, imageSize, this.url);
        } else {
            OpenLayers.Util.modifyDOMElement(this.imgDiv,
                    null, null, imageSize);
            this.imgDiv.src = this.url;
        }
        return true;
    },
    destroy: function() {
        if (this.imgDiv != null) {
            if (this.layerAlphaHack) {
                OpenLayers.Event.stopObservingElement(this.imgDiv.childNodes[0].id);
            }
            OpenLayers.Event.stopObservingElement(this.imgDiv.id);

            if (this.imgDiv.parentNode == this.frame) {
                this.frame.removeChild(this.imgDiv);
                this.imgDiv.map = null;
            }
            this.imgDiv.urls = null;
            this.imgDiv.src = OpenLayers.Util.getImagesLocation() + "blank.gif";
        }
        this.imgDiv = null;
        if ((this.frame != null) && (this.frame.parentNode == this.layer.div)) {
            this.layer.div.removeChild(this.frame);
        }
        this.frame = null;

        if (this.backBufferTile) {
            this.backBufferTile.destroy();
            this.backBufferTile = null;
        }

        this.layer.events.unregister("loadend", this, this.resetBackBuffer);

        OpenLayers.Tile.prototype.destroy.apply(this, arguments);
    }
});

OpenLayers.Handler.RegularPolygon = OpenLayers.Class(OpenLayers.Handler.RegularPolygon, {
    isMoved: false,
    down: function(evt) {
        this.moved = false;
        this.fixedRadius = !!(this.radius);
        var maploc = this.map.getLonLatFromPixel(evt.xy);
        this.origin = new OpenLayers.Geometry.Point(maploc.lon, maploc.lat);
        if (!this.fixedRadius || this.irregular) {
            this.radius = this.map.getResolution();
        }
        if (this.persist) {
            this.clear();
        }
        this.feature = new OpenLayers.Feature.Vector();
        this.createGeometry();
        this.layer.addFeatures([this.feature], { silent: true });
        this.layer.drawFeature(this.feature, this.style);
    },
    move: function(evt) {
        this.moved = true;
        var maploc = this.map.getLonLatFromPixel(evt.xy);
        var point = new OpenLayers.Geometry.Point(maploc.lon, maploc.lat);
        if (this.irregular) {
            var ry = Math.sqrt(2) * Math.abs(point.y - this.origin.y) / 2;
            this.radius = Math.max(this.map.getResolution() / 2, ry);
        } else if (this.fixedRadius) {
            this.origin = point;
        } else {
            this.calculateAngle(point, evt);
            this.radius = Math.max(this.map.getResolution() / 2,
                                   point.distanceTo(this.origin));
        }
        this.modifyGeometry();
        if (this.irregular) {
            var dx = point.x - this.origin.x;
            var dy = point.y - this.origin.y;
            var ratio;
            if (dy == 0) {
                ratio = dx / (this.radius * Math.sqrt(2));
            } else {
                ratio = dx / dy;
            }
            this.feature.geometry.resize(1, this.origin, ratio);
            this.feature.geometry.move(dx / 2, dy / 2);
        }
        this.layer.drawFeature(this.feature, this.style);
    },
    up: function(evt) {
       
        if (!this.moved) {
            this.layer.eraseFeatures([this.feature]);
        }
        this.finalize();
    }
});

OpenLayers.Popup.Anchored = OpenLayers.Class(OpenLayers.Popup.Anchored, {
    top: null,
    left: null,
    calculateNewPx: function(px) {
        var newPx = px.offset(this.anchor.offset);
        var size = this.size || this.contentSize;

        var top = (this.relativePosition.charAt(0) == 't');
        newPx.y += (top) ? -size.h : this.anchor.size.h;

        var left = (this.relativePosition.charAt(1) == 'l');
        newPx.x += (left) ? -size.w : this.anchor.size.w;

        this.top = top;
        this.left = left;

        return newPx;
    }
});

OpenLayers.Layer.VirtualEarth = OpenLayers.Class(OpenLayers.Layer.VirtualEarth, {
    loadMapObject: function() {
        var veDiv = OpenLayers.Util.createDiv(this.name);
        var sz = this.map.getSize();
        veDiv.style.width = sz.w + "px";
        veDiv.style.height = sz.h + "px";
        this.div.appendChild(veDiv);

        try { 
            this.mapObject = new VEMap(this.name);
        } catch (e) { }

        if (this.mapObject != null) {
            try { 
                this.mapObject.LoadMap(null, null, this.type, true);
                this.mapObject.AttachEvent("onmousedown", function() { return true; });
                this.mapObject.SetAnimationEnabled(false);
            } catch (e) { }
            this.mapObject.HideDashboard();
        }
        if (!this.mapObject ||
             !this.mapObject.vemapcontrol ||
             !this.mapObject.vemapcontrol.PanMap ||
             (typeof this.mapObject.vemapcontrol.PanMap != "function")) {

            this.dragPanMapObject = null;
        }
    },

    onMapResize: function() {
        if (this.map.baseLayer != null) {
            var newSize = this.map.getCurrentSize();
            this.mapObject.Resize(newSize.w, newSize.h);
        }
    },

    getMapObjectLonLatFromMapObjectPixel: function(moPixel) {
        var latlon;
        if (typeof VEPixel != 'underfined') {
            latlon = this.mapObject.PixelToLatLong(moPixel);
        }
        else {
            latlon = this.mapObject.PixelToLatLong(moPixel.x, moPixel.y);
        }
        return (new _xy1).Decode(latlon);
    }
});

OpenLayers.Layer.Google = OpenLayers.Class(OpenLayers.Layer.Google, {
    setMap: function(map) {
        OpenLayers.Layer.EventPane.prototype.setMap.apply(this, arguments);
        if (this.type != null) {
            this.mapObject.setMapType(this.type);
            this.map.events.register("moveend", this, this.setMapType);
        }
    }
});

OpenLayers.Icon = OpenLayers.Class(OpenLayers.Icon, {
    initialize: function(url, size, offset, calculateOffset) {
        this.url = url;
        this.size = (size) ? size : new OpenLayers.Size(20, 20);
        this.offset = offset ? offset : new OpenLayers.Pixel(-(this.size.w / 2), -(this.size.h / 2));
        this.calculateOffset = calculateOffset;
        var id = OpenLayers.Util.createUniqueID("OL_Icon_");
       
        this.size = size;
        this.imageDiv = OpenLayers.Util.createAlphaImageDiv(id);
    }
});

OpenLayers.Control.AnimationPan = OpenLayers.Class(OpenLayers.Control, {
    defaultHandlerOptions: {
        'single': true,
        'delay': 200
    },
    initialize: function() {
        this.handlerOptions = OpenLayers.Util.extend({}, this.defaultHandlerOptions);
        OpenLayers.Control.prototype.initialize.apply(this, arguments);
        this.handler = new OpenLayers.Handler.Click(this, { 'click': this.onClick }, this.handlerOptions);
    },
    onClientClick: function(context) { },
    onClick: function(evt) {
        if (this.onClientClick != "") {
            var context = {
                'animationPan': this,
                'evt': evt
            };
            this.onClientClick(context);
        }
        else {
            this.map.panTo(this.map.getLonLatFromPixel(evt.xy));
        }
    }
});