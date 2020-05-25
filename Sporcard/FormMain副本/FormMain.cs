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
using YN_TCPinterface;

namespace Sporcard
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            InitFuncButton();
            RefreshTree();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //int ret = TCPinterface.ConnectToServer();
            //if (ret != 0)
            //{
            //    MessageBox.Show("连接远程服务失败！");
            //    this.Close();
            //}
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //TCPinterface.DisConnectServer();
        }

        #region ----------------控件事件----------------
        private void InitFuncButton()
        {
            ucFuncButton1.AddButton(null, btnRefresh_Click);
            ucFuncButton1.AddButton(Properties.Resources.个人查询, btnSearchPersonInfo_Click);
            ucFuncButton1.AddButton(Properties.Resources.制作卡片, btnProductCard_Click);
            ucFuncButton1.AddButton(Properties.Resources.回盘, btnReport_Click);
            ucFuncButton1.AddButton(Properties.Resources.读卡, btnReadCard_Click);
            
            txtIDNo.GotFocus += txtIDNo_GotFocus;
            txtIDNo.LostFocus += txtIDNo_LostFocus;
            tbName.GotFocus += tbName_GotFocus;
            tbName.LostFocus += tbName_LostFocus;
            tbBankNo.GotFocus += tbReceipt_GotFocus;
            tbBankNo.LostFocus += tbReceipt_LostFocus;
            cbCertificateType.SelectedIndex = 0;
            cbChangeType.SelectedIndex = 0;
        }

        void tbReceipt_LostFocus(object sender, EventArgs e)
        {
            tbBankNo.Text = String.Equals(tbBankNo.Text.Trim(), "") ? "原银行卡号" : tbBankNo.Text;
        }
        void tbReceipt_GotFocus(object sender, EventArgs e)
        {
            tbBankNo.Text = String.Equals(tbBankNo.Text.Trim(), "原银行卡号") ? "" : tbBankNo.Text;
        }
        void tbName_LostFocus(object sender, EventArgs e)
        {
            tbName.Text = String.Equals(tbName.Text.Trim(), "") ? "姓名" : tbName.Text;
        }
        void tbName_GotFocus(object sender, EventArgs e)
        {
            tbName.Text = String.Equals(tbName.Text.Trim(), "姓名") ? "" : tbName.Text;
        }
        void txtIDNo_GotFocus(object sender, EventArgs e)
        {
            txtIDNo.Text = String.Equals(txtIDNo.Text.Trim(), "身份证号") ? "" : txtIDNo.Text;
        }
        void txtIDNo_LostFocus(object sender, EventArgs e)
        {
            txtIDNo.Text = String.Equals(txtIDNo.Text.Trim(), "") ? "身份证号" : txtIDNo.Text;
        }

        private string strCertificateType = "";

        private void cbCertificateType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbCertificateType.SelectedIndex)
            {
                case 0:
                    strCertificateType = "01";
                    break;
                case 1:
                    strCertificateType = "04";
                    break;
                case 2:
                    strCertificateType = "06";
                    break;
                case 3:
                    strCertificateType = "07";
                    break;
                case 4:
                    strCertificateType = "08";
                    break;
                default:
                    strCertificateType = "";
                    break;
            }
            txtIDNo.Focus();
        }

        private string changeType = "";

        private void cbChangeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbChangeType.SelectedIndex)
            {
                case 0:
                    changeType = "15";
                    break;
                case 1:
                    changeType = "14";
                    break;
                case 2:
                    changeType = "13";
                    break;
                case 3:
                    changeType = "12";
                    break;
                case 4:
                    changeType = "11";
                    break;
                default:
                    changeType = "";
                    break;
            }
            tbBankNo.Focus();
        }

        private void tvCardInit_AfterSelect(object sender, TreeViewEventArgs e)
        {
            AccessOperator Accor = new AccessOperator();
            TreeNode Node = (TreeNode)tvCardInit.SelectedNode;

            ucFuncButton1.SetButtonEnabled(3, false);
            ucFuncButton1.SetButtonEnabled(4, false);
            if (Node.Parent == null)
            {
                pcPrintView.Image = Properties.Resources.社保正面;
            }
            else
            {
                string strsql = "SELECT [Status01],[XM27],[SHBZHM28],[XB30],[SGRSBH37],[FullDATA59],[BY60],[BY61] FROM ProductData WHERE [SHBZHM28]='" + Node.Name + "'";
                DataTable dtread = Accor.ExecuteDataTable(strsql);
                if (dtread.Rows.Count > 0)
                {
                    GlobalClass.status = dtread.Rows[0]["Status01"].ToString();
                    GlobalClass.name = dtread.Rows[0]["XM27"].ToString();
                    GlobalClass.idcard = dtread.Rows[0]["SHBZHM28"].ToString();
                    GlobalClass.sex = dtread.Rows[0]["XB30"].ToString();
                    GlobalClass.provNo = dtread.Rows[0]["SGRSBH37"].ToString();
                    GlobalClass.fulldata = dtread.Rows[0]["FullDATA59"].ToString();
                    GlobalClass.idcardType = dtread.Rows[0]["BY60"].ToString();
                    GlobalClass.receipt = dtread.Rows[0]["BY61"].ToString();

                    if (GlobalClass.status.Equals("1") || GlobalClass.status.Equals("2"))
                    {
                        ucFuncButton1.SetButtonEnabled(3, true);
                    }
                    else if (GlobalClass.status.Equals("3"))
                    {
                        ucFuncButton1.SetButtonEnabled(4, true);
                    }

                    PrintParameter pp = new PrintParameter();
                    PrintFormatView pfv = new PrintFormatView();
                    pfv.DrawFormat(pp);
                    pcPrintView.Image = pfv.image;
                }
            }
        }

        public void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshTree();
        }

        public void btnSearchPersonInfo_Click(object sender, EventArgs e)
        {
            //查询制卡数据
            if (String.Equals(txtIDNo.Text.Trim(), "身份证号") || String.Equals(tbName.Text.Trim(), "姓名"))
            {
                MessageBox.Show("【身份证号】【姓名】不能为空！");
                return;
            }
            if (2 != cbChangeType.SelectedIndex && String.Equals(tbBankNo.Text.Trim(), "原银行卡号"))
            {
                MessageBox.Show("【原银行卡号】不能为空！");
                return;
            }

            int ret;
            string recode = "";
            string receipt = "";
            string carddata = "";
            string photodata = "";
            AccessOperator Accor = new AccessOperator();
            MsgPutOut("查询数据", "正在查询数据，请耐心等待...");
            //查询业务单据号
            recode = TCPinterface.CheckCardFees(tbName.Text.Trim(), strCertificateType, txtIDNo.Text.Trim(), changeType, tbBankNo.Text.Trim(), out receipt);
            if (!recode.Equals("0000"))
            {
                MsgPutOut("查询数据", "查询业务单据号失败:" + receipt);
                return;
            }

            recode = TCPinterface.SearchPersonInfo(tbName.Text.Trim(), strCertificateType, txtIDNo.Text.Trim(), changeType, tbBankNo.Text.Trim(), receipt, out carddata, out photodata);
            if (recode.Equals("0000"))
            {
                string[] data = carddata.Split('|');
                ret = SaveData(data[20],carddata, strCertificateType, receipt);
                if (ret < 0)
                {
                    MsgPutOut("查询数据", "保存个人数据失败");
                    return;
                }
                SavePhoto(photodata, data[20] + "_" + data[19]);
                RefreshTree();
                MsgPutOut("查询数据", "查询成功");
            }
            else
            {
                MsgPutOut("查询数据", carddata);
            }
        }

        public void btnProductCard_Click(object sender, EventArgs e)
        {
            ucFlowChart1.Clear();
            this.Refresh();
            ProductCard(GlobalClass.name, GlobalClass.idcardType, GlobalClass.idcard, GlobalClass.receipt);
        }

        public void btnReport_Click(object sender, EventArgs e)
        {
            AccessOperator Accor = new AccessOperator();
            TreeNode Node = (TreeNode)tvCardInit.SelectedNode;

            string strsql = "SELECT [Status01],[CGBZ02],[SBYY03],[KSDM04],[YHKH05],[KSBM06],[ATR07],[XM27],[SHBZHM28] FROM ProductData WHERE [SHBZHM28]='" + Node.Name + "' AND [Status01]='3'";
            DataTable dtread = Accor.ExecuteDataTable(strsql);
            string strResult = dtread.Rows[0]["CGBZ02"].ToString() + "|"
                                +dtread.Rows[0]["SBYY03"].ToString() + "|"
                                +dtread.Rows[0]["KSDM04"].ToString() + "|"
                                +dtread.Rows[0]["YHKH05"].ToString() + "|"
                                +dtread.Rows[0]["KSBM06"].ToString() + "|"
                                +dtread.Rows[0]["ATR07"];
            tvCardInit.Enabled = false;
            ReportResult(dtread.Rows[0]["XM27"].ToString(),GlobalClass.idcardType, dtread.Rows[0]["SHBZHM28"].ToString(),GlobalClass.receipt, strResult);
            RefreshTree();
            tvCardInit.Enabled = true;
        }

        public void btnReadCard_Click(object sender, EventArgs e)
        {
            //StringBuilder sb = new StringBuilder();
            //string outstring = "";
            //TCPinterface.AccessJMJ(GlobalClass.name, strCertificateType, GlobalClass.idcard, GlobalClass.receipt, "F6X0001904081709010001FF9009001389C73A497EAA86FD84D4000124", out outstring);
            //TS_DLL.iTest(TS_DLL.SendMsgToJMJ,sb);

            string reportStr = "";
            int ret = ReadCardInfo(out reportStr);
            if (ret == -1)
            {
                MsgPutOut("读社保卡", "打开读写器端口失败");
            }
            else if (ret == -2)
            {
                MsgPutOut("读社保卡", "读银行卡号失败");
            }
            else if (ret == -3)
            {
                MsgPutOut("读社保卡", "读卡识别码失败");
            }
            else 
            {
                MsgPutOut("读社保卡","读卡结果:" + reportStr);
            }
        }

        #endregion

        #region ----------------事件实现----------------
        public string printName = "Zebra ZXP Series 3C USB Card Printer";

        private void RefreshTree()
        {
            AccessOperator Accor = new AccessOperator();
            tvCardInit.Nodes.Clear();
            string strsql = "SELECT [Status01],[XM27],[SHBZHM28] FROM ProductData WHERE [Status01]='1' OR [Status01]='2' OR [Status01]='3'";
            DataTable dtread = Accor.ExecuteDataTable(strsql);
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

        private void ProductCard(string name, string idcardType, string idNo, string receipt)
        {
            string ICdata = "";
            string reportStr = "";
            string strsql = "";
            
            ICdata = TCPinterface.WriteICBaseData(name, idcardType, idNo, receipt) + "#" + GlobalClass.fulldata;

            AccessOperator Accor = new AccessOperator();
            PrintParameter pp = new PrintParameter();

            tvCardInit.Enabled = false;
            int ret = ProductExecute(pp, ICdata, out reportStr);
            if (ret != 0)
            {
                MsgPutOut("制作卡片", "制卡失败。错误:" + ret);
                strsql = "UPDATE ProductData SET [Status01]='2' WHERE [SHBZHM28]='" + idNo + "'";
                int count = Accor.ExecuteNonQuery(strsql);
                if (count <= 0)
                {
                    MsgPutOut("制作卡片", "更新本地数据库失败");
                }
            }
            else
            {
                MsgPutOut("制作卡片", "制卡成功！");
                string[] strrep = reportStr.Split('|');
                strsql = "UPDATE ProductData SET [Status01]='3',[CGBZ02]='1',[SBYY03]='',[KSDM04]='"
                    + strrep[0] + "',[YHKH05]='" + strrep[1] + "',[KSBM06]='" + strrep[2] + "',[ATR07]='" + strrep[3] + "' WHERE [SHBZHM28]='" + idNo + "'";
                int count = Accor.ExecuteNonQuery(strsql);
                if (count <= 0)
                {
                    MsgPutOut("制作卡片", "更新本地数据库失败");
                }
                ReportResult(name,idcardType, idNo,receipt, reportStr);
            }
            RefreshTree();
            tvCardInit.Enabled = true;
        }

        public int ProductExecute(PrintParameter printPra, string ICdata, out string outCardData)
        {
            int ret = 0;
            outCardData = "";
            PrintProduct ppro = new PrintProduct();
            //打开打印机
            int printHandle = PrintProduct.OpenPrinter(printName);
            if (printHandle <= 0)
            {
                MsgPutOut("制作卡片", "打开打印机失败");
                return -101;
            }

            //进卡
            int steps = 100;
            ret = PrintProduct.FeedCard(printHandle, 0, steps);
            if (ret != 1)
            {
                ucFlowChart1.SetStepStatus(1, false);
                MsgPutOut("制作卡片", "进卡失败");
                return -102;
            }
            ucFlowChart1.SetStepStatus(1, true);
            this.Refresh();

            //写IC
            MsgPutOut("制作卡片", "正在写卡，请耐心等待...");
            ret = WriteIC(ICdata);
            if (ret != 0)
            {
                ucFlowChart1.SetStepStatus(2, false);
                PrintProduct.Eject(printHandle);
                MsgPutOut("制作卡片", "写卡失败，错误代码:" + ret);
                return -103;
            }

            //读卡
            ret = ReadCardInfo(out outCardData);
            if (ret != 0)
            {
                MsgPutOut("制作卡片", "读卡失败，错误代码:" + ret);
                return -104;
            }
            ucFlowChart1.SetStepStatus(2, true);
            this.Refresh();

            //排版照片
            if (!ppro.DrawPhoto(printPra))
            {
                MsgPutOut("制作卡片", "排版照片失败");
                return -105;
            }

            //排版个人信息
            if (!ppro.DrawPersonInfo(printPra))
            {
                MsgPutOut("制作卡片", "排版个人信息失败");
                return -106;
            }

            //开始打印
            MsgPutOut("制作卡片", "正在打印，请耐心等待...");
            ret = PrintProduct.StartPrint(printHandle, 2, 0);
            if (ret != 1)
            {
                ucFlowChart1.SetStepStatus(3, false);
                MsgPutOut("制作卡片", "打印失败");
                return -107;
            }
            ucFlowChart1.SetStepStatus(3, true);
            this.Refresh();
            return 0;
        }

        private void ReportResult(string name,string idcardType, string cardID,string receipt, string result)
        { 
            string outstring;
            string strsql = "";
            AccessOperator Accor = new AccessOperator();
            //01身份证
            MsgPutOut("数据回盘", "正在回盘数据，请耐心等待...");
            string recode = TCPinterface.ReportResult(name, idcardType, cardID, receipt,result, out outstring);
            if (recode.Equals("0000"))
            {
                strsql = "UPDATE ProductData SET [Status01]='4' WHERE [SHBZHM28]='" + cardID + "'";
                ucFlowChart1.SetStepStatus(4, true);
                MsgPutOut("数据回盘", "回盘成功");
            }
            else
            {
                strsql = "UPDATE ProductData SET [Status01]='3' WHERE [SHBZHM28]='" + cardID + "'";
                ucFlowChart1.SetStepStatus(4, false);
                MsgPutOut("数据回盘", "回盘失败:" + outstring);
            }
            int count = Accor.ExecuteNonQuery(strsql);
            if (count <=0)
            {
                MsgPutOut("数据回盘", "更新本地数据库失败");
            }
        }

        #endregion

        #region ----------------通用功能----------------
        private void MsgPutOut(string type,string msg)
        {
            tbShow.Text += string.Format("[{0}]->:{1}\r\n",type,msg);
            tbShow.Select(tbShow.TextLength,0);
            tbShow.ScrollToCaret();
            this.Refresh();
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
                string dir = Application.StartupPath + @"\Photo";
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

        private int SaveData(string idcard, string carddata, string idcardType, string receipt)
        {
            //Status01,CGBZ02,SBYY03,KSDM04,YHKH05,KSBM06,ATR07,ShJPCH08,SJPCH09,CSMC10,
            //CJJGZKPCH11,CJJGMC12,XZQHDM13,XZQHMC14,DWBH15,DWMC16,YXBJ17,KDLB18,GFBB19,
            //CSHJGBM20,FKRQ21,KYXQ22,KH23,SGDBBH24,ZKZL25,RYCCSLDBM26,XM27,SHBZHM28,YHBM29,XB30,
            //MZ31,CSRQ32,XMKZ33,CSD34,BHKBZ35,CSKSBM36,SGRSBH37,ZZMM38,CZDZ39,HKXZ40,
            //HKSZD41,XL42,GRSF43,ZJLX44,ZJHM45,ZJYXQ46,ZJZZRQ47,LXDH48,GJ49,ZY50,
            //HYLB51,JHRXM52,JHRXB53,JHRZJLX54,JHRZJHM55,JHRDZ56,JHRLXDH57,JYHKH58,YL1,YL2
            AccessOperator Accor = new AccessOperator();
            string sql = "SELECT [SHBZHM28] FROM ProductData WHERE [SHBZHM28]='" + idcard + "'";
            DataTable dtread = Accor.ExecuteDataTable(sql);
            if (dtread.Rows.Count > 0)
                return 0;
            string tableFields = "Status01,CGBZ02,SBYY03,KSDM04,YHKH05,KSBM06,ATR07,ShJPCH08,SJPCH09,CSMC10,CJJGZKPCH11,CJJGMC12,XZQHDM13,XZQHMC14,DWBH15,DWMC16,YXBJ17,KDLB18,GFBB19,CSHJGBM20,FKRQ21,KYXQ22,KH23,SGDBBH24,ZKZL25,RYCCSLDBM26,XM27,SHBZHM28,YHBM29,XB30,MZ31,CSRQ32,XMKZ33,CSD34,BHKBZ35,CSKSBM36,SGRSBH37,ZZMM38,CZDZ39,HKXZ40,HKSZD41,XL42,GRSF43,ZJLX44,ZJHM45,ZJYXQ46,ZJZZRQ47,LXDH48,GJ49,ZY50,HYLB51,JHRXM52,JHRXB53,JHRZJLX54,JHRZJHM55,JHRDZ56,JHRLXDH57,JYHKH58,YL1,YL2,FullDATA59,BY60,BY61,BY62";
            string[] tabledata = tableFields.Split(',');
            string[] data = carddata.Split('|');
            string insertvalue = "'1','','','','','','','" + carddata.Replace("|", "','") + "','" + carddata + "','"+idcardType+"','"+receipt+"',''";
            string strsql = "insert into ProductData (" + tableFields + ") values (" + insertvalue + ")";
            int count = Accor.ExecuteNonQuery(strsql);
            int ret = count > 0 ? 0 : -1; //插入成功或失败
            return ret;
        }

        private int WriteIC(string ICdata)
        {
            int ret = 0;
            //打开读写器
            if (PrintProduct.ICCPowerOn("USB1") != 0)
                return -11;

            byte[] ucpATR = new byte[256];
            int nATRLen = 0;
            PrintProduct.ResetCard(ref ucpATR[0], ref nATRLen);
            //string stratr = System.Text.Encoding.Default.GetString(ucpATR, 0, nATRLen);
            //东信和平
            if (ucpATR[9] == 0x86 && ucpATR[10] == 0x60)
            {
                string[] data = ICdata.Split('#');
                //ret = EP_DLL.PersoCard(PrintProduct.Transmit, PrintProduct.ResetCard, "", 0, ICdata, ICdata.Length);
                ret = EP_DLL.PersoCard(PrintProduct.Transmit, PrintProduct.ResetCard, EP_DLL.SendMsgToJMJ, "", 0, data[1], data[1].Length);
                if (ret != 0)
                    return ret;
            }
            //金邦达
            else if ((ucpATR[9] == 0x86 && ucpATR[10] == 0x65) || (ucpATR[10] == 0x86 && ucpATR[11] == 0x65))
            {
                string[] data = ICdata.Split('#');
                ret = PAC_DLL.PersoCard(PrintProduct.Transmit, PrintProduct.ResetCard,PAC_DLL.SendMsgToJMJ, "", 0, data[1], data[1].Length);
                if (ret != 0)
                    return ret;
            }
            else if ((ucpATR[9] == 0x86 && ucpATR[10] == 0x49))
            {
                string[] data = ICdata.Split('#');
                ret = TS_DLL.iWriteCard(data[1],TS_DLL.SendMsgToJMJ);
                if (ret != 0)
                    return ret;
            }


            //关闭读写器
            PrintProduct.ICCPowerOff();
            return 0;
        }

        public int ReadCardInfo(out string reportStr)
        {
            int ret = 0;
            StringBuilder atr = new StringBuilder();
            StringBuilder bankno = new StringBuilder();

            StringBuilder szCardCertID = new StringBuilder();
            StringBuilder szCardType = new StringBuilder();
            StringBuilder szCardVersion = new StringBuilder();
            StringBuilder szOrgDeptID = new StringBuilder();
            StringBuilder szDispCardDate = new StringBuilder();
            StringBuilder szExpireDate = new StringBuilder();
            StringBuilder szCardID = new StringBuilder();

            reportStr = "";
            //卡商代码|银行卡号|卡识别码|ATR值
            int hCom = LSCard.iDOpenPort(1);
            if (hCom <= 0)
            {
                return -1;
            }
            ret = LSCard.iDInitReader(hCom, 0, atr);
            ret = LSCard.iGetBankNO(hCom, bankno);
            //if (ret != 0)
            //{
            //    return -2;
            //}
            ret = LSCard.iDInitReader(hCom, 0, atr);
            ret = LSCard.iRMFCardDeptInfo(hCom, szCardCertID, szCardType, szCardVersion, szOrgDeptID, szDispCardDate, szExpireDate, szCardID);
            if (ret != 0)
            {
                return -3;
            }
            reportStr = atr.ToString().Substring(18, 4) + "|" + bankno + "|" + szCardCertID + "|" + atr;
            return 0;
        }
        #endregion
    }
}
