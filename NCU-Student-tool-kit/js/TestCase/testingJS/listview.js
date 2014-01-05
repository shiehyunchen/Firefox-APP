var buildingInfoFile = "./js/building.json";

$.getJSON(buildingInfoFile, function(building){
	$('#buildingList').empty();
	$.each(building, function(i, item){
		$('#buildingList').append(generateLink(item));
	});
	$('#buildingList').listview('refresh');
});

 function generateLink(item){
	return '<li><a onclick="showRoute('
		+ item.id
		+ ')">'
		+ item.name
		+ '</a></li>';
 }