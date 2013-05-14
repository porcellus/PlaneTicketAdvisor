using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using BusinessLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{

    [TestClass]
    public class UnitTest1
    {
        private class MockSearchEngine : ITravelSearchEngine
        {
            public bool searchinprogress=false;
            public List<Search> searches = new List<Search>();
            public int id;

            public MockSearchEngine(int pid)
            {
                id = pid;
            }

            public void Initialize()
            {
            }

            public void StartSearches()
            {
                searchinprogress = true;
            }

            public void CancelSearches()
            {
                searchinprogress = false;
            }

            public void AddSearch(Search nSearch)
            {
                searches.Add(nSearch);
            }

            public void ClearSearches()
            {
                searches.Clear();
            }

            public double GetProgressPercent()
            {
                return id*10;
            }

            public IDictionary<Search, Ticket[]> GetResults()
            {
                var retVal = new Dictionary<Search,Ticket[]>();
                foreach (var search in searches)
                {
                    var testTicket = new Ticket("testEngine" + id);
                    testTicket.OutStartStation = search.From;
                    testTicket.OutArriveStation = search.To;
                    testTicket.OutStartDate = search.Date;
                    if(search.RetDate.HasValue) testTicket.BackStartDate = search.RetDate.Value;
                    retVal.Add(search, new[] { testTicket });
                }

                return retVal;
            }

            public void Dispose()
            {
            }
        }

        [TestMethod]
        public void AddTravelTest()
        {
            var myTravelManager = new TravelSearchManager(new ITravelSearchEngine[0]);

            var travels = new List<Travel>{
                new Travel("London", "Budapest", DateTime.Today.AddDays(4)),
                new Travel("Budapest", "London", DateTime.Today.AddDays(10)),
                new Travel("London", "Budapest", DateTime.Today.AddDays(11)),
                new Travel("Budapest", "London", DateTime.Today.AddDays(20)),
                new Travel("London", "Budapest", DateTime.Today.AddDays(21))
            };
            foreach (var t in travels)
                myTravelManager.AddTravel(t);
            myTravelManager.AddTravel("Budapest", "London", DateTime.Today.AddDays(3));
            Debug.Assert(myTravelManager.Travels.Count == 6);
            Debug.Assert(
                    myTravelManager.Travels[0].Equals(travels[0]) &&
                    myTravelManager.Travels[1].Equals(travels[1]) &&
                    myTravelManager.Travels[2].Equals(travels[2]) &&
                    myTravelManager.Travels[3].Equals(travels[3]) &&
                    myTravelManager.Travels[4].Equals(travels[4]) &&
                    myTravelManager.Travels[5].Equals(new Travel("Budapest", "London", DateTime.Today.AddDays(3))));
        }

        [TestMethod]
        public void ClearTravelTest()
        {
            var myTravelManager = new TravelSearchManager(new ITravelSearchEngine[0]);

            var travels = new List<Travel>{
                new Travel("Budapest", "London", DateTime.Today.AddDays(3)),
                new Travel("London", "Budapest", DateTime.Today.AddDays(4)),
                new Travel("Budapest", "London", DateTime.Today.AddDays(10)),
                new Travel("London", "Budapest", DateTime.Today.AddDays(11)),
                new Travel("Budapest", "London", DateTime.Today.AddDays(20)),
                new Travel("London", "Budapest", DateTime.Today.AddDays(21))
            };

            foreach(var t in travels)
                myTravelManager.AddTravel(t);

            myTravelManager.RemoveTravel(0);
            Debug.Assert(myTravelManager.Travels.Count == 5 
                    && myTravelManager.Travels[0].Equals(travels[1])
                    && myTravelManager.Travels[1].Equals(travels[2])
                    && myTravelManager.Travels[2].Equals(travels[3])
                    && myTravelManager.Travels[3].Equals(travels[4])
                    && myTravelManager.Travels[4].Equals(travels[5]));
        }


        [TestMethod]
        public void StartSearchTest()
        {
            var mockEngine = new MockSearchEngine(1);
            var myTravelManager = new TravelSearchManager(new []{ mockEngine});

            var travels = new List<Travel>{
                new Travel("Budapest", "London", DateTime.Today.AddDays(3)),
                new Travel("London", "Budapest", DateTime.Today.AddDays(4))
            };

            foreach (var t in travels)
                myTravelManager.AddTravel(t);

            myTravelManager.StartSearch();

            Debug.Assert(mockEngine.searchinprogress);
            Debug.Assert(mockEngine.searches.Count == 3);
        }


        [TestMethod]
        public void CancelSearchTest()
        {
            var mockEngine = new MockSearchEngine(1);
            var myTravelManager = new TravelSearchManager(new[] { mockEngine });

            var travels = new List<Travel>{
                new Travel("Budapest", "London", DateTime.Today.AddDays(3)),
                new Travel("London", "Budapest", DateTime.Today.AddDays(4))
            };

            foreach (var t in travels)
                myTravelManager.AddTravel(t);

            myTravelManager.StartSearch();

            myTravelManager.CancelSearch();

            Debug.Assert(!mockEngine.searchinprogress);
        }
        
        [TestMethod]
        public void GetProgressTest()
        {
            var mockEngine = new MockSearchEngine(1);
            var mockEngine2 = new MockSearchEngine(2);
            var myTravelManager = new TravelSearchManager(new[] { mockEngine, mockEngine2 });

            var travels = new List<Travel>{
                new Travel("Budapest", "London", DateTime.Today.AddDays(3)),
                new Travel("London", "Budapest", DateTime.Today.AddDays(4))
            };

            foreach (var t in travels)
                myTravelManager.AddTravel(t);

            myTravelManager.StartSearch();
            Debug.Assert(myTravelManager.GetProgress() == 15);
        }


        [TestMethod]
        public void GetResultTest()
        {
            var mockEngine = new MockSearchEngine(1);
            var myTravelManager = new TravelSearchManager(new[] { mockEngine });

            var travels = new List<Travel>{
                new Travel("Budapest", "London", DateTime.Today.AddDays(3)),
                new Travel("London", "Budapest", DateTime.Today.AddDays(4))
            };

            foreach (var t in travels)
                myTravelManager.AddTravel(t);

            myTravelManager.StartSearch();
            var results = myTravelManager.GetResults();

            Debug.Assert(
                results.Count == 2);
        }

        [TestMethod]
        public void GetResultTest2()
        {
            var mockEngine = new MockSearchEngine(1);
            var mockEngine2 = new MockSearchEngine(2);
            var myTravelManager = new TravelSearchManager(new[] { mockEngine});

            var travels = new List<Travel>{
                new Travel("Budapest", "London", DateTime.Today.AddDays(3)),
                new Travel("London", "Budapest", DateTime.Today.AddDays(4)),
                new Travel("Budapest", "London", DateTime.Today.AddDays(20)),
                new Travel("London", "Budapest", DateTime.Today.AddDays(22))
            };

            foreach (var t in travels)
                myTravelManager.AddTravel(t);

            myTravelManager.StartSearch();
            var results = myTravelManager.GetResults().ToList();

            Debug.Assert(results.Count == 7 &&
                results.Count(a=>a.TicketCount == 4) == 1 &&
                results.Count(a=>a.TicketCount == 3) == 4 &&
                results.Count(a=>a.TicketCount == 2) == 2);
        }

        [TestMethod]
        public void GetResultTest3()
        {
            var mockEngine = new MockSearchEngine(1);
            var mockEngine2 = new MockSearchEngine(2);
            var myTravelManager = new TravelSearchManager(new[] { mockEngine, mockEngine2 });

            var travels = new List<Travel>{
                new Travel("Budapest", "London", DateTime.Today.AddDays(3)),
                new Travel("London", "Budapest", DateTime.Today.AddDays(4)),
                new Travel("Budapest", "London", DateTime.Today.AddDays(20)),
                new Travel("London", "Budapest", DateTime.Today.AddDays(22))
            };

            foreach (var t in travels)
                myTravelManager.AddTravel(t);

            myTravelManager.StartSearch();
            var results = myTravelManager.GetResults().ToList();

            Debug.Assert(results.Count == 56 &&
                results.Count(a => a.TicketCount == 4) == 16 &&
                results.Count(a => a.TicketCount == 3) == 32 &&
                results.Count(a => a.TicketCount == 2) == 8);
        }
    }
}
