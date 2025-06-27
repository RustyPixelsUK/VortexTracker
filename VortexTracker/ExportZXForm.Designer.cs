using VortexTracker.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace VortexTracker
{
    partial class ExportZXForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportZXForm));
            toolTip1 = new ToolTip(components);
            OkButton = new Button();
            CancelButton = new Button();
            ParametersGroup = new GroupBox();
            CompilationAddressValue = new Label();
            DecLabel = new Label();
            HexLabel = new Label();
            CompilationAddressLabel = new Label();
            DecAddressBox = new TextBox();
            HexAddressBox = new TextBox();
            LoopChk = new CheckBox();
            FormatGroup = new RadioGroup();
            HintsGroup = new GroupBox();
            CurrentPosValue = new Label();
            VariablesLengthValue = new Label();
            Module2LengthValue = new Label();
            Module2AddressValue = new Label();
            ModuleLengthValue = new Label();
            ModuleAddressValue = new Label();
            PlayerCodesLengthValue = new Label();
            VariablesAddressValue = new Label();
            SetupByteValue = new Label();
            MuteCallValue = new Label();
            PlayCallValue = new Label();
            InitCallValue = new Label();
            InitCallLabel = new Label();
            PlayCallLabel = new Label();
            MuteCallLabel = new Label();
            SetupByteLabel = new Label();
            Bit7Label = new Label();
            Bit0Label = new Label();
            CurrentPosLabel = new Label();
            VariablesAddressLabel = new Label();
            VariablesLengthLabel = new Label();
            PlayerCodesLengthLabel = new Label();
            ModuleAddressLabel = new Label();
            ModuleLengthLabel = new Label();
            Module2LengthLabel = new Label();
            Module2AddressLabel = new Label();
            SourceBox = new TextBox();
            ParametersGroup.SuspendLayout();
            HintsGroup.SuspendLayout();
            SuspendLayout();
            // 
            // OkButton
            // 
            OkButton.Location = new Point(144, 288);
            OkButton.Name = "OkButton";
            OkButton.Size = new Size(75, 25);
            OkButton.TabIndex = 1;
            OkButton.Text = "OK";
            OkButton.Click += OkButton_Click;
            // 
            // CancelButton
            // 
            CancelButton.Location = new Point(280, 288);
            CancelButton.Name = "CancelButton";
            CancelButton.Size = new Size(75, 25);
            CancelButton.TabIndex = 2;
            CancelButton.Text = "Cancel";
            CancelButton.Click += CancelButton_Click;
            // 
            // ParametersGroup
            // 
            ParametersGroup.Controls.Add(CompilationAddressValue);
            ParametersGroup.Controls.Add(DecLabel);
            ParametersGroup.Controls.Add(HexLabel);
            ParametersGroup.Controls.Add(CompilationAddressLabel);
            ParametersGroup.Controls.Add(DecAddressBox);
            ParametersGroup.Controls.Add(HexAddressBox);
            ParametersGroup.Controls.Add(LoopChk);
            ParametersGroup.Location = new Point(8, 8);
            ParametersGroup.Name = "ParametersGroup";
            ParametersGroup.Size = new Size(225, 81);
            ParametersGroup.TabIndex = 0;
            ParametersGroup.TabStop = false;
            ParametersGroup.Text = "Parameters";
            // 
            // CompilationAddressValue
            // 
            CompilationAddressValue.AutoSize = true;
            CompilationAddressValue.Location = new Point(152, 16);
            CompilationAddressValue.Name = "CompilationAddressValue";
            CompilationAddressValue.Size = new Size(0, 13);
            CompilationAddressValue.TabIndex = 3;
            // 
            // DecLabel
            // 
            DecLabel.Location = new Point(113, 35);
            DecLabel.Name = "DecLabel";
            DecLabel.Size = new Size(29, 13);
            DecLabel.TabIndex = 0;
            DecLabel.Text = "DEC";
            // 
            // HexLabel
            // 
            HexLabel.Location = new Point(6, 35);
            HexLabel.Name = "HexLabel";
            HexLabel.Size = new Size(32, 13);
            HexLabel.TabIndex = 1;
            HexLabel.Text = "HEX";
            // 
            // CompilationAddressLabel
            // 
            CompilationAddressLabel.Location = new Point(16, 16);
            CompilationAddressLabel.Name = "CompilationAddressLabel";
            CompilationAddressLabel.Size = new Size(131, 13);
            CompilationAddressLabel.TabIndex = 2;
            CompilationAddressLabel.Text = "Compilation Address (Max)";
            // 
            // DecAddressBox
            // 
            DecAddressBox.Location = new Point(144, 32);
            DecAddressBox.Name = "DecAddressBox";
            DecAddressBox.Size = new Size(57, 20);
            DecAddressBox.TabIndex = 1;
            DecAddressBox.TextChanged += DecAddressBox_TextChanged;
            // 
            // HexAddressBox
            // 
            HexAddressBox.Location = new Point(40, 32);
            HexAddressBox.Name = "HexAddressBox";
            HexAddressBox.Size = new Size(57, 20);
            HexAddressBox.TabIndex = 0;
            HexAddressBox.TextChanged += HexAddressBox_TextChanged;
            // 
            // LoopChk
            // 
            LoopChk.Location = new Point(16, 56);
            LoopChk.Name = "LoopChk";
            LoopChk.Size = new Size(97, 17);
            LoopChk.TabIndex = 2;
            LoopChk.Text = "Disable Loop";
            // 
            // FormatGroup
            // 
            FormatGroup.Items.Add("Hobeta with Player");
            FormatGroup.Items.Add("Hobeta without Player");
            FormatGroup.Items.Add(".AY-File");
            FormatGroup.Items.Add(".SCL-File (Player and Module Separately)");
            FormatGroup.Items.Add(".TAP-File (Player and Module Separately)");
            FormatGroup.Location = new Point(8, 96);
            FormatGroup.Name = "FormatGroup";
            FormatGroup.Size = new Size(225, 186);
            FormatGroup.TabIndex = 3;
            FormatGroup.TabStop = false;
            FormatGroup.Text = "Format";
            FormatGroup.Click += FormatGroup_Click;
            // 
            // HintsGroup
            // 
            HintsGroup.Controls.Add(CurrentPosValue);
            HintsGroup.Controls.Add(VariablesLengthValue);
            HintsGroup.Controls.Add(Module2LengthValue);
            HintsGroup.Controls.Add(Module2AddressValue);
            HintsGroup.Controls.Add(ModuleLengthValue);
            HintsGroup.Controls.Add(ModuleAddressValue);
            HintsGroup.Controls.Add(PlayerCodesLengthValue);
            HintsGroup.Controls.Add(VariablesAddressValue);
            HintsGroup.Controls.Add(SetupByteValue);
            HintsGroup.Controls.Add(MuteCallValue);
            HintsGroup.Controls.Add(PlayCallValue);
            HintsGroup.Controls.Add(InitCallValue);
            HintsGroup.Controls.Add(InitCallLabel);
            HintsGroup.Controls.Add(PlayCallLabel);
            HintsGroup.Controls.Add(MuteCallLabel);
            HintsGroup.Controls.Add(SetupByteLabel);
            HintsGroup.Controls.Add(Bit7Label);
            HintsGroup.Controls.Add(Bit0Label);
            HintsGroup.Controls.Add(CurrentPosLabel);
            HintsGroup.Controls.Add(VariablesAddressLabel);
            HintsGroup.Controls.Add(VariablesLengthLabel);
            HintsGroup.Controls.Add(PlayerCodesLengthLabel);
            HintsGroup.Controls.Add(ModuleAddressLabel);
            HintsGroup.Controls.Add(ModuleLengthLabel);
            HintsGroup.Controls.Add(Module2LengthLabel);
            HintsGroup.Controls.Add(Module2AddressLabel);
            HintsGroup.Controls.Add(SourceBox);
            HintsGroup.Location = new Point(239, 9);
            HintsGroup.Name = "HintsGroup";
            HintsGroup.Size = new Size(265, 273);
            HintsGroup.TabIndex = 4;
            HintsGroup.TabStop = false;
            HintsGroup.Text = "Hints";
            // 
            // CurrentPosValue
            // 
            CurrentPosValue.AutoSize = true;
            CurrentPosValue.Location = new Point(174, 96);
            CurrentPosValue.Name = "CurrentPosValue";
            CurrentPosValue.Size = new Size(0, 13);
            CurrentPosValue.TabIndex = 11;
            // 
            // VariablesLengthValue
            // 
            VariablesLengthValue.AutoSize = true;
            VariablesLengthValue.Location = new Point(200, 128);
            VariablesLengthValue.Name = "VariablesLengthValue";
            VariablesLengthValue.Size = new Size(0, 13);
            VariablesLengthValue.TabIndex = 15;
            // 
            // Module2LengthValue
            // 
            Module2LengthValue.AutoSize = true;
            Module2LengthValue.Location = new Point(200, 159);
            Module2LengthValue.Name = "Module2LengthValue";
            Module2LengthValue.Size = new Size(0, 13);
            Module2LengthValue.TabIndex = 25;
            // 
            // Module2AddressValue
            // 
            Module2AddressValue.AutoSize = true;
            Module2AddressValue.Location = new Point(111, 159);
            Module2AddressValue.Name = "Module2AddressValue";
            Module2AddressValue.Size = new Size(0, 13);
            Module2AddressValue.TabIndex = 23;
            // 
            // ModuleLengthValue
            // 
            ModuleLengthValue.AutoSize = true;
            ModuleLengthValue.Location = new Point(200, 144);
            ModuleLengthValue.Name = "ModuleLengthValue";
            ModuleLengthValue.Size = new Size(0, 13);
            ModuleLengthValue.TabIndex = 21;
            // 
            // ModuleAddressValue
            // 
            ModuleAddressValue.AutoSize = true;
            ModuleAddressValue.Location = new Point(104, 144);
            ModuleAddressValue.Name = "ModuleAddressValue";
            ModuleAddressValue.Size = new Size(0, 13);
            ModuleAddressValue.TabIndex = 19;
            // 
            // PlayerCodesLengthValue
            // 
            PlayerCodesLengthValue.AutoSize = true;
            PlayerCodesLengthValue.Location = new Point(127, 112);
            PlayerCodesLengthValue.Name = "PlayerCodesLengthValue";
            PlayerCodesLengthValue.Size = new Size(0, 13);
            PlayerCodesLengthValue.TabIndex = 17;
            // 
            // VariablesAddressValue
            // 
            VariablesAddressValue.AutoSize = true;
            VariablesAddressValue.Location = new Point(113, 128);
            VariablesAddressValue.Name = "VariablesAddressValue";
            VariablesAddressValue.Size = new Size(0, 13);
            VariablesAddressValue.TabIndex = 13;
            // 
            // SetupByteValue
            // 
            SetupByteValue.AutoSize = true;
            SetupByteValue.Location = new Point(81, 64);
            SetupByteValue.Name = "SetupByteValue";
            SetupByteValue.Size = new Size(0, 13);
            SetupByteValue.TabIndex = 7;
            // 
            // MuteCallValue
            // 
            MuteCallValue.AutoSize = true;
            MuteCallValue.Location = new Point(90, 48);
            MuteCallValue.Name = "MuteCallValue";
            MuteCallValue.Size = new Size(0, 13);
            MuteCallValue.TabIndex = 5;
            // 
            // PlayCallValue
            // 
            PlayCallValue.AutoSize = true;
            PlayCallValue.Location = new Point(85, 32);
            PlayCallValue.Name = "PlayCallValue";
            PlayCallValue.Size = new Size(0, 13);
            PlayCallValue.TabIndex = 3;
            // 
            // InitCallValue
            // 
            InitCallValue.AutoSize = true;
            InitCallValue.Location = new Point(80, 16);
            InitCallValue.Name = "InitCallValue";
            InitCallValue.Size = new Size(0, 13);
            InitCallValue.TabIndex = 1;
            // 
            // InitCallLabel
            // 
            InitCallLabel.Location = new Point(16, 16);
            InitCallLabel.Name = "InitCallLabel";
            InitCallLabel.Size = new Size(63, 13);
            InitCallLabel.TabIndex = 0;
            InitCallLabel.Text = "INIT: CALL";
            // 
            // PlayCallLabel
            // 
            PlayCallLabel.Location = new Point(16, 32);
            PlayCallLabel.Name = "PlayCallLabel";
            PlayCallLabel.Size = new Size(67, 13);
            PlayCallLabel.TabIndex = 2;
            PlayCallLabel.Text = "PLAY: CALL";
            // 
            // MuteCallLabel
            // 
            MuteCallLabel.Location = new Point(16, 48);
            MuteCallLabel.Name = "MuteCallLabel";
            MuteCallLabel.Size = new Size(75, 13);
            MuteCallLabel.TabIndex = 4;
            MuteCallLabel.Text = "MUTE: CALL";
            // 
            // SetupByteLabel
            // 
            SetupByteLabel.Location = new Point(16, 64);
            SetupByteLabel.Name = "SetupByteLabel";
            SetupByteLabel.Size = new Size(63, 13);
            SetupByteLabel.TabIndex = 6;
            SetupByteLabel.Text = "Setup Byte:";
            // 
            // Bit7Label
            // 
            Bit7Label.Location = new Point(112, 80);
            Bit7Label.Name = "Bit7Label";
            Bit7Label.Size = new Size(147, 13);
            Bit7Label.TabIndex = 8;
            Bit7Label.Text = "Bit 7 is Set After Each Loop.";
            // 
            // Bit0Label
            // 
            Bit0Label.Location = new Point(112, 64);
            Bit0Label.Name = "Bit0Label";
            Bit0Label.Size = new Size(137, 13);
            Bit0Label.TabIndex = 9;
            Bit0Label.Text = "Set Bit 0 to Disable Loop;";
            // 
            // CurrentPosLabel
            // 
            CurrentPosLabel.Location = new Point(16, 96);
            CurrentPosLabel.Name = "CurrentPosLabel";
            CurrentPosLabel.Size = new Size(158, 13);
            CurrentPosLabel.TabIndex = 10;
            CurrentPosLabel.Text = "Current Position Pointer (Word):";
            // 
            // VariablesAddressLabel
            // 
            VariablesAddressLabel.Location = new Point(16, 128);
            VariablesAddressLabel.Name = "VariablesAddressLabel";
            VariablesAddressLabel.Size = new Size(107, 13);
            VariablesAddressLabel.TabIndex = 12;
            VariablesAddressLabel.Text = "Variables Address:";
            // 
            // VariablesLengthLabel
            // 
            VariablesLengthLabel.Location = new Point(148, 128);
            VariablesLengthLabel.Name = "VariablesLengthLabel";
            VariablesLengthLabel.Size = new Size(51, 13);
            VariablesLengthLabel.TabIndex = 14;
            VariablesLengthLabel.Text = "; Length:";
            // 
            // PlayerCodesLengthLabel
            // 
            PlayerCodesLengthLabel.Location = new Point(16, 112);
            PlayerCodesLengthLabel.Name = "PlayerCodesLengthLabel";
            PlayerCodesLengthLabel.Size = new Size(115, 13);
            PlayerCodesLengthLabel.TabIndex = 16;
            PlayerCodesLengthLabel.Text = "Player Codes Length:";
            // 
            // ModuleAddressLabel
            // 
            ModuleAddressLabel.Location = new Point(16, 144);
            ModuleAddressLabel.Name = "ModuleAddressLabel";
            ModuleAddressLabel.Size = new Size(91, 13);
            ModuleAddressLabel.TabIndex = 18;
            ModuleAddressLabel.Text = "Module Address:";
            // 
            // ModuleLengthLabel
            // 
            ModuleLengthLabel.Location = new Point(148, 144);
            ModuleLengthLabel.Name = "ModuleLengthLabel";
            ModuleLengthLabel.Size = new Size(51, 13);
            ModuleLengthLabel.TabIndex = 20;
            ModuleLengthLabel.Text = "; Length:";
            // 
            // Module2LengthLabel
            // 
            Module2LengthLabel.Location = new Point(148, 159);
            Module2LengthLabel.Name = "Module2LengthLabel";
            Module2LengthLabel.Size = new Size(51, 13);
            Module2LengthLabel.TabIndex = 22;
            Module2LengthLabel.Text = "; Length:";
            // 
            // Module2AddressLabel
            // 
            Module2AddressLabel.Location = new Point(16, 159);
            Module2AddressLabel.Name = "Module2AddressLabel";
            Module2AddressLabel.Size = new Size(100, 13);
            Module2AddressLabel.TabIndex = 24;
            Module2AddressLabel.Text = "Module2 Address:";
            // 
            // SourceBox
            // 
            SourceBox.Location = new Point(16, 176);
            SourceBox.Multiline = true;
            SourceBox.Name = "SourceBox";
            SourceBox.ReadOnly = true;
            SourceBox.ScrollBars = ScrollBars.Vertical;
            SourceBox.Size = new Size(233, 89);
            SourceBox.TabIndex = 0;
            SourceBox.Text = resources.GetString("SourceBox.Text");
            // 
            // ExportZXForm
            // 
            ClientSize = new Size(516, 322);
            Controls.Add(OkButton);
            Controls.Add(CancelButton);
            Controls.Add(ParametersGroup);
            Controls.Add(FormatGroup);
            Controls.Add(HintsGroup);
            Font = new Font("Microsoft Sans Serif", 8.25F);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Location = new Point(204, 151);
            Name = "ExportZXForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "ZX Spectrum PT3/TS Player Parameters";
            FormClosing += ExportZXForm_FormClosing;
            ParametersGroup.ResumeLayout(false);
            ParametersGroup.PerformLayout();
            HintsGroup.ResumeLayout(false);
            HintsGroup.PerformLayout();
            ResumeLayout(false);

        }
        #endregion

        public System.Windows.Forms.Button OkButton;
        public System.Windows.Forms.Button CancelButton;
        public System.Windows.Forms.GroupBox ParametersGroup;
        public System.Windows.Forms.Label DecLabel;
        public System.Windows.Forms.Label HexLabel;
        public System.Windows.Forms.Label CompilationAddressLabel;
        public System.Windows.Forms.Label CompilationAddressValue;
        public System.Windows.Forms.TextBox DecAddressBox;
        public System.Windows.Forms.TextBox HexAddressBox;
        public System.Windows.Forms.CheckBox LoopChk;
        public RadioGroup FormatGroup;
        public System.Windows.Forms.GroupBox HintsGroup;
        public System.Windows.Forms.Label InitCallLabel;
        public System.Windows.Forms.Label PlayCallLabel;
        public System.Windows.Forms.Label PlayCallValue;
        public System.Windows.Forms.Label MuteCallLabel;
        public System.Windows.Forms.Label MuteCallValue;
        public System.Windows.Forms.Label SetupByteLabel;
        public System.Windows.Forms.Label SetupByteValue;
        public System.Windows.Forms.Label Bit7Label;
        public System.Windows.Forms.Label Bit0Label;
        public System.Windows.Forms.Label CurrentPosLabel;
        public System.Windows.Forms.Label CurrentPosValue;
        public System.Windows.Forms.Label VariablesAddressLabel;
        public System.Windows.Forms.Label VariablesLengthLabel;
        public System.Windows.Forms.Label VariablesLengthValue;
        public System.Windows.Forms.Label PlayerCodesLengthLabel;
        public System.Windows.Forms.Label PlayerCodesLengthValue;
        public System.Windows.Forms.Label ModuleAddressLabel;
        public System.Windows.Forms.Label ModuleLengthLabel;
        public System.Windows.Forms.Label Module2LengthLabel;
        public System.Windows.Forms.Label Module2AddressLabel;
        public System.Windows.Forms.Label Module2LengthValue;
        public System.Windows.Forms.Label InitCallValue;
        public System.Windows.Forms.Label VariablesAddressValue;
        public System.Windows.Forms.Label ModuleAddressValue;
        public System.Windows.Forms.Label ModuleLengthValue;
        public System.Windows.Forms.Label Module2AddressValue;
        public System.Windows.Forms.TextBox SourceBox;
        private System.Windows.Forms.ToolTip toolTip1 = null;
    }
}
