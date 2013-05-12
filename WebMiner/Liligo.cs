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
        #region Fields
        private int _activeSearches = 0;
        private const int MaxSearchCount = 5;
        private const int SearchDelay = 3000;

        private readonly ConcurrentQueue<Search> _searchQueue = new ConcurrentQueue<Search>();
        private readonly ConcurrentDictionary<Search, Thread> _searchThreads = new ConcurrentDictionary<Search, Thread>();
        private readonly ConcurrentDictionary<Search, Ticket[]> _searchResults = new ConcurrentDictionary<Search, Ticket[]>();
        private readonly ConcurrentDictionary<Search, int> _searchProgresses = new ConcurrentDictionary<Search, int>();
        #endregion

        #region ITravelSearchEngine
        public void Initialize()
        {
        }

        public void StartSearches()
        {
            new Task(delegate
                {
                    while(_activeSearches < MaxSearchCount && _searchQueue.Count > 0)
                    {
                        Search search;
                        while(!_searchQueue.TryDequeue(out search)) Thread.Sleep(10);

                        _searchProgresses[search] = 0;
                        _searchResults[search] = new Ticket[0];

                        _searchThreads.TryAdd(search, new Thread(StartSearchThread)); //ha már hozzá van adva az bug..
                       
                        _searchThreads[search].IsBackground = true;
                        _searchThreads[search].Start(search);

                        ++_activeSearches;
                        Thread.Sleep(SearchDelay);
                    }
                }).Start();
        }

        public void CancelSearches()
        {
            ClearSearches();
            _searchProgresses.Clear();
            _activeSearches = 0;
            while(_searchThreads.Count>0)
            {
                Thread tmpThread;
                if(_searchThreads.TryRemove(_searchThreads.First().Key, out tmpThread))
                    tmpThread.Abort();
            }
        }

        public void AddSearch(Search nSearch)
        {
            _searchQueue.Enqueue(nSearch);
        }

        public void ClearSearches()
        {
            Search tmpSearch;
            while (_searchQueue.Count > 0) _searchQueue.TryDequeue(out tmpSearch);
        }

        public double GetProgressPercent()
        {
            if (_searchProgresses.Count == 0) return 100;
            return _searchProgresses.Sum(a => a.Value) * 1.25 / (_searchQueue.Count + _searchProgresses.Count);
        }

        public IDictionary<Search, Ticket[]> GetResults()
        {
            return (IDictionary<Search, Ticket[]>)_searchResults ?? new Dictionary<Search, Ticket[]>();
        }

        public void Dispose()
        {
            CancelSearches();
        }

        #endregion

        #region Individual Search

        private void StartSearchThread(object obj)
        {
            StartSearch(obj as Search);
        }

        private void StartSearch(Search searchInp)
        {
            var browser = new BrowserSimulator("http://liligo.hu");
            browser.DocumentReady += browser_DocumentReady;
            browser.Source = new Uri("http://liligo.hu");

            while (!browser.IsDocumentReady)
            {
                Thread.Sleep(100);
            }
            Thread.Sleep(1000);
            BrowserSimulator.Update();
            //((Awesomium.Core.BitmapSurface)_searchBrowsers[searchInp].Surface).SaveToJPEG(searchInp.From + "-" + searchInp.To + "-before-input.jpg");

            browser.ClickElementPersistent("air-from");
            browser.Type(searchInp.From);
            Thread.Sleep(1000);
            _searchProgresses[searchInp] += 2;
            browser.ClickElementPersistent("air-to");
            browser.Type(searchInp.To);
            Thread.Sleep(1000);
            _searchProgresses[searchInp] += 2;
            browser.ClickElementPersistent("air-out-date");
            browser.ExecuteJavascriptWithResult("liligo.DatePicker2.actual.select(Date.parse('" + searchInp.Date.ToShortDateString() + "'),false)");
            Thread.Sleep(1000);
            _searchProgresses[searchInp] += 2;

            if (searchInp.RetDate.HasValue)
            {
                browser.ClickElementPersistent("air-ret-date");
                browser.ExecuteJavascriptWithResult("liligo.DatePicker2.actual.select(Date.parse('" + searchInp.RetDate.Value.ToShortDateString() + "'),false)");
            }
            else
            {
                browser.ClickElementPersistent("air-subcategory-oneway");
            }

            browser.ExecuteJavascriptWithResult("document.getElementById('air-adults').value=" + searchInp.Adults + ";");
            browser.ExecuteJavascriptWithResult("document.getElementById('air-children').value=" + searchInp.Children + ";");
            browser.ExecuteJavascriptWithResult("document.getElementById('air-infants').value=" + searchInp.Infants + ";");

            Thread.Sleep(1000);
            _searchProgresses[searchInp] += 2;
            browser.ClickElementPersistent("air-flexibility");
            //((Awesomium.Core.BitmapSurface)_searchBrowsers[searchInp].Surface).SaveToJPEG(searchInp.From + "-"+searchInp.To+"-after-input.jpg");
            browser.ClickElementPersistent("air-submit");
            browser.ClickElementPersistent("air-submit");
            ((Awesomium.Core.BitmapSurface)browser.Surface).SaveToJPEG(searchInp.From + "-" + searchInp.To + "-after-click.jpg");
            ResultCheckLoop(searchInp, browser);
        }

        private void ResultCheckLoop(Search searchInp, BrowserSimulator browser)
        {
            while (
                   !(browser.IsDocumentReady && browser.Source.AbsoluteUri == "http://www.liligo.hu/air/SearchFlights.jsp"))
            {
                Thread.Sleep(100);
            }

            Thread.Sleep(500);

            bool end = false;
            while (!end)
            {
                ((Awesomium.Core.BitmapSurface)browser.Surface).SaveToJPEG(searchInp.From + "-" + searchInp.To + "-results.jpg");
                Awesomium.Core.JSValue[] res = browser.ExecuteJavascriptWithResult("objArrayToString(getResults());");
                if (res.Count() > 0 && !res[0].IsUndefined)
                    _searchResults[searchInp] = (from r in res select new Ticket("liligo", r.ToString())).ToArray();
                else _searchResults[searchInp] = new Ticket[0];

                var isOver = browser.ExecuteJavascriptWithResult(
                        "(function(){return document.getElementsByClassName('stopped-finished').length == 1})()");
                if (isOver.IsBoolean && (bool)isOver)
                {
                    _searchProgresses[searchInp] = 80;
                    --_activeSearches;
                    end = true;
                }
                else if (_searchProgresses[searchInp] < 79) _searchProgresses[searchInp]++;
            }
            Thread ignored;
            _searchThreads.TryRemove(searchInp, out ignored);
            if (_searchQueue.Count > 0) StartSearches();
            
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
        
        #endregion
    }
}
