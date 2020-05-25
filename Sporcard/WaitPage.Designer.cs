namespace Sporcard
{
    partial class WaitPage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.waitControlPage1 = new Sporcard.WaitControlPage();
            this.SuspendLayout();
            // 
            // waitControlPage1
            // 
            this.waitControlPage1.InnerCircleRadius = 25;
            this.waitControlPage1.LineWidth = 10;
            this.waitControlPage1.Location = new System.Drawing.Point(-2, 1);
            this.waitControlPage1.Name = "waitControlPage1";
            this.waitControlPage1.OutnerCircleRadius = 50;
            this.waitControlPage1.Size = new System.Drawing.Size(285, 262);
            this.waitControlPage1.Speed = 200;
            this.waitControlPage1.SpokesMember = 12;
            this.waitControlPage1.TabIndex = 0;
            this.waitControlPage1.ThemeColor = System.Drawing.Color.Blue;
            // 
            // WaitPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.waitControlPage1);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "WaitPage";
            this.Text = "WaitPage";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        public WaitControlPage waitControlPage1;
    }
}