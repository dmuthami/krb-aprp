(function () {

    var _url = "http://localhost:8080" + '/geoserver/nrs/wms';

    var _layers = [
        new ol.layer.Group({
            'title': 'Base maps',
            layers: [
                new ol.layer.Group({
                    title: 'Water color with labels',
                    type: 'base',
                    combine: true,
                    visible: false,
                    layers: [
                        new ol.layer.Tile({
                            source: new ol.source.Stamen({
                                layer: 'watercolor'
                            })
                        }),
                        new ol.layer.Tile({
                            source: new ol.source.Stamen({
                                layer: 'terrain-labels'
                            })
                        })
                    ]
                }),
                new ol.layer.Tile({
                    title: 'Water color',
                    type: 'base',
                    visible: false,
                    source: new ol.source.Stamen({
                        layer: 'watercolor'
                    })
                }),
                new ol.layer.Tile({
                    title: 'OSM',
                    type: 'base',
                    visible: true,
                    source: new ol.source.OSM()
                })
            ]
        }),
        new ol.layer.Group({
            title: 'Overlays',
            layers: [
                new ol.layer.Image({
                    title: 'Parcels',
                    extent: [4078253, -160345, 4131171, 4102998],
                    source: new ol.source.ImageWMS({
                        url: _url,
                        params: { 'LAYERS': 'nrs:parcel' },
                        ratio: 1,
                        serverType: 'geoserver'
                    })
                })
            ]
        })
    ]

    var map = new ol.Map({
        target: 'map',
        layers: _layers,
        view: new ol.View({
            center: ol.proj.transform([36.8219, -1.2921], 'EPSG:4326', 'EPSG:3857'),
            zoom: 12
        })
    });

    var layerSwitcher = new ol.control.LayerSwitcher({
        tipLabel: 'Légende' // Optional label for button
    });
    map.addControl(layerSwitcher);

})();
