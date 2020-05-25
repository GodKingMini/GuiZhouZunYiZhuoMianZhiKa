using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sporcard
{
    public partial class UCFuncButton : UserControl
    {
        public UCFuncButton()
        {
            InitializeComponent();
        }

        private List<Button> listBut = new List<Button>();
        public void AddButton(Image btnImage,EventHandler evnhd)
        {
            Button tempbtn = null;
            try
            {
                tempbtn = (Button)this.Controls.Find("BTN" + (listBut.Count + 1).ToString(), true)[0];
                tempbtn.Image = (btnImage==null) ? tempbtn.Image:btnImage;
                tempbtn.Click += new System.EventHandler(evnhd);
                tempbtn.Visible = true;
                listBut.Add(tempbtn);
                this.Width = (tempbtn.Width) * listBut.Count;
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public void SetButtonEnabled(int btnIndex, bool value)
        {
            Button tempbtn = null;
            try
            {
                tempbtn = (Button)this.Controls.Find("BTN" + btnIndex.ToString(), true)[0];
                tempbtn.Enabled = true;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
