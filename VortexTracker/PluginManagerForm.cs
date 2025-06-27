using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using LibVT;

namespace VortexTracker
{
    /// <summary>
    /// Summary description for PluginManagerForm.
    /// </summary>
    public partial class PluginManagerForm : Form
    {
        private bool _suppressItemChecked;

        public PluginManagerForm(Form parent)
        {
            Owner = parent;
            InitializeComponent();

            PluginsListView.ItemChecked += PluginsListView_ItemChecked;
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (!Visible) return;

            CenterForm();
            PopulateListView();
        }

        private void CenterForm()
        {
            var parent = Owner;
            Location = new Point(
                parent.Left + (parent.Width - Width) / 2,
                parent.Top + (parent.Height - Height) / 2);
        }

        private void PopulateListView()
        {
            PluginsListView.BeginUpdate();

            try
            {
                PluginsListView.Items.Clear();

                // Remember column width ratios (first time only)
                for (int i = 0; i < PluginsListView.Columns.Count; i++)
                {
                    if (PluginsListView.Columns[i].Tag == null)
                    {
                        PluginsListView.Columns[i].Tag =
                            (float)PluginsListView.Columns[i].Width / PluginsListView.Width;
                    }
                }

                foreach (var wrapper in PluginManager.PluginWrappers)
                {
                    string name = wrapper.Name;
                    string version, author, descr;

                    if (wrapper.Instance != null)
                    {
                        // already loaded – we can ask the live object
                        version = wrapper.Instance.Version;
                        author = wrapper.Instance.Author;
                        descr = wrapper.Instance.Description;
                    }
                    else
                    {
                        // still unloaded – read the file version resource
                        (version, author, descr) = PluginManager.ReadVersionResource(wrapper.Path);
                    }

                    var lvi = new ListViewItem(new[] { name, version, author, descr })
                    {
                        Tag = wrapper,
                        Checked = wrapper.IsEnabled
                    };
                    PluginsListView.Items.Add(lvi);
                }
            }
            finally
            {
                PluginsListView.EndUpdate();
            }
        }

        private void PluginsListView_Resize(object sender, EventArgs e)
        {
            foreach (ColumnHeader col in PluginsListView.Columns)
            {
                if (col.Tag is float ratio)
                    col.Width = (int)(PluginsListView.Width * ratio);
            }
        }

        private void PluginsListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (_suppressItemChecked)
                return;

            if (e.Item.Tag is not PluginWrapper wrapper)
                return;

            bool wantEnabled = e.Item.Checked;

            if (wantEnabled == wrapper.IsEnabled)
                return;

            if (wantEnabled)
            {
                wrapper.IsEnabled = true;

                if (!PluginManager.TryActivatePlugin(wrapper, Globals.MainForm.PluginHost))
                {
                    wrapper.IsEnabled = false;

                    _suppressItemChecked = true;
                    e.Item.Checked = false;
                    _suppressItemChecked = false;

                    MessageBox.Show(this, $"Failed to Load Plugin \"{wrapper.Name}\" \n{wrapper.LastError}.", "Plugin Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                PluginManager.UnloadPlugin(wrapper.Name);
                wrapper.IsEnabled = false;
            }

            WriteIni();
        }

        private void PluginManagerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;         // hide instead of destroy

            WriteIni();
            Owner?.Activate();
            Hide();
        }

        private static void WriteIni()
        {
            var ini = new IniFile(MainForm.ConfigFilePath);

            // wipe existing plug-in sections
            for (int i = 0; i < PluginManager.MaxPlugins; i++)
            {
                string section = $"Plugin{i + 1}";
                ini.SetValue(section, "Name", null);
                ini.SetValue(section, "Enabled", null);
            }

            // write current state in the UI order
            for (int i = 0; i < PluginManager.PluginWrappers.Count; i++)
            {
                var w = PluginManager.PluginWrappers[i];
                string section = $"Plugin{i + 1}";
                ini.SetValue(section, "Name", w.Name);
                ini.SetValue(section, "Enabled", w.IsEnabled);
            }

            ini.SaveToFile();
        }

        private void PluginManagerForm_Resize(object sender, System.EventArgs e)
        {
            for (int i = 0; i < PluginsListView.Columns.Count; i++)
            {
                if (PluginsListView.Columns[i].Width == 0)
                    continue;

                float columnRatio = (float)PluginsListView.Columns[i].Tag;
                PluginsListView.Columns[i].Width = (int)(PluginsListView.Width * columnRatio);
            }
        }

        private void PluginsListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (PluginsListView.SelectedItems.Count == 0) return;

            var wrapper = (PluginWrapper)PluginsListView.SelectedItems[0].Tag;
            if (wrapper.Instance == null)
            {
                MessageBox.Show("The Plugin Must be Enabled Before it Can be Configured.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            wrapper.Instance.ShowMainForm(Globals.MainForm);
        }
    }
}
