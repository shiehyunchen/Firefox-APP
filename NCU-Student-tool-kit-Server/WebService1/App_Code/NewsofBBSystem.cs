/*
 * NewsofBBSystem - get the account's course titles and its detail content
 * Written by Hao Chen - 102522094
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Net;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using System.Data;// use DataTable and DataRow to store the data
using System.Runtime.Serialization.Json;

namespace WebService1
{
    public class NewsofBBSystem
    {
        // use for storing course titles
        public class tagCourseList
        {
            public string NAME { get; set; }
            public string ID { get; set; }
        }

        public class tagCourseContent
        {
            public string TITLE { get; set; }
            public string CONTENT { get; set; }
        }

        /**
         * GetBBSystemCourseTitle - Get the BB system's course titles what the account took
         * @cookies: pointer to cookies of the account already logined message
         * @strStudentID: the account's ID
         * @strPassword: the account's password
         * 
         * This function is called to get the account already logined message.
         **/
        public void BBSystemLogin(ref CookieContainer cookies, string strStudentID, string strPassword)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://bb.ncu.edu.tw/webapps/login/");
            request.Method = "POST";

            string postData = "user_id=" + strStudentID + "&password=&login=%E7%99%BB%E5%85%A5&action=login&remote-user=&new_loc=%2Fwebapps%2Fgradebook%2Fdo%2Fstudent%2FviewCourses&auth_type=&one_time_token=&encoded_pw=" + Convert.ToBase64String(Encoding.UTF8.GetBytes(strPassword));
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.63 Safari/537.36";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Headers.Add("Accept-Encoding: gzip,deflate,sdch");
            request.Headers.Add("Accept-Language:zh-TW,zh;q=0.8,en-US;q=0.6,en;q=0.4");
            request.KeepAlive = true;
            request.Host = "bb.ncu.edu.tw";

            request.UnsafeAuthenticatedConnectionSharing = true;
            request.Referer = "https://bb.ncu.edu.tw/";
            request.CookieContainer = new CookieContainer();
            request.CookieContainer = cookies;
            request.ContentLength = byteArray.Length;

            // for the self-signed SSL certificate
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader objReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string strTemp = objReader.ReadToEnd();
            if (strTemp.Contains("保留所有權利。美國專利編號 6,988,138。其他專利申請中。"))
            {
                cookies = new CookieContainer();
            }
            response.Close();
        }

        /**
         * GetBBSystemCourseTitle - Get the BB system's course titles what the account took
         * @cookies: cookies of the account already logined message
         * Return: a json string about Name, and course ID
         * 
         * This function is called to get course titles which the student took.
         **/
        public string GetBBSystemCourseTitle(CookieContainer cookies)
        {
            if (cookies.Count <= 0)
            {
                return "No Login data";
            }
            DataTable dtTemp = new DataTable("BBSystemCourseTable");
            DataColumn c0 = new DataColumn("NAME");
            dtTemp.Columns.Add(c0);
            DataColumn c1 = new DataColumn("ID");
            dtTemp.Columns.Add(c1);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://bb.ncu.edu.tw/webapps/gradebook/do/student/viewCourses");
            request.Method = "GET";

            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Headers.Add("Accept-Encoding:gzip,deflate,sdch");
            request.Headers.Add("Accept-Language:zh-TW,zh;q=0.8,en-US;q=0.6,en;q=0.4");
            request.KeepAlive = true;
            request.Headers.Add("Cookie:cookies_enabled=yes");
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.63 Safari/537.36";
            request.Referer = "https://bb.ncu.edu.tw/webapps/login/";
            request.Host = "bb.ncu.edu.tw";
            request.CookieContainer = cookies;

            // for the self-signed SSL certificate
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader objReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

            FormatCourseListContent(ref dtTemp, objReader.ReadToEnd());
            string strTemp = HandleCourseJSONFormat(ref dtTemp);

            response.Close();

            return strTemp;
        }

        /**
         * FormatCourseListContent - Parsing the html
         * @dtTemp: pointer to data table from GetBBSystemCourseTitle()
         * @strHtml: the html content from website of department news
         * 
         * This function is called for parsing the html.
         * 
         * The received html content is validated and valid factors are
         * replied to. In addition, data table(Name, course)
         * is configured at the end of a successful parsing.
         **/
        protected void FormatCourseListContent(ref DataTable dtTemp, string strHtml)
        {
            string strTemp;
            string strName, strID;

            Match matche = Regex.Match(strHtml, "<li>\\s*<h3\\s*>(\\s|.)+?</ul>\\s*</div>", RegexOptions.Multiline);
            strTemp = matche.Value;

            MatchCollection matches = Regex.Matches(strTemp, "href=(\\s|.)+?</a>", RegexOptions.Multiline);
            foreach (Match match in matches)
            {

                strName = Regex.Match(match.Value, ">(\\s|.)+?[(]", RegexOptions.Multiline).Value;
                strName = Regex.Replace(strName, ">(\\s)+|(\\s)[(]", "");

                strID = Regex.Match(match.Value, "&id=(\\s|.)+?&", RegexOptions.Multiline).Value;
                strID = Regex.Replace(strID, "&id=|&", "");

                SaveCourseDataToRow(ref dtTemp, strName, strID);
            }
        }

        /**
         * HandleCourseJSONFormat - Take parsing result to json format
         * @dtTemp: pointer to data table from GetBBSystemCourseTitle()
         * Return: a json string about name, and ID
         * 
         * This function is called for making json string by elements
         * (Name, ID).
         **/
        protected string HandleCourseJSONFormat(ref DataTable dtTemp)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0; i < dtTemp.Rows.Count; i++)
            {
                tagCourseList m_NewsRows = new tagCourseList();
                m_NewsRows.NAME = dtTemp.Rows[i]["NAME"].ToString();
                m_NewsRows.ID = dtTemp.Rows[i]["ID"].ToString();

                DataContractJsonSerializer dj = new DataContractJsonSerializer(typeof(tagCourseList));
                using (MemoryStream ms = new MemoryStream())
                {
                    dj.WriteObject(ms, m_NewsRows);
                    if (i > 0)
                    {
                        sb.Append(",");
                    }//end if
                    sb.Append(Encoding.UTF8.GetString(ms.ToArray()));
                }
            }//end for
            sb.Append("]");
            return sb.ToString();
        }

        /**
         * SaveCourseDataToRow - Store parsing result to data table
         * @dtTemp: pointer to data table from GetDepartmentNewsList()
         * @strName: the course name
         * @strID: the ID of course detail content
         * 
         * This function is called for storing the parsing results to 
         * data table.
         **/
        protected void SaveCourseDataToRow(ref DataTable dtTemp, string strName, string strID)
        {
            DataRow r = dtTemp.NewRow();
            r["NAME"] = strName;
            r["ID"] = strID;

            dtTemp.Rows.Add(r);
        }

        public string GetBBSystemCourseContent(CookieContainer cookies, string strID)
        {
            if (cookies.Count <= 0)
            {
                return "No Login data";
            }
            else if (strID.Length == 0)
            {
                return "strID could not be empty";
            }
            DataTable dtTemp = new DataTable("BBSystemCourseContentTable");
            DataColumn c0 = new DataColumn("TITLE");
            dtTemp.Columns.Add(c0);
            DataColumn c1 = new DataColumn("CONTENT");
            dtTemp.Columns.Add(c1);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://bb.ncu.edu.tw/webapps/blackboard/execute/announcement?method=search&context=mybb&course_id=" + strID + "&viewChoice=3");
            request.Method = "GET";

            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Headers.Add("Accept-Encoding:gzip,deflate,sdch");
            request.Headers.Add("Accept-Language:zh-TW,zh;q=0.8,en-US;q=0.6,en;q=0.4");
            request.KeepAlive = true;
            request.Headers.Add("Cookie:cookies_enabled=yes");
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.63 Safari/537.36";
            request.Referer = "https://bb.ncu.edu.tw/webapps/login/";
            request.Host = "bb.ncu.edu.tw";
            request.CookieContainer = cookies;

            // for the self-signed SSL certificate
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader objReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

            FormatCourseContent(ref dtTemp, objReader.ReadToEnd());
            string strTemp = HandleContentJSONFormat(ref dtTemp);

            response.Close();

            return strTemp;
        }

        protected void FormatCourseContent(ref DataTable dtTemp, string strHtml)
        {
            string strTemp;
            string strTitle, strContent;

            Match matche = Regex.Match(strHtml, "<!-- System announcements will not be displayed if a course is selected  from filter option-->(\\s|.)+?</form>", RegexOptions.Multiline);
            strTemp = matche.Value;

            MatchCollection matches = Regex.Matches(strTemp, "<!-- showOnCourses can be true only for system announcements -->(\\s|.)+?</li>", RegexOptions.Multiline);
            foreach (Match match in matches)
            {

                strTitle = Regex.Match(match.Value, "<h3 class=\"item\" style=\"color:#000000; cursor: default; background: transparent\">(\\s|.)+?</h3>", RegexOptions.Multiline).Value;
                strTitle = Regex.Replace(strTitle, "<h3 class=\"item\" style=\"color:#000000; cursor: default; background: transparent\">|</h3>", "");
                strTitle = Regex.Replace(strTitle, "\r\n\t\t\t\t        ", "");


                strContent = Regex.Match(match.Value, "<p>(\\s|.)+?</p>", RegexOptions.Multiline).Value;
                strContent = Regex.Replace(strContent, "<p>|</p>", "");
                strContent = Regex.Replace(strContent, "<br /><br />", "\n");
                strContent = Regex.Replace(strContent, "<br />", "\n");
                strContent = Regex.Replace(strContent, "<div>", "\n");
                strContent = Regex.Replace(strContent, "</div>", "");
                strContent = Regex.Replace(strContent, "<font (\\s|.)+?>", "");
                strContent = Regex.Replace(strContent, "</font>", "");
                strContent = Regex.Replace(strContent, "&nbsp;", "");
                strContent = Regex.Replace(strContent, "\n", "<br />");

                SaveContentDataToRow(ref dtTemp, strTitle, strContent);
            }
        }

        protected string HandleContentJSONFormat(ref DataTable dtTemp)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0; i < dtTemp.Rows.Count; i++)
            {
                tagCourseContent m_ContentRows = new tagCourseContent();
                m_ContentRows.TITLE = dtTemp.Rows[i]["TITLE"].ToString();
                m_ContentRows.CONTENT = dtTemp.Rows[i]["CONTENT"].ToString();

                DataContractJsonSerializer dj = new DataContractJsonSerializer(typeof(tagCourseContent));
                using (MemoryStream ms = new MemoryStream())
                {
                    dj.WriteObject(ms, m_ContentRows);
                    if (i > 0)
                    {
                        sb.Append(",");
                    }//end if
                    sb.Append(Encoding.UTF8.GetString(ms.ToArray()));
                }
            }//end for
            sb.Append("]");
            return sb.ToString();
        }

        protected void SaveContentDataToRow(ref DataTable dtTemp, string strTitle, string strContent)
        {
            DataRow r = dtTemp.NewRow();
            r["TITLE"] = strTitle;
            r["CONTENT"] = strContent;

            dtTemp.Rows.Add(r);
        }
    }
}