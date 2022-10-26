
/* Land Rates*/
function AddlandRateIcon(landRate) {
    var latitude;
    var longitude;
    
    longitude = landRate.latz;
    latitude = landRate.lonz;
    var centre = ol.proj.transform([latitude ,longitude ], 'EPSG:4326', 'EPSG:3857');
    var iconFeature = new ol.Feature({
        geometry: new ol.geom.Point(centre),
        Obligation: 'Land Rates',
        LRNo: landRate.plot_No,
        Owner: landRate.owner,
        Balance: landRate.outstanding
    });


    var iconStyle = new ol.style.Style({
        image: new ol.style.Icon(/** @type {olx.style.IconOptions} */({
            anchor: [0.5, 46],
            anchorXUnits: 'fraction',
            anchorYUnits: 'pixels',
            scale: 0.75,
            src: '/nrsmap/icons/icon.png'
        }))
    });

    iconFeature.setStyle(iconStyle);

    var vectorSource = new ol.source.Vector({
        features: [iconFeature]
    });

    var vectorLayer = new ol.layer.Vector({
        title: 'lr' + landRate.landRateID,
        source: vectorSource
    });

    map.addLayer(vectorLayer);

    //Set text to remove
    $("#lr" + landRate.landRateID).text("Remove");
}

function RemovelandRateIcon(landRate) {
    //console.log("Please Remove");
    map.getLayers().forEach(function (layer) {
        //console.log(layer);
        //console.log("Layer Title: "+layer.get('title'));

        // optionally check that the layer is the one you want.
        if (layer.get('title') === 'lr' + landRate.landRateID) {
            //Remove layer
            map.removeLayer(layer);

            //Set text to Map
            $("#lr" + landRate.landRateID).text("Map");
        }
    });
}

/* Sectional Properties*/

function AddSectPropIcon(sectProp) {
    var latitude;
    var longitude;

    longitude = sectProp.latz;
    latitude = sectProp.lonz;
    var centre = ol.proj.transform([latitude , longitude], 'EPSG:4326', 'EPSG:3857');
    var iconFeature = new ol.Feature({
        geometry: new ol.geom.Point(centre),
        Obligation: 'Sectional Property',
        LRNo: sectProp.lR_No,
        UnitNo: sectProp.unit_No,
        Balance: sectProp.outstanding
    });


    var iconStyle = new ol.style.Style({
        image: new ol.style.Icon(/** @type {olx.style.IconOptions} */({
            anchor: [0.5, 46],
            anchorXUnits: 'fraction',
            anchorYUnits: 'pixels',
            scale: 0.75,
            src: '/nrsmap/icons/Building.png'
        }))
    });

    iconFeature.setStyle(iconStyle);

    var vectorSource = new ol.source.Vector({
        features: [iconFeature]
    });

    var vectorLayer = new ol.layer.Vector({
        title: 'sc' + sectProp.id,
        source: vectorSource
    });

    map.addLayer(vectorLayer);

    //Set text to remove
    $("#sc" + sectProp.id).text("Remove");
}

function RemoveSectPropIcon(sectProp) {
    map.getLayers().forEach(function (layer) {
        //console.log(layer);
        //console.log("Layer Title: "+layer.get('title'));

        // optionally check that the layer is the one you want.
        if (layer.get('title') === 'sc' + sectProp.id) {
            //Remove layer
            map.removeLayer(layer);
            //Set text to Map
            $("#sc" + sectProp.id).text("Map");
        }
    });
}

