$(function(){
	$('#saveButton').click(function() {
		window.localStorage.setItem("acc", $('#acc').val());				
		window.localStorage.setItem("pwd", $('#pwd').val());
		window.sessionStorage.clear();
		//window.localStorage
		//alert($('#acc').val()+'+'+$('#pwd').val());
	});	
	$("#acc").val(window.localStorage.getItem("acc"));
	$("#pwd").val(window.localStorage.getItem("pwd"));
	$('.ui-page-text').css('font-size','10px');
	setTheme(window.localStorage.getItem("AppTheme"));
	setFontSize(window.localStorage.getItem("AppFontSize"));
});