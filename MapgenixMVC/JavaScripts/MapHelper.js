var LoadingImageUrl = '/theme/default/img/loading.gif';
var MarkerOverlayEventPrefix = 'MARKEROVERLAYCLICK';
var TopZIndex = 100000;
var unitLengthsForExtent = { 'degrees': 180, 'm': 20001364.487263177, 'ft': 65621309.99999999 };

if (window.Node && Node.prototype && !Node.prototype.contains) {
    Node.prototype.contains = function (arg) {
        return !!(this.compareDocumentPosition(arg) & 16)
    }
}

var Class = {
    Create: function () {
        return function () {
            this.initialize.apply(this, arguments);
        }
    },

    Extent: function (destination, source) {
        for (var key in source) {
            destination[key] = source[key];
        }

        return destination;
    }
};
function handleMeasurements(feature) {
    var units = feature.units;
    var measure = feature.measure.toFixed(3);
    if (feature.order == 1) {
        html = String.format("<div style='font-size:small'>Total Length: <span style='color:red'>{0} {1}</span></div>", measure.toString(), units.toString());
    }
    else {
        html = String.format("<div style='font-size:small'>Total Area: <span style='color:red'>{0} {1}<sup>2<sup></span></div>", measure.toString(), units.toString());
    }
    if (feature.geometry.CLASS_NAME.indexOf('LineString') > -1) {
        var length = feature.geometry.components.length;
        lonLat = { lon: feature.geometry.components[length - 1].x, lat: feature.geometry.components[length - 1].y };
    }
    else {
        var length = feature.geometry.components[0].components.length;
        lonLat = { lon: feature.geometry.components[0].components[length - 2].x, lat: feature.geometry.components[0].components[length - 2].y };
    }
    var size = new OpenLayers.Size(450, 200);
    var popup = new OpenLayers.Popup.FramedCloud('DistancePopup', lonLat, size, html, null, true);
    popup.isAlphaImage = true; 
    this.map.addPopup(popup);
}
function GetShorterGeographyUnit(unit) {
    switch (unit) {
        case 'Feet': return 'ft';
        case 'Meter': return 'm';
        default: return 'degrees'
    }
}

function GetMaxExtentByUnits(unit) {
    var unitLength = unitLengthsForExtent[unit];
    if (unitLength)
        return new OpenLayers.Bounds(unitLength * -1, unitLength * -1, unitLength, unitLength);
}

function ConvertJsonToBounds(json) {
    return new OpenLayers.Bounds(json.left, json.bottom, json.right, json.top);
}

function ConvertJsonToLonLat(json) {
    return new OpenLayers.LonLat(json.x, json.y);
}

function ConvertJsonToIcon(json, imgRootPath) {
    var size;
    if (json.w == 0 && json.h == 0) {
        size = null;
    } else {
        size = new OpenLayers.Size(json.w, json.h);
    }

    if (json.url == '') {
        json.url = imgRootPath + 'theme/default/img/marker_blue.gif';
    }

    var icon = new OpenLayers.Icon(json.url, size);

    if (size) {
        icon.offset = new OpenLayers.Pixel(CalculateOffset(json.ox, size.w), CalculateOffset(json.oy, size.h));
    }

    return icon;
}

function ConvertJsonToSize(json) {
    if (json.w != 0 && json.h != 0) {
        return new OpenLayers.Size(json.w, json.h);
    } else {
        return null;
    }
}

function CalculateOffset(offset, width) {
    if (offset == NaN || offset.toString().toUpperCase() == 'NAN') {
        return -width / 2;
    } else {
        return offset;
    }
}

function ConvertBoundsToString(bounds) {
    return (bounds.left + ',' + bounds.bottom + ',' + bounds.right + ',' + bounds.top);
}

function ConvertLonLatToString(lonlat) {
    return lonlat.lon + ',' + lonlat.lat;
}

