﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Security.Permissions;
using System.Collections;
using Npgsql;
using System.Runtime.InteropServices;

namespace BoxIdDb
{
    public partial class frmModule : Form
    {
        // The delegate variable to signal the occurrance of delegate event to the parent form "formBoxid"
        public delegate void RefreshEventHandler(object sender, EventArgs e);
        public event RefreshEventHandler RefreshEvent;

        // The variable for degignate the shared floder to save text files for printing, 
        // which is to be printed by separate printing application
        string appconfig = System.Environment.CurrentDirectory + "\\info.ini";
        string directory = @"C:\Users\mt-qc20\Desktop\Auto Print\";

        // Other global variables
        bool formEditMode;
        string user;
        string m_model;
        string m_lot;
        int okCount;
        bool inputBoxModeOriginal;
        string testerTableThisMonth;
        string testerTableLastMonth;
        DataTable dtOverall;
        int limit = 100;
        public int limit1 = 0;
        int limitls4a;
        int limitls12;
        int limitlaa;
        int updateRowIndex;
        bool sound;
        string fltls = "process in ('EN2-LVT', 'MOTOR')";
        string fltls3P = "process in ('EN2-LVT', 'MOTOR', 'FINAL')";
        string fltls4A = "process in ('EN2-LVT', 'FINAL')";
        string fltlsreturn = "process ='EN2-LVT'";
        string fltlsreturn3P = "process in ('EN2-LVT','FINAL')";
        string fltlsreturn4A = "process in ('EN2-LVT')";
        string fltlaa = "process = 'LAA_LVT'";
        string fltls24 = "process = 'MOTOR'";
        // ConstructorOk
        public frmModule()
        {
            InitializeComponent();
            ShSQL SQL = new ShSQL();
            string query = "Select model from model_tbl";
            SQL.getComboBoxData(query, ref cmbModel);
            //cmbModel.DisplayMember = "model_name";
            //cmbModel.ValueMember = "model";
            cmbModel.ResetText();
        }

        // Load event
        private void frmModule_Load(object sender, EventArgs e)
        {
            // Store user name to the variable
            user = txtUser.Text;

            // Show box capacity in the text box
            txtLimit.Text = limit1.ToString();
            txtOkCount.Text = okCount + "/" + limit;

            // Get the printing folder directory from the application setting file and store it to the variable
            directory = @"Z:\(01)KK03\QA\(00)Public\03 OQC\02. LS\29.print (ko xoa)\";
            limitls4a = 480;
            limitls12 = 100; // limit
            limitlaa = 3000;

            // Set this form's position on the screen
            this.Left = 350;
            this.Top = 30;

            // Generate datatbles to hold modules records
            dtOverall = new DataTable();
            defineAndReadDtOverall(ref dtOverall);
            updateDataGripViews(dtOverall, ref dgvProductSerial);
            if (!string.IsNullOrEmpty(txtBoxId.Text))
            {
                int index = cmbModel.Items.IndexOf(txtBoxId.Text.Split('-')[0]);
                cmbModel.SelectedIndex = index;
            }
        }

        // Sub procedure: Read ini file content
        private string readIni(string s, string k, string cfs)
        {
            StringBuilder retVal = new StringBuilder(255);
            string section = s;
            string key = k;
            string def = String.Empty;
            int size = 255;
            //get the value from the key in section
            int strref = GetPrivateProfileString(section, key, def, retVal, size, cfs);
            return retVal.ToString();
        }
        // Import Windows API
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filepath);

        // Sub procedure: Transfer the parent from's information to this child form's objects
        public void updateControls(string boxId, DateTime printDate, string user, string serialNo, bool editMode)
        {
            txtBoxId.Text = boxId;
            dtpPrintDate.Value = printDate;
            txtUser.Text = user;
            txtProductSerial.Text = serialNo;

            cmbModel.Enabled = editMode;
            txtProductSerial.Enabled = editMode;
            btnRegisterBoxId.Enabled = !editMode;
            btnDeleteAll.Visible = editMode;
            btnDeleteSelection.Visible = editMode;
            formEditMode = editMode;
            this.Text = editMode ? "Product Serial - Edit Mode" : "Product Serial - Browse Mode";
            btnRegisterBoxId.Text = editMode ? "Register Box ID" : "Re-print";

            if (user == "User_9" || user == "admin")
            {
                btnChangeLimit.Visible = true;
                txtLimit.Visible = true;
                btnDeleteSelection.Visible = true;
                btnUpdate.Enabled = true;
                // btnRegisterBoxId.Visible = true;
            }

            if (!editMode && user == "User_9" || !editMode && user == "User_1")
            {
                btnReplace.Visible = true;
                btnDeleteBoxId.Visible = true;
            }
        }

