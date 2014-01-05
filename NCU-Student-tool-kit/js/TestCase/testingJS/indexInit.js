$(function(){
	var theme = window.localStorage.getItem("AppTheme");
	var fontsize = window.localStorage.getItem("AppFontSize");
	if(theme == null)
		window.localStorage.setItem("AppTheme","a");
	if(fontsize == null)
		window.localStorage.setItem("AppFontSize","100%");
});