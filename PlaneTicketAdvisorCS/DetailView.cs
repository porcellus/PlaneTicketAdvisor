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

namespace PlaneTicketAdvisorCS
{
    public partial class DetailView : UserControl
    {
        private ResultSet _resultSet = new ResultSet();
        private bool _isPinned;

        public DetailView(bool isPinned)
        {
            _isPinned = isPinned;
            InitializeComponent();
            //dataGridView1.AutoGenerateColumns = false;
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
            dataGridView1.DataSource = resultSet.Tickets;
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

                    comp.SetTicket(_resultSet);
                    Parent.Controls.Add(comp);
                }
                SetTicket(new ResultSet());
            }
        }
    }
}
