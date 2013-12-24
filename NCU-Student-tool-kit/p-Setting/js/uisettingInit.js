$(function(){		
	$('#themeBtn-a').click(function() {
		setTheme($('#themeBtn-a').attr('data-theme'));
	});
	$('#themeBtn-b').click(function() {
		setTheme($('#themeBtn-b').attr('data-theme'));
	});
	$('#themeBtn-c').click(function() {
		setTheme($('#themeBtn-c').attr('data-theme'));
	});
	$('#themeBtn-d').click(function() {
		setTheme($('#themeBtn-d').attr('data-theme'));
	});
	$('#fontBtn-a').click(function() {
		setFontSize('110%');
	});
	$('#fontBtn-b').click(function() {
		setFontSize('100%');
	});
	$('#fontBtn-c').click(function() {
		setFontSize('90%');
	});
	setTheme(window.localStorage.getItem("AppTheme"));
	setFontSize(window.localStorage.getItem("AppFontSize"));
});

