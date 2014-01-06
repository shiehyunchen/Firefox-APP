function send(){
	if($('#acc').val()=="")
		alert("Please Fill the Account");
	else if($('#pwd').val()=="")
		alert("Please Fill the Password");
	else {
		var acc = $('#acc').val();
		var pwd = $('#pwd').val();
		if($('#remember').is("checked")) {
			window.localStorage.setItem("acc", acc);				
			window.localStorage.setItem("pwd", pwd);
		}
		window.sessionStorage.setItem("acc", acc);				
		window.sessionStorage.setItem("pwd", pwd);
		var info = "strStudentID="+acc+"&"+"strPassword="+pwd;
		
		$.ajax({
		type: "POST",
		contentType: "application/x-www-form-urlencoded; charset=utf-8",
		url: "http://140.115.156.46/webservice/WebService.asmx/GetBBSystemCourseList",
		data: info,
		dataType: "json",
		success: function (data, textStatus) {	 
			if (textStatus == "success") {
				window.sessionStorage.setItem("fromLogin","1");
				self.location.href='./BlackBoard.html';
			}
		 },
		error: function (data, status, error) {
			alert("error");
		}
		});
	}
}