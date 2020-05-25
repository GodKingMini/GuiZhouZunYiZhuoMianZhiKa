using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sporcard
{
    public class MyImage
    {

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
            try
            {
                MemoryStream stream = ReadFile(path);
                return stream == null ? null : Image.FromStream(stream);
            }
            catch
            {
                //MessageBox.Show("图片加载失败！");
            }
            return null;
        }
    }

    class PrintFormatView
    {
        public Image image;
        public float _dpiX = 300;
        public float _dpiY = 300;
        public int _height = 1010;  //图像高度(以像素为单位)85.5f * 300dpi = 1010
        public int _width = 638;    //图像宽度(以像素为单位)54f * 300dpi = 638

      
        public void SetDPI(float dpiX, float dpiY)
        {
            _dpiX = dpiX;
            _dpiY = dpiY;
        }

        public void SetSize(int height, int width)
        {
            _height = height;
            _width = width;
        }

        private float GetHPixel(float mm)
        {
            return _dpiX / 25.4f * mm;
        }

        private float GetVPixel(float mm)
        {
            return _dpiY / 25.4f * mm;
        }

        public bool DrawFormat(PrintParameter printPra)
        {
            Bitmap img = new Bitmap(_height, _width);
            img.SetResolution(_dpiX, _dpiY);           //设置图片DPI
            Graphics g = Graphics.FromImage(img);
            if (!DrawFormat(ref g, printPra))   //绘制卡面
            {
                return false;
            }
            image = img;
            return true;
        }

        public bool DrawFormat(ref Graphics g, PrintParameter printPra)
        {
            if (!DrawPhotoBack(ref g, printPra.background))
            {
                return false;
            }

            if (!DrawPhoto(ref g, printPra))
            {
                return false;
            }

            if (!DrawPersonInfo(ref g, printPra))
            {
                return false;
            }

            return true;

        }

        public bool DrawPhotoBack(ref Graphics g, string name)
        {
            Image img = null;

            if (name.Equals("") || !File.Exists(name))
            {
                img = Properties.Resources.HolderPictureback;
            }
            else
            {
                img = Image.FromFile(name);
            }

            

            if (img.Equals(null))
            {
                return false;
            }

            float _fwhith = GetHPixel(85.6f);
            float _fHight = GetVPixel(54.0f);

            g.DrawImage(img, 0, 0, (int)_fwhith, (int)_fHight);

            return true;
        }

        public bool DrawPhoto(PrintParameter printPra, ref ZBRGraphics g)
        {
            Image img = null;
            LogHelper.WriteLog(typeof(PrintFormatView), "photo path:" + printPra.personPhoto);
            if (printPra.personPhoto.Equals("") || !File.Exists(printPra.personPhoto))
            {
                img = Properties.Resources.blank;
            }
            else
            {
                LogHelper.WriteLog(typeof(PrintFormatView), "printPra.personPhoto:" + printPra.personPhoto);
                img = MyImage.GetFile(printPra.personPhoto);
                //img = Image.FromFile(printPra.personPhoto);
            }

            if (img == null)
            {
                return false;
            }

            int _fwhith = Convert.ToInt32(GetHPixel(20.0f));
            int _fHight = Convert.ToInt32(GetVPixel(25.0f));

            string[] pos = printPra.photoPos.Split(',');

            int X = Convert.ToInt32(GetHPixel((float)Convert.ToDouble(pos[0])));
            int Y = Convert.ToInt32(GetVPixel((float)Convert.ToDouble(pos[1])));


            //int ret = PrintPicture((int)X, (int)Y, printPra.personPhoto, (int)_fwhith, (int)_fHight);
            int errValue = 0;
            g.DrawImage(Encoding.Default.GetBytes(printPra.personPhoto), (int)X, (int)Y, (int)_fwhith, (int)_fHight, out errValue);
            return true;
        }

        public bool DrawPhoto(ref Graphics g, PrintParameter printPra)
        {
            Image img = null;
            if (printPra.personPhoto.Equals("") || !File.Exists(printPra.personPhoto))
            {
                img = Properties.Resources.blank;
            }
            else
            {
                img = Image.FromFile(printPra.personPhoto);
            }

            if (img.Equals(null))
            {
                return false;
            }

            float _fwhith = GetHPixel(20.0f);
            float _fHight = GetVPixel(25.0f);
            string[] pos = printPra.photoPos.Split(',');
            float X = GetHPixel((float)Convert.ToDouble(pos[0]));
            float Y = GetVPixel((float)Convert.ToDouble(pos[1]));

            //float X = GetHPixel(4.0f);
            //float Y = GetVPixel(11.0f);

            g.DrawImage(img, X, Y, (int)_fwhith, (int)_fHight);
            return true;
        }

        public bool DrawPersonInfo(ref Graphics g, PrintParameter printPra)
        {
            bool bDrawReslut = false;
            bDrawReslut = LINE_Format(ref g, printPra);
            return bDrawReslut;
        }

        public bool DrawPersonInfo(PrintParameter printPra, ref ZBRGraphics g)
        {
            //float X = 0;
            //float Y = 0;
            string[] pos;

            int iCountParams = printPra.printstr.Length;

            if (true)
            {
                for (int i = 0; i < iCountParams; i++)
                {
                    pos = printPra.position[i].Split(',');

                    //X = GetHPixel((float)Convert.ToDouble(pos[0]));
                    //Y = GetVPixel((float)Convert.ToDouble(pos[1]));

                    int X = Convert.ToInt32(GetHPixel((float)Convert.ToDouble(pos[0])));
                    int Y = Convert.ToInt32(GetVPixel((float)Convert.ToDouble(pos[1])));

                    int err = 0;
                    g.DrawText((int)X, (int)Y, Encoding.Default.GetBytes(printPra.printstr[i]), Encoding.Default.GetBytes("宋体"), 8, 0, 0, out err);
                    g.DrawText((int)X, (int)Y + 1, Encoding.Default.GetBytes(printPra.printstr[i]), Encoding.Default.GetBytes("宋体"), 8, 0, 0, out err);

                    //ret = PrintText((int)X, (int)Y, printPra.printstr[i], 8, "宋体", false);                  
                    //ret = PrintText((int)X, (int)Y + 1, printPra.printstr[i], 8, "宋体", false);                    
                }
            }
            return true;
        }

        private bool LINE_Format(ref Graphics g, PrintParameter printPra)
        {

            float X =0;
            float Y =0;
            string[] pos;

            int iCountParams = printPra.printstr.Length;

            if (true)
            {
                for (int i = 0; i < iCountParams; i++)
                {
                    pos = printPra.position[i].Split(',');
                    X = GetHPixel((float)Convert.ToDouble(pos[0]));
                    Y = GetVPixel((float)Convert.ToDouble(pos[1]));
                    g.DrawString(printPra.printstr[i], new Font(new FontFamily("宋体"), 8, System.Drawing.FontStyle.Bold),
                        System.Drawing.Brushes.Black, X , Y);
                    Y += GetVPixel(4.5f);

                }
            }

            //if (cfg.bNeedPrintCardNo)
            //{
            //    g.DrawString(cfg.printfCardNo, new Font(new FontFamily(cfg.cardNo.fontName), cfg.cardNo.FontSize, System.Drawing.FontStyle.Regular),
            //            System.Drawing.Brushes.Black, GetHPixel(cfg.cardNo.X), GetHPixel(cfg.cardNo.Y));
            //}
            return true;
        }
    }
    
    class EP_DLL
    {
        public delegate int PTransmit(ref byte ucpCmd, int nCmdLen, ref byte ucpResp, ref int nRespLen);
        public delegate void PReset(ref byte ucpATR, ref  int nATRLen);
        public delegate string ISendFunction(string SendMsg);

        public static string SendMsgToJMJ(string SendMsg)
        {
            string  sendData = "";
            PlatFormInterface pfi = PlatFormInterface.getInstance();
            string statusCode = "";
            string message = "";

            string recode = "";
            if (GlobalClass.oldMethodFlag)
            {
                //recode = YN_TCPinterface.TCPinterface.AccessJMJ(GlobalClass.name, GlobalClass.idcardType, GlobalClass.idcard, GlobalClass.receipt, SendMsg, out message);

                if (recode.Equals("0000"))
                {
                    return message;
                }
            }
            else
            {

                string jmjbm = pfi.getEncryptor(GlobalClass.idcard, GlobalClass.name, GlobalClass.distCode/*pfi.adCode*/, SendMsg, GlobalClass.userId /*pfi.operatorNo*/, GlobalClass.jylsh, GlobalClass.receipt, GlobalClass.strCertificateType, GlobalClass.oldyhkh, GlobalClass.ssbm, out statusCode, out message, out sendData);

                if (statusCode == "200")
                {                  
                    return jmjbm;
                }
            }           
          
            return "";
        }

        [DllImport("CHIP32_JHSB_EP.dll", EntryPoint = "PersoCard", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int PersoCard(PTransmit pTransmit, PReset pReset, ISendFunction sendFuc, string Header, int HeaderLen, string Data, int DataLen);
    }

    class PAC_DLL
    {
        public delegate int PTransmit(ref byte ucpCmd, int nCmdLen, ref byte ucpResp, ref int nRespLen);
        public delegate void PReset(ref byte ucpATR, ref  int nATRLen);
        public delegate string ISendFunction(string SendMsg);

        public static string SendMsgToJMJ(string SendMsg)
        {
            string sendData = "";
            PlatFormInterface pfi = PlatFormInterface.getInstance();
            string statusCode = "";
            string message = "";

            string recode = "";
            if (GlobalClass.oldMethodFlag)
            { 
                if (recode.Equals("0000"))
                {
                    return message;
                }
            }
            else
            {

                string jmjbm = pfi.getEncryptor(GlobalClass.idcard, GlobalClass.name, GlobalClass.distCode/*pfi.adCode*/, SendMsg, GlobalClass.userId /*pfi.operatorNo*/, GlobalClass.jylsh, GlobalClass.receipt, GlobalClass.strCertificateType, GlobalClass.oldyhkh, GlobalClass.ssbm, out statusCode, out message, out sendData);

                if (statusCode == "200")
                {
                    return jmjbm;
                }
            }

            return "";

            //string outRecMsg = "";
            //string recode = YN_TCPinterface.TCPinterface.AccessJMJ(GlobalClass.name, GlobalClass.idcardType, GlobalClass.idcard, GlobalClass.receipt, SendMsg, out outRecMsg);
   
            //if (recode.Equals("0000"))
            //{
            //    return outRecMsg;
            //}
            //return "";
        }

        [DllImport("JBDYunNanSSSE.dll", EntryPoint = "PersoCard", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int PersoCard(PTransmit pTransmit, PReset pReset,ISendFunction sendFuc, string Header, int HeaderLen, string Data, int DataLen);
    }

    class TS_DLL
    {
        public delegate string ISendFunction(string SendMsg,out string RecMsg);

        public static string SendMsgToJMJ(string SendMsg,out string RecMsg)
        {
            string  sendData = "";
            PlatFormInterface pfi = PlatFormInterface.getInstance();
            string statusCode = "";
            string message = "";
            RecMsg = "";
            string recode = "";

            if (GlobalClass.oldMethodFlag)
            {
                //recode = YN_TCPinterface.TCPinterface.AccessJMJ(GlobalClass.name, GlobalClass.idcardType, GlobalClass.idcard, GlobalClass.receipt, SendMsg, out message);               
                if (recode.Equals("0000"))
                {
                    return message;
                }
            }
            else
            {

                string jmjbm = pfi.getEncryptor(GlobalClass.idcard, GlobalClass.name, GlobalClass.distCode/*pfi.adCode*/, SendMsg, GlobalClass.userId /*pfi.operatorNo*/, GlobalClass.jylsh, GlobalClass.receipt, GlobalClass.strCertificateType, GlobalClass.oldyhkh, GlobalClass.ssbm, out statusCode, out message, out sendData);

                if (statusCode == "200")
                { 
                    return jmjbm;
                }
            }

            return "";
        }

        [DllImport("LSCard.dll", EntryPoint = "iTest", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int iTest(ISendFunction sendFuc,StringBuilder rec);
        [DllImport("LSCard.dll", EntryPoint = "iWriteCard", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int iWriteCard(string data,ISendFunction sendFuc);
    }

    class LSCard
    {
        [DllImport("LSCard.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int iDOpenPort(int readerPort, int readType);

         [DllImport("LSCard.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int iDCloseReader(int iReaderHandle);

        [DllImport("LSCard.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int iDInitReader(int iReaderHandle, StringBuilder Atr);

        [DllImport("LSCard.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int iGetBankNO(int iReaderHandle, StringBuilder Bankno);

        [DllImport("LSCard.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int iGetValiDate(int iReaderHandle, StringBuilder ValidDate);

        [DllImport("LSCard.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int iRMFCardDeptInfo(int iReaderHandle, 
            StringBuilder szCardCertID,
            StringBuilder szCardType,
            StringBuilder szCardVersion,
            StringBuilder szOrgDeptID,
            StringBuilder szDispCardDate,
            StringBuilder szExpireDate,
            StringBuilder szCardID);

        [DllImport("LSCard.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int iRMFCardOwnerInfo(int iReaderHandle,
                             StringBuilder szSfzhm,
                             StringBuilder szName,
                             StringBuilder szSex,
                             StringBuilder szFolk,
                             StringBuilder szBirthPlace,
                             StringBuilder szBirthDate
                             );

        [DllImport("LSCard.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int iCalcuVerifyCodeWithPSAM(int hCom, string CityCode, string szInData, StringBuilder szMac);

        [DllImport("LSCard.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int iWMFCardDeptInfo(int iReaderHandle,
                            string strCityCode,
                            string szCardCertCode,
                            string szCardType,
                            string szCardVersion,
                            string szOrgDeptID,
                            string szDispCardDate,
                            string szExpireDate,
                            string szCardNo
                            );

        [DllImport("LSCard.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int iWMFCardOwnerInfo(int iReaderHandle,
                             string strCityCode,
                             string szSfzhm,
                             string szName,
                             string szSex,
                             string szFolk,
                             string szBirthPlace,
                             string szBirthDate
                             );

        [DllImport("LSCard.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int iWAPPRegResidInfo(int iReaderHandle,
                             string szRegType,
                             string szRegAddr,
                             string szRegAddrkz
                             );

        [DllImport("LSCard.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int iWAPPCommInfo(int iReaderHandle,
                         string szCommandAddr,
                         string szCommandAddrKZ,
                         string szPostalCode,
                         string szTel
                         );

        [DllImport("LSCard.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int iWAPPPersonStatus(int iReaderHandle,
                             string szWorkStatus,
                             string szEduLevel
                             );

        [DllImport("LSCard.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int iWAPPDeptInfo(int iReaderHandle,
                         string szDeptName,
                         string szDeptNameKZ,
                         string szDeptOrgID
                         );
    }

    class PrintProduct
    {
        #region ------------reader------------

        [DllImport("Reader.dll", EntryPoint = "ICCPowerOn", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Winapi)]
        public static extern int ICCPowerOn(string pcReaderName);

        [DllImport("Reader.dll", EntryPoint = "ICCPowerOff", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Winapi)]
        public static extern void ICCPowerOff();

        [DllImport("Reader.dll", EntryPoint = "Transmit", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Winapi)]
        public static extern int Transmit(ref byte ucpCmd, int nCmdLen, ref byte ucpResp, ref int nRespLen);

        [DllImport("Reader.dll", EntryPoint = "ResetCard", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Winapi)]
        public static extern void ResetCard(ref byte ucpATR, ref int nATRLen);

        #endregion

        #region ------------zxp3------------

        [DllImport("TScardPrinter.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int OpenPrinter(string dev_Name);

        [DllImport("TScardPrinter.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int ClosePrinter(int PrinterHandle);

        //进卡接口
        //入参 ： PrinterHandle 打印机句柄  Source  进卡起始位置  Destination  进卡目标位置
        //返回值：1 成功，0 失败
        [DllImport("TScardPrinter.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int FeedCard(int PrinterHandle, int Source, int Destination);

        //准备打印
        [DllImport("TScardPrinter.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int WaitForPrint(int PrinterHandle);

        //打印文本接口
        [DllImport("TScardPrinter.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int PrintText(int X, int Y, string Text, int FontSize, string FontName, bool FontBold);

        //打印图片接口
        [DllImport("TScardPrinter.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int PrintPicture(int X, int Y, string PicturePath, int Width, int Height);

        //开始打印
        [DllImport("TScardPrinter.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int StartPrint(int PrinterHandle, int Source, int Destination);

        //退卡
        [DllImport("TScardPrinter.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Eject(int PrinterHandle);

        //退废卡
        [DllImport("TScardPrinter.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Reject(int PrinterHandle);

        //获取设备状态
        [DllImport("TScardPrinter.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetLastStatusMessage(int PrinterHandle, string retMsg);

        //取消当前任务
        [DllImport("TScardPrinter.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int CancelCurrentTask(int PrinterHandle);

        //获取打印设备序列号
        [DllImport("TScardPrinter.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetPrinterSerialNo(int PrinterHandle, string serialNo);

        //设备复位
        [DllImport("TScardPrinter.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern int Reset(int PrinterHandle);

        #endregion

        public float _dpiX = 300;
        public float _dpiY = 300;

        public void SetDPI(float dpiX, float dpiY)
        {
            _dpiX = dpiX;
            _dpiY = dpiY;
        }

        public float GetHPixel(float mm)
        {
            return _dpiX / 25.4f * mm;
        }

        public float GetVPixel(float mm)
        {
            return _dpiY / 25.4f * mm;
        }

        public bool DrawPhoto(PrintParameter printPra)
        {
            Image img = null;
            if (printPra.personPhoto.Equals("") || !File.Exists(printPra.personPhoto))
            {
                img = Properties.Resources.blank;
            }
            else
            {
                img = Image.FromFile(printPra.personPhoto);
            }

            if (img.Equals(null))
            {
                return false;
            }

            float _fwhith = GetHPixel(20.0f);
            float _fHight = GetVPixel(25.0f);
            string[] pos = printPra.photoPos.Split(',');
            float X = GetHPixel((float)Convert.ToDouble(pos[0]));
            float Y = GetVPixel((float)Convert.ToDouble(pos[1]));

            int ret = PrintPicture((int)X, (int)Y, printPra.personPhoto, (int)_fwhith, (int)_fHight);
            return true;
        }

        public bool DrawPersonInfo(PrintParameter printPra)
        {
            int ret = 0;
            float X = 0;
            float Y = 0;
            string[] pos;

            int iCountParams = printPra.printstr.Length;

            if (true)
            {
                for (int i = 0; i < iCountParams; i++)
                {
                    pos = printPra.position[i].Split(',');
                    X = GetHPixel((float)Convert.ToDouble(pos[0]));
                    Y = GetVPixel((float)Convert.ToDouble(pos[1]));
                    ret = PrintText((int)X, (int)Y, printPra.printstr[i], 8, "宋体", false);
                    ret = PrintText((int)X, (int)Y+1, printPra.printstr[i], 8, "宋体", false);
                    //Y += GetVPixel(3.5f);
                }
            }
            return true;
        }
    }
}
