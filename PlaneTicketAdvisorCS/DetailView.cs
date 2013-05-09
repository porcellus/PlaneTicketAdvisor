using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BusinessLogic;
using PlaneTicketAdvisorCS.Properties;

namespace PlaneTicketAdvisorCS
{
    public partial class DetailView : UserControl
    {
        private ResultSet _resultSet = new ResultSet();
        private readonly bool _isPinned;

        public DetailView(bool isPinned)
        {
            _isPinned = isPinned;
            InitializeComponent();

            grdTickets.AutoGenerateColumns = false;
            grdTickets.ColumnHeadersVisible = true;

            grdTickets.DefaultCellStyle = new DataGridViewCellStyle();
            grdTickets.ColumnCount = 18;

            grdTickets.Columns[0].DataPropertyName = "Price";
            grdTickets.Columns[0].HeaderText = Resources.Form1_Form1_Load_Price;

            grdTickets.Columns[1].DataPropertyName = "Perprice";
            grdTickets.Columns[1].HeaderText = Resources.DetailView_DetailView_PerPrice;

            grdTickets.Columns[2].DataPropertyName = "OutCompany";
            grdTickets.Columns[2].HeaderText = Resources.DetailView_DetailView_OutCompany;

            grdTickets.Columns[3].DataPropertyName = "OutStartStation";
            grdTickets.Columns[3].HeaderText = Resources.DetailView_DetailView_OutStartStation;

            grdTickets.Columns[4].DataPropertyName = "OutStartDate";
            grdTickets.Columns[4].HeaderText = Resources.DetailView_DetailView_OutStartDate;
            grdTickets.Columns[5].DataPropertyName = "OutStartTime";
            grdTickets.Columns[5].HeaderText = Resources.DetailView_DetailView_OutStartTime;

            grdTickets.Columns[6].DataPropertyName = "OutArriveStation";
            grdTickets.Columns[6].HeaderText = Resources.DetailView_DetailView_OutArriveStation;

            grdTickets.Columns[7].DataPropertyName = "OutArriveTime";
            grdTickets.Columns[7].HeaderText = Resources.DetailView_DetailView_OutArriveTime;

            grdTickets.Columns[8].DataPropertyName = "RetCompany";
            grdTickets.Columns[8].HeaderText = Resources.DetailView_DetailView_RetCompany;

            grdTickets.Columns[9].DataPropertyName = "BackStartStation";
            grdTickets.Columns[9].HeaderText = Resources.DetailView_DetailView_BackStartStation;

            grdTickets.Columns[10].DataPropertyName = "BackStartDate";
            grdTickets.Columns[10].HeaderText = Resources.DetailView_DetailView_BackStartDate;
            grdTickets.Columns[11].DataPropertyName = "BackStartTime";
            grdTickets.Columns[11].HeaderText = Resources.DetailView_DetailView_BackStartTime;

            grdTickets.Columns[12].DataPropertyName = "BackArriveStation";
            grdTickets.Columns[12].HeaderText = Resources.DetailView_DetailView_BackArriveStation;

            grdTickets.Columns[13].DataPropertyName = "BackArriveTime";
            grdTickets.Columns[13].HeaderText = Resources.DetailView_DetailView_BackArriveTime;

            grdTickets.Columns[14].DataPropertyName = "OutStops";
            grdTickets.Columns[14].HeaderText = Resources.DetailView_DetailView_OutStops;
            grdTickets.Columns[15].DataPropertyName = "BackStops";
            grdTickets.Columns[15].HeaderText = Resources.DetailView_DetailView_BackStops;

            grdTickets.Columns[16].DataPropertyName = "OutTravelTime";
            grdTickets.Columns[16].HeaderText = Resources.DetailView_DetailView_OutTravelTime;
            grdTickets.Columns[17].DataPropertyName = "BackTravelTime";
            grdTickets.Columns[17].HeaderText = Resources.DetailView_DetailView_BackTravelTime;

            foreach (DataGridViewColumn col in grdTickets.Columns)
            {
                col.MinimumWidth = 50;
            }

        }
        public DetailView(): this(false) //Designer miatt szukseges
        {
        
        }

        public void SetTicket(ResultSet resultSet)
        {
            _resultSet = resultSet;
            lEngineName.Text = resultSet.EngineName;
            lPrice.Text = resultSet.SumPrice.ToString("C");
            lTicketCount.Text = resultSet.TicketCount.ToString(CultureInfo.InvariantCulture);
            lSumStops.Text = resultSet.SumStops.ToString(CultureInfo.InvariantCulture);
            lSumTravelTime.Text = resultSet.SumTravelTime.ToString();
            grdTickets.DataSource = resultSet.Tickets;
        }

        private void btnPin_Click(object sender, EventArgs e)
        {
            if (_isPinned) this.Dispose();
            else
            {
                if (Parent != null)
                {
                    var comp = new DetailView(true);
                    comp.Anchor = Anchor;
                    comp.Width = this.Width;
                    comp.Height = this.Height;

                    comp.SetTicket(_resultSet);
                    Parent.Controls.Add(comp);
                }
                SetTicket(new ResultSet());
            }
        }
    }
}
