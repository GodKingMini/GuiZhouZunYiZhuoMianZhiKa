using System;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Sporcard
{
    /// <summary>   
    /// Summary description for cilpButton.   
    /// </summary>   
    public class RoundButton : Button
    {
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, this.Width, this.Height);
            this.Region = new Region(path);      
        }
     
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {         
            Pen pen = new Pen(this.BackColor);
            Graphics g = this.CreateGraphics();
            g.Clear(Color.Goldenrod);
            g.FillEllipse(Brushes.DarkKhaki, new Rectangle(0, 0, this.Width, this.Height));
        }

        //protected override void OnMouseDown(MouseEventArgs e)
        //{
        //    //OnPaintBackground(null);          
        //    this.BackgroundImage = MyImage.GetFile(Application.StartupPath + "\\photo\\图片\\公用\\" + string.Format("1_btnmainpage_点击.png"));
        //    this.BackgroundImageLayout = ImageLayout.Stretch;
        //}
    }
}