function CreateImageNode(id, infos, parentW, parentH, localUrl) {
    var w = infos.w == 0 ? 128 : infos.w;
    var h = infos.h == 0 ? 15 : infos.h;
    var url = infos.uri;
    var img = NewDomNode('IMG');

    img.style.position = 'absolute';
    img.style.zIndex = TopZIndex;
    img.style.width = w + 'px';
    img.style.height = h + 'px';
    img.style.left = ((parentW / 2) - (w / 2)) + 'px';
    img.style.top = ((parentH / 2) - (h / 2)) + 'px';
    img.style.display = 'none';
    img.id = id;
    img.src = (!url || url == '') ? localUrl : url;
    return img;
}

function HideContextMenus() {
    if (CurrentContextMenu) {
        CurrentContextMenu.display(false);
    }
}

function NewDomNode(tagName) {
    return document.createElement(tagName);
}

function GetDomNode(id) {
    return document.getElementById(id);
}

function CheckObjectExists(obj) {
    if (obj && obj != 'undefined' && obj != undefined) {
        return true;
    } else {
        return false;
    }
}

function IsBoundsValidated(extent) {
    return (extent.left != null
	    && extent.bottom != null
	    && extent.right != null
	    && extent.top != null);
}

function AddMarkersByAjax(map, json) {
    var markerOverlays = eval('(' + json + ')');
    if (markerOverlays && markerOverlays.length != 0) {
        for (var i = 0; i < markerOverlays.length; i++) {
            var layer = map.getLayer(markerOverlays[i].id);
            if (layer) {
                RemoveMarkerPopups(layer);
                var markersCount = layer.markers.length;
                for (var j = 0; j < markersCount; j++) {
                    var removeMarker = layer.markers[0];
                    layer.removeMarker(removeMarker);
                    removeMarker.destroy();
                }

                var hasCtx = false;
                var rootPath = layer.rootPath;
                if (markerOverlays[i].markers && markerOverlays[i].markers.length != 0) {
                    for (var j = 0; j < markerOverlays[i].markers.length; j++) {
                        var markerJson = markerOverlays[i].markers[j];
                        var markerCreator = new Marker(markerJson, rootPath);
                        markerCreator.overlayId = layer.id;
                        markerCreator.mapUid = map.uniqueId;
                        markerCreator.mapCid = map.clientId;
                        markerCreator.currentZoom = map.getZoom();
                        if (markerJson.contextMenu && markerJson.contextMenu != 'undefined' && markerJson.contextMenu.items.length != 0) {
                            markerCreator.setContextMenu(markerJson);
                            hasCtx = true;
                        }

                        var marker = markerCreator.element;

                        if (markerOverlays[i].iscluster && markerOverlays[i].iscluster == '1') {
                            if (markerJson.popup && markerJson.popup != 'undefined') {

                                marker.events.register('mouseover', marker, function (evt) {
                                    if (this.id) {
                                        var url = "popup_GeoResource.axd?oid=" + layer.id + "&id=" + this.id + "&t=" + (new Date()).toString();
                                        var mapparser = eval(map.clientId + ".GetMapParser();");
                                        url += '&pageName=' + mapparser.pageName;
                                        url += '&clientId=' + map.clientId;
                                        url += '&zoom=' + (parseInt(map.getZoom()) + 1)
                                        var m = this;
                                        var httpRequest = new OpenLayers.Ajax.Request(url, { 'method': 'get', 'onComplete': function (transport) {
                                            if (transport.status == 200) {
                                                var resdpopup = eval('(' + transport.responseText + ')');
                                                if (resdpopup && resdpopup.html != '') {
                                                    m.popupJson = resdpopup;
                                                    m.popupJson.id = m.id + '_popup';
                                                    m.popupDelay = markerJson.popupDelay;
                                                    var p = contains(map.popups, m.popupJson.id);
                                                    if (p == '-1') {
                                                        m.initializePopup(map);
                                                    }
                                                    m.Popup = p;
                                                    m.popup.show();
                                                    m.events.register('mouseover', m, function (evt) { this.showPopup(); });
                                                    m.events.register('mouseout', m, function (evt) { this.hidePopup(); });
                                                }
                                            }
                                        }
                                        });
                                    }
                                });
                            }
                        } else if (markerJson.popup && markerJson.popup != 'undefined' && markerJson.popup.html && markerJson.popup.html != '') {
                            marker.popupJson = markerJson.popup;
                            marker.popupDelay = markerJson.popupdelay;
                            marker.initializePopup(map);
                            marker.events.register('mouseover', marker, function (evt) { this.showPopup(); });
                            marker.events.register('mouseout', marker, function (evt) { this.hidePopup(); });
                        }

                        marker = GetHookedClickEventMarker(marker, layer);
                        layer.addMarker(marker);

                        if (hasCtx) {
                            layer.div.oncontextmenu = function () { return false; };
                        } else {
                            layer.div.oncontextmenu = null;
                        }
                    }
                }
            }
        }
    }
}

