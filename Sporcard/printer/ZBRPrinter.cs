// Zebra Printer SDK Support
// **********************************************************************************************************

using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Deployment;
using System.Windows.Forms;

namespace Sporcard
{
    public class ZBRPrinter
    {
        #region Private Variables

        private IntPtr  _handle;    // device context
        private int     _prnType;

        #endregion

        #region Constructor

        public ZBRPrinter()
		{
            _handle = IntPtr.Zero;
            _prnType = 0;
		}

		#endregion

		#region Printer DLLImports

        // SDK DLL Version

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNGetSDKVer", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, 
                   SetLastError = true)]
        static extern void ZBRPRNGetSDKVer(out int major, out int minor, out int engLevel);

        // Handle

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRGetHandle", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall,
                   SetLastError = true)]
        static extern int ZBRGetHandle(out IntPtr _handle, byte[] drvName, out int prn_type, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRGetHandle", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, 
                   SetLastError = true)]
        static extern int ZBRGetHandle(out IntPtr _handle, [MarshalAs(UnmanagedType.LPStr)] string drvName, out int prn_type, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRCloseHandle", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, 
                   SetLastError = true)]
        static extern int ZBRCloseHandle(IntPtr _handle, out int err);

        // Card Movement

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNEjectCard", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall,
                   SetLastError = true)]
        static extern int ZBRPRNEjectCard(IntPtr _handle, int prn_type, out int err);

        // Magnetic Encoding

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNReadMag", CharSet = CharSet.Auto,
                    CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern int ZBRPRNReadMag(IntPtr _handle, int prn_type, int trksToRead, byte[] trk1Buf,
                                        out int trk1BytesNeeded, byte[] trk2Buf, out int trk2BytesNeeded, byte[] trk3Buf,
                                        out int trk3BytesNeeded, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNReadMag", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, 
                    SetLastError = true)]
        static extern int ZBRPRNReadMag(IntPtr _handle, int prn_type, int trksToRead, char[] trk1Buf,
                                        out int trk1BytesNeeded, char[] trk2Buf, out int trk2BytesNeeded,
                                        char[] trk3Buf, out int trk3BytesNeeded, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNReadMag", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, 
                    SetLastError = true)]
        static extern int ZBRPRNReadMag(IntPtr _handle, int prn_type, int trksToRead, IntPtr trk1Buf,
                                        out int trk1BytesNeeded, IntPtr trk2Buf, out int trk2BytesNeeded,
                                        IntPtr trk3Buf, out int trk3BytesNeeded, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNWriteMag", CharSet = CharSet.Auto, 
                    CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern int ZBRPRNWriteMag(IntPtr _handle, int prn_type, int trksToWrite,
                                         byte[] trk1Data, byte[] trk2Data, byte[] trk3Data, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNWriteMag", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, 
                    SetLastError = true)]
        static extern int ZBRPRNWriteMag(IntPtr _handle, int prn_type, int trksToWrite,
                                         char[] trk1Data, char[] trk2Data, char[] trk3Data, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNWriteMag", CharSet = CharSet.Auto,
                    CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern int ZBRPRNWriteMag(IntPtr _handle, int prn_type, int trksToWrite,
                                         IntPtr trk1Data, IntPtr trk2Data, IntPtr trk3Data, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNWriteMagPassThru", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, 
                    SetLastError = true)]
        static extern int ZBRPRNWriteMagPassThru(IntPtr hDC, int prn_Type, int trksToWrite, 
                                                  byte[] trk1Data, byte[] trk2Data, byte[] trk3Data, 
                                                  out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNSetMagEncodingStd", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall,
                    SetLastError = true)]
        static extern int ZBRPRNSetMagEncodingStd(IntPtr hDC, int prn_Type, int std, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNSetEncoderCoercivity", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall,
                    SetLastError = true)]
        static extern int ZBRPRNSetEncoderCoercivity(IntPtr hDC, int prn_Type, int coercivity, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNGetPrinterStatus", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, 
                    SetLastError = true)]
        static extern int ZBRPRNGetPrinterStatus(out int status);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNGetPrinterOptions", CharSet = CharSet.Auto,
                   CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        private static extern int ZBRPRNGetPrinterOptions(IntPtr handle, int prn_type, byte[] options, out int respSize, out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNStartSmartCard",CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int ZBRPRNStartSmartCard(IntPtr hPrinter,int printerType,int cardType,out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNEndSmartCard",CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int ZBRPRNEndSmartCard(IntPtr hPrinter, int printerType,int cardType,int moveType,out int err);

        [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNGetPrinterSerialNumb",CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int ZBRPRNGetPrinterSerialNumb(IntPtr hPrinter,int printerType,byte[] serialNum, out int respSize,out int err);

         [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRSXGetPrinterName",CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int ZBRSXGetPrinterName(string deviceId, [MarshalAs(UnmanagedType.BStr)]out string deviceName, out int retError);

         [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNMoveCard",CharSet = CharSet.Auto, SetLastError = true)]
         public static extern int ZBRPRNMoveCard(IntPtr hPrinter,int printerType,int steps,out int err);

         [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNFlipCard", CharSet = CharSet.Auto, SetLastError = true)]
         public static extern int ZBRPRNFlipCard(IntPtr hPrinter, int printerType, out int err);

         [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNMoveCardBkwd", CharSet = CharSet.Auto, SetLastError = true)]
         public static extern int ZBRPRNMoveCardBkwd(IntPtr hPrinter,         int printerType,         int steps,         out int err);

         [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNMoveCardFwd",CharSet = CharSet.Auto, SetLastError = true)]
         public static extern int ZBRPRNMoveCardFwd(IntPtr hPrinter,int printerType,int steps,out int err);

         [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNMovePrintReady", CharSet = CharSet.Auto, SetLastError = true)]
         public static extern int ZBRPRNMovePrintReady(IntPtr hPrinter, int printerType, out int err);

         [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNIsPrinterReady", CharSet = CharSet.Auto, SetLastError = true)]
         public static extern int ZBRPRNIsPrinterReady(IntPtr hPrinter,int printerType, out int err);

         [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNPrintTestCard", CharSet = CharSet.Auto, SetLastError = true)]
         public static extern int ZBRPRNPrintTestCard(IntPtr hPrinter,int printerType,int testcardType,out int err);

        //读取色带剩余数量
         [DllImport("ZBRPrinter.dll", EntryPoint = "ZBRPRNGetPanelsRemaining", CharSet = CharSet.Auto, SetLastError = true)]
         public static extern int ZBRPRNGetPanelsRemaining(IntPtr hPrinter,int printerType,out int panels,out int err);

        #endregion

         public int test(out int err)
         {
             int testcardType = 0; //print standard test card            

             int result = ZBRPRNPrintTestCard(_handle, _prnType, testcardType, out err);
             if (result == 1 && err == 0)
                 return 1;
             else
                 return 0;   
         }

         public int IsPrinterReady(out int err)
         {
             int result = ZBRPRNIsPrinterReady(_handle, _prnType, out err);
             LogHelper.WriteLog(typeof(ZBRPrinter), string.Format("ZBRPRNIsPrinterReady : {0},{1}", result, err));//"IsPrinterReady: " + result);

             if (result == 1 && err == 0)
                 return 1;
             else
             {
                 Thread.Sleep(2000);
                 result = ZBRPRNIsPrinterReady(_handle, _prnType, out err);
                 LogHelper.WriteLog(typeof(ZBRPrinter), string.Format("ZBRPRNIsPrinterReady : {0},{1}", result, err));//"IsPrinterReady: " + result);
                 if (result == 1 && err == 0)
                     return 1;
                 return 0;
             }
         }

         public int GetPanelRemaining(int zkCount,out int err)
         {
             int panels = 0;
             int result = ZBRPRNGetPanelsRemaining(_handle, _prnType, out panels, out err);
             LogHelper.WriteLog(typeof(ZBRPrinter), string.Format("ZBRPRNIsPrinterReady : {0},{1}", result, err));//"IsPrinterReady: " + result);
             //判断制卡数量
             if (result == 1 && err == 0)
             {
                 return 1;
             }
             else
             {
                 Thread.Sleep(2000);
                 result = ZBRPRNGetPanelsRemaining(_handle, _prnType, out panels, out err);
                 LogHelper.WriteLog(typeof(ZBRPrinter), string.Format("ZBRPRNIsPrinterReady : {0},{1}", result, err));//"IsPrinterReady: " + result);
                 if (result == 1 && err == 0)
                     return 1;
                 return 0;
             }
         }
                 
        public int MoveToPrintLocation(out int err)
         {
             int result = ZBRPRNMovePrintReady(_handle, _prnType, out err);

             LogHelper.WriteLog(typeof(ZBRPrinter), string.Format("ZBRPRNMovePrintReady : {0},{1}", result, err));
             if (result == 1 && err == 0)
                 return 1;
             else
             {
                 Thread.Sleep(2000);
                 result = ZBRPRNMovePrintReady(_handle, _prnType, out err);
                 LogHelper.WriteLog(typeof(ZBRPrinter), string.Format("ZBRPRNMovePrintReady : {0},{1}", result, err));

                 if (result == 1 && err == 0)
                     return 1;                
                 return 0;
             }
         }

        public int MoveCardBKFwd(int steps, out int err)
         {
             int result = ZBRPRNMoveCardBkwd(_handle, _prnType, steps, out err);
             if (result == 1 && err == 0)
                 return 1;
             else
                 return 0; 
         }
         
        public int MoveCard(int steps, out int err)
         {
             int result = ZBRPRNMoveCard(_handle, _prnType, steps, out err);
             LogHelper.WriteLog(typeof(ZBRPrinter), string.Format("ZBRPRNMoveCard : {0},{1}", result, err));
             if (result == 1 && err == 0)
                 return 1;
             else
                 return 0; 
         }
        
         public int MoveCardFwd(int steps, out int err)
         {
             int result =ZBRPRNMoveCardFwd(_handle, _prnType, steps, out err);
             if (result == 1 && err == 0)
                 return 1;
             else
                 return 0; 
         }

        public int StartPostionCard(int cardType, out int err)
        {
            int result = 0;
            result = ZBRPRNStartSmartCard(_handle, _prnType, cardType, out err);

            LogHelper.WriteLog(typeof(FormMain), string.Format("StartPostionCard ret:{0},err:{1}", result, err));
            if (result == 1 && err == 0)
                return 1;
            else
            {
                Thread.Sleep(2000);
                result = ZBRPRNStartSmartCard(_handle, _prnType, cardType, out err);
                if (result == 1 && err == 0)
                    return 1;
                return 0;
            }          
            
        }

        //cardType[in ]SmartCard Type:
        //1 = Contact
        //2 = Contactless
        //3 = UHF
        //moveType : 0  move card to print ready position, 1 = eject card
         public int EndPostionCard(int cardType, int moveType, out int err)
         {
             int result = 0;
             result = ZBRPRNEndSmartCard(_handle, _prnType, cardType, moveType, out err);

             LogHelper.WriteLog(typeof(FormMain), string.Format("ZBRPRNEndSmartCard ret:{0},err:{1}", result, err));
             if (result == 1 && err == 0)
             {

                 return 1;
             }
             else
             {
                 Thread.Sleep(2000);
                 result = ZBRPRNEndSmartCard(_handle, _prnType, cardType, moveType, out err);
                 LogHelper.WriteLog(typeof(FormMain), string.Format("ZBRPRNEndSmartCard 222 ret:{0},err:{1}", result, err));
                 if (result == 1 && err == 0)
                 {                    
                     return 1;
                 }
                 else return 0;
             }
         }

         public int FlipCard(out int err)
        {
            int result = 0;
            result =ZBRPRNFlipCard(_handle, _prnType, out err);
            if (result == 1 && err == 0)
                return 1;
            else
                return 0;
    
        }

        public int OutCard(out int err)
        {            
            int result = ZBRPRNEjectCard(_handle, _prnType, out err);
            if (result == 1 && err == 0)
                return 1;
            else
                return 0; 
        }
       
        //public int PrintCard(PrintParameter pParas, out int err)
        //{
        //    SampleCodeGraphics prn = null;
        //    string msg = string.Empty;
        //    int jobID = 0;
        //    err = 0;
        //    try
        //    {
        //        if (EndPostionCard(1, 0, out err) == 1)
        //        {
        //            if (MoveToPrintLocation(out err) == 0)
        //            {                       
        //                return 0;
        //            }

        //            prn = new SampleCodeGraphics();
        //            if (IsPrinterReady(out err) == 1)
        //            {
        //                prn.PrintFrontSideOnly(pParas, printName,out jobID, out msg);
        //                if (string.IsNullOrEmpty(msg))
        //                {
        //                    WaitForJobToComplete(printName, jobID, out msg);
        //                }                        
        //            }

        //            LogHelper.WriteLog(typeof(ZBRPrinter), "PrintCard: " + msg);
        //        }
        //        return 1;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        msg += ex.Message;
        //        MessageBox.Show(ex.ToString(), "PrintCard");
        //        LogHelper.WriteLog(typeof(ZBRPrinter), "PrintCard: " + msg);
        //        return 0;
        //    }
        //    finally
        //    {
        //        if (prn != null)
        //        {
        //            prn = null;
        //        }
        //    }


        //}


        public int PrintCard(PrintParameter pParas, out int err)
        {
            LogHelper.WriteLog(typeof(FormMain), "PrintCard" );

            SampleCodeGraphics prn = null;
            string msg = string.Empty;
            int jobID = 0;
            err = 0;
            try
            {
                LogHelper.WriteLog(typeof(ZBRPrinter), "EndPostionCard Begin");
                if (EndPostionCard(1, 0, out err) == 1)
                {
                    LogHelper.WriteLog(typeof(ZBRPrinter), "EndPostionCard 成功");

                    LogHelper.WriteLog(typeof(ZBRPrinter), "MoveToPrintLocation Begin");

                    if (MoveToPrintLocation(out err) == 0)
                    {
                        LogHelper.WriteLog(typeof(ZBRPrinter), "MoveToPrintLocation 失败: err +" + err);
                        return 0;
                    }

                    LogHelper.WriteLog(typeof(ZBRPrinter), "MoveToPrintLocation 成功: err +" + err);

                    prn = new SampleCodeGraphics();
                    if (IsPrinterReady(out err) == 1)
                    {
                        LogHelper.WriteLog(typeof(ZBRPrinter), "IsPrinterReady 成功: err +" + err);
                        prn.PrintFrontSideOnly(pParas, printName, out jobID, out msg);

                        LogHelper.WriteLog(typeof(ZBRPrinter), "PrintFrontSideOnly: msg +" + msg);

                        if (string.IsNullOrEmpty(msg))
                        {
                            WaitForJobToComplete(printName, jobID, out msg);
                        }

                        return 1;
                    }
                    else
                    {
                        LogHelper.WriteLog(typeof(ZBRPrinter), "IsPrinterReady 失败: err +" + err);
                        return 0;
                    }
                }
                else
                {
                    LogHelper.WriteLog(typeof(ZBRPrinter), "EndPostionCard 失败: err +" + err);
                    return 0;
                }              
            }
            catch (System.Exception ex)
            {
                msg += ex.Message;
                MessageBox.Show(ex.ToString(), "PrintCard");
                LogHelper.WriteLog(typeof(ZBRPrinter), "PrintCard: " + msg);
                return 0;
            }
            finally
            {
                if (prn != null)
                {
                    prn = null;
                }
            }

        }

        #region SDK Information

        // Get ZBRPrinter.dll Version -----------------------------------------------------------------------
        
        public void GetSDKVer(out int major, out int minor, out int engLevel)
        {
            ZBRPRNGetSDKVer(out major, out minor, out engLevel);
        }

        #endregion
        
        #region open / close Handle

        // Opens a connection to a printer driver -----------------------------------------------------------
        IntPtr hPrinter = IntPtr.Zero;

        int printerType = 31;
        public string printName = "Zebra ZXP Series 3C USB Card Printer";
        public int Open(out int err)
		{            

            System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();            
            Byte[] drvName = null;
            drvName = ascii.GetBytes(printName);

            LogHelper.WriteLog(typeof(FormMain), "Open:" + drvName);

            return ZBRGetHandle(out _handle, drvName, out _prnType, out err);
		}

        // Closes the connection to a printer driver --------------------------------------------------------
        
        public int Close(out int err) 
		{
            return ZBRCloseHandle(_handle, out err);
		}

        #endregion

        #region Printer Configuration

        public int GetPrinterConfiguration(byte[] options, out int respSize, out int err, out string errMsg)
        {
            errMsg = string.Empty;
            err = -1;
            respSize = 0;
            int result = -1;
            try
            {
                result = ZBRPRNGetPrinterOptions(_handle, _prnType, options, out respSize, out err);
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                result = -1;
            }
            return result;
        }

        #endregion Printer Configuration

        #region Printer Status

        public bool IsPrinterInErrorMode(out int errValue)
        {
            ZBRPRNGetPrinterStatus(out errValue);
            if (errValue == 0)
                return false;
            return true;
        }

        #endregion //Printer Status
        
        #region Card Movement

        // Ejects a card from a printer ---------------------------------------------------------------------

        public int EjectCard (out int errValue)
		{
            return ZBRPRNEjectCard(_handle, _prnType, out errValue);
		}

        #endregion

        #region Magnetic Encoding

        // Reads all three magnetic strip tracks ------------------------------------------------------------

        public int ReadMag(int trksToRead, ref string track1, ref string track2, ref string track3, out string errMsg)
        {
            errMsg = string.Empty;

            byte[] trkBuf1 = null;
            byte[] trkBuf2 = null;
            byte[] trkBuf3 = null;

            int errValue = 0;
            try
            {
                trkBuf1 = new byte[50];
                trkBuf2 = new byte[50];
                trkBuf3 = new byte[50];

                int size1 = 0;
                int size2 = 0;
                int size3 = 0;
                
                int result = ZBRPRNReadMag(_handle, _prnType, trksToRead, trkBuf1, out size1, trkBuf2, out size2,
                                            trkBuf3, out size3, out errValue);

                if (result == 1 && errValue == 0)
                {
                    track1 = ASCIIEncoding.ASCII.GetString(trkBuf1, 0, size1);
                    track2 = ASCIIEncoding.ASCII.GetString(trkBuf2,0, size2);
                    track3 = ASCIIEncoding.ASCII.GetString(trkBuf3, 0, size3);
                }
                return result;
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            finally
            {
                trkBuf1 = null;
                trkBuf2 = null;
                trkBuf3 = null;
            }
            return 0;
        }

        // Writes to the magnetic strip tracks
        //     if data is null or "", the track is not written ----------------------------------------------

        public int WriteMag(int trksToWrite, string track1, string track2, string track3, out string errMsg)
        {
            errMsg = string.Empty;
            
            byte[] trkBuf1 = null;
            byte[] trkBuf2 = null;
            byte[] trkBuf3 = null;
            int errValue = 0;
            int IsoMag = 1;
            int HiCo = 1;
            try
            {
                //set to ISO mag encode standard:
                ZBRPRNSetMagEncodingStd(_handle, _prnType, IsoMag, out errValue);

                //set hico:
                ZBRPRNSetEncoderCoercivity(_handle, _prnType, HiCo, out errValue);

                trkBuf1 = ASCIIEncoding.ASCII.GetBytes(track1);
                trkBuf2 = ASCIIEncoding.ASCII.GetBytes(track2);
                trkBuf3 = ASCIIEncoding.ASCII.GetBytes(track3);

                int result = ZBRPRNWriteMag(_handle, _prnType, trksToWrite, trkBuf1, trkBuf2, trkBuf3, out errValue);
                if(result != 1)
                    errMsg = "WriteMag failed. Error = " + Convert.ToString(errValue);

                return result;
            }
            catch(Exception ex)
            {
                errMsg = ex.Message;
            }
            finally
            {
                trkBuf1 = null;
                trkBuf2 = null;
                trkBuf3 = null;
            }
            return 0;
        }

        private IntPtr AllocateUnmanagedArray(byte[] arr)
        {
            int size = Marshal.SizeOf(arr[0]) * arr.Length;
            IntPtr pointer = Marshal.AllocHGlobal(size);
            return pointer;
        }

        private void InitUnmanagedArray(ref IntPtr pointer, byte[] unmanagedArray)
        {
            int uidSize = Marshal.SizeOf(unmanagedArray[0]) * unmanagedArray.Length;
            pointer = Marshal.AllocHGlobal(uidSize);
            Marshal.Copy(unmanagedArray, 0, pointer, unmanagedArray.Length);
        }
        
        private void FreeUnmanagedMemory(IntPtr pointer)
        {
            Marshal.FreeHGlobal(pointer);
        }

        private void CopyUnmanagedArray(IntPtr pointer, ref byte[] ManagedArray, int paramLength)
        {
            Marshal.Copy(ManagedArray, 0, pointer, ManagedArray.Length);
        }

        private void CopyToUnmanagedArray(IntPtr pointer, ref byte[] ManagedArray, int paramLength)
        {
            Marshal.Copy(pointer, ManagedArray, 0, ManagedArray.Length);
        }
                    
        #endregion

        #region WaitForJobToComplete
        private void WaitForJobToComplete(string printerName, int jobID, out string status)
        {
            status = string.Empty;
            SampleCodeGraphics g = null;
            ZBRPrinter printer = null;
            try
            {
                int errValue = 0;
                g = new SampleCodeGraphics();
                printer = new ZBRPrinter();

                while (true)
                {
                    bool ready = g.IsPrinterReady(printerName, 60, out status);
                    if (!ready && string.IsNullOrEmpty(status))
                    {
                        if (printer.IsPrinterInErrorMode(out errValue))
                        {
                            status = "Printer in error mode: " + Convert.ToString(errValue);
                            break;
                        }
                    }
                    else if (ready) //print job completed
                    {
                        if (printer.IsPrinterInErrorMode(out errValue))
                        {
                            status = "Printer in error mode: " + Convert.ToString(errValue);
                            break;
                        }
                        status = "Success";
                        break;
                    }
                    else if (!string.IsNullOrEmpty(status))
                    {
                        string temp = string.Empty;
                        g.CancelJob(printerName, jobID, out temp);
                        status += " " + temp;
                        break;
                    }

                    if (printer.IsPrinterInErrorMode(out errValue))
                    {
                        status = "Printer in error mode: " + Convert.ToString(errValue);
                        break;
                    }
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                status = "WaitForJobToComplete exception: " + e.Message;
            }
            finally
            {
                g = null;
                printer = null;
            }
        }
        #endregion
    }
}
