/*
 * NewsofDepartment - get department's news titles and its detail content
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
    public class NewsofDepartment
    {
        // use for storing news title
        public class tagNewsList
        {
            public string ID { get; set; }
            public string TITLE { get; set; }
            public string URL { get; set; }
            public DateTime NEWSDATE { get; set; }
        } // end class

        // use for storing news content
        public class tagNewsContent
        {
            public string TITLE { get; set; }
            public string CONTENT { get; set; }
        } // end class

        /**
         * GetDepartmentNewsList - Get the department's news titles
         * @iPage: which page of department website
         * Return: a json string about ID, title, URL, and date
         * 
         * This function is called to get one page news titles.
         **/
        public string GetDepartmentNewsList(int iPage)
        {
            // Fool-proofing
            if (iPage <= 0)
                iPage = 1;

            DataTable dtTemp = new DataTable("DepartmentNewsListTable");
            DataColumn c0 = new DataColumn("ID");
            dtTemp.Columns.Add(c0);
            DataColumn c1 = new DataColumn("TITLE");
            dtTemp.Columns.Add(c1);
            DataColumn c2 = new DataColumn("URL");
            dtTemp.Columns.Add(c2);
            DataColumn c3 = new DataColumn("NEWSDATE");
            dtTemp.Columns.Add(c3);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.csie.ncu.edu.tw/show.php?page=" + iPage.ToString());
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader objReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            FormatNewsListContent(ref dtTemp, objReader.ReadToEnd());
            string strTemp = HandleJSONFormat(ref dtTemp);

            response.Close();

            return strTemp;
        }

        /**
         * FormatNewsListContent - Parsing the html
         * @dtTemp: pointer to data table from GetDepartmentNewsList()
         * @strHtml: the html content from website of department news
         * 
         * This function is called for parsing the html.
         * 
         * The received html content is validated and valid factors are
         * replied to. In addition, data table(ID, Title, URL, Newsdate)
         * is configured at the end of a successful parsing.
         **/
        protected void FormatNewsListContent(ref DataTable dtTemp, string strHtml)
        {
            string strTemp;
            string strID, strTitle, strUrl, strInfoDate;

            Match matche = Regex.Match(strHtml, "<tr>\\s*<td>(\\s|.)+?<div class=\"pglink\">", RegexOptions.Multiline);
            strTemp = matche.Value;

            MatchCollection matches = Regex.Matches(strTemp, "href=(\\s|.)+?</a></td>", RegexOptions.Multiline);
            foreach (Match match in matches)
            {

                strTitle = Regex.Match(match.Value, "title=\"(\\s|.)+?\">", RegexOptions.Multiline).Value;
                strTitle = Regex.Replace(strTitle, "title=\"|\">", "");

                strUrl = Regex.Match(match.Value, "href=\"(\\s|.)+?\"", RegexOptions.Multiline).Value;
                strUrl = Regex.Replace(strUrl, "href=|\"", "");

                strID = Regex.Match(strUrl, "pno=.+?$", RegexOptions.Multiline).Value;
                strID = Regex.Replace(strID, "pno=|\\s", "");

                strInfoDate = Regex.Match(match.Value, "[(](\\d)+-(\\d)+-(\\d)+[)]", RegexOptions.Multiline).Value;
                strInfoDate = Regex.Replace(strInfoDate, "[(]|[)]", "");

                SaveDataToRow(ref dtTemp, strID, strTitle, strUrl, strInfoDate);
            }
        }

        /**
         * HandleJSONFormat - Take parsing result to json format
         * @dtTemp: pointer to data table from GetDepartmentNewsList()
         * Return: a json string about ID, title, URL, and date
         * 
         * This function is called for making json string by elements
         * (ID, Title, URL, Newsdate).
         **/
        protected string HandleJSONFormat(ref DataTable dtTemp)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0; i < dtTemp.Rows.Count; i++)
            {
                tagNewsList m_NewsRows = new tagNewsList();
                m_NewsRows.ID = dtTemp.Rows[i]["ID"].ToString();
                m_NewsRows.TITLE = dtTemp.Rows[i]["TITLE"].ToString();
                m_NewsRows.URL = "http://www.csie.ncu.edu.tw/" + dtTemp.Rows[i]["URL"].ToString();
                m_NewsRows.NEWSDATE = Convert.ToDateTime(dtTemp.Rows[i]["NEWSDATE"].ToString());

                DataContractJsonSerializer dj = new DataContractJsonSerializer(typeof(tagNewsList));
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
         * SaveDataToRow - Store parsing result to data table
         * @dtTemp: pointer to data table from GetDepartmentNewsList()
         * @strID: the news ID
         * @strTitle: the news title
         * @strUrl: the URL of news detail content
         * @strDate: the date when news be posted
         * 
         * This function is called for storing the parsing results to 
         * data table.
         **/
        protected void SaveDataToRow(ref DataTable dtTemp, string strID, string strTitle, string strUrl, string strDate)
        {
            DataRow r = dtTemp.NewRow();
            r["ID"] = strID;
            r["TITLE"] = strTitle;
            r["URL"] = strUrl;
            r["NEWSDATE"] = strDate;

            dtTemp.Rows.Add(r);
        }

        /**
         * GetDepartmentNewsContent - parse detail content of the news
         * @strUrl: the URL of news detail content
         * 
         **/
        public string GetDepartmentNewsContent(string strID)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.csie.ncu.edu.tw/show.php?cate=0&mode=view&pno=" + strID);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            StreamReader objReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string strTemp = objReader.ReadToEnd();
            strTemp = WebUtility.HtmlDecode(strTemp);
            response.Close();

            strTemp = Regex.Match(strTemp, "<td style=\"table-layout:fixed; word-break: break-all;\">(\\s|.)+?</td>", RegexOptions.Multiline).Value;
            strTemp = Regex.Replace(strTemp, "<a href(.|\\s)+?</a>", "");
            strTemp = Regex.Replace(strTemp, "<[^>]*>", "");
            strTemp = Regex.Replace(strTemp, "</p>|</div>", "");
            strTemp = Regex.Replace(strTemp, "&nbsp;", " ");

            return strTemp;
        }

    }
}