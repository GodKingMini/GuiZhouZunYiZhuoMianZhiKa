using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;



namespace Sporcard
{
    public partial class FrmLogin : Form
    {
        List<string> loginInfo = new List<string>();
        public string username = "";
        public string password = "";
        public FrmLogin()
        {
            InitializeComponent();

            this.Text = "登入";
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (tbPws.Text.Equals("")||tbUser.Text.Equals(""))
            {
                labelInfo.Text = "输入不能为空！";
                return;
            }
            string username = tbUser.Text;
            string password = tbPws.Text;
            string statusCode = "";
            string message = "";
            PlatFormInterface pfi = PlatFormInterface.getInstance();
            loginInfo = pfi.checkLogin(username, password, out statusCode, out message);
            if (statusCode == "200" && message == "登录成功")
            {
                
                GlobalClass.userId = loginInfo[0];
                GlobalClass.orgId = loginInfo[5];
                GlobalClass.loginName = loginInfo[1];//username;
                GlobalClass.distCode = loginInfo[16];
                GlobalClass.yhbm = loginInfo[17];
                this.DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                labelInfo.Text = message;
            }    
          
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
