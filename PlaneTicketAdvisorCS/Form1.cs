using System;
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

        private TravelManager _travelManager;
        private WebMiner.Liligo _liligo;

        private void Form1_Load(object sender, EventArgs e)
        {
            _liligo = new WebMiner.Liligo();
            _travelManager = new TravelManager(new ITravelSearchEngine[] {_liligo});

            dtDep.Value = DateTime.Today.AddDays(7);
            dtDep.MinDate = DateTime.Today;
            dtRet.Value = DateTime.Today.AddDays(9);

            grdTravels.Columns.AddRange(new[]
                {
                    new DataGridViewColumn{DataPropertyName = "From", HeaderText = Resources.Form1_Form1_Load_From, CellTemplate = new DataGridViewTextBoxCell()},
                    new DataGridViewColumn{DataPropertyName = "To", HeaderText = Resources.Form1_Form1_Load_To, CellTemplate = new DataGridViewTextBoxCell()}, 
                    new DataGridViewColumn{DataPropertyName = "Date", HeaderText = Resources.Form1_Form1_Load_Date, CellTemplate = new DataGridViewTextBoxCell()},
                    new DataGridViewColumn{DataPropertyName = "Adults", HeaderText = Resources.Form1_Form1_Load_Adults, CellTemplate = new DataGridViewTextBoxCell()},
                    new DataGridViewColumn{DataPropertyName = "Children", HeaderText = Resources.Form1_Form1_Load_Children, CellTemplate = new DataGridViewTextBoxCell()},
                    new DataGridViewColumn{DataPropertyName = "Infants", HeaderText = Resources.Form1_Form1_Load_Infants, CellTemplate = new DataGridViewTextBoxCell()},
                });

            listView1.Columns.AddRange(new[]
                {
                    new ColumnHeader {Text = Resources.Form1_Form1_Load_From, Width = -1},
                    new ColumnHeader {Text = Resources.Form1_Form1_Load_To, Width = -1},
                    new ColumnHeader {Text = Resources.Form1_Form1_Load_OutDate, Width = -1},
                    new ColumnHeader {Text = Resources.Form1_Form1_Load_BackDate, Width = -1},
                    new ColumnHeader {Text = Resources.Form1_Form1_Load_Price, Width = -1},
                });

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += MyHandler;
        }

        static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
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
            
            _travelManager.StartSearch();
            
            var resultCheck = new Timer {Interval = 1000};
            resultCheck.Tick += resultCheck_Tick;
            resultCheck.Start();
        }

        private void resultCheck_Tick(object sender, EventArgs e)
        {
            progressBar.Value = _travelManager.GetProgress();
            if (progressBar.Value == 100) statusLabel.Text = Resources.Form1_resultCheck_Tick_Done;
            var results = _travelManager.GetResults();
            //if (dataGridView1.DataSource == null || results.TrueForAll(a => ((object[])dataGridView1.DataSource).Length))
            //listView1.Clear();
            if (listView1.Items.Count >= results.Sum(a => a.Count) + results.Count) return;
            listView1.Items.Clear();
            foreach (var resultset in results)
            {
                if (resultset.Count != 0)
                {
                    listView1.Items.AddRange(
                        resultset.Select(ticket => new ListViewItem(new []{ticket.OutStartStation, ticket.OutArriveStation, ticket.Price.ToString()})).ToArray()
                        );
                    listView1.Items.Add(new ListViewItem(new string[] {"end", "sum", resultset.Sum(a=> a.Price).ToString()}));
                }
            }
            listView1.Refresh();
        }

        private void cbIsRet_CheckedChanged(object sender, EventArgs e)
        {
            dtRet.Enabled = cbIsRet.Checked;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _travelManager.AddTravel(tbFrom.Text,tbTo.Text,dtDep.Value,(int) spAdult.Value,(int) spChild.Value,(int) spInfant.Value);
            if(cbIsRet.Checked)
                _travelManager.AddTravel(tbTo.Text, tbFrom.Text, dtRet.Value, (int) spAdult.Value, (int) spChild.Value, (int) spInfant.Value);

            grdTravels.DataSource = _travelManager.FlightList.ToList();
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

    }
}
