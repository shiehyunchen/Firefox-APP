using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;

namespace WebService1
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class Service1 : System.Web.Services.WebService
    {
        protected NewsofDepartment m_NewsofDepartment;      // Declare NewsofDepartment class
        protected NewsofBBSystem m_NewsofBBSystem;          // Declare NewsofBBSystem class

        public Service1()
        {
            m_NewsofDepartment = new NewsofDepartment();    // allocate
            m_NewsofBBSystem = new NewsofBBSystem();        // allocate
        }

        [WebMethod] //******102522095 莊凱迪
        public string HelloWorld()
        {
            return "Hello World";
        }

        /**
         * GetDepartmentNewsList - Get the department's news titles
         * @iPage: which page is requested by client
         * Return: a json string about ID, title, URL, and date
         * 
         * This method is called to get one page news titles.
         * Client can chose one news URL to get more detail content
         * by function GetDepartmentNewsContent().
         * 
         * Written by Hao Chen - 102522094
         **/
        [WebMethod]
        public string GetDepartmentNewsList(int iPage)
        {
            Context.Response.Write(m_NewsofDepartment.GetDepartmentNewsList(iPage));
            Context.Response.End();

            return "";
        }

        /**
         * GetDepartmentNewContent - Get the department's news content
         * @Url: which news is requested by client
         * Return: a text string about content detail
         *  
         * Written by Hao Chen - 102522094
         **/
        [WebMethod]
        public string GetDepartmentNewContent(string strID)
        {
            Context.Response.Write(m_NewsofDepartment.GetDepartmentNewsContent(strID));
            Context.Response.End();

            return "";
        }

        /**
         * GetBBSystemCourseList - Get the account's all tittle of courses
         * @strStudentID: the account's ID
         * @strPassword: the account's password
         * Return: a json string about course title, and URL
         * 
         * This function is called to get course titles which the student took.
         * Client can chose one course URL to get more detail content
         * by function GetBBSystemCourseContent().
         * 
         * Written by Hao Chen - 102522094
         **/
        [WebMethod]
        public string GetBBSystemCourseList(string strStudentID, string strPassword)
        {
            CookieContainer cookies = new CookieContainer();

            m_NewsofBBSystem.BBSystemLogin(ref cookies, strStudentID, strPassword);
            Context.Response.Write(m_NewsofBBSystem.GetBBSystemCourseTitle(cookies));
            Context.Response.End();

            return "";
        }

        /**
         * GetBBSystemCourseList - Get titles and contents of one course
         * @strStudentID: the account's ID
         * @strPassword: the account's password
         * @strUrl: the URL of the course
         * Return: a json string about title, and content
         * 
         * Written by Hao Chen - 102522094
         **/
        [WebMethod]
        public string GetBBSystemCourseContentList(string strStudentID, string strPassword, string strID)
        {
            CookieContainer cookies = new CookieContainer();

            m_NewsofBBSystem.BBSystemLogin(ref cookies, strStudentID, strPassword);
            Context.Response.Write(m_NewsofBBSystem.GetBBSystemCourseContent(cookies, strID));
            Context.Response.End();

            return "";
        }

        [WebMethod] //******102522095 莊凱迪
        public string GetState(string id, string pw)                   //登入帳號,回傳帳號的狀態
        {
            CookieContainer cc = new CookieContainer();             //存網站的cookie
            cc = login(id, pw);

            if (cc.Count == 0)                                       //cookie取得失敗 結束並回傳
                return "網路連線異常";

            string ReturnPage = GetWebSource("signin", "None", cc);                    //存到ReturnPage

            if (ReturnPage.Contains("國立中央大學入口網站"))                                     //包含此字串,代表登入失敗
            {
                var result = new { state = "User或Password錯誤" };                             //回傳訊息包程JSON
                ReturnPage = JsonConvert.SerializeObject(result);
            }
            else if (ReturnPage.Contains("請選擇下列計畫進行簽到"))                         //登入成功,且尚未簽到,  回傳簽到項目
            {
                List<int> RadioIndex = new List<int>();              //存radio button在 page裡的index
                List<int> LineIndex = new List<int>();              //存切割出來的行數 index
                List<string> title = new List<string>();            //存切割出來的title
                string radiobuttonRegex = "input type=\"radio\" name=\"signin\" id=\"checkin\" value=";   //keyword
                string titleRegex = "td align=\"center";                                             //keyword

                foreach (Match m in Regex.Matches(ReturnPage, radiobuttonRegex))                 //找出keyword的index ,存在RadioIndex
                {
                    RadioIndex.Add(m.Index);
                }
                RadioIndex.Add(ReturnPage.Length - 1);                  //End的index

                for (int i = 1; i < RadioIndex.Count; i++)              //切割title出來
                {
                    string tmpPage = "";
                    string start = ">";
                    string end = "<";
                    int start_idx;
                    int end_idx;
                    tmpPage = ReturnPage.Substring(RadioIndex[i - 1], RadioIndex[i] - RadioIndex[i - 1]);   //找出各radio button間的string
                    foreach (Match m in Regex.Matches(tmpPage, titleRegex))
                        LineIndex.Add(m.Index);
                    start_idx = tmpPage.IndexOf(start, LineIndex[3]);                           //找出title那行的開頭的index
                    end_idx = tmpPage.IndexOf(end, LineIndex[3]);                               //找出title那行的結尾的index
                    tmpPage = tmpPage.Substring(start_idx + 1, end_idx - 1 - (start_idx + 1));  //切割title
                    title.Add(tmpPage);                                                         //存入List
                }
                var result = new
                {
                    state = "登入成功,尚未簽到",
                    count = title.Count,
                    title = from s in title select s
                };                                                            //包成JSON

                ReturnPage = JsonConvert.SerializeObject(result);
            }
            else if (ReturnPage.Contains("您尚未簽退"))                                          //出現這訊息表示尚未簽退
            {
                var result = new { state = "尚未簽退" };
                ReturnPage = JsonConvert.SerializeObject(result);
            }
            else if (ReturnPage.Contains("您沒有兼任任何計畫"))                                          //出現這訊息表示尚未簽退
            {
                var result = new { state = "沒有兼任任何計畫" };
                ReturnPage = JsonConvert.SerializeObject(result);
            }
            return ReturnPage;
        }


        [WebMethod] //******102522095 莊凱迪
        public string signin_out(string id, string pw, string  in_out, int TitleNum)        //in_out 傳入值為 signin 或 signout  ,   TitleNum為第幾個案子
        {
            CookieContainer cc = new CookieContainer();             //存網站的cookie

            string key;
            int lastvalue;
            if (in_out == "signin")
            {
                key = "name=\"signin\"";                            //signin要找的關鍵字
                lastvalue = 2;                                      //距離字尾的個數
            }
            else
            {
                key = "name=\"signout\"";                           //signout要找的關鍵字
                lastvalue = 16;
            }

            string ButtonValue = "";
            using (StringReader reader = new StringReader(GetWebSource(in_out, "None", cc = login(id, pw))))        //擷取button的value
            {
                string line;
                while ((line = reader.ReadLine()) != null)              //一行一行讀進來
                {
                    if (line.Contains(key))
                    {
                        int index = line.IndexOf("value");
                        ButtonValue = line.Substring(index + 7, line.Length - index - 7 - lastvalue);

                        if (TitleNum == 1)                              //如果到選取的title則結束
                            break;
                        else
                            TitleNum--;
                    }
                }
            }

            string postString = in_out + "=" + ButtonValue + "&submit=送出";          //要傳送給伺服器的訊息
            string ReturnWeb = GetWebSource(in_out, postString, cc);            //傳送訊息並取得回傳的頁面

            if (ReturnWeb.Contains("簽到成功"))
            {
                ReturnWeb = "簽到成功";
            }
            else if (ReturnWeb.Contains("簽退成功"))
            {
                ReturnWeb = "簽退成功";
            }
            else
            {
                ReturnWeb = "Error";
            }
            return ReturnWeb;
        }

        //******102522095 莊凱迪
        private string GetWebSource(string in_out, string postString, CookieContainer cc) 
        {
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create("http://140.115.182.62/PartTime/parttime.php/" + in_out);      //建立網站連線
            myRequest.CookieContainer = cc;
            myRequest.ContentType = "application/x-www-form-urlencoded";                                                                //設定型態
            myRequest.Method = "POST";

            if (postString != "None")                                                       //postString有要傳送的訊息要另外做的
            {
                byte[] postData = Encoding.ASCII.GetBytes(postString);
                myRequest.ContentLength = postData.Length;

                Stream stream1 = myRequest.GetRequestStream(); //open connection
                stream1.Write(postData, 0, postData.Length); // Send the data.
                stream1.Close();
            }
            else
                myRequest.ContentLength = 0;

            HttpWebResponse response = (HttpWebResponse)myRequest.GetResponse();                                                        //取得回傳的頁面
            StreamReader reader = new StreamReader(response.GetResponseStream());                                                       //並存成string
            string websource = reader.ReadToEnd();

            myRequest.Abort();
            response.Close();
            reader.Close();

            return websource;
        }

        //******102522095 莊凱迪
        private CookieContainer login(string id, string pw)             //登入網站,取得cookie
        {
            CookieContainer cc = new CookieContainer();             //存網站的cookie
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create("https://portal.ncu.edu.tw/login/j_spring_security_check");
            myRequest.CookieContainer = new CookieContainer();                  //開啟登入網站

            myRequest.Timeout = 5000;

            myRequest.ContentType = "application/x-www-form-urlencoded";        //設定傳送的資料型態
            myRequest.Method = "POST";
            string postString = "j_username=" + id + "&j_password=" + pw + "&submit=Login";
            byte[] postData = Encoding.ASCII.GetBytes(postString);              //轉換型態
            myRequest.ContentLength = postData.Length;

            try
            {
                Stream stream = myRequest.GetRequestStream(); //open connection
                stream.Write(postData, 0, postData.Length); // Send the data.
                stream.Close();

                HttpWebResponse response = (HttpWebResponse)myRequest.GetResponse();        //紀錄回傳的cookie
            }
            catch                                                           //連線異常的處理
            {
                return cc;
            }
            cc = myRequest.CookieContainer;

            myRequest.Abort();

            return cc;
        }
    }
}