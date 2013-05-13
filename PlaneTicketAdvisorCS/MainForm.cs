using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using BusinessLogic;
using PlaneTicketAdvisorCS;
using PlaneTicketAdvisorCS.Properties;

namespace TicketAdvisor
{
    public partial class MainForm : Form
    {
        #region Fields
        private readonly SortableBindingList<ResultSet> _resultSets = new SortableBindingList<ResultSet>();
        private TravelSearchManager _travelSearchManager;
        private WebMiner.Liligo _liligo;
        #endregion

        public MainForm()
        {
            InitializeComponent();
        }

        ~MainForm()
        {
            _liligo.Dispose();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _liligo = new WebMiner.Liligo();
            _travelSearchManager = new TravelSearchManager(new ITravelSearchEngine[] { _liligo });

            btnRemove.Text = Resources.MainForm_MainForm_Load_Delete;
            btnSearch.Text = Resources.MainForm_MainForm_Load_Search;
            cbIsRet.Text = Resources.MainForm_MainForm_Load_IsReturn;
            label8.Text = Resources.MainForm_MainForm_Load_Passangers;
            label7.Text = Resources.MainForm_MainForm_Load_Infants;
            label6.Text = Resources.MainForm_MainForm_Load_Children;
            label5.Text = Resources.MainForm_MainForm_Load_Adult;
            label4.Text = Resources.MainForm_MainForm_Load_BackDate;
            btnAdd.Text = Resources.MainForm_MainForm_Load_Add;
            label3.Text = Resources.MainForm_MainForm_Load_ToDate;
            label2.Text = Resources.MainForm_MainForm_Load_To;
            label1.Text = Resources.MainForm_MainForm_Load_From;
            Text = Resources.MainForm_MainForm_Load_AppName;

            dtDep.Value = DateTime.Today.AddDays(7);
            dtDep.MinDate = DateTime.Today;
            dtRet.Value = DateTime.Today.AddDays(9);

            grdTravels.AutoGenerateColumns = false;
            grdTravels.ColumnHeadersVisible = true;
            grdTravels.DefaultCellStyle = new DataGridViewCellStyle();
            grdTravels.ColumnCount = 6;

            grdTravels.Columns[0].DataPropertyName = "From";
            grdTravels.Columns[0].HeaderText = Resources.MainForm_MainForm_Load_From;
            grdTravels.Columns[1].DataPropertyName = "To";
            grdTravels.Columns[1].HeaderText = Resources.MainForm_MainForm_Load_To;
            grdTravels.Columns[2].DataPropertyName = "Date";
            grdTravels.Columns[2].HeaderText = Resources.MainForm_MainForm_Load_Date;
            grdTravels.Columns[3].DataPropertyName = "Adults";
            grdTravels.Columns[3].HeaderText = Resources.MainForm_MainForm_Load_Adults;
            grdTravels.Columns[4].DataPropertyName = "Children";
            grdTravels.Columns[4].HeaderText = Resources.MainForm_MainForm_Load_Children;
            grdTravels.Columns[5].DataPropertyName = "Infants";
            grdTravels.Columns[5].HeaderText = Resources.MainForm_MainForm_Load_Infants;
            
            grdResults.AutoGenerateColumns = false;
            grdResults.ColumnHeadersVisible = true;

            grdResults.DefaultCellStyle = new DataGridViewCellStyle();
            grdResults.ColumnCount = 5;

            grdResults.Columns[0].DataPropertyName = "TicketCount";
            grdResults.Columns[0].HeaderText = Resources.MainForm_MainForm_Load_TicketCount;
            grdResults.Columns[1].DataPropertyName = "SumPrice";
            grdResults.Columns[1].HeaderText = Resources.MainForm_MainForm_Load_Price;
            grdResults.Columns[2].DataPropertyName = "SumStops";
            grdResults.Columns[2].HeaderText = Resources.MainForm_MainForm_Load_Stops;
            grdResults.Columns[3].DataPropertyName = "SumTravelTime";
            grdResults.Columns[3].HeaderText = Resources.MainForm_MainForm_Load_TravelTime;
            grdResults.Columns[4].DataPropertyName = "EngineName";
            grdResults.Columns[4].HeaderText = Resources.MainForm_MainForm_Load_SearchEngineName;
            grdResults.DataSource = _resultSets;
            
            foreach (DataGridViewColumn col in grdResults.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.Automatic;
            }
            var currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += MyHandler;
        }

        static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            var e = (Exception)args.ExceptionObject;
            Debug.WriteLine("MyHandler caught : " + e.Message);
        }

        #region Event handlers

        #region ButtonClicks

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
                    statusLabel.Text = Resources.MainForm_btnSearch_Click_Searching;
                    progressBar.Step = 1;

                    _travelSearchManager.StartSearch();

                    var resultCheck = new Timer {Interval = 1000};
                    resultCheck.Tick += resultCheck_Tick;
                    resultCheck.Start();
                }
                btnSearch.Text = Resources.MainForm_btnSearch_Click_Stop;
                grdTravels.Enabled = false;
                btnAdd.Enabled = btnRemove.Enabled = false;
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _travelSearchManager.AddTravel(tbFrom.Text, tbTo.Text, dtDep.Value, (int) spAdult.Value, (int) spChild.Value,
                                           (int) spInfant.Value);
            if (cbIsRet.Checked)
                _travelSearchManager.AddTravel(tbTo.Text, tbFrom.Text, dtRet.Value, (int) spAdult.Value,
                                               (int) spChild.Value, (int) spInfant.Value);

            grdTravels.DataSource = _travelSearchManager.Travels.ToList();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (grdTravels.SelectedRows.Count != 1) return;
            var ind = grdTravels.SelectedRows[0].Index;
            grdTravels.DataSource = null;
            grdTravels.ClearSelection();
            _travelSearchManager.RemoveTravel(ind);
            grdTravels.DataSource = _travelSearchManager.Travels;
        }

        #endregion

        private void cbIsRet_CheckedChanged(object sender, EventArgs e)
        {
            dtRet.Enabled = cbIsRet.Checked;
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
                dvSelected.SetTicket((ResultSet) grdResults.SelectedRows[0].DataBoundItem);
                dvSelected.Visible = true;
            }
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
                ((DetailView) ctrl).Height = (flowLayoutPanel1.Height - 10)/flowLayoutPanel1.Controls.Count;
            }
        }

        private void flowLayoutPanel1_ControlAddedRemoved(object sender, ControlEventArgs e)
        {
            foreach (var ctrl in flowLayoutPanel1.Controls)
                ((DetailView)ctrl).Height = (flowLayoutPanel1.Height - 10) / flowLayoutPanel1.Controls.Count;
        }

        #endregion

        private void resultCheck_Tick(object sender, EventArgs e)
        {
            progressBar.Value = _travelSearchManager.GetProgress();
            if (progressBar.Value == 100) statusLabel.Text = Resources.MainForm_resultCheck_Tick_Done;
            var results = _travelSearchManager.GetResults();
            if (grdResults.RowCount != results.Count)
            {
                _resultSets.Clear();
                foreach (var resultSet in results)
                {
                    _resultSets.Add(resultSet);
                }
                grdResults.Refresh();
            }
            if (!_travelSearchManager.IsSearchInProgress)
            {
                statusLabel.Text = Resources.MainForm_resultCheck_Tick_Done;
                btnSearch.Text = Resources.MainForm_resultCheck_Tick_Search;
                grdTravels.Enabled = true;
                btnAdd.Enabled = btnRemove.Enabled = true;
                ((Timer)sender).Stop();
                ((Timer)sender).Dispose();
            }
        }
    }
}
