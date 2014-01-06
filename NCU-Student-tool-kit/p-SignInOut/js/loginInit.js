function send(){
	if($('#acc').val()=="")
		alert("Please Fill the Account");
	else if($('#pwd').val()=="")
		alert("Please Fill the Password");
	else {
		var acc = $('#acc').val();
		var pwd = $('#pwd').val();
		if($('#remember').is(":checked")) {
			window.localStorage.setItem("acc", acc);				
			window.localStorage.setItem("pwd", pwd);
		}
<<<<<<< HEAD
		window.sessionStorage.setItem("acc", acc);				
		window.sessionStorage.setItem("pwd", pwd);
		//Saving the session with acc and pwd
		var info = "id="+acc+"&"+"pw="+pwd;
		//Server format for acc and pwd
=======
		else{
			window.sessionStorage.setItem("acc", acc);				
			window.sessionStorage.setItem("pwd", pwd);
		}
		var info = "strStudentID="+acc+"&"+"strPassword="+pwd;
		
>>>>>>> shaggy_BB
		$.ajax({
		type: "POST",
		contentType: "application/x-www-form-urlencoded; charset=utf-8",
		url: "http://140.115.156.46/webservice/WebService.asmx/GetState",
		data: info,
		dataType: "xml",
		success: function (data, textStatus) {	 
			if (textStatus == "success") {
				msg = JSON.parse($(data).find('string').text());
				if(msg.state == "User name or password error."){
					alert("User name or password error.");					
				}
				else{
					window.sessionStorage.setItem("fromLogin","1");
<<<<<<< HEAD
					self.location.href='./SignInOut.html';
					//Redirect to SignInOut
=======
					self.location.href='./BlackBoard.html';					
>>>>>>> shaggy_BB
				}
			}
		 },
		error: function (data, status, error) {
			alert("error");
		}
		});
	}
}