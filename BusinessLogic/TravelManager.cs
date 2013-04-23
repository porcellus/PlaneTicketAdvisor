using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic
{
    public class TravelManager
    {
        public List<Travel> FlightList { get; private set; }

        private readonly Func<ITravelSearchEngine>[] _engineFactories;

        private readonly Dictionary<Search, List<ITravelSearchEngine>> _searches;

        public TravelManager(Func<ITravelSearchEngine>[] engineFactories)
        {
            _engineFactories = engineFactories;

            FlightList = new List<Travel>();
            _searches = new Dictionary<Search, List< ITravelSearchEngine>>();
        }

        public void AddTravel(string @from,string to, DateTime date, int adults = 1, int children = 0, int infants = 0)
        {
            FlightList.Add(new Travel(from, to, date, adults, children, infants));
        }

        public List<List<Search>> GetSearchPlans(bool[] checks = null)
        {
            if(checks==null) checks = Enumerable.Repeat(false, FlightList.Count).ToArray();
            var first = Array.IndexOf(checks,false);

            if (first == -1) return new List<List<Search>> { new List<Search>() };

            checks[first] = true;

            var retVal = GetSearchPlans(checks);
            retVal.ForEach(
                a =>
                a.Add(new Search(FlightList[first].From, FlightList[first].To, FlightList[first].Date,
                                 null,
                                 FlightList[first].Adults, FlightList[first].Children, FlightList[first].Infants)
                    ));

            for (int i = first+1; i < FlightList.Count; ++i)
            {
                if (!checks[i] && FlightList[i].To == FlightList[first].From && FlightList[i].From == FlightList[first].To &&
                    FlightList[i].Adults == FlightList[first].Adults && FlightList[i].Children == FlightList[first].Children &&
                    FlightList[i].Infants == FlightList[first].Infants && FlightList[i].Date >= FlightList[first].Date)
                {
                    checks[i] = true;
                    var rec = GetSearchPlans(checks);
                    rec.ForEach(
                        a =>
                        a.Add(new Search(FlightList[first].From, FlightList[first].To, FlightList[first].Date,
                                         FlightList[i].Date,
                                         FlightList[first].Adults, FlightList[first].Children, FlightList[first].Infants)
                            ));
                    retVal.AddRange(rec);

                    checks[i] = false;
                }
            }
            checks[first] = false;
            //if (checks.All(a => a))

            return retVal;
        }

        public void StartSearch()
        {
            if (FlightList.Count == 0) return;
            var itiner = new List<Search>();
            for (var i = 0; i < FlightList.Count; ++i)
            {
                var search = new Search(FlightList[i].From, FlightList[i].To, FlightList[i].Date, null,
                                        FlightList[i].Adults, FlightList[i].Children, FlightList[i].Infants);
                if (i+1<FlightList.Count&&FlightList[i + 1].To == search.From && FlightList[i + 1].From == search.To &&
                    FlightList[i + 1].Adults == search.Adults && FlightList[i + 1].Children == search.Children && FlightList[i + 1].Infants == search.Infants)
                {
                    search.RetDate = FlightList[i+1].Date;
                    ++i;
                }
                itiner.Add(search);
            }

            foreach (var engineFactory in _engineFactories) //TODO: Csoportositas...
            {
                foreach (var search in itiner)
                {
                    var searchEngine = engineFactory();
                    searchEngine.Initialize();
                    searchEngine.StartSearch(search);
                    if(!_searches.ContainsKey(search)) _searches[search] = new List<ITravelSearchEngine>();
                    _searches[search].Add(searchEngine);
                }
            }
        }

        public List<List<Ticket>> Combine(List< List<Ticket>> lists, List<Ticket> nList)
        {
            var retVal = new List<List<Ticket>>();

            foreach (var ticket in nList)
            {
                foreach (var list in lists)
                {
                    var addList = list.ToList();
                    addList.Add(ticket);
                    retVal.Add(addList);
                }
            }

            return retVal;
        }

        public List<List<Ticket>> GetResults()
        {
            var resultSets = new Dictionary< Travel ,List<Ticket>>();
            foreach (var search in _searches)
            {
                resultSets.Add(search.Key,new List<Ticket>());
                foreach (var searchEngine in search.Value)
                    resultSets[search.Key].AddRange(searchEngine.GetResults());
            }
            var resultSetIt = resultSets.GetEnumerator();
            var retVal = resultSetIt.Current.Value.Select(a => new List<Ticket> { a }).ToList();

            while (resultSetIt.MoveNext())
            {
                retVal = Combine(retVal, resultSetIt.Current.Value);
            }

            return retVal;
        }

        public int GetProgress()
        {
            if (_searches.Count == 0) return 0;
            return (int) Math.Floor(_searches.Average(search => search.Value.Average(eng => eng.GetProgressPercent())));
        }
    }
}
