using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;
using BoxIdDb;

namespace BoxIdDB
{
    public partial class FormLPD : Form
    {
        public delegate void RefreshEventHandler(object sender, EventArgs e);
        public event RefreshEventHandler RefreshEvent;
        //    static string conStringBoxidDb = @"Server=192.168.145.4;Port=5432;User Id=pqm;Password=dbuser;Database=boxidlpddb; CommandTimeout=100; Timeout=100;";

        // The variable for degignate the shared floder to save text files for printing, 
        // which is to be printed by separate printing application
        string appconfig = System.Environment.CurrentDirectory + "\\info.ini";
        //string directory = @"Z:\(01)Motor\(00)Public\11-Suka-Sugawara\LD model\printer\print\";
        string directory = @"Z:\(01)Motor\(00)Public\LPD model\printer\print\";
        // Other global variables
        bool formEditMode;
        string user;
        string config;
        string m_model;
        int okCount;
        bool inputBoxModeOriginal;
        string testerTableThisMonth;
        string testerTableLastMonth;
        DataTable dtOverall;
        DataTable dtReplace;
        public int limit1 = 0;
        int limit = 100;
        int limitlpd10g111 = 100;
        int limitlpd10ge107 = 100;
        //int limitlaa = 3000;
        int updateRowIndex;
        bool sound;
        //string fltld4 = "process = 'LD4-LVT'";
        string fltlpd10g111 = "process in ('MARKING','MOTOR')";
        string fltlpd10ge107 = "process = 'QCS'";
        public FormLPD()
        {
            InitializeComponent();

        }

        private void FormLPD_Load(object sender, EventArgs e)
        {
            // Store user name to the variable
            user = txtUser.Text;

            // Show box capacity in the text box
            txtLimit.Text = limit1.ToString();
            txtOkCount.Text = okCount + "/" + limit;

            // Show box capacity in the text box
            if (!String.IsNullOrEmpty(txtBoxId.Text))
            {
                string[] mol = txtBoxId.Text.Split('-');
                string mdl = mol[0];
                switch (mdl)
                {
                    case "LPD10G111":
                        limit = limitlpd10g111;
                        break;
                    case "LPD10GE107":
                        limit = limitlpd10ge107;
                        break;
                    //case "LA10":
                    //    limit = limitlaa;
                    //    break;
                    default:
                        limit = 100;
                        break;
                }
            }
            // Get the printing folder directory from the application setting file and store it to the variable
            //directory = @"Z:\(01)Motor\(00)Public\11-Suka-Sugawara\LD model\printer\print\";
            directory = @"Z:\(01)Motor\(00)Public\11-Suka-Sugawara\LD model\printer\print\";
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

            txtProductSerial.Enabled = editMode;
            //btnRegisterBoxId.Enabled = !editMode;
            btnDeleteAll.Visible = editMode;
            //btnDeleteSelection.Visible = editMode;
            formEditMode = editMode;
            this.Text = editMode ? "Product Serial - Edit Mode" : "Product Serial - Browse Mode";
            btnRegisterBoxId.Text = editMode ? "Register Box ID" : "Register Again";
            btnPrint.Text = editMode ? "Print Box ID" : "Re_Print";

            if (user == "User_9")
            {
                btnChangeLimit.Visible = true;
                txtLimit.Visible = true;
            }
            if (!editMode && user == "User_9")
            {
                //btnChangeLimit.Enabled = false;
                ckbDeleteBox.Visible = true;
                if (ckbDeleteBox.Checked == true) btnDeleteBoxId.Visible = true;
            }
        }
        private void defineAndReadDtOverall(ref DataTable dt)
        {
            string boxId = txtBoxId.Text;

            dt.Columns.Add("id", Type.GetType("System.String"));
            dt.Columns.Add("serialno", Type.GetType("System.String"));
            dt.Columns.Add("model", Type.GetType("System.String"));
            dt.Columns.Add("lot", Type.GetType("System.String"));
            dt.Columns.Add("line", Type.GetType("System.String"));
            // dt.Columns.Add("config", Type.GetType("System.String"));
            dt.Columns.Add("process", Type.GetType("System.String"));
            dt.Columns.Add("linepass", Type.GetType("System.String"));
            dt.Columns.Add("testtime", Type.GetType("System.DateTime"));

            if (!formEditMode)
            {
                string sql = "select id, serialno, lot, line, process, linepass, testtime, model " +
                    "FROM product_serial WHERE boxid='" + boxId + "' order by serialno";
                ShSQL tf = new ShSQL();
                tf.sqlDataAdapterFillDatatable(sql, ref dt);
            }
        }
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
            //   colorMixedConfig(dt1, ref dgv1);

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
            txtOkCount.Text = okCount.ToString();

