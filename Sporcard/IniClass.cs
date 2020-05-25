using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sporcard
{
    public class INIClass
    {
        private string iniPath;
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="INIPath">文件路径</param>
        public INIClass(string INIPath)
        {
            iniPath = INIPath;
        }

        /// <summary>
        /// 写INI文件
        /// </summary>
        /// <param name="Section">项目名称</param>
        /// <param name="Key">参数名称</param>
        /// <param name="Value">参数值</param>
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.iniPath);
        }

        /// <summary>
        /// 读INI文件
        /// </summary>
        /// <param name="Section">项目名称</param>
        /// <param name="Key">参数名称</param>
        /// <returns>参数值</returns>
        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(1024);
            int i = GetPrivateProfileString(Section, Key, "", temp, 1024, this.iniPath);
            return temp.ToString();
        }

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <returns>布尔值</returns>
        public bool ExistINIFile()
        {
            return File.Exists(iniPath);
        }


        string iniFile = Application.StartupPath + "\\ini\\Config.ini";//基本配置信息文件
        string InterfaceCofnigFile = Application.StartupPath + "\\ini\\InterfaceCofnig.ini";//基本配置信息文件

        //  string iniFile = Application.StartupPath + "\\Config.ini";//基本配置信息文件  
        static INIClass ini = null;
        static object locker = new object();//线程锁
        Dictionary<string, string> iniDic = new Dictionary<string, string>();

        static INIClass iniData = new INIClass();

        public INIClass()
        {
            getIniData(iniDic, iniFile);
        }

        //取出变量和值
        void getIniData(Dictionary<string, string> dic, string filePath)
        {
            if (File.Exists(filePath))
            {
                string[] values = File.ReadAllLines(filePath, Encoding.Default);
                foreach (string v in values)
                {
                    string[] vs = v.Split('#');
                    string[] vn = vs[0].Split('=');
                    if (vn.Length != 2)
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

        //获取指定key的value
        public static string getDicValue(string key)
        {
            if (ini == null)
                ini = getInstance();
            if (ini.iniDic.ContainsKey(key))
                return ini.iniDic[key];
            else
                return null;
        }

        //单例
        static INIClass getInstance()
        {
            if (ini != null)
                return ini;
            lock (locker)
            {
                if (ini == null)
                    ini = new INIClass();
            }
            return ini;
        }
    }
}
