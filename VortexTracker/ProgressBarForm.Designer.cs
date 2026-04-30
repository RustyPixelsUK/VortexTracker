using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace VortexTracker
{
    partial class ProgressBarForm
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
            ExportProgress = new ProgressBar();
            ExportActions = new Panel();
            StopExport = new Panel();
            ExportActions.SuspendLayout();
            SuspendLayout();
            // 
            // ExportProgress
            // 
            ExportProgress.Location = new Point(16, 16);
            ExportProgress.Name = "ExportProgress";
            ExportProgress.Size = new Size(360, 33);
            ExportProgress.Step = 1;
            ExportProgress.TabIndex = 0;
            ExportProgress.Value = 100;
            // 
            // ExportActions
            // 
            ExportActions.Controls.Add(StopExport);
            ExportActions.Location = new Point(0, 0);
            ExportActions.Name = "ExportActions";
            ExportActions.Size = new Size(200, 100);
            ExportActions.TabIndex = 0;
            // 
            // StopExport
            // 
            StopExport.Location = new Point(0, 0);
            StopExport.Name = "StopExport";
            StopExport.Size = new Size(200, 100);
            StopExport.TabIndex = 0;
            // 
            // ProgressBarForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(393, 63);
            Controls.Add(ExportProgress);
            Font = new Font("Microsoft Sans Serif", 8.25F);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            KeyPreview = true;
            Location = new Point(421, 165);
            Name = "ProgressBarForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            ExportActions.ResumeLayout(false);
            ResumeLayout(false);

        }
        #endregion

        public System.Windows.Forms.ProgressBar ExportProgress;
        public System.Windows.Forms.Panel ExportActions;
        public System.Windows.Forms.Panel StopExport;
        private System.Windows.Forms.ToolTip toolTip1 = null;
    }
}
