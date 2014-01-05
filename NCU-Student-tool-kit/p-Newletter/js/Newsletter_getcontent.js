$(function(){
	setTheme(window.localStorage.getItem("AppTheme"));
	setFontSize(window.localStorage.getItem("AppFontSize"));
	updateBtn(0);
	//callWebService(1);
});

function getContent(ID){
 
	var result;
	var info = "strID=" + ID;
	$.ajax({
		type: "POST",
		contentType: "application/x-www-form-urlencoded; charset=utf-8",
		url: "http://140.115.156.46/webservice/WebService.asmx/GetDepartmentNewContent",
		data: info,
		dataType: "text",
		async: false,
		success: function (data, textStatus) 
		{	 	
			result = data;
		 },
		error: function (data, status, error) 
		{
			alert("error");
		}
	});
	return result;
 }

function callWebService(pageNum,startTitle) {
	var info = "iPage=" + pageNum;
	var msg;
	$.ajax({
		type: "POST",
		contentType: "application/x-www-form-urlencoded; charset=utf-8",
		url: "http://140.115.156.46/webservice/WebService.asmx/GetDepartmentNewsList",
		data: info,
		dataType: "JSON",
		async: false,
		success: function (data, textStatus) 
		{	
			$("#set").empty();
			for(i=startTitle;i<startTitle+5;i++)
			{
        		var content = "<div data-role='collapsible' id='set" + i + " '><h3>" + ($(data)[i]).TITLE + "</h3><p>" + getContent(($(data)[i]).ID) + "</p></div>";
        		$("#set").append( content ).collapsibleset('refresh');
			}
		 },
		error: function (data, status, error) {
			alert("error");
		}
	});
}

function updateBtn(titleNum)
{
	var pageNum = Math.floor(titleNum / 25) + 1;
	var startTitle = titleNum % 25;
	
	callWebService(pageNum,startTitle);
	$("#btnPage1").empty();
	if(titleNum==0)
	{
		var content = "<a href=\"#\" data-role=\"button\" data-inline=\"true\" class=\"ui-disabled\">Last</a>"
					+ "<a href=\"#\" data-role=\"button\" data-inline=\"true\" onclick= updateBtn(5)>Next</a>";
	}
	else
	{
		var lastPage=titleNum-5;
		var nextPage=titleNum+5;
		var content = "<a href=\"#\" data-role=\"button\" data-inline=\"true\" onclick= updateBtn(" + lastPage + ")>Last</a>"
					+ "<a href=\"#\" data-role=\"button\" data-inline=\"true\" onclick= updateBtn(" + nextPage + ")>Next</a>";
	}
	$("#btnPage1").append( content );
	$("#btnPage1").trigger("create");

}