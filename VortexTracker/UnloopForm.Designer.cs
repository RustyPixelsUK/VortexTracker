using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VortexTracker
{
    public partial class UnloopForm
    {
        private System.ComponentModel.Container components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);

        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            InfoLabel = new Label();
            Label2 = new Label();
            UnloopUpDown = new NumericUpDown();
            CalcSlides = new CheckBox();
            OkButton = new Button();
            CancelButton = new Button();
            ((System.ComponentModel.ISupportInitialize)UnloopUpDown).BeginInit();
            SuspendLayout();
            // 
            // InfoLabel
            // 
            InfoLabel.Location = new Point(16, 16);
            InfoLabel.Name = "InfoLabel";
            InfoLabel.Size = new Size(181, 26);
            InfoLabel.TabIndex = 0;
            InfoLabel.Text = "Enter Unloop Count.\r\n0 - for Unloop Till the End of Sample.";
            // 
            // Label2
            // 
            Label2.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);
            Label2.Location = new Point(12, 67);
            Label2.Name = "Label2";
            Label2.Size = new Size(51, 13);
            Label2.TabIndex = 1;
            Label2.Text = "Count:";
            // 
            // UnloopUpDown
            // 
            UnloopUpDown.Location = new Point(60, 64);
            UnloopUpDown.Name = "UnloopUpDown";
            UnloopUpDown.Size = new Size(46, 20);
            UnloopUpDown.TabIndex = 1;
            // 
            // CalcSlides
            // 
            CalcSlides.Location = new Point(16, 96);
            CalcSlides.Name = "CalcSlides";
            CalcSlides.Size = new Size(115, 21);
            CalcSlides.TabIndex = 2;
            CalcSlides.Text = "Calculate Slides";
            // 
            // OkButton
            // 
            OkButton.Location = new Point(124, 136);
            OkButton.Name = "OkButton";
            OkButton.Size = new Size(73, 25);
            OkButton.TabIndex = 4;
            OkButton.Text = "OK";
            OkButton.Click += OkButton_Click;
            // 
            // CancelButton
            // 
            CancelButton.Location = new Point(12, 136);
            CancelButton.Name = "CancelButton";
            CancelButton.Size = new Size(73, 25);
            CancelButton.TabIndex = 3;
            CancelButton.Text = "Cancel";
            CancelButton.Click += CancelButton_Click;
            // 
            // UnloopForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(209, 176);
            Controls.Add(UnloopUpDown);
            Controls.Add(InfoLabel);
            Controls.Add(Label2);
            Controls.Add(CalcSlides);
            Controls.Add(OkButton);
            Controls.Add(CancelButton);
            Font = new Font("Microsoft Sans Serif", 8.25F);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Location = new Point(838, 300);
            Name = "UnloopForm";
            ShowInTaskbar = false;
            FormClosing += UnloopForm_FormClosing;
            ((System.ComponentModel.ISupportInitialize)UnloopUpDown).EndInit();
            ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label InfoLabel;
        public System.Windows.Forms.Label Label2;
        public System.Windows.Forms.NumericUpDown UnloopUpDown;
        public System.Windows.Forms.CheckBox CalcSlides;
        public System.Windows.Forms.Button OkButton;
        public System.Windows.Forms.Button CancelButton;
    }
}
