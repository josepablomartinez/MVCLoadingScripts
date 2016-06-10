var MultiMap = new Array();
var CurrentContextMenu = null;
var CircleSide = 32;
var DefaultLogoUrl = 'logo_GeoResource.axd';
var TransparentStyle = { 'fillOpacity': 0, 'strokeOpacity': 0 };
var IconPath = 'theme/default/img/';
var CssPath = 'theme/default/style.css';
var LoadingImageUrl = 'theme/default/img/loading.gif';
var WmsUri = "tile_GeoResource.axd";
var MaxNum = 1000000000;
var MinNum = -1000000000;
var MinScale = 442943842.5;
var CurrentMapClientId = '';
var SelectedFeature = null;
var PointRadius = 6

var mapParser = Class.Create();
Class.Extent(mapParser.prototype, {
    json: null,
    map: null,
    pageName: null,
    markerOverlayIds: null,
    simpleMarkerOverlayIds: null,
    editOverlay: null,
    paintControls: null,
    load: true,
    initialize: function (json) {
        OpenLayers.ImgPath = json.rootPath + IconPath;
        this.json = json;
        this.pageName = json.pageName;
        this.markerOverlayIds = new Array();
        this.simpleMarkerOverlayIds = new Array();
        this.initFeatureStyle();
        this.initMap(json);
    },
    destroy: function () {
        var clientId = this.map.clientId;
        if (this.map && window.navigator.appName.toUpperCase() != 'NETSCAPE') {
            this.map.destroy();
            this.map = null;
        }
        if (this.json) {
            this.json = null;
        }
        if (this.markerOverlayIds) {
            this.markerOverlayIds = null;
        }
        if (document.getElementById(clientId)) {
            document.getElementById(clientId).innerHTML = '';
        }
    },
    initFeatureStyle: function (style) {
        for (var key in this.json.editOverlay.style) {
            OpenLayers.Feature.Vector.style['default'][key] = this.json.editOverlay.style[key];
        }
        var selectedStyle = this.getDefaultSelectStyle();
        for (var key in selectedStyle) {
            OpenLayers.Feature.Vector.style['select'][key] = selectedStyle[key];
        }
    },
    getDefaultSelectStyle: function () {
        var defaultSelectStyle = {};
        defaultSelectStyle.fillColor = '#ff6500';
        defaultSelectStyle.strokeColor = '#ff6500';
        defaultSelectStyle.pointRadius = '6';
        defaultSelectStyle.fillOpacity = '0.3';
        defaultSelectStyle.strokeWidth = '1';
        defaultSelectStyle.strokeOpacity = '0.6';
        return defaultSelectStyle;
    },
    initMap: function (json) {
        var options = { 'theme': json.rootPath + CssPath, 'numZoomLevels': 20 };
        options.units = GetShorterGeographyUnit(json.units);
        if (options.units == 'degrees') {
            options.maxExtent = new OpenLayers.Bounds(-180, -90, 180, 90);
        } else {
            options.maxExtent = new OpenLayers.Bounds(MinNum, MinNum, MaxNum, MaxNum);
        }
        options.controls = [];
        if (json.restrictedExtent) {
            options.restrictedExtent = ConvertJsonToBounds(json.restrictedExtent);
        }

        options.resolutions = json.resolutions;
        this.map = new OpenLayers.Map(json.cid, options);
        this.map.uniqueId = json.uid;
        this.map.clientId = json.cid;
        this.map.extentChanged = json.extentChanged;

        this.setCursor();
        this.map.events.register('mouseup', this, function (e) { this.setCursor(); });

        if (json.onClientExtentChanged && json.onClientExtentChanged != '') {
            this.map.events.register('moveend', this, eval(json.onClientExtentChanged));
        }
        this.map.events.register('moveend', this, this.extentChanged);
        this.map.events.register('moveend', this, this.extentChangedPostBack);
        this.map.events.register('move', this, function (evt) { HideContextMenus(); });
        this.map.events.addEventType("maprefreshcompleted");

        this.initLoadingControl(json);

        if (!HasObject(MultiMap, json.cid)) {
            MultiMap.push(json.cid);
        }

        this.map.click = json.click;
        this.map.events.register('click', this.map, function (e) { HideContextMenus(); });

        if (json.onClientClick && json.onClientClick != '') {
            this.map.events.register('click', this.map, eval(json.onClientClick));
        }
        this.map.events.register('click', this.map, function (e) {
            if (this.click && !this.isMarkerDragging) {
                var lonlat = this.getLonLatFromViewPortPx(e.xy);
                __doPostBack(json.uid, 'CLICK^' + lonlat.lon + '^' + lonlat.lat);
            }
        });

        this.initDblClick(json);

        if (json.contextMenu && json.contextMenu != 'undefined') {
            this.map.contextMenu = json.contextMenu;
            document.getElementById(this.map.clientId).oncontextmenu = function () { return false; };
            this.map.events.register('mousedown', this.map, function (evt) {
                HideContextMenus();
                if (evt && evt.button == 2) {
                    var lonlat = this.getLonLatFromPixel(evt.xy);
                    var target = this.CLASS_NAME + '|' + this.id;

                    var contextMenu = new ContextMenu(this.contextMenu, lonlat, target, this.uniqueId);
                    for (var i = 0; i < this.contextMenu.items.length; i++) {
                        contextMenu.addItem(this.contextMenu.items[i]);
                    }
                    contextMenu.moveTo(evt.clientX, evt.clientY);
                    contextMenu.display(true);
                    document.getElementById(this.clientId).parentNode.appendChild(contextMenu.div);
                    CurrentContextMenu = contextMenu;
                }
            });
        }
    },
    initDblClick: function (json) {
        var dblClick = null;
        var dblClientClick = null;
        if (json.dblClick && json.dblClick != '') {
            dblClick = function (evt) {
                var lonlat = this.map.getLonLatFromViewPortPx(evt.xy);
                __doPostBack(json.uid, 'DBLCLICK^' + lonlat.lon + '^' + lonlat.lat);
            };
        }

        if (json.onClientDoubleClick && json.onClientDoubleClick != '') {
            dblClientClick = eval(json.onClientDoubleClick);
        }

        var dblClickEventHandlers = new Array();
        if (dblClientClick)
            dblClickEventHandlers.push({ obj: this, func: dblClientClick });
        if (dblClick)
            dblClickEventHandlers.push({ obj: this, func: dblClick });

        var dblClickMain = this.getEventHandlerFromArray(dblClickEventHandlers);
        if (dblClickMain) {
            this.map.events.register('dblclick', this.map, dblClickMain);
            this.hasDblClick = true;
        }
    },
    initLoadingControl: function (json) {
        var loadObjId = 'loading' + json.cid;
        var loadObj = document.getElementById(loadObjId);
        var loadJsonObj = json.controls.LoadingImage;
        if (loadJsonObj.enabled && (loadObj == null || loadObj == 'undefined')) {
            loadObj = CreateImageNode(loadObjId, loadJsonObj, parseInt(this.map.div.clientWidth), parseInt(this.map.div.clientHeight), json.rootPath + LoadingImageUrl);
            this.map.div.appendChild(loadObj);
        }
    },
    initOverlays: function (json) {
        var layers = new Array();

        if (CheckObjectExists(json.backgroundOverlay)) {
            var layer = null;
            var oType = json.backgroundOverlay.otype.toUpperCase();
            if (oType == 'LAYER' && (json.backgroundOverlay.visibility || !json.backgroundOverlay.isDefault)) {
                layer = this.getLayerOverlay(json.backgroundOverlay, json.cid);
            } else if (oType != 'LAYER') {
                layer = this.getLayerByJson(json.backgroundOverlay, json.cid);
            }
            if (layer) {
                layers.push(layer);
            }
        }

        if (CheckObjectExists(json.staticOverlay)) {
            if (json.staticOverlay.visibility || !json.staticOverlay.isDefault) {
                var staticOverlay = this.getLayerOverlay(json.staticOverlay, json.cid);
                layers.push(staticOverlay);
            }
        }

        for (var i = 0; i < json.layers.length; i++) {
            if (json.layers[i].isBaseLayer) {
                var overlayJson = json.layers[i];
                var overlay = this.getLayerByJson(overlayJson, json.cid);
                if (overlay) {
                    layers.push(overlay);
                }
                json.layers.splice(i, 1);
            }
        }

        if (CheckObjectExists(json.dynamicOverlay)) {
            if (json.dynamicOverlay.visibility || !json.dynamicOverlay.isDefault) {
                var dynamicOverlay = this.getLayerOverlay(json.dynamicOverlay, json.cid);
                layers.push(dynamicOverlay);
            }
        }

        var layersLength = json.layers.length;
        for (var i = 0, layerIndex = 0; i < layersLength; i++) {
            if (!json.layers[layerIndex].isBaseLayer) {
                var overlayJson = json.layers[layerIndex];
                var otype = overlayJson.otype.toUpperCase();
                if (otype == 'LAYER') {
                    var overlay = this.getLayerByJson(overlayJson, json.cid);
                    if (overlay) {
                        layers.push(overlay);
                        json.layers.splice(layerIndex, 1);
                    }
                }
            }
            else {
                layerIndex++;
            }
        }

        if (CheckObjectExists(json.markerOverlay)) {
            if (json.markerOverlay.visibility || !json.markerOverlay.isDefault) {
                var markerOverlay = this.getMarkerOverlay(json.markerOverlay);
                markerOverlay.gridSize = json.markerOverlay.gridSize;
                layers.push(markerOverlay);
                this.markerOverlayIds.push(json.markerOverlay.Id);
            }
        }

        for (var i = 0; i < json.layers.length; i++) {
            var overlayJson = json.layers[i];
            var otype = overlayJson.otype.toUpperCase();
            var overlay = this.getLayerByJson(overlayJson, json.cid);
            if (overlay) {
                layers.push(overlay);
                if (otype == 'MARKERS') this.markerOverlayIds.push(overlayJson.Id);
                else if (otype == 'SIMPLEMARKERS') this.simpleMarkerOverlayIds.push(overlayJson.Id);
            }
        }

        if (layers.length != 0) {
            var hasBaseLayer = false;
            for (var i = 0; i < layers.length; i++) {
                if (layers[i].isBaseLayer) {
                    hasBaseLayer = true;
                    break;
                }
            }
            if (!hasBaseLayer) this.addBlankWmsLayer();
            this.onOverlaysDrawing(layers);
            this.map.addLayers(layers);
         
        } else {
            this.addBlankWmsLayer();
        }
    },
    getLayerByJson: function (json, clientId) {
        var otype = json.otype.toUpperCase();
        var layer = null;
        if (otype == 'WMS') {
            layer = this.getWmsOverlay(json);
        }
        else if (otype == "ARCGIS") {
            layer = this.getArcGISServerRestOverlay(json);
        }
        else if (otype == 'LAYER') {
            layer = this.getLayerOverlay(json, clientId);
        } else if (otype == "MARKERS") {
            layer = this.getMarkerOverlay(json);
            layer.gridSize = json.gridSize;
        } else if (otype == "SIMPLEMARKERS") {
            layer = this.getSimpleMarkerOverlay(json);
        } else {
            layer = this.getSphericalOverlay(json);
        }

        return layer;
    },
    initControls: function (json) {
        var ctrJson = json.controls;
        for (var key in ctrJson) {
            var enabled = ctrJson[key].enabled;
            var control = null;
            if (key == 'LoadingImage' || key == 'Measure') {
                continue;
            } else if (key == 'Logo' && enabled) {
                var url = DefaultLogoUrl;
                if (ctrJson[key].url && ctrJson[key].url != '') {
                    url = ctrJson[key].url;
                }
                control = new OpenLayers.Control.Logo(url);
                control.id = key;
            } else if (enabled) {
                var options = ctrJson[key];
                if (json.cursor && key == 'Navigation') {
                    options.defaultCursor = json.cursor;
                    options.defaultCursorUrl = json.cursorUrl;
                    if (options.zoomBoxKeyMask == 2) {
                        options.handleRightClicks = true;
                    }

                } else if (key == 'LayerSwitcher') {
                    options.activeColor = ctrJson[key].activeColor;
                    if (ctrJson[key].onLayerChanged != '') {
                        eval("options.onLayerChanged = " + ctrJson[key].onLayerChanged);
                    }
                } else if (key == 'AnimationPan') {
                    if (ctrJson[key].onClientClick != '') {
                        eval("options.onClientClick=" + ctrJson[key].onClientClick);
                    }
                }
                control = new Control(key, options).element;
                control.id = key;
            }

            if (control && !this.map.getControl(key)) {
                this.map.addControl(control);

                if (key == 'OverviewMap' || key == 'LayerSwitcher') {
                    control.maximizeControl();
                    if (key == 'LayerSwitcher') {
                        control.baseLbl.innerHTML = OpenLayers.i18n("BaseOverlays");

                        if (ctrJson[key].baseTitle != '') {
                            control.baseLbl.innerHTML = OpenLayers.i18n(ctrJson[key].baseTitle);
                        }

                        if (ctrJson[key].switchTitle != '') {
                            control.dataLbl.innerHTML = OpenLayers.i18n(ctrJson[key].switchTitle);
                        }
                    }
                } else if (key == 'Navigation') {
                    control.dragPan.defaultCursor = control.defaultCursor;
                    control.dragPan.defaultCursorUrl = control.defaultCursorUrl;
                    if (ctrJson[key].wheelDisabled) {
                        control.disableZoomWheel();
                    }
                    if (this.hasDblClick) {
                        control.handlers.click.double = false;
                        control.handlers.click.stopDouble = false;
                    }
                } else if (key == 'AnimationPan') {
                    control.handler.delay = options['delay'];
                    control.activate();
                }
            }
        }
        this.map.addControl(new OpenLayers.Control.NavigationHistory('NavHistory'));
    },
    initExtent: function (json) {
        if (!this.map.baseLayer || this.map.layers.length == 0) {
            return;
        }
        if (json.zoom >= 0) {
            if (json.zoom > this.map.getNumZoomLevels() - 1) {
                json.zoom = this.map.getNumZoomLevels() - 1;
            }
            this.map.setCenter(new OpenLayers.LonLat(json.centerX, json.centerY), json.zoom);
        } else {
            var extent = ConvertJsonToBounds(json.currentExtent);
            if (IsBoundsValidated(extent)) {
                var center = extent.getCenterLonLat();
                var zoom = this.map.getZoomForExtent(extent);
                this.map.setCenter(center, zoom);
            }
        }
    },
    initPopups: function (json) {
        if (json.popups) {
            for (var i = 0; i < json.popups.length; i++) {
                var popupCreator = new Popup(json.popups[i], null);
                this.map.addPopup(popupCreator.element);
                var isPopupVisible = json.popups[i].visibility;
                popupCreator.setVisible(isPopupVisible);
                if (isPopupVisible && typeof (OnPopupShowing) != 'undefined' && OnPopupShowing != null) {
                    OnPopupShowing.call(popupCreator.element, { isLeft: popupCreator.element.left, isTop: popupCreator.element.top });
                }
            }
        }
    },
    initDrawControls: function () {
        this.editOverlay = new OpenLayers.Layer.Vector('EditOverlay');
        this.editOverlay.displayInLayerSwitcher = false;
        this.map.addLayer(this.editOverlay);

        var onClientDrawEnd = null, onClientEditEnd = null, onServerTrackShapeEnd = null;
        if (this.json.onClientDrawEnd && this.json.onClientDrawEnd != '') {
            onClientDrawEnd = eval(this.json.onClientDrawEnd);
        }
        if (this.json.onClientEditEnd && this.json.onClientEditEnd != '') {
            onClientEditEnd = eval(this.json.onClientEditEnd);
        }
        if (this.json.hasTrackShapeEnd && this.json.hasTrackShapeEnd != '') {
            onServerTrackShapeEnd = function (evt) { __doPostBack(this.map.uniqueId, 'FINISHEDTRACKSHAPE^'); };
        }

        var drawEndHandlers = new Array();
        var editEndHandlers = new Array();
        if (onClientDrawEnd)
            drawEndHandlers.push({ obj: this, func: onClientDrawEnd });
        if (onClientEditEnd)
            editEndHandlers.push({ obj: this, func: onClientEditEnd });
        if (onServerTrackShapeEnd) {
            drawEndHandlers.push({ obj: this, func: onServerTrackShapeEnd });
            editEndHandlers.push({ obj: this, func: onServerTrackShapeEnd });
        }

        var onDrawEnd = this.getEventHandlerFromArray(drawEndHandlers);
        var onEditEnd = this.getEventHandlerFromArray(editEndHandlers);

        var modifyMode = this.getModifyMode(this.json.editOverlay.editSettings);
        var optionsEdit = { mode: modifyMode, id: 'Modify' };
        var optionsCircle = { handlerOptions: { sides: CircleSide }, id: 'Circle' };
        var optionsEllipse = { handlerOptions: { sides: CircleSide, irregular: true }, id: 'Ellipse' };
        var optionsRectangle = { handlerOptions: { irregular: true }, id: 'Rectangle' };
        this.paintControls = {
            Point: new OpenLayers.Control.DrawFeature(this.editOverlay, OpenLayers.Handler.Point, { id: 'Point' }),
            Line: new OpenLayers.Control.DrawFeature(this.editOverlay, OpenLayers.Handler.Path, { id: 'Line' }),
            Polygon: new OpenLayers.Control.DrawFeature(this.editOverlay, OpenLayers.Handler.Polygon, { id: 'Polygon' }),
            Square: new OpenLayers.Control.DrawFeature(this.editOverlay, OpenLayers.Handler.RegularPolygon, { id: 'Square' }),
            Circle: new OpenLayers.Control.DrawFeature(this.editOverlay, OpenLayers.Handler.RegularPolygon, optionsCircle),
            Ellipse: new OpenLayers.Control.DrawFeature(this.editOverlay, OpenLayers.Handler.RegularPolygon, optionsEllipse),
            Rectangle: new OpenLayers.Control.DrawFeature(this.editOverlay, OpenLayers.Handler.RegularPolygon, optionsRectangle),
            Modify: new OpenLayers.Control.ModifyFeature(this.editOverlay, optionsEdit),
            Normal: null
        };
        this.addControlsInJson(this.paintControls, onDrawEnd, onEditEnd);
    },
    getModifyMode: function (modifySettings) {
        var modifyMode = 0;
        if (modifySettings.resize) modifyMode |= OpenLayers.Control.ModifyFeature.RESIZE
        if (modifySettings.drag) modifyMode |= OpenLayers.Control.ModifyFeature.DRAG;
        if (modifySettings.rotate) modifyMode |= OpenLayers.Control.ModifyFeature.ROTATE;
        if (modifySettings.reshape) modifyMode |= OpenLayers.Control.ModifyFeature.RESHAPE;
        return modifyMode;
    },
    initEditOverlay: function (json) {
        if (json && json != 'undefined' && json.features && json.features.length != 0) {
            if (this.editOverlay == null) {
                this.initDrawControls();
            }
            var featuresObj = json.features;
            var featureArray = new Array();

            for (var i = 0; i < featuresObj.length; i++) {
                var featureObj = featuresObj[i];
                var featureId = featureObj.id;
                var featureWkt = featureObj.wkt;
              
                var fieldValues = new Array();
                for (var key in featureObj.values) {
                    fieldValues.push({ Key: key, Value: featureObj.values[key] });
                }

                var feature = new OpenLayers.Format.WKT().read(featureWkt);
                feature.fid = featureId;
                feature.fieldValues = fieldValues;
                featureArray.push(feature);
            }
            this.editOverlay.addFeatures(featureArray);
        }
    },
    initHighlightOverlay: function (json) {
        if (json && json.features && json.features.length != 0) {
            var highlightOverlay = new OpenLayers.Layer.Vector('HighlightOverlay');
            highlightOverlay.displayInLayerSwitcher = false;
            highlightOverlay.id = 'HighlightOverlay';
            this.map.addLayer(highlightOverlay);
            highlightOverlay.events.includeXY = true;
            highlightOverlay.events.fallThrough = true;
            json.style.pointRadius = PointRadius;
            json.highlightStyle.pointRadius = PointRadius;
            highlightOverlay.style = json.style;
            var options = { hover: true, selectStyle: json.highlightStyle };
            var selectControl = new OpenLayers.Control.SelectFeature(highlightOverlay, options);
            this.map.addControl(selectControl);
            selectControl.activate();
            selectControl.onSelect = function (feature) {
                SelectedFeature = feature;
            };
            selectControl.onUnselect = function () {
                SelectedFeature = null;
            };
            var featureList = new Array();
            var toolTip = json.toolTip;
            for (var i = 0; i < json.features.length; i++) {
                var wkt = json.features[i].wkt;
                var feature = new OpenLayers.Format.WKT().read(wkt);
                feature.id = json.features[i].id;
                var fieldValues = new Array();
                for (var key in json.features[i].values) {
                    fieldValues.push({ Key: key, Value: json.features[i].values[key] });
                }
                feature.fieldValues = fieldValues;
                if (toolTip && toolTip != '' && toolTip != 'undefined')
                    feature.toolTip = toolTip;

                featureList.push(feature);
            }

            highlightOverlay.addFeatures(featureList);
            highlightOverlay.events.register('click', highlightOverlay, function (e) { HideContextMenus(); });

            highlightOverlay.events.register('mousedown', highlightOverlay, function () {
                if (SelectedFeature != null && SelectedFeature.layer != null) {
                    selectControl.unhighlight(SelectedFeature);
                }
                selectControl.deactivate();
            });

            highlightOverlay.events.register('mouseup', highlightOverlay, function () {
                selectControl.activate();
            });

            if (json.onClientClick && json.onClientClick != '') {
                highlightOverlay.events.register('click', highlightOverlay, eval(json.onClientClick));
            }

            if (json.click) {
                highlightOverlay.events.register('click', highlightOverlay, function (evt) {
                    var lonlat = this.getLonLatFromViewPortPx(evt.xy);
                    var featureExtent = this.getDataExtent();
                    if (featureExtent.containsLonLat(lonlat)) {
                        if (SelectedFeature)
                            __doPostBack(this.map.uniqueId, 'HIGHLIGHTOVERLAYCLICK^' + lonlat.lon + '^' + lonlat.lat + '^' + SelectedFeature.id);
                    }
                });
            }

            if (CheckObjectExists(json.contextMenu)) {
                highlightOverlay.ctxJson = json.contextMenu;

                if (highlightOverlay.div.oncontextmenu != 'undefined')
                    highlightOverlay.div.oncontextmenu = function () { return false; };

                highlightOverlay.events.register('mousedown', highlightOverlay, function (evt) {
                    HideContextMenus();
                    if (evt && evt.button == 2) {
                        var lonlat = this.map.getLonLatFromPixel(evt.xy);
                        var featureExtent = this.getDataExtent();
                        if (featureExtent.containsLonLat(lonlat)) {
                            var target = this.CLASS_NAME + '|' + this.id;
                            var contextMenu = new ContextMenu(this.ctxJson, lonlat, target, this.map.uniqueId);
                            for (var i = 0; i < this.ctxJson.items.length; i++) {
                                contextMenu.addItem(this.ctxJson.items[i]);
                            }
                            contextMenu.moveTo(evt.clientX, evt.clientY);
                            contextMenu.display(true);
                            document.getElementById(this.map.clientId).appendChild(contextMenu.div);
                            CurrentContextMenu = (contextMenu == undefined ? null : contextMenu);
                        }
                    }
                });
            }
        }
    },
    initBaseOverlay: function (baseOverlayId) {
        if (baseOverlayId && baseOverlayId != 'undefined') {
            var layer = this.map.getLayer(baseOverlayId);
            if (!layer) {
                layer = this.map.getLayer('_blankWms');
            }
            if (layer) {
                this.map.setBaseLayer(layer);
                if (!layer.initVisibility && layer.initVisibility != 'undefined' && layer.initVisibility != undefined) {
                    layer.setVisibility(false);
                }
            }
        }
    },
    initMapMode: function () {
        var trackMode = this.json.editOverlay.mode;
        if (this.editOverlay == null) {
            if (trackMode == 'None') return;
            else this.initDrawControls();
        }
        for (var key in this.paintControls) {
            if (this.paintControls[key]) {
                if (key == trackMode) this.paintControls[key].activate();
                else this.paintControls[key].deactivate();
            }
        }
    },
    addBlankWmsLayer: function () {
        var blankWms = new OpenLayers.Layer.WMS('_blankWms', '', {}, { 'displayInLayerSwitcher': false, 'initVisibility': false, 'visibility': false });
        blankWms.id = '_blankWms';
        this.map.addLayer(blankWms);
    },
    releasePageLoad: function () {
        Sys.Application.remove_load(CreateAllMaps);
    },
    extentChanged: function (e) {
        if (!this.map.panByPopup) this.sendMarkersRequest();
        if (typeof (OnExtentChanged) != 'undefined') OnExtentChanged(this.map);
    },
    extentChangedPostBack: function (e) {
        if (!this.load)
            if (this.map.extentChanged && this.map.extentChanged != null)
                __doPostBack(this.map.uniqueId, 'EXTENTCHANGED');
    this.load = false;
},
sendMarkersRequest: function () {
    for (var i = 0; i < this.markerOverlayIds.length; i++) {
        var markerOverlayId = this.markerOverlayIds[i];
        var overlay = this.map.getLayer(markerOverlayId);
        var url = this.getMarkerRequestUrl(markerOverlayId, overlay.gridSize);
        if (url) {
            CurrentMapClientId = this.map.clientId;
            var httpRequest = new OpenLayers.Ajax.Request(url, { 'method': 'get', 'onComplete': function (transport) {
                if (transport.status == 200) {
                    var map = eval('parser' + CurrentMapClientId + '.map');
                    AddMarkersByAjax(map, transport.responseText);
                }
            }
            });
        }
    }
},
addControlsInJson: function (controls, onDrawEnd, onEditEnd) {
    for (var key in controls) {
        if (controls[key]) {
            if (key == 'Modify') {
                if (onEditEnd) {
                    controls[key].onModificationEnd = onEditEnd;
                }
            } else if (key != 'Normal') {
                if (onDrawEnd) {
                    controls[key].featureAdded = onDrawEnd;
                }
            }
            this.map.addControl(controls[key]);
        }
    }
},
addSimpleMarkers: function () {
    for (var i = 0; i < this.simpleMarkerOverlayIds.length; i++) {
        var layer = this.map.getLayer(this.simpleMarkerOverlayIds[i]);
        var hasCtx = false;
        var rootPath = layer.rootPath;
        if (layer.isSimpleMarkerOverlay && layer.isSimpleMarkerOverlay != undefined) {
            if (layer.markersJson && layer.markersJson.length != 0) {
                for (var j = 0; j < layer.markersJson.length; j++) {
                    var markerJson = layer.markersJson[j];
                    if (!markerJson.visible) {
                        continue;
                    }

                    var markerCreator = new Marker(markerJson, rootPath);
                    markerCreator.overlayId = layer.id;
                    markerCreator.mapUid = this.map.uniqueId;
                    markerCreator.mapCid = this.map.clientId;
                    markerCreator.currentZoom = this.map.getZoom();
                    if (markerJson.contextMenu && markerJson.contextMenu != 'undefined' && markerJson.contextMenu.items.length != 0) {
                        markerCreator.setContextMenu(markerJson);
                        hasCtx = true;
                    }

                    var marker = markerCreator.element;

                    if (markerJson.popup && markerJson.popup != 'undefined' && markerJson.popup.html && markerJson.popup.html != '') {
                        marker.popupJson = markerJson.popup;
                        marker.popupDelay = markerJson.popupdelay;
                        marker.initializePopup(this.map);
                        marker.events.register('mouseover', marker, function (evt) { this.showPopup(); });
                        marker.events.register('mouseout', marker, function (evt) { this.hidePopup(); });
                    }

                    layer.wireDragEvents();
                    marker = GetHookedClickEventMarker(marker, layer);
                    layer.addMarker(marker);

                    if (hasCtx) {
                        layer.div.oncontextmenu = function () { return false; };
                    } else {
                        layer.div.oncontextmenu = null;
                    }
                }
                layer.markersJson = null;
            }
        }
    }
},
setCursor: function () {
    if (this.json.cursor == 'Custom') {
        this.map.div.style.cursor = 'url(' + this.json.cursorUrl + ')';
    } else {
        this.map.div.style.cursor = this.json.cursor;
    }
},
getSphericalOverlay: function (json) {
    var creator = new SphericalOverlay(json);
    creator.createOverlay();
    return creator.element;
},
getWmsOverlay: function (json) {
    var creator = new WmsOverlay(json, this.pageName);
    creator.setResolutions(this.map.resolutions);
    creator.createWms();
    creator.setProjection(json.SRS);
    creator.setWebImageFormat(json.format);
    creator.hookLoadingImage(this.map.clientId);
    return creator.element;
},
getArcGISServerRestOverlay: function (json) {
    var creator = new ArcGISServerRestOverlay(json, this.pageName);
    creator.createArcGISServerRest();
    return creator.element;
},
getLayerOverlay: function (json, mapClientId) {
    var creator = new LayerOverlay(json, this.pageName);
    creator.setResolutions(this.map.resolutions);
    creator.createWms();
    creator.hookLoadingImage(this.map.clientId);
    creator.element.mergeNewParams({ 'clientid': mapClientId, 'extra': json.extra });
    creator.element.projection = new OpenLayers.Projection(json.projection);
    return creator.element;
},
getMarkerOverlay: function (json) {
    var creator = new MarkerOverlay(json);
    creator.createMarkerOverlay();
    creator.hookEvent(json.click);
    creator.element.rootPath = this.json.rootPath;
    return creator.element;
},
getSimpleMarkerOverlay: function (json) {
    var creator = new SimpleMarkerOverlay(json);
    creator.setOptions(json);
    creator.createMarkerOverlay();
    creator.hookEvent(json.click);
    creator.hookMarkersJson(json.markers);
    creator.element.rootPath = this.json.rootPath;
    return creator.element;
},
getMarkerRequestUrl: function (markerOverlayId, gridSize) {
    var url = null;
    if (gridSize == undefined) {
        gridSize = 0;
    }
    if (this.markerOverlayIds.length != 0) {
        url = 'markers_GeoResource.axd?';
        url += 'overlayIds=' + markerOverlayId;
        url += '&extent=' + ConvertBoundsToString(this.map.getExtent());
        url += '&level=' + (parseInt(this.map.getZoom()) + 1);
        url += '&pageName=' + this.pageName;
        url += '&clientId=' + this.json.cid;
        url += '&scale=' + this.map.getScale();
        url += '&gridSize=' + gridSize;
        url += '&extra=' + Math.random().toString();
    }
    return url;
},
getResolutionList: function (scales, units) {
    var resolutions = new Array();
    for (var i = 0; i < scales.length; i++) {
        var resolution = OpenLayers.Util.getResolutionFromScale(scales[i], units);
        resolutions.push(resolution);
    }
    return resolutions;
},
getEventHandlerFromArray: function (handlers) {
    var eventHandler = null;
    if (handlers.length != 0) {
        eventHandler = function (evt) {
            var result = null;
            for (var i = 0; i < handlers.length; i++) {
                var sender = handlers[i].obj;
                var func = handlers[i].func;
                result = func.apply(sender, [evt]);
                if (!result && result != undefined) break;
            }
        };
    }

    return eventHandler;
},
registerChangeBaseLayerEvent: function (json) {
    if (!this.map.baseLayer) {
        return;
    }
    var lastBaseLayer = document.getElementById('__BASELAYERCHANGEDSTATUSSTORAGE' + this.map.clientId);
    if (json.onClientBaseOverlayChanged && json.onClientBaseOverlayChanged != '')
        this.map.events.register('changebaselayer', this.map, eval(json.onClientBaseOverlayChanged));
    if (json.baseOverlayChanged) {
        this.map.events.register('changebaselayer', this.map, function (e) {
            __doPostBack(json.uid, 'BASELAYERCHANGED' + '^' + this.baseLayer.name + '^' + lastBaseLayer.value);
        });
    };
    lastBaseLayer.value = this.map.baseLayer.name;
},
registerWindowSizeChangedEvent: function (map) {
    var registerEvents = function (element, eventName, eventHandler) {
        if (element.addEventListener) {
            element.addEventListener(eventName, eventHandler, false);
        } else if (element.attachEvent) {
            element.attachEvent('on' + eventName, eventHandler);
        } else {
            eval("element.on" + eventName + "=eventHandler");
        }
    };

    var resizeVeMap = function (veLayer, newWidth, newHeight) {
        veLayer.mapelement.style.width = newWidth;
        veLayer.mapelement.style.height = newHeight;
        veLayer.Resize();
    }

    var onMapResize = function (map) {
        for (var i = 0; i < map.layers.length; i++) {
            if (map.layers[i].CLASS_NAME == 'OpenLayers.Layer.VirtualEarth')
                resizeVeMap(map.layers[i].mapObject, map.div.clientWidth, map.div.clientHeight);
        }
    };

    registerEvents(window, 'resize', function () { onMapResize(map); });
},
initMeasureMode: function () {
    var measureControl = this.json.controls.Measure;
    if (measureControl != null && measureControl != undefined && measureControl.enabled) {
        var measureType = measureControl.measureType;
        if (measureType == "None") {
            reutrn;
        }
        else {
            this.initMeasureControls();
        }
        for (var key in this.measureControls) {
            if (this.measureControls[key]) {
                if (key == measureType) this.measureControls[key].activate();
                else this.measureControls[key].deactivate();
            }
        }
    }
},
initMeasureControls: function () {
    var pathMeasureOption = { id: 'PathMeasure', persist: true };
    var polygonMeasureOption = { id: 'PolygonMeasure', persist: true };
    var units = this.map.getUnits();
    var geodesic = this.json.controls.Measure.geodesic
    OpenLayers.Util.extend(pathMeasureOption, { geodesic: geodesic });
    OpenLayers.Util.extend(polygonMeasureOption, { geodesic: geodesic });
    var displaySystem = this.json.controls.Measure.displaySystem;
    if (displaySystem != 'metric') {
        OpenLayers.Util.extend(pathMeasureOption, { displaySystem: displaySystem });
        OpenLayers.Util.extend(polygonMeasureOption, { displaySystem: displaySystem });
    }
    if (typeof getDefaultMeasureStyleMap == 'function') {
        var styleMap = getDefaultMeasureStyleMap();
        if (styleMap != null) {
            OpenLayers.Util.extend(pathMeasureOption, { handlerOptions: { layerOptions: { styleMap: styleMap}} });
            OpenLayers.Util.extend(polygonMeasureOption, { handlerOptions: { layerOptions: { styleMap: styleMap}} });
        }
    }
    this.measureControls = {
        PathMeasure: new OpenLayers.Control.Measure(OpenLayers.Handler.Path, pathMeasureOption),
        PolygonMeasure: new OpenLayers.Control.Measure(OpenLayers.Handler.Polygon, polygonMeasureOption)
    };
    for (var key in this.measureControls) {
        var control = this.measureControls[key];
        this.map.addControl(control);
        control.events.on({
            "measure": handleMeasurements,
            "measurepartial": function (event) {
                var parser = eval('parser' + this.map.div.id);
                var popup = parser.getPopup('DistancePopup');
                if (popup != null) {
                    event.object.map.removePopup(popup);
                    popup.destroy();
                }
            },
            scope: control
        });
    }
},
getPopup: function (id) {
    var returnPopup = null;
    for (var i = 0, len = this.map.popups.length; i < len; i++) {
        var popup = this.map.popups[i];
        if (popup.id == id) {
            returnPopup = popup;
            break;
        }
    }
    return returnPopup;
},
onOverlaysDrawing: function (layers) {
    if (typeof (OnOverlaysDrawing) == 'function') {
        OnOverlaysDrawing(layers);
    }
},
onMapCreating: function (map) {
    if (typeof (OnMapCreating) == 'function') {
        OnMapCreating(map);
    }
},
onMapCreated: function (map) {
    if (typeof (OnMapCreated) == 'function') {
        OnMapCreated(map);
    }
},
onMapRefresh: function (map) {
    if (typeof (OnMapRefresh) == 'function') {
        OnMapRefresh(map);
    }
},
createMap: function () {
    this.onMapCreating(this.map);
    this.initOverlays(this.json);
    this.initBaseOverlay(this.json.baselayerid);
    this.initExtent(this.json);
    this.addSimpleMarkers();
    this.initPopups(this.json);
    this.initControls(this.json);
    this.initHighlightOverlay(this.json.highlightOverlay);
    this.initEditOverlay(this.json.editOverlay);
    this.initMapMode();
    this.initMeasureMode();
    this.registerChangeBaseLayerEvent(this.json);
    
    this.releasePageLoad();
    this.onMapCreated(this.map);
    this.onMapRefresh(this.map);
    this.map.panByPopup = false;
}
});