            // If the OK record count has already reached to the capacity, disenable the scan text box
            if (okCount == limit)
                txtProductSerial.Enabled = false;
            else
                txtProductSerial.Enabled = true;

            // If the OK record coutn has already reached to the capacity, enable the register button
            if (okCount == limit && dgv1.Rows.Count == limit)
                btnRegisterBoxId.Enabled = true;
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
        private void updateDataGripViewsSub(DataTable dt1, ref DataGridView dgv1)
        {
            dgv1.DataSource = dt1;
            dgv1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            //string[] criteriaLine = { "1", "2", "3", "4", "Total" };
            //makeDatatableSummary(dt1, ref dgvLine, criteriaLine, "line");

            //string[] criteriaConfig = { "01", "02", "1C", "1D", "Total" };
            //makeDatatableSummary(dt1, ref dgvConfig, criteriaConfig, "config");

            string[] criteriaPassFail = { "PASS", "FAIL", "Total" };
            makeDatatableSummary(dt1, ref dgvPassFail, criteriaPassFail, "linepass");

            string[] criteriaDateCode = getLotArray(dt1);
            makeDatatableSummary(dt1, ref dgvDateCode, criteriaDateCode, "lot");
        }
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
        private void colorViewForDuplicateSerial(ref DataGridView dgv)
        {
            DataTable dt = ((DataTable)dgv.DataSource).Copy();
            if (dt.Rows.Count <= 0) return;

            for (int i = 0; i < dtOverall.Rows.Count; i++)
            {
                string serial = dgv[1, i].Value.ToString();
                DataRow[] dr = dt.Select("serialno = '" + serial + "'");
                if (dr.Length >= 2 || dgv[1, i].Value.ToString().Length > 13 || dgv[1, i].Value.ToString().Length < 13)
                {
                    dgv[1, i].Style.BackColor = Color.Red;
                    soundAlarm();
                }
                else
                {
                    dgv[0, i].Style.BackColor = Color.FromKnownColor(KnownColor.Window);
                }
            }
            //    DataTable dt = ((DataTable)dgv.DataSource).Copy();
            //    if (dt.Rows.Count <= 0) return;

            //    for (int i = 0; i < dtOverall.Rows.Count; i++)
            //    {
            //        string serial = dgv[0, i].Value.ToString();
            //        DataRow[] dr = dt.Select("serialno = '" + serial + "'");
            //        if (dr.Length >= 2 || dgv[0, i].Value.ToString().Length != 13 && dgv[0, i].Value.ToString().Length != 8 && dgv[0, i].Value.ToString().Length != 10)
            //        {
            //            dgv[0, i].Style.BackColor = Color.Red;
            //            soundAlarm();
            //        }
            //        else
            //        {
            //            dgv[0, i].Style.BackColor = Color.FromKnownColor(KnownColor.Window);
            //        }
            //    }
        }
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