function contains(popupsArray,obj){
    var i = popupsArray.length; 
      while (i--) { 
        if (popupsArray[i].id === obj) { 
          return popupsArray[i]; 
        } 
      } 
      return '-1'; 
}

function RemoveMarkerPopups(layer) {
    for (var i = 0; i < layer.markers.length; i++) {
        if (layer.markers[i].popup) {
            var removePopup = layer.markers[i].popup;
            layer.map.removePopup(removePopup);
            removePopup.destroy();
        }
    }
}

function GetHookedClickEventMarker(marker, markerOverlay) {
    marker.parentId = markerOverlay.id;
    if (markerOverlay.click) {
        marker.icon.imageDiv.style.cursor = 'pointer';
        marker.events.register('click', marker, function (evt) {
            __doPostBack(this.map.uniqueId, MarkerOverlayEventPrefix + '^' + this.parentId + '^' + this.id);
        });
    }
    return marker;
}

function HasObject(array, value) {
    for (var i = 0; i < array.length; i++) {
        if (array[i] == value) {
            return true;
        }
    }

    return false;
}

function CreateAllMaps() {
    for (var i = 0; i < MultiMap.length; i++) {
        eval('var parser = parser' + MultiMap[i]);
        parser.createMap();
    }
}

function CleanMap(mapId) {
    var map = eval(mapId);
    if (map) {
        map.Destory();
    }
}

function CleanAllMaps() {
    for (var i = 0; i < MultiMap.length; i++) {
        CleanMap(MultiMap[i]);
    }
}

function keepsession() {
    window.setInterval("IssueKeepSessionRequest()", 900000);
}

function IssueKeepSessionRequest() {
    OpenLayers.Request.issue({ url: 'session_GeoResource.axd' });
}

function OnMapRefresh(map) {
    for (var i in map.layers) {
        var layer = map.layers[i];
        var updateInterval = layer.autoRefreshMilliseconds;
        var tick = layer.tick;
        if (tick && updateInterval && updateInterval != 0) {
            var timer = window.setInterval(Function.createDelegate({ 'args': 'AutoRefresh', 'id': layer.id }, function (evt) {
                var args = this.args + "_" + this.id;
                eval("CallServerMethod" + map.clientId + "(args, map.clientId)");
            }), updateInterval);
        }
    }
}

