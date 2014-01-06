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
	//Getting acc and pwd from session or local Storage
	if(acc==null||pwd==null||acc==""||pwd=="") {
		self.location.href='./Login.html';	
		//Redirect to Login page
	}
	else {
		var info = "id="+acc+"&pw="+pwd;
		window.sessionStorage.setItem("acc",acc);
		window.sessionStorage.setItem("pwd",pwd);
		var state;
		var title;
		$.ajax({
			type: "POST",
			contentType: "application/x-www-form-urlencoded; charset=utf-8",
			url: "http://140.115.156.46/webservice/WebService.asmx/GetState",
			data: info,
			dataType: "xml",
			success: function (data, textStatus) {	 
				if (textStatus == "success") {
					msg = JSON.parse($(data).find('string').text());
					state = msg.state;
					if(msg.state == "Login Success! Please signin.") {	
						//SignIn
						title = msg.title;
						var id = 'option_1';								
						var $container = $('#optionsgroup').find('.ui-controlgroup-controls');	
						$('<input />', {
							'id': id,
							'type': 'radio',
							'name': 'options',
							'value': '1',
							'checked': 'true'
						}).append('<label for="' + id + '">' + title + '</label>').appendTo($container);								
						$container.find('input[type=radio]').checkboxradio();																
						$("#image").attr('src','./img/signIn.png');
						$("#image").attr('onclick','callWebService("signin",window.sessionStorage.getItem("acc"),window.sessionStorage.getItem("pwd"))');
					}
					else if(msg.state == "Login Success! No projects."){
						//Non project
						$("#image").attr('src','./img/nonProjects.png');						
					}
					else if(msg.state == "Login Success! Please signout."){
						//SignOut
						$("#image").attr('src','./img/signOut.png');
						$("#image").attr('onclick','callWebService("signout",window.sessionStorage.getItem("acc"),window.sessionStorage.getItem("pwd"))');
					}
					else if(msg.state == "User name or password error."){
						//Acc or Pwd error
						alert("User name or password error.");
						self.location.href="../index.html";
					}
				}
			 },
			error: function (data, status, error) {
				alert("error");
			}
		});
	}			
});

function callWebService(signInOut,id,pwd) {
	var info = "id="+id+"&pw="+pwd+"&in_out="+signInOut+"&TitleNum=0";
	$.ajax({
		type: "POST",
		contentType: "application/x-www-form-urlencoded; charset=utf-8",
		url: "http://140.115.156.46/webservice/WebService.asmx/signin_out",
		data: info,
		dataType: "xml",
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