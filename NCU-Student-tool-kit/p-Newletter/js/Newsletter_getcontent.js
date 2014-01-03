$(function(){
	setTheme(window.localStorage.getItem("AppTheme"));
	setFontSize(window.localStorage.getItem("AppFontSize"));
	
	var info = "iPage=" + 1;
	var msg;
	$.ajax({
		type: "POST",
		contentType: "application/x-www-form-urlencoded; charset=utf-8",
		url: "http://140.115.156.46/webservice/WebService.asmx/GetDepartmentNewsList",
		data: info,
		dataType: "JSON",
		success: function (data, textStatus) {	 
			if (textStatus == "success") {
				alert("123");
				//msg = JSON.parse($(data).text());
				//msg = JSON.parse(($(data)[0]));
				alert($(data).length);
				alert(($(data)[0]).TITLE);
				//alert($(msg));
			}
		 },
		error: function (data, status, error) {
			alert("error");
		}
	});
});

function callWebService(pageNum) {
	var info = "iPage=" + 1;
	$.ajax({
		type: "POST",
		contentType: "application/x-www-form-urlencoded; charset=utf-8",
		url: "http://140.115.156.46/webservice/WebService.asmx/GetDepartmentNewsList",
		data: info,
		dataType: "JSON",
		success: function (data, textStatus) {	 
			if (textStatus == "success") {
				alert($(data).find('id').text());
			}
		 },
		error: function (data, status, error) {
			alert("error");
		}
	});
}