        private void txtProductSerial_KeyDown(object sender, KeyEventArgs e)
        {
            #region OLD
            //if (e.KeyCode == Keys.Enter)
            //{
            //    // Disenalbe the textbox to block scanning
            //    txtProductSerial.Enabled = false;

            //    string serLong = txtProductSerial.Text;
            //    string serShort = serLong;
            //    string filterkey = decideReferenceTable(serShort);
            //    if (serLong != String.Empty)
            //    {
            //        // Get the tester data from current month's table and store it in datatable
            //        string sql = "SELECT serno, process, tjudge, inspectdate" +
            //            " FROM " + testerTableThisMonth +
            //            " WHERE serno = '" + serShort + "'";
            //        DataTable dt1 = new DataTable();
            //        ShSQL tf = new ShSQL();
            //        tf.sqlDataAdapterFillDatatableFromTesterDb(sql, ref dt1);

            //        System.Diagnostics.Debug.Print(sql);

            //        // Get the tester data from last month's table and store it in the same datatable
            //        sql = "SELECT serno, process, tjudge, inspectdate" +
            //            " FROM " + testerTableLastMonth +
            //            " WHERE serno = '" + serShort + "'";
            //        tf.sqlDataAdapterFillDatatableFromTesterDb(sql, ref dt1);

            //        System.Diagnostics.Debug.Print(sql);

            //        string filterLine = string.Empty;
            //        if (filterkey == "LPD10G111")
            //            filterLine = fltlpd10g111;
            //        if (filterkey == "LPD10GE107")
            //            filterLine = fltlpd10ge107;

            //        DataView dv = new DataView(dt1);
            //        dv.RowFilter = filterLine;
            //        dv.Sort = "tjudge, inspectdate desc";
            //        System.Diagnostics.Debug.Print(System.Environment.NewLine + "In-Line:");
            //        printDataView(dv);
            //        DataTable dt2 = dv.ToTable();

            //        //‡@ƒCƒ“ƒ‰ƒCƒ“
            //        // ˆêŽžƒe[ƒuƒ‹‚Ö‚Ì“o˜^€”õ
            //        string lot = string.Empty;
            //        string line = string.Empty;
            //        string config = VBStrings.Mid(serShort, 1, 2);
            //        string model = string.Empty;
            //        //if (VBStrings.Mid(serLong, 1, 2) == "1C" || VBStrings.Mid(serLong, 1, 2) == "1D")
            //        //{ model = "LD04"; }
            //        //else if (serLong.Length == 8) model = "LA10";
            //        //else
            //        //{ model = "LD25"; }

            //        if (model == "LPD10G_111")
            //        {
            //            line = "1";
            //            lot = VBStrings.Mid(serShort, 1, 4);
            //        }
            //        else
            //        {
            //            line = "1";
            //            lot = VBStrings.Mid(serShort, 1, 4);
            //        }



            //        // Even when no tester data is found, the module have to appear in the datagridview
            //        DataRow newrow = dtOverall.NewRow();
            //        newrow["serialno"] = serLong;
            //        newrow["model"] = model;
            //        newrow["lot"] = lot;
            //        newrow["line"] = line;
            //        //  newrow["config"] = config;

            //        // If tester data exists, show it in the datagridview
            //        if (dt2.Rows.Count != 0)
            //        {
            //            string process = dt2.Rows[0][1].ToString();
            //            string linepass = String.Empty;
            //            string buff = dt2.Rows[0][2].ToString();
            //            if (buff == "0") linepass = "PASS";
            //            else if (buff == "1") linepass = "FAIL";
            //            else linepass = "ERROR";
            //            DateTime testtime = (DateTime)dt2.Rows[0][3];
            //            newrow["process"] = process;
            //            newrow["linepass"] = linepass;
            //            newrow["testtime"] = testtime;
            //        }

            //        // Add the row to the datatable
            //        dtOverall.Rows.Add(newrow);

            //        //Set limit
            //        if (dtOverall.Rows.Count >= 1)
            //        {
            //            string m = dtOverall.Rows[0]["model"].ToString();
            //            switch (m)
            //            {
            //                case "LPD10G111":
            //                    limit = limitlpd10g111;
            //                    break;
            //                case "LPD10GE107":
            //                    limit = limitlpd10ge107;
            //                    break;
            //                default:
            //                    limit = 100;
            //                    break;
            //            }

            //            txtLimit.Text = limit.ToString();
            //            // ‚t‚r‚d‚q‚X‚ª‚k‚h‚l‚h‚s‚ðÝ’è‚µ‚½ê‡‚ÍA‚»‚ê‚É]‚¤
            //            if (limit1 != 0) limit = limit1;
            //        }

            //        // ƒf[ƒ^ƒOƒŠƒbƒgƒrƒ…[‚ÌXV
            //        updateDataGripViews(dtOverall, ref dgvProductSerial);
            //    }

            //    // For the operator to continue scanning, enable the scan text box and select the text in the box
            //    if (okCount >= limit)
            //    {
            //        txtProductSerial.Enabled = false;
            //    }
            //    else
            //    {
            //        txtProductSerial.Enabled = true;
            //        txtProductSerial.Focus();
            //        txtProductSerial.SelectAll();
            //    }
            // }
            #endregion
            #region NEW 
            if (e.KeyCode == Keys.Enter)
            {
                // Disenalbe the extbox to block scanning
                txtProductSerial.Enabled = false;

                string serLong = txtProductSerial.Text;
                string serShort = serLong;
                string filterkey = decideReferenceTable(serShort);
                //switch (m_short)
                //{
                //    case "3L":
                serShort = serLong;
                //}
                DateTime sDate = DateTime.Today;
                A:
                // string filterkey = decideReferenceTable(serShort, sDate);
                if (serLong != String.Empty)
                {
                    // Get the tester data from current month's table and store it in datatable
                    string filterLine = string.Empty;
                    if (filterkey == "LPD10G111") { filterLine = fltlpd10g111; }
                    else filterLine = fltlpd10ge107;

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
                    //  string fact = "2A";
                    string model = string.Empty;
                    string line = "L1";
                    if (cmbModel.Text == "LPD10G_111")
                    {
                        model = "LPD10G_111";

                        lot = VBStrings.Mid(serShort, 4, 4);
                        limit = limitlpd10g111;

                    }
                    if (cmbModel.Text == "LPD10GE_107")
                    {
                        model = "LPD10G_107";
                        lot = VBStrings.Mid(serShort, 4, 4);
                        limit = limitlpd10ge107;
                    }
                    // Even when no tester data is found, the module have to appear in the datagridview
                    DataRow newrow = dtOverall.NewRow();
                    newrow["serialno"] = serLong;
                    newrow["model"] = model;
                    newrow["lot"] = lot;
                    newrow["line"] = line;
                    // newrow["fact"] = fact;
                    #region ADD new code
                    // If tester data exists, show it in the datagridview
                    if (dt2.Rows.Count != 0)
                    {
                        if (dt2.Rows.Count == 1)
                        {
                            string process = dt2.Rows[0][1].ToString();
                            string linepass = dt2.Rows[0][2].ToString();
                            DateTime testtime = (DateTime)dt2.Rows[0][3];
                            if (process == "MARKING" || process == "MOTOR")
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
                    #endregion
                    // Add the row to the datatable
                    dtOverall.Rows.Add(newrow);

                    //Set limit
                    if (dtOverall.Rows.Count == 1)
                    {
                        string m = dtOverall.Rows[0]["model"].ToString();
                        if (limit1 != 0) limit = limit1;
                    }
                    // ƒf[ƒ^ƒOƒŠƒbƒgƒrƒ…[‚ÌXV
                    updateDataGripViews(dtOverall, ref dgvProductSerial);

                    // For the operator to continue scanning, enable the scan text box and select the text in the box
                    if (okCount >= limit)
                    {
                        txtProductSerial.Enabled = false;

                    }
                    else
                    {
                        txtProductSerial.Enabled = true;
                        txtProductSerial.Focus();
                        txtProductSerial.SelectAll();
                    }
                }
            }
            #endregion
        }

        // Event when a module is scanned 
        private string decideReferenceTable(string serno)
        {
            string tablekey = string.Empty;
            string filterkey = string.Empty;
            if (cmbModel.Text == "LPD10G_111")
            { tablekey = "lpd10g_111"; filterkey = "LPD10G111"; }
            if (cmbModel.Text == "LPD10GE_107")
            { tablekey = "lpd10ge_107"; filterkey = "LPD10GE107"; }

            testerTableThisMonth = tablekey + DateTime.Today.ToString("yyyyMM");
            testerTableLastMonth = tablekey + ((VBStrings.Right(DateTime.Today.ToString("yyyyMM"), 2) != "01") ?
                (long.Parse(DateTime.Today.ToString("yyyyMM")) - 1).ToString() : (long.Parse(DateTime.Today.ToString("yyyy")) - 1).ToString() + "12");

            return filterkey;
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            string boxId = txtBoxId.Text;
            if (!formEditMode)
            {
                if (okCount == limit && dgvProductSerial.Rows.Count == limit)
                {
                    config = dtOverall.Rows[0]["config"].ToString();
                    printBarcode(directory, boxId, config, m_model, dgvDateCode, ref dgvDateCode2, ref txtBoxIdPrint);
                }
            }
            else
            {
                string boxIdNew = getNewBoxId();
                if (okCount == limit && dgvProductSerial.Rows.Count == limit)
                {
                    // Print barcode
                    printBarcode(directory, boxIdNew, config, m_model, dgvDateCode, ref dgvDateCode2, ref txtBoxIdPrint);

                    // Clear the datatable
                    dtOverall.Clear();

                    txtBoxId.Text = boxIdNew;
                    dtpPrintDate.Value = DateTime.ParseExact(VBStrings.Mid(boxIdNew, 6, 6), "yyMMdd", CultureInfo.InvariantCulture);
                }
            }
        }

        private void btnRegisterBoxId_Click(object sender, EventArgs e)
        {
            btnRegisterBoxId.Enabled = false;
            btnDeleteSelection.Enabled = false;
            btnDeleteAll.Enabled = false;
            btnCancel.Enabled = false;

            string boxId = txtBoxId.Text;

            // If this form' mode is not for EDIT, this botton works for RE-PRINTING barcode lable
            if (btnRegisterBoxId.Text == "Register Again")
            {
                DataTable dt_c = dtOverall.Copy();
                dt_c.Columns.Add("boxid", Type.GetType("System.String"));
                for (int i = 0; i < dt_c.Rows.Count; i++)
                {
                    dt_c.Rows[i]["boxid"] = txtBoxId.Text;
                }
                ShSQL sh = new ShSQL();
                bool res = sh.sqlMultipleInsert(dt_c);
                btnCancel.Enabled = true;
                MessageBox.Show("The box id " + txtBoxId.Text + " and " + Environment.NewLine +
                    "its product serials were registered.", "Process Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            string boxIdNew;
            if (btnRegisterBoxId.Text == "Register Box ID")
            {
                boxIdNew = getNewBoxId();
            }
            else boxIdNew = boxId;

            // As the first step, add new box id information to the product serial datatable
            DataTable dt = dtOverall.Copy();
            dt.Columns.Add("boxid", Type.GetType("System.String"));
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["boxid"] = boxIdNew;
            }
            // As the second step, register datatables' each record into database table
            ShSQL tf = new ShSQL();
            bool res1 = tf.sqlMultipleInsert(dt);
            bool res2 = false;

            if (VBStrings.Left(boxIdNew, 4) == "LA10" && txtOkCount.Text == "3000") { res2 = true; }
            if (res1 & res2)
            {
                if (okCount == limit && dgvProductSerial.Rows.Count == limit)
                {
                    // Print barcode
                    printBarcode(directory, boxIdNew, config, m_model, dgvDateCode, ref dgvDateCode2, ref txtBoxIdPrint);

                    // Clear the datatable
                    dtOverall.Clear();

                    txtBoxId.Text = boxIdNew;
                    dtpPrintDate.Value = DateTime.ParseExact(VBStrings.Mid(boxIdNew, 6, 6), "yyMMdd", CultureInfo.InvariantCulture);
                }
                // Generate delegate event to update parant form frmBoxid's datagridview (box id list)
                this.RefreshEvent(this, new EventArgs());

                this.Focus();
                MessageBox.Show("The box id " + boxIdNew + " and " + Environment.NewLine +
                    "its product serials were registered.", "Process Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtBoxId.Text = String.Empty;
                txtProductSerial.Text = String.Empty;
                updateDataGripViews(dtOverall, ref dgvProductSerial);
                btnRegisterBoxId.Enabled = false;
                btnPrint.Enabled = false;
                btnDeleteSelection.Enabled = false;
                btnDeleteAll.Enabled = false;
                btnCancel.Enabled = true;
            }
            else
            {
                MessageBox.Show("Box id and product serials were registered without print the label.", "Process Result", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //btnRegisterBoxId.Enabled = true;
                btnDeleteSelection.Enabled = true;
                btnDeleteAll.Enabled = true;
                btnCancel.Enabled = true;
            }
        }
        private string checkDataTableWithRealTable(DataTable dt1)
        {
            string result = String.Empty;

            string sql = "select serialno, boxid " +
                 "FROM product_serial WHERE testtime BETWEEN '" + System.DateTime.Today.AddDays(-7) + "' AND '" + System.DateTime.Today.AddDays(1) + "'";

            DataTable dt2 = new DataTable();
            ShSQL tf = new ShSQL();
            tf.sqlDataAdapterFillDatatableLPD(sql, ref dt2);

            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                string serial = VBStrings.Left(dt1.Rows[i]["serialno"].ToString(), 17);
                DataRow[] dr = dt2.Select("serialno = '" + serial + "'");
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
            string boxIdOld = yn.sqlExecuteScalarStringLPD(sql);

            DateTime dateOld = new DateTime(0);
            long numberOld = 0;
            string boxIdNew;

            //if (boxIdOld != string.Empty)
            //{
            //    dateOld = DateTime.ParseExact(VBStrings.Mid(boxIdOld, 7, 6), "yyMMdd", CultureInfo.InvariantCulture);
            //    numberOld = long.Parse(VBStrings.Right(boxIdOld, 3));
            //}
            if (dateOld != DateTime.Today)
            {
                boxIdNew = m_model + "-" + DateTime.Today.ToString("yyMMdd") + "001";
            }
            else
            {
                boxIdNew = m_model + "-" + DateTime.Today.ToString("yyMMdd") + (numberOld + 1).ToString("000");
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
            yn.sqlExecuteNonQueryLPD(sql, false);
            return boxIdNew;
        }

        // Sub procedure: Print barcode, by generating a text file in shared folder and let another application print it
        private void printBarcode(string dir, string id, string config, string m_model, DataGridView dgv1, ref DataGridView dgv2, ref TextBox txt)
        {
            ShPrint tf = new ShPrint();
            tf.createBoxidFiles(dir, id, config, m_model, dgv1, ref dgv2, ref txt);
        }
        public string check;
        // Delete records on datagridview selected by the user
        private void btnDeleteSelection_Click(object sender, EventArgs e)
        {
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

        private void btnChangeLimit_Click(object sender, EventArgs e)
        {
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
        private void defineAndReadDt(ref DataTable dt)
        {
            dt.Columns.Add("serialno", Type.GetType("System.String"));
            dt.Columns.Add("model", Type.GetType("System.String"));
            dt.Columns.Add("lot", Type.GetType("System.String"));
            dt.Columns.Add("line", Type.GetType("System.String"));
            // dt.Columns.Add("config", Type.GetType("System.String"));
            dt.Columns.Add("process", Type.GetType("System.String"));
            dt.Columns.Add("linepass", Type.GetType("System.String"));
            dt.Columns.Add("testtime", Type.GetType("System.DateTime"));
        }
        private void setSerialInfoAndTesterResult(string serLong)
        {
            string filterkey = decideReferenceTable(serLong);
            if (serLong != String.Empty)
            {
                // Get the tester data from current month's table and store it in datatable
                string sql = "SELECT serno, process, tjudge, inspectdate" +
                    " FROM " + testerTableThisMonth +
                    " WHERE serno = '" + serLong + "'";
                DataTable dt1 = new DataTable();
                ShSQL tf = new ShSQL();
                tf.sqlDataAdapterFillDatatableFromTesterDb(sql, ref dt1);

                System.Diagnostics.Debug.Print(sql);

                // Get the tester data from last month's table and store it in the same datatable
                sql = "SELECT serno, process, tjudge, inspectdate" +
                    " FROM " + testerTableLastMonth +
                    " WHERE serno = '" + serLong + "'";
                tf.sqlDataAdapterFillDatatableFromTesterDb(sql, ref dt1);

                System.Diagnostics.Debug.Print(sql);

                string filterLine = string.Empty;
                if (filterkey == "LPD10G111") { filterLine = fltlpd10g111; }
                else { filterLine = fltlpd10ge107; }

                DataView dv = new DataView(dt1);
                dv.RowFilter = filterLine;
                dv.Sort = "tjudge, inspectdate desc";
                System.Diagnostics.Debug.Print(System.Environment.NewLine + "In-Line:");
                printDataView(dv);
                DataTable dt2 = dv.ToTable();

                string lot = VBStrings.Mid(serLong, 3, 4);
                string line = VBStrings.Mid(serLong, 7, 1);
                string config = VBStrings.Mid(serLong, 1, 2);
                string model = string.Empty;
                if (cmbModel.Text == "LPD10G_111")
                { model = "LPD10G_111"; }
                else
                { model = "LPD10GE_107"; }

                // Even when no tester data is found, the module have to appear in the datagridview
                DataRow newr = dtReplace.NewRow();
                newr["serialno"] = serLong;
                newr["model"] = model;
                newr["lot"] = lot;
                newr["line"] = line;
                //newr["config"] = config;

                // If tester data exists, show it in the datagridview
                if (dt2.Rows.Count != 0)
                {
                    string process = dt2.Rows[0][1].ToString();
                    string linepass = String.Empty;
                    string buff = dt2.Rows[0][2].ToString();
                    if (buff == "0") linepass = "PASS";
                    else if (buff == "1") linepass = "FAIL";
                    else linepass = "ERROR";
                    DateTime testtime = (DateTime)dt2.Rows[0][3];
                    newr["process"] = process;
                    newr["linepass"] = linepass;
                    newr["testtime"] = testtime;
                }

                dtReplace.Rows.Add(newr);
            }
        }

        private void ckbDeleteBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!ckbDeleteBox.Checked) btnDeleteBoxId.Visible = false;
            else btnDeleteBoxId.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void btnDeleteBoxId_Click(object sender, EventArgs e)
        {
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
        private void readDatatable(ref DataTable dt)
        {
            string boxId = txtBoxId.Text;
            string sql = "select serialno, lot, line, process, linepass, testtime " +
                "FROM product_serial WHERE boxid='" + boxId + "'";
            ShSQL tf = new ShSQL();
            tf.sqlDataAdapterFillDatatable(sql, ref dt);
        }

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
            string fileName = @"Z:\(01)Motor\(00)Public\11-Suka-Sugawara\LD model\printer\warning.mp3";
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
            //foreach (DataRowView drv in dv)
            //{
            //    System.Diagnostics.Debug.Print(drv["process"].ToString() + ": " +
            //        drv["tjudge"].ToString() + ": " + drv["inspectdate"].ToString());
            //}
            foreach (DataRowView drv in dv)
            {
                System.Diagnostics.Debug.Print(drv["process"].ToString() + ": " +
                    drv["judge"].ToString() + ": " + drv["inspectdate"].ToString());
            }
        }

        private void btnReplaceSerial_Click(object sender, EventArgs e)
        {
            //btnRegisterBoxId.Enabled = false;
            //new frmReplace(txtBoxId.Text).ShowDialog();
        }
    }
}