function AutoRefreshCompleted(args, context) {
    eval('var map = ' + context + ".GetOpenLayersMap();");
    var argsArray = args.split("_");
    var currentExtentArray = argsArray[0];
    var markersJson = argsArray[1];
    for (var i in map.layers) {
        var updateInterval = map.layers[i].autoRefreshMilliseconds;
        var layer = map.layers[i];
        if (updateInterval && updateInterval != 0) {
            if (layer instanceof OpenLayers.Layer.WMS) {
                layer.redraw(true);
            }
            else if (layer.CLASS_NAME == "OpenLayers.Layer.Markers.DragMarkers") {
                var serial = Sys.Serialization.JavaScriptSerializer;
                var json = serial.deserialize(markersJson);

                for (var i = 0; i < layer.markers.length; i++) {
                    if (layer.markers[i].popup) {
                        layer.markers[i].popup.destroy();
                        map.removePopup(layer.markers[i].popup);
                    }
                }

                if (layer.markers != null) {
                    while (layer.markers.length > 0) {
                        layer.markers[0].destroy();
                        layer.removeMarker(layer.markers[0]);
                    }
                }

                layer.wireDragEvents();

                var hasCtx = false;
                var rootPath = layer.rootPath;
                if (json && json != 'undefined') {
                    if (json.markers && json.markers.length != 0) {
                        for (var j = 0; j < json.markers.length; j++) {
                            var markerJson = json.markers[j];
                            if (!markerJson.visible) {
                                continue;
                            }

                            var markerCreator = new Marker(markerJson, rootPath);
                            markerCreator.overlayId = layer.id;
                            markerCreator.mapUid = map.uniqueId;
                            markerCreator.mapCid = map.clientId;
                            markerCreator.currentZoom = map.getZoom();

                            if (markerJson.contextMenu && markerJson.contextMenu != 'undefined' && markerJson.contextMenu.items.length != 0) {
                                markerCreator.setContextMenu(markerJson);
                                hasCtx = true;
                            }

                            var marker = markerCreator.element;

                            if (markerJson.popup && markerJson.popup != 'undefined' && markerJson.popup.html && markerJson.popup.html != '') {
                                marker.popupJson = markerJson.popup;
                                marker.popupDelay = markerJson.popupdelay;
                                marker.initializePopup(map);
                                marker.events.register('mouseover', marker, function (evt) { this.showPopup(); });
                                marker.events.register('mouseout', marker, function (evt) { this.hidePopup(); });
                            }

                            marker = GetHookedClickEventMarker(marker, layer);
                            layer.addMarker(marker);

                            if (hasCtx) {
                                layer.div.oncontextmenu = function () { return false; };
                            } else {
                                layer.div.oncontextmenu = null;
                            }
                        }
                        json.markers = null;
                    }
                }
            }
            else {
                eval('parser' + context).sendMarkersRequest();
            }
        }
    }


    if (currentExtentArray != null && currentExtentArray != "") {
        var currentExtent = currentExtentArray.split(",");
        if (currentExtent.length == 4) {
            var extent = new OpenLayers.Bounds(currentExtent[0], currentExtent[3], currentExtent[2], currentExtent[1]);
            if (IsBoundsValidated(extent)) {
                var center = extent.getCenterLonLat();
                var zoom = map.getZoomForExtent(extent);
                map.events.triggerEvent("maprefreshcompleted", { center: center, zoom: zoom });
            }
        }
    }
}

var Overlay = Class.Create();
var SphericalOverlay = Class.Create();
var WmsOverlay = Class.Create();
var ArcGISServerRestOverlay = Class.Create();
var LayerOverlay = Class.Create();
var MarkerOverlay = Class.Create();
var SimpleMarkerOverlay = Class.Create();
var Control = Class.Create();
var Popup = Class.Create();
var Marker = Class.Create();

Class.Extent(Overlay.prototype, {
    id: null,
    options: {},
    element: null,
    initialize: function (json) {
        this.options = {};
        this.setCommonOptions(json);
    },
    setCommonOptions: function (json) {
        this.id = json.Id;
        this.name = json.name;
        if (this.name == 'undefined' || !this.name) {
            this.name = '';
        }
        this.options.isBaseLayer = json.isBaseLayer;
        this.options.visibility = json.visibility;
        this.options.initVisibility = json.visibility;
        this.options.name = this.name;
        this.options.opacity = json.opacity;
        this.options.displayInLayerSwitcher = json.displayInLayerSwitcher;
        this.autoRefreshMilliseconds = json.AutoRefreshMilliseconds;
        this.tick = json.tick;
    }
});

