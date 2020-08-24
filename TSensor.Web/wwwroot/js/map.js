var Map = function () {
    var that = this;

    that.featureStyle = new ol.style.Style({
        image: new ol.style.Icon({
            anchor: [0.5, 1],
            src: '/img/feature.png'
        })
    });

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