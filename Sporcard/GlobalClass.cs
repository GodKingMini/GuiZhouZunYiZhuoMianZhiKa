using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sporcard;
using System.Runtime.InteropServices;

namespace Sporcard
{
    public class GlobalClass
    {
        #region  //声明 API 函数

        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern int SendMessage(int hwnd, int wMsg, int wParam, int lParam);


        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("user32", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern int ReleaseCapture();


        public const int WM_NCLBUTTONDOWN = 0XA1;   //.定义鼠標左鍵按下
        public const int HTCAPTION = 2;

        #endregion       

        public static string strPersonSerial;
        public static string pPrintInfo;

        public static string strATR;
        public static string strBankNo;
        public static string strValidDate;
        public static string personID;
        public static string name;
        public static string idcard;//身份证号码
        public static string sbNo;
        public static string cardcertcode;
        public static string fkrq;
        public static string kyxq;
        public static string DeviceSerial;

        public const int WM_MY_MESSAGE = 0x400 + 0XA1;   //.定义鼠標左鍵按下
        public static string sendJMJData = "";

        // public static string distCode = "";
        public static string oldyhkh = "";

        public static string status;
        //姓名
        public static string idcardType;//证件类型

        public static string sex;//xin性别
        public static string provNo;
        public static string receipt;//业务单据号
        public static string fulldata;

        public static string ssbm = "";//省个人识别号
        public static string yhbm = "";//银行网点编码
        public static string busId = "";//制卡业务表id
        public static string strCertificateType = "";//类型
        public static bool oldMethodFlag = false;//测试
        public static string changeType = "";//
        public static string jylsh = "";//交易流水号
        public static string photoData = "";//照片数据
        public static string loginName = "";
        public static string orgId = "";
        public static string carddata = "";
        public static string userId = "";
        public static string distCode = "";
        public static string strWriteCardData = "";
        public static string ksdm = "";//卡商代码
        public static string newYHKH = "";//新银行卡号
        public static string backErrMsgType = "";//人为看到的失败类型
        public static string zkErrorCode = "";//
        public static bool bCheckFeeFlag = true;//ture:需要查询，false：不需要查询

        public static string sFSJKM = "";//非税校验码
        public static string sFSPH = "";//非税票号
        public static int bFeeType = 0;//否缴费 1：已缴费  0：未缴费（已缴费时必填）
        //static GlobalClass gclass = null;
        //private GlobalClass()
        //{
        //}
        //private static readonly object snycRoot = new object();
        //public static GlobalClass getInstance()
        //{
        //    if (gclass == null)
        //    {
        //        lock (snycRoot)
        //        {
        //            if (gclass == null)
        //            {
        //                gclass = new GlobalClass();
        //            }
        //        }
        //    }
        //    return gclass;
        //}
    }


    public class PrintParameter
    {
        public string printerInfo = "";
        public string background = "";
        public string photoPos = "4,11";
        public string[] position = { "28,14.5", "28,19.0", "28,23.5", "28,28.0" };
        //public string[] position = { "28,14.0", "28,18.5", "28,23.0", "28,27.5", "28,32.0", "28,36.5" };
        public string _photoPath = "";
        public string strValidDate = "";

        private string _personPhoto = Application.StartupPath + "\\Photo\\Sample.jpg";
        private string[] _printstr = { "", "", "", "" };

        public PrintParameter()
        {
            INIClass ini = new INIClass(Application.StartupPath + "\\TSconfig.ini");
            background = Application.StartupPath + @"\Photo\" + ini.IniReadValue("PRINT", "BackGround");
            photoPos = ini.IniReadValue("PRINT", "PhotoPos");
            position = ini.IniReadValue("PRINT", "StrPos").Split('|');

        }
        public string personPhoto
        {
            //get { return Application.StartupPath + @"\Photo\" + GlobalClass.idcard + "_" + GlobalClass.name + ".jpg";}
            //set { return this._photoPath; }
            get { return _photoPath; }
        }

        public string[] printstr
        {
            get
            {
                if ( !printerInfo.Equals(""))
                {
                    _printstr[0] = printerInfo.Split('|')[0];
                    _printstr[1] = printerInfo.Split('|')[1];
                    _printstr[2] = printerInfo.Split('|')[2];
                    _printstr[3] = printerInfo.Split('|')[3];
                    //_printstr[3] = strValidDate;
                }
                return _printstr;
            }
        }
    }
}