Class.Extent(SphericalOverlay.prototype, Overlay.prototype);
Class.Extent(SphericalOverlay.prototype, {
    json: null,
    type: null,
    overlayType: null,
    initialize: function (json) {
        this.json = json;
        this.setCommonOptions(json);
        this.type = json.otype;
        this.options.type = eval(json.type);
        this.options.sphericalMercator = true;
        this.options.maxResolution = 156543.0339;
        this.options.maxExtent = new OpenLayers.Bounds(-20037508.34, -20037508.34, 20037508.34, 20037508.34);
    },
    createGoogleOverlay: function () {
        this.element = new OpenLayers.Layer.Google(this.name, this.options);
        this.setTickEventArguments();
    },
    createVirtualEarthOverlay: function () {
        this.element = new OpenLayers.Layer.VirtualEarth(this.name, this.options);
        this.setTickEventArguments();
    },
    createYahooOverlay: function () {
        this.element = new OpenLayers.Layer.Yahoo(this.name, this.options);
        this.setTickEventArguments();
    },
    createOSMOverlay: function () {
        this.element = new OpenLayers.Layer.OSM(this.name);
        this.element.setTileSize(new OpenLayers.Size(this.json.w, this.json.h));
        this.setTickEventArguments();
    },
    createCustomerOverlay: function () {
        this.element = this.getCustomerOverlay(this.json);
        this.setTickEventArguments();
    },
    setTickEventArguments: function () {
        this.element.autoRefreshMilliseconds = this.autoRefreshMilliseconds;
        this.element.tick = this.tick;
    },
    getCustomerOverlay: function (json) { return null },
    createOverlay: function () {
        switch (this.type.toUpperCase()) {
            case "GOOGLE": this.createGoogleOverlay(); break;
            case "VIRTUALEARTH": this.createVirtualEarthOverlay(); break;
            case "YAHOO": this.createYahooOverlay(); break;
            case "OSM": this.createOSMOverlay(); break;
            default: this.createCustomerOverlay(); break;
        }
        this.element.id = this.id;
    }
});

Class.Extent(ArcGISServerRestOverlay.prototype, Overlay.prototype);
Class.Extent(ArcGISServerRestOverlay.prototype, {
    url: null,
    params: {},
    pageName: null,
    initialize: function (json, pageName) {
        this.setCommonOptions(json);
        if (!this.url) { this.url = json.uri }
        this.pageName = pageName;

        this.options.ratio = 1;
        this.options.singleTile = (parseInt(json.singleTile) == 1);
        this.options.tileSize = new OpenLayers.Size(json.w, json.h);
        this.options.transitionEffect = json.transitionEffect;
        this.options.layername = json.layername;
        this.setParameters(json);
    },
    setParameters: function (json) {
        for (var key in json.parameters) {
            this.params[key] = json.parameters[key];
        }
    },
    createArcGISServerRest: function () {
        this.element = new OpenLayers.Layer.ArcGIS93Rest(this.name, this.url, this.params, this.options);
        this.element.id = this.id;
        this.element.autoRefreshMilliseconds = this.autoRefreshMilliseconds;
        this.element.tick = this.tick;
    }
});

