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
        private const int MaxResultCount = 30;

        public List<Travel> Travels { get; private set; }

        //private readonly Dictionary<Tuple<Search,DateTime>, List<Ticket>> _cache = new Dictionary<Tuple<Search, DateTime>, List<Ticket>>();

        private readonly ITravelSearchEngine[] _searchEngines;

        public bool IsSearchInProgress { get;
            private set;
        }

        private List<List<Search>> _searchPlans;
#endregion

        public TravelSearchManager(ITravelSearchEngine[] searchEngines)
        {
            _searchEngines = searchEngines;
            foreach (var engine in searchEngines)
            {
                engine.Initialize();
            }

            Travels = new List<Travel>();
        }

        public bool AddTravel(string @from,string to, DateTime date, int adults = 1, int children = 0, int infants = 0)
        {
            if(!IsSearchInProgress) Travels.Add(new Travel(from, to, date, adults, children, infants));
            return !IsSearchInProgress;
        }

        public bool AddTravel(Travel travel)
        {
            if (!IsSearchInProgress) Travels.Add(travel);
            return !IsSearchInProgress;

        }

        public bool RemoveTravel(int ind)
        {
            if (!IsSearchInProgress) Travels.RemoveAt(ind);
            return !IsSearchInProgress;
        }

        public void CancelSearch()
        {
            foreach (var engine in _searchEngines)
            {
                engine.CancelSearches();
            }
            IsSearchInProgress = false;
        }

        private List<List<Search>> GetSearchPlans(bool[] checks = null)
        {
            if(checks==null) checks = Enumerable.Repeat(false, Travels.Count).ToArray();
            var first = Array.IndexOf(checks,false);

            if (first == -1) return new List<List<Search>> { new List<Search>() };

            checks[first] = true;

            var retVal = GetSearchPlans(checks);
            retVal.ForEach(
                a =>
                a.Add(new Search(Travels[first].From, Travels[first].To, Travels[first].Date,
                                 null,
                                 Travels[first].Adults, Travels[first].Children, Travels[first].Infants)
                    ));

            for (int i = first+1; i < Travels.Count; ++i)
            {
                if (!checks[i] && Travels[i].To == Travels[first].From && Travels[i].From == Travels[first].To &&
                    Travels[i].Adults == Travels[first].Adults && Travels[i].Children == Travels[first].Children &&
                    Travels[i].Infants == Travels[first].Infants && Travels[i].Date >= Travels[first].Date)
                {
                    checks[i] = true;
                    var rec = GetSearchPlans(checks);
                    rec.ForEach(
                        a =>
                        a.Add(new Search(Travels[first].From, Travels[first].To, Travels[first].Date,
                                         Travels[i].Date,
                                         Travels[first].Adults, Travels[first].Children, Travels[first].Infants)
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
            if (Travels.Count == 0) return;
            IsSearchInProgress = true;

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

            var tmp = new Dictionary<ITravelSearchEngine, IDictionary<Search, Ticket[]>>();
            foreach (var engine in _searchEngines)
                tmp[engine] = engine.GetResults();

            foreach (var search in _searchPlans.Aggregate((w,a)=> w.Union(a).ToList()))
            {
                resultSets.Add(search, new List<Ticket>());
                foreach (var engine in _searchEngines)
                {
                    if (tmp[engine].ContainsKey(search))
                    {
                        resultSets[search].AddRange(tmp[engine][search].OrderBy(a => a.Price));
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

                retVal.AddRange(result.Select(a=> new ResultSet(a.ToArray())).ToList().OrderBy(a=> a.SumPrice).Take(MaxResultCount / plan.Count));
            }

            retVal = retVal.OrderBy(a => a.SumPrice).ToList();

            IsSearchInProgress &= (int)Math.Floor(_searchEngines.Average(c => c.GetProgressPercent())) < 100;

            return retVal.Take(100).ToList();
        }

        public int GetProgress()
        {
            var retVal = (int) Math.Floor(_searchEngines.Average(c => c.GetProgressPercent()));
            IsSearchInProgress &= retVal < 100;
            return retVal;
        }
    }
}
