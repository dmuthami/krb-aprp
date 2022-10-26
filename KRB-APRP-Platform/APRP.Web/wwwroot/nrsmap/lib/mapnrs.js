/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
//var map;



$(document).ready(function () {

    var _url = "http://localhost:8080" + '/geoserver/nrs/wms';

    var geoserverURL = $("#Mapping_MapURL").val();
    _url = geoserverURL + '/geoserver/krb/wms';

    //KenHa Roads
    _wmsSource = new ol.layer.Image({
        title: 'KeNHA Roads',
        extent: [3734499, -563148, 4706706, 618898],/*xmin,ymin,xmax,ymax*/
        source: new ol.source.ImageWMS({
            url: _url,
            crossOrigin: 'anonymous',
            params: { 'LAYERS': 'krb:KenhaRoad' },
            ratio: 1,
            serverType: 'geoserver'
        })
    });

    //Counties
    /*
     * xmin,ymin,xmax,ymax
     * 3778611, -473031, 4560120, 549249
     */
    _wmsCounties = new ol.layer.Image({
        title: 'Counties',
        extent: [3734499, -563148, 4706706, 618898],
        source: new ol.source.ImageWMS({
            url: _url,
            crossOrigin: 'anonymous',
            params: { 'LAYERS': 'krb:County' },
            ratio: 1,
            serverType: 'geoserver'
        })
    });

    //Constituencies
    /*
     */
    _wmsConstituencies = new ol.layer.Image({
        title: 'Constituencies',
        visible: false,
        source: new ol.source.ImageWMS({
            url: _url,
            crossOrigin: 'anonymous',
            params: { 'LAYERS': 'krb:Constituency' },
            ratio: 1,
            serverType: 'geoserver'
        })
    });

    //KeRRA
    /*
     */
    _wmsKERRA = new ol.layer.Image({
        title: 'KeRRa Roads',
        visible: false,
        source: new ol.source.ImageWMS({
            url: _url,
            crossOrigin: 'anonymous',
            params: { 'LAYERS': 'krb:KerraRoad' },
            ratio: 1,
            serverType: 'geoserver'
        })
    });

    //Kura
    /*
     */
    _wmsKURA = new ol.layer.Image({
        title: 'KURA Roads',
        visible: false,
        source: new ol.source.ImageWMS({
            url: _url,
            crossOrigin: 'anonymous',
            params: { 'LAYERS': 'krb:KuraRoad' },
            ratio: 1,
            serverType: 'geoserver'
        })
    });

    //KWS
    /*
     */
    _wmsKWS = new ol.layer.Image({
        title: 'KWS Roads',
        visible: false,
        source: new ol.source.ImageWMS({
            url: _url,
            crossOrigin: 'anonymous',
            params: { 'LAYERS': 'krb:KwsRoad' },
            ratio: 1,
            serverType: 'geoserver'
        })
    });


    //CountiesRoads
    /*
     */
    _wmsCountiesRoads = new ol.layer.Image({
        title: 'Counties Roads',
        visible: false,
        source: new ol.source.ImageWMS({
            url: _url,
            crossOrigin: 'anonymous',
            params: { 'LAYERS': 'krb:CountiesRoad' },
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
            layers: [_wmsCounties, _wmsConstituencies, _wmsSource, _wmsKERRA, _wmsKURA, _wmsKWS, _wmsCountiesRoads]
        })
    ];

    _view = new ol.View({
        center: ol.proj.transform([37.13, -0.06], 'EPSG:4326', 'EPSG:3857'),
        zoom: 7
    });

    map = new ol.Map({
        overlays: [overlay],
        target: 'mapp',
        layers: _layers,
        view: _view
    });

    var layerSwitcher = new ol.control.LayerSwitcher({
        tipLabel: 'Légende' // Optional label for button
    });

    map.addControl(layerSwitcher);

    var sidebar = new ol.control.Sidebar({ element: 'sidebar', position: 'left' });

    map.addControl(sidebar);

    /**
    * Add a click handler to the map to render the popup.
    */
    map.on('singleclick', function (evt) {
        var coordinate = evt.coordinate;
        var hdms = ol.coordinate.toStringHDMS(ol.proj.transform(
            coordinate, 'EPSG:3857', 'EPSG:4326'));
        try {
            displayInfoFeatures_Final(evt, hdms, coordinate);
        } catch (err) {
            console.log(err);
        }
    });
});

function populateDataTable(data) {
    console.log("populating data table...");
    // clear the table before populating it with more data
    $("#idTable").DataTable().clear();

    var length = Object.keys(data).length;
    for (var i = 1; i < length + 1; i++) {
        var sectProp = data[i];
        console.log(sectProp);
        // You could also use an ajax property on the data table initialization
        $('#idTable').dataTable().fnAddData([
            sectProp.id,
            sectProp.lR_No,
            sectProp.unit_No,
            sectProp.floor_No
        ]);
    }
}


function populateAgencyTable(data, Agency) {
    var tableID;
    //Set table ID based on agency
    if (Agency === "KenhaRoad") {
        tableID = "idTableKenHA ";
    }
    if (Agency === "KerraRoad") {
        tableID = "idTableKeRRA ";
    }
    if (Agency === "KuraRoad") {
        tableID = "idTableKuRA ";
    }
    if (Agency === "KwsRoad") {
        tableID = "idTableKwS ";
    }
    if (Agency === "CountiesRoad") {
        tableID = "idTableCountiesRoads ";
    }
    // clear the table before populating it with more data
    try {
        $("#" + tableID).find("tr:gt(0)").remove();
    } catch (err) {
        console.log(err);
    }

    $.each(data, function (index, value) {
        tblRow = "<tr><td>" + index + "</td>" +
            "<td>" + value + "</td>";
        $('#' + tableID +'> tbody:last').append(tblRow);
    });
}

function TestURL(url) {
    //console.log("URLLLL:" + url);
    var arr = url.split("&");
    var str = arr[6];
    var res = str.replace("%3A", ":");
    var str2 = res.split(":");
    //console.log(str2[1]);
    return str2[1];
}


function displayInfoFeatures_Final(evt, hdms, coordinate) {
    //-----------Trial code goes here---
    var viewResolution = _view.getResolution();
    var viewProjection = _view.getProjection();

    var url = _wmsSource.getSource().getGetFeatureInfoUrl(
        evt.coordinate, viewResolution, viewProjection, {
        'INFO_FORMAT': 'text/javascript',
        'propertyName': 'ID,RdNum,RdClass,Section_ID,Length'
    });

    //
    var Agency = "";
    //Call function to return True if URL Layrer is that of KenHA
    Agency = TestURL(url);
    if (Agency === "KenhaRoad") {
        displayInfoFeatures_Kenha(url, hdms, coordinate, Agency);
    }

    url = _wmsKERRA.getSource().getGetFeatureInfoUrl(
        evt.coordinate, viewResolution, viewProjection, {
        'INFO_FORMAT': 'text/javascript',
        'propertyName': 'ID,RdNum,RdClass,Section_ID,Length'
    });
    Agency = TestURL(url);//Check agency again
    if (Agency === "KerraRoad") {
        displayInfoFeatures_kerra(url, hdms, coordinate, Agency);
    }

    //KURA
    url = _wmsKURA.getSource().getGetFeatureInfoUrl(
        evt.coordinate, viewResolution, viewProjection, {
        'INFO_FORMAT': 'text/javascript',
        'propertyName': 'ID,RdNum,RdClass,Section_ID,Length'
    });
    Agency = TestURL(url);//Check agency again
    if (Agency === "KuraRoad") {
        displayInfoFeatures_kura(url, hdms, coordinate, Agency);
    }

    //KwS
    url = _wmsKWS.getSource().getGetFeatureInfoUrl(
        evt.coordinate, viewResolution, viewProjection, {
        'INFO_FORMAT': 'text/javascript',
        'propertyName': 'ID,RdNum,RdClass,Section_ID,Length'
    });
    Agency = TestURL(url);//Check agency again
    if (Agency === "KwsRoad") {
        displayInfoFeatures_kws(url, hdms, coordinate, Agency);
    }

    //Counties
    url = _wmsCountiesRoads.getSource().getGetFeatureInfoUrl(
        evt.coordinate, viewResolution, viewProjection, {
        'INFO_FORMAT': 'text/javascript',
        'propertyName': 'ID,RdNum,RdClass,Section_ID,Length'
    });
    Agency = TestURL(url);//Check agency again
    if (Agency === "CountiesRoad") {
        displayInfoFeatures_counties(url, hdms, coordinate, Agency);
    }
}

function displayInfoFeatures_Kenha(url, hdms, coordinate, Agency) {
    if (url) {
        var parser = new ol.format.GeoJSON();
        $.ajax({
            url: url,
            type: 'POST',
            dataType: 'jsonp',
            jsonpCallback: 'parseResponse'
        }).then(function (response) {
            var result = parser.readFeatures(response);
            //console.log(result);
            var str = '<p>You clicked here:</p><code>' + hdms +
                '</code>';
            var myID = 0;
            if (result.length) {
                var info = [];
                for (var i = 0, ii = result.length; i < ii; ++i) {
                /***/
                    info.push(result[i].get('ID'));
                    info.push(result[i].get('RdNum'));
                    info.push(result[i].get('RdClass'));
                    info.push(result[i].get('Section_ID'));
                    info.push(result[i].get('Length'));

                }

                //Get ID
                ID = info[0];

                str = '<b>Road ID: </b>' + info[1] + '<br>' +
                    '<b>Road Class: </b>' + info[2] + '<br>' +
                    '<b>Section ID: </b>' + info[3] + '<br>' +
                    '<b>Length: </b>' + info[4] + '<br>';
                content.innerHTML = str;

                //Ajax Call to pull GIS Data for Roads
                $.ajax({
                    method: "POST",
                    url: "/GIS/GetKenNHARoadDetail",
                    datatype: "json",
                    data: { ID: ID }
                })
                    .done(function (data) {
                        //write json array object
                        try {
                            populateAgencyTable(data, Agency);
                        } catch (err) {
                            console.log(err);
                        }
                        try {
                            overlay.setPosition(coordinate);
                        } catch (err) {
                            console.log(err);
                        }
                                               
                    });
            }
        });
    }
}

function displayInfoFeatures_kerra(url, hdms, coordinate, Agency) {
    if (url) {
        var parser = new ol.format.GeoJSON();
        $.ajax({
            url: url,
            type: 'POST',
            dataType: 'jsonp',
            jsonpCallback: 'parseResponse'
        }).then(function (response) {
            var result = parser.readFeatures(response);
            //console.log(result);
            var str = '<p>You clicked here:</p><code>' + hdms +
                '</code>';
            var myID = 0;
            if (result.length) {
                var info = [];
                for (var i = 0, ii = result.length; i < ii; ++i) {
                /***/
                    info.push(result[i].get('ID'));
                    info.push(result[i].get('RdNum'));
                    info.push(result[i].get('RdClass'));
                    info.push(result[i].get('Section_ID'));
                    info.push(result[i].get('Length'));
                }

                //Get ID
                ID = info[0];

                str = '<b>Road ID: </b>' + info[1] + '<br>' +
                    '<b>Road Class: </b>' + info[2] + '<br>' +
                    '<b>Section ID: </b>' + info[3] + '<br>' +
                    '<b>Length: </b>' + info[4] + '<br>';
                content.innerHTML = str;

                //Ajax Call to pull GIS Data for Roads
                $.ajax({
                    method: "POST",
                    url: "/GIS/GetKeRRARoadDetail",
                    datatype: "json",
                    data: { ID: ID }
                })
                    .done(function (data) {
                        //write json array object
                        try {
                            populateAgencyTable(data, Agency);
                        } catch (err) {
                            console.log(err);
                        }
                        try {
                            overlay.setPosition(coordinate);
                        } catch (err) {
                            console.log(err);
                        }
                    });
            }
        });
    }
}

function displayInfoFeatures_kura(url, hdms, coordinate, Agency) {
    if (url) {
        var parser = new ol.format.GeoJSON();
        $.ajax({
            url: url,
            type: 'POST',
            dataType: 'jsonp',
            jsonpCallback: 'parseResponse'
        }).then(function (response) {
            var result = parser.readFeatures(response);
            //console.log(result);
            //RdNum,First_RdCl,First_RdNa,Shape_Le_1
            var str = '<p>You clicked here:</p><code>' + hdms +
                '</code>';
            var myID = 0;
            if (result.length) {
                var info = [];
                for (var i = 0, ii = result.length; i < ii; ++i) {
                /***/
                    info.push(result[i].get('ID'));
                    info.push(result[i].get('RdNum'));
                    info.push(result[i].get('RdClass'));
                    info.push(result[i].get('Section_ID'));
                    info.push(result[i].get('Length'));
                }
                //Get Road Number
                ID = info[0];

                str = '<b>Road ID: </b>' + info[1] + '<br>' +
                    '<b>Road Class: </b>' + info[2] + '<br>' +
                    '<b>Section ID: </b>' + info[3] + '<br>' +
                    '<b>Length: </b>' + info[4] + '<br>';
                content.innerHTML = str;

                //Ajax Call to pull GIS Data for Roads
                $.ajax({
                    method: "POST",
                    url: "/GIS/GetKURARoadDetail",
                    datatype: "json",
                    data: { ID: ID }
                })
                    .done(function (data) {
                        //write json array object
                        try {
                            populateAgencyTable(data, Agency);
                        } catch (err) {
                            console.log(err);
                        }
                        try {
                            overlay.setPosition(coordinate);
                        } catch (err) {
                            console.log(err);
                        }
                    });
            }
        });
    }
}

function displayInfoFeatures_kws(url, hdms, coordinate, Agency) {
    if (url) {
        var parser = new ol.format.GeoJSON();
        $.ajax({
            url: url,
            type: 'POST',
            dataType: 'jsonp',
            jsonpCallback: 'parseResponse'
        }).then(function (response) {
            var result = parser.readFeatures(response);
            //console.log(result);
            //RdNum,First_RdCl,First_RdNa,Shape_Le_1
            var str = '<p>You clicked here:</p><code>' + hdms +
                '</code>';
            var myID = 0;
            if (result.length) {
                var info = [];
                for (var i = 0, ii = result.length; i < ii; ++i) {
                /***/
                    info.push(result[i].get('ID'));
                    info.push(result[i].get('RdNum'));
                    info.push(result[i].get('RdClass'));
                    info.push(result[i].get('Section_ID'));
                    info.push(result[i].get('Length'));
                }
                //Get Section ID
                ID = info[0];

                str = '<b>Road ID: </b>' + info[1] + '<br>' +
                    '<b>Road Class: </b>' + info[2] + '<br>' +
                    '<b>Section ID: </b>' + info[3] + '<br>' +
                    '<b>Length: </b>' + info[4] + '<br>';
                content.innerHTML = str;

                //Ajax Call to pull GIS Data for Roads
                $.ajax({
                    method: "POST",
                    url: "/GIS/GetKWSRoadDetail",
                    datatype: "json",
                    data: { ID: ID }
                })
                    .done(function (data) {
                        //write json array object
                        try {
                            populateAgencyTable(data, Agency);
                        } catch (err) {
                            console.log(err);
                        }
                        try {
                            overlay.setPosition(coordinate);
                        } catch (err) {
                            console.log(err);
                        }
                    });
            }
        });
    }
}

function displayInfoFeatures_counties(url, hdms, coordinate, Agency) {
    if (url) {
        var parser = new ol.format.GeoJSON();
        $.ajax({
            url: url,
            type: 'POST',
            dataType: 'jsonp',
            jsonpCallback: 'parseResponse'
        }).then(function (response) {
            var result = parser.readFeatures(response);
            //console.log(result);
            //RdNum,First_RdCl,First_RdNa,Shape_Le_1
            var str = '<p>You clicked here:</p><code>' + hdms +
                '</code>';
            var myID = 0;
            if (result.length) {
                var info = [];
                for (var i = 0, ii = result.length; i < ii; ++i) {
                /***/
                    info.push(result[i].get('ID'));
                    info.push(result[i].get('RdNum'));
                    info.push(result[i].get('RdClass'));
                    info.push(result[i].get('Section_ID'));
                    info.push(result[i].get('Length'));
                }
                //Get Section ID
                ID = info[0];

                str = '<b>Road ID: </b>' + info[1] + '<br>' +
                    '<b>Road Class: </b>' + info[2] + '<br>' +
                    '<b>Section ID: </b>' + info[3] + '<br>' +
                    '<b>Length: </b>' + info[4] + '<br>';
                content.innerHTML = str;

                //Ajax Call to pull GIS Data for Roads
                $.ajax({
                    method: "POST",
                    url: "/GIS/GetCountiesRoadDetail",
                    datatype: "json",
                    data: { ID: ID }
                })
                    .done(function (data) {
                        //write json array object
                        try {
                            populateAgencyTable(data, Agency);
                        } catch (err) {
                            console.log(err);
                        }
                        try {
                            overlay.setPosition(coordinate);
                        } catch (err) {
                            console.log(err);
                        }
                    });
            }
        });
    }
}