Class.Extent(WmsOverlay.prototype, Overlay.prototype);
Class.Extent(WmsOverlay.prototype, {
    url: null,
    params: {},
    pageName: null,
    initialize: function (json, pageName) {
        this.params = {};
        this.setCommonOptions(json);
        if (!this.url) {
            this.url = json.uris;
        }

        this.pageName = pageName;

        if (json.buffer && json.buffer != 'undefined') {
            this.options.buffer = json.buffer;
        } else {
            this.options.buffer = 0;
        }
        this.options.wrapDateLine = json.wrapDateLine == "1" ? true : false;
        this.options.ratio = 1;
        //this.options.displayOutsideMaxExtent = true;
        this.options.singleTile = (parseInt(json.singleTile) == 1);
        this.options.tileSize = new OpenLayers.Size(json.w, json.h);
        this.options.transitionEffect = json.transitionEffect;
        this.setAlphaChanel();
        this.setParameters(json);
    },
    setResolutions: function (resolutions) {
        this.options.resolutions = resolutions;
    },
    setAlphaChanel: function () {
        var versionString = navigator.appVersion.split("MSIE");
        var version = parseFloat(versionString[1]);
        if (version == 6) {
            this.options.alpha = true;
        }
    },
    setProjection: function (srs) {
        if (srs && srs != 'undefined') {
            this.element.projection = srs;
        }
    },
    setWebImageFormat: function (format) {
        if (format && typeof (format) != 'undefined') {
            this.element.params.FORMAT = format;
        }
    },
    setParameters: function (json) {
        this.setCustomerParams(json);
    },
    createWms: function () {
        this.element = new OpenLayers.Layer.WMS(this.name, this.url, this.params, this.options);
        this.element.autoRefreshMilliseconds = this.autoRefreshMilliseconds;
        this.element.tick = this.tick;
        this.element.id = this.id;
    },
    hookLoadingImage: function (mapClientId) {
        this.element.mapClientId = mapClientId;
        this.element.events.register('loadstart', this.element, function (evt) {
            var loadingDiv = GetDomNode('loading' + this.mapClientId);
            if (loadingDiv) {
                loadingDiv.style.display = 'block';
            }
        });
        this.element.events.register('loadend', this.element, function (evt) {
            var loadingDiv = GetDomNode('loading' + this.mapClientId);
            if (loadingDiv) {
                loadingDiv.style.display = 'none';
            }
        });
    },
    setCustomerParams: function (json) {
        for (var key in json.parameters) {
            if (key.toUpperCase() == 'SRS') {
                this.options.projection = new OpenLayers.Projection(json.parameters[key.toString()]);
            } else {
                this.params[key] = json.parameters[key];
            }
        }
    },
    setDefaultParams: function (json) {
        this.params.overlayid = escape(json.Id);
        this.params.format = json.imageFormat;
        this.params.cacheid = escape(json.clientCache.cacheId);
        this.params.pageName = escape(this.pageName);
        this.params.exceptions = null;
        this.params.request = null;
        this.params.service = null;
        this.params.version = null;
        this.params.style = null;
    }
});

Class.Extent(LayerOverlay.prototype, WmsOverlay.prototype);
Class.Extent(LayerOverlay.prototype, {
    url: 'tile_GeoResource.axd',
    setParameters: function (json) {
        if (json.singleThread) {
            this.url = 'singletile_GeoResource.axd';
        }
        WmsOverlay.prototype.setParameters.apply(this, arguments);
        this.setDefaultParams(json);
    }
});

Class.Extent(MarkerOverlay.prototype, Overlay.prototype);
Class.Extent(MarkerOverlay.prototype, {
    element: null,
    createMarkerOverlay: function () {
        this.element = new OpenLayers.Layer.Markers(this.name, this.options);
        this.element.autoRefreshMilliseconds = this.autoRefreshMilliseconds;
        this.element.tick = this.tick;
        this.element.id = this.id;
    },
    hookEvent: function (flag) {
        if (flag) {
            this.element.click = true;
        }
    }
});

Class.Extent(SimpleMarkerOverlay.prototype, MarkerOverlay.prototype);
Class.Extent(SimpleMarkerOverlay.prototype, {
    createMarkerOverlay: function () {
        this.element = new OpenLayers.Layer.Markers.DragableMarkers(this.name, this.options);
        this.element.id = this.id;
        this.element.isSimpleMarkerOverlay = true;
        this.element.autoRefreshMilliseconds = this.autoRefreshMilliseconds;
        this.element.tick = this.tick;
    },
    setOptions: function (json) {
        this.options.dragMode = json.dragMode;
        this.options.draggedEvent = json.draggedEvent;
    },
    hookMarkersJson: function (markersJson) {
        this.element.markersJson = markersJson;
    },
    hookEvent: function (flag) {
        if (flag) {
            this.element.click = true;
        }
    }
});

Class.Extent(Control.prototype, {
    element: null,
    initialize: function (name, options) {
        if (options.enabled) {
            this.element = eval('new OpenLayers.Control.' + name + '()');
            for (var key in options) {
                if (key != 'enabled')
                    this.element[key] = options[key];
            }
        }
    }
});

