using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BusinessLogic;

namespace WebMiner
{
    public class Liligo: ITravelSearchEngine
    {
        private int _activeSearches = 0;
        private const int MaxSearchCount = 5;
        private const int SearchDelay = 3000;
        private readonly Queue<Search> _searchList = new Queue<Search>();
        private readonly Dictionary<Search, Thread> _searchThreads = new Dictionary<Search, Thread>();
        private readonly ConcurrentDictionary<Search, ResultSet> _searchResults = new ConcurrentDictionary<Search, ResultSet>();
        private readonly Dictionary<Search, int>  _searchProgresses = new Dictionary<Search, int>();
        private readonly Dictionary<Search, BrowserSimulator> _searchBrowsers = new Dictionary<Search, BrowserSimulator>();
        private readonly Dictionary<Search, Timer> _searchTimers = new Dictionary<Search, Timer>();

        #region ITravelSearchEngine
        public void Initialize()
        {
        }

        public void StartSearches()
        {
            new Task(delegate
                {
                    while(_activeSearches < MaxSearchCount && _searchList.Count > 0)
                    {
                        var search = _searchList.Dequeue();
                        _searchBrowsers[search] = new BrowserSimulator("http://liligo.hu");
                        _searchProgresses[search] = 0;

                        _searchThreads.Add(search, new Thread(StartSearchThread));
                        _searchThreads[search].Start(search);

                        ++_activeSearches;
                        Thread.Sleep(SearchDelay);
                    }
                }).Start();
        }

        private void StartSearchThread(object obj)
        {
            StartSearch(obj as Search);
        }

        public void CancelSearches()
        {
            foreach (var thread in _searchThreads)
            {
                thread.Value.Abort();
            }
            _searchThreads.Clear();
        }

        public void AddSearch(Search nSearch)
        {
            _searchList.Enqueue(nSearch);
        }

        public void ClearSearches()
        {
            if(_activeSearches>0) CancelSearches();
            _searchList.Clear();
        }

        public double GetProgressPercent()
        {
            if (_searchProgresses.Count == 0) return 100;
            return _searchProgresses.Sum(a => a.Value) * 1.25 / (_searchList.Count + _searchProgresses.Count);
        }

        public IDictionary<Search, ResultSet> GetResults()
        {
            return (IDictionary<Search, ResultSet>)_searchResults ?? new Dictionary<Search, ResultSet>();
        }

        public void Dispose()
        {
            CancelSearches();
        }

        #endregion

        #region Individual Search
        private void StartSearch(Search searchInp)
        {
            _searchBrowsers[searchInp].DocumentReady += browser_DocumentReady;
            _searchBrowsers[searchInp].AddressChanged += (s, e) => Browser_AddressChanged(e, searchInp);
            _searchBrowsers[searchInp].Source = new Uri("http://liligo.hu");

            while (!_searchBrowsers[searchInp].IsDocumentReady)
            {
                Thread.Sleep(100);
            }
            Thread.Sleep(1000);
            BrowserSimulator.Update();
            //((Awesomium.Core.BitmapSurface)_searchBrowsers[searchInp].Surface).SaveToJPEG(searchInp.From + "-" + searchInp.To + "-before-input.jpg");

            _searchBrowsers[searchInp].ClickElementPersistent("air-from");
            _searchBrowsers[searchInp].Type(searchInp.From);
            Thread.Sleep(1000);
            _searchProgresses[searchInp] += 2;
            _searchBrowsers[searchInp].ClickElementPersistent("air-to");
            _searchBrowsers[searchInp].Type(searchInp.To);
            Thread.Sleep(1000);
            _searchProgresses[searchInp] += 2;
            _searchBrowsers[searchInp].ClickElementPersistent("air-out-date");
            _searchBrowsers[searchInp].ExecuteJavascriptWithResult("liligo.DatePicker2.actual.select(Date.parse('" + searchInp.Date.ToShortDateString() + "'),false)");
            Thread.Sleep(1000);
            _searchProgresses[searchInp] += 2;

            if (searchInp.RetDate.HasValue)
            {
                _searchBrowsers[searchInp].ClickElementPersistent("air-ret-date");
                _searchBrowsers[searchInp].ExecuteJavascriptWithResult("liligo.DatePicker2.actual.select(Date.parse('" + searchInp.RetDate.Value.ToShortDateString() + "'),false)");
            }
            else
            {
                _searchBrowsers[searchInp].ClickElementPersistent("air-subcategory-oneway");
            }

            _searchBrowsers[searchInp].ExecuteJavascriptWithResult("document.getElementById('air-adults').value=" + searchInp.Adults + ";");
            _searchBrowsers[searchInp].ExecuteJavascriptWithResult("document.getElementById('air-children').value=" + searchInp.Children + ";");
            _searchBrowsers[searchInp].ExecuteJavascriptWithResult("document.getElementById('air-infants').value=" + searchInp.Infants + ";");

            Thread.Sleep(1000);
            _searchProgresses[searchInp] += 2;
            _searchBrowsers[searchInp].ClickElementPersistent("air-flexibility");
            //((Awesomium.Core.BitmapSurface)_searchBrowsers[searchInp].Surface).SaveToJPEG(searchInp.From + "-"+searchInp.To+"-after-input.jpg");
            _searchBrowsers[searchInp].ClickElementPersistent("air-submit");
            _searchBrowsers[searchInp].ClickElementPersistent("air-submit");
            //((Awesomium.Core.BitmapSurface)_searchBrowsers[searchInp].Surface).SaveToJPEG(searchInp.From + "-" + searchInp.To + "-after-click.jpg");
        }

