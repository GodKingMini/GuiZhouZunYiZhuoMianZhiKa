using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sporcard;


namespace Sporcard
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //FrmLogin frmlogin = new FrmLogin();
            //if (DialogResult.OK == frmlogin.ShowDialog())
            {
                Application.Run(new FormMain());
            }   
        }
    }
}
