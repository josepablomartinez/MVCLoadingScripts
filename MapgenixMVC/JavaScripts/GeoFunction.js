var Mapgenix = Class.Create();
Class.Extent(Mapgenix.prototype, {
    clientId: null,
    initialize: function(clientId) {
        this.clientId = clientId;
    },

    SendMarkersRequest: function() {
        var parser = this.GetMapParser();
        parser.sendMarkersRequest();
    },

    GetMapParser: function() {
        eval('var parser = parser' + this.clientId);
        return parser;
    },

    GetCurrentExtent: function() {
        return this.GetMapParser().map.getExtent();
    },

    GetZoomLevel: function() {
        return this.GetOpenLayersMap().getZoom() + 1;
    },

    SetCenter: function(lon, lat) {
        var mapcontrol = this.GetMapParser().map;
        mapcontrol.setCenter(new OpenLayers.LonLat(lon, lat), mapcontrol.getZoom(), false, false);
    },

    ZoomIn: function() {
        this.GetMapParser().map.zoomIn();
    },

    ZoomOut: function() {
        this.GetMapParser().map.zoomOut();
    },

    ZoomToExtent: function(extent) {
        this.GetMapParser().map.zoomToExtent(extent);
    },

    PanByScreenPixel: function(x, y) {
        this.GetMapParser().map.pan(x, y);
    },

    PanToWorldCoordinate: function(x, y) {
        var xy = new OpenLayers.LonLat(x, y);
        this.GetMapParser().map.panTo(xy);
    },

    ZoomToPreviousExtent: function() {
        var nav = this.GetMapParser().map.getControlsByClass('OpenLayers.Control.NavigationHistory');
        nav[0].previousTrigger();
    },

    ZoomToNextExtent: function() {
        var nav = this.GetMapParser().map.getControlsByClass('OpenLayers.Control.NavigationHistory');
        nav[0].nextTrigger();
    },

    ToggleExtent: function() {
        var nav = this.GetMapParser().map.getControlsByClass('OpenLayers.Control.NavigationHistory');
        var current = nav[0].previousStack.shift();
        var state = nav[0].previousStack.shift();
        if (state != undefined) {
            nav[0].previousStack.unshift(state);
            nav[0].previousStack.unshift(current);
            nav[0].nextStack.unshift(current);
            nav[0].previousStack.unshift(state);
            nav[0].restoring = true;
            nav[0].restore(state);
            nav[0].restoring = false;
            nav[0].onNextChange(nav[0].nextStack[0], nav[0].nextStack.length);
            nav[0].onPreviousChange(
                nav[0].previousStack[1], nav[0].previousStack.length + 1
            );
        } else {
            nav[0].previousStack.unshift(current);
        }
        return state;
    },
    ResetLayerZIndex: function() {
        
        var layers = this.GetMapParser().map.layers;
        for (var i = 0; i < layers.length; i++) {
            var layer = layers[i];
            if (layer.CLASS_NAME == 'OpenLayers.Layer.Vector') {
                layer.setZIndex(100 + i);
            } else if (layer.CLASS_NAME == 'OpenLayers.Layer.Markers' || layer.CLASS_NAME == 'OpenLayers.Layer.Markers.DragMarkers') {
                layer.setZIndex(200 + i);
            } else {
                layer.setZIndex(i);
            }
        }
    },
    SetDrawMode: function(drawMode) {
        eval('var parser = parser' + this.clientId);
        if (!parser.editOverlay) {
            parser.initDrawControls();
        }
        var map = parser.map;
        if (drawMode == 'Normal') {
            for (var key in parser.paintControls) {
                if (key != 'Normal') {
                    parser.paintControls[key].deactivate();
                }
            }
        } else {
            for (var key in parser.paintControls) {
                var control = map.getControl(key);
                if (key == drawMode && drawMode != 'Normal') {
                    control.activate();
                }
                else if (key != 'Normal') {
                    control.deactivate();
                }
            }
        }
        this.ResetLayerZIndex();
    },
    SetMeasureMode: function(measureMode) {
        var parser = eval('parser' + this.clientId);
        if (!parser.measureControls) {
            parser.initMeasureControls();
        }
        var map = parser.map;
        if (measureMode == 'Normal') {
            for (var key in parser.measureControls) {
                parser.measureControls[key].deactivate();
            }
            var popup = parser.getPopup('DistancePopup');
            if (popup != null) {
                parser.map.removePopup(popup);
                popup.destroy();
            }
        }
        else {
            for (var key in parser.measureControls) {
                var control = map.getControl(key);
                if (key == measureMode) {
                    control.activate();
                }
                else {
                    control.deactivate();
                }
            }
        }
        this.ResetLayerZIndex();
    },
    SetEditSetting: function(editMode) {
        this.SetDrawMode('Modify');
        eval('var parser = parser' + this.clientId);
        parser.paintControls['Modify'].deactivate();
        parser.paintControls['Modify'].mode = editMode;
        parser.paintControls['Modify'].activate();
    },

    ClearFeatures: function() {
        eval('var parser = parser' + this.clientId);
        if (parser.editOverlay.features.length != 0) {
            parser.editOverlay.removeFeatures(parser.editOverlay.features);
        }
    },

    CancelLatestFeature: function() {
        eval('var parser = parser' + this.clientId);
        if (!parser.editOverlay) {
            alert('no features left here.');
            return;
        }
        if (parser.editOverlay.features.length > 0) {
            if (parser.paintControls['Modify'].active) {
                var modifyEnd = parser.paintControls['Modify'].onModificationEnd;
                parser.paintControls['Modify'].onModificationEnd = function() { };
                parser.paintControls['Modify'].deactivate();
                parser.paintControls['Modify'].onModificationEnd = modifyEnd;
            }
            parser.editOverlay.removeFeatures([parser.editOverlay.features[parser.editOverlay.features.length - 1]]);
        }
        else {
            alert('no features left here.');
        }
    },

    PrintMap: function() {
        var controlIds = '';
        for (var controlIndex = 0; controlIndex < this.GetOpenLayersMap().controls.length; controlIndex++) {
            controlIds += this.GetOpenLayersMap().controls[controlIndex].id + ',';
        }
        if (controlIds.length != 0) {
            controlIds = controlIds.substr(0, controlIds.length - 1);
        }
        window.open('print_GeoResource.axd?containerid=' + this.GetOpenLayersMap().div.id + '&controls=' + controlIds, 'PrintPreview');
    },

    GetCenter: function() {
        return this.GetOpenLayersMap().getCenter();
    },

    GetOpenLayersMap: function() {
        return this.GetMapParser().map;
    },

    GetCurrentBaseLayer: function() {
        return this.GetOpenLayersMap().baseLayer;
    },

    SetBaseLayer: function(layerId) {
        var oplMap = this.GetOpenLayersMap();
        oplMap.setBaseLayer(oplMap.getLayer(layerId));
    },

    SetCurrentBackgroundMapType: function(backgroundMapType) {
        if (!backgroundMapType) {
            alert("Background Map Type Error.");
        } else {
            try {
                var baseLayer = this.GetCurrentBaseLayer();
                baseLayer.type = backgroundMapType;

                if (baseLayer.CLASS_NAME == "OpenLayers.Layer.Google") {
                    baseLayer.setMapType();
                } else if (baseLayer.CLASS_NAME == "OpenLayers.Layer.VirtualEarth") {
                    baseLayer.mapObject.SetMapStyle(backgroundMapType)
                } else if (baseLayer.CLASS_NAME == "OpenLayers.Layer.Yahoo") {
                    baseLayer.mapObject.setMapType(backgroundMapType)
                }
            } catch (e) {
            }
        }
    },
    Destory: function() {
        var mapParser = this.GetMapParser();
        mapParser.destroy();
    }
});

