using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
//配置文件读取类
namespace DataExchange
{
    class Config
    {
        const string iniNation = @"ini\national.ini";
        const string iniAreaFile = @"ini\area.ini";//区域配置
        const string iniFile = @"ini\Config.ini";//基本配置信息文件
        const string iniWindows = @"ini\windows.ini";//基本配置信息文件
        Dictionary<string, string> win_dic = new Dictionary<string, string>();
        Dictionary<string, string> config_dic = new Dictionary<string, string>();
        Dictionary<string, string> config_Area_dic = new Dictionary<string, string>();
        Dictionary<string, string> config_nation_dic = new Dictionary<string, string>();
        static Config con = null;
        static object locker = new object();//线程锁
        
        Config()
        {          
            iniDic(config_dic, iniFile);
            iniDic(win_dic, iniWindows);
			getIfaceIniData(iniInterfaceDic, interfaceFile);         
            getIfaceIniData(config_Area_dic,iniAreaFile);
            getIfaceIniData(config_nation_dic, iniNation);
        }

        void iniDic(Dictionary<string, string> dic, string filePath)
        {
            if (File.Exists(filePath))
            {
                string[] values = File.ReadAllLines(filePath,Encoding.Default);
                foreach (string v in values)
                {
                    string[] vs = v.Split('#');
                    string[] vn = vs[0].Split('=');
                    if(vn.Length != 2)
                        continue;
                    string key = vn[0];
                    string value = "";
                    if (vn[1].Length > 0)
                        for (int k = vn[1].Length - 1; k >= 0; k--)
                            if (vn[1][k] != ' ' && vn[1][k] != '\t')
                            {
                                value = vn[1].Substring(0, k + 1);
                                break;
                            }
                    dic.Add(key, value);
                }
            }
        }
        static Config getConfig()
        {
            if (con != null)
                return con;
            lock (locker)
            {
                if(con == null)
                    con = new Config();
            }
            return con;
        }
        /// <summary>
        /// 获取配置文件的信息
        /// </summary>
        /// <param name="key">配置数据的名称</param>
        /// <returns>名称对应的值，不存在就返回null</returns>
        public static string dic(string key)
        {
            if (con == null)
                con = getConfig();
            if (con.config_dic.ContainsKey(key))
                return con.config_dic[key];
            else
                return null;
        }

        public static string winDic(string key)
        {
            if (con == null)
                con = getConfig();
            if (con.win_dic.ContainsKey(key))
                return con.win_dic[key];
            else
                return null;
        }

        /////////////////////////////////////////////

        string interfaceFile = Application.StartupPath + "\\ini\\InterfaceCofnig.ini";//接口配置文件;
        Dictionary<string, string> iniInterfaceDic = new Dictionary<string, string>();


        public static string GetIFace(string key)
        {
            if (con == null)
                con = getConfig();
            if (con.iniInterfaceDic.ContainsKey(key))
            {
                return con.iniInterfaceDic[key];
            }
            else
            {
                return "";
            }
        }

        void getIfaceIniData(Dictionary<string, string> dic, string filePath)
        {
            if (File.Exists(filePath))
            {
                string[] values = File.ReadAllLines(filePath, EncodingType.GetType(filePath));
                foreach (string v in values)
                {
                    string[] vn = v.Split('=');
                    if (vn.Length != 2)
                        continue;
                    string key = vn[0];
                    string value = vn[1];

                    if (vn[1].Length > 0)
                    {
                        dic[key] = value;
                    }
                }
            }
        }

        public static string GetArea(string key)
        {
            if (con == null)
                con = getConfig();
            if (con.config_Area_dic.ContainsKey(key))
            {
                return con.config_Area_dic[key];
            }
            else
            {
                return "";
            }
        }

        //chaka查看是否包含某个 value

        public static string GetNationKey(string value)
        {
            if (con == null)
                con = getConfig();
            //foreach dic.Keys
            if (con.config_nation_dic.ContainsValue(value))
            {
                foreach (string key in con.config_nation_dic.Keys)
                {
                    if (con.config_nation_dic[key].Equals(value))
                    {
                        return key;
                    }
                }
            }       
            return "";
        }
        
    }


    /// <summary> 
    /// 获取文件的编码格式 
    /// </summary> 
    public class EncodingType
    {
        /// <summary> 
        /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型 
        /// </summary> 
        /// <param name=“FILE_NAME“>文件路径</param> 
        /// <returns>文件的编码类型</returns> 
        public static System.Text.Encoding GetType(string FILE_NAME)
        {
            FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read);
            Encoding r = GetType(fs);
            fs.Close();
            return r;
        }

        /// <summary> 
        /// 通过给定的文件流，判断文件的编码类型 
        /// </summary> 
        /// <param name=“fs“>文件流</param> 
        /// <returns>文件的编码类型</returns> 
        public static System.Text.Encoding GetType(FileStream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM 
            Encoding reVal = Encoding.Default;

            BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }
            r.Close();
            return reVal;
        }

        /// <summary> 
        /// 判断是否是不带 BOM 的 UTF8 格式 
        /// </summary> 
        /// <param name=“data“></param> 
        /// <returns></returns> 
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1; //计算当前正分析的字符应还有的字节数 
            byte curByte; //当前分析的字节. 
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前 
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X 
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1 
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }

    } 
}
