using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Data;
//using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using DataExchange;
using System.Text.RegularExpressions;

namespace Sporcard
{
    public class PlatFormInterface
    {
        INIClass ini = new INIClass(Application.StartupPath + "\\TSconfig.ini");
        public string TESTFLAG = "0";
        //string testPath = "";
        string _path = "";//接口地址
        string _path2 = "";//测试地址
        string channelcode = "";
        string devId = "";

        public string sendUnit = "";      //发送单位
        public string operatorNo = "";    //操作员号
        public string userInfo = "";      //用户信息
        public string PasswordInfo = "";  //口令信息
        public string agencyCode = "";    //机构代码  银行编码 
        public string manager = "";       //经办人
        public string adCode = "";      //行政区划代码

        private static PlatFormInterface pfi;
        private static readonly object snycRoot = new object();
        private PlatFormInterface()
        {
            TESTFLAG = ini.IniReadValue("TEST", "TESTFLAG");
            _path = ini.IniReadValue("PATH", "INTERFACEPATH");
            _path2 = ini.IniReadValue("PATH", "INTERFACEPATH2");
            //sendUnit = ini.IniReadValue("UNIT", "SendUnit");
            //operatorNo = ini.IniReadValue("UNIT", "OperatorNo");
            //userInfo = ini.IniReadValue("UNIT", "UserInfo");
            //PasswordInfo = ini.IniReadValue("UNIT", "PasswordInfo");
            //agencyCode = ini.IniReadValue("UNIT", "AgencyCode");
            //manager = ini.IniReadValue("UNIT", "Manager");
            //adCode = ini.IniReadValue("UNIT", "ADCode");
        }

        public static PlatFormInterface getInstance()
        {
            if (pfi == null)
            {
                lock (snycRoot)
                {
                    if (pfi == null)
                    {
                        pfi = new PlatFormInterface();
                    }
                }
            }
            return pfi;
        }

        public List<string> checkLogin(string username,
                                       string password,
                                       out string statusCode,
                                       out string message)
        {
            List<string> listDatas = new List<string>();
            string jsonStr = "";
            statusCode = "";
            message = "";

            Uri address = null;
            if (TESTFLAG.Trim() == "0")
                address = new Uri(_path + "/iface/busAccount/checkLogin");
            else
                address = new Uri(_path2 + "/adapter/busAccount/checkLogin");

            StringBuilder data = new StringBuilder();
            data.Append("{\"loginName\":\"" + username + "\"");
            data.Append(",\"password\":\"" + password + "\"}");

            LogHelper.WriteLog(typeof(FormMain), string.Format("checkLogin sendData :{0}", data.ToString()));

            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValueByHttps(address, data).ToString();
            }
            catch (Exception ex)
            {
                message = ex.Message;
                LogHelper.WriteLog(typeof(FormMain), string.Format("message :{0}", message));
            }

