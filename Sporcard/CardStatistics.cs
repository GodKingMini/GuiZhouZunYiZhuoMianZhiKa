using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Sporcard
{
    public partial class CardStatistics : Form
    {
        public CardStatistics()
        {
            InitializeComponent();

            SelectDataTable();
        }

        int ii = 18;  //每页的行数
        int rows;     //总行数
        int num = 1;      //开始页数
        int sum;      //总页数

        System.Data.DataTable dtread = null;

        private void SelectDataTable()
        {
            string strsql = string.Format("SELECT NAME AS 姓名,IDCARD AS 身份证号,CARDID AS 社保卡号,BANKCARDNO AS 银行卡号,DATESAVE AS 制卡日期时间, SWITCH(MARK = 4, '是', TRUE, '否') AS 是否回盘 FROM NEWPRODUCTDATA UNION ALL SELECT NAME AS 姓名,IDCARD AS 身份证号,CARDID AS 社保卡号,BANKCARDNO AS 银行卡号,DATESAVE AS 制卡日期时间, SWITCH(MARK = 4, '是', TRUE, '否') AS 是否回盘 FROM NEWPRODUCTDATAS WHERE MARK IN (3,4) AND DATESAVE >= #{0}# and DATESAVE < #{1}# ORDER BY 制卡日期时间 ASC", dateTP_Start.Value.ToString("yyyy-MM-dd"), Convert.ToDateTime(dateTP_End.Value).AddDays(1).ToString("yyyy-MM-dd"));

            LogHelper.WriteLog(typeof(FormMain), "Sql: " + strsql);

            AccessOperator Accor = new AccessOperator();

            dtread = Accor.ExecuteDataTable(strsql);

            dataGridView1.DataSource = dtread;

            //FenYe(dtread);

            label5.Text = dtread.Rows.Count.ToString() + "张";
        }

        private void dateTP_Start_ValueChanged(object sender, EventArgs e)
        {
            SelectDataTable();
        }

        private void lklb_Save_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SaveFileDialog saveFile_DownloadData = new SaveFileDialog();

            saveFile_DownloadData.FileName = dateTP_Start.Value.ToString("yyyy.MM.dd") + "-" + dateTP_End.Value.ToString("yyyy.MM.dd");

            saveFile_DownloadData.InitialDirectory = System.Windows.Forms.Application.StartupPath + "\\数据导出";

            if (saveFile_DownloadData.ShowDialog() == DialogResult.OK)
            {
                Export(dateTP_Start.Value.ToString("yyyy-MM-dd"), dateTP_End.Value.AddDays(1).ToString("yyyy-MM-dd"), (saveFile_DownloadData.FileName).Substring((saveFile_DownloadData.FileName).LastIndexOf("\\") + 1), (saveFile_DownloadData.FileName).Substring(0, (saveFile_DownloadData.FileName).LastIndexOf("\\")));
            }
            MessageBox.Show("保存结束");
        }

        #region 导出数据

        private void Export(string startTime, string endTime, string fileName, string filePath)
        {
            LogHelper.WriteLog(typeof(FormMain), "Export");

            string strsql = string.Format("SELECT NAME AS 姓名,IDCARD AS 身份证号,CARDID AS 社保卡号,BANKCARDNO AS 银行卡号,DATESAVE AS 制卡日期时间, SWITCH(MARK = 4, '是', TRUE, '否') AS 是否回盘 FROM NEWPRODUCTDATA UNION ALL SELECT NAME AS 姓名,IDCARD AS 身份证号,CARDID AS 社保卡号,BANKCARDNO AS 银行卡号,DATESAVE AS 制卡日期时间, SWITCH(MARK = 4, '是', TRUE, '否') AS 是否回盘 FROM NEWPRODUCTDATAS WHERE MARK IN (3,4) AND DATESAVE >= #{0}# and DATESAVE < #{1}#  ORDER BY 制卡日期时间 ASC", startTime, endTime);

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

                colIndex = 1;

                foreach (DataColumn col in dataTable.Columns)
                {
                    colIndex++;

                    if (colIndex == 3)
                    {
                        excel.Cells[rowIndex, colIndex].NumberFormatLocal = "@";
                    }

                    excel.Cells[rowIndex, 1] = rowIndex - 3;

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

            ((Range)workSheet.Rows[string.Format("{0}:{0}", rowIndex + 1), System.Type.Missing]).RowHeight = 40;

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

        private void dateTP_End_ValueChanged(object sender, EventArgs e)
        {
            SelectDataTable();
        }

        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(e.RowBounds.Location.X,
      e.RowBounds.Location.Y,
      dataGridView1.RowHeadersWidth - 4,
      e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics,
                  (e.RowIndex + 1).ToString(),
                   dataGridView1.RowHeadersDefaultCellStyle.Font,
                   rectangle,
                   dataGridView1.RowHeadersDefaultCellStyle.ForeColor,
                   TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void FenYe(System.Data.DataTable dataTable)
        {
            //    if (oo != -1)
            //    {
            //        num = oo;
            //    }
            System.Data.DataTable dt = dataTable.Clone(); //复制原数据的结构

            rows = dataTable.Rows.Count; //总行数
            sum = rows % ii == 0 ? rows / ii : (rows / ii) + 1; //总页数=总行数/每页的行数(如果除不尽则+1)

            if (sum == 0)
            {
                sum++;
            }

            int ks = (num * ii) - ii; //开始行数（当前页数*每页的行数）-每页的行数
            int js = num * ii; //结束行数
            if (num == sum)
            {
                js = ks + (rows - ((num - 1) * ii)); //当最后一页时，结束的行数 = 开始行数 + (总行数 - （（总页数-1） * 每页的行数）)
            }
            //实现分页
            if (rows != 0) //必须要有一条记录
            {
                for (int i = ks; i < js; i++)
                {
                    dt.ImportRow(dataTable.Rows[i]); //复制行
                }
            }
            label6.Text = num.ToString() + "/" + sum.ToString();

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (num == 1)
            {
                return;
            }

            num--;
            //FenYe(dtread);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (num == sum) //最后一页时直接跳出
            {
                return;
            }
            num++;
            //FenYe(dtread);
        }
    }
}
