var map;
//var myMarker;
//var desMarker;
var myLatlng;
var desLatlng;
var directionsDisplay;

$(document).ready(function () {
	initializeMap();
});

function initializeMap() {
    //Create the latlng object based on the GPS Position retrieved
    var NCU_Center = new google.maps.LatLng(24.9680, 121.1930);

    //Set Google Map options
    var options = {
        zoom: 15,
        center: NCU_Center,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };

    //Display the Map
    map = new google.maps.Map(document.getElementById("NCU_map"), options);
}

function errorHandler(error) {
    alert("Error while retrieving current position. Error code: " + error.code + ",Message: " + error.message);
}

function showMyCurrentLocation(position) {
    myLatlng = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
	findRoute();
	/*
	var Name = "Your location";
    var optionOfMarker = {
        animation: google.maps.Animation.DROP,
        position: myLatlng,
        title: Name
    };
	if(myMarker)
	{
		myMarker.setMap(null);
	}
    myMarker = new google.maps.Marker(optionOfMarker);
	myMarker.setMap(map);
	*/
}

function findMyCurrentLocation() {
    var geoService = navigator.geolocation;
    if (geoService) {
        navigator.geolocation.getCurrentPosition(showMyCurrentLocation, errorHandler, { enableHighAccuracy: true });
    } else {
        alert("Your Browser does not support GeoLocation.");
    }
}

function showDestination(name,latitude,longitude) {
	desLatlng = new google.maps.LatLng(latitude, longitude);
	
    /*var Name = name;
    var optionOfMarker = {
		animation: google.maps.Animation.DROP,
        position: desLatlng,
        title: Name
    };
	if(desMarker)
	{
		desMarker.setMap(null);
	}
    desMarker = new google.maps.Marker(optionOfMarker);
	desMarker.setMap(map);
	*/
}

function findDestination(value) {
	var buildingInfoFile = "./building.json";
	$.getJSON(buildingInfoFile, function(building){
		$.each(building, function(i, item){
			if(item.id == value)
			{
				showDestination(item.name,item.latitude,item.longitude);
				//alert(desLatlng+"\n1");
				findMyCurrentLocation();
				
			}
		});
	});
}

function findRoute(){
	
	if(directionsDisplay)
	{
		//alert(directionsDisplay);
		directionsDisplay.setMap(null);
	}
	directionsDisplay = new google.maps.DirectionsRenderer({
		'map': map,
		'preserveViewport': true
	});	
	//alert(myLatlng+"\n12");
	var start = myLatlng;
	//alert(desLatlng+"\n22");
	var end = desLatlng;
	
	//directionsDisplay.setPanel(document.getElementById("directions_panel"));
	
	var request = {
		origin:start,
		destination:end,
		travelMode: google.maps.DirectionsTravelMode.WALKING 
	};
	
	var directionsService = new google.maps.DirectionsService();
	
	directionsService.route(request, function(response, status) {
		if (status == google.maps.DirectionsStatus.OK) {
			directionsDisplay.setDirections(response);
		}
	});
}

function showRoute(value) {
	
	findDestination(value);
}