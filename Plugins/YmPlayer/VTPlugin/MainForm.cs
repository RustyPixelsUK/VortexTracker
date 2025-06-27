using LibVT;
using LibVT.Plugins;
using System;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace VTPlugin
{
    public partial class MainForm : Form
    {
        private Plugin _plugin = null;
        private readonly IHost _host;
        private IniFile _iniFile;

        // --- UI choices --------------------------------------------------------------
        private static readonly string[] BaudRateNames =
        {
            "110","300","600","1200","2400","4800","9600","14400","19200","28800",
            "38400","56000","57600","115200","128000","153600","230400","256000",
            "460800","921600", "1000000", "1500000", "2000000"
        };

        private static readonly string[] DataBitNames = { "5", "6", "7", "8" };
        private static readonly string[] ParityNames = { "N", "O", "E", "M", "S" };
        private static readonly string[] StopBitNames = { "0", "1", "1.5", "2" };

        // --- ctor --------------------------------------------------------------------
        public MainForm(Plugin plugin, IHost host, IniFile iniFile)
        {
            _plugin = plugin;
            _host = host;
            _iniFile = iniFile;

            InitializeComponent();

            PortBox.Items.AddRange(_plugin.GetPortList());
            BaudRateBox.Items.AddRange(BaudRateNames);
            DataBitsBox.Items.AddRange(DataBitNames);
            ParityBox.Items.AddRange(ParityNames);
            StopBitsBox.Items.AddRange(StopBitNames);

            ReadConfig();

            PortBox.SelectedIndexChanged += PortBox_SelectedIndexChanged;
            BaudRateBox.SelectedIndexChanged += BaudRateBox_SelectedIndexChanged;
            DataBitsBox.SelectedIndexChanged += DataBitsBox_SelectedIndexChanged;
            ParityBox.SelectedIndexChanged += ParityBox_SelectedIndexChanged;
            StopBitsBox.SelectedIndexChanged += StopBitsBox_SelectedIndexChanged;            
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible)
                CenterForm();
        }

        private void CenterForm()
        {
            if (Owner == null) return;
            int x = Owner.Left + (Owner.Width - Width) / 2;
            int y = Owner.Top + (Owner.Height - Height) / 2;
            Location = new Point(x, y);
        }

        private void ReadConfig()
        {
            string port = _iniFile.GetValue("Serial", "Port", "COM1");

            if (PortBox.Items.Contains(port))
                PortBox.SelectedItem = port;
            else
                PortBox.SelectedIndex = 0;

            BaudRateBox.SelectedItem = _iniFile.GetValue("Serial", "BaudRate", "2000000");
            DataBitsBox.SelectedItem = _iniFile.GetValue("Serial", "DataBits", "8");
            ParityBox.SelectedIndex = (int)_iniFile.GetValue<Parity>("Serial", "Parity", Parity.None);
            StopBitsBox.SelectedIndex = (int)_iniFile.GetValue<StopBits>("Serial", "StopBits", StopBits.One);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;

            Owner?.Activate();

            this.Hide();
        }

        private void PortBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _iniFile.SetValue("Serial", "Port", PortBox.SelectedItem?.ToString());
            _iniFile.Save();
        }

        private void BaudRateBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _iniFile.SetValue("Serial", "BaudRate", BaudRateBox.SelectedItem?.ToString());
            _iniFile.Save();
        }

        private void DataBitsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _iniFile.SetValue("Serial", "DataBits", DataBitsBox.SelectedItem?.ToString());
            _iniFile.Save();
        }

        private void ParityBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _iniFile.SetValue<Parity>("Serial", "Parity", (Parity)ParityBox.SelectedIndex);
            _iniFile.Save();
        }

        private void StopBitsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _iniFile.SetValue<StopBits>("Serial", "StopBits", (StopBits)StopBitsBox.SelectedIndex);
            _iniFile.Save();
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            _plugin.TryOpenSerialPort();

            this.Hide();
        }
    }
}