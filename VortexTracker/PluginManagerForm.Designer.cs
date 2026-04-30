using VortexTracker.Controls;

namespace VortexTracker
{
	partial class PluginManagerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PluginManagerForm));
            PluginsListView = new MyListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            columnHeader4 = new ColumnHeader();
            statusStrip1 = new StatusStrip();
            InfoLabel = new ToolStripStatusLabel();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // PluginsListView
            // 
            PluginsListView.Activation = ItemActivation.OneClick;
            PluginsListView.AllowColumnReorder = true;
            PluginsListView.AutoArrange = false;
            PluginsListView.CheckBoxes = true;
            PluginsListView.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3, columnHeader4 });
            PluginsListView.Dock = DockStyle.Fill;
            PluginsListView.FullRowSelect = true;
            PluginsListView.GridLines = true;
            PluginsListView.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            PluginsListView.LabelWrap = false;
            PluginsListView.Location = new Point(0, 0);
            PluginsListView.MultiSelect = false;
            PluginsListView.Name = "PluginsListView";
            PluginsListView.Size = new Size(544, 336);
            PluginsListView.TabIndex = 1;
            PluginsListView.UseCompatibleStateImageBehavior = false;
            PluginsListView.View = View.Details;
            PluginsListView.MouseDoubleClick += PluginsListView_MouseDoubleClick;
            PluginsListView.Resize += PluginsListView_Resize;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Name";
            columnHeader1.Width = 134;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Version";
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Author";
            columnHeader3.Width = 88;
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "Description";
            columnHeader4.Width = 247;
            // 
            // statusStrip1
            // 
            statusStrip1.GripStyle = ToolStripGripStyle.Visible;
            statusStrip1.Items.AddRange(new ToolStripItem[] { InfoLabel });
            statusStrip1.Location = new Point(0, 336);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(544, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // InfoLabel
            // 
            InfoLabel.Name = "InfoLabel";
            InfoLabel.Size = new Size(192, 17);
            InfoLabel.Text = "Double-Click a Plugin to Configure";
            // 
            // PluginManagerForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(544, 358);
            Controls.Add(PluginsListView);
            Controls.Add(statusStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "PluginManagerForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Plugin Manager";
            FormClosing += PluginManagerForm_FormClosing;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private MyListView PluginsListView;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel InfoLabel;
    }
}

