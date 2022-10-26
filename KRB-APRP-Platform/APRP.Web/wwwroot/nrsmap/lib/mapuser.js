/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
//var map;



$(document).ready(function () {

    var _url = "http://localhost:8080" + '/geoserver/nrs/wms';

    //var geoserverURL = $("#Mapping_MapURL").val();
    //_url = geoserverURL + '/geoserver/nrs/wms';

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
        zoom: 12
    });

    window.map = new ol.Map({
        target: 'map',
        layers: _layers,
        view: _view
    });

    var layerSwitcher = new ol.control.LayerSwitcher({
        tipLabel: 'Légende' // Optional label for button
    });

    map.addControl(layerSwitcher);

    var sidebar = new ol.control.Sidebar({ element: 'sidebar', position: 'left' });

    map.addControl(sidebar);

    /*
     * Code for icons and popup
     */
    ShowMap();

});


function ShowMap() {
    var element = document.getElementById('popup');
    var content = document.getElementById('popup-content');
    var closer = document.getElementById('popup-closer');

    var popup = new ol.Overlay({
        element: element,
        positioning: 'bottom-center',
        stopEvent: false,
        offset: [0, -50]
    });
    map.addOverlay(popup);

    //Popup close event
    closer.onclick = function () {
        popup.setPosition(undefined);
        closer.blur();
        return false;
    };

    // display popup on click
    map.on('click', function (evt) {
        var feature = map.forEachFeatureAtPixel(evt.pixel,
            function (feature) {
                return feature;
            });
        if (feature) {
            var coordinates = feature.getGeometry().getCoordinates();
            popup.setPosition(coordinates);
            var str = "";
            if (feature.get('Obligation') === 'Sectional Property') {
                str = '<p>' + "<strong>Sectional Property:</strong>" +
                    '</p>' +
                    '<b>Lr No: </b>' + feature.get('LRNo') + '<br>' +
                    '<b>Unit No: </b>' + feature.get('UnitNo') + '<br>' +
                    '<b>Balance: </b>' + feature.get('Balance') + '<br>';
            } else if (feature.get('Obligation') === 'Advertisement') {
                str = '<p>' + "<strong>Advertisement:</strong>" +
                    '</p>' +
                    '<b>Company: </b>' + feature.get('Company') + '<br>' +
                    '<b>Size: </b>' + feature.get('Owner') + '<br>' +
                    '<b>Monthly Charges: </b>' + feature.get('MonthlyCharges') + '<br>';
            }
            else if (feature.get('Obligation') === 'Health') {
                str = '<p>' + "<strong>Health:</strong>" +
                    '</p>' +
                    '<b>Facility Name: </b>' + feature.get('FaciltyName') + '<br>' +
                    '<b>Facility Code: </b>' + feature.get('FaciltyCode') + '<br>' +
                    '<b>Balance: </b>' + feature.get('Balance') + '<br>';
            }
            else if (feature.get('Obligation') === 'Liqour') {
                str = '<p>' + "<strong>Liqour:</strong>" +
                    '</p>' +
                    '<b>Joint: </b>' + feature.get('LiqourJointName') + '<br>' +
                    '<b>Amount: </b>' + feature.get('Amount') + '<br>' +
                    '<b>Balance: </b>' + feature.get('Balance') + '<br>';
            }
            else if (feature.get('Obligation') === 'Market') {
                str = '<p>' + "<strong>Market:</strong>" +
                    '</p>' +
                    '<b>Market: </b>' + feature.get('Market') + '<br>' +
                    '<b>Stall Type: </b>' + feature.get('Stall_type') + '<br>' +
                    '<b>Balance: </b>' + feature.get('Balance') + '<br>';
            }
            else if (feature.get('Obligation') === 'SBP') {
                str = '<p>' + "<strong>SBP:</strong>" +
                    '</p>' +
                    '<b>SBP No: </b>' + feature.get('SBP_No') + '<br>' +
                    '<b>Business Name: </b>' + feature.get('BusinessName') + '<br>' +
                    '<b>Activity Amount: </b>' + feature.get('ActivityAmount') + '<br>';
            }
            else {
                str = '<p>' + "<strong>Land Rate:</strong>" +
                    '</p>' +
                    '<b>Lr No: </b>' + feature.get('LRNo') + '<br>' +
                    '<b>Owner: </b>' + feature.get('Owner') + '<br>' +
                    '<b>Balance: </b>' + feature.get('Balance') + '<br>';
            }
            content.innerHTML = str;

            $(element).popover('show');
        } else {
            $(element).popover('dispose');
        }
    });

    map.on('pointermove', function (e) {
        var pixel = map.getEventPixel(e.originalEvent);
        var hit = map.hasFeatureAtPixel(pixel);
        map.getViewport().style.cursor = hit ? 'pointer' : '';
    });
}