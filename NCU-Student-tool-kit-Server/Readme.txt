NCU-Student-tool-kit-Server

2013/12/26:
140.115.156.46/webservice/WebService.asmx

2013/12/13:
add method: GetDepartmentNewsList
	@iPage: which page is requested by client
        Return: a json string about ID, title, URL, and date
        
        This function is called to get one page news titles.
        Client can chose one news URL to get more detail content
        by function GetDepartmentNewsContent().

