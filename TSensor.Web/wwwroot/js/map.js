var Map = function (options) {
    var that = this;

    that.options = options;

    that.map = null;
    that.layerDataSource = null;

    that.getFeatureStyle = function(feature) {
        var pointTypeImage = that.options.PointTypeImageList[feature.get('pointTypeGuid')];
        if (!pointTypeImage) {
            pointTypeImage = that.options.PushpinImage
        }
        
        return new ol.style.Style({
            image: new ol.style.Icon({
                anchor: [0.5, 1],
                src: pointTypeImage
            })
        });
    };

    that._overlay = new ol.Overlay({
        element: document.getElementById('ol-popup'),
        autoPan: true,
        autoPanAnimation: {
            duration: 200,
        },
    });

    that.overlayPointGuid = null;
    that.openOverlay = function (feature) {
        if (feature) {
            document.querySelector('.ol-popup-header').innerHTML =
                `<a href="${feature.get('url')}" target="_blank">${feature.get('name')}</a>`;

            var html = '';
            var tankList = feature.get('tankList');
            if (tankList.length) {
                html = tankList.map(function (i) {
                    return `<div>${i.name}</div>`;
                }).join('');
            }
            document.querySelector('.ol-popup-content').innerHTML = html;

            that._overlay.setPosition(feature.getGeometry().getCoordinates());
            that.overlayPointGuid = feature.get('guid');
        } else {
            that._overlay.setPosition(undefined);
            that.overlayPointGuid = null;
        }
    };

    that.fitMobile = function (e) {
        if (window.screen.width < 768) {
            document.querySelector('.ol-map').style.height = (window.screen.height - 170) + 'px';
        }
    };

    that.init = function (features) {
        that.fitMobile();

        var _features = features.map(function (i) {
            return new ol.Feature({
                geometry: new ol.geom.Point(
                    ol.proj.transform([i.lon, i.lat], 'EPSG:4326', 'EPSG:3857')
                ),
                name: i.name,
                tankList: i.tankList,
                url: i.url,
                guid: i.guid,
                pointTypeGuid: i.pointTypeGuid
            });
        });

        var layerDataSource = new ol.source.Vector({ features: _features });

        var _view = new ol.View({
            center: ol.proj.fromLonLat([that.options.DefaultLongitude, that.options.DefaultLatitude]),
            zoom: 10,
            maxZoom: that.options.MaxZoom
        });

        var _map = new ol.Map({
            target: 'ol-map',
            layers: [
                new ol.layer.Tile({
                    source: new ol.source.OSM()
                }),
                new ol.layer.Vector({
                    source: layerDataSource,
                    style: that.getFeatureStyle
                })
            ],
            view: _view,
            overlays: [that._overlay],
        });

        if (_features && _features.length) {
            var extent = layerDataSource.getExtent();
            _view.fit(extent, _map.getSize());

            var zoom = _view.getZoom() - 0.25;
            _view.setZoom(zoom);
        }

        var _select = new ol.interaction.Select({
            style: that.getFeatureStyle
        });
        _map.addInteraction(_select);
        _select.on('select', function (e) {
            var features = e.target.getFeatures();
            if (features.getLength()) {
                that.openOverlay(features.item(0));
            } else {
                that.openOverlay(undefined);
            }
        });

        that.map = _map;
        that.layerDataSource = layerDataSource;
    };

    that.coordinatesUpdate = function (data, date) {
        data.forEach(function (item) {
            var coord = ol.proj.transform([item.lon, item.lat], 'EPSG:4326', 'EPSG:3857');

            var feature = that.layerDataSource.getFeatures().filter(p => p.get('guid') == item.pointGuid);

            if (feature.length == 1) {
                feature[0].getGeometry().setCoordinates(coord);

                if (that._overlay.getPosition() && that.overlayPointGuid == item.pointGuid) {
                    that._overlay.setPosition(coord);
                }
            }
        });
    };

    that.initPointSelect = function (data, controls) {
        var disableChangeEvent = false;

        var setCoordinates = function (lonlat) {
            disableChangeEvent = true;

            controls.lon.value = lonlat[0].toFixed(7).toString().replace('.', ',');
            controls.lat.value = lonlat[1].toFixed(7).toString().replace('.', ',');

            disableChangeEvent = false;
        };

        var changeCoordinates = function (e) {
            if (disableChangeEvent) {
                return;
            }

            var newLon = parseFloat(controls.lon.value.replace(",", "."));
            var newLat = parseFloat(controls.lat.value.replace(",", "."));

            if (newLon && newLon != 0 && newLat && newLat != 0) {
                var _coord = ol.proj.transform([newLon, newLat], 'EPSG:4326', 'EPSG:3857');

                if (point) {
                    point.setGeometry(new ol.geom.Point(_coord));
                } else {
                    point = new ol.Feature({
                        geometry: new ol.geom.Point(_coord),
                        pointTypeGuid: controls.type.value
                    });
                        
                    layerDataSource.addFeature(point);
                }

                _map.getView().fit(point.getGeometry().getExtent());
                _map.getView().setZoom(10);
            } else {
                if (point) {
                    point = null;
                    layerDataSource.clear();
                }
            }
        };

        controls.lon.addEventListener('change', function (e) {
            changeCoordinates(e);
        });
        controls.lat.addEventListener('change', function (e) {
            changeCoordinates(e);
        });
        controls.type.addEventListener('change', function (e) {
            if (point) {
                point.set('pointTypeGuid', controls.type.value);
            }
        });

        var layerDataSource = new ol.source.Vector({ features: [] });

        var center, point;

        if (data.lon && data.lat) {
            point = new ol.Feature({
                geometry: new ol.geom.Point(
                    ol.proj.transform([data.lon, data.lat], 'EPSG:4326', 'EPSG:3857')
                ),
                pointTypeGuid: data.type
            });
            layerDataSource.addFeature(point);

            center = [data.lon, data.lat];
        } else {
            center = [that.options.DefaultLongitude, that.options.DefaultLatitude];
        }

        var _view = new ol.View({
            center: ol.proj.fromLonLat(center),
            zoom: 10
        });

        var _layer = new ol.layer.Vector({
            source: layerDataSource,
            style: that.getFeatureStyle
        });
        var _map = new ol.Map({
            target: 'map',
            layers: [
                new ol.layer.Tile({
                    source: new ol.source.OSM()
                }),
                _layer
            ],
            view: _view
        });

        var translate = new ol.interaction.Translate({});
        translate.on('translateend', function (e) {
            var lonlat = ol.proj.transform(e.coordinate, 'EPSG:3857', 'EPSG:4326');
            setCoordinates(lonlat);
        });

        _map.addInteraction(translate);

        _map.on('pointermove', function (e) {
            if (e.dragging) {
                return;
            }

            var hit = _map.hasFeatureAtPixel(_map.getEventPixel(e.originalEvent));
            _map.getTargetElement().style.cursor = hit ? 'pointer' : '';
        });

        _map.on('click', function (e) {
            if (point) {
                return;
            }

            point = new ol.Feature({
                geometry: new ol.geom.Point(e.coordinate),
                pointTypeGuid: controls.type.value
            });
                
            layerDataSource.addFeature(point);

            var lonlat = ol.proj.transform(e.coordinate, 'EPSG:3857', 'EPSG:4326');
            setCoordinates(lonlat);
        });
    };

    return that;
}