$(function(){
	setTheme(window.localStorage.getItem("AppTheme"));
	setFontSize(window.localStorage.getItem("AppFontSize"));
});

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
		else{
			window.sessionStorage.setItem("acc", acc);				
			window.sessionStorage.setItem("pwd", pwd);
		}
		var info = "id="+acc+"&"+"pw="+pwd;
		
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
					self.location.href='./SignInOut.html';					
				}
			}
		 },
		error: function (data, status, error) {
			alert("error");
		}
		});
	}
}