/* Advertisement*/
function AddAdvertisementIcon(advert) {
    var latitude;
    var longitude;

    longitude = advert.latz;
    latitude = advert.lonz;
    var centre = ol.proj.transform([latitude , longitude], 'EPSG:4326', 'EPSG:3857');
    var iconFeature = new ol.Feature({
        geometry: new ol.geom.Point(centre),
        Obligation: 'Advertisement',
        Company: advert.company,
        Size: advert.sze,
        MonthlyCharges: advert.monthlyCharges
    });


    var iconStyle = new ol.style.Style({
        image: new ol.style.Icon(/** @type {olx.style.IconOptions} */({
            anchor: [0.5, 46],
            anchorXUnits: 'fraction',
            anchorYUnits: 'pixels',
            scale: 0.75,
            src: '/nrsmap/icons/Advert.png'
        }))
    });

    iconFeature.setStyle(iconStyle);

    var vectorSource = new ol.source.Vector({
        features: [iconFeature]
    });

    var vectorLayer = new ol.layer.Vector({
        title: 'ad' + advert.advertisementID,
        source: vectorSource
    });

    map.addLayer(vectorLayer);

    //Set text to remove
    $("#ad" + advert.advertisementID).text("Remove");
}

function RemoveAdvertisementIcon(advert) {
    map.getLayers().forEach(function (layer) {
        //console.log(layer);
        //console.log("Layer Title: "+layer.get('title'));

        // optionally check that the layer is the one you want.
        if (layer.get('title') === 'ad' + advert.advertisementID) {
            //Remove layer
            map.removeLayer(layer);
            //Set text to Map
            $("#ad" + advert.advertisementID).text("Map");
        }
    });
}


/* Health*/

function AddHealthIcon(health) {
    var latitude;
    var longitude;

    longitude = health.latz;
    latitude = health.lonz;
    var centre = ol.proj.transform([latitude ,longitude ], 'EPSG:4326', 'EPSG:3857');
    var iconFeature = new ol.Feature({
        geometry: new ol.geom.Point(centre),
        Obligation: 'Health',
        FaciltyName: health.faciltyName,
        FaciltyCode: health.faciltyCode,
        Balance: health.outstanding
    });


    var iconStyle = new ol.style.Style({
        image: new ol.style.Icon(/** @type {olx.style.IconOptions} */({
            anchor: [0.5, 46],
            anchorXUnits: 'fraction',
            anchorYUnits: 'pixels',
            scale: 0.75,
            src: '/nrsmap/icons/Health.png'
        }))
    });

    iconFeature.setStyle(iconStyle);

    var vectorSource = new ol.source.Vector({
        features: [iconFeature]
    });

    var vectorLayer = new ol.layer.Vector({
        title: 'hl' + health.id,
        source: vectorSource
    });

    map.addLayer(vectorLayer);

    //Set text to remove
    $("#hl" + health.id).text("Remove");
}

function RemoveHealthIcon(health) {
    map.getLayers().forEach(function (layer) {
        //console.log(layer);
        //console.log("Layer Title: "+layer.get('title'));

        // optionally check that the layer is the one you want.
        if (layer.get('title') === 'hl' + health.id) {
            //Remove layer
            map.removeLayer(layer);
            //Set text to Map
            $("#hl" + health.id).text("Map");
        }
    });
}

/* Liqour*/

function AddLiqourIcon(liqour) {
    var latitude;
    var longitude;

    longitude = liqour.latz;
    latitude = liqour.lonz;
    var centre = ol.proj.transform([latitude ,longitude ], 'EPSG:4326', 'EPSG:3857');
    var iconFeature = new ol.Feature({
        geometry: new ol.geom.Point(centre),
        Obligation: 'Liqour',
        LiqourJointName: liqour.liqourJointName,
        Amount: liqour.amount,
        Balance: liqour.outstanding
    });


    var iconStyle = new ol.style.Style({
        image: new ol.style.Icon(/** @type {olx.style.IconOptions} */({
            anchor: [0.5, 46],
            anchorXUnits: 'fraction',
            anchorYUnits: 'pixels',
            scale: 0.75,
            src: '/nrsmap/icons/Bar.png'
        }))
    });

    iconFeature.setStyle(iconStyle);

    var vectorSource = new ol.source.Vector({
        features: [iconFeature]
    });

    var vectorLayer = new ol.layer.Vector({
        title: 'lq' + liqour.id,
        source: vectorSource
    });

    map.addLayer(vectorLayer);

    //Set text to remove
    $("#lq" + liqour.id).text("Remove");
}

