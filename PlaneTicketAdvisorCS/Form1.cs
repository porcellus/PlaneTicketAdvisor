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

            //grdTravels.AutoGenerateColumns = false;
            //grdResults.AutoGenerateColumns = false;
            grdTravels.Columns.AddRange(new[]
                {
                    new DataGridViewColumn{DataPropertyName = "From", HeaderText = Resources.Form1_Form1_Load_From, CellTemplate = new DataGridViewTextBoxCell()},
                    new DataGridViewColumn{DataPropertyName = "To", HeaderText = Resources.Form1_Form1_Load_To, CellTemplate = new DataGridViewTextBoxCell()}, 
                    new DataGridViewColumn{DataPropertyName = "Date", HeaderText = Resources.Form1_Form1_Load_Date, CellTemplate = new DataGridViewTextBoxCell()},
                    new DataGridViewColumn{DataPropertyName = "Adults", HeaderText = Resources.Form1_Form1_Load_Adults, CellTemplate = new DataGridViewTextBoxCell()},
                    new DataGridViewColumn{DataPropertyName = "Children", HeaderText = Resources.Form1_Form1_Load_Children, CellTemplate = new DataGridViewTextBoxCell()},
                    new DataGridViewColumn{DataPropertyName = "Infants", HeaderText = Resources.Form1_Form1_Load_Infants, CellTemplate = new DataGridViewTextBoxCell()}
                });

            grdResults.Columns.Clear();
            grdResults.Columns.AddRange(new[]
                {
                    new DataGridViewColumn{DataPropertyName = "TicketCount", HeaderText = Resources.Form1_Form1_Load_TicketCount}, 
                    new DataGridViewColumn{DataPropertyName = "SumPrice", HeaderText = Resources.Form1_Form1_Load_Price}, 
                    new DataGridViewColumn{DataPropertyName = "EngineName", HeaderText = Resources.Form1_Form1_Load_SearchEngineName}
                });
            grdResults.DataSource = ResultSets;

            AppDomain currentDomain = AppDomain.CurrentDomain;
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

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fdialog = new OpenFileDialog();
            fdialog.ShowDialog();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sdialog = new SaveFileDialog();
            sdialog.ShowDialog();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            statusLabel.Text = Resources.Form1_btnSearch_Click_Searching;
            progressBar.Step = 1; 
            
            _travelSearchManager.StartSearch();
            
            var resultCheck = new Timer {Interval = 1000};
            resultCheck.Tick += resultCheck_Tick;
            resultCheck.Start();
        }

        protected readonly List<ResultSet> ResultSets = new List<ResultSet>();
        
        private void resultCheck_Tick(object sender, EventArgs e)
        {
            progressBar.Value = _travelSearchManager.GetProgress();
            if (progressBar.Value == 100) statusLabel.Text = Resources.Form1_resultCheck_Tick_Done;
            var results = _travelSearchManager.GetResults();
            //if (dataGridView1.DataSource == null || results.TrueForAll(a => ((object[])dataGridView1.DataSource).Length))
            //listView1.Clear();
            if(grdResults.RowCount == results.Count) return;
            ResultSets.Clear();
            ResultSets.AddRange(
                results.Select(
                    a =>
                    new ResultSet
                        {
                            EngineName = "liligo.hu",
                            Tickets = a.ToArray()
                        }));
            grdResults.DataSource = ResultSets;
            grdResults.Refresh();
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

            grdTravels.DataSource = _travelSearchManager.FlightList.ToList();
        }

        private void dtDep_ValueChanged(object sender, EventArgs e)
        {
            dtRet.MinDate = dtDep.Value;
        }

        private void grdTravels_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
        }

        private void runtestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _liligo.TestImageExp();
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

    }
}
