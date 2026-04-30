using System;
using System.Windows.Forms;

namespace VortexTracker
{
    partial class TurboSoundForm
    {
        private System.ComponentModel.IContainer components;

        // Clean up any resources being used.
        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            toolTip1 = new ToolTip(components);
            ListBox1 = new ListBox();
            SuspendLayout();
            // 
            // ListBox1
            // 
            ListBox1.Dock = DockStyle.Fill;
            ListBox1.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold);
            ListBox1.Items.AddRange(new object[] { "Test", "3231", "3424", "12343214" });
            ListBox1.Location = new Point(0, 0);
            ListBox1.Name = "ListBox1";
            ListBox1.Size = new Size(380, 246);
            ListBox1.TabIndex = 0;
            ListBox1.MouseDoubleClick += ListBox1_MouseDoubleClick;
            // 
            // TurboSoundForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(380, 246);
            Controls.Add(ListBox1);
            Font = new Font("Microsoft Sans Serif", 8.25F);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Location = new Point(443, 290);
            Name = "TurboSoundForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Select Module for Turbo Sound Mode";
            ResumeLayout(false);

        }
        #endregion

        public ListBox ListBox1;
        private ToolTip toolTip1 = null;
    }
}
