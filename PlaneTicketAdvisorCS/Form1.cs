using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PlaneTicketAdvisorCS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
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
            statusLabel.Text = "Keresés...";
            progressBar.Step = 1;
            var browser = new WebMiner.BrowserSimulator(Awesomium.Core.WebCore.CreateWebView(1024, 768));
            browser.Source = new Uri("http://liligo.hu", UriKind.Absolute);
            while (!browser.IsDocumentReady)
            {
                System.Threading.Thread.Sleep(100);
                Awesomium.Core.WebCore.Update();
            }
            Awesomium.Core.WebCore.Update();
            
            //((Awesomium.Core.BitmapSurface)browser.Surface).SaveToJPEG("browser-before-input.jpg");
            browser.ClickElement("air-from");
            browser.Type(tbFrom.Text);
            System.Threading.Thread.Sleep(1000);
            browser.ClickElement("air-to");
            browser.Type(tbTo.Text);
            System.Threading.Thread.Sleep(1000);
            browser.ClickElement("air-out-date");
            browser.ExecuteJavascriptWithResult("liligo.DatePicker2.actual.select(Date.parse('" + dtDep.Value.ToShortDateString() + "'),false)");
            System.Threading.Thread.Sleep(1000);

            if (cbIsRet.Checked)
            {
                browser.ClickElement("air-ret-date");
                browser.ExecuteJavascriptWithResult("liligo.DatePicker2.actual.select(Date.parse('" + dtRet.Value.ToShortDateString() + "'),false)");
            }
            else
            {
                browser.ClickElement("air-subcategory-oneway");
            }

            browser.ExecuteJavascriptWithResult("document.getElementById('air-adults').value="+ spAdult.Value +";");
            browser.ExecuteJavascriptWithResult("document.getElementById('air-children').value=" + spChild.Value + ";");
            browser.ExecuteJavascriptWithResult("document.getElementById('air-infants').value=" + spInfant.Value + ";");

            System.Threading.Thread.Sleep(1000);
            browser.ClickElement("air-flexibility");
            //((Awesomium.Core.BitmapSurface)browser.Surface).SaveToJPEG("browser-after-input.jpg");
            browser.ClickElement("air-submit");
            browser.AddressChanged += Browser_AddressChanged;
        }

        Timer timer;
        public void Browser_AddressChanged(object sender, Awesomium.Core.UrlEventArgs e)
        {
            if (e.Url.AbsoluteUri == "http://www.liligo.hu/air/SearchFlights.jsp")
            {
                timer = new Timer();
                timer.Tick += (s,ev)=>getFirstRes(s,ev,(WebMiner.BrowserSimulator)sender);
                timer.Interval = 500;
                timer.Start();
            }
        }

        public void getFirstRes(object sender, EventArgs e, WebMiner.BrowserSimulator browser){
            progressBar.PerformStep();
            if ((int)browser.ExecuteJavascriptWithResult("(function(){return document.getElementsByClassName('stopped-finished').length})()") == 1)
            {
                timer.Stop();
                progressBar.Value = 0;
                statusLabel.Text = "Kész...";
                ((Awesomium.Core.BitmapSurface)browser.Surface).SaveToJPEG("browser-test.jpg");
            }

            dataGridView1.DataSource = browser.GetResults();
            dataGridView1.Refresh();
        }

        private void cbIsRet_CheckedChanged(object sender, EventArgs e)
        {
            dtRet.Enabled = cbIsRet.Checked;
        }

    }
}
