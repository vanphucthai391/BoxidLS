namespace BoxIdDB
{
    partial class FormLPD
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.ckbDeleteBox = new System.Windows.Forms.CheckBox();
            this.btnReplaceSerial = new System.Windows.Forms.Button();
            this.dgvDateCode2 = new System.Windows.Forms.DataGridView();
            this.btnDeleteBoxId = new System.Windows.Forms.Button();
            this.dgvDateCode = new System.Windows.Forms.DataGridView();
            this.dtpPrintDate = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtBoxIdPrint = new System.Windows.Forms.TextBox();
            this.txtProductSerial = new System.Windows.Forms.TextBox();
            this.txtBoxId = new System.Windows.Forms.TextBox();
            this.txtOkCount = new System.Windows.Forms.TextBox();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnRegisterBoxId = new System.Windows.Forms.Button();
            this.btnDeleteSelection = new System.Windows.Forms.Button();
            this.btnDeleteAll = new System.Windows.Forms.Button();
            this.dgvPassFail = new System.Windows.Forms.DataGridView();
            this.tabForGridview = new System.Windows.Forms.TabControl();
            this.tpOverall = new System.Windows.Forms.TabPage();
            this.dgvProductSerial = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnChangeLimit = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txtLimit = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.cmbModel = new System.Windows.Forms.ComboBox();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.serialno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.model = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Lot = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Line = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.process = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.linepass = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.testtime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDateCode2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDateCode)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPassFail)).BeginInit();
            this.tabForGridview.SuspendLayout();
            this.tpOverall.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductSerial)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // ckbDeleteBox
            // 
            this.ckbDeleteBox.AutoSize = true;
            this.ckbDeleteBox.Location = new System.Drawing.Point(656, 161);
            this.ckbDeleteBox.Name = "ckbDeleteBox";
            this.ckbDeleteBox.Size = new System.Drawing.Size(89, 17);
            this.ckbDeleteBox.TabIndex = 50;
            this.ckbDeleteBox.Text = "Delete BoxID";
            this.ckbDeleteBox.UseVisualStyleBackColor = true;
            this.ckbDeleteBox.CheckedChanged += new System.EventHandler(this.ckbDeleteBox_CheckedChanged);
            // 
            // btnReplaceSerial
            // 
            this.btnReplaceSerial.Location = new System.Drawing.Point(459, 177);
            this.btnReplaceSerial.Name = "btnReplaceSerial";
            this.btnReplaceSerial.Size = new System.Drawing.Size(83, 54);
            this.btnReplaceSerial.TabIndex = 49;
            this.btnReplaceSerial.Text = "Replace Serial";
            this.btnReplaceSerial.UseVisualStyleBackColor = true;
            this.btnReplaceSerial.Click += new System.EventHandler(this.btnReplaceSerial_Click);
            // 
            // dgvDateCode2
            // 
            this.dgvDateCode2.AllowUserToAddRows = false;
            this.dgvDateCode2.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            this.dgvDateCode2.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDateCode2.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvDateCode2.ColumnHeadersHeight = 18;
            this.dgvDateCode2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDateCode2.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvDateCode2.EnableHeadersVisualStyles = false;
            this.dgvDateCode2.GridColor = System.Drawing.Color.White;
            this.dgvDateCode2.Location = new System.Drawing.Point(72, 12);
            this.dgvDateCode2.Name = "dgvDateCode2";
            this.dgvDateCode2.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDateCode2.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvDateCode2.RowHeadersVisible = false;
            this.dgvDateCode2.RowHeadersWidth = 40;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvDateCode2.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvDateCode2.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dgvDateCode2.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.White;
            this.dgvDateCode2.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
            this.dgvDateCode2.RowTemplate.Height = 18;
            this.dgvDateCode2.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDateCode2.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dgvDateCode2.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvDateCode2.Size = new System.Drawing.Size(344, 43);
            this.dgvDateCode2.TabIndex = 48;
            this.dgvDateCode2.Visible = false;
            // 
            // btnDeleteBoxId
            // 
            this.btnDeleteBoxId.Location = new System.Drawing.Point(654, 177);
            this.btnDeleteBoxId.Name = "btnDeleteBoxId";
            this.btnDeleteBoxId.Size = new System.Drawing.Size(100, 24);
            this.btnDeleteBoxId.TabIndex = 45;
            this.btnDeleteBoxId.Text = "Delete BoxID";
            this.btnDeleteBoxId.UseVisualStyleBackColor = true;
            this.btnDeleteBoxId.Visible = false;
            this.btnDeleteBoxId.Click += new System.EventHandler(this.btnDeleteBoxId_Click);
            // 
            // dgvDateCode
            // 
            this.dgvDateCode.AllowUserToAddRows = false;
            this.dgvDateCode.AllowUserToDeleteRows = false;
            this.dgvDateCode.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.dgvDateCode.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.dgvDateCode.GridColor = System.Drawing.SystemColors.ControlLight;
            this.dgvDateCode.Location = new System.Drawing.Point(72, 24);
            this.dgvDateCode.Name = "dgvDateCode";
            this.dgvDateCode.ReadOnly = true;
            this.dgvDateCode.RowTemplate.Height = 21;
            this.dgvDateCode.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvDateCode.Size = new System.Drawing.Size(327, 66);
            this.dgvDateCode.TabIndex = 36;
            // 
            // dtpPrintDate
            // 
            this.dtpPrintDate.Enabled = false;
            this.dtpPrintDate.Location = new System.Drawing.Point(108, 164);
            this.dtpPrintDate.Name = "dtpPrintDate";
            this.dtpPrintDate.Size = new System.Drawing.Size(254, 20);
            this.dtpPrintDate.TabIndex = 43;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 195);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 34;
            this.label3.Text = "Product Serial: ";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(499, 15);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 33;
            this.label7.Text = "By Result:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(10, 29);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(43, 13);
            this.label8.TabIndex = 35;
            this.label8.Text = "By Lot: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 143);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 31;
            this.label2.Text = "Box ID: ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 167);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 30;
            this.label1.Text = "Print Date: ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(460, 138);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 28;
            this.label4.Text = "OK Count: ";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(460, 112);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(35, 13);
            this.label12.TabIndex = 29;
            this.label12.Text = "User: ";
            // 
            // txtBoxIdPrint
            // 
            this.txtBoxIdPrint.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtBoxIdPrint.Location = new System.Drawing.Point(226, 15);
            this.txtBoxIdPrint.Multiline = true;
            this.txtBoxIdPrint.Name = "txtBoxIdPrint";
            this.txtBoxIdPrint.Size = new System.Drawing.Size(190, 24);
            this.txtBoxIdPrint.TabIndex = 26;
            this.txtBoxIdPrint.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtBoxIdPrint.Visible = false;
            // 
            // txtProductSerial
            // 
            this.txtProductSerial.Location = new System.Drawing.Point(108, 190);
            this.txtProductSerial.Name = "txtProductSerial";
            this.txtProductSerial.Size = new System.Drawing.Size(254, 20);
            this.txtProductSerial.TabIndex = 25;
            this.txtProductSerial.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtProductSerial_KeyDown);
            // 
            // txtBoxId
            // 
            this.txtBoxId.Enabled = false;
            this.txtBoxId.Location = new System.Drawing.Point(108, 138);
            this.txtBoxId.Name = "txtBoxId";
            this.txtBoxId.Size = new System.Drawing.Size(254, 20);
            this.txtBoxId.TabIndex = 24;
            // 
            // txtOkCount
            // 
            this.txtOkCount.Enabled = false;
            this.txtOkCount.Location = new System.Drawing.Point(559, 135);
            this.txtOkCount.Name = "txtOkCount";
            this.txtOkCount.Size = new System.Drawing.Size(193, 20);
            this.txtOkCount.TabIndex = 23;
            // 
            // txtUser
            // 
            this.txtUser.Enabled = false;
            this.txtUser.Location = new System.Drawing.Point(559, 109);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(193, 20);
            this.txtUser.TabIndex = 27;
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(548, 207);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(100, 24);
            this.btnPrint.TabIndex = 42;
            this.btnPrint.Text = "Print Box ID";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnRegisterBoxId
            // 
            this.btnRegisterBoxId.Location = new System.Drawing.Point(548, 177);
            this.btnRegisterBoxId.Name = "btnRegisterBoxId";
            this.btnRegisterBoxId.Size = new System.Drawing.Size(100, 24);
            this.btnRegisterBoxId.TabIndex = 39;
            this.btnRegisterBoxId.Text = "Register Box ID";
            this.btnRegisterBoxId.UseVisualStyleBackColor = true;
            this.btnRegisterBoxId.Click += new System.EventHandler(this.btnRegisterBoxId_Click);
            // 
            // btnDeleteSelection
            // 
            this.btnDeleteSelection.Location = new System.Drawing.Point(654, 207);
            this.btnDeleteSelection.Name = "btnDeleteSelection";
            this.btnDeleteSelection.Size = new System.Drawing.Size(100, 24);
            this.btnDeleteSelection.TabIndex = 40;
            this.btnDeleteSelection.Text = "Delete Selection";
            this.btnDeleteSelection.UseVisualStyleBackColor = true;
            this.btnDeleteSelection.Click += new System.EventHandler(this.btnDeleteSelection_Click);
            // 
            // btnDeleteAll
            // 
            this.btnDeleteAll.Location = new System.Drawing.Point(654, 177);
            this.btnDeleteAll.Name = "btnDeleteAll";
            this.btnDeleteAll.Size = new System.Drawing.Size(100, 24);
            this.btnDeleteAll.TabIndex = 41;
            this.btnDeleteAll.Text = "Delete All";
            this.btnDeleteAll.UseVisualStyleBackColor = true;
            this.btnDeleteAll.Click += new System.EventHandler(this.btnDeleteAll_Click);
            // 
            // dgvPassFail
            // 
            this.dgvPassFail.AllowUserToAddRows = false;
            this.dgvPassFail.AllowUserToDeleteRows = false;
            this.dgvPassFail.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.dgvPassFail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPassFail.GridColor = System.Drawing.SystemColors.ControlLight;
            this.dgvPassFail.Location = new System.Drawing.Point(561, 12);
            this.dgvPassFail.Name = "dgvPassFail";
            this.dgvPassFail.ReadOnly = true;
            this.dgvPassFail.RowTemplate.Height = 21;
            this.dgvPassFail.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvPassFail.Size = new System.Drawing.Size(327, 62);
            this.dgvPassFail.TabIndex = 37;
            // 
            // tabForGridview
            // 
            this.tabForGridview.Controls.Add(this.tpOverall);
            this.tabForGridview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabForGridview.Location = new System.Drawing.Point(0, 0);
            this.tabForGridview.Name = "tabForGridview";
            this.tabForGridview.SelectedIndex = 0;
            this.tabForGridview.Size = new System.Drawing.Size(931, 360);
            this.tabForGridview.TabIndex = 44;
            // 
            // tpOverall
            // 
            this.tpOverall.Controls.Add(this.dgvProductSerial);
            this.tpOverall.Location = new System.Drawing.Point(4, 22);
            this.tpOverall.Name = "tpOverall";
            this.tpOverall.Padding = new System.Windows.Forms.Padding(3);
            this.tpOverall.Size = new System.Drawing.Size(923, 334);
            this.tpOverall.TabIndex = 0;
            this.tpOverall.Text = "In-Line";
            this.tpOverall.UseVisualStyleBackColor = true;
            // 
            // dgvProductSerial
            // 
            this.dgvProductSerial.AllowUserToAddRows = false;
            this.dgvProductSerial.AllowUserToDeleteRows = false;
            this.dgvProductSerial.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgvProductSerial.BackgroundColor = System.Drawing.Color.SeaShell;
            this.dgvProductSerial.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProductSerial.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id,
            this.serialno,
            this.model,
            this.Lot,
            this.Line,
            this.process,
            this.linepass,
            this.testtime});
            this.dgvProductSerial.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvProductSerial.Location = new System.Drawing.Point(3, 3);
            this.dgvProductSerial.Name = "dgvProductSerial";
            this.dgvProductSerial.ReadOnly = true;
            this.dgvProductSerial.RowTemplate.Height = 21;
            this.dgvProductSerial.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvProductSerial.Size = new System.Drawing.Size(917, 328);
            this.dgvProductSerial.TabIndex = 9;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmbModel);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.btnChangeLimit);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.txtLimit);
            this.panel1.Controls.Add(this.ckbDeleteBox);
            this.panel1.Controls.Add(this.btnReplaceSerial);
            this.panel1.Controls.Add(this.dgvPassFail);
            this.panel1.Controls.Add(this.dgvDateCode2);
            this.panel1.Controls.Add(this.btnDeleteAll);
            this.panel1.Controls.Add(this.btnDeleteSelection);
            this.panel1.Controls.Add(this.btnDeleteBoxId);
            this.panel1.Controls.Add(this.btnRegisterBoxId);
            this.panel1.Controls.Add(this.dgvDateCode);
            this.panel1.Controls.Add(this.btnPrint);
            this.panel1.Controls.Add(this.dtpPrintDate);
            this.panel1.Controls.Add(this.txtUser);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.txtOkCount);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.txtBoxId);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.txtProductSerial);
            this.panel1.Controls.Add(this.txtBoxIdPrint);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(931, 250);
            this.panel1.TabIndex = 51;
            // 
            // btnChangeLimit
            // 
            this.btnChangeLimit.Location = new System.Drawing.Point(779, 132);
            this.btnChangeLimit.Name = "btnChangeLimit";
            this.btnChangeLimit.Size = new System.Drawing.Size(100, 24);
            this.btnChangeLimit.TabIndex = 52;
            this.btnChangeLimit.Text = "Change Limit";
            this.btnChangeLimit.UseVisualStyleBackColor = true;
            this.btnChangeLimit.Visible = false;
            this.btnChangeLimit.Click += new System.EventHandler(this.btnChangeLimit_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(779, 175);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 24);
            this.btnCancel.TabIndex = 53;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtLimit
            // 
            this.txtLimit.Enabled = false;
            this.txtLimit.Location = new System.Drawing.Point(779, 109);
            this.txtLimit.Name = "txtLimit";
            this.txtLimit.Size = new System.Drawing.Size(100, 20);
            this.txtLimit.TabIndex = 51;
            this.txtLimit.Visible = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tabForGridview);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 250);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(931, 360);
            this.panel2.TabIndex = 52;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(19, 112);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(39, 13);
            this.label9.TabIndex = 54;
            this.label9.Text = "Model:";
            // 
            // cmbModel
            // 
            this.cmbModel.FormattingEnabled = true;
            this.cmbModel.Items.AddRange(new object[] {
            "LPD10G_111",
            "LPD10GE_107"});
            this.cmbModel.Location = new System.Drawing.Point(108, 109);
            this.cmbModel.Name = "cmbModel";
            this.cmbModel.Size = new System.Drawing.Size(254, 21);
            this.cmbModel.TabIndex = 55;
            // 
            // id
            // 
            this.id.DataPropertyName = "id";
            this.id.HeaderText = "ID";
            this.id.Name = "id";
            this.id.ReadOnly = true;
            this.id.Visible = false;
            this.id.Width = 43;
            // 
            // serialno
            // 
            this.serialno.DataPropertyName = "serialno";
            this.serialno.HeaderText = "Serial No";
            this.serialno.Name = "serialno";
            this.serialno.ReadOnly = true;
            this.serialno.Width = 75;
            // 
            // model
            // 
            this.model.DataPropertyName = "model";
            this.model.HeaderText = "Model";
            this.model.Name = "model";
            this.model.ReadOnly = true;
            this.model.Width = 61;
            // 
            // Lot
            // 
            this.Lot.DataPropertyName = "lot";
            this.Lot.HeaderText = "Lot";
            this.Lot.Name = "Lot";
            this.Lot.ReadOnly = true;
            this.Lot.Width = 47;
            // 
            // Line
            // 
            this.Line.DataPropertyName = "line";
            this.Line.HeaderText = "Line";
            this.Line.Name = "Line";
            this.Line.ReadOnly = true;
            this.Line.Width = 52;
            // 
            // process
            // 
            this.process.DataPropertyName = "process";
            this.process.HeaderText = "Process";
            this.process.Name = "process";
            this.process.ReadOnly = true;
            this.process.Width = 70;
            // 
            // linepass
            // 
            this.linepass.DataPropertyName = "linepass";
            this.linepass.HeaderText = "Judge";
            this.linepass.Name = "linepass";
            this.linepass.ReadOnly = true;
            this.linepass.Width = 61;
            // 
            // testtime
            // 
            this.testtime.DataPropertyName = "testtime";
            this.testtime.HeaderText = "Test Time";
            this.testtime.Name = "testtime";
            this.testtime.ReadOnly = true;
            this.testtime.Width = 79;
            // 
            // FormLPD
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(931, 610);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "FormLPD";
            this.Text = "Box ID LPD";
            this.Load += new System.EventHandler(this.FormLPD_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDateCode2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDateCode)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPassFail)).EndInit();
            this.tabForGridview.ResumeLayout(false);
            this.tpOverall.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductSerial)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox ckbDeleteBox;
        private System.Windows.Forms.Button btnReplaceSerial;
        private System.Windows.Forms.DataGridView dgvDateCode2;
        private System.Windows.Forms.Button btnDeleteBoxId;
        private System.Windows.Forms.DataGridView dgvDateCode;
        private System.Windows.Forms.DateTimePicker dtpPrintDate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtBoxIdPrint;
        private System.Windows.Forms.TextBox txtProductSerial;
        private System.Windows.Forms.TextBox txtBoxId;
        private System.Windows.Forms.TextBox txtOkCount;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnRegisterBoxId;
        private System.Windows.Forms.Button btnDeleteSelection;
        private System.Windows.Forms.Button btnDeleteAll;
        private System.Windows.Forms.DataGridView dgvPassFail;
        private System.Windows.Forms.TabControl tabForGridview;
        private System.Windows.Forms.TabPage tpOverall;
        private System.Windows.Forms.DataGridView dgvProductSerial;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnChangeLimit;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TextBox txtLimit;
        private System.Windows.Forms.ComboBox cmbModel;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn serialno;
        private System.Windows.Forms.DataGridViewTextBoxColumn model;
        private System.Windows.Forms.DataGridViewTextBoxColumn Lot;
        private System.Windows.Forms.DataGridViewTextBoxColumn Line;
        private System.Windows.Forms.DataGridViewTextBoxColumn process;
        private System.Windows.Forms.DataGridViewTextBoxColumn linepass;
        private System.Windows.Forms.DataGridViewTextBoxColumn testtime;
    }
}