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
    public partial class UCFlowChart : UserControl
    {
        public UCFlowChart()
        {
            InitializeComponent();
        }

        private String[] listStep = new String[8];
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public String[] LISTSTEP
        {
            get
            {
                return this.listStep;
            }
            set
            {
                this.listStep = value;
                ShowSteps();
            }
        }

        //设置步骤显示
        private void ShowSteps()
        {
            PictureBox temppb = null;
            Label templbl = null;
            Label temptag = null;
            for (int i = 1; i <= listStep.Length; i++)
            {
                temptag = (Label)this.Controls.Find("lblStepTag" + i.ToString(), true)[0];
                templbl = (Label)this.Controls.Find("lblStep" + i.ToString(), true)[0];
                temppb = i == 1 ? null : (PictureBox)this.Controls.Find("pbArrow" + i.ToString(), true)[0];
                if (temptag != null)
                {
                    temptag.Visible = true;
                }
                if (templbl != null)
                {
                    templbl.Visible = true;
                    templbl.Text = listStep[i - 1];
                }
                if (temppb != null)
                {
                    temppb.Visible = true;
                }
            }
        }

        public void Clear()
        {
            Label temptag = null;
            for (int i = 1; i <= 8; i++)
            {
                temptag = (Label)this.Controls.Find("lblStepTag" + i.ToString(), true)[0];
                if (temptag != null)
                {
                    temptag.Image = Properties.Resources.白圆;
                }
            }
        }

        public bool SetStepStatus(int stepIndex, bool status)
        {
            if (stepIndex < 0 || stepIndex > listStep.Length)
            {
                return false;
            }
            Label temptag = (Label)this.Controls.Find("lblStepTag" + stepIndex.ToString(), true)[0];
            temptag.Image = status ? Properties.Resources.绿圆 : Properties.Resources.红圆;
            return true;
        }
    }
}
