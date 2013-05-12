using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using BusinessLogic;
using PlaneTicketAdvisorCS.Properties;

namespace PlaneTicketAdvisorCS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        ~Form1()
        {
            _liligo.Dispose();
        }

        private TravelSearchManager _travelSearchManager;
        private WebMiner.Liligo _liligo;

        private void Form1_Load(object sender, EventArgs e)
        {
            _liligo = new WebMiner.Liligo();
            _travelSearchManager = new TravelSearchManager(new ITravelSearchEngine[] {_liligo});

            dtDep.Value = DateTime.Today.AddDays(7);
            dtDep.MinDate = DateTime.Today;
            dtRet.Value = DateTime.Today.AddDays(9);

            grdTravels.AutoGenerateColumns = false;
            grdTravels.ColumnHeadersVisible = true;
            grdTravels.DefaultCellStyle = new DataGridViewCellStyle();
            grdTravels.ColumnCount = 6;

            grdTravels.Columns[0].DataPropertyName = "From";
            grdTravels.Columns[0].HeaderText = Resources.Form1_Form1_Load_From;
            grdTravels.Columns[1].DataPropertyName = "To";
            grdTravels.Columns[1].HeaderText = Resources.Form1_Form1_Load_To;
            grdTravels.Columns[2].DataPropertyName = "Date";
            grdTravels.Columns[2].HeaderText = Resources.Form1_Form1_Load_Date;
            grdTravels.Columns[3].DataPropertyName = "Adults";
            grdTravels.Columns[3].HeaderText = Resources.Form1_Form1_Load_Adults;
            grdTravels.Columns[4].DataPropertyName = "Children";
            grdTravels.Columns[4].HeaderText = Resources.Form1_Form1_Load_Children;
            grdTravels.Columns[5].DataPropertyName = "Infants";
            grdTravels.Columns[5].HeaderText = Resources.Form1_Form1_Load_Infants;
            
            grdResults.AutoGenerateColumns = false;
            grdResults.ColumnHeadersVisible = true;
            grdResults.DefaultCellStyle = new DataGridViewCellStyle();
            grdResults.ColumnCount = 5;

            grdResults.Columns[0].DataPropertyName = "TicketCount";
            grdResults.Columns[0].HeaderText = Resources.Form1_Form1_Load_TicketCount;
            grdResults.Columns[1].DataPropertyName = "SumPrice";
            grdResults.Columns[1].HeaderText = Resources.Form1_Form1_Load_Price;
            grdResults.Columns[2].DataPropertyName = "SumStops";
            grdResults.Columns[2].HeaderText = Resources.Form1_Form1_Load_Stops;
            grdResults.Columns[3].DataPropertyName = "SumTravelTime";
            grdResults.Columns[3].HeaderText = Resources.Form1_Form1_Load_TravelTime;
            grdResults.Columns[4].DataPropertyName = "EngineName";
            grdResults.Columns[4].HeaderText = Resources.Form1_Form1_Load_SearchEngineName;

            //grdResults.DataSource = ResultSets;

            var currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += MyHandler;
        }

        static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            var e = (Exception)args.ExceptionObject;
            Debug.WriteLine("MyHandler caught : " + e.Message);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var abox = new AboutBox();
            abox.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (_travelSearchManager.IsSearchInProgress)
            {
                _travelSearchManager.CancelSearch();
            }
            else
            {
                if (_travelSearchManager.Travels.Count > 0)
                {
                    statusLabel.Text = Resources.Form1_btnSearch_Click_Searching;
                    progressBar.Step = 1;

                    _travelSearchManager.StartSearch();

                    var resultCheck = new Timer {Interval = 1000};
                    resultCheck.Tick += resultCheck_Tick;
                    resultCheck.Start();
                }
                btnSearch.Text = Resources.Form1_btnSearch_Click_Stop;
                grdTravels.Enabled = false;
                btnAdd.Enabled = btnRemove.Enabled = false;
            }
        }

        protected readonly List<ResultSet> ResultSets = new List<ResultSet>();
        
        private void resultCheck_Tick(object sender, EventArgs e)
        {
            progressBar.Value = _travelSearchManager.GetProgress();
            if (progressBar.Value == 100) statusLabel.Text = Resources.Form1_resultCheck_Tick_Done;
            var results = _travelSearchManager.GetResults();
            if (grdResults.RowCount != results.Count)
            {
                ResultSets.Clear();
                ResultSets.AddRange(results);
                grdResults.DataSource = null;
                grdResults.DataSource = ResultSets;
                grdResults.Refresh();
            }
            if (!_travelSearchManager.IsSearchInProgress)
            {
                statusLabel.Text = Resources.Form1_resultCheck_Tick_Done;
                btnSearch.Text = Resources.Form1_resultCheck_Tick_Search;
                grdTravels.Enabled = true;
                btnAdd.Enabled = btnRemove.Enabled = true;
                ((Timer)sender).Stop();
                ((Timer)sender).Dispose();
            }
        }

        private void cbIsRet_CheckedChanged(object sender, EventArgs e)
        {
            dtRet.Enabled = cbIsRet.Checked;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _travelSearchManager.AddTravel(tbFrom.Text,tbTo.Text,dtDep.Value,(int) spAdult.Value,(int) spChild.Value,(int) spInfant.Value);
            if(cbIsRet.Checked)
                _travelSearchManager.AddTravel(tbTo.Text, tbFrom.Text, dtRet.Value, (int) spAdult.Value, (int) spChild.Value, (int) spInfant.Value);

            grdTravels.DataSource = _travelSearchManager.Travels.ToList();
        }

        private void dtDep_ValueChanged(object sender, EventArgs e)
        {
            dtRet.MinDate = dtDep.Value;
        }

        private void grdResults_SelectionChanged(object sender, EventArgs e)
        {
            if (grdResults.SelectedRows.Count == 0) dvSelected.Visible = false;
            else
            {
                dvSelected.SetTicket((ResultSet)grdResults.SelectedRows[0].DataBoundItem);
                dvSelected.Visible = true;
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if(grdTravels.SelectedRows.Count!=1) return;
            var ind = grdTravels.SelectedRows[0].Index;
            grdTravels.DataSource = null;
            grdTravels.ClearSelection();
            _travelSearchManager.RemoveTravel(ind);
            grdTravels.DataSource = _travelSearchManager.Travels;
        }

        private void grdTravels_SelectionChanged(object sender, EventArgs e)
        {
            btnRemove.Enabled = grdTravels.SelectedRows.Count == 1;
            btnSearch.Enabled = grdTravels.RowCount > 0;
        }

        private void grdTravels_RowsChanged(object sender, EventArgs e)
        {
            btnSearch.Enabled = grdTravels.RowCount > 0;
            btnRemove.Enabled = grdTravels.RowCount > 0;
        }

        private void flowLayoutPanel1_Resize(object sender, EventArgs e)
        {
            foreach (var ctrl in flowLayoutPanel1.Controls)
            {
                ((DetailView) ctrl).Width = flowLayoutPanel1.Width - 5;
            }
        }

    }
}
