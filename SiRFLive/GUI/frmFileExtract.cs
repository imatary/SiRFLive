﻿namespace SiRFLive.GUI
{
    using SiRFLive.General;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;

    public class frmFileExtract : Form
    {
        private bool _abort;
        private bool _extractByLineNum;
        private bool _extractStatus;
        private string _inputFilePath = string.Empty;
        private string[] _isSubStrings = new string[12];
        private double _lineFrom;
        private double _lineTo;
        private int _numStrings;
        private string _outFilePath = string.Empty;
        private string[] _searchStrs = new string[12];
        private bool _show_CountLineNum;
        private double[] _totalNumMatches = new double[12];
        private Button autoTestDirBrowser;
        private Label autoTestFilePathLabel;
        private Button btn_Abort;
        private Button btn_Add;
        private Button btn_clear;
        private Button btn_clearAll;
        private Button btn_exit;
        private Button btn_outFileLink;
        private Button btn_start;
        private CheckBox checkBox_extractByLineNum;
        private CheckBox checkBox_showCountLineNum;
        private CheckBox checkBox_SubStr;
        private IContainer components;
        private DataGridView dataGridView1;
        private int extractThreadID;
        private DataGridViewTextBoxColumn isSubString;
        private Label label_endline;
        private Label label_fromline;
        private Label label_searchstr;
        private Label label_status;
        private static frmFileExtract m_SChildform;
        private DataGridViewTextBoxColumn num_Found;
        private ProgressBar progressBar1;
        private DataGridViewTextBoxColumn searchString;
        private TextBox textBox_endline;
        private TextBox textBox_fromline;
        private ToolTip toolTip1;
        private ToolTip toolTip2;
        private ToolTip toolTip3;
        private TextBox TxtBox_FilePath;
        private TextBox txtBox_searchStr;

        public frmFileExtract()
        {
            this.InitializeComponent();
        }

        private void autoTestDirBrowser_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = "gps";
            dialog.Filter = "gps files (*.gps)|*.gps";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = dialog.FileName;
                this.TxtBox_FilePath.Text = fileName;
            }
        }

        private void autoTestExitBtn_Click(object sender, EventArgs e)
        {
            this._abort = true;
            this._extractStatus = false;
            base.Close();
        }

        private void autoTestRunBtn_Click(object sender, EventArgs e)
        {
            this._abort = false;
            if (!this._extractStatus)
            {
                this._inputFilePath = this.TxtBox_FilePath.Text;
                if (!File.Exists(this._inputFilePath))
                {
                    MessageBox.Show("File does not exist, please enter a valid file path", "Error");
                    return;
                }
                if (this._extractByLineNum)
                {
                    if ((this.textBox_fromline.Text == "") && (this.textBox_endline.Text == ""))
                    {
                        MessageBox.Show("Invalid entries in 'From Line' and/or 'End Line'", "Error");
                        return;
                    }
                    if (this.textBox_fromline.Text == "")
                    {
                        this._lineFrom = 0.0;
                    }
                    if (this.textBox_endline.Text == "")
                    {
                        this._lineTo = 1000000.0;
                    }
                    try
                    {
                        this._lineFrom = Convert.ToDouble(this.textBox_fromline.Text);
                        this._lineTo = Convert.ToDouble(this.textBox_endline.Text);
                        if (this._lineFrom < 0.0)
                        {
                            this._lineFrom = 0.0;
                        }
                        if (this._lineTo < 0.0)
                        {
                            this._lineTo = 0.0;
                        }
                        if (this._lineFrom > this._lineTo)
                        {
                            double num = this._lineFrom;
                            this._lineFrom = this._lineTo;
                            this._lineTo = num;
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Invalid entries in 'From Line' and/or 'End Line'", "Error");
                        return;
                    }
                    string str = this._inputFilePath.Substring(0, this._inputFilePath.Length - 4) + "_" + this.textBox_fromline.Text + "_" + this.textBox_endline.Text;
                    this._outFilePath = str.Replace(",", "_") + ".par";
                    goto Label_0301;
                }
                this._numStrings = this.dataGridView1.RowCount;
                this._numStrings--;
                if (this._numStrings >= 1)
                {
                    for (int i = 0; i < this._numStrings; i++)
                    {
                        this._searchStrs[i] = this.dataGridView1.Rows[i].Cells[0].Value.ToString();
                        this._isSubStrings[i] = this.dataGridView1.Rows[i].Cells[1].Value.ToString();
                        this._totalNumMatches[i] = 0.0;
                    }
                    string str2 = this._inputFilePath.Substring(0, this._inputFilePath.Length - 4) + "_" + this._searchStrs[0];
                    this._outFilePath = str2.Replace(",", "_") + ".par";
                    goto Label_0301;
                }
                MessageBox.Show("Please enter string(s) for search");
            }
            return;
        Label_0301:
            this.btn_outFileLink.Visible = false;
            this._extractStatus = false;
            this.progressBar1.Value = 0;
            Thread.Sleep(100);
            try
            {
                this._extractStatus = true;
                Thread thread = new Thread(new ThreadStart(this.extractFile));
                this.extractThreadID = thread.ManagedThreadId;
                thread.IsBackground = true;
                thread.Start();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error: " + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void btn_Abort_Click(object sender, EventArgs e)
        {
            this.extractAbort();
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            if (this.txtBox_searchStr.Text == "")
            {
                MessageBox.Show("Please enter string to add", "Need more information");
            }
            else
            {
                DataGridViewRow dataGridViewRow = new DataGridViewRow();
                int rowIndex = this.dataGridView1.RowCount - 1;
                this.dataGridView1.Rows.Insert(rowIndex, dataGridViewRow);
                this.dataGridView1.Rows[rowIndex].Cells[0].Value = this.txtBox_searchStr.Text;
                this.dataGridView1.Rows[rowIndex].Cells[1].Value = this.checkBox_SubStr.Checked ? "Y" : "N";
                this.dataGridView1.Rows[rowIndex].Cells[2].Value = "";
                this.txtBox_searchStr.Text = "";
                this.checkBox_SubStr.Checked = false;
            }
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            try
            {
                if ((this.dataGridView1.SelectedRows.Count > 0) || (this.dataGridView1.SelectedCells.Count > 0))
                {
                    if (this.dataGridView1.SelectedRows.Count > 0)
                    {
                        foreach (DataGridViewRow row in this.dataGridView1.SelectedRows)
                        {
                            this.dataGridView1.Rows.Remove(row);
                        }
                    }
                    else
                    {
                        foreach (DataGridViewCell cell in this.dataGridView1.SelectedCells)
                        {
                            this.dataGridView1.Rows.RemoveAt(cell.RowIndex);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void btn_clear_MouseHover(object sender, EventArgs e)
        {
            string text = "Removes selected rows";
            this.toolTip3.Show(text, this.btn_clear, 0x7530);
        }

        private void btn_clearAll_Click(object sender, EventArgs e)
        {
            int num = this.dataGridView1.RowCount - 1;
            if (num >= 1)
            {
                for (int i = 0; i < num; i++)
                {
                    this.dataGridView1.Rows.RemoveAt(0);
                }
            }
        }

        private void btn_outFileLink_Click(object sender, EventArgs e)
        {
            Process.Start("notepad.exe", this._outFilePath);
        }

        private void checkBox_extractByLineNum_CheckedChanged(object sender, EventArgs e)
        {
            this.updateCtrlbyExtractType(this.checkBox_extractByLineNum.Checked);
        }

        private void checkBox_extractByLineNum_MouseHover(object sender, EventArgs e)
        {
            string text = "Extract an entire section of large files";
            this.toolTip2.Show(text, this.checkBox_extractByLineNum, 0x7530);
        }

        private void checkBox_showCountLineNum_CheckedChanged(object sender, EventArgs e)
        {
            this._show_CountLineNum = this.checkBox_showCountLineNum.Checked;
        }

        private void checkBox_SubStr_MouseHover(object sender, EventArgs e)
        {
            string text = "If the desired string does not start at the beginning of the line";
            this.toolTip1.Show(text, this.checkBox_SubStr, 0x7530);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void extractAbort()
        {
            if (MessageBox.Show("Abort extraction?", "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
            {
                this._abort = true;
                this._extractStatus = false;
            }
        }

        private void extractFile()
        {
            EventHandler method = null;
            Thread.CurrentThread.CurrentCulture = clsGlobal.MyCulture;
            StreamWriter writer = null;
            StreamReader reader = null;
            new StringBuilder();
			this.label_status.BeginInvoke((MethodInvoker)delegate
			{
                this.label_status.Text = "Status: Extracting in progress, please wait...";
            });
            double lineNo = 0.0;
            try
            {
                FileInfo info = new FileInfo(this._inputFilePath);
                double length = info.Length;
                writer = new StreamWriter(this._outFilePath);
                reader = new StreamReader(this._inputFilePath);
                string str = string.Empty;
                int num2 = 0;
                double num3 = 0.0;
                if (this._extractByLineNum)
                {
                    if (this._show_CountLineNum)
                    {
                        writer.WriteLine("Line#");
                    }
                    if (Math.Abs((double) (this._lineTo - 1000000.0)) > 1.0)
                    {
                        length = Math.Abs((double) (this._lineTo - this._lineFrom));
                    }
                    lineNo = 1.0;
                    while ((lineNo < this._lineFrom) && (str != null))
                    {
                        str = reader.ReadLine();
                        lineNo++;
                    }
                    while ((lineNo <= this._lineTo) && (str != null))
                    {
                        if (this._abort)
                        {
                            break;
                        }
                        num3 = (num3 + str.Length) + 2.0;
                        num2++;
                        if (num2 > 100)
                        {
                            int percent = (int) ((num3 / length) * 100.0);
                            if (percent > 100)
                            {
                                percent = 100;
                            }
							this.progressBar1.BeginInvoke((MethodInvoker)delegate
							{
                                this.progressBar1.Value = percent;
                            });
                            num2 = 0;
                        }
                        str = reader.ReadLine();
                        if (!this._show_CountLineNum)
                        {
                            writer.WriteLine(str);
                        }
                        else
                        {
                            writer.WriteLine((lineNo.ToString() + "\t") + str);
                        }
                        lineNo++;
                    }
                }
                else
                {
                    if (this._show_CountLineNum)
                    {
                        string str3 = "Line#\t";
                        for (int j = 0; j < this._numStrings; j++)
                        {
                            str3 = str3 + this._searchStrs[j] + "\t";
                        }
                        writer.WriteLine(str3);
                    }
                    lineNo = 1.0;
                    str = reader.ReadLine();
                    while (str != null)
                    {
                        if (this._abort)
                        {
                            break;
                        }
                        num3 = (num3 + str.Length) + 2.0;
                        num2++;
                        if (num2 > 100)
                        {
                            int percent = (int) ((num3 / length) * 100.0);
                            if (percent > 100)
                            {
                                percent = 100;
                            }
							this.progressBar1.BeginInvoke((MethodInvoker)delegate
							{
                                this.progressBar1.Value = percent;
                            });
                            if (method == null)
                            {
                                method = delegate {
                                    for (int m = 0; m < this._numStrings; m++)
                                    {
                                        this.dataGridView1.Rows[m].Cells[2].Value = this._totalNumMatches[m].ToString();
                                    }
                                };
                            }
                            this.dataGridView1.BeginInvoke(method);
                            num2 = 0;
                        }
                        bool flag = false;
                        string str4 = lineNo.ToString() + "\t";
                        for (int k = 0; k < this._numStrings; k++)
                        {
                            bool flag2 = false;
                            if (this._isSubStrings[k] == "Y")
                            {
                                flag2 = true;
                            }
                            string str5 = this._searchStrs[k];
                            if (str.StartsWith(str5) || (flag2 && str.Contains(str5)))
                            {
                                this._totalNumMatches[k]++;
                                str4 = str4 + this._totalNumMatches[k].ToString() + "\t";
                                flag = true;
                            }
                            else
                            {
                                str4 = str4 + " \t";
                            }
                        }
                        if (flag)
                        {
                            if (!this._show_CountLineNum)
                            {
                                writer.WriteLine(str);
                            }
                            else
                            {
                                writer.WriteLine(str4 + str);
                            }
                        }
                        str = reader.ReadLine();
                        lineNo++;
                    }
                }
                if (writer != null)
                {
                    writer.Close();
                }
                if (reader != null)
                {
                    reader.Close();
                }
                this._extractStatus = false;
            }
            catch (Exception exception)
            {
                if (writer != null)
                {
                    writer.Close();
                }
                if (reader != null)
                {
                    reader.Close();
                }
                this._extractStatus = false;
                this._abort = false;
                MessageBox.Show("Error: " + exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
			this.dataGridView1.BeginInvoke((MethodInvoker)delegate
			{
                for (int n = 0; n < this._numStrings; n++)
                {
                    this.dataGridView1.Rows[n].Cells[2].Value = this._totalNumMatches[n].ToString();
                }
            });
			this.label_status.BeginInvoke((MethodInvoker)delegate
			{
                if (this._abort)
                {
                    this.label_status.Text = "Parsing aborted.";
                }
                else
                {
                    this.label_status.Text = "Parsing completed. Total lines processed: " + ((lineNo - 1.0)).ToString() + ".  Output file:";
                }
            });
			this.btn_outFileLink.BeginInvoke((MethodInvoker)delegate
			{
                if (!this._abort)
                {
                    this.btn_outFileLink.Visible = true;
                    this.btn_outFileLink.Text = this._outFilePath;
                }
            });
        }

        private void frmFileExtract_Load(object sender, EventArgs e)
        {
            this.updateCtrlbyExtractType(this.checkBox_extractByLineNum.Checked);
            this._show_CountLineNum = this.checkBox_showCountLineNum.Checked;
            this.btn_start.Enabled = true;
            this.btn_exit.Enabled = true;
            this.btn_outFileLink.Visible = false;
        }

        public static frmFileExtract GetChildInstance()
        {
            if (m_SChildform == null)
            {
                m_SChildform = new frmFileExtract();
            }
            return m_SChildform;
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            DataGridViewCellStyle style2 = new DataGridViewCellStyle();
            DataGridViewCellStyle style3 = new DataGridViewCellStyle();
            ComponentResourceManager manager = new ComponentResourceManager(typeof(frmFileExtract));
            this.label_status = new Label();
            this.btn_start = new Button();
            this.btn_exit = new Button();
            this.autoTestDirBrowser = new Button();
            this.autoTestFilePathLabel = new Label();
            this.TxtBox_FilePath = new TextBox();
            this.txtBox_searchStr = new TextBox();
            this.label_searchstr = new Label();
            this.progressBar1 = new ProgressBar();
            this.btn_Abort = new Button();
            this.dataGridView1 = new DataGridView();
            this.searchString = new DataGridViewTextBoxColumn();
            this.isSubString = new DataGridViewTextBoxColumn();
            this.num_Found = new DataGridViewTextBoxColumn();
            this.checkBox_SubStr = new CheckBox();
            this.btn_Add = new Button();
            this.btn_clearAll = new Button();
            this.btn_outFileLink = new Button();
            this.textBox_fromline = new TextBox();
            this.textBox_endline = new TextBox();
            this.label_fromline = new Label();
            this.label_endline = new Label();
            this.checkBox_extractByLineNum = new CheckBox();
            this.checkBox_showCountLineNum = new CheckBox();
            this.toolTip1 = new ToolTip(this.components);
            this.toolTip2 = new ToolTip(this.components);
            this.btn_clear = new Button();
            this.toolTip3 = new ToolTip(this.components);
            ((ISupportInitialize) this.dataGridView1).BeginInit();
            base.SuspendLayout();
            this.label_status.AutoSize = true;
            this.label_status.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.label_status.ForeColor = Color.RoyalBlue;
            this.label_status.Location = new Point(0x3e, 0x196);
            this.label_status.Name = "label_status";
            this.label_status.Size = new Size(0x3b, 13);
            this.label_status.TabIndex = 0x3a;
            this.label_status.Text = "Status: idle";
            this.btn_start.Location = new Point(0xba, 0x1c8);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new Size(0x4b, 0x17);
            this.btn_start.TabIndex = 5;
            this.btn_start.Text = "&Start";
            this.btn_start.UseVisualStyleBackColor = true;
            this.btn_start.Click += new EventHandler(this.autoTestRunBtn_Click);
            this.btn_exit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_exit.Location = new Point(0x194, 0x1c8);
            this.btn_exit.Name = "btn_exit";
            this.btn_exit.Size = new Size(0x4b, 0x17);
            this.btn_exit.TabIndex = 7;
            this.btn_exit.Text = "E&xit";
            this.btn_exit.UseVisualStyleBackColor = true;
            this.btn_exit.Click += new EventHandler(this.autoTestExitBtn_Click);
            this.autoTestDirBrowser.Location = new Point(0x223, 0x3e);
            this.autoTestDirBrowser.Name = "autoTestDirBrowser";
            this.autoTestDirBrowser.Size = new Size(0x22, 20);
            this.autoTestDirBrowser.TabIndex = 1;
            this.autoTestDirBrowser.Text = "&...";
            this.autoTestDirBrowser.UseVisualStyleBackColor = true;
            this.autoTestDirBrowser.Click += new EventHandler(this.autoTestDirBrowser_Click);
            this.autoTestFilePathLabel.AutoSize = true;
            this.autoTestFilePathLabel.Location = new Point(0x23, 0x2d);
            this.autoTestFilePathLabel.Name = "autoTestFilePathLabel";
            this.autoTestFilePathLabel.Size = new Size(0x30, 13);
            this.autoTestFilePathLabel.TabIndex = 0x33;
            this.autoTestFilePathLabel.Text = "File Path";
            this.TxtBox_FilePath.Location = new Point(0x26, 0x3e);
            this.TxtBox_FilePath.Name = "TxtBox_FilePath";
            this.TxtBox_FilePath.Size = new Size(0x1f6, 20);
            this.TxtBox_FilePath.TabIndex = 0;
            this.txtBox_searchStr.Location = new Point(0x26, 0x95);
            this.txtBox_searchStr.Name = "txtBox_searchStr";
            this.txtBox_searchStr.Size = new Size(0x185, 20);
            this.txtBox_searchStr.TabIndex = 2;
            this.label_searchstr.AutoSize = true;
            this.label_searchstr.Location = new Point(0x23, 0x84);
            this.label_searchstr.Name = "label_searchstr";
            this.label_searchstr.Size = new Size(160, 13);
            this.label_searchstr.TabIndex = 60;
            this.label_searchstr.Text = "String (example: 225,6 [id,subid])";
            this.progressBar1.Location = new Point(0x26, 0x181);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new Size(0x21f, 0x11);
            this.progressBar1.TabIndex = 0x3e;
            this.btn_Abort.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Abort.Location = new Point(0x127, 0x1c8);
            this.btn_Abort.Name = "btn_Abort";
            this.btn_Abort.Size = new Size(0x4b, 0x17);
            this.btn_Abort.TabIndex = 6;
            this.btn_Abort.Text = "A&bort";
            this.btn_Abort.UseVisualStyleBackColor = true;
            this.btn_Abort.Click += new EventHandler(this.btn_Abort_Click);
            style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            style.BackColor = SystemColors.Control;
            style.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            style.ForeColor = SystemColors.WindowText;
            style.SelectionBackColor = SystemColors.Highlight;
            style.SelectionForeColor = SystemColors.HighlightText;
            style.WrapMode = DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = style;
            this.dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new DataGridViewColumn[] { this.searchString, this.isSubString, this.num_Found });
            style2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            style2.BackColor = SystemColors.Window;
            style2.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            style2.ForeColor = SystemColors.ControlText;
            style2.SelectionBackColor = SystemColors.Highlight;
            style2.SelectionForeColor = SystemColors.HighlightText;
            style2.WrapMode = DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = style2;
            this.dataGridView1.Location = new Point(0x26, 190);
            this.dataGridView1.Name = "dataGridView1";
            style3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            style3.BackColor = SystemColors.Control;
            style3.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            style3.ForeColor = SystemColors.WindowText;
            style3.SelectionBackColor = SystemColors.Highlight;
            style3.SelectionForeColor = SystemColors.HighlightText;
            style3.WrapMode = DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = style3;
            this.dataGridView1.Size = new Size(0x21f, 0xb9);
            this.dataGridView1.TabIndex = 0x42;
            this.searchString.FillWeight = 300f;
            this.searchString.HeaderText = "String";
            this.searchString.Name = "searchString";
            this.searchString.ReadOnly = true;
            this.searchString.Width = 300;
            this.isSubString.HeaderText = "SubString? (Y/N)";
            this.isSubString.Name = "isSubString";
            this.isSubString.ReadOnly = true;
            this.num_Found.HeaderText = "Number Found";
            this.num_Found.Name = "num_Found";
            this.num_Found.ReadOnly = true;
            this.checkBox_SubStr.AutoSize = true;
            this.checkBox_SubStr.Location = new Point(440, 0x97);
            this.checkBox_SubStr.Name = "checkBox_SubStr";
            this.checkBox_SubStr.Size = new Size(0x4e, 0x11);
            this.checkBox_SubStr.TabIndex = 3;
            this.checkBox_SubStr.Text = "SubString?";
            this.checkBox_SubStr.UseVisualStyleBackColor = true;
            this.checkBox_SubStr.MouseHover += new EventHandler(this.checkBox_SubStr_MouseHover);
            this.btn_Add.Location = new Point(0x20c, 0x94);
            this.btn_Add.Name = "btn_Add";
            this.btn_Add.Size = new Size(0x39, 0x17);
            this.btn_Add.TabIndex = 4;
            this.btn_Add.Text = "A&dd";
            this.btn_Add.UseVisualStyleBackColor = true;
            this.btn_Add.Click += new EventHandler(this.btn_Add_Click);
            this.btn_clearAll.Location = new Point(0x24f, 0x11c);
            this.btn_clearAll.Name = "btn_clearAll";
            this.btn_clearAll.Size = new Size(0x39, 0x17);
            this.btn_clearAll.TabIndex = 9;
            this.btn_clearAll.Text = "Clear &All";
            this.btn_clearAll.UseVisualStyleBackColor = true;
            this.btn_clearAll.Click += new EventHandler(this.btn_clearAll_Click);
            this.btn_outFileLink.FlatAppearance.BorderColor = Color.LightGray;
            this.btn_outFileLink.FlatStyle = FlatStyle.Flat;
            this.btn_outFileLink.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Underline, GraphicsUnit.Point, 0);
            this.btn_outFileLink.ForeColor = Color.Blue;
            this.btn_outFileLink.Location = new Point(0x80, 0x1a7);
            this.btn_outFileLink.Name = "btn_outFileLink";
            this.btn_outFileLink.Size = new Size(0x1c5, 0x1b);
            this.btn_outFileLink.TabIndex = 70;
            this.btn_outFileLink.Text = "outfile";
            this.btn_outFileLink.TextAlign = ContentAlignment.TopLeft;
            this.btn_outFileLink.UseVisualStyleBackColor = true;
            this.btn_outFileLink.Click += new EventHandler(this.btn_outFileLink_Click);
            this.textBox_fromline.Location = new Point(0x26, 0x67);
            this.textBox_fromline.Name = "textBox_fromline";
            this.textBox_fromline.Size = new Size(150, 20);
            this.textBox_fromline.TabIndex = 12;
            this.textBox_fromline.Text = "0";
            this.textBox_endline.Location = new Point(0x100, 0x67);
            this.textBox_endline.Name = "textBox_endline";
            this.textBox_endline.Size = new Size(140, 20);
            this.textBox_endline.TabIndex = 13;
            this.textBox_endline.Text = "0";
            this.label_fromline.AutoSize = true;
            this.label_fromline.Location = new Point(0x23, 0x56);
            this.label_fromline.Name = "label_fromline";
            this.label_fromline.Size = new Size(0x37, 13);
            this.label_fromline.TabIndex = 0x4a;
            this.label_fromline.Text = "Start Line:";
            this.label_endline.AutoSize = true;
            this.label_endline.Location = new Point(0xfd, 0x56);
            this.label_endline.Name = "label_endline";
            this.label_endline.Size = new Size(0x34, 13);
            this.label_endline.TabIndex = 0x4b;
            this.label_endline.Text = "End Line:";
            this.checkBox_extractByLineNum.AutoSize = true;
            this.checkBox_extractByLineNum.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.checkBox_extractByLineNum.Location = new Point(40, 0x12);
            this.checkBox_extractByLineNum.Name = "checkBox_extractByLineNum";
            this.checkBox_extractByLineNum.Size = new Size(0x86, 0x11);
            this.checkBox_extractByLineNum.TabIndex = 10;
            this.checkBox_extractByLineNum.Text = "Extract by Line number";
            this.checkBox_extractByLineNum.UseVisualStyleBackColor = true;
            this.checkBox_extractByLineNum.CheckedChanged += new EventHandler(this.checkBox_extractByLineNum_CheckedChanged);
            this.checkBox_extractByLineNum.MouseHover += new EventHandler(this.checkBox_extractByLineNum_MouseHover);
            this.checkBox_showCountLineNum.AutoSize = true;
            this.checkBox_showCountLineNum.Font = new Font("Microsoft Sans Serif", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.checkBox_showCountLineNum.Location = new Point(0x180, 0x12);
            this.checkBox_showCountLineNum.Name = "checkBox_showCountLineNum";
            this.checkBox_showCountLineNum.Size = new Size(0xcf, 0x11);
            this.checkBox_showCountLineNum.TabIndex = 11;
            this.checkBox_showCountLineNum.Text = "Show Count/Line number in output file";
            this.checkBox_showCountLineNum.UseVisualStyleBackColor = true;
            this.checkBox_showCountLineNum.CheckedChanged += new EventHandler(this.checkBox_showCountLineNum_CheckedChanged);
            this.btn_clear.Location = new Point(0x24f, 0xff);
            this.btn_clear.Name = "btn_clear";
            this.btn_clear.Size = new Size(0x39, 0x17);
            this.btn_clear.TabIndex = 8;
            this.btn_clear.Text = "&Clear";
            this.btn_clear.UseVisualStyleBackColor = true;
            this.btn_clear.Click += new EventHandler(this.btn_clear_Click);
            this.btn_clear.MouseHover += new EventHandler(this.btn_clear_MouseHover);
            base.AcceptButton = this.btn_Add;
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.CancelButton = this.btn_exit;
            base.ClientSize = new Size(0x29a, 0x1f2);
            base.Controls.Add(this.btn_clear);
            base.Controls.Add(this.checkBox_showCountLineNum);
            base.Controls.Add(this.checkBox_extractByLineNum);
            base.Controls.Add(this.label_endline);
            base.Controls.Add(this.label_fromline);
            base.Controls.Add(this.textBox_endline);
            base.Controls.Add(this.textBox_fromline);
            base.Controls.Add(this.btn_outFileLink);
            base.Controls.Add(this.btn_clearAll);
            base.Controls.Add(this.btn_Add);
            base.Controls.Add(this.checkBox_SubStr);
            base.Controls.Add(this.dataGridView1);
            base.Controls.Add(this.btn_Abort);
            base.Controls.Add(this.progressBar1);
            base.Controls.Add(this.label_searchstr);
            base.Controls.Add(this.txtBox_searchStr);
            base.Controls.Add(this.label_status);
            base.Controls.Add(this.btn_start);
            base.Controls.Add(this.btn_exit);
            base.Controls.Add(this.autoTestDirBrowser);
            base.Controls.Add(this.autoTestFilePathLabel);
            base.Controls.Add(this.TxtBox_FilePath);
            base.FormBorderStyle = FormBorderStyle.FixedSingle;
            base.Icon = (Icon) manager.GetObject("$this.Icon");
            base.Name = "frmFileExtract";
            base.ShowInTaskbar = false;
            base.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "File Extract/Find";
            base.Load += new EventHandler(this.frmFileExtract_Load);
            ((ISupportInitialize) this.dataGridView1).EndInit();
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        protected override void OnClosed(EventArgs e)
        {
            this._abort = true;
            this._extractStatus = false;
            Thread.Sleep(500);
        }

        private void updateCtrlbyExtractType(bool byLineNum)
        {
            this._extractByLineNum = byLineNum;
            if (byLineNum)
            {
                this.textBox_fromline.Visible = true;
                this.textBox_endline.Visible = true;
                this.label_fromline.Visible = true;
                this.label_endline.Visible = true;
                this.checkBox_SubStr.Visible = false;
                this.txtBox_searchStr.Visible = false;
                this.btn_Add.Visible = false;
                this.dataGridView1.Visible = false;
                this.btn_clear.Visible = false;
                this.btn_clearAll.Visible = false;
                this.label_searchstr.Visible = false;
            }
            if (!byLineNum)
            {
                this.textBox_fromline.Visible = false;
                this.textBox_endline.Visible = false;
                this.label_fromline.Visible = false;
                this.label_endline.Visible = false;
                this.checkBox_SubStr.Visible = true;
                this.txtBox_searchStr.Visible = true;
                this.btn_Add.Visible = true;
                this.dataGridView1.Visible = true;
                this.btn_clear.Visible = true;
                this.btn_clearAll.Visible = true;
                this.label_searchstr.Visible = true;
            }
        }
    }
}