        // Sub procedure: Get module recors from database and set them into this form's datatable
        private void defineAndReadDtOverall(ref DataTable dt)
        {
            string boxId = txtBoxId.Text;

            dt.Columns.Add("serialno", Type.GetType("System.String"));
            dt.Columns.Add("model", Type.GetType("System.String"));
            dt.Columns.Add("lot", Type.GetType("System.String"));
            dt.Columns.Add("fact", Type.GetType("System.String"));
            dt.Columns.Add("process", Type.GetType("System.String"));
            dt.Columns.Add("linepass", Type.GetType("System.String"));
            dt.Columns.Add("testtime", Type.GetType("System.DateTime"));

            if (!formEditMode)
            {
                // DataTable dtadd = new DataTable();
                string sql = "select serialno, lot, fact, process, linepass, testtime, model " +
                    "FROM product_serial WHERE boxid='" + boxId + "'";
                ShSQL tf = new ShSQL();
                tf.sqlDataAdapterFillDatatable(sql, ref dt);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string value = dt.Rows[i][2].ToString();
                    if (value.Contains(" "))
                        dt.Rows[i][2] = value.Replace(" ", "");
                }
            }
        }

        // Sub procedure: Update datagridviews
        private void updateDataGripViews(DataTable dt1, ref DataGridView dgv1)
        {
            // Store the ENABLED status to the variable, then make the text boxs disenabled
            inputBoxModeOriginal = txtProductSerial.Enabled;
            txtProductSerial.Enabled = true;

            // Bind datatable to the datagridview
            updateDataGripViewsSub(dt1, ref dgv1);

            // Mark the records with the test result FAIL or missing
            colorViewForFailAndBlank(ref dgv1);

            // Mark config with duplicate or character length error
            colorMixedConfig(dt1, ref dgv1);
            //colorMixedLot(dt1, ref dgv1);

            // Mark the records with duplicate product serial or the serial with not enough character length
            colorViewForDuplicateSerial(ref dgv1);

            // Show row number to the row header
            for (int i = 0; i < dgv1.Rows.Count; i++)
                dgv1.Rows[i].HeaderCell.Value = (i + 1).ToString();

            // Adjust the width of the row header
            dgv1.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);

            // Show the bottom of the datagridview
            if (dgv1.Rows.Count >= 1)
                dgv1.FirstDisplayedScrollingRowIndex = dgv1.Rows.Count - 1;

            // Set the text box back to the original state
            txtProductSerial.Enabled = inputBoxModeOriginal;

            // Store the OK record count to the variable and show in the text box
            okCount = getOkCount(dt1);
            txtOkCount.Text = okCount + "/" + limit;

            // If the OK record count has already reached to the capacity, disenable the scan text box
            if (okCount == limit)
            {
                txtProductSerial.Enabled = false;
                btnRegisterBoxId.Visible = true;
            }
            else
                txtProductSerial.Enabled = true;

            // If the OK record coutn has already reached to the capacity, enable the register button
            if (okCount == limit && dgv1.Rows.Count == limit)
            {
                btnRegisterBoxId.Enabled = true;
                btnRegisterBoxId.Visible = true;
            }
            else
                btnRegisterBoxId.Enabled = false;
        }

        // Sub procedure: Count the without-duplicate OK records
        private int getOkCount(DataTable dt)
        {
            if (dt.Rows.Count <= 0) return 0;
            DataTable distinct = dt.DefaultView.ToTable(true, new string[] { "serialno", "linepass" });
            //DataRow[] dr = distinct.Select("linepass = 'PASS' and noise = 'SPEC IN'");
            DataRow[] dr = distinct.Select("linepass = 'PASS'");
            int dist = dr.Length;
            return dist;
        }

        // Sub procedure: Bind main datatable to the datagridview and make summary datatables
        private void updateDataGripViewsSub(DataTable dt1, ref DataGridView dgv1)
        {
            dgv1.DataSource = dt1;
            dgv1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            string[] criteriaFact = { "2A", "1", "2", "3", "4", "Total" };
            makeDatatableSummary(dt1, ref dgvLine, criteriaFact, "fact");

            string[] criteriaPassFail = { "PASS", "FAIL", "No Data", "Total" };
            makeDatatableSummary(dt1, ref dgvPassFail, criteriaPassFail, "linepass");

            string[] criteriaDateCode = getLotArray(dt1);
            makeDatatableSummary(dt1, ref dgvDateCode, criteriaDateCode, "lot");
        }

        // Sub procedure: Make the summary datatables and bind them to the summary datagridviews
        public void makeDatatableSummary(DataTable dt0, ref DataGridView dgv, string[] criteria, string header)
        {
            DataTable dt1 = new DataTable();
            DataRow dr = dt1.NewRow();
            Int32 count;
            Int32 total = 0;
            string condition;

            for (int i = 0; i < criteria.Length; i++)
            {
                dt1.Columns.Add(criteria[i], typeof(Int32));
                condition = header + " = '" + criteria[i] + "'";
                count = dt0.Select(condition).Length;
                total += count;
                dr[criteria[i]] = (i != criteria.Length - 1 ? count : total);
                if (criteria[i] == "Total" && header == "linepass")
                {
                    dr[criteria[i]] = dgvProductSerial.Rows.Count;
                    dr[criteria[i - 1]] = dgvProductSerial.Rows.Count - total;
                }
            }
            dt1.Rows.Add(dr);

            dgv.Columns.Clear();
            dgv.DataSource = dt1;
            dgv.AllowUserToAddRows = false; // remove the null line
            dgv.ReadOnly = true;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        // Sub procedure: Make lot summary
        private string[] getLotArray(DataTable dt0)
        {
            DataTable dt1 = dt0.Copy();
            DataView dv = dt1.DefaultView;
            dv.Sort = "lot";
            DataTable dt2 = dv.ToTable(true, "lot");
            string[] array = new string[dt2.Rows.Count + 1];
            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                array[i] = dt2.Rows[i]["lot"].ToString();
            }
            array[dt2.Rows.Count] = "Total";
            return array;
        }

        // Sub procedure: Mark the test results with FAIL or missing test records
        private void colorViewForFailAndBlank(ref DataGridView dgv)
        {
            int rowCount = dgv.BindingContext[dgv.DataSource, dgv.DataMember].Count;
            for (int i = 0; i < rowCount; ++i)
            {
                if (dgv["linepass", i].Value.ToString() == "FAIL" || dgv["linepass", i].Value.ToString() == String.Empty)
                {
                    dgv["process", i].Style.BackColor = Color.Red;
                    dgv["linepass", i].Style.BackColor = Color.Red;
                    dgv["testtime", i].Style.BackColor = Color.Red;
                    soundAlarm();
                }
                else
                {
                    dgv["process", i].Style.BackColor = Color.FromKnownColor(KnownColor.Window);
                    dgv["linepass", i].Style.BackColor = Color.FromKnownColor(KnownColor.Window);
                    dgv["testtime", i].Style.BackColor = Color.FromKnownColor(KnownColor.Window);
                }
            }
        }

        // Sub procesure: Mark product serials with duplicate or character length error
        private void colorViewForDuplicateSerial(ref DataGridView dgv)
        {
            DataTable dt = ((DataTable)dgv.DataSource).Copy();
            if (dt.Rows.Count <= 0) return;

            for (int i = 0; i < dtOverall.Rows.Count; i++)
            {
                string serial = dgv[0, i].Value.ToString();
                DataRow[] dr = dt.Select("serialno = '" + serial + "'");
                if (dr.Length >= 2 || dgv[0, i].Value.ToString().Length != 13 && dgv[0, i].Value.ToString().Length != 8 && dgv[0, i].Value.ToString().Length != 10)
                {
                    dgv[0, i].Style.BackColor = Color.Red;
                    soundAlarm();
                }
                else
                {
                    dgv[0, i].Style.BackColor = Color.FromKnownColor(KnownColor.Window);
                }
            }
        }

        // Sub procesure: Mark config with duplicate or character length error
        private void colorMixedConfig(DataTable dt, ref DataGridView dgv)
        {
            if (dt.Rows.Count <= 0) return;

            DataTable distinct = dt.DefaultView.ToTable(true, new string[] { "model" });

            if (distinct.Rows.Count == 1)
                m_model = distinct.Rows[0]["model"].ToString();

            if (distinct.Rows.Count >= 2)
            {
                string A = distinct.Rows[0]["model"].ToString();
                string B = distinct.Rows[1]["model"].ToString();
                int a = distinct.Select("model = '" + A + "'").Length;
                int b = distinct.Select("model = '" + B + "'").Length;

                // Œ”‚Ì‘½‚¢ƒRƒ“ƒtƒBƒO‚ðA‚±‚Ì” ‚ÌƒƒCƒ“ƒ‚ƒfƒ‹‚Æ‚·‚é
                m_model = a > b ? A : B;

                // Œ”‚Ì­‚È‚¢‚Ù‚¤‚ÌƒƒCƒ“ƒ‚ƒfƒ‹•¶Žš‚ðŽæ“¾‚µAƒZƒ‹”Ô’n‚ð“Á’è‚µ‚Äƒ}[ƒN‚·‚é
                string C = a < b ? A : B;
                int c = -1;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["model"].ToString() == C) { c = i; }
                }

                if (c != -1)
                {
                    dgv["model", c].Style.BackColor = Color.Red;
                    soundAlarm();
                }
                else
                {
                    dgv.Columns["model"].DefaultCellStyle.BackColor = Color.FromKnownColor(KnownColor.Window);
                }
            }
        }

        private void colorMixedLot(DataTable dt, ref DataGridView dgv)
        {
            if (dt.Rows.Count <= 0) return;

            DataTable distinct1 = dt.DefaultView.ToTable(true, new string[] { "lot" });

            if (distinct1.Rows.Count == 1)
                m_lot = distinct1.Rows[0]["lot"].ToString();

            if (distinct1.Rows.Count >= 2)
            {
                string A = distinct1.Rows[0]["lot"].ToString();
                string B = distinct1.Rows[1]["lot"].ToString();
                int a = distinct1.Select("lot = '" + A + "'").Length;
                int b = distinct1.Select("lot = '" + B + "'").Length;

                // Œ”‚Ì‘½‚¢ƒRƒ“ƒtƒBƒO‚ðA‚±‚Ì” ‚ÌƒƒCƒ“ƒ‚ƒfƒ‹‚Æ‚·‚é
                m_lot = a > b ? A : B;

                // Œ”‚Ì­‚È‚¢‚Ù‚¤‚ÌƒƒCƒ“ƒ‚ƒfƒ‹•¶Žš‚ðŽæ“¾‚µAƒZƒ‹”Ô’n‚ð“Á’è‚µ‚Äƒ}[ƒN‚·‚é
                string C = a < b ? A : B;
                int c = -1;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["lot"].ToString() == C) { c = i; }
                }

                if (c != -1)
                {
                    dgv["lot", c].Style.BackColor = Color.Red;
                    soundAlarm();
                }
                else
                {
                    dgv.Columns["lot"].DefaultCellStyle.BackColor = Color.FromKnownColor(KnownColor.Window);
                }
            }
        }
        // Event when a module is scanned 
        private void txtProductSerial_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Disenalbe the extbox to block scanning
                txtProductSerial.Enabled = false;

                string serLong = txtProductSerial.Text;
                string serShort;
                string m_short = VBStrings.Mid(serLong, 3, 2);
                //switch (m_short)
                //{
                //    case "3L":
                serShort = serLong;
                //}
                DateTime sDate = DateTime.Today;
            A:
                string filterkey = decideReferenceTable(serShort, sDate);
                if (serLong != String.Empty)
                {
                    // Get the tester data from current month's table and store it in datatable
                    string filterLine = string.Empty;
                    if (filterkey == "LS3P") { filterLine = fltls3P; }
                    else if (filterkey == "LS4A") { filterLine = fltls4A; }
                    else if (filterkey == "BMS_0240") { filterLine = fltls24; }
                    else filterLine = fltls;
                    if (ckReturn.Checked)
                    {
                        string sql = "";
                        if (cmbModel.Text == "LS3P")
                        {
                            sql = "select serno, process, judge, inspectdate from " +
                         "(select serno, process, judge, max(inspectdate) as inspectdate, row_number() OVER (PARTITION BY process ORDER BY max(inspectdate) desc) as flag from (" +
                         "(select serno, process, case when tjudge = '0' then 'PASS' else 'FAIL' end as judge, inspectdate from " + testerTableThisMonth + " where " + fltlsreturn3P + " and serno = '" + serShort + "') union all " +
                         "(select serno, process, case when tjudge = '0' then 'PASS' else 'FAIL' end as judge, inspectdate from " + testerTableLastMonth + " where " + fltlsreturn3P + " and serno = '" + serShort + "')" +
                         ") d group by serno, judge, process order by judge desc, process) b where flag = 1";
                        }
                        //if (cmbModel.Text == "LS4A")
                        //{
                        //    sql = "select serno, process, judge, inspectdate from " +
                        // "(select serno, process, judge, max(inspectdate) as inspectdate, row_number() OVER (PARTITION BY process ORDER BY max(inspectdate) desc) as flag from (" +
                        // "(select serno, process, case when tjudge = '0' then 'PASS' else 'FAIL' end as judge, inspectdate from " + testerTableThisMonth + " where " + fltlsreturn4A + " and serno = '" + serShort + "') union all " +
                        // "(select serno, process, case when tjudge = '0' then 'PASS' else 'FAIL' end as judge, inspectdate from " + testerTableLastMonth + " where " + fltlsreturn4A + " and serno = '" + serShort + "')" +
                        // ") d group by serno, judge, process order by judge desc, process) b where flag = 1";
                        //}
                        else
                        {
                            sql = "select serno, process, judge, inspectdate from " +
                        "(select serno, process, judge, max(inspectdate) as inspectdate, row_number() OVER (PARTITION BY process ORDER BY max(inspectdate) desc) as flag from (" +
                        "(select serno, process, case when tjudge = '0' then 'PASS' else 'FAIL' end as judge, inspectdate from " + testerTableThisMonth + " where " + fltlsreturn + " and serno = '" + serShort + "') union all " +
                        "(select serno, process, case when tjudge = '0' then 'PASS' else 'FAIL' end as judge, inspectdate from " + testerTableLastMonth + " where " + fltlsreturn + " and serno = '" + serShort + "')" +
                        ") d group by serno, judge, process order by judge desc, process) b where flag = 1";
                        }
                        DataTable dt1 = new DataTable();
                        ShSQL tf = new ShSQL();
                        tf.sqlDataAdapterFillDatatableFromTesterDb(sql, ref dt1);

                        if (dt1.Rows.Count <= 0)
                        {
                            if (sDate.Year.ToString() == "2015")
                            {
                                MessageBox.Show("Not Found!");
                                goto B;
                            }
                            sDate = sDate.AddMonths(-1);
                            goto A;
                        }
                    B:
                        System.Diagnostics.Debug.Print(sql);

                        // Get the tester data from last month's table and store it in the same datatable
                        //sql = "SELECT serno, process, tjudge, inspectdate" +
                        //    " FROM " + testerTableLastMonth +
                        //    " WHERE serno = '" + serShort + "'";
                        //tf.sqlDataAdapterFillDatatableFromTesterDb(sql, ref dt1);

                        System.Diagnostics.Debug.Print(sql);

                        DataView dv = new DataView(dt1);
                        //dv.RowFilter = filterLine;
                        //dv.Sort = "tjudge, inspectdate desc";

                        System.Diagnostics.Debug.Print(System.Environment.NewLine + "Inline:");
                        printDataView(dv);
                        DataTable dt2 = dv.ToTable();

                        //‡@ƒCƒ“ƒ‰ƒCƒ“
                        // ˆêŽžƒe[ƒuƒ‹‚Ö‚Ì“o˜^€”õ
                        string lot = string.Empty;
                        string fact = "2A";
                        string model = string.Empty;
                        switch (m_short)
                        {
                            case "4A":
                                model = "LS4A";
                                lot = VBStrings.Mid(serShort, 5, 3);
                                break;
                            case "4D":
                                model = "LS4D";
                                lot = VBStrings.Mid(serShort, 5, 3);
                                break;
                            case "3D":
                                model = "LS3D";
                                lot = VBStrings.Mid(serShort, 5, 3);
                                break;
                            case "3E":
                                model = "LS3E";
                                lot = VBStrings.Mid(serShort, 5, 3);
                                break;
                            case "3F":
                                model = "LS3F";
                                lot = VBStrings.Mid(serShort, 5, 3);
                                break;
                            case "3K":
                                model = "LS3K";
                                lot = VBStrings.Mid(serShort, 5, 3);
                                break;
                            case "3J":
                                model = "LS3J";
                                lot = VBStrings.Mid(serShort, 5, 3);
                                break;
                            case "3P":
                                model = "LS3P";
                                lot = VBStrings.Mid(serShort, 5, 3);
                                break;
                            default:
                                if (serLong.Length == 13)
                                {
                                    if (cmbModel.Text == "BMS69")
                                    {
                                        model = "BMS69";
                                        lot = VBStrings.Mid(serShort, 3, 3);
                                    }
                                    else if (cmbModel.Text == "BMS70")
                                    {
                                        model = "BMS70";
                                        lot = VBStrings.Mid(serShort, 3, 3);
                                    }
                                    else if (cmbModel.Text == "BMS58")
                                    {
                                        model = "BMS58";
                                        lot = VBStrings.Mid(serShort, 5, 3);
                                    }
                                    else if (cmbModel.Text == "BMS57")
                                    {
                                        model = "BMS57";
                                        lot = VBStrings.Mid(serShort, 5, 3);
                                    }
                                    else if (cmbModel.Text == "BMS_0240")
                                    {
                                        model = "BMS_0240";
                                        lot = VBStrings.Mid(serShort, 5, 3);
                                    }
                                    else if (cmbModel.Text == "BMS_0314")
                                    {
                                        model = "BMS_0314";
                                        lot = VBStrings.Mid(serShort, 2, 4);
                                    }
                                }
                                else if (serLong.Length == 8) { model = "LA10"; lot = VBStrings.Mid(serShort, 5, 3); }
                                else if (VBStrings.Mid(serLong, 6, 1) == "L") { model = "LS3L"; lot = VBStrings.Mid(serShort, 3, 3); }
                                else if (VBStrings.Left(serLong, 1) == "M") { model = "LMOD"; lot = VBStrings.Mid(serShort, 5, 3); }
                                else model = "Error";
                                break;
                        }

                        // Even when no tester data is found, the module have to appear in the datagridview
                        DataRow newrow = dtOverall.NewRow();
                        newrow["serialno"] = serLong;
                        newrow["model"] = model;
                        newrow["lot"] = lot;
                        newrow["fact"] = fact;
                        #region ADD new code
                        // If tester data exists, show it in the datagridview
                        if (dt2.Rows.Count != 0)
                        {
                            if (cmbModel.Text == "LS3P")
                            {
                                if (dt2.Rows.Count == 1)
                                {
                                    string process = dt2.Rows[0][1].ToString();
                                    string linepass = dt2.Rows[0][2].ToString();
                                    DateTime testtime = (DateTime)dt2.Rows[0][3];
                                    if (process == "EN2-LVT" || process == "MOTOR")
                                        linepass = "FAIL";
                                    newrow["process"] = process;
                                    newrow["linepass"] = linepass;
                                    newrow["testtime"] = testtime;
                                }
                                else
                                {
                                    string process = dt2.Rows[0][1].ToString();
                                    string linepass = dt2.Rows[0][2].ToString();
                                    string process2 = dt2.Rows[1][1].ToString();
                                    string linepass2 = dt2.Rows[1][2].ToString();

                                    string resultprocess = process + "," + process2;
                                    string resultlinepass = "";
                                    if (linepass == "PASS" && linepass2 == "PASS")
                                        resultlinepass = "PASS";
                                    if (linepass == "FAIL" || linepass2 == "FAIL")
                                        resultlinepass = "FAIL";


                                    DateTime testtime = (DateTime)dt2.Rows[0][3];
                                    //newrow["process"] = process;
                                    //newrow["linepass"] = linepass;
                                    newrow["process"] = resultprocess;
                                    newrow["linepass"] = resultlinepass;
                                    newrow["testtime"] = testtime;
                                }
                            }
                            else
                            {
                                string process = dt2.Rows[0][1].ToString();
                                string linepass = dt2.Rows[0][2].ToString();
                                DateTime testtime = (DateTime)dt2.Rows[0][3];
                                newrow["process"] = process;
                                newrow["linepass"] = linepass;
                                newrow["testtime"] = testtime;
                            }
                        }
                        #endregion
                        // Add the row to the datatable
                        dtOverall.Rows.Add(newrow);

                        //Set limit
                        if (dtOverall.Rows.Count == 1)
                        {
                            string m = dtOverall.Rows[0]["model"].ToString();
                            switch (m)
                            {
                                case "LS4A":
                                    limit = limitls4a;
                                    break;
                                case "LS4D":
                                    limit = limitls4a;
                                    break;
                                case "LS3D":
                                case "LS3E":
                                case "LS3F":
                                case "LS3J":
                                case "LS3K":
                                case "LS3L":
                                case "LS3P":
                                case "LMOD":
                                case "BMS69":
                                case "BMS_0314":
                                case "BMS58":
                                case "BMS57":
                                case "BMS70":
                                case "BMS14":
                                case "BMS15":
                                    limit = limitls12;
                                    break;
                                case "LA10":
                                    limit = limitlaa;
                                    break;
                                default:
                                    limit = 9999;
                                    break;
                            }
                            //if (m == "LS4A") limit = limitls4a;
                            //else if (m == "LS3D" || m == "LS3E" || m == "LS3F" || m == "LS3J" || m == "LS3K" || m == "LS3L") limit = limitls12;
                            //else if (m == "LA10") limit = limitlaa;
                            //else limit = 9999;

                            // ‚t‚r‚d‚q‚X‚ª‚k‚h‚l‚h‚s‚ðÝ’è‚µ‚½ê‡‚ÍA‚»‚ê‚É]‚¤
                            if (limit1 != 0) limit = limit1;
                        }
                        // ƒf[ƒ^ƒOƒŠƒbƒgƒrƒ…[‚ÌXV
                        updateDataGripViews(dtOverall, ref dgvProductSerial);
                    }
                    // check hàng thường
                    else
                    {
                        string sql = "select serno, process, judge, inspectdate from " +
                              "(select serno, process, judge, max(inspectdate) as inspectdate, row_number() OVER (PARTITION BY process ORDER BY max(inspectdate) desc) as flag from (" +
                              "(select serno, process, case when tjudge = '0' then 'PASS' else 'FAIL' end as judge, inspectdate from " + testerTableThisMonth + " where " + filterLine + " and serno = '" + serShort + "') union all " +
                              "(select serno, process, case when tjudge = '0' then 'PASS' else 'FAIL' end as judge, inspectdate from " + testerTableLastMonth + " where " + filterLine + " and serno = '" + serShort + "')" +
                              ") d group by serno, judge, process order by judge desc, process) b where flag = 1";

                        DataTable dt1 = new DataTable();
                        ShSQL tf = new ShSQL();
                        tf.sqlDataAdapterFillDatatableFromTesterDb(sql, ref dt1);
                        if (dt1.Rows.Count <= 0)
                        {
                            if (sDate.Year.ToString() == "2015")
                            {
                                MessageBox.Show("Not Found!");
                                goto B;
                            }
                            sDate = sDate.AddMonths(-1);
                            goto A;
                        }
                    B:
                        System.Diagnostics.Debug.Print(sql);

                        // Get the tester data from last month's table and store it in the same datatable
                        //sql = "SELECT serno, process, tjudge, inspectdate" +
                        //    " FROM " + testerTableLastMonth +
                        //    " WHERE serno = '" + serShort + "'";
                        //tf.sqlDataAdapterFillDatatableFromTesterDb(sql, ref dt1);

                        System.Diagnostics.Debug.Print(sql);

                        DataView dv = new DataView(dt1);
                        //dv.RowFilter = filterLine;
                        //dv.Sort = "tjudge, inspectdate desc";

                        System.Diagnostics.Debug.Print(System.Environment.NewLine + "Inline:");
                        printDataView(dv);
                        DataTable dt2 = dv.ToTable();

                        //‡@ƒCƒ“ƒ‰ƒCƒ“
                        // ˆêŽžƒe[ƒuƒ‹‚Ö‚Ì“o˜^€”õ
                        string lot = string.Empty;
                        string fact = "2A";
                        string model = string.Empty;
                        switch (m_short)
                        {
                            case "4A":
                                model = "LS4A";
                                lot = VBStrings.Mid(serShort, 5, 3);
                                break;
                            case "4D":
                                model = "LS4D";
                                lot = VBStrings.Mid(serShort, 5, 3);
                                break;
                            case "3D":
                                model = "LS3D";
                                lot = VBStrings.Mid(serShort, 5, 3);
                                break;
                            case "3E":
                                model = "LS3E";
                                lot = VBStrings.Mid(serShort, 5, 3);
                                break;
                            case "3F":
                                model = "LS3F";
                                lot = VBStrings.Mid(serShort, 5, 3);
                                break;
                            case "3K":
                                model = "LS3K";
                                lot = VBStrings.Mid(serShort, 5, 3);
                                break;
                            case "3J":
                                model = "LS3J";
                                lot = VBStrings.Mid(serShort, 5, 3);
                                break;
                            case "3P":
                                model = "LS3P";
                                lot = VBStrings.Mid(serShort, 5, 3);
                                break;
                            default:
                                if (serLong.Length == 13)
                                {
                                    if (cmbModel.Text == "BMS69")
                                    {
                                        model = "BMS69";
                                        lot = VBStrings.Mid(serShort, 3, 3);
                                    }
                                    else if (cmbModel.Text == "BMS_0314")
                                    {
                                        model = "BMS_0314";
                                        lot = VBStrings.Mid(serShort, 2, 4);
                                    }
                                    else if (cmbModel.Text == "BMS70")
                                    {
                                        model = "BMS70";
                                        lot = VBStrings.Mid(serShort, 3, 3);
                                    }
                                    else if (cmbModel.Text == "BMS58")
                                    {
                                        model = "BMS58";
                                        lot = VBStrings.Mid(serShort, 5, 3);
                                    }
                                    else if (cmbModel.Text == "BMS57")
                                    {
                                        model = "BMS57";
                                        lot = VBStrings.Mid(serShort, 5, 3);
                                    }
                                    else if (cmbModel.Text == "BMS14")
                                    {
                                        model = "BMS14";
                                        lot = VBStrings.Mid(serShort, 2, 4);
                                    }
                                    else if (cmbModel.Text == "BMS15")
                                    {
                                        model = "BMS15";
                                        lot = VBStrings.Mid(serShort, 2, 4);
                                    }
                                    else if (cmbModel.Text == "BMS_0240")
                                    {
                                        model = "BMS_0240";
                                        lot = VBStrings.Mid(serShort, 2, 4);
                                    }
                                }
                                else if (serLong.Length == 8) { model = "LA10"; lot = VBStrings.Mid(serShort, 5, 3); }
                                else if (VBStrings.Mid(serLong, 6, 1) == "L") { model = "LS3L"; lot = VBStrings.Mid(serShort, 3, 3); }
                                else if (VBStrings.Left(serLong, 1) == "M") { model = "LMOD"; lot = VBStrings.Mid(serShort, 5, 3); }
                                else model = "Error";
                                break;
                        }

                        // Even when no tester data is found, the module have to appear in the datagridview
                        DataRow newrow = dtOverall.NewRow();
                        newrow["serialno"] = serLong;
                        newrow["model"] = model;
                        newrow["lot"] = lot;
                        newrow["fact"] = fact;
                        #region ADD new code
                        // If tester data exists, show it in the datagridview
                        if (dt2.Rows.Count != 0)
                        {
                            if (cmbModel.Text == "BMS57" || cmbModel.Text == "BMS58" || cmbModel.Text == "LS3K" || cmbModel.Text == "LS3E" || cmbModel.Text == "LS3F" || cmbModel.Text == "LS3L" || cmbModel.Text == "LS3P" || cmbModel.Text == "LS4A" || cmbModel.Text == "BMS14" || cmbModel.Text == "BMS15" || cmbModel.Text == "BMS_0314")
                            {
                                if (cmbModel.Text == "LS3P")
                                {
                                    if (dt2.Rows.Count == 1)
                                    {
                                        string process = dt2.Rows[0][1].ToString();
                                        string linepass = dt2.Rows[0][2].ToString();
                                        DateTime testtime = (DateTime)dt2.Rows[0][3];
                                        if (process == "EN2-LVT" || process == "MOTOR" || process == "FINAL")
                                            linepass = "FAIL";
                                        newrow["process"] = process;
                                        newrow["linepass"] = linepass;
                                        newrow["testtime"] = testtime;
                                    }
                                    else if (dt2.Rows.Count == 2)
                                    {
                                        string process1 = dt2.Rows[0][1].ToString();
                                        string process2 = dt2.Rows[1][1].ToString();

                                        string linepass = dt2.Rows[0][2].ToString();
                                        DateTime testtime = (DateTime)dt2.Rows[0][3];
                                        if (process1 == "EN2-LVT" || process1 == "MOTOR" || process1 == "FINAL")
                                            linepass = "FAIL";
                                        newrow["process"] = process1+","+process2;
                                        newrow["linepass"] = linepass;
                                        newrow["testtime"] = testtime;
                                    }
                                    else if (dt2.Rows.Count == 3)
                                    {
                                        string process = dt2.Rows[0][1].ToString();
                                        string linepass = dt2.Rows[0][2].ToString();
                                        string process2 = dt2.Rows[1][1].ToString();
                                        string linepass2 = dt2.Rows[1][2].ToString();
                                        string process3 = dt2.Rows[2][1].ToString();
                                        string linepass3 = dt2.Rows[2][2].ToString();
                                        string resultprocess = process + "," + process2 + "," + process3;
                                        string resultlinepass = "";
                                        if (linepass == "PASS" && linepass2 == "PASS" && linepass3 == "PASS")
                                            resultlinepass = "PASS";
                                        if (linepass == "FAIL" || linepass2 == "FAIL" || linepass3 == "FAIL")
                                            resultlinepass = "FAIL";
                                        DateTime testtime3p= DateTime.MinValue;
                                        foreach (DataRow row in dt2.Rows)
                                        {
                                            if (row[1].ToString() == "FINAL")
                                            {
                                                testtime3p = (DateTime)row[3];
                                                break;
                                            }
                                            else testtime3p = (DateTime)dt2.Rows[0][3];
                                        }
                                        //newrow["process"] = process;
                                        //newrow["linepass"] = linepass;
                                        newrow["process"] = resultprocess;
                                        newrow["linepass"] = resultlinepass;
                                        newrow["testtime"] = testtime3p;
                                    }
                                }
                                else if (cmbModel.Text == "LS4A")
                                {
                                    if (dt2.Rows.Count == 1)
                                    {
                                        string process = dt2.Rows[0][1].ToString();
                                        string linepass = dt2.Rows[0][2].ToString();
                                        DateTime testtime = (DateTime)dt2.Rows[0][3];
                                        if (process == "EN2-LVT" || process == "FINAL")
                                            linepass = "FAIL";
                                        newrow["process"] = process;
                                        newrow["linepass"] = linepass;
                                        newrow["testtime"] = testtime;
                                    }
                                    else
                                    {
                                        string process = dt2.Rows[0][1].ToString();
                                        string linepass = dt2.Rows[0][2].ToString();
                                        string process2 = dt2.Rows[1][1].ToString();
                                        string linepass2 = dt2.Rows[1][2].ToString();
                                        //string process3 = dt2.Rows[2][1].ToString();
                                        //string linepass3 = dt2.Rows[2][2].ToString();
                                        string resultprocess = process + "," + process2;
                                        string resultlinepass = "";
                                        if (linepass == "PASS" && linepass2 == "PASS")
                                            resultlinepass = "PASS";
                                        if (linepass == "FAIL" || linepass2 == "FAIL")
                                            resultlinepass = "FAIL";
                                        DateTime testtime = (DateTime)dt2.Rows[0][3];
                                        //newrow["process"] = process;
                                        //newrow["linepass"] = linepass;
                                        newrow["process"] = resultprocess;
                                        newrow["linepass"] = resultlinepass;
                                        newrow["testtime"] = testtime;
                                    }
                                }
                                else
                                {
                                    if (dt2.Rows.Count == 1)
                                    {
                                        string process = dt2.Rows[0][1].ToString();
                                        string linepass = dt2.Rows[0][2].ToString();
                                        DateTime testtime = (DateTime)dt2.Rows[0][3];
                                        if (process == "EN2-LVT" || process == "MOTOR")
                                            linepass = "FAIL";
                                        newrow["process"] = process;
                                        newrow["linepass"] = linepass;
                                        newrow["testtime"] = testtime;
                                    }
                                    else
                                    {
                                        string process = dt2.Rows[0][1].ToString();
                                        string linepass = dt2.Rows[0][2].ToString();
                                        string process2 = dt2.Rows[1][1].ToString();
                                        string linepass2 = dt2.Rows[1][2].ToString();
                                        string resultprocess = process + "," + process2;
                                        string resultlinepass = "";
                                        if (linepass == "PASS" && linepass2 == "PASS")
                                            resultlinepass = "PASS";
                                        if (linepass == "FAIL" && linepass2 == "PASS")
                                            resultlinepass = "FAIL";
                                        if (linepass == "PASS" && linepass2 == "FAIL")
                                            resultlinepass = "FAIL";
                                        if (linepass == "FAIL" && linepass2 == "FAIL")
                                            resultlinepass = "FAIL";

                                        DateTime testtime = (DateTime)dt2.Rows[0][3];
                                        //newrow["process"] = process;
                                        //newrow["linepass"] = linepass;
                                        newrow["process"] = resultprocess;
                                        newrow["linepass"] = resultlinepass;
                                        newrow["testtime"] = testtime;
                                    }
                                }
                            }

                            else
                            {
                                string process = dt2.Rows[0][1].ToString();
                                string linepass = dt2.Rows[0][2].ToString();
                                DateTime testtime = (DateTime)dt2.Rows[0][3];
                                newrow["process"] = process;
                                newrow["linepass"] = linepass;
                                newrow["testtime"] = testtime;
                            }
                        }
                        #endregion
                        // Add the row to the datatable
                        dtOverall.Rows.Add(newrow);

                        //Set limit
                        if (dtOverall.Rows.Count == 1)
                        {
                            string m = dtOverall.Rows[0]["model"].ToString();
                            switch (m)
                            {
                                case "LS4A":
                                    limit = limitls4a;
                                    break;
                                case "LS4D":
                                    limit = limitls4a;
                                    break;
                                case "LS3D":
                                case "LS3E":
                                case "LS3F":
                                case "LS3J":
                                case "LS3K":
                                case "LS3L":
                                case "LS3P":
                                case "LMOD":
                                case "BMS69":
                                case "BMS_0314":
                                case "BMS58":
                                case "BMS57":
                                case "BMS70":
                                case "BMS14":
                                case "BMS15":
                                    limit = limitls12;
                                    break;
                                case "LA10":
                                    limit = limitlaa;
                                    break;
                                default:
                                    limit = 9999;
                                    break;
                            }
                            //if (m == "LS4A") limit = limitls4a;
                            //else if (m == "LS3D" || m == "LS3E" || m == "LS3F" || m == "LS3J" || m == "LS3K" || m == "LS3L") limit = limitls12;
                            //else if (m == "LA10") limit = limitlaa;
                            //else limit = 9999;

                            // ‚t‚r‚d‚q‚X‚ª‚k‚h‚l‚h‚s‚ðÝ’è‚µ‚½ê‡‚ÍA‚»‚ê‚É]‚¤
                            if (limit1 != 0) limit = limit1;
                        }
                        // ƒf[ƒ^ƒOƒŠƒbƒgƒrƒ…[‚ÌXV
                        updateDataGripViews(dtOverall, ref dgvProductSerial);
                    }

                    // For the operator to continue scanning, enable the scan text box and select the text in the box
                    if (okCount >= limit)
                    {
                        txtProductSerial.Enabled = false;
                        btnUpdate.Enabled = true;
                        //btnRegisterBoxId.Visible = true;
                        //btnRegisterBoxId.Enabled = true;
                    }
                    else
                    {
                        txtProductSerial.Enabled = true;
                        txtProductSerial.Focus();
                        txtProductSerial.SelectAll();
                    }
                }
            }
        }

        // Select datatable
        private string decideReferenceTable(string serno, DateTime tbldate)
        {
            string tablekey = string.Empty;
            string filterkey = string.Empty;
            switch (VBStrings.Mid(serno, 3, 2))
            {
                case "4A":
                    tablekey = "ls12_004a"; filterkey = "LS4A";
                    break;
                case "4D":
                    tablekey = "ls12_004d"; filterkey = "LS4D";
                    break;
                case "3D":
                    tablekey = "ls12_003d"; filterkey = "LS3D";
                    break;
                case "3E":
                    tablekey = "ls12_003e"; filterkey = "LS3E";
                    break;
                case "3F":
                    tablekey = "ls12_003f"; filterkey = "LS3F";
                    break;
                case "3K":
                    tablekey = "ls12_003k"; filterkey = "LS3K";
                    break;
                case "3J":
                    tablekey = "ls12_003j"; filterkey = "LS3J";
                    break;
                case "3P":
                    tablekey = "ls12_003p"; filterkey = "LS3P";
                    break;

                default:
                    if (serno.Length == 8) tablekey = "laa10_003"; filterkey = "LA10";
                    if (serno.Length == 13)
                    {
                        if (cmbModel.Text == "BMS69")
                        {
                            tablekey = "bms_0069";
                            filterkey = "BMS69";
                        }
                        else if (cmbModel.Text == "BMS70")
                        {
                            tablekey = "bms_0070";
                            filterkey = "BMS70";
                        }
                        else if (cmbModel.Text == "BMS58")
                        {
                            tablekey = "bms_0058";
                            filterkey = "BMS58";
                        }
                        else if (cmbModel.Text == "BMS57")
                        {
                            tablekey = "bms_0057";
                            filterkey = "BMS57";
                        }
                        else if (cmbModel.Text == "BMS14")
                        {
                            tablekey = "bms_0214";
                            filterkey = "BMS14";
                        }
                        else if (cmbModel.Text == "BMS15")
                        {
                            tablekey = "bms_0215";
                            filterkey = "BMS15";
                        }
                        else if (cmbModel.Text == "BMS_0240")
                        {
                            tablekey = "bms_0240";
                            filterkey = "BMS_0240";
                        }
                        else if (cmbModel.Text == "BMS_0314")
                        {
                            tablekey = "bms_0314";
                            filterkey = "BMS_0314";
                        }
                    }
                    if (VBStrings.Mid(serno, 6, 1) == "L") tablekey = "ls12_003l"; filterkey = "LS3L";
                    if (VBStrings.Left(serno, 1) == "M") tablekey = "ls12_003mod"; filterkey = "LMOD";
                    break;
            }
            //if (VBStrings.Mid(serno, 3, 2) == "3D")
            //{ tablekey = "ls12_003d"; filterkey = "LS3D"; }
            //else if (VBStrings.Mid(serno, 3, 2) == "3E")
            //{ tablekey = "ls12_003e"; filterkey = "LS3E"; }
            //else if (VBStrings.Mid(serno, 3, 2) == "3F")
            //{ tablekey = "ls12_003f"; filterkey = "LS3F"; }
            //else if (VBStrings.Mid(serno, 3, 2) == "3J")
            //{ tablekey = "ls12_003j"; filterkey = "LS3J"; }
            //else if (VBStrings.Mid(serno, 3, 2) == "3K")
            //{ tablekey = "ls12_003k"; filterkey = "LS3K"; }
            //else if (VBStrings.Mid(serno, 3, 2) == "4A")
            //{ tablekey = "ls12_004a"; filterkey = "LS4A"; }
            //else if (VBStrings.Mid(serno, 3, 2) == "3L")
            //{ tablekey = "ls12_003l"; filterkey = "LS3L"; }
            //else if (serno.Length == 8)
            //{ tablekey = "laa10_003"; filterkey = "LA10"; }

            ShSQL sql = new ShSQL();
            //testerTableLastMonth = tablekey + ((VBStrings.Right(DateTime.Today.ToString("yyyyMM"), 2) != "01") ?
            //    (long.Parse(DateTime.Today.ToString("yyyyMM")) - 1).ToString() : (long.Parse(DateTime.Today.ToString("yyyy")) - 1).ToString() + "12");
            int n = 3;
        B:
            testerTableThisMonth = tablekey + tbldate.ToString("yyyyMM");
            if (!sql.CheckTableExist(testerTableThisMonth) && n > 0)
            {
                n--;
                tbldate = tbldate.AddMonths(-1);
                goto B;
            }
            n = 5;
        C:
            testerTableLastMonth = tablekey + tbldate.AddMonths(-1).ToString("yyyyMM");
            if (!sql.CheckTableExist(testerTableLastMonth) && n > 0)
            {
                n--;
                tbldate = tbldate.AddMonths(-1);
                goto C;
            }
            else if (n == 0)
                testerTableLastMonth = testerTableThisMonth;
            return filterkey;
        }
        // Issue new box id, register product serials, and save text file for barcode printing
        private void btnRegisterBoxId_Click(object sender, EventArgs e)
        {
            btnRegisterBoxId.Enabled = false;
            btnDeleteSelection.Enabled = false;
            btnDeleteAll.Enabled = false;
            btnCancel.Enabled = false;
            if (m_model == "LA10") directory = @"Z:\(01)Motor\(00)Public\11-Suka-Sugawara\LD model\printer\print";

            string boxId = txtBoxId.Text;

            // If this form' mode is not for EDIT, this botton works for RE-PRINTING barcode label
            if (!formEditMode)
            {
                // ƒo[ƒR[ƒhƒtƒ@ƒCƒ‹‚Ì¶¬
                m_lot = dtOverall.Rows[0]["lot"].ToString();
                printBarcode(directory, boxId, m_model, m_lot, dgvDateCode, ref dgvDateCode2, ref txtBoxIdPrint);
                btnRegisterBoxId.Enabled = true;
                btnCancel.Enabled = true;
                return;
            }

            // Check if the product serials had already registered in the database table
            string checkResult = checkDataTableWithRealTable(dtOverall);

            if (checkResult != String.Empty)
            {
                MessageBox.Show("The following serials are already registered with box id:" + Environment.NewLine +
                    checkResult + Environment.NewLine + "Please check and delete.", "Notice",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                btnRegisterBoxId.Enabled = true;
                btnDeleteSelection.Enabled = true;
                btnDeleteAll.Enabled = true;
                btnCancel.Enabled = true;
                return;
            }

            // Issue new box id
            string boxIdNew = getNewBoxId();

            // As the first step, add new box id information to the product serial datatable
            DataTable dt = dtOverall.Copy();
            dt.Columns.Add("boxid", Type.GetType("System.String"));
            for (int i = 0; i < dt.Rows.Count; i++)
                dt.Rows[i]["boxid"] = boxIdNew;

            // As the second step, register datatables' each record into database table
            ShSQL tf = new ShSQL();
            bool res1 = tf.sqlMultipleInsertOverall(dt);
            bool res2 = true; //tf.sqlMultipleInsertNoise(dtSI);

            if (res1 & res2)
            {
                // Print barcode
                printBarcode(directory, boxIdNew, m_lot, m_model, dgvDateCode, ref dgvDateCode2, ref txtBoxIdPrint);
                // Clear the datatable
                dtOverall.Clear();
                dt = null;

                txtBoxId.Text = boxIdNew;
                if (m_model == "BMS69" || m_model == "BMS70" || m_model == "BMS58" || m_model == "BMS57" || m_model == "BMS14" || m_model == "BMS15")

                    dtpPrintDate.Value = DateTime.ParseExact(VBStrings.Mid(boxIdNew, 7, 6), "yyMMdd", CultureInfo.InvariantCulture);
               else if (m_model == "BMS_0240"|| m_model == "BMS_0314")

                    dtpPrintDate.Value = DateTime.ParseExact(VBStrings.Mid(boxIdNew, 10, 6), "yyMMdd", CultureInfo.InvariantCulture);
                else
                    dtpPrintDate.Value = DateTime.ParseExact(VBStrings.Mid(boxIdNew, 6, 6), "yyMMdd", CultureInfo.InvariantCulture);
                // Generate delegate event to update parant form frmBoxid's datagridview (box id list)
                this.RefreshEvent(this, new EventArgs());

                this.Focus();
                MessageBox.Show("The box id " + boxIdNew + " and " + Environment.NewLine +
                    "its product serials were registered.", "Process Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtBoxId.Text = String.Empty;
                txtProductSerial.Text = String.Empty;
                updateDataGripViews(dtOverall, ref dgvProductSerial);
                btnRegisterBoxId.Enabled = false;
                btnDeleteSelection.Enabled = true;
                btnDeleteAll.Enabled = false;
                btnCancel.Enabled = true;
            }
            else
            {
                MessageBox.Show("Box id and product serials were not registered." + System.Environment.NewLine +
                @"Please try again by clicking ""Register Box ID"".", "Process Result", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnRegisterBoxId.Enabled = true;
                btnDeleteSelection.Enabled = true;
                btnDeleteAll.Enabled = true;
                btnCancel.Enabled = true;
            }
        }

        // Sub procedure: Check if datatable's product serial is included in the database table
        // (actually, database itself blocks the duplicate, so this process is not needed)
        private string checkDataTableWithRealTable(DataTable dt1)
        {
            string result = String.Empty;

            string sql = "select serial_short, boxid " +
                 "FROM product_serial_printdate WHERE testtime BETWEEN '" + System.DateTime.Today.AddDays(-7) + "' AND '" + System.DateTime.Today.AddDays(1) + "'";

            DataTable dt2 = new DataTable();
            ShSQL tf = new ShSQL();
            tf.sqlDataAdapterFillDatatable(sql, ref dt2);

            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                string serial = VBStrings.Left(dt1.Rows[i]["serialno"].ToString(), 17);
                DataRow[] dr = dt2.Select("serial_short = '" + serial + "'");
                if (dr.Length >= 1)
                {
                    string boxid = dr[0]["boxId"].ToString();
                    result += (i + 1 + ": " + serial + " / " + boxid + Environment.NewLine);
                }
            }

            if (result == String.Empty)
            {
                return String.Empty;
            }
            else
            {
                return result;
            }
        }

        // Sub procedure: Issue new box id
        private string getNewBoxId()
        {
            m_model = dtOverall.Rows[0]["model"].ToString();
            string sql = "select MAX(boxid) FROM box_id where boxid like '" + m_model + "%'";
            System.Diagnostics.Debug.Print(sql);
            ShSQL yn = new ShSQL();
            string boxIdOld = yn.sqlExecuteScalarString(sql);

            DateTime dateOld = new DateTime(0);
            //  long numberOld = 0;
            long numberOld = 0;
            string boxIdNew;
            if (m_model == "BMS69" || m_model == "BMS70" || m_model == "BMS58" || m_model == "BMS57" || m_model == "BMS14" || m_model == "BMS15")
            {
                if (!string.IsNullOrEmpty(boxIdOld))
                {
                    dateOld = DateTime.ParseExact(VBStrings.Mid(boxIdOld, 7, 6), "yyMMdd", CultureInfo.InvariantCulture);
                    numberOld = long.Parse(VBStrings.Right(boxIdOld, 3));
                }
                if (dateOld != DateTime.Today)
                {
                    boxIdNew = m_model + "-" + DateTime.Today.ToString("yyMMdd") + "001";
                }
                else
                {
                    boxIdNew = m_model + "-" + DateTime.Today.ToString("yyMMdd") + (numberOld + 1).ToString("000");
                }
            }
            else if(m_model == "BMS_0314")
            {
                if (boxIdOld != string.Empty)
                {
                    dateOld = DateTime.ParseExact(VBStrings.Mid(boxIdOld, 10, 6), "yyMMdd", CultureInfo.InvariantCulture);
                    numberOld = long.Parse(VBStrings.Right(boxIdOld, 3));
                }
                if (dateOld != DateTime.Today)
                {
                    boxIdNew = m_model + "-" + DateTime.Today.ToString("yyMMdd") + "001";
                }
                else
                {
                    boxIdNew = m_model + "-" + DateTime.Today.ToString("yyMMdd") + (numberOld + 1).ToString("000");
                }
            }
            else
            {
                if (boxIdOld != string.Empty)
                {
                    dateOld = DateTime.ParseExact(VBStrings.Mid(boxIdOld, 6, 6), "yyMMdd", CultureInfo.InvariantCulture);
                    numberOld = long.Parse(VBStrings.Right(boxIdOld, 3));
                }
                if (dateOld != DateTime.Today)
                {
                    boxIdNew = m_model + "-" + DateTime.Today.ToString("yyMMdd") + "001";
                }
                else
                {
                    boxIdNew = m_model + "-" + DateTime.Today.ToString("yyMMdd") + (numberOld + 1).ToString("000");
                }
            }
            sql = "INSERT INTO box_id(" +
                "boxid," +
                "suser," +
                "printdate) " +
                "VALUES(" +
                "'" + boxIdNew + "'," +
                "'" + user + "'," +
                "'" + DateTime.Now.ToString() + "')";
            System.Diagnostics.Debug.Print(sql);
            yn.sqlExecuteNonQuery(sql, false);
            return boxIdNew;
        }

        // Sub procedure: Print barcode, by generating a text file in shared folder and let another application print it
        private void printBarcode(string dir, string id, string m_lot, string m_model, DataGridView dgv1, ref DataGridView dgv2, ref TextBox txt)
        {
            ShPrint tf = new ShPrint();
            tf.createBoxidFiles(dir, id, m_lot, m_model, dgv1, ref dgv2, ref txt);
        }

        // Delete records on datagridview selected by the user
        private void btnDeleteSelection_Click(object sender, EventArgs e)
        {
            // ƒZƒ‹‚Ì‘I‘ð”ÍˆÍ‚ª‚Q—ñˆÈã‚Ìê‡‚ÍAƒƒbƒZ[ƒW‚Ì•\Ž¦‚Ì‚Ý‚ÅƒvƒƒV[ƒWƒƒ‚ð”²‚¯‚é
            if (dgvProductSerial.Columns.GetColumnCount(DataGridViewElementStates.Selected) >= 2)
            {
                MessageBox.Show("Please select range with only one columns.", "Notice",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                return;
            }

            DialogResult result = MessageBox.Show("Do you really want to delete the selected rows?",
                "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (result == DialogResult.Yes)
            {
                foreach (DataGridViewCell cell in dgvProductSerial.SelectedCells)
                {
                    int i = cell.RowIndex;
                    dtOverall.Rows[i].Delete();
                }
                dtOverall.AcceptChanges();
                updateDataGripViews(dtOverall, ref dgvProductSerial);
                txtProductSerial.Focus();
            }
        }

        // Delete all records on datagridview, by the user's click on the delete all button
        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            int rowCount = dgvProductSerial.Rows.Count;
            if (rowCount != 0)
            {
                DialogResult result = MessageBox.Show("Do you really want to delete all the record?",
                    "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Yes)
                {
                    dtOverall.Clear();
                    dtOverall.AcceptChanges();
                    updateDataGripViews(dtOverall, ref dgvProductSerial);
                    txtProductSerial.Focus();
                }
            }
        }

        // Change the capacity of the box (only for the super user)
        private void btnChangeLimit_Click(object sender, EventArgs e)
        {
            // Open frmCapacity with delegate event
            bool bl = ShGeneral.checkOpenFormExists("frmCapacity");
            if (bl)
            {
                MessageBox.Show("Please close or complete another form.", "Warning",
                MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            }
            else
            {
                frmCapacity f4 = new frmCapacity();
                // When the delegate event is triggered by child, update this form's datagridview
                f4.RefreshEvent += delegate (object sndr, EventArgs excp)
                {
                    limit = f4.getLimit();
                    txtLimit.Text = limit.ToString();
                    updateDataGripViews(dtOverall, ref dgvProductSerial);
                    this.Focus();
                };

                f4.updateControls(limit.ToString());
                f4.Show();
            }
        }

        // Replace a product serial in already registered box id (only for super user)
        private void btnReplace_Click(object sender, EventArgs e)
        {
            // If more than 2 columns or more than 2 records are selected, leave the procedure
            if (dgvProductSerial.Columns.GetColumnCount(DataGridViewElementStates.Selected) >= 2 ||
                    dgvProductSerial.Rows.GetRowCount(DataGridViewElementStates.Selected) >= 2 ||
                    dgvProductSerial.CurrentCell.ColumnIndex != 0)
            {
                MessageBox.Show("Please select only one serial number.", "Notice",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                return;
            }

            // Open form frmModuleReplace with delegate function
            bool bl = ShGeneral.checkOpenFormExists("frmModuleReplace");
            if (bl)
            {
                MessageBox.Show("Please close or complete another form.", "Warning",
                MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                return;
            }

            updateRowIndex = dgvProductSerial.CurrentCell.RowIndex;
            frmModuleReplace f2 = new frmModuleReplace();
            // Update this form's datagridview when child's delegate event is triggered
            f2.RefreshEvent += delegate (object sndr, EventArgs excp)
            {
                dtOverall.Clear();
                readDatatable(ref dtOverall);
                updateDataGripViews(dtOverall, ref dgvProductSerial);
                dgvProductSerial.CurrentCell = dgvProductSerial[0, updateRowIndex];
                dgvProductSerial.FirstDisplayedScrollingRowIndex = updateRowIndex;
                this.Focus();
            };

            string curSerial = dgvProductSerial.CurrentCell.Value.ToString();
            int curRowIndex = dgvProductSerial.CurrentRow.Index;
            f2.updateControls(curSerial, curRowIndex + 1);
            f2.Show();
        }

        // Delete box is and its product module data(done by only the user user)
        private void btnDeleteBoxId_Click(object sender, EventArgs e)
        {
            // Ask 2 times to the user for check
            DialogResult result1 = MessageBox.Show("Do you really delete this box id's all the serial data?",
                "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (result1 == DialogResult.Yes)
            {
                DialogResult result2 = MessageBox.Show("Are you really sure? Please select NO if you are not sure.",
                    "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (result2 == DialogResult.Yes)
                {
                    string boxid = txtBoxId.Text;
                    string sql = "delete from product_serial where boxid = '" + boxid + "'";
                    string sql1 = "delete from box_id where boxid = '" + boxid + "'";
                    ShSQL tf = new ShSQL();
                    tf.sqlExecuteNonQuery(sql, true);
                    tf.sqlExecuteNonQuery(sql1, true);

                    dtOverall.Clear();
                    // Update datagridviw
                    updateDataGripViews(dtOverall, ref dgvProductSerial);
                }
            }
        }

        // Sub procedure: Read product serial records from database to datatable
        private void readDatatable(ref DataTable dt)
        {
            string boxId = txtBoxId.Text;
            string sql = "select serialno, lot, fact, process, linepass, testtime " +
                "FROM product_serial WHERE boxid='" + boxId + "'";
            ShSQL tf = new ShSQL();
            tf.sqlDataAdapterFillDatatable(sql, ref dt);
        }

        // When cancel button is clicked, let the user check if it is OK that the records are deleted in add mode.
        private void btnCancel_Click(object sender, EventArgs e)
        {
            // When frmCapacity is remaining open, let the user close it
            string formName = "frmCapacity";
            bool bl = false;
            foreach (Form buff in Application.OpenForms)
            {
                if (buff.Name == formName) { bl = true; }
            }
            if (bl)
            {
                MessageBox.Show("You need to close another form before canceling.", "Notice",
                  MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                return;
            }

            // When frmModuleReplace is remaining open, let the user close it
            formName = "frmModuleReplace";
            bl = false;
            foreach (Form buff in Application.OpenForms)
            {
                if (buff.Name == formName) { bl = true; }
            }
            if (bl)
            {
                MessageBox.Show("You need to close another form before canceling..", "Notice",
                  MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                return;
            }

            // If there is no record in the datatable or the form is opened as for view, let the user close the form
            if (dtOverall.Rows.Count == 0 || !formEditMode)
            {
                Application.OpenForms["frmBoxid"].Focus();
                Close();
                return;
            }

            // Show alarm that all the temporary records in datatable will be completely deleted
            DialogResult result = MessageBox.Show("The current serial data has not been saved." + System.Environment.NewLine +
                "Do you rally cancel?", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (result == DialogResult.Yes)
            {
                dtOverall.Clear();
                updateDataGripViews(dtOverall, ref dgvProductSerial);
                MessageBox.Show("The temporary serial numbers are deleted.", "Notice",
                    MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
                Application.OpenForms["frmBoxid"].Focus();
                Close();
            }
            else
            {
                return;
            }
        }


        // Do not allow user to close this form by right top close button or by Alt+F4 shor cut
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        //protected override void WndProc(ref Message m)
        //{
        //    const int WM_SYSCOMMAND = 0x112;
        //    const long SC_CLOSE = 0xF060L;
        //    if (m.Msg == WM_SYSCOMMAND && (m.WParam.ToInt64() & 0xFFF0L) == SC_CLOSE) { return; }
        //    base.WndProc(ref m);
        //}

        // Play the MP3 file for alarming users
        [System.Runtime.InteropServices.DllImport("winmm.dll")]
        private static extern int mciSendString(String command,
           StringBuilder buffer, int bufferSize, IntPtr hwndCallback);

        private string aliasName = "MediaFile";

        private void soundAlarm()
        {
            string currentDir = System.Environment.CurrentDirectory;
            string fileName = currentDir + @"\warning.mp3";
            string cmd;

            if (sound)
            {
                cmd = "stop " + aliasName;
                mciSendString(cmd, null, 0, IntPtr.Zero);
                cmd = "close " + aliasName;
                mciSendString(cmd, null, 0, IntPtr.Zero);
                sound = false;
            }

            cmd = "open \"" + fileName + "\" type mpegvideo alias " + aliasName;
            if (mciSendString(cmd, null, 0, IntPtr.Zero) != 0) return;
            cmd = "play " + aliasName;
            mciSendString(cmd, null, 0, IntPtr.Zero);
            sound = true;
        }

        // Sub procedure: Check the records in dataview for debug
        private void printDataView(DataView dv)
        {
            foreach (DataRowView drv in dv)
            {
                System.Diagnostics.Debug.Print(drv["process"].ToString() + ": " +
                    drv["judge"].ToString() + ": " + drv["inspectdate"].ToString());
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {

            string sqlupdate = "UPDATE product_serial set serialno = '" + txtProductSerial.Text + "' where boxid = '" + txtBoxId.Text + "' and model = '" + cmbModel.Text + "' and serialno = '" + txtProduc.Text + "'";
            ShSQL con = new ShSQL();
            con.sqlExecuteScalarString(sqlupdate);
            MessageBox.Show("Update Finish", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void dgvProductSerial_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            txtProduc.Text = dgvProductSerial.CurrentRow.Cells[0].Value.ToString();
            MessageBox.Show("Choose Barcode " + txtProduc.Text + "", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ckReturn_CheckedChanged(object sender, EventArgs e)
        {

        }

    }
}