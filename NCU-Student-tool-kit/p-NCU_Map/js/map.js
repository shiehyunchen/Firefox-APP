var map;
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
        mapTypeId: google.maps.MapTypeId.ROADMAP,
		disableDoubleClickZoom: true,
		streetViewControl: false
    };

    //Display the Map
    map = new google.maps.Map(document.getElementById("NCU_map"), options);
}

function findRoute(myLatlng,desLatlng){
	
	if(directionsDisplay)
	{
		directionsDisplay.setMap(null);
	}
	directionsDisplay = new google.maps.DirectionsRenderer({
		'map': map,
		'preserveViewport': true //dont change the preserveViewport
	});	
	
	//show the text instruction
	//directionsDisplay.setPanel(document.getElementById("directions_panel"));
	
	var request = {
		origin: myLatlng,
		destination: desLatlng,
		travelMode: google.maps.DirectionsTravelMode.WALKING 
	};
	
	var directionsService = new google.maps.DirectionsService();
	
	directionsService.route(request, function(response, status) {
		if (status == google.maps.DirectionsStatus.OK) {
			directionsDisplay.setDirections(response);
		}
	});
}

function findMyCurrentLocation(desLatlng) {
	//require geoService
    var geoService = navigator.geolocation;
	
    if (geoService) {
        navigator.geolocation.getCurrentPosition(
			//successCallback
			function(position) {
				var myLatlng = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
				findRoute(myLatlng,desLatlng);
			}, 
			//errorCallback
			function(error) {
				alert("Error while retrieving current position. Error code: " + error.code + ",Message: " + error.message);
			});
    } else {
        alert("Your Browser does not support GeoLocation.");
    }
}

function showRoute(value) {
	var buildingInfoFile = "./js/building.json";
	$.getJSON(buildingInfoFile, function(building){
		$.each(building, function(i, item){
			if(item.id == value){
				var desLatlng = new google.maps.LatLng(item.latitude, item.longitude);
				findMyCurrentLocation(desLatlng);
			}
		});
	});
}