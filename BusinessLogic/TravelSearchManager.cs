using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic
{
    public class TravelSearchManager
    {
        private static List<List<Ticket>> Combine(List<List<Ticket>> lists, IEnumerable<Ticket> nList)
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

#region Fields
        public List<Travel> FlightList { get; private set; }

        //private readonly Dictionary<Tuple<Search,DateTime>, List<Ticket>> _cache = new Dictionary<Tuple<Search, DateTime>, List<Ticket>>();

        private readonly ITravelSearchEngine[] _searchEngines;

        public bool IsSearchInProgress { get; private set;}

        private List<List<Search>> _searchPlans;
#endregion

        public TravelSearchManager(ITravelSearchEngine[] searchEngines)
        {
            _searchEngines = searchEngines;
            foreach (var engine in searchEngines)
            {
                engine.Initialize();
            }

            FlightList = new List<Travel>();
        }

        public void AddTravel(string @from,string to, DateTime date, int adults = 1, int children = 0, int infants = 0)
        {
            if(!IsSearchInProgress) FlightList.Add(new Travel(from, to, date, adults, children, infants));
        }

        public void RemoveTravel(int ind)
        {
            if(!IsSearchInProgress) FlightList.RemoveAt(ind);
        }

        public void CancelSearch()
        {
            IsSearchInProgress = false;

            foreach (var engine in _searchEngines)
            {
                engine.CancelSearches();
            }
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

        public void StartSearch(/*bool forceRefresh = false*/)
        {
            if (FlightList.Count == 0) return;

            _searchPlans = GetSearchPlans();

            var reqSearches = _searchPlans.Aggregate((flat, a)=>flat.Union(a).ToList());

            foreach (var engine in _searchEngines)
            {
                engine.ClearSearches();
                foreach (var search in reqSearches)
                {
                    //if (forceRefresh ||
                        //!_cache.Any(a => Equals(a.Key.Item1, search) && a.Key.Item2 >= DateTime.Now.AddMinutes(10)))
                        engine.AddSearch(search);
                }
                engine.StartSearches();
            }
        }


        public List<ResultSet> GetResults()
        {
            var resultSets = new Dictionary<Travel, List<Ticket>>();
            var retVal = new List<ResultSet>();

            if (_searchPlans.Count == 0) return retVal;

            var tmp = new Dictionary<ITravelSearchEngine, IDictionary<Search, ResultSet>>();
            foreach (var engine in _searchEngines)
                tmp[engine] = engine.GetResults();

            foreach (var search in _searchPlans.Aggregate((w,a)=> w.Union(a).ToList()))
            {
                resultSets.Add(search, new List<Ticket>());
                foreach (var engine in _searchEngines)
                {
                    if (tmp[engine].ContainsKey(search))
                    {
                        resultSets[search].AddRange(tmp[engine][search].Tickets.OrderBy(a => a.Price));
                    }
                }
                /*
                if (resultSets[search].Count != 0)
                    _cache.Add(new Tuple<Search, DateTime>(search, DateTime.UtcNow), resultSets[search]);
                else
                {
                    if (_cache.Any(a=> Equals(a.Key.Item1, search) && a.Key.Item2 >= DateTime.Now.AddHours(-1)))
                        resultSets[search] = _cache.Last(a=>Equals(a.Key.Item1,search)).Value;
                }*/
            }

            foreach (var plan in _searchPlans)
            {
                var result = plan.Aggregate(new List<List<Ticket>> {new List<Ticket>()}, (current, search) => Combine(current, resultSets[search]));

                retVal.AddRange(result.Select(a=> new ResultSet("liligo.hu", a.ToArray()) ));
            }

            retVal = retVal.OrderBy(a => a.SumPrice).ToList();

            return retVal.Take(100).ToList();
        }

        public int GetProgress()
        {
            return (int) Math.Floor(_searchEngines.Average(c => c.GetProgressPercent()));
        }
    }
}