var EditSettings = { Reshape: 1, Resize: 2, Rotate: 4, Drag: 8 };

theForm.onsubmit = function() {
    for (var i = 0; i < MultiMap.length; i++) {
        var cid = MultiMap[i];
        eval('var parser = parser' + cid);
        SynchronizeMapInfo(parser);
        SynchronizeVectors(parser.editOverlay, parser.map.clientId, parser.paintControls);
        SynchronizeSimpleMarkers(parser);
    }
    return true;
}

function SynchronizeMapInfo(parser) {
    var zoom = parser.map.getZoom();
    var result = ConvertBoundsToString(parser.map.getExtent());
    result += ',' + parser.map.getScale();
    result += ',' + ConvertLonLatToString(parser.map.getCenter());
    result += ',' + parser.map.getZoom();
    result += ',' + parser.map.div.offsetWidth;
    result += ',' + parser.map.div.offsetHeight;
    result += ',' + parser.map.baseLayer.id;
    result += ',';

    for (var i = 0; i < parser.map.layers.length; i++) {
        var layer = parser.map.layers[i];
        if (!layer.isBaseLayer) {
            var visible = layer.getVisibility();
            result += layer.id + '^' + (visible ? '1' : '0') + '%';
        }
    }

    result += ',';
    for (var i = 0; i < parser.map.popups.length; i++) {
        var popup = parser.map.popups[i];
        if (!popup.isMarkerHoverPopup || popup.isMarkerHoverPopup == 'undefined') {
            result += popup.id;
            if (popup.visible()) {
                result += '^1%';
            } else {
                result += '^0%';
            }
        }
    }

    document.getElementById("__RestoreStatusField" + parser.map.clientId).value = result;
}

