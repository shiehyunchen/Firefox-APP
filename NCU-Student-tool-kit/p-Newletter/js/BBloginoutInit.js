$(function(){
	setTheme(window.localStorage.getItem("AppTheme"));
	setFontSize(window.localStorage.getItem("AppFontSize"));
	var login = window.sessionStorage.getItem("fromLogin");
	var StudentID,pwd;
	if(login==null){
		StudentID = window.localStorage.getItem("strStudentID");
		pwd = window.localStorage.getItem("strPassword");
	}
	else{
		StudentID = window.sessionStorage.getItem("strStudentID");
		pwd = window.sessionStorage.getItem("strPassword");
	}
	if(StudentID==null||pwd==null||StudentID==""||pwd=="")
		self.location.href='./BBLogin.html';
	else {
		var info = "strStudentID="+StudentID+"&strPassword="+pwd;
		window.sessionStorage.setItem("strStudentID",StudentID);
		window.sessionStorage.setItem("strPassword",pwd);
		var state;
		var title;
		$.ajax({
			type: "POST",
			contentType: "application/x-www-form-urlencoded; charset=utf-8",
			url: "http://140.115.156.46/webservice/WebService.asmx/GetBBSystemCourseList",
			data: info,
			dataType: "xml",
			success: function (data, textStatus) {	 
				if (textStatus == "success") {
					msg = JSON.parse($(data).find('string').text());
					state = msg.state;
					if(msg.state == "登入成功,尚未簽到") {								
						title = msg.title;
						var id = 'option_1';								
						var $container = $('#optionsgroup').find('.ui-controlgroup-controls');	
						$('<input />', {
							'name': 'options',
							'URL': '****',
							'checked': 'true'
						}).append('<label for="' + id + '">' + title + '</label>').appendTo($container);								
						$container.find('input[type=radio]').checkboxradio();																
						//$("#image").attr('src','./img/signIn.png');
						$("#image").attr('onclick','callWebService("signin",window.sessionStorage.getItem("strStudentID"),window.sessionStorage.getItem("strPassword"))');
					}
					else if(msg.state == "User或Password錯誤"){
						alert("User或Password錯誤");
						self.location.href="../index.html";
					}
					else{
					//尚未簽退
						$("#image").attr('src','./img/signOut.png');
						$("#image").attr('onclick','callWebService("signout",window.sessionStorage.getItem("strStudentID"),window.sessionStorage.getItem("strPassword"))');
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