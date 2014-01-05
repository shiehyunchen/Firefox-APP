$(function(){
	$('#saveButton').click(function() {
		window.localStorage.setItem("acc", $('#acc').val());				
		window.localStorage.setItem("pwd", $('#pwd').val());
		window.sessionStorage.clear();
		alert("Saving successful!");
	});	
	$("#acc").val(window.localStorage.getItem("acc"));
	$("#pwd").val(window.localStorage.getItem("pwd"));
	$('.ui-page-text').css('font-size','10px');
});