function SynchronizeVectors(editOverlay, clientId, paintControls) {
    if (editOverlay) {
        if (editOverlay.features.length >= 0) {
            paintControls['Modify'].deactivate();

            var jsonObject = { features: null };
            var submitArray = new Array();
            for (var i = 0; i < editOverlay.features.length; i++) {
                var featureId = editOverlay.features[i].fid;
                var featureWkt = editOverlay.features[i].geometry.toString();

                var fieldValues = new Array();
                if (editOverlay.features[i].fieldValues) {
                    for (var j = 0; j < editOverlay.features[i].fieldValues.length; j++) {
                        var fieldValue = editOverlay.features[i].fieldValues[j];
                        fieldValues.push({ 'Key': fieldValue.Key, 'Value': fieldValue.Value });
                    }
                }

                submitArray.push({ id: featureId, wkt: featureWkt, fieldValues: fieldValues });
            }

            jsonObject.features = submitArray;

            var serializer = Sys.Serialization.JavaScriptSerializer;
            var jsonString = serializer.serialize(jsonObject);

            document.getElementById('__RestoreVectors' + clientId).value = jsonString;
        }
    }
}

function SynchronizeSimpleMarkers(parser) {
    var tempMap = parser.map;
    var simpleMarkerOverlayIds = parser.simpleMarkerOverlayIds;
    var overlaysJson = new Array();

    for (var i = 0; i < simpleMarkerOverlayIds.length; i++) {
        var overlayJson = {};
        overlayJson.id = simpleMarkerOverlayIds[i];
        overlayJson.markers = new Array();
        var layer = tempMap.getLayer(overlayJson.id);
        for (var j = 0; j < layer.markers.length; j++) {
            if (layer.markers[j].isShadow && layer.markers[j].isShadow != undefined)
                continue;
            var markerJson = {};
            markerJson.id = layer.markers[j].id;
            markerJson.x = layer.markers[j].lonlat.lon;
            markerJson.y = layer.markers[j].lonlat.lat;
            overlayJson.markers.push(markerJson);
        }

        overlaysJson.push(overlayJson);
    }

    var serializer = Sys.Serialization.JavaScriptSerializer;
    var jsonString = serializer.serialize(overlaysJson);

    document.getElementById('__SimpleMarkerOverlayField' + parser.map.clientId).value = jsonString;
}



  
