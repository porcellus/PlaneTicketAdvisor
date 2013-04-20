using System;
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

        private TravelManager _travelManager;

        private void Form1_Load(object sender, EventArgs e)
        {
            _travelManager = new TravelManager(new Func<ITravelSearchEngine>[] {() => new WebMiner.Liligo()});
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
            var results = _travelManager.GetResults();
            if (dataGridView1.DataSource == null || ((object[])dataGridView1.DataSource).Length != results.Length)
                dataGridView1.DataSource = results;

            progressBar.Step = _travelManager.GetProgress();
            if (progressBar.Step == 100) statusLabel.Text = Resources.Form1_resultCheck_Tick_Done;
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

            grdTravels.DataSource = _travelManager.Travels;
        }

    }
}