Class.Extent(Popup.prototype, {
    element: null,
    initialize: function (json, anchor) {
        var lonlat = ConvertJsonToLonLat(json.lonlat);
        var size = ConvertJsonToSize({ 'w': json.w, 'h': json.h });
        if (json.popupType == 'NormalPopup') {
            this.element = new OpenLayers.Popup.Anchored(json.id, lonlat, size, json.html, anchor, json.closable);
            this.setBorder(json);
            this.setBgColor(json);
            this.setBgImg(json);
        } else if (json.popupType == 'CloudPopup') {
            this.element = new OpenLayers.Popup.FramedCloud(json.id, lonlat, size, json.html, anchor, json.closable);
            this.setAlphaChanel();
        } else {
            this.element = this.getCustomerPopup(json);
        }
        this.element.panMapIfOutOfView = json.autopan;
        this.element.autoSize = json.autosize;
        this.setOffset(json);
    },
    setAlphaChanel: function () {
        var versionString = navigator.appVersion.split("MSIE");
        var version = parseFloat(versionString[1]);
        if (version == 6) {
            this.element.isAlphaImage = true;
        }
    },
    setBorder: function (json) {
        this.element.border = 'solid ' + json.bw + 'px ' + json.bdcolor;
    },
    setBgColor: function (json) {
        if (json.bgcolor.toUpperCase == 'TRANSPARENT') {
            this.element.backgroundColor = '';
            this.element.groupDiv.style.backgroundColor = null;
        } else {
            this.element.backgroundColor = json.bgcolor;
        }
    },
    setBgImg: function (json) {
        if (CheckObjectExists(json.backImg)) {
            this.element.div.style.backgroundImage = 'url("' + json.backImg + '")';
        }
    },
    setVisible: function (visible) {
        if (visible) {
            this.element.show();
        } else {
            this.element.hide();
        }
    },
    setOffset: function (json) {
        if (json.ox && json.ox != 'undefined' && json.ox != 0) {
            this.element.offsetX = json.ox;
        }
        if (json.oy && json.oy != 'undefined' && json.oy != 0) {
            this.element.offsetY = json.oy;
        }
    },
    getCustomerPopup: function (json) {
        return null;
    }
});

Class.Extent(Marker.prototype, {
    element: null,
    overlayId: null,
    mapUid: null,
    mapCid: null,
    currentZoom: null,
    initialize: function (json, imgRootPath) {
        var popupJson = json.popup;
        var lonlat = ConvertJsonToLonLat(json.lonlat);
        var icon = ConvertJsonToIcon(json.img, imgRootPath);
        this.element = new OpenLayers.Marker.GeoMarker(json.id, lonlat, icon);
        if (json.opacity && json.opacity != 'undefined') {
            this.element.setOpacity(json.opacity);
        }
    },
    setContextMenu: function (json) {
        this.element.contextMenu = json.contextMenu;
        this.element.overlayId = this.overlayId;
        this.element.mapUid = this.mapUid;
        this.element.mapCid = this.mapCid;
        this.element.currentZoom = this.currentZoom;
        this.element.events.register('mousedown', this.element, function (evt) {
            if (evt && evt.button == 2) {
                var lonlat = this.lonlat;
                var target = this.CLASS_NAME + '|' + this.overlayId + '|' + this.currentZoom;

                HideContextMenus();
                var contextMenu = new ContextMenu(this.contextMenu, lonlat, target, this.mapUid);
                for (var i = 0; i < this.contextMenu.items.length; i++) {
                    contextMenu.addItem(this.contextMenu.items[i]);
                }
                contextMenu.moveTo(evt.clientX, evt.clientY);
                contextMenu.display(true);
                document.getElementById(this.mapCid).parentNode.appendChild(contextMenu.div);
                CurrentContextMenu = contextMenu;
                return false;
            }
        });
    }
});

