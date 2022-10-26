/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
//var map;



$(document).ready(function () {

    var _url = "http://localhost:8080" + '/geoserver/nrs/wms';

    var geoserverURL = $("#Mapping_MapURL").val();
    _url = geoserverURL + '/geoserver/nrs/wms';

    _wmsSource = new ol.layer.Image({
        title: 'Zones',
        extent: [4078253, -160345, 4131171, 4102998],
        source: new ol.source.ImageWMS({
            url: _url,
            crossOrigin: 'anonymous',
            params: { 'LAYERS': 'nrs:arreas_zones' },
            ratio: 1,
            serverType: 'geoserver'
        })
    });

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
            layers: [_wmsSource]
        })
    ];

    _view = new ol.View({
        center: ol.proj.transform([36.8219, -1.2921], 'EPSG:4326', 'EPSG:3857'),
        zoom: 10
    });

    map = new ol.Map({
        //overlays: [overlay],
        target: 'mapp',
        layers: _layers,
        view: _view
    });

    var layerSwitcher = new ol.control.LayerSwitcher({
        tipLabel: 'Légende' // Optional label for button
    });

    map.addControl(layerSwitcher);
});


