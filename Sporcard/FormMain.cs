using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
//using YN_TCPinterface;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json.Linq;



/* 2019.12.5 张博
 * 本次修改将读写卡升级为加密机读写，卡片版本为1.0。
 * 重新修改了数据库，之前的数据结构全部重做，本次直接将Json数据存入库中，要使用时直接调用，无需其他数据拼接。
 * 数据库移除了所有的标记字符，程序只用Mark标记符独自处理状态。
 * mark 标记 1 录入 2 制卡失败 3 制卡成功（未回盘） 4 回盘
 * 程序之前有两种颜色控制，蓝色为正常状态，红色为制卡失败状态，回盘成功移除该数据。先修改颜色，录入时为蓝色，制卡后为红色（即未回盘），回盘后移除数据。其他功能保持原状不变。
 * 程序之前未控制人员查询，导致未回盘（回盘后数据存入2表）但是已经制卡的数据会被新查询的个人数据替换掉，状态会从已制卡变成未制卡。现服务器端进行了数据处理，当一个人未回盘但多次查询返回的数据每次都是新生成数据，这样按照之前的流程，制卡后重新获取人员数据，获取到的人员数据和卡内数据不符，故流程上不再允许制卡成功人员（即红色标记人员）再次查询个人数据，只能进行回盘操作。
 * 卡管个人数据没有发卡日期一行，这将导致发卡日期的混乱，考虑到人员查询和制卡不在同一天，现以制卡当天为发卡日期
 * 统计功能要求统计所有打印过的卡片，包括制卡成功的卡片，故有2表，专门存放制卡成功数据（即Mark=4），在回盘时将1表内数据标记改为4，将该条数据存入表2，删除表1内数据
 */
namespace Sporcard
{
    public partial class FormMain : Form
    {
        public string IC_PRINT_FLAG = "";

        public FormMain()
        {
            //测试添加
            AccessOperator acc = new AccessOperator();

            InitializeComponent();
            InitFuncButton();

            //测试添加
            //acc.ExecuteNonQuery("delete from ProductData");
            //RefreshTree();
            RefreshTreeWithBatch();

            info.wdcode = ini.IniReadValue("DeviceSerial", "aff002");
            info.printname = ini.IniReadValue("DeviceSerial", "username");
            info.printcode = ini.IniReadValue("DeviceSerial", "aae011");
        }


        //public Dictionary<string, int> sbyydic = new Dictionary<string, int>();

        INIClass ini = new INIClass(System.Windows.Forms.Application.StartupPath + "\\TSconfig.ini");

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

        }

        #region ----------------控件事件----------------
        private void InitFuncButton()
        {
            ucFuncButton1.AddButton(null, btnRefresh_Click);
            ucFuncButton1.AddButton(Properties.Resources.个人查询, btnSearchPersonInfo_Click);
            ucFuncButton1.AddButton(Properties.Resources.制作卡片, btnProductCard_Click);
            ucFuncButton1.AddButton(Properties.Resources.回盘, btnReport_Click);
            ucFuncButton1.AddButton(Properties.Resources.制卡统计, btnExport_Click);
            txtIDNo.GotFocus += txtIDNo_GotFocus;
            txtIDNo.LostFocus += txtIDNo_LostFocus;
            txtName.GotFocus += txtName_GotFocus;
            txtName.LostFocus += txtName_LostFocus;
            cbCertificateType.SelectedIndex = 0;
            //comboBox_sbyy.SelectedIndex = 0;
        }

        private void ControlStatusEnabled(bool TrueOrFalse)
        {
            ucFuncButton1.Enabled = TrueOrFalse;
        }

        void tbReceipt_LostFocus(object sender, EventArgs e)
        {
        }

        void tbReceipt_GotFocus(object sender, EventArgs e)
        {
        }
        void tbName_LostFocus(object sender, EventArgs e)
        {
        }
        void tbName_GotFocus(object sender, EventArgs e)
        {
        }
        void txtIDNo_GotFocus(object sender, EventArgs e)
        {
            txtIDNo.Text = String.Equals(txtIDNo.Text.Trim(), "身份证号") ? "" : txtIDNo.Text;
        }
        void txtIDNo_LostFocus(object sender, EventArgs e)
        {
            txtIDNo.Text = String.Equals(txtIDNo.Text.Trim(), "") ? "身份证号" : txtIDNo.Text;
        }
        void txtName_GotFocus(object sender, EventArgs e)
        {
            txtName.Text = String.Equals(txtName.Text.Trim(), "姓名") ? "" : txtName.Text;
        }
        void txtName_LostFocus(object sender, EventArgs e)
        {
            txtName.Text = String.Equals(txtName.Text.Trim(), "") ? "姓名" : txtName.Text;
        }

