$(function(){
	if($('#strStudentID').val()=="")
		alert("Please Fill the Student ID Number");
	else if($('#strPassword').val()=="")
		alert("Please Fill the Password");
	else {
		var StudentID = $('#strStudentID').val();
		var pwd = $('#strPassword').val();
		if($('#remember').is(":checked")) {
			window.localStorage.setItem("strStudentID", StudentID);				
			window.localStorage.setItem("strPassword", pwd);
		}
		else{
			window.sessionStorage.setItem("strStudentID",StudentID);				
			window.sessionStorage.setItem("strPassword", pwd);
		}
		var info = "strStudentID="+StudentID+"&"+"strPassword="+pwd;
		alert(info);
		$.ajax({
		type: "POST",
		contentType: "application/x-www-form-urlencoded; charset=utf-8",
		url: "http://140.115.156.46/webservice/WebService.asmx/GetBBSystemCourseList",
		data: info,
		dataType: "json",
		success: function (data, textStatus) {	 
			if (textStatus == "success") {
				alert($(data)[0].NAME);
			}
		 },
		error: function (data, status, error) {
			alert("error");
		}
		});
	}
});