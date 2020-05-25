using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Data;
using System.Text.RegularExpressions;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Runtime.Serialization.Json;
using System.Runtime.InteropServices;

namespace Sporcard
{
    public class MyPolicy1 : ICertificatePolicy
    {
        //这个方法应该是在client和server已经进行了初步验证之后才调用的。
        //不能实现我的要求。
        public bool CheckValidationResult(ServicePoint srvPoint, X509Certificate certificate, WebRequest request, int certificateProblem)
        {
            Console.WriteLine("now CheckValidationResult.......");
            //Return True to force the certificate to be accepted.
            return true;
        } // end CheckValidationResult
    } // class MyPolicy

    public class Common
    {
        HttpWebRequest request = null;
        //byte[] aa = null;
        public Common()
        { }

        public Common(string url, string param)
        {
        }

        public string ReturnResponseValue()
        {
            try
            {
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    return sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        /// <summary>
        /// http获取数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string RetrunJSONValue(Uri url, StringBuilder data)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("X-Auth-Token", HttpUtility.UrlEncode("openstack"));
            request.Method = "POST";

            request.ContentType = "application/json";
            request.Accept = "application/json";
            byte[] byteData = Encoding.UTF8.GetBytes(data.ToString());

            using (Stream postStream = request.GetRequestStream())
            {
                postStream.Write(byteData, 0, byteData.Length);
                postStream.Close();
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// https获取数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string RetrunJSONValueByHttps(Uri url, StringBuilder data)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            //需要验证证书
            //ServicePointManager.CertificatePolicy = new MyPolicy1();
            //request.ClientCertificates.Add(X509Certificate.CreateFromCertFile("d:\\error.cer"));
            //request.KeepAlive = true;

            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            //X509Certificate cer = X509Certificate.CreateFromCertFile("你的cer证书文件");
            //request.ClientCertificates.Add(cer);

            request.Headers.Add("X-Auth-Token", HttpUtility.UrlEncode("openstack"));
            request.Method = "POST";
            request.KeepAlive = true;
            request.ContentType = "application/json";
            request.Accept = "application/json";
            byte[] byteData = Encoding.UTF8.GetBytes(data.ToString());

            using (Stream postStream = request.GetRequestStream())
            {
                postStream.Write(byteData, 0, byteData.Length);
                postStream.Close();
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                return reader.ReadToEnd();
            }
        }

        public string GetDataFromHttpService(string url)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = "GET";
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            StreamReader sr = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8);
            return sr.ReadToEnd();
        }

        /// <summary>
        /// 证书验证总是返回true
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {

            return true; //总是接受
        }

        public object GetListFromJSON(string jsondata, object list)
        {
            byte[] _Using = null;
            DataContractJsonSerializer _JSON = new DataContractJsonSerializer(list.GetType());
            if (jsondata.IndexOf('[', 0, 1) < 0)
            {
                _Using = Encoding.UTF8.GetBytes("[" + jsondata + "]");
            }
            else
            {
                _Using = Encoding.UTF8.GetBytes(jsondata);
            }
            MemoryStream _MemoryStream = new MemoryStream(_Using);
            _MemoryStream.Position = 0;
            list = (object)_JSON.ReadObject(_MemoryStream);

            return list;
        }

        public class LOGIN
        {
            public string statusCode = "";
            public string message = "";
            public string data = "";
        }
        public class YLBXINFO
        {
            public string channelcode = "";
            public string aae140 = "";
            public string id = "";
            public string name = "";
            public int pagesize = 0;
            public int pageno = 0;
        }

        /// <summary>        
        /// 将json转换为DataTable        
        /// </summary>        
        /// <param name="strJson">得到的json</param>        
        /// <returns></returns>        
        public DataTable JsonToDataTable(string strJson)
        {
            //转换json格式            
            strJson = strJson.Replace(",\"", "&\"").Replace("\":", "\"#").ToString();
            //取出表名               
            var rg = new Regex(@"(?<={)[^:]+(?=:\[)", RegexOptions.IgnoreCase);
            string strName = rg.Match(strJson).Value;
            DataTable tb = null;
            //去除表名      
            try
            {
                strJson = strJson.Substring(strJson.IndexOf("[") + 1);
                strJson = strJson.Substring(0, strJson.IndexOf("]"));
            }
            catch
            {
                strJson = strJson.Substring(strJson.IndexOf("#{") + 1);
                strJson = strJson.Substring(0, strJson.IndexOf("}") + 1);
            }

            //获取数据               
            rg = new Regex(@"(?<={)[^}]+(?=})");
            MatchCollection mc = rg.Matches(strJson);
            for (int i = 0; i < mc.Count; i++)
            {
                string strRow = mc[i].Value;
                string[] strRows = strRow.Split('&');
                //创建表                   
                if (tb == null)
                {
                    tb = new DataTable();
                    tb.TableName = strName;
                    foreach (string str in strRows)
                    {
                        var dc = new DataColumn();
                        string[] strCell = str.Split('#');
                        if (strCell[0].Substring(0, 1) == "\"")
                        {
                            int a = strCell[0].Length;
                            dc.ColumnName = strCell[0].Substring(1, a - 2);
                        }
                        else
                        {
                            dc.ColumnName = strCell[0];
                        }
                        tb.Columns.Add(dc);
                    }
                    tb.AcceptChanges();
                }
                //增加内容                   
                DataRow dr = tb.NewRow();
                for (int r = 0; r < strRows.Length; r++)
                {
                    dr[r] = strRows[r].Split('#')[1].Trim().Replace("，", ",").Replace("：", ":").Replace("\"", "");
                }
                tb.Rows.Add(dr);
                tb.AcceptChanges();
            }
            return tb;
        }
    }
}