        static void browser_DocumentReady(object sender, Awesomium.Core.WebViewEventArgs e)
        {
            ((BrowserSimulator)sender).ExecuteJavascriptWithResult(@"
                    function getResultObject(x){
                        var obj = {
                            perprice: x.getElementsByClassName('per-price')[0].getElementsByClassName('integer')[0].innerHTML.trim().replace(' ',''),
                            fullprice: x.getElementsByClassName('price')[0].getElementsByClassName('integer')[0].innerHTML.trim().replace(' ',''),
        
                            outCompany: x.getElementsByClassName('company')[1].getElementsByClassName('content')[0].innerHTML.trim(),
        
                            outStartDate: document.getElementsByClassName('header-summary')[0].getElementsByClassName('dates')[1].innerHTML.trim(),
                            outStartTime: x.getElementsByClassName('from-time')[1].getElementsByClassName('content')[0].innerHTML.trim(),
                            outStartStation: x.getElementsByClassName('from-station')[1].getElementsByClassName('content')[0].innerHTML.trim(),
        
                            outArriveTime: x.getElementsByClassName('to-time')[1].getElementsByClassName('content')[0].innerHTML.trim(),
                            outArriveStation: x.getElementsByClassName('to-station')[1].getElementsByClassName('content')[0].innerHTML.trim(),
                            outStops: x.getElementsByClassName('stops')[1]
                                                .getElementsByTagName('span')[2].innerHTML.trim() == 'közvetlen'? '0' : 
                                                 x.getElementsByClassName('stops')[1]
                                                  .getElementsByTagName('span')[2].innerHTML.trim()
                                                  .substr(0,x.getElementsByClassName('stops')[1]
                                                    .getElementsByTagName('span')[2].innerHTML.trim().indexOf(' ')
                                            ),
                            outTravelTime: x.getElementsByClassName('stops')[1].getElementsByTagName('span')[1].innerHTML.trim().replace('ó',':').substr(1,4)
                        };
    
                        if(x.getElementsByClassName('company').length >= 2 && x.getElementsByClassName('company')[1].getElementsByClassName('content').length >= 2){
                            obj.backCompany = x.getElementsByClassName('company')[1].getElementsByClassName('content')[1].innerHTML.trim();
                            obj.backStartStation = x.getElementsByClassName('from-station')[1].getElementsByClassName('content')[1].innerHTML.trim();
                            obj.backStartDate= document.getElementsByClassName('header-summary')[0].getElementsByClassName('dates')[1].innerHTML.trim();
                            obj.backStartTime = x.getElementsByClassName('from-time')[1].getElementsByClassName('content')[1].innerHTML.trim();
                            obj.backArriveStation = x.getElementsByClassName('to-station')[1].getElementsByClassName('content')[1].innerHTML.trim();
                            obj.backArriveTime = x.getElementsByClassName('to-time')[1].getElementsByClassName('content')[1].innerHTML.trim();
                            obj.backStops= x.getElementsByClassName('stops')[1]
                                                .getElementsByTagName('span')[2].innerHTML.trim() == 'közvetlen'? '0' : 
                                                 x.getElementsByClassName('stops')[1]
                                                  .getElementsByTagName('span')[2].innerHTML.trim()
                                                  .substr(0,x.getElementsByClassName('stops')[1]
                                                    .getElementsByTagName('span')[2].innerHTML.trim().indexOf(' ')
                                            );
                            obj.backTravelTime= x.getElementsByClassName('stops')[1].getElementsByTagName('span')[3].innerHTML.trim().replace('ó',':').substr(1,4);
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
        }

        private void Browser_AddressChanged(Awesomium.Core.UrlEventArgs ev, Search search)
        {
            if (ev.Url.AbsoluteUri == "http://www.liligo.hu/air/SearchFlights.jsp")
            {
                _searchTimers.Add(search, new Timer(ResultChecker, search, 0, 500));
            }
        }

        private void ResultChecker(Object obj)
        {
            var search = obj as Search;

            Debug.Assert(search != null, "search != null");
            if(!_searchBrowsers[search].IsDocumentReady) return;
            Awesomium.Core.JSValue[] res = _searchBrowsers[search].ExecuteJavascriptWithResult("objArrayToString(getResults());");
            if (res.Count() > 0 && !res[0].IsUndefined)
                _searchResults[search] = new ResultSet("liligo", (from r in res select new Ticket(r.ToString())).ToArray());
            else _searchResults[search] = new ResultSet("liligo");
            /*
             foreach (var result in _searchResults[search].Tickets)
            {
                result.OutStartDate = search.Date;
                if(search.RetDate.HasValue) result.BackStartDate = search.RetDate.Value.Date;
            }*/
            var isOver = _searchBrowsers[search].ExecuteJavascriptWithResult(
                    "(function(){return document.getElementsByClassName('stopped-finished').length == 1})()");
            if (isOver.IsBoolean && (bool)isOver)
            {
                _searchProgresses[search] = 80;
                --_activeSearches;
                _searchTimers[search].Dispose();
                StartSearches();
            }
            else if(_searchProgresses[search] < 79) _searchProgresses[search]++;
        }

        public void TestImageExp()
        {
            foreach (var browser in _searchBrowsers)
            {
                ((Awesomium.Core.BitmapSurface) browser.Value.Surface).SaveToJPEG(browser.Key.From + "-" +
                                                                                  browser.Key.To + "-test.jpg");
            }
        }
        #endregion
    }
}