        private void cbCertificateType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbCertificateType.SelectedIndex)
            {
                case 0:
                    GlobalClass.strCertificateType = "01";
                    break;
                case 1:
                    GlobalClass.strCertificateType = "04";
                    break;
                case 2:
                    GlobalClass.strCertificateType = "06";
                    break;
                case 3:
                    GlobalClass.strCertificateType = "07";
                    break;
                case 4:
                    GlobalClass.strCertificateType = "08";
                    break;
                default:
                    GlobalClass.strCertificateType = "";
                    break;
            }
            txtIDNo.Focus();
        }

        public PlatFormInterface pfi = PlatFormInterface.getInstance();

        Info info = new Info();

        public List<string> cardFeesList = new List<string>();
        public List<string> searchInfoList = new List<string>();
        public List<string> photoList = new List<string>();
        public List<string> CallBackList = new List<string>();
        public bool zkFlag = false;

        private void cbChangeType_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void tvCardInit_AfterSelect(object sender, TreeViewEventArgs e)
        {
            AccessOperator Accor = new AccessOperator();
            PrintParameter pp = new PrintParameter();
            TreeNode Node = (TreeNode)tvCardInit.SelectedNode;
            ucFuncButton1.SetButtonEnabled(3, false);
            ucFuncButton1.SetButtonEnabled(4, false);
            ucFuncButton1.SetButtonEnabled(7, false);
            if (Node.Parent == null)
            {
                pcPrintView.Image = Properties.Resources.社保正面;
            }
            else if (Node.Parent.Parent == null)
            {
                pcPrintView.Image = Properties.Resources.社保正面;

                ucFuncButton1.SetButtonEnabled(3, true);

            }

            string strsql = string.Format("SELECT [IDCARD],[NAME],[PHOTOPATH],[PRINTINFO] FROM NEWPRODUCTDATA WHERE [IDCARD]='{0}'", Node.Name);
            System.Data.DataTable dtread = Accor.ExecuteDataTable(strsql);
            if (dtread.Rows.Count > 0)
            {
                pp._photoPath = dtread.Rows[0]["PHOTOPATH"].ToString();
                pp.printerInfo = dtread.Rows[0]["PRINTINFO"].ToString();

                info.Clear();

                info.name = dtread.Rows[0]["NAME"].ToString();
                info.idcard = dtread.Rows[0]["IDCARD"].ToString();
            }

            PrintFormatView pfv = new PrintFormatView();
            pfv.DrawFormat(pp);
            pcPrintView.Image = pfv.image;
        }

        public void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshTreeWithBatch();
        }

        //查找个人数据
        public void btnSearchPersonInfo_Click(object sender, EventArgs e)
        {
            LogHelper.WriteLog(typeof(FormMain), "查询个人信息");

            ControlStatusEnabled(false);

            AccessOperator Accor = new AccessOperator();

            try
            {
                LogHelper.WriteLog(typeof(FormMain), string.Format("身份证号: {0}", txtIDNo.Text.Trim()));
                LogHelper.WriteLog(typeof(FormMain), string.Format("姓名: {0}", txtName.Text.Trim()));
                //查询制卡数据
                if (String.Equals(txtIDNo.Text.Trim(), "身份证号"))
                {
                    LogHelper.WriteLog(typeof(FormMain), "【身份证号】不能为空！");
                    MessageBox.Show("【身份证号】不能为空！");
                    return;
                }
                if (String.Equals(txtName.Text.Trim(), "姓名"))
                {
                    LogHelper.WriteLog(typeof(FormMain), "【姓名】不能为空！");
                    MessageBox.Show("【姓名】不能为空！");
                    return;
                }

                string strsql = string.Format("SELECT * FROM NEWPRODUCTDATA WHERE [IDCARD]='{0}' AND [MARK] >= 3", txtIDNo.Text.Trim());
                System.Data.DataTable dtread = Accor.ExecuteDataTable(strsql);
                if (dtread.Rows.Count > 0)
                {
                    MsgPutOut("查询数据", string.Format("查询个人信息失败，已有一条此人数据尚未回盘，请完成业务后重试"));
                    return;
                }

                int ret = 0;
                string message = "";
                string statusCode = "";

                if (searchInfoList != null)
                {
                    searchInfoList.Clear();
                }
                if (photoList != null)
                {
                    photoList.Clear();
                }
                if (CallBackList != null)
                {
                    CallBackList.Clear();
                }

                MsgPutOut("查询数据", "正在查询数据，请耐心等待...");
                JArray jArray = pfi.searchInfo(txtName.Text.Trim(), txtIDNo.Text.Trim(), info.wdcode, info.printname, info.printcode, out statusCode, out message);

                if (!statusCode.Equals("1"))
                {
                    MsgPutOut("查询数据", string.Format("查询个人信息失败 身份证：{0}， message：{1}:", txtIDNo.Text.Trim(), jArray[0]["message"].ToString()));
                    return;
                }

                ret = SaveData(jArray[0]["aac003"].ToString(), jArray[0]["aac002"].ToString(), jArray);
                if (ret < 0)
                {
                    MsgPutOut("查询数据", "保存个人数据失败");
                    return;
                }

                //RefreshTree();
                RefreshTreeWithBatch();

                MsgPutOut("查询数据", string.Format("查询个人信息成功 姓名：{0}，身份证：{1}", jArray[0]["aac003"].ToString(), jArray[0]["aac002"].ToString()));
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                ControlStatusEnabled(true);
            }
        }

        public void btnProductCard_Click(object sender, EventArgs e)
        {
            tvCardInit.Enabled = false;
            ControlStatusEnabled(false);

            try
            {
                if (tvCardInit.SelectedNode.Parent != null && tvCardInit.SelectedNode.Parent.Parent == null)
                {
                    LogHelper.WriteLog(typeof(FormMain), "btnProductCard_Click");
                    ucFlowChart1.Clear();
                    this.Refresh();
                    ProductCard(info.name, info.idcard);
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                RefreshTreeWithBatch();
                tvCardInit.Enabled = true;
                ControlStatusEnabled(true);
            }



        }

        public void btnExport_Click(object sender, EventArgs e)
        {
            ControlStatusEnabled(false);

            try
            {
                MsgPutOut("制卡统计", string.Format("制卡统计界面已启动"));

                CardStatistics cardStatistics = new CardStatistics();

                cardStatistics.Show();
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                ControlStatusEnabled(true);
            }
        }

        public void btnReport_Click(object sender, EventArgs e)
        {
            ControlStatusEnabled(false);
            try
            {
                ReportResult();
                RefreshTreeWithBatch();
            }
            catch
            {
                throw;
            }
            finally
            {
                tvCardInit.Enabled = true;
                ControlStatusEnabled(true);
            }
        }

        public void btnReadCard_Click(object sender, EventArgs e)
        {
        }

        public void btnSearchBatch_Click(object sender, EventArgs e)
        {
        }

        public void btnDownLoadBatch_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region ----------------事件实现----------------
        public string printName = "Zebra ZXP Series 3C USB Card Printer";

        private void RefreshTree()
        {
            AccessOperator Accor = new AccessOperator();
            tvCardInit.Nodes.Clear();

            string strsql = "SELECT [Status01],[XM27],[SHBZHM28] FROM ProductData WHERE [Status01]='1' OR [Status01]='2' OR [Status01]='3'";
            System.Data.DataTable dtread = Accor.ExecuteDataTable(strsql);
            TreeNode Root = tvCardInit.Nodes.Add(string.Format("个人补换卡[数量:{0}]", dtread.Rows.Count));
            TreeNode Node = null;
            foreach (DataRow dr in dtread.Rows)
            {
                Node = Root.Nodes.Add(dr["SHBZHM28"].ToString(), dr["SHBZHM28"] + "_" + dr["XM27"]);
                if (dr["Status01"].ToString().Equals("2"))
                {
                    Node.ForeColor = System.Drawing.Color.Red;
                }
                else if (dr["Status01"].ToString().Equals("3"))
                {
                    Node.ForeColor = System.Drawing.Color.Blue;
                }
            }
            tvCardInit.ExpandAll();
            tvCardInit.SelectedNode = Root;
        }

        private void RefreshTreeWithBatch()
        {
            LogHelper.WriteLog(typeof(FormMain), "RefreshTreeWithBatch Start");
            AccessOperator Accor = new AccessOperator();
            tvCardInit.Nodes.Clear();

            TreeNode Root = tvCardInit.Nodes.Add(string.Format("个人补换卡"));
            //TreeNode BatchNode = null;
            TreeNode PersonNode = null;
            LogHelper.WriteLog(typeof(FormMain), "ExecuteDataTable start");
            string strsql = "SELECT * FROM NEWPRODUCTDATA ORDER BY [ID] DESC";
            System.Data.DataTable dtread = Accor.ExecuteDataTable(strsql);
            LogHelper.WriteLog(typeof(FormMain), "ExecuteDataTable end");
            //BatchNode = Root.Nodes.Add("SingleProduct", string.Format("单张补换卡[数量:{0}]", dtread.Rows.Count));
            foreach (DataRow dr in dtread.Rows)
            {
                PersonNode = Root.Nodes.Add(dr["IDCard"].ToString(), dr["IDCard"] + "_" + dr["Name"]);
                if (dr["MARK"].ToString() == "3")
                {
                    PersonNode.ForeColor = System.Drawing.Color.Red;
                }
                else if (dr["MARK"].ToString().Trim() != "3")
                {
                    PersonNode.ForeColor = System.Drawing.Color.Blue;
                }
            }
            tvCardInit.ExpandAll();
            if (Root.FirstNode == null)
            {
                tvCardInit.SelectedNode = Root;
            }
            else
            {
                tvCardInit.SelectedNode = Root.FirstNode;
            }
        }

        private bool DownloadPresonData(string cardType,
                                        string idCard,
                                        string name,
                                        string oldBankNo,
                                        string grsbh,
                                        string areaCode,
                                        string changeType,
                                        string bankCode,
                                        string businessID)
        {
            string ALLYwdjh = string.Empty;
            string statusCode = string.Empty;
            string message = string.Empty;
            string sendData = string.Empty;
            string carddata = string.Empty;
            string photodata = string.Empty;

            // pfi.searchInfo(idCard,"","","",out statusCode, out message, out sendData);

            if (!statusCode.Equals("1"))
            {
                MsgPutOut("查询数据", string.Format("查询个人信息失败 身份证：{0}， message：{1}:", idCard, message));
                return false;
            }

            photoList = pfi.QueryPhoto(idCard,   //是	身份证号
            out statusCode, out message, out sendData);
            if (!statusCode.Equals("1"))
            {
                MsgPutOut("查询数据", string.Format("查询照片信息失败 姓名：{0}，身份证：{1}， message：{2}:", name, idCard, message));
                return false;
            }
            SavePhoto(photodata, photoList[1] + "_" + photoList[0]);
            RefreshTreeWithBatch();

            MsgPutOut("查询数据", string.Format("查询个人信息成功 姓名：{0}，身份证：{1}", photoList[0], photoList[1]));
            return true;
        }

        private void ProductCard(string name, string idcard)
        {
            LogHelper.WriteLog(typeof(FormMain), "ProductCard");
            //string ICdata = "";
            string reportStr = "";
            string strsql = string.Format("SELECT * FROM NEWPRODUCTDATA WHERE IDCARD = '{0}'", idcard);

            AccessOperator Accor = new AccessOperator();
            PrintParameter pp = new PrintParameter();
            System.Data.DataTable dtread = Accor.ExecuteDataTable(strsql);
            LogHelper.WriteLog(typeof(FormMain), "人员查询,SQL:" + strsql);

            pp.printerInfo = dtread.Rows[0]["PRINTINFO"].ToString();

            pp._photoPath = dtread.Rows[0]["PHOTOPATH"].ToString();

            int ret = ProductExecute(pp, dtread, out reportStr);
            LogHelper.WriteLog(typeof(FormMain), "ProductExecute : " + ret);

            if (ret != 0)
            {
                MsgPutOut("制作卡片", "制卡失败,错误:" + ret);
                LogHelper.WriteLog(typeof(FormMain), "制卡失败,错误:" + ret);

                strsql = string.Format("UPDATE NEWPRODUCTDATA SET [MARK]='2' WHERE [IDCARD]='{0}'", idcard);

                int count = Accor.ExecuteNonQuery(strsql);
                if (count <= 0)
                {
                    MsgPutOut("制作卡片", "更新本地数据库失败");
                    LogHelper.WriteLog(typeof(FormMain), "更新本地数据库失败");
                    LogHelper.WriteLog(typeof(FormMain), "SQL:" + strsql);
                }
                return;
            }
            else
            {
                MsgPutOut("制作卡片", "制卡成功！");
                LogHelper.WriteLog(typeof(FormMain), "制卡成功");

                strsql = string.Format("UPDATE [NEWPRODUCTDATA] SET [MARK]='3' WHERE [IDCard]='{0}'", idcard);

                LogHelper.WriteLog(typeof(FormMain), "SQL:" + strsql);

                int count = Accor.ExecuteNonQuery(strsql);
                if (count <= 0)
                {
                    MsgPutOut("制作卡片", "更新本地数据库失败");
                    LogHelper.WriteLog(typeof(FormMain), "更新本地数据库失败");
                }
                MessageBox.Show("制卡成功，请回盘");
            }
        }

        #region 导出数据

        private void Export(string startTime, string endTime, string fileName, string filePath)
        {
            LogHelper.WriteLog(typeof(FormMain), "Export");

            string strsql = string.Format("select PersonName as 姓名,IDCard as 身份证号,CardId as 社保卡号,BankNO as 银行卡号,DateSave as 制卡日期时间, Mark as 是否回盘 from ProductData where DateSave >= '{0}' and DateSave < {1} and Mark = 4", startTime, Convert.ToDateTime(endTime).AddDays(1).ToString("yyyy-MM-dd"));

            LogHelper.WriteLog(typeof(FormMain), "Sql: " + strsql);

            AccessOperator Accor = new AccessOperator();

            System.Data.DataTable dtread = Accor.ExecuteDataTable(strsql);

            exportDataTableToExcel(dtread, fileName, filePath);

        }

        private string exportDataTableToExcel(System.Data.DataTable dataTable, string fileName, string filePath)
        {
            Microsoft.Office.Interop.Excel.Application excel;

            Microsoft.Office.Interop.Excel._Workbook workBook;

            Microsoft.Office.Interop.Excel._Worksheet workSheet;

            object misValue = System.Reflection.Missing.Value;

            excel = new Microsoft.Office.Interop.Excel.Application();

            Microsoft.Office.Interop.Excel.Range excelRange;

            workBook = excel.Workbooks.Add(misValue);

            workSheet = (Microsoft.Office.Interop.Excel._Worksheet)workBook.ActiveSheet;

            int rowIndex = 3;

            int colIndex = 1;

            ((Microsoft.Office.Interop.Excel.Range)workSheet.Rows["1:2", System.Type.Missing]).RowHeight = 40;

            excel.ActiveSheet.Rows[1].Font.Size = 16;

            excel.ActiveSheet.Rows[1].Font.Bold = true;

            excelRange = (Range)workSheet.get_Range("A1", "G1");

            excelRange.Merge(0);

            excel.Cells[1, 1] = "遵义长征村镇银行制卡明细统计表";

            excel.ActiveSheet.Rows[1].HorizontalAlignment = XlHAlign.xlHAlignCenter;

            excel.ActiveSheet.Rows[2].Font.Size = 12;

            excel.ActiveSheet.Rows[2].Font.Bold = true;

            excelRange = (Range)workSheet.get_Range("A2", "G2");

            excelRange.Merge(0);

            excel.Cells[2, 1] = "统计开始时间：" + fileName.Split('-')[0] + "       统计结束时间：" + fileName.Split('-')[1];

            excel.ActiveSheet.Rows[2].HorizontalAlignment = XlHAlign.xlHAlignLeft;

            excelRange = (Range)workSheet.get_Range("A1", "G1");

            excelRange.ColumnWidth = 15;

            excel.Cells[3, 1] = "序号";

            //取得标题  
            foreach (DataColumn col in dataTable.Columns)
            {
                colIndex++;

                excel.Cells[3, colIndex] = col.ColumnName;
            }

            //取得表格中的数据  
            foreach (DataRow row in dataTable.Rows)
            {
                rowIndex++;

                colIndex = 0;

                foreach (DataColumn col in dataTable.Columns)
                {
                    colIndex++;

                    excel.Cells[rowIndex, colIndex] = colIndex;

                    excel.Cells[rowIndex, colIndex] =

                          row[col.ColumnName].ToString().Trim();

                    //设置表格内容居中对齐  
                    //workSheet.get_Range(excel.Cells[rowIndex, colIndex],

                    //  excel.Cells[rowIndex, colIndex]).HorizontalAlignment =

                    //  Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                }
            }

            excelRange = (Range)workSheet.get_Range(string.Format("A{0}", rowIndex + 1), string.Format("F{0}", rowIndex + 1));

            excelRange.Merge(0);

            excelRange.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

            excel.Cells[rowIndex + 1, 1] = "制卡张数:";

            excel.Cells[rowIndex + 1, 7] = rowIndex - 3 + "张";

            excel.Cells[rowIndex + 1, 7].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;

            excelRange.Merge(0);

            ((Microsoft.Office.Interop.Excel.Range)workSheet.Rows[string.Format("{0}:{0}", rowIndex + 1), System.Type.Missing]).RowHeight = 40;

            excel.Visible = true;

            string saveFile = filePath + "\\" + fileName + ".xls";

            if (File.Exists(saveFile))
            {
                File.Delete(saveFile);
            }

            workBook.SaveAs(saveFile, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValue,

                misValue, misValue, misValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive,

                misValue, misValue, misValue, misValue, misValue);

            dataTable = null;

            workBook.Close(true, misValue, misValue);

            excel.Quit();

            PublicMethod.Kill(excel);//调用kill当前excel进程  

            releaseObject(workSheet);

            releaseObject(workBook);

            releaseObject(excel);

            if (!File.Exists(saveFile))
            {
                return null;
            }
            return saveFile;
        }

        /// <summary>
        /// 释放COM组件对象
        /// </summary>
        /// <param name="obj"></param>
        private static void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch
            {
                obj = null;
            }
            finally
            {
                GC.Collect();
            }
        }

        /// <summary>
        /// 关闭进程的内部类
        /// </summary>
        public class PublicMethod
        {
            [DllImport("User32.dll", CharSet = CharSet.Auto)]

            public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);

            public static void Kill(Microsoft.Office.Interop.Excel.Application excel)
            {
                //如果外层没有try catch方法这个地方需要抛异常。
                IntPtr t = new IntPtr(excel.Hwnd);

                int k = 0;

                GetWindowThreadProcessId(t, out k);

                System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(k);

                p.Kill();
            }
        }

        #endregion


        public int ProductExecute(PrintParameter printPra, System.Data.DataTable dtread, out string outCardData)
        {
            //INIClass ini = new INIClass(System.Windows.Forms.Application.StartupPath + "\\TSconfig.ini");
            string bWriteIC = ini.IniReadValue("BWRITEIC", "bWriteIC");
            LogHelper.WriteLog(typeof(FormMain), "ProductExecute");
            int ret = 0;
            int err = 0;
            outCardData = "";
            ZBRPrinter zb = new ZBRPrinter();
            try
            {
                //打开打印机
                for (int i = 0; i < 4; i++)
                {
                    ret = zb.Open(out err);
                    LogHelper.WriteLog(typeof(FormMain), string.Format("Open  ret:{0},err:{1}", ret, err));
                    if (ret == 1 && err == 0)
                        break;
                    Thread.Sleep(5000);
                }

                if (ret != 1 || err != 0)
                {
                    MsgPutOut("制作卡片", "打开打印机失败");
                    LogHelper.WriteLog(typeof(FormMain), string.Format("打开打印机失败 ret:{0},err:{1}", ret, err));
                    return -101;
                }
                //读取色带剩余量
                //zb.ZBRPRNGetPanelsRemaining

                LogHelper.WriteLog(typeof(FormMain), "打开打印机成功");
                //byte[] strPrintSerial = null;
                //ret = zb.GetDeviceSerial(out strPrintSerial, out err);
                //GlobalClass.DeviceSerial = System.Text.Encoding.ASCII.GetString(strPrintSerial);


                //进卡
                ret = zb.StartPostionCard(1, out err);
                if (ret != 1)
                {
                    ucFlowChart1.SetStepStatus(1, false);
                    MsgPutOut("制作卡片", "进卡失败");
                    LogHelper.WriteLog(typeof(FormMain), string.Format("StartPostionCard ret:{0},err:{1}", ret, err));
                    return -102;
                }

                LogHelper.WriteLog(typeof(FormMain), string.Format("进卡成功 ret:{0},err:{1}", ret, err));

                ucFlowChart1.SetStepStatus(1, true);
                this.Refresh();

                Thread.Sleep(2000);

                //停两秒,卡片到位

                #region 写IC

                if (bWriteIC == "true")
                {

                    //写IC
                    MsgPutOut("制作卡片", "正在写卡，请耐心等待...");
                    LogHelper.WriteLog(typeof(FormMain), "正在写卡，请耐心等待...");
                    ret = WriteIC(dtread);

                    LogHelper.WriteLog(typeof(FormMain), "WriteIC ret:" + ret);

                    if (ret != 0)
                    {
                        MsgPutOut("制作卡片", "写卡失败，错误代码:" + ret);
                        LogHelper.WriteLog(typeof(FormMain), "写卡失败，错误代码:" + ret);
                        ucFlowChart1.SetStepStatus(2, false);
                        zb.EjectCard(out err);
                        return -103;
                    }

                    LogHelper.WriteLog(typeof(FormMain), "写卡成功：" + ret);

                    ucFlowChart1.SetStepStatus(2, true);
                    this.Refresh();
                }

                #endregion
                LogHelper.WriteLog(typeof(FormMain), "DrawPhoto");
                MsgPutOut("制作卡片", "正在打印，请耐心等待...");
                try
                {
                    LogHelper.WriteLog(typeof(ZBRPrinter), "PrintCard  ");
                    // ret = PrintProduct.StartPrint(printHandle, 2, 0);    

                    LogHelper.WriteLog(typeof(ZBRPrinter), printPra._photoPath);

                    ret = zb.PrintCard(printPra, out err);

                    LogHelper.WriteLog(typeof(ZBRPrinter), "PrintCard:err:  " + err);
                }
                catch
                {
                    LogHelper.WriteLog(typeof(FormMain), "StartPrint 异常");
                }

                LogHelper.WriteLog(typeof(FormMain), "StartPrint ret :" + ret);


                if (ret != 1)
                {
                    ucFlowChart1.SetStepStatus(3, false);
                    MsgPutOut("制作卡片", string.Format("打印失败,ret:{0}", ret));
                    LogHelper.WriteLog(typeof(FormMain), "打印失败，错误代码:" + ret);
                    return -107;
                }

                LogHelper.WriteLog(typeof(FormMain), "打印成功 ret: " + ret);
                MsgPutOut("制作卡片", "打印成功");
                Thread.Sleep(2000);


                ucFlowChart1.SetStepStatus(3, true);
                this.Refresh();

                return 0;
            }
            catch (System.Exception ex)
            {
                MsgPutOut("制作卡片", "打印异常，请重新拔插，或重启打印机");
                LogHelper.WriteLog(typeof(FormMain), "打印异常，请重新拔插，或重启打印机");
                LogHelper.WriteLog(typeof(FormMain), ex.Message);
                return -108;
            }
            finally
            {
                zb.Close(out err);
            }
        }

        private void ReportResult()
        {
            //INIClass ini = new INIClass(System.Windows.Forms.Application.StartupPath + "\\TSconfig.ini");
            string sendData = "";
            string statusCode = "";
            string message = "";
            string recode = "";
            string strsql = "";
            //string compareStr = "";
            AccessOperator Accor = new AccessOperator();
            string DeviceSerial = ini.IniReadValue("DeviceSerial", "serial");

            string sql = string.Format("SELECT * FROM NEWPRODUCTDATA WHERE [IDCARD] = '{0}'", info.idcard);

            System.Data.DataTable dtread = Accor.ExecuteDataTable(sql);

            if (dtread.Rows[0]["ksbm"].ToString().Trim().Equals("") || dtread.Rows[0]["ATR"].ToString().Trim().Equals("") || dtread.Rows[0]["BankCardNo"].ToString().Trim().Equals(""))
            {
                MsgPutOut("数据回盘", "卡识别码或ATR或银行卡号为空，请检查该人员有无制卡成功");
                return;
            }

            //01身份证
            MsgPutOut("数据回盘", "正在回盘数据，请耐心等待...");
            LogHelper.WriteLog(typeof(FormMain), "正在回盘数据，请耐心等待...");
            PrintParameter pp = new PrintParameter();

            recode = pfi.callDataBack(info.wdcode, info.printname, info.printcode, info.idcard, info.name, dtread.Rows[0]["ksbm"].ToString(), dtread.Rows[0]["ATR"].ToString(), dtread.Rows[0]["BankCardNo"].ToString(), dtread.Rows[0]["CardId"].ToString(), out statusCode, out message, out sendData);

            if (recode.Equals("1"))
            {
                strsql = string.Format("UPDATE NEWPRODUCTDATA SET [MARK]='4' WHERE [IDCARD]='{0}'", info.idcard);
                int i = Accor.ExecuteNonQuery(strsql);

                strsql = string.Format("INSERT INTO NEWPRODUCTDATAS SELECT * FROM NEWPRODUCTDATA WHERE([IDCARD] = '{0}' AND MARK = 4)", info.idcard);
                i = Accor.ExecuteNonQuery(strsql);

                strsql = string.Format("DELETE FROM NEWPRODUCTDATA WHERE [IDCARD] = '{0}' AND [MARK] = 4", info.idcard);
                i = Accor.ExecuteNonQuery(strsql);

                ucFlowChart1.SetStepStatus(4, true);
                MsgPutOut("数据回盘", "回盘成功");
                pp.printstr[0] = "";
                pp.printstr[1] = "";
                pp.printstr[2] = "";
                pp.printstr[3] = "";
                pp._photoPath = "";
            }
            else
            {
                strsql = string.Format("UPDATE NEWPRODUCTDATA SET [MARK]='3' WHERE [IDCARD]='{0}'", info.idcard);
                Accor.ExecuteNonQuery(strsql);

                ucFlowChart1.SetStepStatus(4, false);
                MsgPutOut("数据回盘:", message);
            }
        }

        #endregion


        protected override void DefWndProc(ref System.Windows.Forms.Message m)
        {
            int messageType = m.Msg;
            switch (m.Msg)
            {
                case GlobalClass.WM_MY_MESSAGE:
                    MsgPutOut("发送加密机数据", GlobalClass.sendJMJData);
                    break;
                default:
                    base.DefWndProc(ref m);//一定要调用基类函数，以便系统处理其它消息。
                    break;
            }
        }
        #region ----------------通用功能----------------
        private void MsgPutOut(string type, string msg)
        {
            this.Invoke(new MethodInvoker(delegate
            {
                tbShow.Text += string.Format("[{0}]->:{1}\r\n", type, msg);
                tbShow.Select(tbShow.TextLength, 0);
                tbShow.ScrollToCaret();
                this.Refresh();
            }));

        }

        private void SavePhoto(string strbase, string photoname)
        {
            //无相片字符串bmpBase64则推出
            if (strbase.Equals(""))
            {
                return;
            }
            try
            {
                MemoryStream stream = new MemoryStream(Convert.FromBase64String(strbase));
                Bitmap img = new Bitmap(stream);
                string dir = System.Windows.Forms.Application.StartupPath + @"\Photo";
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                string photoFullname = dir + "\\" + photoname + ".jpg";
                img.Save(photoFullname);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private int SaveData(string name, string idcard, JArray jArray)
        {
            AccessOperator Accor = new AccessOperator();
            int ret = -1;
            //如果本地数据库已经有此人的数据，则不插入
            string sql = string.Format("SELECT * FROM NEWPRODUCTDATA WHERE [IDCARD]='{0}'", idcard);
            System.Data.DataTable dtread = Accor.ExecuteDataTable(sql);
            if (dtread.Rows.Count > 0)
            {
                Accor.ExecuteNonQuery(string.Format("DELECT FROM NEWPRODUCTDATA WHERE [IDCARD]='{0}'", idcard));

            }

            #region 保存照片和采集打印数据
            SavePhoto(jArray[0]["zp"].ToString(), jArray[0]["aac002"].ToString() + "_" + jArray[0]["aac003"].ToString());
            string photoPath = System.Windows.Forms.Application.StartupPath + @"\Photo\" + jArray[0]["aac002"].ToString() + "_" + jArray[0]["aac003"].ToString() + ".jpg";

            string strTimeNow = jArray[0]["yaa405"].ToString();
            string pPrintInfo = "姓名  " + jArray[0]["aac003"] + "|" + "社会保障号码  " + jArray[0]["aac002"] + "|" + "卡号  " + jArray[0]["aaz501"] + "|" + "有效期  " + strTimeNow.Substring(0, 4) + "年" + strTimeNow.Substring(4, 2) + "月";
            #endregion

            string tableFields = "[NAME],[IDCARD],[JSONINFO],[MARK],[PHOTOPATH],[PRINTINFO],[CARDID],[KSBM]";
            string insertvalue = string.Format("'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}'", name, idcard, jArray.ToString(), "1", photoPath, pPrintInfo, jArray[0]["aaz501"].ToString(), jArray[0]["ksbm"].ToString());

            string strsql = string.Format("INSERT INTO NEWPRODUCTDATA ({0}) VALUES ({1})", tableFields, insertvalue);
            LogHelper.WriteLog(typeof(FormMain), "strsql:" + strsql);
            int count = Accor.ExecuteNonQuery(strsql);
            ret = count > 0 ? 0 : -1; //插入成功或失败

            return ret;
        }

        private int SaveData(string idcard, string name, string photopath, string personid, string printinfo, string cardid, string ef05ssse,
            string ef06ssse, string ef05df01, string ef06df01, string ef07df01, string ef09df01)
        {
            AccessOperator Accor = new AccessOperator();
            //如果本地数据库已经有此人的数据，则不插入
            string sql = string.Format("SELECT [IDCard] FROM ProductData WHERE [IDCard]='{0}' And Mark <> 3", idcard);
            System.Data.DataTable dtread = Accor.ExecuteDataTable(sql);
            if (dtread.Rows.Count > 0)
            {

                Accor.ExecuteNonQuery(string.Format("delete from ProductData where [IDCard]='{0}'", idcard));
                string tableFields = "[IDCard],[PersonName],[PhotoPath],[PersonSerial],[PrintInfo],[CardID],[EF05SSSE],[EF06SSSE],[EF05DF01],[EF06DF01],[EF07DF01],[EF09DF01],[DateSave],[Mark]";
                //string[] tabledata = tableFields.Split(',');
                //string[] data = carddata.Split('|');
                string insertvalue = string.Format("'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}'", idcard, name, photopath, personid, printinfo, cardid, ef05ssse, ef06ssse, ef05df01, ef06df01, ef07df01, ef09df01, DateTime.Now, 1);
                string strsql = "insert into ProductData (" + tableFields + ") values (" + insertvalue + ")";
                int count = Accor.ExecuteNonQuery(strsql);
                int ret = count > 0 ? 0 : -1; //插入成功或失败
                return ret;
            }
            else
            {
                return -1;
            }
        }

        private int SaveBatchData(DataRow drBatchData)
        {
            int ret = 0;
            AccessOperator Accor = new AccessOperator();
            //如果本地数据库已经有此人的数据，则删除
            string sql = "Delete * From T_Batch WHERE [ZJHM]='" + drBatchData["aae135"].ToString() + "'";
            ret = Accor.ExecuteNonQuery(sql);

            string tableFields = "BatchID,ZJLX,ZJHM,XM,YYHKH,SGRSBH,XZQHBM,KZT,KBGLX,YHWDBM,YWDJH,CreateTime";
            string insertvalue = string.Format("'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}'",
                                                drBatchData["batchid"].ToString(),  //批次号
                                                drBatchData["aac058"].ToString(),   //证件类型
                                                drBatchData["aae135"].ToString(),   //证件号码
                                                drBatchData["aac003"].ToString(),   //姓名
                                                drBatchData["baz923"].ToString(),   //原银行卡号
                                                drBatchData["baz805"].ToString(),   //省个人识别号
                                                drBatchData["aaa027"].ToString(),   //行政区划编码
                                                drBatchData["aaz502"].ToString(),   //卡状态
                                                drBatchData["by1"].ToString(),      //卡变更类型
                                                drBatchData["by2"].ToString(),      //银行网点编码
                                                drBatchData["by3"].ToString(),      //业务单据号
                                                drBatchData["createtime"].ToString());  //创建时间
            string strsql = "insert into T_Batch (" + tableFields + ") values (" + insertvalue + ")";
            int count = Accor.ExecuteNonQuery(strsql);
            ret = count > 0 ? 0 : -1; //插入成功或失败
            return ret;
        }

        private int WriteIC(string ef05ssse, string ef06ssse, string ef05df01, string ef06df01, string ef07df01, string ef09df01)
        {
            int ret = 0;
            int iReaderHandle = 0;
            //string[] parms = new string[searchInfoList.Count];
            //for(int i = 0; i < searchInfoList.Count; i++)
            //{
            //    parms[i] = searchInfoList[i];
            //}
            string[] sEf05SSSE = ef05ssse.Split(',');
            string[] sEf06SSSE = ef06ssse.Split(',');
            string[] sEf05Df01 = ef05df01.Split(',');
            string[] sEf06Df01 = ef06df01.Split(',');
            string[] sEf07Df01 = ef07df01.Split(',');
            string[] sEf09Df01 = ef09df01.Split(',');
            GlobalClass.strATR = "";
            GlobalClass.strBankNo = "";
            GlobalClass.personID = "";
            GlobalClass.name = "";
            GlobalClass.idcard = "";//身份证号码
            GlobalClass.sbNo = "";
            GlobalClass.cardcertcode = "";
            GlobalClass.fkrq = "";
            GlobalClass.kyxq = "";
            StringBuilder atr = new StringBuilder(36);
            StringBuilder szMac = new StringBuilder(16);
            StringBuilder szBankNo = new StringBuilder(24);
            StringBuilder szValidDate = new StringBuilder(10);
            StringBuilder szCardCertID = new StringBuilder();
            StringBuilder szCardType = new StringBuilder();
            StringBuilder szCardVersion = new StringBuilder();
            StringBuilder szOrgDeptID = new StringBuilder();
            StringBuilder szDispCardDate = new StringBuilder();
            StringBuilder szExpireDate = new StringBuilder();
            StringBuilder szCardID = new StringBuilder();
            string now = DateTime.Now.ToString("yyyyMMdd");
            string ten = now.Substring(0, 2) + (Convert.ToInt32(now.Substring(2, 2)) + 10).ToString() + now.Substring(4);
            LogHelper.WriteLog(typeof(FormMain), "iDOpenPort");
            iReaderHandle = LSCard.iDOpenPort(1, 5);
            //打开读写器
            if (iReaderHandle <= 0)
            {
                LogHelper.WriteLog(typeof(FormMain), "iDOpenPort 失败");
                return iReaderHandle;
            }

            LogHelper.WriteLog(typeof(FormMain), "iDInitReader");
            ret = LSCard.iDInitReader(iReaderHandle, atr);
            if (ret != 0)
            {
                LogHelper.WriteLog(typeof(FormMain), "iDInitReader 失败");
                return ret;
            }
            MsgPutOut("制作卡片", "读卡器上电成功");
            LogHelper.WriteLog(typeof(FormMain), "iCalcuVerifyCodeWithPSAM");
            ret = LSCard.iCalcuVerifyCodeWithPSAM(iReaderHandle, sEf05SSSE[0].Substring(0, 6), sEf05SSSE[0], szMac);
            if (ret != 0)
            {
                LogHelper.WriteLog(typeof(FormMain), "iCalcuVerifyCodeWithPSAM 失败");
                return ret;
            }

            LogHelper.WriteLog(typeof(FormMain), "iDInitReader");
            ret = LSCard.iDInitReader(iReaderHandle, atr);
            if (ret != 0)
            {
                LogHelper.WriteLog(typeof(FormMain), "iDInitReader 失败");
                return ret;
            }
            ret = LSCard.iRMFCardDeptInfo(iReaderHandle, szCardCertID, szCardType, szCardVersion, szOrgDeptID, szDispCardDate, szExpireDate, szCardID);
            if (ret != 0)
            {
                LogHelper.WriteLog(typeof(FormMain), "iRMFCardDeptInfo 失败");
                return ret;
            }

            LogHelper.WriteLog(typeof(FormMain), "szCardCertID：" + szCardCertID.ToString());
            LogHelper.WriteLog(typeof(FormMain), "卡识别码：" + sEf05SSSE[0] + szMac.ToString().Substring(0, 8));
            ret = LSCard.iWMFCardDeptInfo(iReaderHandle, szCardCertID.ToString().Substring(0, 6), sEf05SSSE[0] + szMac.ToString().Substring(0, 8), sEf05SSSE[1], sEf05SSSE[2],
                sEf05SSSE[3], now, ten, sEf05SSSE[4]);
            if (ret != 0)
            {
                LogHelper.WriteLog(typeof(FormMain), "iWMFCardDeptInfo 失败");
                return ret;
            }
            ret = LSCard.iWMFCardOwnerInfo(iReaderHandle, szCardCertID.ToString().Substring(0, 6), sEf06SSSE[0], sEf06SSSE[1], sEf06SSSE[2], sEf06SSSE[3], sEf06SSSE[4], sEf06SSSE[5]);
            if (ret != 0)
            {
                LogHelper.WriteLog(typeof(FormMain), "iWMFCardOwnerInfo 失败");
                return ret;
            }
            ret = LSCard.iWAPPRegResidInfo(iReaderHandle, sEf05Df01[0], sEf05Df01[1].Length > 62 ? sEf05Df01[1].Substring(0, 62) : sEf05Df01[1], sEf05Df01[1].Length > 62 ? sEf05Df01[1].Substring(62) : "");
            if (ret != 0)
            {
                LogHelper.WriteLog(typeof(FormMain), "iWAPPRegResidInfo 失败");
                return ret;
            }
            ret = LSCard.iWAPPCommInfo(iReaderHandle, sEf06Df01[0].Length > 62 ? sEf06Df01[0].Substring(0, 62) : sEf06Df01[0], sEf06Df01[0].Length > 62 ? sEf06Df01[0].Substring(62) : "", sEf06Df01[1], sEf06Df01[2]);
            if (ret != 0)
            {
                LogHelper.WriteLog(typeof(FormMain), "iWAPPCommInfo 失败");
                return ret;
            }
            ret = LSCard.iWAPPPersonStatus(iReaderHandle, sEf07Df01[0], sEf07Df01[1]);
            if (ret != 0)
            {
                LogHelper.WriteLog(typeof(FormMain), "iWAPPPersonStatus 失败");
                return ret;
            }
            ret = LSCard.iWAPPDeptInfo(iReaderHandle, sEf09Df01[0].Length > 62 ? sEf09Df01[0].Substring(0, 62) : sEf09Df01[0], sEf09Df01[0].Length > 62 ? sEf09Df01[0].Substring(62) : "", sEf09Df01[1]);
            if (ret != 0)
            {
                LogHelper.WriteLog(typeof(FormMain), "iWAPPDeptInfo 失败");
                return ret;
            }
            ret = LSCard.iDInitReader(iReaderHandle, atr);
            if (ret != 0)
            {
                LogHelper.WriteLog(typeof(FormMain), "iDInitReader 失败");
                return ret;
            }
            ret = LSCard.iGetBankNO(iReaderHandle, szBankNo);
            if (ret != 0)
            {
                LogHelper.WriteLog(typeof(FormMain), "iGetBankNO 失败");
                return ret;
            }

            ret = LSCard.iGetValiDate(iReaderHandle, szValidDate);
            if (ret != 0)
            {
                LogHelper.WriteLog(typeof(FormMain), "iGetValiDate 失败");
                return ret;
            }

            GlobalClass.strATR = atr.ToString();
            GlobalClass.strBankNo = szBankNo.ToString();
            GlobalClass.strValidDate = szValidDate.ToString();

            //GlobalClass.personID = sEf06SSSE[0];
            //GlobalClass.name = sEf06SSSE[1];
            //GlobalClass.idcard = sEf06SSSE[5];//身份证号码
            GlobalClass.sbNo = sEf05SSSE[4];
            GlobalClass.cardcertcode = sEf05SSSE[0] + szMac.ToString().Substring(0, 8);
            GlobalClass.fkrq = now;
            GlobalClass.kyxq = ten;

            LSCard.iDCloseReader(iReaderHandle);
            return ret;
        }

        private int WriteIC(System.Data.DataTable dtread)
        {
            int iRet = 0;

            AccessOperator Accor = new AccessOperator();

            JArray jArray = JArray.Parse(dtread.Rows[0]["JSONINFO"].ToString());

            StringBuilder strOutInfo = new StringBuilder(1024);

            string DateSave = DateTime.Now.ToString("yyyyMMdd");

            iRet = iCalSSCardCode(0, 1, 3, dtread.Rows[0]["KSBM"].ToString(), string.Format("JKEY|0000000000000000|$HSM52|{0}|{1}|{2}|$", info.printname, info.wdcode, info.printcode), strOutInfo);
            if (!iRet.Equals(0))
            {
                LogHelper.WriteLog(typeof(FormMain), "iCalSSCardCode失败");
                return iRet;
            }

            string ksbm = strOutInfo.ToString();

            strOutInfo.Clear();
            iRet = iReadBankNumExt(0, 1, strOutInfo);
            string BankCardNo = strOutInfo.ToString();
            if (!iRet.Equals(0))
            {
                LogHelper.WriteLog(typeof(FormMain), "iReadBankNumExt失败");
                return iRet;
            }

            strOutInfo.Clear();
            iRet = iGetATRExt(0, 1, strOutInfo);
            string ATR = strOutInfo.ToString();
            if (!iRet.Equals(0))
            {
                LogHelper.WriteLog(typeof(FormMain), "iGetATRExt失败");
                return iRet;
            }

            string cardWriteInfo1 = "SSSEEF05|01|02|03|04|05|06|07|$SSSEEF06|08|09|0A|0B|0D|$DF01EF05|20|21|$DF01EF06|25|27|28|$";

            //string cardWriteInfo2 = $"SSSEEF05|{(ksbm == "[]" ? ksbm : "")}|{jArray[0]["yaa402"].ToString()}|{jArray[0]["yaa403"].ToString()}|{jArray[0]["yaa404"].ToString()}|{DateSave}|{jArray[0]["yaa405"].ToString()}|{jArray[0]["aaz501"].ToString()}|$SSSEEF06|{jArray[0]["aac002"].ToString()}|{jArray[0]["aac003"].ToString()}|{jArray[0]["aac004"].ToString()}|{jArray[0]["aac005"].ToString()}|{jArray[0]["aac006"].ToString()}|$DF01EF05|{jArray[0]["aac009"].ToString()}|{jArray[0]["aac010"].ToString()}|$DF01EF06|{jArray[0]["aae006"].ToString()}|{jArray[0]["aae007"].ToString()}|{jArray[0]["aae005"].ToString()}|$";

            string cardWriteInfo2 = $"SSSEEF05|" +
                $"{(ksbm == "[]" ? "" : ksbm)}|" +
                $"{(jArray[0]["yaa402"].ToString() == "[]" ? "" : jArray[0]["yaa402"].ToString())}|" +
                $"{(jArray[0]["yaa403"].ToString() == "[]" ? "" : jArray[0]["yaa403"].ToString())}|" +
                $"{(jArray[0]["yaa404"].ToString() == "[]" ? "" : jArray[0]["yaa404"].ToString())}|" +
                $"{(DateSave == "[]" ? "" : DateSave)}|" +
                $"{(jArray[0]["yaa405"].ToString() == "[]" ? "" : jArray[0]["yaa405"].ToString())}|" +
                $"{(jArray[0]["aaz501"].ToString() == "[]" ? "" : jArray[0]["aaz501"].ToString())}|" +
                $"$SSSEEF06|" +
                $"{(jArray[0]["aac002"].ToString() == "[]" ? "" : jArray[0]["aac002"].ToString())}|" +
                $"{(jArray[0]["aac003"].ToString() == "[]" ? "" : jArray[0]["aac003"].ToString())}|" +
                $"{(jArray[0]["aac004"].ToString() == "[]" ? "" : jArray[0]["aac004"].ToString())}|" +
                $"{(jArray[0]["aac005"].ToString() == "[]" ? "" : jArray[0]["aac005"].ToString())}|" +
                $"{(jArray[0]["aac006"].ToString() == "[]" ? "" : jArray[0]["aac006"].ToString())}|" +
                $"$DF01EF05|" +
                $"{(jArray[0]["aac009"].ToString() == "[]" ? "" : jArray[0]["aac009"].ToString())}|" +
                $"{(jArray[0]["aac010"].ToString() == "[]" ? "" : jArray[0]["aac010"].ToString())}|" +
                $"$DF01EF06|" +
                $"{(jArray[0]["aae006"].ToString() == "[]" ? "" : jArray[0]["aae006"].ToString())}|" +
                $"{(jArray[0]["aae007"].ToString() == "[]" ? "" : jArray[0]["aae007"].ToString())}|" +
                $"{(jArray[0]["aae005"].ToString() == "[]" ? "" : jArray[0]["aae005"].ToString())}|$";

            //cardWriteInfo2 = "SSSEEF05|520300D156000005050842704E187F49|3|1.00|91560000025202005202006A|20191120|20291120|BA0010547|$SSSEEF06|520102198506020233|林岩|1|02|19850602|$DF01EF05|10|贵州省遵义市红花岗区凤凰南路豆芽湾巷3号2单元附12号|$DF01EF06|贵州省遵义市红花岗区凤凰南路豆芽湾巷3号2单元附13号|563001|18293128473|$";

            strOutInfo.Clear();
            LogHelper.WriteLog(typeof(FormMain), "cardWriteInfo1：" + cardWriteInfo1);
            LogHelper.WriteLog(typeof(FormMain), "cardWriteInfo2：" + cardWriteInfo2);
            iRet = iRepairCardExt(0, 1, 3, cardWriteInfo1, cardWriteInfo2, string.Format("HSM52|{0}|{1}|{2}|$CODE|520300|$", info.printname, info.wdcode, info.printcode), strOutInfo);
            if (!iRet.Equals(0))
            {
                LogHelper.WriteLog(typeof(FormMain), "iRepairCardExt失败");
                return iRet;
            }

            strOutInfo.Clear();
            //DF01EF05|20|21|$DF01EF06|25|27|28|$
            string strRead = "SSSEEF05|01|02|03|04|05|06|07|$SSSEEF06|08|09|0A|0B|0D|$";
            iRet = iReadCardExt(0, 1, 3, "", strRead, string.Format("HSM52|{0}|{1}|{2}|$", info.printname, info.wdcode, info.printcode), strOutInfo);
            if (!iRet.Equals(0))
            {
                LogHelper.WriteLog(typeof(FormMain), "iReadCardExt失败");
                return iRet;
            }

            //string strWrite = string.Format("SSSEEF05|{0}|{1}|{2}|{3}|{4}|{5}|{6}|$SSSEEF06|{7}|{8}|{9}|{10}|{11}|$",
            //ksbm, jArray[0]["yaa402"].ToString(), jArray[0]["yaa403"].ToString(), jArray[0]["yaa404"].ToString(), DateSave, jArray[0]["yaa405"].ToString(), jArray[0]["aaz501"].ToString(),

            // jArray[0]["aac002"].ToString(), jArray[0]["aac003"].ToString(), jArray[0]["aac004"].ToString(), jArray[0]["aac005"].ToString(), jArray[0]["aac006"].ToString());

            string strWrite = $"SSSEEF05|" +
                $"{(ksbm == "[]" ? "" : ksbm)}|" +
                $"{(jArray[0]["yaa402"].ToString() == "[]" ? "" : jArray[0]["yaa402"].ToString())}|" +
                $"{(jArray[0]["yaa403"].ToString() == "[]" ? "" : jArray[0]["yaa403"].ToString())}|" +
                $"{(jArray[0]["yaa404"].ToString() == "[]" ? "" : jArray[0]["yaa404"].ToString())}|" +
                $"{(DateSave == "[]" ? "" : DateSave)}|" +
                $"{(jArray[0]["yaa405"].ToString() == "[]" ? "" : jArray[0]["yaa405"].ToString())}|" +
                $"{(jArray[0]["aaz501"].ToString() == "[]" ? "" : jArray[0]["aaz501"].ToString())}|" +
                $"$SSSEEF06|" +
                $"{(jArray[0]["aac002"].ToString() == "[]" ? "" : jArray[0]["aac002"].ToString())}|" +
                $"{(jArray[0]["aac003"].ToString() == "[]" ? "" : jArray[0]["aac003"].ToString())}|" +
                $"{(jArray[0]["aac004"].ToString() == "[]" ? "" : jArray[0]["aac004"].ToString())}|" +
                $"{(jArray[0]["aac005"].ToString() == "[]" ? "" : jArray[0]["aac005"].ToString())}|" +
                $"{(jArray[0]["aac006"].ToString() == "[]" ? "" : jArray[0]["aac006"].ToString())}|$";

            if (!strOutInfo.ToString().Equals(strWrite))
            {
                LogHelper.WriteLog(typeof(FormMain), "读取数据和写入数据不符");
                LogHelper.WriteLog(typeof(FormMain), "写入：" + strWrite);

                LogHelper.WriteLog(typeof(FormMain), "读取：" + strOutInfo.ToString());
               
                return iRet = -1;
            }

            string strsql = string.Format("UPDATE NEWPRODUCTDATA SET [KSBM]= '{0}' , [BANKCARDNO] = '{1}' , [ATR] = '{2}' , [DATESAVE] = '{3}'  WHERE [IDCARD]='{4}'", ksbm, BankCardNo, ATR, DateTime.Now.ToString("yyyy-MM-dd"), info.idcard);
            if (Accor.ExecuteNonQuery(strsql).Equals(0))
            {
                LogHelper.WriteLog(typeof(FormMain), "读取数据写入数据库失败");
            }

            return iRet;
        }

        public MemoryStream ReadFile(string path)
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

        public Image GetFile(string path)
        {
            MemoryStream stream = ReadFile(path);
            return stream == null ? null : Image.FromStream(stream);
        }

        private void comboBox_sbyy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_sbyy.SelectedIndex == 0)
            {
                GlobalClass.backErrMsgType = "0";//制卡失败原因代码
                GlobalClass.zkErrorCode = "10";//制卡代码
            }
            else
            {
                GlobalClass.backErrMsgType = comboBox_sbyy.SelectedIndex.ToString();//制卡失败原因代码
                GlobalClass.zkErrorCode = "11";//制卡代码
            }

        }


        #endregion

        #region//dll部分
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct IDCardData
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string Name; //姓名   
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
            public string Sex;   //性别
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
            public string Nation; //名族
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
            public string Born; //出生日期
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 72)]
            public string Address; //住址
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 38)]
            public string IDCardNo; //身份证号
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string GrantDept; //发证机关
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
            public string UserLifeBegin; // 有效开始日期
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
            public string UserLifeEnd;  // 有效截止日期
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 38)]
            public string reserved; // 保留
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 25536)]
            public string PhotoFileName; // 照片路径
        }
        /************************端口类API *************************/
        [DllImport("Syn_IDCardRead.dll", EntryPoint = "Syn_GetCOMBaud", CharSet = CharSet.Ansi)]
        public static extern int Syn_GetCOMBaud(int iComID, ref uint puiBaud);
        [DllImport("Syn_IDCardRead.dll", EntryPoint = "Syn_SetCOMBaud", CharSet = CharSet.Ansi)]
        public static extern int Syn_SetCOMBaud(int iComID, uint uiCurrBaud, uint uiSetBaud);
        [DllImport("Syn_IDCardRead.dll", EntryPoint = "Syn_OpenPort", CharSet = CharSet.Ansi)]
        public static extern int Syn_OpenPort(int iPortID);
        [DllImport("Syn_IDCardRead.dll", EntryPoint = "Syn_ClosePort", CharSet = CharSet.Ansi)]
        public static extern int Syn_ClosePort(int iPortID);

        /************************ SAM类API *************************/
        [DllImport("Syn_IDCardRead.dll", EntryPoint = "Syn_GetSAMStatus", CharSet = CharSet.Ansi)]
        public static extern int Syn_GetSAMStatus(int iPortID, int iIfOpen);
        [DllImport("Syn_IDCardRead.dll", EntryPoint = "Syn_ResetSAM", CharSet = CharSet.Ansi)]
        public static extern int Syn_ResetSAM(int iPortID, int iIfOpen);
        [DllImport("Syn_IDCardRead.dll", EntryPoint = "Syn_GetSAMID", CharSet = CharSet.Ansi)]
        public static extern int Syn_GetSAMID(int iPortID, ref byte pucSAMID, int iIfOpen);
        [DllImport("Syn_IDCardRead.dll", EntryPoint = "Syn_GetSAMIDToStr", CharSet = CharSet.Ansi)]
        public static extern int Syn_GetSAMIDToStr(int iPortID, ref byte pcSAMID, int iIfOpen);
        /********************身份证卡类API *************************/
        [DllImport("Syn_IDCardRead.dll", EntryPoint = "Syn_StartFindIDCard", CharSet = CharSet.Ansi)]
        public static extern int Syn_StartFindIDCard(int iPortID, ref byte pucManaInfo, int iIfOpen);
        [DllImport("Syn_IDCardRead.dll", EntryPoint = "Syn_SelectIDCard", CharSet = CharSet.Ansi)]
        public static extern int Syn_SelectIDCard(int iPortID, ref byte pucManaMsg, int iIfOpen);
        [DllImport("Syn_IDCardRead.dll", EntryPoint = "Syn_ReadMsg", CharSet = CharSet.Ansi)]
        public static extern int Syn_ReadMsg(int iPortID, int iIfOpen, ref IDCardData pIDCardData);
        /********************附加类API *****************************/
        [DllImport("Syn_IDCardRead.dll", EntryPoint = "Syn_SendSound", CharSet = CharSet.Ansi)]
        public static extern int Syn_SendSound(int iCmdNo);
        [DllImport("Syn_IDCardRead.dll", EntryPoint = "Syn_DelPhotoFile", CharSet = CharSet.Ansi)]
        public static extern void Syn_DelPhotoFile();
        #endregion

        #region

        [DllImport("TSSelfSendDev.dll", EntryPoint = "iReadCardExt", CharSet = CharSet.Ansi)]
        public static extern int iReadCardExt(int iReaderCtrl, int iType, int iAuthType, string pPIN, string pFileAddr, string pUserInfo, StringBuilder pOutInfo);

        [DllImport("TSSelfSendDev.dll", EntryPoint = "iRepairCardExt", CharSet = CharSet.Ansi)]
        public static extern int iRepairCardExt(int iReaderCtrl, int iType, int iAuthType, string pFileAddr, string pFileInfo, string pUserInfo, StringBuilder pOutInfo);

        [DllImport("TSSelfSendDev.dll", EntryPoint = "iReadBankNumExt", CharSet = CharSet.Ansi)]
        public static extern int iReadBankNumExt(int iReaderCtrl, int iType, StringBuilder pOutInfo);

        [DllImport("TSSelfSendDev.dll", EntryPoint = "iGetATRExt", CharSet = CharSet.Ansi)]
        public static extern int iGetATRExt(int iReaderCtrl, int iType, StringBuilder pOutInfo);

        [DllImport("TSSelfSendDev.dll", EntryPoint = "iCalSSCardCode", CharSet = CharSet.Ansi)]
        public static extern int iCalSSCardCode(int iReaderCtrl, int iType, int iAuthType, string pCardId, string pUserInfo, StringBuilder pOutInfo);

        #endregion

        private void btnReadIDCard_Click(object sender, EventArgs e)
        {
            try
            {
                txtIDNo.Text = string.Empty;
                txtName.Text = string.Empty;
                string idcard = string.Empty;
                string name = string.Empty;
                string errorMsg = string.Empty;
                int ret = iGetIDCardandName(out idcard, out name, out errorMsg);
                if (ret != 0)
                {
                    MessageBox.Show(errorMsg);
                    return;
                }
                else
                {
                    //MessageBox.Show("idcar:" + idcard + ";;name:" + name);
                    this.txtIDNo.Text = idcard;
                    this.txtName.Text = name;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        int iGetIDCardandName(out string idcard, out string name, out string errorMsg)
        {
            int nRet = 0;
            idcard = string.Empty;
            name = string.Empty;
            errorMsg = string.Empty;
            try
            {
                byte[] pucIIN = new byte[4];
                byte[] pucSN = new byte[8];
                IDCardData CardMsg = new IDCardData();
                int iPort = iGetReaderPort();
                nRet = Syn_OpenPort(iPort);
                if (nRet == 0)
                {
                    nRet = Syn_GetSAMStatus(iPort, 0);
                    Syn_StartFindIDCard(iPort, ref pucIIN[0], 0);
                    Syn_SelectIDCard(iPort, ref pucSN[0], 0);
                    Syn_ReadMsg(iPort, 0, ref CardMsg);
                    idcard = CardMsg.IDCardNo;
                    name = CardMsg.Name;
                    if (string.IsNullOrEmpty(idcard))
                    {
                        nRet = -3;
                        errorMsg = "身份证读卡器读身份信息失败";
                    }
                    //if (Syn_StartFindIDCard(iPort, ref pucIIN[0], 0) == 0)
                    //{
                    //    nRet = Syn_SelectIDCard(iPort, ref pucSN[0], 0);
                    //    if (Syn_ReadMsg(iPort, 0, ref CardMsg) == 0)
                    //    {
                    //        idcard = CardMsg.IDCardNo;
                    //        name = CardMsg.Name;
                    //    }
                    //    else
                    //    {
                    //        nRet = -3;
                    //        errorMsg = "身份证读卡器读身份信息失败";
                    //    }
                    //}
                    //else
                    //{
                    //    nRet = -2;
                    //    errorMsg = "身份证读卡器找卡失败";
                    //}
                }
                else
                {
                    nRet = -1;
                    errorMsg = "身份证读卡器端口打开失败";
                }
                Syn_ClosePort(iPort);
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                nRet = -4;
                errorMsg = ex.Message;
            }
            return nRet;

        }
        private int iGetReaderPort()
        {
            int iPort = 0;
            try
            {
                for (int port = 1001; port < 1017; port++)
                {
                    if (Syn_OpenPort(port) == 0)
                    {
                        if (Syn_GetSAMStatus(port, 0) == 0)
                        {
                            iPort = port;
                            Syn_ClosePort(port);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return iPort;
        }

    }
}
