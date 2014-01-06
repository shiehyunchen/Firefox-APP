$(function(){	
	var login = window.sessionStorage.getItem("fromLogin");
	var acc,pwd;
	if(login==null){
		acc = window.localStorage.getItem("acc");
		pwd = window.localStorage.getItem("pwd");
	}
	else{
		acc = window.sessionStorage.getItem("acc");
		pwd = window.sessionStorage.getItem("pwd");
	}
	if(acc==null||pwd==null||acc==""||pwd=="")
		self.location.href='./Login.html';
	else {
		var info = "strStudentID="+acc+"&"+"strPassword="+pwd;
		window.sessionStorage.setItem("strStudentID",acc);
		window.sessionStorage.setItem("pwd",pwd);
		var state;
		var title;
		$.ajax({
			type: "POST",
			contentType: "application/x-www-form-urlencoded; charset=utf-8",
			url: "http://140.115.156.46/webservice/WebService.asmx/GetBBSystemCourseList",
			data: info,
			dataType: "json",
			async: false,
			success: function (data, textStatus) {	 
				if (textStatus == "success") {
					$("#course").empty();
					for(i=0;i<data.length;i++)
					{	
						var courseID = $(data)[i].ID.substring(1,6);
						var content = "<option value=\""+courseID+"\">"+$(data)[i].NAME+"</option>";
		        		$("#course").append( content ).selectmenu('refresh');
					}
					get_content(courseID);
				}
			 },
			error: function (data, status, error) {
				alert("error");
			}
		});
	}			
});
 function get_content(ID)
 {
		var acc = window.sessionStorage.getItem("acc");
		var pwd = window.sessionStorage.getItem("pwd");
		var info = "strStudentID="+acc+"&"+"strPassword="+pwd+"&"+"strID="+ID;
		var state;
		var title;
		$.ajax({
			type: "POST",
			contentType: "application/x-www-form-urlencoded; charset=utf-8",
			url: "http://140.115.156.46/webservice/WebService.asmx/GetBBSystemCourseContentList",
			data: info,
			dataType: "json",
			async: false,
			success: function (data, textStatus) {	 
				if (textStatus == "success") {
					$("#course_content").empty();
					var tt=data.length;
					for(i=0;i<tt;i++)
					{
						var content = "<div data-role='collapsible' id='course_content" + i + " '><h3>" + ($(data)[i]).TITLE + "</h3><p>" + ($(data)[i]).CONTENT + "</p></div>";
						$("#course_content").append( content ).collapsibleset('refresh');

					}
				}
			 },
			error: function (data, status, error) {
				alert("error");
			}
		});}

function callWebService(signInOut,id,pwd) {
	var info = "id="+id+"&pw="+pwd+"&in_out="+signInOut+"&TitleNum=0";
	$.ajax({
		type: "POST",
		contentType: "application/x-www-form-urlencoded; charset=utf-8",
		url: "http://140.115.156.46/webservice/WebService.asmx/signin_out",
		data: info,
		dataType: "xml",
		async: false,
		success: function (data, textStatus) {	 
			if (textStatus == "success") {
				alert($(data).find('string').text());
				self.location.href="../index.html";
				//state = msg.state;
			}
		 },
		error: function (data, status, error) {
			alert("error");
		}
	});
}