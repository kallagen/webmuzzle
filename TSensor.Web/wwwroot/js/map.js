var Map = function () {
    var that = this;

    that.featureStyle = new ol.style.Style({
        image: new ol.style.Icon({
            anchor: [0.52, 1],
            src: '/img/feature.png'
        })
    });

    that.map = null;

    that._overlay = new ol.Overlay({
        element: document.getElementById('ol-popup'),
        autoPan: true,
        autoPanAnimation: {
            duration: 200,
        },
    });

    that.openOverlay = function (feature) {
        if (feature) {
            document.querySelector('.ol-popup-header').innerHTML = feature.get('name');

            var html = '';
            var tankList = feature.get('tankList');
            if (tankList.length) {
                html = tankList.map(function (i) {
                    return `<a href="${i.url}" target="_blank">${i.name}</a>`;
                }).join('');
            }
            document.querySelector('.ol-popup-content').innerHTML = html;

            that._overlay.setPosition(feature.getGeometry().getCoordinates());
        } else {
            that._overlay.setPosition(undefined);
        }
    };

    that.init = function (e) {
        var _features = e.features.map(function (i) {
            return new ol.Feature({
                geometry: new ol.geom.Point(
                    ol.proj.transform([i.lon, i.lat], 'EPSG:4326', 'EPSG:3857')
                ),
                name: i.name,
                tankList: i.tankList
            });
        });

        var layerDataSource = new ol.source.Vector({ features: _features });

        var _view = new ol.View({
            center: ol.proj.fromLonLat([e.options.defaultLon, e.options.defaultLat]),
            zoom: 10
        });

        var _map = new ol.Map({
            target: 'ol-map',
            layers: [
                new ol.layer.Tile({
                    source: new ol.source.OSM()
                }),
                new ol.layer.Vector({
                    source: layerDataSource,
                    style: that.featureStyle
                })
            ],
            view: _view,
            overlays: [that._overlay],
        });

        if (_features && _features.length) {
            var extent = layerDataSource.getExtent();
            _view.fit(extent, _map.getSize());

            _view.setZoom(_view.getZoom() - 0.25);
        }

        var _select = new ol.interaction.Select({
            style: that.featureStyle
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
    };

    that.initPointSelect = function (options, controls) {
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
                    point = new ol.Feature(
                        new ol.geom.Point(_coord));
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

        var layerDataSource = new ol.source.Vector({ features: [] });

        var center, point;

        if (options.lon && options.lat) {
            point = new ol.Feature(
                new ol.geom.Point(
                    ol.proj.transform([options.lon, options.lat], 'EPSG:4326', 'EPSG:3857')
                ));
            layerDataSource.addFeature(point);

            center = [options.lon, options.lat];
        } else {
            center = [options.defaultLon, options.defaultLat];
        }

        var _view = new ol.View({
            center: ol.proj.fromLonLat(center),
            zoom: 10
        });

        var _map = new ol.Map({
            target: 'map',
            layers: [
                new ol.layer.Tile({
                    source: new ol.source.OSM()
                }),
                new ol.layer.Vector({
                    source: layerDataSource,
                    style: that.featureStyle
                })
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

            point = new ol.Feature(
                new ol.geom.Point(e.coordinate));
            layerDataSource.addFeature(point);

            var lonlat = ol.proj.transform(e.coordinate, 'EPSG:3857', 'EPSG:4326');
            setCoordinates(lonlat);
        });
    };

    return that;
}