            LogHelper.WriteLog(typeof(FormMain), string.Format("checkLogin jsonStr :{0}", jsonStr));

            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                statusCode = jsonArray[0]["statusCode"].ToString();
                message = jsonArray[0]["message"].ToString();
                DataTable dt = c.JsonToDataTable(jsonStr);
                int count = dt.Rows.Count;
                if (jsonArray[0]["statusCode"].ToString() == "200")
                {
                    listDatas.Add(jsonArray[0]["data"]["userId"].ToString());//主键
                    listDatas.Add(jsonArray[0]["data"]["logName"].ToString());//登录帐号
                    listDatas.Add(jsonArray[0]["data"]["password"].ToString());//密码
                    listDatas.Add(jsonArray[0]["data"]["name"].ToString());//用户名
                    listDatas.Add(jsonArray[0]["data"]["isSocailUser"].ToString());//是否社保用户
                    listDatas.Add(jsonArray[0]["data"]["orgId"].ToString());//社保机构ID
                    listDatas.Add(jsonArray[0]["data"]["description"].ToString());//描述
                    listDatas.Add(jsonArray[0]["data"]["createTime"].ToString());//创建时间
                    listDatas.Add(jsonArray[0]["data"]["updateTime"].ToString());//更新时间
                    listDatas.Add(jsonArray[0]["data"]["idcard"].ToString());//身份证号
                    listDatas.Add(jsonArray[0]["data"]["birthday"].ToString());  //生日
                    listDatas.Add(jsonArray[0]["data"]["sex"].ToString());//性别
                    listDatas.Add(jsonArray[0]["data"]["phone"].ToString());//手机
                    listDatas.Add(jsonArray[0]["data"]["qq"].ToString());//qq号码
                    listDatas.Add(jsonArray[0]["data"]["email"].ToString());//邮箱
                    listDatas.Add(jsonArray[0]["data"]["status"].ToString());//状态，0：不可用，1：可用
                    listDatas.Add(jsonArray[0]["data"]["distCode"].ToString()); //用户所在行政区划编码
                    listDatas.Add(jsonArray[0]["data"]["bankCode"].ToString());//银行编码

                }
            }
            return listDatas;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="certnum">身份证号</param>
        /// <param name="name">姓名</param>
        /// <param name="userId">操作员id</param>
        /// <param name="distCode">操作员所属行政区划编码</param>
        /// <param name="certType">证件类型</param>
        /// <param name="cardType">卡变更类型</param>
        /// <param name="statusCode"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public List<string> checkCardFees(string certnum,
                                          string name,
                                          string userId,
                                          string distCode,
                                          string certType,
                                          string cardType,
                                          string oldyhkh,
                                          string bankNo,
                                          out string statusCode,
                                          out string message,
                                          out string sendData)
        {
            List<string> listDatas = new List<string>();
            string jsonStr = "";
            statusCode = "";
            message = "";

            Uri address = null;
            if (TESTFLAG.Trim() == "0")
                address = new Uri(_path + "/iface/busCard/checkCardFees");
            else
                address = new Uri(_path2 + "/adapter/busCard/checkCardFees");

            StringBuilder data = new StringBuilder();
            data.Append("{\"certnum\":\"" + certnum + "\"");
            data.Append(",\"name\":\"" + name + "\"");
            data.Append(",\"userId\":\"" + userId + "\"");
            data.Append(",\"yhkh\":\"" + oldyhkh + "\"");
            data.Append(",\"distCode\":\"" + distCode + "\"");
            data.Append(",\"certType\":\"" + certType + "\"");
            data.Append(",\"bankNo\":\"" + bankNo + "\"");
            data.Append(",\"cardType\":\"" + cardType + "\"}");

            sendData = data.ToString();
            LogHelper.WriteLog(typeof(FormMain), string.Format("checkCardFees sendData :{0}", sendData));

            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValueByHttps(address, data).ToString();
            }
            catch (Exception ex)
            {
                message = ex.Message;
                LogHelper.WriteLog(typeof(FormMain), string.Format("message :{0}", message));
            }

            LogHelper.WriteLog(typeof(FormMain), string.Format("checkCardFees jsonStr :{0}", jsonStr));

            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                statusCode = jsonArray[0]["statusCode"].ToString();
                message = jsonArray[0]["message"].ToString();
                if (jsonArray[0]["statusCode"].ToString() == "200")
                {
                    listDatas.Add(jsonArray[0]["data"]["name"].ToString());//姓名
                    listDatas.Add(jsonArray[0]["data"]["certnum"].ToString());//身份证
                    listDatas.Add(jsonArray[0]["data"]["fees"].ToString());//所需缴纳的费用
                    listDatas.Add(jsonArray[0]["data"]["ywdjh"].ToString());//业务单据号
                    listDatas.Add(jsonArray[0]["data"]["orderNo"].ToString());//交易流水序号                 
                }
            }
            return listDatas;
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="idcard">身份证号</param>
        /// <param name="statusCode">状态</param>
        /// <param name="message">错误信息</param>
        /// <param name="sendData">json入参</param>
        /// <returns></returns>
        public JArray searchInfo(string name, string idcard, string wdcode, string printname, string printcode, out string statusCode, out string message)
        {
            List<string> ds = new List<string>();
            string jsonStr = "";
            statusCode = "";
            message = "";

            Uri address = null;
            if (TESTFLAG == "0")
                address = new Uri(_path + "/kg/zcbSiCard!querySiCardForYh.action");
            else
                address = new Uri(_path2 + "/kg/zcbSiCard!querySiCardForYh.action");

            //测试使用
            string sendData = address.ToString() + "?sid=Y8000&aff002=" + wdcode + "&username=" + printname + "&aae011=" + printcode + "&aac147=" + idcard + "&aac003=" + name;

            //sendData = "http://161.24.9.7:8080/kgzxzcb/kg/zcbSiCard!querySiCardForYh.action?sid=Y8000&aff002=CZB0000000000&username=营业部1&aae011=00001&aac147=421125199705100910&aac003=张博";

            LogHelper.WriteLog(typeof(FormMain), string.Format("searchInfo sendData :{0}", sendData));

            Common c = new Common();
            try
            {
                LogHelper.WriteLog(typeof(FormMain), address.ToString() + "?sid=Y8000&aac002=" + idcard);
                //jsonStr = c.RetrunJSONValueByHttps(address, data).ToString();

                jsonStr = c.GetDataFromHttpService(sendData);

                #region
                //jsonStr = "{\"message\":[],\"code\":\"1\",\"output\":{\"aac004\":\"1\",\"aac003\":\"林岩\",\"aab001\":[],\"aac006\":\"19850602\",\"aac005\":\"02\",\"aac009\":\"10\",\"yaa405\":\"20291120\",\"aae008\":\"101\",\"aae007\":\"563001\",\"aae006\":\"贵州省遵义市红花岗区凤凰南路豆芽湾巷3号2单元附13号\",\"aae005\":\"18293128473\",\"aaa130\":\"CHN\",\"zp\":\"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\",\"aab004\":[],\"aac002\":\"520102198506020233\",\"aac016\":\"1\",\"ksbm\":\"520200D156000005A0067509\",\"aaz501\":\"BA0010547\",\"yaa403\":\"1.00\",\"yaa404\":\"91560000025202005202006A\",\"yae036\":\"20191120\",\"yaa402\":\"3\",\"aab301\":\"520222\",\"aac010\":\"贵州省遵义市红花岗区凤凰南路豆芽湾巷3号2单元附12号\",\"aac011\":\"21\"}}";
                #endregion
            }
            catch (Exception ex)
            {
                message = ex.Message;
                statusCode = "0";

                jsonStr ="{\"message\":\"" + message + "\"}";
                return JArray.Parse("[" + jsonStr + "]");
            }

            LogHelper.WriteLog(typeof(FormMain), string.Format("searchInfo jsonStr :{0}", jsonStr));

            JArray jsonArray = null;

            if (jsonStr != "")
            {
                jsonArray = JArray.Parse("[" + jsonStr + "]");

                statusCode = jsonArray[0]["code"].ToString();

                if (statusCode == "1")
                {
                    var dataArray = JArray.Parse("[" + jsonArray[0]["output"].ToString() + "]");

                    //dataArray[0]["zp"].Replace("");

                    return dataArray;
                }


            }
            return jsonArray;
        }

        /// <summary>
        /// 查询照片
        /// </summary>
        /// <param name="idcard">身份证号</param>
        /// <param name="statusCode">状态</param>
        /// <param name="message">报错信息</param>
        /// <param name="sendData">json数据</param>
        /// <returns></returns>
        public List<string> QueryPhoto(string idcard,//是	身份证号
                                    out string statusCode,
                                    out string message,
                                    out string sendData)
        {
            List<string> listDatas = new List<string>();
            string jsonStr = "";
            statusCode = "";
            message = "";

            Uri address = null;
            if (TESTFLAG.Trim() == "0")
                address = new Uri(_path + "/kg/zcbSiCard!querySiCardForYh.action");
            else
                address = new Uri(_path2 + "/kg/zcbSiCard!querySiCardForYh.action");


            //StringBuilder data = new StringBuilder();
            //data.Append("{\"sid\":\"" + "84" + "\"");
            //data.Append(",\"cardno\":\"" + idcard + "\"}");

            //测试使用
            sendData = address.ToString() + "?sid=84&cardno=" + idcard;

            LogHelper.WriteLog(typeof(FormMain), string.Format("doCommit sendData :{0}", sendData));

            Common c = new Common();
            try
            {
                //jsonStr = c.RetrunJSONValueByHttps(address, data).ToString();
                jsonStr = c.GetDataFromHttpService(sendData);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                LogHelper.WriteLog(typeof(FormMain), string.Format("message :{0}", message));
            }

            LogHelper.WriteLog(typeof(FormMain), string.Format("QueryPhoto jsonStr :{0}", jsonStr));
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                string parameterJAray = jsonArray[0]["parameter"].ToString();
                JArray returnArray = JArray.Parse(parameterJAray);


                for (int i = 0; i < returnArray.Count; i++)
                {
                    if (returnArray[i]["fid"].ToString() == "return")
                    {
                        statusCode = returnArray[i]["text"].ToString();
                    }
                    try
                    {
                        if (returnArray[i]["fid"].ToString() == "errMsg")
                        {
                            message = returnArray[i]["text"].ToString();
                        }
                    }
                    catch
                    {
                        continue;
                    }

                    try
                    {
                        if (returnArray[i]["fid"].ToString() == "PHOTO")
                        {
                            listDatas.Add(returnArray[i]["text"].ToString());
                            //message = returnArray[i]["text"].ToString();
                        }
                    }
                    catch
                    {
                        continue;
                    }

                }

                //if (statusCode == "1")
                //{
                //    //JArray dataArray = JArray.Parse("[" + jsonArray[0]["row"].ToString() + "]");
                //    //listDatas.Add();
                //    //JArray rowParms = JArray.Parse(jsonArray[0]["row"].ToString());
                //    listDatas.Add(rowParms[0]["fid"].ToString());
                //    listDatas.Add(rowParms[0]["ftext"].ToString());
                //    listDatas.Add(rowParms[3]["text"].ToString());
                //}
            }
            return listDatas;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errCode"></param>
        /// <param name="errMsg"></param>
        /// <param name="kfwm"></param>
        /// <param name="ksbm"></param>
        /// <param name="busId"></param>
        /// <param name="oldYhkh"></param>
        /// <param name="certnum"></param>
        /// <param name="name"></param>
        /// <param name="certType"></param>
        /// <param name="userId"></param>
        /// <param name="statusCode"></param>
        /// <param name="message"></param>
        /// <param name="sendData"></param>
        /// <returns></returns>
        public string callDataBack(string wdCode, string printName, string printCode, string idCard, string name, string ksbm, string ATR, string BankCardNo, string cardID, out string statusCode, out string message, out string sendData)
        {
            List<string> listDatas = new List<string>();
            string jsonStr = "";
            statusCode = "";
            message = "";

            Uri address = null;
            if (TESTFLAG.Trim() == "0")
                address = new Uri(_path + "/kg/zcbSiCard!querySiCardForYh.action");
            else
                address = new Uri(_path2 + "/kg/zcbSiCard!querySiCardForYh.action");

            sendData = address.ToString() + "?sid=MakeCardResult&aff002=" + wdCode + "&username=" + printName + "&aae011=" + printCode + "&isOK=1" + "&sbyy= " + "&aaz501=" + cardID + "&ksbm=" + ksbm + "&atr=" + ATR + "&yhkh=" + BankCardNo + "&aac002=" + idCard + "&aac003=" + name;
            //测试使用
            //sendData = data.ToString();
            LogHelper.WriteLog(typeof(FormMain), string.Format("callDataBack sendData:{0}", sendData));
            Common c = new Common();
            try
            {
                //jsonStr = c.RetrunJSONValueByHttps(address, data).ToString();
                jsonStr = c.GetDataFromHttpService(sendData);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                LogHelper.WriteLog(typeof(FormMain), string.Format("message :{0}", message));
            }
            LogHelper.WriteLog(typeof(FormMain), string.Format("callDataBack jsonStr:{0}", jsonStr));
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");

                statusCode = jsonArray[0]["code"].ToString();

                message = jsonArray[0]["message"].ToString();
            }
            else
            {
                statusCode = "-5";

                message = "Json值为空";

                LogHelper.WriteLog(typeof(FormMain), string.Format("jsonStr值 :{0}", jsonStr));
            }
            return statusCode;
        }



        /// <summary>
        /// 加密机报文
        /// </summary>
        /// <param name="certnum"></param>
        /// <param name="name"></param>
        /// <param name="distCode"></param>
        /// <param name="encryption"></param>
        /// <param name="userId"></param>
        /// <param name="orderNo"></param>
        /// <param name="ywdjh"></param>
        /// <param name="certType"></param>
        /// <param name="oldyhkh"></param>
        /// <param name="ssbm"></param>
        /// <param name="statusCode"></param>
        /// <param name="message"></param>
        /// <param name="sendData"></param>
        /// <returns></returns>
        public string getEncryptor(string certnum,  // 是	渠道编码
                                        string name,  //     是	设备编码
                                        string distCode,  // 是	权限验证码
                                        string encryption,	//是	用户姓名
                                        string userId,	//    是	身份证号码
                                        string orderNo,  // 是	交易流水号
                                        string ywdjh,	//    是	业务单据号
                                        string certType,  // 是	证件类别
                                        string oldyhkh,
                                        string ssbm,//省个人识别号
                                        out string statusCode,
                                        out string message,
                                        out string sendData)
        {
            //List<string> listDatas = new List<string>();
            string jmjbm = "";
            string jsonStr = "";
            statusCode = "";
            message = "";

            Uri address = null;
            if (TESTFLAG.Trim() == "0")
                address = new Uri(_path + "/iface/busCard/getEncryptor");
            else
                address = new Uri(_path2 + "/adapter/busCard/getEncryptor");



            StringBuilder data = new StringBuilder();
            data.Append("{\"certnum\":\"" + certnum + "\"");
            data.Append(",\"name\":\"" + name + "\"");
            data.Append(",\"distCode\":\"" + distCode + "\"");
            data.Append(",\"encryption\":\"" + encryption + "\"");
            data.Append(",\"userId\":\"" + userId + "\"");
            data.Append(",\"orderNo\":\"" + orderNo + "\"");
            data.Append(",\"ywdjh\":\"" + ywdjh + "\"");
            data.Append(",\"yhkh\":\"" + oldyhkh + "\"");
            data.Append(",\"ssbm\":\"" + ssbm + "\"");//添加的省个人识别号
            data.Append(",\"certType\":\"" + certType + "\"}");

            //测试使用
            sendData = data.ToString();
            LogHelper.WriteLog(typeof(FormMain), string.Format("getEncryptor sendData:{0}", sendData));
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValueByHttps(address, data).ToString();
            }
            catch (Exception ex)
            {
                message = ex.Message;
                LogHelper.WriteLog(typeof(FormMain), string.Format("message :{0}", message));
            }

            LogHelper.WriteLog(typeof(FormMain), string.Format("getEncryptor jsonStr:{0}", jsonStr));
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                statusCode = jsonArray[0]["statusCode"].ToString();
                message = jsonArray[0]["message"].ToString();
                if (jsonArray[0]["statusCode"].ToString() == "200")
                {
                    jmjbm = jsonArray[0]["data"]["encryption"].ToString();//加密应答报文            
                }
            }
            return jmjbm;
        }

        /// <summary>
        /// 和银行通信  获取制卡数据
        /// </summary>
        /// <param name="username"></param>
        /// <param name="identy"></param>
        /// <param name="oldBankNo"></param>
        /// <param name="orgId"></param>
        /// <param name="zdh"></param>
        /// <param name="fsdw"></param>
        /// <param name="jsdw"></param>
        /// <param name="wdbm"></param>
        /// <param name="statusCode"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public List<string> getMarkCardData(string username,
                                            string identy,
                                            string oldBankNo,
                                            string orgId,
                                            string zdh,
                                            string fsdw,
                                            string jsdw,
                                            string wdbm,
                                            out string statusCode,
                                            out string message)
        {
            List<string> listDatas = new List<string>();
            string jsonStr = "";
            statusCode = "";
            message = "";

            Uri address = null;
            if (TESTFLAG.Trim() == "0")
                address = new Uri(_path + "/iface/busAccount/getMarkCardData");
            else
                address = new Uri(_path2 + "/adapter/busCard/getMarkCardData");

            StringBuilder data = new StringBuilder();
            data.Append("{\"jbr\":\"" + username + "\"");//经办人 GlobalClass.loginName           
            data.Append(",\"baz923\":\"" + oldBankNo + "\"");//原银行卡号
            data.Append(",\"aae135\":\"" + identy + "\"");//证件号码

            data.Append(",\"zdh\":\"" + zdh + "\"");//
            data.Append(",\"wdbm\":\"" + wdbm + "\""); //网点编码           
            data.Append(",\"jsdw\":\"" + jsdw + "\"");//接收单位// KGXT
            data.Append(",\"fsdw\":\"" + fsdw + "\"");//发送单位 //YH05

            data.Append(",\"jkdm\":\"" + "G88201" + "\"");//接口代码
            data.Append(",\"bwywlx\":\"" + "G88201" + "\"");//报文业务类型            

            data.Append(",\"jgdm\":\"" + orgId + "\"}");//机构代码 


            LogHelper.WriteLog(typeof(FormMain), string.Format("getMarkCardData sendData :{0}", data.ToString()));

            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValueByHttps(address, data).ToString();
            }
            catch (Exception ex)
            {
                message = ex.Message;
                LogHelper.WriteLog(typeof(FormMain), string.Format("message :{0}", message));
            }

            LogHelper.WriteLog(typeof(FormMain), string.Format("getMarkCardData jsonStr :{0}", jsonStr));

            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                statusCode = jsonArray[0]["statusCode"].ToString();
                message = jsonArray[0]["message"].ToString();
                DataTable dt = c.JsonToDataTable(jsonStr);
                int count = dt.Rows.Count;
                if (jsonArray[0]["statusCode"].ToString() == "200")
                {
                    listDatas.Add(jsonArray[0]["data"]["JGDM"].ToString());     //0  机构代码
                    listDatas.Add(jsonArray[0]["data"]["JBR"].ToString());      //1  经办人
                    listDatas.Add(jsonArray[0]["data"]["JYLSH"].ToString());    //2  交易流水号
                    listDatas.Add(jsonArray[0]["data"]["BWYWLX"].ToString());   //3  报文业务类型
                    listDatas.Add(jsonArray[0]["data"]["BWFSRQ"].ToString());   //4  报文发送日期
                    listDatas.Add(jsonArray[0]["data"]["BWFSSJ"].ToString());   //5  报文发送时间
                    listDatas.Add(jsonArray[0]["data"]["XYM"].ToString());      //6  响应码
                    listDatas.Add(jsonArray[0]["data"]["XYXX"].ToString());     //7  响应信息
                    listDatas.Add(jsonArray[0]["data"]["BY1"].ToString());      //8  备用1:卡变更类型
                    listDatas.Add(jsonArray[0]["data"]["BY2"].ToString());      //9  备用2:银行网点编码
                    listDatas.Add(jsonArray[0]["data"]["BY3"].ToString());      //10 备用3: 业务单据号
                    listDatas.Add(jsonArray[0]["data"]["AAC003"].ToString());   //11 姓名
                    listDatas.Add(jsonArray[0]["data"]["AAC058"].ToString());   //12 证件类型
                    listDatas.Add(jsonArray[0]["data"]["AAE135"].ToString());   //13 证件号码
                    listDatas.Add(jsonArray[0]["data"]["BAZ805"].ToString());   //14 省个人识别号
                    listDatas.Add(jsonArray[0]["data"]["BAZ923"].ToString());   //15 原银行卡号
                    listDatas.Add(jsonArray[0]["data"]["AAA027"].ToString());   //16 行政区划编码
                    listDatas.Add(jsonArray[0]["data"]["AAZ502"].ToString());   //17 卡状态
                }
            }
            return listDatas;
        }

        /// <summary>
        /// 向银行系统请求批量制卡
        /// </summary>
        /// <param name="wdbh">网点编号</param>
        /// <param name="jbr">经办人</param>
        /// <param name="jgdm">机构代码</param>
        /// <param name="statusCode"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public DataTable getBatchMarkCardData(string wdbh,
                                              string jbr,
                                              string jgdm,
                                              out string statusCode,
                                              out string message)
        {
            List<string> listDatas = new List<string>();
            string jsonStr = "";
            statusCode = "";
            message = "";
            DataTable dt = null;

            Uri address = null;
            if (TESTFLAG == "0")
                address = new Uri(_path + "/iface/busCard/batchMarkCardData");
            else
                address = new Uri(_path2 + "/adapter/busCard/batchMarkCardData");


            StringBuilder data = new StringBuilder();
            data.Append("{\"wdbm\":\"" + wdbh + "\"");
            data.Append(",\"jbr\":\"" + jbr + "\"");
            data.Append(",\"aae135\":\"" + "" + "\"");
            data.Append(",\"baz923\":\"" + "" + "\"");
            data.Append(",\"jkdm\":\"" + "G88301" + "\"");
            data.Append(",\"jgdm\":\"" + jgdm + "\"");
            data.Append(",\"fsdw\":\"" + "YH05" + "\"");
            data.Append(",\"jsdw\":\"" + "KGXT" + "\"");
            data.Append(",\"bwywlx\":\"" + "G88301" + "\"}");

            LogHelper.WriteLog(typeof(FormMain), string.Format("searchInfo sendData :{0}", data.ToString()));

            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValueByHttps(address, data).ToString();
                //jsonStr = "{\"statusCode\":\"200\",\"message\":\"操作成功\",\"data\":[{\"aac058\":\"1\",\"aae135\":\"451423199602049565\",\"aac003\":\"曹怀刚1\",\"baz923\":\"6217562700000002096\",\"baz805\":\"8181784552\",\"aaa027\":\"027\",\"aaz502\":\"01\",\"by1\":\"02\",\"by2\":\"99556\",\"by3\":\"0000000\",\"createtime\":null,\"batchid\":\"010101000120170916103133.txt\"},{\"aac058\":\"1\",\"aae135\":\"451423199602049565\",\"aac003\":\"李小明2\",\"baz923\":\"6217562700000002096\",\"baz805\":\"8181784552\",\"aaa027\":\"027\",\"aaz502\":\"01\",\"by1\":\"02\",\"by2\":\"99556\",\"by3\":\"0000000\",\"createtime\":null,\"batchid\":\"010101000120170916103133.txt\"}]}";
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            LogHelper.WriteLog(typeof(FormMain), string.Format("searchInfo jsonStr :{0}", jsonStr));

            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                statusCode = jsonArray[0]["statusCode"].ToString();
                message = jsonArray[0]["message"].ToString();
                if (jsonArray[0]["statusCode"].ToString() == "200")
                {
                    dt = c.JsonToDataTable(jsonStr);
                }
            }
            return dt;
        }

        /// <summary>
        /// 返回制卡结果，给银行
        /// </summary>
        /// <param name="id"></param>
        /// <param name="orgId"></param>
        /// <param name="ssbm"></param>
        /// <param name="oldBankNo"></param>
        /// <param name="newBankNo"></param>
        /// <param name="result"></param>
        /// <param name="statusCode"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public List<string> getMarkCardResult(string id,
                                              string orgId,
                                              string ssbm,
                                              string oldBankNo,
                                              string newBankNo,
                                              string result,
                                              out string statusCode,
                                              out string message)
        {
            List<string> listDatas = new List<string>();
            string jsonStr = "";
            statusCode = "";
            message = "";

            Uri address = null;
            if (TESTFLAG.Trim() == "0")
                address = new Uri(_path + "/iface/busAccount/sendMarkCardData");
            else
                address = new Uri(_path2 + "/adapter/busCard/sendMarkCardData");


            StringBuilder data = new StringBuilder();
            data.Append("{\"aae135\":\"" + id + "\"");//证件号码
            data.Append(",\"aae010\":\"" + newBankNo + "\"");//补换后新的银行卡号
            data.Append(",\"baz923\":\"" + oldBankNo + "\"");//补换前的银行卡号
            data.Append(",\"jgdm\":\"" + result + "\"");//制卡结果
            data.Append(",\"bwywlx\":\"" + "G88202" + "\"");//报文业务类型
            data.Append(",\"jkdm\":\"" + "G88202" + "\"");
            data.Append(",\"fsdw\":\"" + "YH05" + "\"");
            data.Append(",\"jsdw\":\"" + "KGXT" + "\"");

            data.Append(",\"by1\":\"" + orgId + "\"}");//机构代码

            LogHelper.WriteLog(typeof(FormMain), string.Format("getMarkCardData sendData :{0}", data.ToString()));

            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValueByHttps(address, data).ToString();
            }
            catch (Exception ex)
            {
                message = ex.Message;
                LogHelper.WriteLog(typeof(FormMain), string.Format("message :{0}", message));
            }

            LogHelper.WriteLog(typeof(FormMain), string.Format("getMarkCardData jsonStr :{0}", jsonStr));

            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                statusCode = jsonArray[0]["statusCode"].ToString();
                message = jsonArray[0]["message"].ToString();
                DataTable dt = c.JsonToDataTable(jsonStr);
                int count = dt.Rows.Count;
                if (jsonArray[0]["statusCode"].ToString() == "200")
                {
                    listDatas.Add(jsonArray[0]["data"]["XYM"].ToString());//响应码
                    listDatas.Add(jsonArray[0]["data"]["XYXX"].ToString());//响应信息
                }
            }
            return listDatas;
        }







        #region"省级系统"


        /// <summary>
        /// 获取卡状态信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public string GetStatusInfo(string name, string idcard, string token, out string statusCode)
        {
            string jsonStr = "";
            statusCode = "";
            string message = "";
            Uri address = new Uri(_path2 + "/iface/card/cardStatus?deviceid=" + devId + "&tokenId=" + token);
            StringBuilder data = new StringBuilder();
            data.Append("{\"aac002\":\"" + idcard + "\"");
            data.Append(",\"aac003\":\"" + name + "\"}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValueByHttps(address, data).ToString();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                statusCode = jsonArray[0]["statusCode"].ToString();
                if (message == "")
                {
                    message = jsonArray[0]["message"].ToString();
                }
            }
            return message;
        }

        /// <summary>
        /// 获取制卡进度信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public List<string> GetMakeCardProgress(string name, string idcard, string token, out string statusCode, out string picihao, out string bankCode, out string boxIndex)
        {
            string jsonStr = "";
            statusCode = "";
            picihao = "";
            bankCode = "";
            boxIndex = "";
            List<string> list = new List<string>();
            Uri address = new Uri(_path2 + "/iface/card/cardProgress?deviceid=" + devId + "&tokenId=" + token);
            StringBuilder data = new StringBuilder();
            data.Append("{\"aac002\":\"" + idcard + "\"");
            data.Append(",\"aac003\":\"" + name + "\"}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValueByHttps(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null)
                {
                    statusCode = jsonArray[0]["statusCode"].ToString();
                    list.Add(jsonArray[0]["data"]["APPLYTIME"].ToString());
                    list.Add(jsonArray[0]["data"]["BANKTIME0"].ToString());
                    list.Add(jsonArray[0]["data"]["BANKFINISHTIME0"].ToString());
                    list.Add(jsonArray[0]["data"]["INSURETIME"].ToString());
                    list.Add(jsonArray[0]["data"]["INSUREFINISHTIME"].ToString());
                    list.Add(jsonArray[0]["data"]["PROVINCETIME"].ToString());
                    list.Add(jsonArray[0]["data"]["CITYTIME"].ToString());
                    list.Add(jsonArray[0]["data"]["GETTIME"].ToString());

                    picihao = jsonArray[0]["data"]["BATCHNO"].ToString();
                    bankCode = jsonArray[0]["data"]["AAE008"].ToString();
                    boxIndex = jsonArray[0]["data"]["ZXWZ"].ToString();
                }
                else
                {
                    statusCode = jsonArray[0]["message"].ToString();
                }
            }
            return list;
        }

        /// <summary>
        /// 获取制卡进度信息
        /// </summary>
        /// <param name="name"></param>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public List<string> GetMakeCardProgress2(string name, string idcard, string token, out string statusCode, out string message)
        {
            string jsonStr = "";
            statusCode = "";
            message = "";
            List<string> list = new List<string>();
            Uri address = new Uri(_path + "/iface/card/getCardProgress?deviceid=" + devId + "&tokenId=" + token);
            StringBuilder data = new StringBuilder();

            data.Append("{\"aac003\":\"" + name + "\"");
            //data.Append(",\"tokenId\":\"" + token + "\"");
            //data.Append(",\"deviceid\":\"" + devId + "\"");
            data.Append(",\"channelcode\":\"" + "SelfService" + "\"");
            data.Append(",\"aac002\":\"" + idcard + "\"}");

            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValueByHttps(address, data).ToString();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null)
                {
                    statusCode = jsonArray[0]["statusCode"].ToString();

                    list.Add(jsonArray[0]["data"]["aac003"].ToString());//姓名 0
                    list.Add(jsonArray[0]["data"]["aac002"].ToString());//身份证号码 1
                    list.Add(jsonArray[0]["data"]["aaz500"].ToString());//社会保障卡号 2
                    list.Add(jsonArray[0]["data"]["aae008"].ToString());//开户银行 3
                    list.Add(jsonArray[0]["data"]["applytime"].ToString());//申请时间 4
                    list.Add(jsonArray[0]["data"]["zkzt"].ToString());//制卡状态 5
                    list.Add(jsonArray[0]["data"]["transacttype"].ToString());//人员类别 6
                    list.Add(jsonArray[0]["data"]["zxwz"].ToString());//装箱位置 7
                    list.Add(jsonArray[0]["data"]["ffdd"].ToString());//发放地点 8
                    list.Add(jsonArray[0]["data"]["dwmc"].ToString());//单位名称 9
                    list.Add(jsonArray[0]["data"]["dabh"].ToString());//单位名称 10
                }
                else
                {
                    statusCode = jsonArray[0]["statusCode"].ToString();
                    message = jsonArray[0]["message"].ToString();
                }
            }
            return list;
        }


        /// <summary>
        /// 获取卡数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public List<string> GetCardData(string name, string idcard, string token, out string statusCode, out string message)
        {
            string jsonStr = "";
            statusCode = "";
            message = "";
            List<string> list = new List<string>();
            Uri address = new Uri(_path2 + "/iface/card/cardInfo?deviceid=" + devId + "&tokenId=" + token);
            StringBuilder data = new StringBuilder();
            data.Append("{\"aac002\":\"" + idcard + "\"");
            data.Append(",\"aac003\":\"" + name + "\"}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null)
                {
                    statusCode = jsonArray[0]["statusCode"].ToString();
                    list.Add(jsonArray[0]["data"]["AAC003"].ToString());//姓名
                    list.Add(jsonArray[0]["data"]["AAC004"].ToString());//性别
                    list.Add(jsonArray[0]["data"]["AAC005"].ToString());//民族
                    list.Add(jsonArray[0]["data"]["AAZ500"].ToString());//社保卡号
                    list.Add(jsonArray[0]["data"]["AAC002"].ToString());//身份证号
                    list.Add(jsonArray[0]["data"]["ZJYXQ"].ToString());//证件有效期
                    list.Add(jsonArray[0]["data"]["AAE005"].ToString());//电话号码
                    list.Add(jsonArray[0]["data"]["AAC008"].ToString());//人员状态
                    list.Add(jsonArray[0]["data"]["AAB301"].ToString());//统筹区域
                    list.Add(jsonArray[0]["data"]["AAB001"].ToString());//单位编码
                    list.Add(jsonArray[0]["data"]["AAB004"].ToString());//单位名称
                    list.Add(jsonArray[0]["data"]["AAE006"].ToString());//通讯地址
                    list.Add(jsonArray[0]["data"]["JHRXM"].ToString());//监护人姓名
                    list.Add(jsonArray[0]["data"]["JHRZH"].ToString());//监护人证号
                    list.Add(jsonArray[0]["data"]["PHOTO"].ToString());//个人相片
                }
                else
                {
                    message = jsonArray[0]["message"].ToString();
                }
            }

            return list;
        }

        /// <summary>
        /// 卡挂失
        /// </summary>
        /// <param name="name"></param>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public string ReportLoss(string name, string idcard, string fwmm, string token, out string statusCode)
        {
            string jsonStr = "";
            statusCode = "";
            string message = "";
            //Uri address = new Uri(_path2 + "/iface/card/cardLossReport?deviceid=" + devId + "&tokenId=" + token);
            Uri address = new Uri(_path + "/iface/rest/tInterimReportLoss?deviceid=" + devId + "&tokenId=" + token);
            StringBuilder data = new StringBuilder();
            data.Append("{\"sfzhm\":\"" + idcard + "\"");
            data.Append(",\"aac003\":\"" + name + "\"");
            data.Append(",\"fwmm\":\"" + fwmm + "\"}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValueByHttps(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200")
                {
                    statusCode = jsonArray[0]["statusCode"].ToString();
                }
                message = jsonArray[0]["message"].ToString();
            }
            return message;
        }

        /// <summary>
        /// 修改服务密码
        /// </summary>
        /// <param name="name"></param>
        /// <param name="idcard"></param>
        /// <param name="oldpass"></param>
        /// <param name="newpass"></param>
        /// <param name="renewpass"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public string UpServicePwd(string name, string idcard, string oldpass, string newpass, string renewpass, string token, out string statusCode)
        {
            string jsonStr = "";
            statusCode = "";
            string message = "";
            Uri address = new Uri(_path + "/iface/card/upServicePwd?deviceid=" + devId + "&tokenId=" + token);
            StringBuilder data = new StringBuilder();
            data.Append("{\"sfzhm\":\"" + idcard + "\"");
            data.Append(",\"aac003\":\"" + name + "\"");
            data.Append(",\"oldpass\":\"" + oldpass + "\"");
            data.Append(",\"newpass\":\"" + newpass + "\"");
            data.Append(",\"renewpass\":\"" + renewpass + "\"");
            data.Append(",\"channelcode\":\"" + channelcode + "\"}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValueByHttps(address, data).ToString();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200")
                {
                    statusCode = jsonArray[0]["statusCode"].ToString();
                }
                message = jsonArray[0]["message"].ToString();
            }
            return message;
        }

        /// <summary>
        /// 城乡养老个人信息
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public List<string> GetCityAndCountryEndowBaseInfo(string idcard, string token, out string statusCode, out string message)
        {
            string jsonStr = "";
            statusCode = "";
            message = "";
            List<string> list = new List<string>();
            Uri address = new Uri(_path2 + "/iface/so/getEndowInfoPerson?deviceid=" + devId + "&tokenId=" + token);
            StringBuilder data = new StringBuilder();
            data.Append("{\"aac002\":\"" + idcard + "\"}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValueByHttps(address, data).ToString();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null)
                {
                    statusCode = jsonArray[0]["statusCode"].ToString();
                    list.Add(jsonArray[0]["data"]["aac001"].ToString());//个人编号
                    list.Add(jsonArray[0]["data"]["aac002"].ToString());//身份证
                    list.Add(jsonArray[0]["data"]["aac003"].ToString());//姓名
                    list.Add(jsonArray[0]["data"]["aac049"].ToString());//参保日期
                    list.Add(jsonArray[0]["data"]["aae094"].ToString());//累计缴费年限
                    list.Add(jsonArray[0]["data"]["aac031"].ToString());//参保状态
                    list.Add(jsonArray[0]["data"]["aae141"].ToString());//最近一年缴费档次
                    list.Add(jsonArray[0]["data"]["aac012"].ToString());//身份类型
                    //list.Add(jsonArray[0]["data"]["aaf116"].ToString());//区县名称（统筹区县）
                }
                else
                {
                    statusCode = jsonArray[0]["statusCode"].ToString();
                    message = jsonArray[0]["message"].ToString();
                }
            }
            return list;
        }


        /// <summary>
        /// 城乡养老个人缴费
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable GetCityAndCountryEndowPayPerson(string idcard, string token, out string statusCode, int page, out int totalRow)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            int pageSize = 5;
            DataTable dt = new DataTable();
            Uri address = new Uri(_path2 + "/iface/so/getEndowPayPerson?deviceid=" + devId + "&tokenId=" + token);
            StringBuilder data = new StringBuilder();
            data.Append("{\"aac002\":\"" + idcard + "\"");
            data.Append(",\"pageNo\":" + page);
            data.Append(",\"pageSize\":" + pageSize + "}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValueByHttps(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null)
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["data"]["rowTotal"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
                else
                {
                    statusCode = jsonArray[0]["message"].ToString();
                }
            }
            return dt;
        }


        /// <summary>
        /// 城乡养老个人账户
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable GetCityAndCountryEndowAccountPerson(string idcard, string token, out string statusCode, int page, out int totalRow)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            int pageSize = 5;
            DataTable dt = new DataTable();
            Uri address = new Uri(_path2 + "/iface/so/getEndowAccountPerson?deviceid=" + devId + "&tokenId=" + token);
            StringBuilder data = new StringBuilder();
            data.Append("{\"aac002\":\"" + idcard + "\"");
            data.Append(",\"pageNo\":" + page);
            data.Append(",\"pageSize\":" + pageSize + "}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValueByHttps(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null)
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["data"]["rowTotal"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
                else
                {
                    statusCode = jsonArray[0]["message"].ToString();
                }
            }
            return dt;
        }

        /// <summary>
        /// 城乡养老金发放信息
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable GetCityAndCountryEndowAnnuityPerson(string idcard, string token, out string statusCode, int page, out int totalRow)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            int pageSize = 5;
            DataTable dt = new DataTable();
            Uri address = new Uri(_path2 + "/iface/so/getEndowAnnuityPerson?deviceid=" + devId + "&tokenId=" + token);
            StringBuilder data = new StringBuilder();
            data.Append("{\"aac002\":\"" + idcard + "\"");
            data.Append(",\"pageNo\":" + page);
            data.Append(",\"pageSize\":" + pageSize + "}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValueByHttps(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null)
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["data"]["rowTotal"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
                else
                {
                    statusCode = jsonArray[0]["message"].ToString();
                }
            }
            return dt;
        }

        /// <summary>
        /// 城乡养老保险缴费标准
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable GetEndowPayStandardPerson(string idcard, string token, out string statusCode, int page, out int totalRow)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            int pageSize = 5;
            DataTable dt = new DataTable();
            Uri address = new Uri(_path2 + "/iface/so/getEndowPayStandardPerson?deviceid=" + devId + "&tokenId=" + token);
            StringBuilder data = new StringBuilder();
            data.Append("{\"aac002\":\"" + idcard + "\"");
            data.Append(",\"pageNo\":" + page);
            data.Append(",\"pageSize\":" + pageSize + "}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValueByHttps(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null)
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["data"]["rowTotal"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
                else
                {
                    statusCode = jsonArray[0]["message"].ToString();
                }
            }
            return dt;
        }

        #endregion

        #region"市级系统"

        /// <summary>
        /// 获取登录信息(社保业务)
        /// </summary>
        /// <param name="path">配置文件中路径名称</param>
        /// <param name="statusCode">状态代码</param>
        /// <returns></returns>
        public string GetLoginInfo2(out string statusCode)
        {
            string jsonStr = "";
            string token = "";
            statusCode = "";
            Uri address = new Uri(_path + "/iface/user/checkLogin");

            string channelCode = ini.IniReadValue("CHANNEL", "CHANNELCODE");
            string username = ini.IniReadValue("USER", "ZKUSER");
            string password = ini.IniReadValue("PASSWORD", "ZKPASSWORD");

            StringBuilder data = new StringBuilder();
            data.Append("{\"type\":\"" + channelCode + "\"");
            data.Append(",\"userName\":\"" + username + "\"");
            data.Append(",\"netpassword\":\"" + password + "\"}");

            Common c = new Common();

            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200")
                {
                    statusCode = jsonArray[0]["statusCode"].ToString();
                    token = jsonArray[0]["data"].ToString();
                }
            }
            return token;
        }

        /// <summary>
        /// 企业养老保险个人信息查询
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public List<string> GetEndowBaseInfo(string idcard, string token, out string statusCode, string name)
        {
            string jsonStr = "";
            statusCode = "";
            string channelCode = "SelfService";
            List<string> list = new List<string>();
            Uri address = new Uri(_path + "/iface/rest/getPensionPerInfo?tokenId=" + token);
            StringBuilder data = new StringBuilder();
            data.Append("{\"aac002\":\"" + idcard + "\"");
            data.Append(",\"aac003\":\"" + name + "\"");
            data.Append(",\"channelcode\":\"" + channelCode + "\"}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null)
                {
                    statusCode = jsonArray[0]["statusCode"].ToString();
                    list.Add(jsonArray[0]["data"]["aac003"].ToString());//姓名
                    list.Add(jsonArray[0]["data"]["aac004"].ToString());//性别
                    list.Add(jsonArray[0]["data"]["aac006"].ToString());//出生年月
                    list.Add(jsonArray[0]["data"]["aac002"].ToString());//身份证号码
                    list.Add(jsonArray[0]["data"]["aac016"].ToString());//就业状态
                    list.Add(jsonArray[0]["data"]["aac005"].ToString());//民族
                    list.Add(jsonArray[0]["data"]["aac009"].ToString());//户口性质
                    list.Add(jsonArray[0]["data"]["aac001"].ToString());//个人编号
                    list.Add(jsonArray[0]["data"]["aab001"].ToString());//单位编号
                    list.Add(jsonArray[0]["data"]["aac007"].ToString());//参加工作日期

                    //list.Add(jsonArray[0]["data"]["aaa029"].ToString());//证件类型
                    //list.Add(jsonArray[0]["data"]["aac002"].ToString());//社会保障号
                    //list.Add(jsonArray[0]["data"]["aae135"].ToString());//证件号码
                    //list.Add(jsonArray[0]["data"]["aab301"].ToString());//常住地所属行政区代码
                    //list.Add(jsonArray[0]["data"]["aac010"].ToString());//户口所在地
                    //list.Add(jsonArray[0]["data"]["aac011"].ToString());//文化程度
                    //list.Add(jsonArray[0]["data"]["aac012"].ToString());//个人身份
                    //list.Add(jsonArray[0]["data"]["aac014"].ToString());//专业技术职务
                    //list.Add(jsonArray[0]["data"]["aac015"].ToString());//国家职业资格等级(工人技术等级)
                    //list.Add(jsonArray[0]["data"]["aac017"].ToString());//婚姻状况
                    //list.Add(jsonArray[0]["data"]["aac020"].ToString());//行政职务(级别)
                    //list.Add(jsonArray[0]["data"]["aae005"].ToString());//联系电话
                    //list.Add(jsonArray[0]["data"]["aae006"].ToString());//地址
                    //list.Add(jsonArray[0]["data"]["aae007"].ToString());//邮政编码
                    //list.Add(jsonArray[0]["data"]["aae159"].ToString());//联系电子邮箱
                    //list.Add(jsonArray[0]["data"]["aab401"].ToString());//户籍证明编号
                    //list.Add(jsonArray[0]["data"]["aae147"].ToString());//本人定居生活地
                    //list.Add(jsonArray[0]["data"]["eac101"].ToString());//手机
                    //list.Add(jsonArray[0]["data"]["aac028"].ToString());//农民工标识
                    //list.Add(jsonArray[0]["data"]["aae013"].ToString());//备注
                    //list.Add(jsonArray[0]["data"]["aac060"].ToString());//生存状态
                }
            }
            return list;
        }

        /// <summary>
        /// 企业养老参保信息
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable GetPensionInsureInfo(string idcard, string token, out string statusCode, int page, out int totalRow, string name)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            string channelCode = "SelfService";
            int pageSize = 5;
            DataTable dt = new DataTable();
            Uri address = new Uri(_path + "/iface/rest/getPensionInsureInfo?tokenId=" + token);
            StringBuilder data = new StringBuilder();
            data.Append("{\"aae135\":\"" + idcard + "\"");
            data.Append(",\"aac003\":\"" + name + "\"");
            data.Append(",\"channelcode\":\"" + channelCode + "\"");
            //data.Append(",\"deviceid\":\"" + devId + "\"");
            data.Append(",\"pageno\":" + page);
            data.Append(",\"pagesize\":" + pageSize + "}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null)
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["total"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
                else
                {
                    statusCode = jsonArray[0]["message"].ToString();
                }
            }
            return dt;
        }


        /// <summary>
        /// 企业养老参保信息（返回List）
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public List<string> GetEndowInfoPerson(string idcard, string token, out string statusCode, string name)
        {
            string jsonStr = "";
            statusCode = "";
            string channelCode = "SelfService";
            List<string> list = new List<string>();
            Uri address = new Uri(_path + "/iface/rest/getPensionInsureInfo?tokenId=" + token);
            StringBuilder data = new StringBuilder();
            data.Append("{\"aac002\":\"" + idcard + "\"");
            data.Append(",\"aac003\":\"" + name + "\"");
            data.Append(",\"channelcode\":\"" + channelCode + "\"");
            data.Append(",\"deviceid\":\"" + devId + "\"}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null)
                {
                    statusCode = jsonArray[0]["statusCode"].ToString();
                    list.Add(jsonArray[0]["data"]["aac003"].ToString());//姓名
                    list.Add(jsonArray[0]["data"]["aac001"].ToString());//个人编号
                    list.Add(jsonArray[0]["data"]["aae140"].ToString());//险种类型
                    list.Add(jsonArray[0]["data"]["aae041"].ToString());//开始年月
                    list.Add(jsonArray[0]["data"]["aae042"].ToString());//终止年月
                    list.Add(jsonArray[0]["data"]["aac031"].ToString());//参保状态
                    list.Add(jsonArray[0]["data"]["aac037"].ToString());//缴费状态
                    list.Add(jsonArray[0]["data"]["aac130"].ToString());//首次参保时间
                    list.Add(jsonArray[0]["data"]["aac002"].ToString());//身份证号

                    //list.Add(jsonArray[0]["data"]["aae030"].ToString());//开始年月
                    //list.Add(jsonArray[0]["data"]["aae031"].ToString());//结束年月
                    //list.Add(jsonArray[0]["data"]["aic020"].ToString());//缴费基数
                    //list.Add(jsonArray[0]["data"]["aac031"].ToString());//个人缴费状态
                    //list.Add(jsonArray[0]["data"]["eac250"].ToString());//记录状态标志
                    //list.Add(jsonArray[0]["data"]["eac114"].ToString());//本次参保日期
                }
            }
            return list;
        }

        /// <summary>
        /// 企业养老个人缴费
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable GetEndowPayPerson(string idcard, string token, out string statusCode, int page, out int totalRow, string name)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            string channelCode = "SelfService";
            DataTable dt = new DataTable();
            Uri address = new Uri(_path + "/iface/rest/getPensionPayInfo?tokenId=" + token);
            int pageSize = 5;
            StringBuilder data = new StringBuilder();
            data.Append("{\"aac002\":\"" + idcard + "\"");
            data.Append(",\"aac003\":\"" + name + "\"");
            data.Append(",\"channelcode\":\"" + channelCode + "\"");
            data.Append(",\"pageNo\":\"" + page + "\"");
            data.Append(",\"pageSize\":\"" + pageSize + "\"}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null)
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["data"]["count"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
                else
                {
                    statusCode = jsonArray[0]["message"].ToString();
                }
            }
            return dt;
        }

        /// <summary>
        /// 企业养老个人缴费
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable GetEndowPayPerson2(string idcard, string token, out string statusCode, int page, out int totalRow, string name, string unitcode)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            string channelCode = "SelfService";
            string aae135 = idcard;
            //string aaa029 = "1";
            string aac003 = name;
            string aae078 = "0";
            DataTable dt = new DataTable();
            //Uri address = new Uri(_path + "/iface/so/getEndowPayPerson?devId=827665329374&token=" + token);
            Uri address = new Uri(_path + "/iface/manager/ab43?tokenId=" + token + "&deviceid=" + devId);
            //idcard = "412829198906262818";
            int pageNo = page;
            //int pageSize = 5;
            StringBuilder data = new StringBuilder();
            data.Append("{\"aae135\":\"" + aae135 + "\"");
            //data.Append(",\"aaa029\":\"" + aaa029 + "\"");
            data.Append(",\"aac003\":\"" + aac003 + "\"");
            data.Append(",\"aae078\":\"" + aae078 + "\"");
            data.Append(",\"aaz001\":\"" + unitcode + "\"");
            data.Append(",\"channelcode\":\"" + channelCode + "\"");
            data.Append(",\"deviceid\":\"" + devId + "\"}");
            //data.Append(",\"pageno\":" + pageNo);
            //data.Append(",\"pagesize\":" + pageSize + "}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null)
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["data"]["count"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
                else
                {
                    statusCode = jsonArray[0]["message"].ToString();
                }
            }
            return dt;
        }

        /// <summary>
        /// 获取单位信息
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable GetCompanyName(string idcard, string token, out string statusCode)
        {
            string jsonStr = "";
            statusCode = "";
            string channelCode = "SelfService";
            DataTable dt = new DataTable();
            //Uri address = new Uri(_path + "/iface/so/getEndowPayPerson?devId=827665329374&token=" + token);
            Uri address = new Uri(_path + "/iface/manager/ab01?tokenId=" + token + "&deviceid=" + devId);
            //idcard = "412829198906262818";
            //int pageNo = page;
            //int pageSize = 5;
            StringBuilder data = new StringBuilder();
            data.Append("{\"aae135\":\"" + idcard + "\"");
            data.Append(",\"channelcode\":\"" + channelCode + "\"");
            data.Append(",\"deviceid\":\"" + devId + "\"}");
            //data.Append(",\"pageno\":" + pageNo);
            //data.Append(",\"pagesize\":" + pageSize + "}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200")
                {
                    //totalRow = Convert.ToInt32(jsonArray[0]["total"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
                else
                {
                    statusCode = jsonArray[0]["message"].ToString();
                }
            }
            return dt;
        }

        /// <summary>
        /// DataRow转换为DataTable
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="strWhere">筛选的条件</param>
        /// <returns></returns>
        public DataTable SreeenDataTable(DataTable dt, string strWhere)
        {
            if (dt.Rows.Count <= 0) return dt;        //当数据为空时返回
            DataTable dtNew = dt.Clone();         //复制数据源的表结构
            DataRow[] dr = dt.Select(strWhere);  //strWhere条件筛选出需要的数据！
            for (int i = 0; i < dr.Length; i++)
            {
                dtNew.Rows.Add(dr[i].ItemArray);  // 将DataRow添加到DataTable中
            }
            return dtNew;
        }

        /// <summary>
        /// 企业养老个人账户信息
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable GetCardTrans(string idcard, string token, out string statusCode, int page, out int totalRow, string startDate, string endDate)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            DataTable dt = new DataTable();
            Uri address = new Uri(_path + "/iface/rest/getCardTrans?tokenId=" + token);
            int pageSize = 5;
            StringBuilder data = new StringBuilder();
            data.Append("{\"sfzhm\":\"" + idcard + "\"");
            data.Append(",\"qsny\":\"" + startDate + "\"");
            data.Append(",\"zzny\":\"" + endDate + "\"");
            data.Append(",\"pageno\":\"" + page + "\"");
            data.Append(",\"pageSize\":\"" + pageSize + "\"}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null)
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["total"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
                else
                {
                    statusCode = jsonArray[0]["message"].ToString();
                }
            }
            return dt;
        }

        /// <summary>
        /// 个人养老金发放
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable GetPensonBenefitsList(string idcard, string token, out string statusCode, int page, out int totalRow)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            DataTable dt = new DataTable();
            Uri address = new Uri(_path + "/iface/rest/getPensonBenefitsList?tokenId=" + token);
            int pageSize = 5;
            StringBuilder data = new StringBuilder();
            data.Append("{\"aac002\":\"" + idcard + "\"");
            data.Append(",\"pageno\":\"" + page + "\"");
            data.Append(",\"pageSize\":\"" + pageSize + "\"}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null)
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["total"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
                else
                {
                    statusCode = jsonArray[0]["message"].ToString();
                }
            }
            return dt;
        }

        /// <summary>
        /// 医疗保险个人参保信息
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public List<string> GetYLGRBXInfo(string idcard, string token, out string statusCode)
        {
            string jsonStr = "";
            statusCode = "";
            List<string> list = new List<string>();
            Uri address = new Uri(_path + "/iface/rest/getMedicalPerInfo?tokenId=" + token);
            StringBuilder data = new StringBuilder();
            data.Append("{\"aac002\":\"" + idcard + "\"}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null)
                {
                    statusCode = jsonArray[0]["statusCode"].ToString();
                    list.Add(jsonArray[0]["data"]["aac003"].ToString());//姓名
                    list.Add(jsonArray[0]["data"]["aac001"].ToString());//个人编号
                    list.Add(jsonArray[0]["data"]["aae140"].ToString());//险种类型
                    list.Add(jsonArray[0]["data"]["aae041"].ToString());//开始年月
                    list.Add(jsonArray[0]["data"]["aae042"].ToString());//终止年月
                    list.Add(jsonArray[0]["data"]["aac031"].ToString());//参保状态
                    list.Add(jsonArray[0]["data"]["aac037"].ToString());//缴费状态
                    list.Add(jsonArray[0]["data"]["aac130"].ToString());//首次参保时间
                    list.Add(jsonArray[0]["data"]["aac002"].ToString());//身份证号

                    //list.Add(jsonArray[0]["data"]["personId"].ToString());//个人编号
                    //list.Add(jsonArray[0]["data"]["dwbh"].ToString());//单位编号
                    //list.Add(jsonArray[0]["data"]["dwmc"].ToString());//单位名称
                    //list.Add(jsonArray[0]["data"]["cbrq"].ToString());//参保日期
                    //list.Add(jsonArray[0]["data"]["cjgzrq"].ToString());//参加工作日期
                    //list.Add(jsonArray[0]["data"]["xzlx"].ToString());//参保类型编码
                }
            }
            return list;
        }

        #region"安阳大终端"

        /// <summary>
        /// 个人缴费信息查询（医疗）
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable queryPerDeclMedicareMx(string idcard, string token, out string statusCode, int page, out int totalRow, string xzlb, string jfzt)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            int pageSize = 5;
            DataTable dt = new DataTable();
            Uri address = null;
            address = new Uri(_path + "/iface/rest/queryPerDeclMedicareMx?tokenId=" + token);
            StringBuilder data = new StringBuilder();
            data.Append("{\"sfzhm\":\"" + idcard + "\"");
            data.Append(",\"pageno\":\"" + page + "\"");
            data.Append(",\"jfzt\":\"" + jfzt + "\"");
            data.Append(",\"xzlb\":\"" + xzlb + "\"");
            data.Append(",\"pageSize\":\"" + pageSize + "\"}");

            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null && jsonArray[0]["data"].ToString() != "[]")
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["total"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
                else
                {
                    statusCode = jsonArray[0]["message"].ToString();
                }
            }
            return dt;
        }

        /// <summary>
        /// 个人缴费信息查询（生育）
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable queryPerDeclBirthMx(string idcard, string token, out string statusCode, int page, out int totalRow, string jfzt)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            int pageSize = 5;
            DataTable dt = new DataTable();
            Uri address = null;
            address = new Uri(_path + "/iface/rest/queryPerDeclBirthMx?tokenId=" + token);
            StringBuilder data = new StringBuilder();
            data.Append("{\"sfzhm\":\"" + idcard + "\"");
            data.Append(",\"jfzt\":\"" + jfzt + "\"");
            data.Append(",\"pageno\":\"" + page + "\"");
            data.Append(",\"pageSize\":\"" + pageSize + "\"}");

            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null && jsonArray[0]["data"].ToString() != "[]")
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["total"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
                else
                {
                    statusCode = jsonArray[0]["message"].ToString();
                }
            }
            return dt;
        }

        /// <summary>
        /// 个人缴费信息查询（养老（1）、失业（2）、工伤（3）共用）
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <param name="page"></param>
        /// <param name="totalRow"></param>
        /// <param name="type"></param>
        /// <param name="aae114">缴费状态</param>
        /// <returns></returns>
        public DataTable getPersonPesionPayOrLoseFeeIncureListOrHurtInfoIncureList(string idcard, string token, out string statusCode, int page, out int totalRow, int type, string aae114)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            int pageSize = 5;
            DataTable dt = new DataTable();
            Uri address = null;
            StringBuilder data = new StringBuilder();
            data.Append("{\"aac002\":\"" + idcard + "\"");
            data.Append(",\"aae114\":\"" + aae114 + "\"");
            data.Append(",\"pageno\":\"" + page + "\"");
            data.Append(",\"pageSize\":\"" + pageSize + "\"}");

            if (type == 1)
            {
                address = new Uri(_path + "/iface/rest/getPersonPesionPay?tokenId=" + token);
            }
            else if (type == 2)
            {
                address = new Uri(_path + "/iface/rest/getLoseFeeIncureList?tokenId=" + token);
            }
            else
            {
                address = new Uri(_path + "/iface/rest/getHurtInfoIncureList?tokenId=" + token);
            }

            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null && jsonArray[0]["data"].ToString() != "[]")
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["total"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
                else
                {
                    statusCode = jsonArray[0]["message"].ToString();
                }
            }
            return dt;
        }

        /// <summary>
        /// 个人缴费合计信息（医疗）
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public List<string> QueryPerDeclMedicareSum(string idcard, string token, out string statusCode, out string message, string xzlb)
        {
            string jsonStr = "";
            statusCode = "";
            message = "";
            List<string> list = new List<string>();
            StringBuilder data = new StringBuilder();
            data.Append("{\"sfzhm\":\"" + idcard + "\"");
            data.Append(",\"xzlb\":\"" + xzlb + "\"}");
            Uri address = null;

            address = new Uri(_path + "/iface/rest/queryPerDeclMedicareMxSum?tokenId=" + token);

            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null && jsonArray[0]["data"].ToString() != "[]")
                {
                    statusCode = jsonArray[0]["statusCode"].ToString();
                    list.Add(jsonArray[0]["data"]["yjys"].ToString());//应缴月数 0
                    list.Add(jsonArray[0]["data"]["sjys"].ToString());//实缴月数 1
                    list.Add(jsonArray[0]["data"]["wjys"].ToString());//欠缴月数 2
                    list.Add(jsonArray[0]["data"]["dwzjfjeSum"].ToString());//单位实缴金额 3
                    list.Add(jsonArray[0]["data"]["hzjeSum"].ToString());//应缴合计 4
                    list.Add(jsonArray[0]["data"]["grzjfjeSum"].ToString());//个人实缴金额 5
                    list.Add(jsonArray[0]["data"]["dwwjjeSum"].ToString());//单位欠缴总金额 6
                    list.Add(jsonArray[0]["data"]["grwjjeSum"].ToString());//个人欠缴总金额 7
                }
                else
                {
                    statusCode = jsonArray[0]["statusCode"].ToString();
                    message = jsonArray[0]["message"].ToString();
                }
            }
            return list;
        }

        /// <summary>
        /// 个人缴费合计信息（生育）
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public List<string> QueryPerDeclBirthSum(string idcard, string token, out string statusCode, out string message)
        {
            string jsonStr = "";
            statusCode = "";
            message = "";
            List<string> list = new List<string>();
            StringBuilder data = new StringBuilder();
            data.Append("{\"sfzhm\":\"" + idcard + "\"}");
            Uri address = null;
            address = new Uri(_path + "/iface/rest/queryPerDeclBirthMxSum?tokenId=" + token);
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null && jsonArray[0]["data"].ToString() != "[]")
                {
                    statusCode = jsonArray[0]["statusCode"].ToString();
                    list.Add(jsonArray[0]["data"]["yjys"].ToString());//应缴月数 0
                    list.Add(jsonArray[0]["data"]["sjys"].ToString());//实缴月数 1
                    list.Add(jsonArray[0]["data"]["wjys"].ToString());//欠缴月数 2
                    list.Add(jsonArray[0]["data"]["dwzjfjeSum"].ToString());//单位实缴金额 3
                    list.Add(jsonArray[0]["data"]["hzjeSum"].ToString());//应缴合计 4
                    list.Add(jsonArray[0]["data"]["grzjfjeSum"].ToString());//个人实缴金额 5
                    list.Add(jsonArray[0]["data"]["dwwjjeSum"].ToString());//单位欠缴总金额 6
                    list.Add(jsonArray[0]["data"]["grwjjeSum"].ToString());//个人欠缴总金额 7
                }
                else
                {
                    statusCode = jsonArray[0]["statusCode"].ToString();
                    message = jsonArray[0]["message"].ToString();
                }
            }
            return list;
        }

        /// <summary>
        /// 个人缴费合计信息（养老（1）、失业（2）、工伤（3）共用）
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public List<string> getPersonPesionPayOrLoseFeeIncureListOrHurtInfoIncureListSum(string idcard, string token, out string statusCode, out string message, int type)
        {
            string jsonStr = "";
            statusCode = "";
            message = "";
            List<string> list = new List<string>();
            StringBuilder data = new StringBuilder();
            data.Append("{\"aac002\":\"" + idcard + "\"}");
            Uri address = null;

            if (type == 1)//养老保险
            {
                address = new Uri(_path + "/iface/rest/getPersonPesionPaySum?tokenId=" + token);
            }
            else if (type == 2)
            {
                address = new Uri(_path + "/iface/rest/getLoseFeeIncureListSum?tokenId=" + token);
            }
            else
            {
                address = new Uri(_path + "/iface/rest/getHurtInfoIncureListSum?tokenId=" + token);
            }

            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null && jsonArray[0]["data"].ToString() != "[]")
                {
                    statusCode = jsonArray[0]["statusCode"].ToString();
                    list.Add(jsonArray[0]["data"]["yjys"].ToString());//应缴月数 0
                    list.Add(jsonArray[0]["data"]["sjys"].ToString());//实缴月数 1
                    list.Add(jsonArray[0]["data"]["dwjnjeSum"].ToString());//单位实缴金额 2
                    list.Add(jsonArray[0]["data"]["grjnjeSum"].ToString());//个人实缴金额 3
                    list.Add(jsonArray[0]["data"]["yjjeSum"].ToString());//应缴合计 4
                    list.Add(jsonArray[0]["data"]["qjys"].ToString());//欠缴月数 5
                    list.Add(jsonArray[0]["data"]["dwtcqjjeSum"].ToString());//单位欠缴总金额 6
                    list.Add(jsonArray[0]["data"]["grtcqjjeSum"].ToString());//个人欠缴总金额 7
                }
                else
                {
                    statusCode = jsonArray[0]["statusCode"].ToString();
                    message = jsonArray[0]["message"].ToString();
                }
            }
            return list;
        }

        /// <summary>
        /// 个人基本信息（医疗、生育共用）
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public List<string> QueryPersonInfo(string idcard, string token, out string statusCode, out string message)
        {
            string jsonStr = "";
            statusCode = "";
            message = "";
            List<string> list = new List<string>();
            StringBuilder data = new StringBuilder();
            data.Append("{\"sfzhm\":\"" + idcard + "\"}");
            Uri address = null;
            address = new Uri(_path + "/iface/rest/queryPersonInfo?tokenId=" + token);
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null && jsonArray[0]["data"].ToString() != "[]")
                {
                    statusCode = jsonArray[0]["statusCode"].ToString();
                    list.Add(jsonArray[0]["data"][0]["dwbh"].ToString());//单位编号 0
                    list.Add(jsonArray[0]["data"][0]["dwmc"].ToString());//单位名称 1
                    list.Add(jsonArray[0]["data"][0]["grbh"].ToString());//个人编号 2
                    list.Add(jsonArray[0]["data"][0]["sfzhm"].ToString());//身份证号码 3
                    list.Add(jsonArray[0]["data"][0]["xm"].ToString());//姓名 4
                    list.Add(jsonArray[0]["data"][0]["xb"].ToString());//性别 5
                    list.Add(jsonArray[0]["data"][0]["csrq"].ToString());//出生日期 6
                    list.Add(jsonArray[0]["data"][0]["mz"].ToString());//民族 7
                    list.Add(jsonArray[0]["data"][0]["cjgzrq"].ToString());//参加工作日期 8
                    list.Add(jsonArray[0]["data"][0]["cbrylb"].ToString());//职工类别 9
                    list.Add(jsonArray[0]["data"][0]["ltxrq"].ToString());//离退休日期 10
                    list.Add(jsonArray[0]["data"][0]["ryzt"].ToString());//人员状态 11
                }
                else
                {
                    statusCode = jsonArray[0]["statusCode"].ToString();
                    message = jsonArray[0]["message"].ToString();
                }
            }
            return list;
        }

        /// <summary>
        /// 个人基本信息（养老、失业、工伤共用）
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public List<string> GetPersonInfo(string idcard, string token, out string statusCode, out string message)
        {
            string jsonStr = "";
            statusCode = "";
            message = "";
            List<string> list = new List<string>();
            StringBuilder data = new StringBuilder();
            data.Append("{\"aac002\":\"" + idcard + "\"}");
            Uri address = null;
            address = new Uri(_path + "/iface/rest/getPersonInfo?tokenId=" + token);
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null && jsonArray[0]["data"].ToString() != "[]")
                {
                    statusCode = jsonArray[0]["statusCode"].ToString();
                    list.Add(jsonArray[0]["data"][0]["aac001"].ToString());//个人编号 0
                    list.Add(jsonArray[0]["data"][0]["aac002"].ToString());//身份证号码 1
                    list.Add(jsonArray[0]["data"][0]["aac003"].ToString());//姓名 2
                    list.Add(jsonArray[0]["data"][0]["aac004"].ToString());//性别 3
                    list.Add(jsonArray[0]["data"][0]["aac005"].ToString());//民族 4
                    list.Add(jsonArray[0]["data"][0]["aac006"].ToString());//出生日期 5
                    list.Add(jsonArray[0]["data"][0]["aac007"].ToString());//参加工作日期 6
                    list.Add(jsonArray[0]["data"][0]["aac031"].ToString());//人员状态 7
                }
                else
                {
                    statusCode = jsonArray[0]["statusCode"].ToString();
                    message = jsonArray[0]["message"].ToString();
                }
            }
            return list;
        }

        /// <summary>
        /// 生育参保信息
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable QueryPerjoin(string idcard, string token, out string statusCode, int page, out int totalRow)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            int pageSize = 5;
            DataTable dt = new DataTable();
            Uri address = null;
            address = new Uri(_path + "/iface/rest/queryPerjoin?tokenId=" + token);
            StringBuilder data = new StringBuilder();
            data.Append("{\"sfzhm\":\"" + idcard + "\"");
            data.Append(",\"pageno\":\"" + page + "\"");
            data.Append(",\"pageSize\":\"" + pageSize + "\"}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null && jsonArray[0]["data"].ToString() != "[]")
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["total"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
                else
                {
                    statusCode = jsonArray[0]["message"].ToString();
                }
            }
            return dt;
        }

        /// <summary>
        /// 医疗参保信息
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable QueryPerjoinMedicare(string idcard, string token, out string statusCode, int page, out int totalRow)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            int pageSize = 5;
            DataTable dt = new DataTable();
            Uri address = null;
            address = new Uri(_path + "/iface/rest/queryPerjoinMedicare?tokenId=" + token);
            StringBuilder data = new StringBuilder();
            data.Append("{\"sfzhm\":\"" + idcard + "\"");
            data.Append(",\"pageno\":\"" + page + "\"");
            data.Append(",\"pageSize\":\"" + pageSize + "\"}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null && jsonArray[0]["data"].ToString() != "[]")
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["total"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
                else
                {
                    statusCode = jsonArray[0]["message"].ToString();
                }
            }
            return dt;
        }

        /// <summary>
        /// 个人参保信息（养老、工伤、失业）
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable GetPersonInsuredInfo(string idcard, string token, out string statusCode, int page, out int totalRow, int type)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            int pageSize = 5;
            DataTable dt = new DataTable();
            Uri address = null;
            address = new Uri(_path + "/iface/rest/getPersonInsuredInfo?tokenId=" + token);
            StringBuilder data = new StringBuilder();
            data.Append("{\"aac002\":\"" + idcard + "\"");
            data.Append(",\"aae140\":\"" + type + "\"");
            data.Append(",\"pageno\":\"" + page + "\"");
            data.Append(",\"pageSize\":\"" + pageSize + "\"}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null && jsonArray[0]["data"].ToString() != "[]")
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["total"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
                else
                {
                    statusCode = jsonArray[0]["message"].ToString();
                }
            }
            return dt;
        }

        /// <summary>
        /// 个人医疗历年缴费基数
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable QueryPerPayMedicareBase(string idcard, string token, out string statusCode, int page, out int totalRow)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            int pageSize = 5;
            DataTable dt = new DataTable();
            Uri address = null;
            address = new Uri(_path + "/iface/rest/queryPerPayMedicareBase?tokenId=" + token);
            StringBuilder data = new StringBuilder();
            data.Append("{\"sfzhm\":\"" + idcard + "\"");
            data.Append(",\"pageno\":\"" + page + "\"");
            data.Append(",\"pageSize\":\"" + pageSize + "\"}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null && jsonArray[0]["data"].ToString() != "[]")
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["total"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
                else
                {
                    statusCode = jsonArray[0]["message"].ToString();
                }
            }
            return dt;
        }

        /// <summary>
        /// 个人生育历年缴费基数
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable QueryPerPayBirthBase(string idcard, string token, out string statusCode, int page, out int totalRow)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            int pageSize = 5;
            DataTable dt = new DataTable();
            Uri address = null;
            address = new Uri(_path + "/iface/rest/queryPerPayBirthBase?tokenId=" + token);
            StringBuilder data = new StringBuilder();
            data.Append("{\"sfzhm\":\"" + idcard + "\"");
            data.Append(",\"pageno\":\"" + page + "\"");
            data.Append(",\"pageSize\":\"" + pageSize + "\"}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null && jsonArray[0]["data"].ToString() != "[]")
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["total"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
                else
                {
                    statusCode = jsonArray[0]["message"].ToString();
                }
            }
            return dt;
        }

        /// <summary>
        /// 缴费基数信息（养老、工伤、失业）
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable GetPayBase(string idcard, string token, out string statusCode, int page, out int totalRow)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            int pageSize = 5;
            DataTable dt = new DataTable();
            Uri address = null;
            address = new Uri(_path + "/iface/rest/getPayBase?tokenId=" + token);
            StringBuilder data = new StringBuilder();
            data.Append("{\"aac002\":\"" + idcard + "\"");
            data.Append(",\"pageno\":\"" + page + "\"");
            data.Append(",\"pageSize\":\"" + pageSize + "\"}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null && jsonArray[0]["data"].ToString() != "[]")
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["total"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
                else
                {
                    statusCode = jsonArray[0]["message"].ToString();
                }
            }
            return dt;
        }

        /// <summary>
        /// 个人医疗账户余额查询
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public List<string> GteCardYeBySfzhm(string idcard, string token, out string statusCode, out string message, string name)
        {
            string jsonStr = "";
            statusCode = "";
            message = "";
            List<string> list = new List<string>();
            StringBuilder data = new StringBuilder();
            data.Append("{\"sfzhm\":\"" + idcard + "\"");
            data.Append(",\"aac003\":\"" + name + "\"}");
            Uri address = null;
            address = new Uri(_path + "/iface/rest/gCardYeBySfzhm?tokenId=" + token);
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null && jsonArray[0]["data"].ToString() != "[]")
                {
                    statusCode = jsonArray[0]["statusCode"].ToString();
                    list.Add(jsonArray[0]["data"]["ye"].ToString());//余额 0
                }
                else
                {
                    statusCode = jsonArray[0]["statusCode"].ToString();
                    message = jsonArray[0]["message"].ToString();
                }
            }
            return list;
        }


        /// <summary>
        /// 医疗保险个人账户信息查询
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable GetYLBXGRZHInfo(string idcard, string token, out string statusCode, int page, out int totalRow, string startDate, string endDate)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            DataTable dt = new DataTable();
            Uri address = new Uri(_path + "/iface/rest/getCardTrans?tokenId=" + token);
            int pageSize = 5;
            StringBuilder data = new StringBuilder();
            data.Append("{\"sfzhm\":\"" + idcard + "\"");
            data.Append(",\"qsny\":\"" + startDate + "\"");
            data.Append(",\"zzny\":\"" + endDate + "\"");
            data.Append(",\"pageno\":\"" + page + "\"");
            data.Append(",\"pageSize\":\"" + pageSize + "\"}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null && jsonArray[0]["data"].ToString() != "[]")
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["total"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
                else
                {
                    statusCode = jsonArray[0]["message"].ToString();
                }
            }
            return dt;
        }

        /// <summary>
        /// 个人住院记录信息
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable QueryZybxmx(string idcard, string token, out string statusCode, int page, out int totalRow)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            DataTable dt = new DataTable();
            Uri address = new Uri(_path + "/iface/rest/queryZybxmx?tokenId=" + token);
            int pageSize = 5;
            StringBuilder data = new StringBuilder();
            data.Append("{\"sfzhm\":\"" + idcard + "\"");
            data.Append(",\"pageno\":\"" + page + "\"");
            data.Append(",\"pageSize\":\"" + pageSize + "\"}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null && jsonArray[0]["data"].ToString() != "[]")
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["total"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
                else
                {
                    statusCode = jsonArray[0]["message"].ToString();
                }
            }
            return dt;
        }

        /// <summary>
        /// 个人门诊记录信息queryMzbxmx
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable QueryMzbxmx(string idcard, string token, out string statusCode, int page, out int totalRow)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            DataTable dt = new DataTable();
            Uri address = new Uri(_path + "/iface/rest/queryMzbxmx?tokenId=" + token);
            int pageSize = 5;
            StringBuilder data = new StringBuilder();
            data.Append("{\"sfzhm\":\"" + idcard + "\"");
            data.Append(",\"pageno\":\"" + page + "\"");
            data.Append(",\"pageSize\":\"" + pageSize + "\"}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null && jsonArray[0]["data"].ToString() != "[]")
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["total"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
                else
                {
                    statusCode = jsonArray[0]["message"].ToString();
                }
            }
            return dt;
        }

        /// <summary>
        /// 个人医疗消费明细信息
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable QueryScriptInfoForAy(string zylsh, string yybm, string yltcdjh, string jshid, string token, out string statusCode, int page, out int totalRow)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            DataTable dt = new DataTable();
            Uri address = new Uri(_path + "/iface/rest/queryScriptInfoForAy?tokenId=" + token);
            int pageSize = 5;
            StringBuilder data = new StringBuilder();
            data.Append("{\"zylsh\":\"" + zylsh + "\"");
            data.Append(",\"yybm\":\"" + yybm + "\"");
            data.Append(",\"yltcdjh\":\"" + yltcdjh + "\"");
            data.Append(",\"jshid\":\"" + jshid + "\"");
            data.Append(",\"pageno\":\"" + page + "\"");
            data.Append(",\"pageSize\":\"" + pageSize + "\"}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null && jsonArray[0]["data"].ToString() != "[]")
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["total"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
                else
                {
                    statusCode = jsonArray[0]["message"].ToString();
                }
            }
            return dt;
        }

        #endregion

        /// <summary>
        /// 医疗保险费用结算信息查询
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable GetYLBXFYJSInfo(string idcard, string token, out string statusCode, int page, out int totalRow)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            DataTable dt = new DataTable();
            Uri address = new Uri(_path + "/iface/rest/getMedicalFeeSetInfo?tokenId=" + token);
            int pageSize = 5;
            StringBuilder data = new StringBuilder();
            data.Append("{\"aac002\":\"" + idcard + "\"");
            data.Append(",\"pageno\":\"" + page + "\"");
            data.Append(",\"pageSize\":\"" + pageSize + "\"}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null && jsonArray[0]["data"].ToString() != "[]")
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["data"]["count"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
                else
                {
                    statusCode = jsonArray[0]["message"].ToString();
                }
            }
            return dt;
        }

        /// <summary>
        /// 生育保险个人参保信息
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public List<string> GetSYBXGRCBInfo(string idcard, string token, string type, out string statusCode)
        {
            string jsonStr = "";
            statusCode = "";
            List<string> list = new List<string>();
            Uri address = new Uri(_path + "/iface/rest/getInsuredInfo?tokenId=" + token);
            StringBuilder data = new StringBuilder();
            data.Append("{\"aac002\":\"" + idcard + "\"");
            data.Append(",\"aae140\":\"" + type + "\"}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null && jsonArray[0]["data"].ToString() != "[]")
                {
                    statusCode = jsonArray[0]["statusCode"].ToString();
                    list.Add(jsonArray[0]["data"]["aac003"].ToString());//姓名
                    list.Add(jsonArray[0]["data"]["aac001"].ToString());//个人年号
                    list.Add(jsonArray[0]["data"]["aae140"].ToString());//险种类型
                    list.Add(jsonArray[0]["data"]["aac031"].ToString());//参保类型
                    list.Add(jsonArray[0]["data"]["aac002"].ToString());//身份证号

                    //list.Add(jsonArray[0]["data"]["rylb"].ToString());//人员类别
                    //list.Add(jsonArray[0]["data"]["cbzt"].ToString());//参保状态
                    //list.Add(jsonArray[0]["data"]["hkszd"].ToString());//户口所在地
                    //list.Add(jsonArray[0]["data"]["personId"].ToString());//个人编号
                    //list.Add(jsonArray[0]["data"]["dwbh"].ToString());//单位编号
                    //list.Add(jsonArray[0]["data"]["dwmc"].ToString());//单位名称
                    //list.Add(jsonArray[0]["data"]["cbrq"].ToString());//参保日期
                    //list.Add(jsonArray[0]["data"]["cjgzrq"].ToString());//参加工作日期
                    //list.Add(jsonArray[0]["data"]["xzlx"].ToString());//参保类型编码
                }
            }
            return list;
        }

        /// <summary>
        /// 生育、工伤、失业保险个人缴费信息查询
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable GetSYBXGRJFInfo(string idcard, string token, out string statusCode, int page, string type, out int totalRow)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            DataTable dt = new DataTable();
            Uri address = new Uri(_path + "/iface/rest/getPayInfo?tokenId=" + token);
            int pageSize = 5;
            StringBuilder data = new StringBuilder();
            data.Append("{\"aac002\":\"" + idcard + "\"");
            data.Append(",\"aae140\":\"" + type + "\"");
            data.Append(",\"pageno\":\"" + page + "\"");
            data.Append(",\"pageSize\":\"" + pageSize + "\"}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null && jsonArray[0]["data"].ToString() != "[]")
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["data"]["count"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
                else
                {
                    statusCode = jsonArray[0]["message"].ToString();
                }
            }
            return dt;
        }

        /// <summary>
        /// 生育保险医疗费用信息查询
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable GetSYBXYLFYInfo(string idcard, string token, out string statusCode, int page, out int totalRow)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            DataTable dt = new DataTable();
            Uri address = new Uri(_path + "/iface/so/syylfyxxcx?devId=827665329374&token=" + token);
            string aac002 = idcard;
            //aac002 = "412829198906262818";
            int pageNo = page;
            int pageSize = 5;
            StringBuilder data = new StringBuilder();
            data.Append("{\"personNo\":\"" + aac002 + "\"");
            data.Append(",\"pageNo\":" + pageNo);
            data.Append(",\"pageSize\":" + pageSize + "}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200")
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["data"]["count"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
            }
            return dt;
        }

        /// <summary>
        /// 生育保险生育津贴查询
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable GetSYBXSYJTInfo(string idcard, string token, out string statusCode, int page, out int totalRow)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            DataTable dt = new DataTable();
            Uri address = new Uri(_path + "/iface/so/syjtxxcx?devId=827665329374&token=" + token);
            string aac002 = idcard;
            //aac002 = "412829198906262818";
            int pageNo = page;
            int pageSize = 5;
            StringBuilder data = new StringBuilder();
            data.Append("{\"personNo\":\"" + aac002 + "\"");
            data.Append(",\"pageNo\":" + pageNo);
            data.Append(",\"pageSize\":" + pageSize + "}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }

            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200")
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["data"]["rowTotal"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
            }
            return dt;
        }

        /// <summary>
        /// 失业待遇发放信息
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable GetUnemployBenefitInfo(string idcard, string token, out string statusCode, int page, out int totalRow)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            DataTable dt = new DataTable();
            Uri address = new Uri(_path + "/iface/rest/getUnemployBenefitInfo?tokenId=" + token);
            string aac002 = idcard;
            int pageSize = 5;
            StringBuilder data = new StringBuilder();
            data.Append("{\"aac002\":\"" + aac002 + "\"");
            data.Append(",\"pageNo\":" + page);
            data.Append(",\"pageSize\":" + pageSize + "}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }

            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200" && jsonArray[0]["data"].ToString() != "" && jsonArray[0]["data"].ToString() != null && jsonArray[0]["data"].ToString() != "[]")
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["data"]["count"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
            }
            return dt;
        }

        /// <summary>
        /// 慢性病审批信息查询
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable GetMXBSPInfo(string idcard, string token, out string statusCode, int page, out int totalRow)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            DataTable dt = new DataTable();
            Uri address = new Uri(_path + "/iface/so/mxbxxcx?devId=827665329374&token=" + token);
            string aac002 = idcard;
            //aac002 = "412829198906262818";
            int pageNo = page;
            int pageSize = 5;
            StringBuilder data = new StringBuilder();
            data.Append("{\"personNo\":\"" + aac002 + "\"");
            data.Append(",\"pageNo\":" + pageNo);
            data.Append(",\"pageSize\":" + pageSize + "}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200")
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["data"]["rowTotal"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
            }
            return dt;
        }

        /// <summary>
        /// 异地安置审批信息查询
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable GetYDAZSPInfo(string idcard, string token, out string statusCode, int page, out int totalRow)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            DataTable dt = new DataTable();
            Uri address = new Uri(_path + "/iface/so/ydanzxxcx?devId=827665329374&token=" + token);
            string aac002 = idcard;
            //aac002 = "412829198906262818";
            int pageNo = page;
            int pageSize = 5;
            StringBuilder data = new StringBuilder();
            data.Append("{\"personNo\":\"" + aac002 + "\"");
            data.Append(",\"pageNo\":" + pageNo);
            data.Append(",\"pageSize\":" + pageSize + "}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200")
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["data"]["rowTotal"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
            }
            return dt;
        }

        /// <summary>
        /// 异地安置审批信息查询
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="token"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        public DataTable GetYDAZSPInfo2(string idcard, string token, out string statusCode, int page, out int totalRow)
        {
            string jsonStr = "";
            statusCode = "";
            totalRow = 0;
            DataTable dt = new DataTable();
            Uri address = new Uri(_path + "/iface/so/ydanzxxcx?devId=827665329374&token=" + token);
            string aac002 = idcard;
            //aac002 = "412829198906262818";
            int pageNo = page;
            int pageSize = 5;
            StringBuilder data = new StringBuilder();
            data.Append("{\"personNo\":\"" + aac002 + "\"");
            data.Append(",\"pageNo\":" + pageNo);
            data.Append(",\"pageSize\":" + pageSize + "}");
            Common c = new Common();
            try
            {
                jsonStr = c.RetrunJSONValue(address, data).ToString();
            }
            catch (Exception ex)
            {
                statusCode = ex.Message;
            }
            if (jsonStr != "")
            {
                JArray jsonArray = JArray.Parse("[" + jsonStr + "]");
                if (jsonArray[0]["statusCode"].ToString() == "200")
                {
                    totalRow = Convert.ToInt32(jsonArray[0]["data"]["rowTotal"].ToString());
                    dt = c.JsonToDataTable(jsonStr);
                }
            }
            return dt;
        }
        #endregion



        public static MemoryStream ReadFile(string path)
        {
            if (!File.Exists(path))
                return null;

            using (FileStream file = new FileStream(path, FileMode.Open))
            {
                byte[] b = new byte[file.Length];
                file.Read(b, 0, b.Length);
                file.Close();
                file.Dispose();

                MemoryStream stream = new MemoryStream(b);
                return stream;
            }
        }

        public static Image GetFile(string path)
        {
            MemoryStream stream = ReadFile(path);
            return stream == null ? null : Image.FromStream(stream);
        }

    }
}