function RemoveLiqourcon(liqour) {
    map.getLayers().forEach(function (layer) {
        //console.log(layer);
        //console.log("Layer Title: "+layer.get('title'));

        // optionally check that the layer is the one you want.
        if (layer.get('title') === 'lq' + liqour.id) {
            //Remove layer
            map.removeLayer(layer);
            //Set text to Map
            $("#lq" + liqour.id).text("Map");
        }
    });
}

/* Market*/

function AddMarketIcon(market) {
    var latitude;
    var longitude;

    longitude = market.latz;
    latitude = market.lonz;
    var centre = ol.proj.transform([latitude , longitude], 'EPSG:4326', 'EPSG:3857');
    var iconFeature = new ol.Feature({
        geometry: new ol.geom.Point(centre),
        Obligation: 'Market',
        Market: market.market,
        Stall_type: market.stall_type,
        Balance: market.outstanding
    });


    var iconStyle = new ol.style.Style({
        image: new ol.style.Icon(/** @type {olx.style.IconOptions} */({
            anchor: [0.5, 46],
            anchorXUnits: 'fraction',
            anchorYUnits: 'pixels',
            scale: 0.75,
            src: '/nrsmap/icons/Market.png'
        }))
    });

    iconFeature.setStyle(iconStyle);

    var vectorSource = new ol.source.Vector({
        features: [iconFeature]
    });

    var vectorLayer = new ol.layer.Vector({
        title: 'mr' + market.id,
        source: vectorSource
    });

    map.addLayer(vectorLayer);

    //Set text to remove
    $("#mr" + market.id).text("Remove");
}

function RemoveMarketIcon(market) {
    map.getLayers().forEach(function (layer) {
        //console.log(layer);
        //console.log("Layer Title: "+layer.get('title'));

        // optionally check that the layer is the one you want.
        if (layer.get('title') === 'mr' + market.id) {
            //Remove layer
            map.removeLayer(layer);
            //Set text to Map
            $("#mr" + market.id).text("Map");
        }
    });
}


/* SBP*/

function AddSBPIcon(SBP) {
    var latitude;
    var longitude;

    longitude = SBP.latz;
    latitude = SBP.lonz;
    var centre = ol.proj.transform([latitude , longitude], 'EPSG:4326', 'EPSG:3857');
    var iconFeature = new ol.Feature({
        geometry: new ol.geom.Point(centre),
        Obligation: 'SBP',
        SBP_No: SBP.sbP_No,
        BusinessName: SBP.businessName,
        ActivityAmount: SBP.activityAmount
    });


    var iconStyle = new ol.style.Style({
        image: new ol.style.Icon(/** @type {olx.style.IconOptions} */({
            anchor: [0.5, 46],
            anchorXUnits: 'fraction',
            anchorYUnits: 'pixels',
            scale: 0.75,
            src: '/nrsmap/icons/Business.png'
        }))
    });

    iconFeature.setStyle(iconStyle);

    var vectorSource = new ol.source.Vector({
        features: [iconFeature]
    });

    var vectorLayer = new ol.layer.Vector({
        title: 'sb' + SBP.id,
        source: vectorSource
    });

    map.addLayer(vectorLayer);

    //Set text to remove
    $("#sb" + SBP.id).text("Remove");
}

function RemoveSBPIcon(SBP) {
    map.getLayers().forEach(function (layer) {
        //console.log(layer);
        //console.log("Layer Title: "+layer.get('title'));

        // optionally check that the layer is the one you want.
        if (layer.get('title') === 'sb' + SBP.id) {
            //Remove layer
            map.removeLayer(layer);
            //Set text to Map
            $("#sb" + SBP.id).text("Map");
        }
    });
}