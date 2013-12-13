function setTheme(select){
	var current = window.localStorage.getItem("AppTheme");
	window.localStorage.setItem("AppTheme",select);
	$('#page').attr('data-theme',select).removeClass('ui-body-'+current).addClass('ui-body-'+select).trigger('create');
	$('#header').attr('data-theme',select).removeClass('ui-body-'+current).addClass('ui-body-'+select).trigger('create');
	$('#back').attr('data-theme',select).removeClass('ui-body-'+current).addClass('ui-body-'+select).trigger('create');
	$('#navbar').attr('data-theme',select).removeClass('ui-body-'+current).addClass('ui-body-'+select).trigger('create');
	$('#content').attr('data-theme',select).removeClass('ui-body-'+current).addClass('ui-body-'+select).trigger('create');
	$('#footer').attr('data-theme',select).removeClass('ui-body-'+current).addClass('ui-body-'+select).trigger('create');
};