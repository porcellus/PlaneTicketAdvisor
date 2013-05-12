namespace PlaneTicketAdvisorCS
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            this.webControl1 = new Awesomium.Windows.Forms.WebControl(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.grdTravels = new System.Windows.Forms.DataGridView();
            this.btnRemove = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.dvSelected = new PlaneTicketAdvisorCS.DetailView();
            this.btnSearch = new System.Windows.Forms.Button();
            this.grdResults = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cbIsRet = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.spInfant = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.spChild = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.spAdult = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dtRet = new System.Windows.Forms.DateTimePicker();
            this.tbTo = new System.Windows.Forms.TextBox();
            this.tbFrom = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.dtDep = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdTravels)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdResults)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spInfant)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spChild)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spAdult)).BeginInit();
            this.SuspendLayout();
            // 
            // webControl1
            // 
            this.webControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webControl1.Location = new System.Drawing.Point(3, 3);
            this.webControl1.Size = new System.Drawing.Size(619, 406);
            this.webControl1.Source = new System.Uri("http://liligo.hu", System.UriKind.Absolute);
            this.webControl1.TabIndex = 0;
            this.webControl1.ViewType = Awesomium.Core.WebViewType.Offscreen;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressBar,
            this.statusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 579);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(746, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer.IsSplitterFixed = true;
            this.splitContainer.Location = new System.Drawing.Point(0, 124);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.grdTravels);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.flowLayoutPanel1);
            this.splitContainer.Panel2.Controls.Add(this.grdResults);
            this.splitContainer.Size = new System.Drawing.Size(734, 452);
            this.splitContainer.SplitterDistance = 78;
            this.splitContainer.TabIndex = 6;
            // 
            // grdTravels
            // 
            this.grdTravels.AllowUserToAddRows = false;
            this.grdTravels.AllowUserToDeleteRows = false;
            this.grdTravels.AllowUserToResizeRows = false;
            this.grdTravels.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdTravels.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllHeaders;
            this.grdTravels.BackgroundColor = System.Drawing.SystemColors.Control;
            this.grdTravels.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.grdTravels.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdTravels.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdTravels.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.grdTravels.GridColor = System.Drawing.SystemColors.Control;
            this.grdTravels.Location = new System.Drawing.Point(0, 0);
            this.grdTravels.MultiSelect = false;
            this.grdTravels.Name = "grdTravels";
            this.grdTravels.ReadOnly = true;
            this.grdTravels.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdTravels.Size = new System.Drawing.Size(732, 76);
            this.grdTravels.TabIndex = 3;
            this.grdTravels.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.grdTravels_RowsChanged);
            this.grdTravels.SelectionChanged += new System.EventHandler(this.grdTravels_SelectionChanged);
            // 
            // btnRemove
            // 
            this.btnRemove.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.btnRemove.Enabled = false;
            this.btnRemove.Location = new System.Drawing.Point(87, 90);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 21;
            this.btnRemove.Text = "Töröl";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.dvSelected);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(347, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(382, 365);
            this.flowLayoutPanel1.TabIndex = 3;
            this.flowLayoutPanel1.Resize += new System.EventHandler(this.flowLayoutPanel1_Resize);
            // 
            // dvSelected
            // 
            this.dvSelected.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dvSelected.Location = new System.Drawing.Point(3, 3);
            this.dvSelected.Name = "dvSelected";
            this.dvSelected.Size = new System.Drawing.Size(400, 154);
            this.dvSelected.TabIndex = 4;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Enabled = false;
            this.btnSearch.Location = new System.Drawing.Point(429, 94);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 20;
            this.btnSearch.Text = "Keres";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // grdResults
            // 
            this.grdResults.AllowUserToAddRows = false;
            this.grdResults.AllowUserToDeleteRows = false;
            this.grdResults.AllowUserToResizeRows = false;
            this.grdResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdResults.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCellsExceptHeaders;
            this.grdResults.BackgroundColor = System.Drawing.SystemColors.Control;
            this.grdResults.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.grdResults.Dock = System.Windows.Forms.DockStyle.Left;
            this.grdResults.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.grdResults.GridColor = System.Drawing.SystemColors.Control;
            this.grdResults.Location = new System.Drawing.Point(0, 0);
            this.grdResults.MultiSelect = false;
            this.grdResults.Name = "grdResults";
            this.grdResults.ReadOnly = true;
            this.grdResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdResults.Size = new System.Drawing.Size(344, 368);
            this.grdResults.TabIndex = 2;
            this.grdResults.SelectionChanged += new System.EventHandler(this.grdResults_SelectionChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnRemove);
            this.panel1.Controls.Add(this.btnSearch);
            this.panel1.Controls.Add(this.cbIsRet);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.spInfant);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.spChild);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.spAdult);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.dtRet);
            this.panel1.Controls.Add(this.tbTo);
            this.panel1.Controls.Add(this.tbFrom);
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Controls.Add(this.dtDep);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(0, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(736, 116);
            this.panel1.TabIndex = 5;
            // 
            // cbIsRet
            // 
            this.cbIsRet.AutoSize = true;
            this.cbIsRet.Checked = true;
            this.cbIsRet.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbIsRet.Location = new System.Drawing.Point(328, 61);
            this.cbIsRet.Name = "cbIsRet";
            this.cbIsRet.Size = new System.Drawing.Size(79, 17);
            this.cbIsRet.TabIndex = 19;
            this.cbIsRet.Text = "Oda-Vissza";
            this.cbIsRet.UseVisualStyleBackColor = true;
            this.cbIsRet.CheckedChanged += new System.EventHandler(this.cbIsRet_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 60);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(44, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "Utasok:";
            // 
            // spInfant
            // 
            this.spInfant.Location = new System.Drawing.Point(288, 60);
            this.spInfant.Name = "spInfant";
            this.spInfant.Size = new System.Drawing.Size(34, 20);
            this.spInfant.TabIndex = 17;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(226, 60);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "Csecsemő";
            // 
            // spChild
            // 
            this.spChild.Location = new System.Drawing.Point(186, 60);
            this.spChild.Name = "spChild";
            this.spChild.Size = new System.Drawing.Size(34, 20);
            this.spChild.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(139, 60);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Gyerek";
            // 
            // spAdult
            // 
            this.spAdult.Location = new System.Drawing.Point(96, 58);
            this.spAdult.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.spAdult.Name = "spAdult";
            this.spAdult.Size = new System.Drawing.Size(37, 20);
            this.spAdult.TabIndex = 13;
            this.spAdult.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(51, 60);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Felnőtt";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(261, 33);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Vissza";
            // 
            // dtRet
            // 
            this.dtRet.Location = new System.Drawing.Point(304, 33);
            this.dtRet.Name = "dtRet";
            this.dtRet.Size = new System.Drawing.Size(200, 20);
            this.dtRet.TabIndex = 10;
            // 
            // tbTo
            // 
            this.tbTo.Location = new System.Drawing.Point(304, 4);
            this.tbTo.Name = "tbTo";
            this.tbTo.Size = new System.Drawing.Size(200, 20);
            this.tbTo.TabIndex = 8;
            this.tbTo.Text = "London";
            // 
            // tbFrom
            // 
            this.tbFrom.Location = new System.Drawing.Point(54, 4);
            this.tbFrom.Name = "tbFrom";
            this.tbFrom.Size = new System.Drawing.Size(200, 20);
            this.tbFrom.TabIndex = 7;
            this.tbFrom.Text = "Budapest";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(6, 90);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 6;
            this.btnAdd.Text = "Hozzáad";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // dtDep
            // 
            this.dtDep.Location = new System.Drawing.Point(54, 33);
            this.dtDep.Name = "dtDep";
            this.dtDep.Size = new System.Drawing.Size(200, 20);
            this.dtDep.TabIndex = 5;
            this.dtDep.ValueChanged += new System.EventHandler(this.dtDep_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Oda";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(261, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Hova";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Honnan";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(746, 601);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdTravels)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdResults)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spInfant)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spChild)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spAdult)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Awesomium.Windows.Forms.WebControl webControl1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.DataGridView grdTravels;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox cbIsRet;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown spInfant;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown spChild;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown spAdult;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtRet;
        private System.Windows.Forms.TextBox tbTo;
        private System.Windows.Forms.TextBox tbFrom;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.DateTimePicker dtDep;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView grdResults;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private DetailView dvSelected;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnSearch;
    }
}

