using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using BusinessLogic;

namespace WebMiner
{
    public class Liligo: ITravelSearchEngine
    {
        public void Initialize()
        {
        }

        public void StartSearch(Search searchInp)
        {
            var browser = new BrowserSimulator("http://liligo.hu");
            browser.DocumentReady += browser_DocumentReady;
            _progress = 0;

            while (!browser.IsDocumentReady)
            {
                System.Threading.Thread.Sleep(100);
                Awesomium.Core.WebCore.Update();
            }
            System.Threading.Thread.Sleep(1000);
            Awesomium.Core.WebCore.Update();
            //((Awesomium.Core.BitmapSurface)browser.Surface).SaveToJPEG("browser-before-input.jpg");

            browser.ClickElementPersistent("air-from");
            browser.Type(searchInp.From);
            System.Threading.Thread.Sleep(1000);
            _progress += 2;
            browser.ClickElementPersistent("air-to");
            browser.Type(searchInp.To);
            System.Threading.Thread.Sleep(1000);
            _progress += 2;
            browser.ClickElementPersistent("air-out-date");
            browser.ExecuteJavascriptWithResult("liligo.DatePicker2.actual.select(Date.parse('" + searchInp.Date.ToShortDateString() + "'),false)");
            System.Threading.Thread.Sleep(1000);
            _progress += 2;

            if (searchInp.RetDate.HasValue)
            {
                browser.ClickElement("air-ret-date");
                browser.ExecuteJavascriptWithResult("liligo.DatePicker2.actual.select(Date.parse('" + searchInp.RetDate.Value.ToShortDateString() + "'),false)");
            }
            else
            {
                browser.ClickElement("air-subcategory-oneway");
            }

            browser.ExecuteJavascriptWithResult("document.getElementById('air-adults').value=" + searchInp.Adults + ";");
            browser.ExecuteJavascriptWithResult("document.getElementById('air-children').value=" + searchInp.Children + ";");
            browser.ExecuteJavascriptWithResult("document.getElementById('air-infants').value=" + searchInp.Infants + ";");

            System.Threading.Thread.Sleep(1000);
            _progress += 2;
            browser.ClickElement("air-flexibility");
            //((Awesomium.Core.BitmapSurface)browser.Surface).SaveToJPEG("browser-after-input.jpg");
            browser.ClickElement("air-submit");
            browser.AddressChanged += Browser_AddressChanged;
        }

        private bool _isDocumentReady = false;

        void browser_DocumentReady(object sender, Awesomium.Core.WebViewEventArgs e)
        {
            ((BrowserSimulator)sender).ExecuteJavascriptWithResult(@"
                    function getResultObject(x){
                        var obj = {
                            perprice: x.getElementsByClassName('per-price')[0].getElementsByClassName('integer')[0].innerHTML.trim().replace(' ',''),
                            fullprice: x.getElementsByClassName('price')[0].getElementsByClassName('integer')[0].innerHTML.trim().replace(' ',''),
        
                            outCompany: x.getElementsByClassName('company')[1].getElementsByClassName('content')[0].innerHTML.trim(),
        
                            outStartTime: x.getElementsByClassName('from-time')[1].getElementsByClassName('content')[0].innerHTML.trim(),
                            outStartStation: x.getElementsByClassName('from-station')[1].getElementsByClassName('content')[0].innerHTML.trim(),
        
                            outArriveTime: x.getElementsByClassName('to-time')[1].getElementsByClassName('content')[0].innerHTML.trim(),
                            outArriveStation: x.getElementsByClassName('to-station')[1].getElementsByClassName('content')[0].innerHTML.trim(),
                        };
    
                        if(x.getElementsByClassName('company').length >= 2 && x.getElementsByClassName('company')[1].getElementsByClassName('content').length >= 2){
                            obj.backCompany = x.getElementsByClassName('company')[1].getElementsByClassName('content')[1].innerHTML.trim();
                            obj.backStartStation = x.getElementsByClassName('from-station')[1].getElementsByClassName('content')[1].innerHTML.trim();
                            obj.backStartTime = x.getElementsByClassName('from-time')[1].getElementsByClassName('content')[1].innerHTML.trim();
                            obj.backArriveStation = x.getElementsByClassName('to-station')[1].getElementsByClassName('content')[1].innerHTML.trim();
                            obj.backArriveTime = x.getElementsByClassName('to-time')[1].getElementsByClassName('content')[1].innerHTML.trim();
                        }
    
                        return obj;
                    }

                    function getResults(){
                        var retVal = [];
                        var elements=document.getElementsByClassName('resultitem');
                        for(var i=0;i<elements.length;i++){
                            retVal[i]=getResultObject(elements[i]);
                        }
                        return retVal;
                    }
            ");
            _isDocumentReady = true;
        }

        public double GetProgressPercent()
        {
            return _progress*1.25; // *100/80
        }

        public IEnumerable<Ticket> GetResults()
        {
            return _results??new Ticket[0];
        }

        private void Browser_AddressChanged(object sender, Awesomium.Core.UrlEventArgs e)
        {
            _isDocumentReady = false;
            if (e.Url.AbsoluteUri == "http://www.liligo.hu/air/SearchFlights.jsp")
            {
                var timer = new Timer();
                timer.Tick += (s,ev)=>OnTick(s,(BrowserSimulator) sender);
                timer.Interval = 500;
                timer.Start();
            }
        }

        private Ticket[] _results;
        private int _progress;

        private void OnTick(object sender, BrowserSimulator browser)
        {
            if (!_isDocumentReady) return;
            var isReady = browser.ExecuteJavascriptWithResult(
                    "(function(){return document.getElementsByClassName('stopped-finished').length == 1})()");
            if (isReady.IsBoolean && (bool)isReady)
            {
                ((Timer) sender).Stop();
                _progress = 80;
            }
            else _progress++;
            Awesomium.Core.JSValue[] res = browser.ExecuteJavascriptWithResult("objArrayToString(getResults());");
            _results = (from r in res select new Ticket(r.ToString())).ToArray();
        }
    }
}
