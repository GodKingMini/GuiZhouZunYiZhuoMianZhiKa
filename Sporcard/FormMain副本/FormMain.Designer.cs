namespace Sporcard
{
    partial class FormMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("窗口1 - 甲");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("窗口1 - 乙");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("窗口2 - 丙");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("张三丰-[412622166802047623]");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("窗口3 - 丁", new System.Windows.Forms.TreeNode[] {
            treeNode4});
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("制卡列表", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode5});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.tvCardInit = new System.Windows.Forms.TreeView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.pnlRight_Fill = new System.Windows.Forms.Panel();
            this.tbShow = new System.Windows.Forms.TextBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.pnlRight_Middle = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbChangeType = new System.Windows.Forms.ComboBox();
            this.tbBankNo = new System.Windows.Forms.TextBox();
            this.tbName = new System.Windows.Forms.TextBox();
            this.txtIDNo = new System.Windows.Forms.TextBox();
            this.cbCertificateType = new System.Windows.Forms.ComboBox();
            this.pcPrintView = new System.Windows.Forms.PictureBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.pnlRight_Top = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lblFlow = new System.Windows.Forms.Label();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ucFlowChart1 = new Sporcard.UCFlowChart();
            this.ucFuncButton1 = new Sporcard.UCFuncButton();
            this.pnlBottom.SuspendLayout();
            this.pnlLeft.SuspendLayout();
            this.panel2.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.pnlRight_Fill.SuspendLayout();
            this.panel5.SuspendLayout();
            this.pnlRight_Middle.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pcPrintView)).BeginInit();
            this.panel4.SuspendLayout();
            this.pnlRight_Top.SuspendLayout();
            this.panel3.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBottom
            // 
            this.pnlBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(59)))), ((int)(((byte)(163)))), ((int)(((byte)(246)))));
            this.pnlBottom.Controls.Add(this.label4);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 699);
            this.pnlBottom.Margin = new System.Windows.Forms.Padding(2);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(978, 30);
            this.pnlBottom.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(418, 2);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(202, 21);
            this.label4.TabIndex = 1;
            this.label4.Text = "广东德生科技股份有限公司";
            // 
            // pnlLeft
            // 
            this.pnlLeft.Controls.Add(this.tvCardInit);
            this.pnlLeft.Controls.Add(this.panel2);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Location = new System.Drawing.Point(0, 67);
            this.pnlLeft.Margin = new System.Windows.Forms.Padding(2);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(243, 632);
            this.pnlLeft.TabIndex = 2;
            // 
            // tvCardInit
            // 
            this.tvCardInit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(234)))), ((int)(((byte)(250)))));
            this.tvCardInit.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvCardInit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvCardInit.Location = new System.Drawing.Point(0, 30);
            this.tvCardInit.Name = "tvCardInit";
            treeNode1.Name = "节点1";
            treeNode1.Text = "窗口1 - 甲";
            treeNode2.Name = "节点2";
            treeNode2.Text = "窗口1 - 乙";
            treeNode3.Name = "节点3";
            treeNode3.Text = "窗口2 - 丙";
            treeNode4.Name = "节点0";
            treeNode4.Text = "张三丰-[412622166802047623]";
            treeNode5.Name = "节点4";
            treeNode5.Text = "窗口3 - 丁";
            treeNode6.Name = "节点0";
            treeNode6.Text = "制卡列表";
            this.tvCardInit.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode6});
            this.tvCardInit.Size = new System.Drawing.Size(243, 602);
            this.tvCardInit.TabIndex = 14;
            this.tvCardInit.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvCardInit_AfterSelect);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(151)))), ((int)(((byte)(206)))), ((int)(((byte)(237)))));
            this.panel2.Controls.Add(this.label5);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(243, 30);
            this.panel2.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(10, 4);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(94, 21);
            this.label5.TabIndex = 0;
            this.label5.Text = "制卡列表";
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(243, 67);
            this.splitter1.Margin = new System.Windows.Forms.Padding(2);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(2, 632);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // pnlRight
            // 
            this.pnlRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(234)))), ((int)(((byte)(250)))));
            this.pnlRight.Controls.Add(this.pnlRight_Fill);
            this.pnlRight.Controls.Add(this.pnlRight_Middle);
            this.pnlRight.Controls.Add(this.pnlRight_Top);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRight.Location = new System.Drawing.Point(245, 67);
            this.pnlRight.Margin = new System.Windows.Forms.Padding(2);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(733, 632);
            this.pnlRight.TabIndex = 4;
            // 
            // pnlRight_Fill
            // 
            this.pnlRight_Fill.Controls.Add(this.tbShow);
            this.pnlRight_Fill.Controls.Add(this.panel5);
            this.pnlRight_Fill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRight_Fill.Location = new System.Drawing.Point(0, 468);
            this.pnlRight_Fill.Margin = new System.Windows.Forms.Padding(2);
            this.pnlRight_Fill.Name = "pnlRight_Fill";
            this.pnlRight_Fill.Size = new System.Drawing.Size(733, 164);
            this.pnlRight_Fill.TabIndex = 2;
            // 
            // tbShow
            // 
            this.tbShow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(234)))), ((int)(((byte)(250)))));
            this.tbShow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbShow.Location = new System.Drawing.Point(0, 30);
            this.tbShow.Multiline = true;
            this.tbShow.Name = "tbShow";
            this.tbShow.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbShow.Size = new System.Drawing.Size(733, 134);
            this.tbShow.TabIndex = 18;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(151)))), ((int)(((byte)(206)))), ((int)(((byte)(237)))));
            this.panel5.Controls.Add(this.label3);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(733, 30);
            this.panel5.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(12, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 21);
            this.label3.TabIndex = 0;
            this.label3.Text = "输出日志";
            // 
            // pnlRight_Middle
            // 
            this.pnlRight_Middle.Controls.Add(this.groupBox1);
            this.pnlRight_Middle.Controls.Add(this.pcPrintView);
            this.pnlRight_Middle.Controls.Add(this.panel4);
            this.pnlRight_Middle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlRight_Middle.Location = new System.Drawing.Point(0, 132);
            this.pnlRight_Middle.Margin = new System.Windows.Forms.Padding(2);
            this.pnlRight_Middle.Name = "pnlRight_Middle";
            this.pnlRight_Middle.Size = new System.Drawing.Size(733, 336);
            this.pnlRight_Middle.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbChangeType);
            this.groupBox1.Controls.Add(this.tbBankNo);
            this.groupBox1.Controls.Add(this.tbName);
            this.groupBox1.Controls.Add(this.txtIDNo);
            this.groupBox1.Controls.Add(this.cbCertificateType);
            this.groupBox1.Location = new System.Drawing.Point(494, 36);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(230, 287);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "人员信息";
            // 
            // cbChangeType
            // 
            this.cbChangeType.BackColor = System.Drawing.SystemColors.Window;
            this.cbChangeType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbChangeType.Font = new System.Drawing.Font("幼圆", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbChangeType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cbChangeType.FormattingEnabled = true;
            this.cbChangeType.Items.AddRange(new object[] {
            "补卡",
            "到期换卡",
            "银行变更换卡",
            "个人信息变更换卡",
            "更换错卡"});
            this.cbChangeType.Location = new System.Drawing.Point(27, 190);
            this.cbChangeType.Margin = new System.Windows.Forms.Padding(2);
            this.cbChangeType.Name = "cbChangeType";
            this.cbChangeType.Size = new System.Drawing.Size(178, 22);
            this.cbChangeType.TabIndex = 15;
            this.cbChangeType.SelectedIndexChanged += new System.EventHandler(this.cbChangeType_SelectedIndexChanged);
            // 
            // tbBankNo
            // 
            this.tbBankNo.BackColor = System.Drawing.SystemColors.Window;
            this.tbBankNo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbBankNo.Font = new System.Drawing.Font("幼圆", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbBankNo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tbBankNo.Location = new System.Drawing.Point(28, 241);
            this.tbBankNo.Name = "tbBankNo";
            this.tbBankNo.Size = new System.Drawing.Size(177, 21);
            this.tbBankNo.TabIndex = 14;
            this.tbBankNo.Text = "原银行卡号";
            // 
            // tbName
            // 
            this.tbName.BackColor = System.Drawing.SystemColors.Window;
            this.tbName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbName.Font = new System.Drawing.Font("幼圆", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbName.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tbName.Location = new System.Drawing.Point(28, 143);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(177, 21);
            this.tbName.TabIndex = 13;
            this.tbName.Text = "姓名";
            // 
            // txtIDNo
            // 
            this.txtIDNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtIDNo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtIDNo.Font = new System.Drawing.Font("幼圆", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtIDNo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.txtIDNo.Location = new System.Drawing.Point(28, 90);
            this.txtIDNo.Name = "txtIDNo";
            this.txtIDNo.Size = new System.Drawing.Size(177, 21);
            this.txtIDNo.TabIndex = 0;
            this.txtIDNo.Text = "身份证号";
            // 
            // cbCertificateType
            // 
            this.cbCertificateType.BackColor = System.Drawing.SystemColors.Window;
            this.cbCertificateType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCertificateType.Font = new System.Drawing.Font("幼圆", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cbCertificateType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cbCertificateType.FormattingEnabled = true;
            this.cbCertificateType.Items.AddRange(new object[] {
            "居民身份证",
            "港澳居民来往内地通行证",
            "台湾居民来往内地通行证",
            "外国人永久居留证",
            "护照"});
            this.cbCertificateType.Location = new System.Drawing.Point(28, 38);
            this.cbCertificateType.Margin = new System.Windows.Forms.Padding(2);
            this.cbCertificateType.Name = "cbCertificateType";
            this.cbCertificateType.Size = new System.Drawing.Size(178, 22);
            this.cbCertificateType.TabIndex = 12;
            this.cbCertificateType.SelectedIndexChanged += new System.EventHandler(this.cbCertificateType_SelectedIndexChanged);
            // 
            // pcPrintView
            // 
            this.pcPrintView.Image = global::Sporcard.Properties.Resources.社保正面;
            this.pcPrintView.Location = new System.Drawing.Point(0, 31);
            this.pcPrintView.Margin = new System.Windows.Forms.Padding(2);
            this.pcPrintView.Name = "pcPrintView";
            this.pcPrintView.Size = new System.Drawing.Size(480, 304);
            this.pcPrintView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pcPrintView.TabIndex = 7;
            this.pcPrintView.TabStop = false;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(151)))), ((int)(((byte)(206)))), ((int)(((byte)(237)))));
            this.panel4.Controls.Add(this.label2);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(733, 30);
            this.panel4.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(12, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 21);
            this.label2.TabIndex = 0;
            this.label2.Text = "用户信息";
            // 
            // pnlRight_Top
            // 
            this.pnlRight_Top.Controls.Add(this.ucFlowChart1);
            this.pnlRight_Top.Controls.Add(this.panel3);
            this.pnlRight_Top.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlRight_Top.Location = new System.Drawing.Point(0, 0);
            this.pnlRight_Top.Margin = new System.Windows.Forms.Padding(2);
            this.pnlRight_Top.Name = "pnlRight_Top";
            this.pnlRight_Top.Size = new System.Drawing.Size(733, 132);
            this.pnlRight_Top.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(151)))), ((int)(((byte)(206)))), ((int)(((byte)(237)))));
            this.panel3.Controls.Add(this.lblFlow);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(733, 30);
            this.panel3.TabIndex = 5;
            // 
            // lblFlow
            // 
            this.lblFlow.AutoSize = true;
            this.lblFlow.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblFlow.Location = new System.Drawing.Point(12, 4);
            this.lblFlow.Name = "lblFlow";
            this.lblFlow.Size = new System.Drawing.Size(94, 21);
            this.lblFlow.TabIndex = 0;
            this.lblFlow.Text = "生产流程";
            // 
            // pnlTop
            // 
            this.pnlTop.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pnlTop.BackgroundImage")));
            this.pnlTop.Controls.Add(this.ucFuncButton1);
            this.pnlTop.Controls.Add(this.panel1);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Margin = new System.Windows.Forms.Padding(2);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(978, 67);
            this.pnlTop.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
            this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panel1.Location = new System.Drawing.Point(708, 11);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(266, 46);
            this.panel1.TabIndex = 8;
            // 
            // ucFlowChart1
            // 
            this.ucFlowChart1.LISTSTEP = new string[] {
        "进卡",
        "写IC",
        "打印",
        "回盘"};
            this.ucFlowChart1.Location = new System.Drawing.Point(0, 30);
            this.ucFlowChart1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ucFlowChart1.Name = "ucFlowChart1";
            this.ucFlowChart1.Size = new System.Drawing.Size(975, 96);
            this.ucFlowChart1.TabIndex = 6;
            // 
            // ucFuncButton1
            // 
            this.ucFuncButton1.BackColor = System.Drawing.Color.Transparent;
            this.ucFuncButton1.Location = new System.Drawing.Point(243, 0);
            this.ucFuncButton1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ucFuncButton1.Name = "ucFuncButton1";
            this.ucFuncButton1.Size = new System.Drawing.Size(166, 67);
            this.ucFuncButton1.TabIndex = 9;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(978, 729);
            this.Controls.Add(this.pnlRight);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.pnlLeft);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlTop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "云南即时补换卡程序";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.pnlBottom.ResumeLayout(false);
            this.pnlBottom.PerformLayout();
            this.pnlLeft.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.pnlRight.ResumeLayout(false);
            this.pnlRight_Fill.ResumeLayout(false);
            this.pnlRight_Fill.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.pnlRight_Middle.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pcPrintView)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.pnlRight_Top.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.pnlTop.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.Panel panel1;
        private UCFuncButton ucFuncButton1;
        private System.Windows.Forms.TextBox txtIDNo;
        private System.Windows.Forms.ComboBox cbCertificateType;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TreeView tvCardInit;
        private System.Windows.Forms.Panel pnlRight_Top;
        private UCFlowChart ucFlowChart1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label lblFlow;
        private System.Windows.Forms.Panel pnlRight_Middle;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pcPrintView;
        private System.Windows.Forms.Panel pnlRight_Fill;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbShow;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tbBankNo;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.ComboBox cbChangeType;

    }
}

