using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace BusinessLogic
{
    public class Ticket
    {
        [DisplayName("Ár")]
        public long Price { get; set; }
        [DisplayName("Ár/fő")]
        public long Perprice { get; set; }

        [DisplayName("Légitársaság-oda")]
        public string OutCompany { get; set; }

        [DisplayName("Induló reptér-oda")]
        public string OutStartStation { get; set; }

        [DisplayName("Indulási idő-oda")]
        public DateTime OutStartTime { get; set; }

        [DisplayName("Erkezési állomás-oda")]
        public string OutArriveStation { get; set; }

        [DisplayName("Erkezési idő-oda")]
        public DateTime OutArriveTime { get; set; }

        [DisplayName("Légitársaság-vissza")]
        public string RetCompany { get; set; }

        [DisplayName("Indulási reptér-vissza")]
        public string BackStartStation { get; set; }

        [DisplayName("Indulási idő-vissza")]
        public DateTime BackStartTime { get; set; }

        [DisplayName("Erkezési állomás-vissza")]
        public string BackArriveStation { get; set; }

        [DisplayName("Erkezési idő-vissza")]
        public DateTime BackArriveTime { get; set; }

        public Ticket(string str)
        {
            str = str.Trim(" {}".ToCharArray());
            var props = str.Split(',');

            foreach (var tmp in props.Select(p => p.Split('=')).Where(tmp => tmp.Length == 2))
            {
                tmp[0] = tmp[0].Trim();
                tmp[1] = tmp[1].Trim(" '".ToCharArray());
                switch (tmp[0])
                {
                    case "fullprice": Price = long.Parse(tmp[1].Replace(" ", "")); break;
                    case "perprice": Perprice = long.Parse(tmp[1].Replace(" ", "")); break;

                    case "outCompany": OutCompany = tmp[1]; break;
                    case "backCompany": RetCompany = tmp[1]; break;

                    case "outStartStation": OutStartStation = tmp[1]; break;
                    case "backStartStation": BackStartStation = tmp[1]; break;

                    case "outStartTime": OutStartTime = DateTime.Parse(tmp[1]); break;
                    case "backStartTime": BackStartTime = DateTime.Parse(tmp[1]); break;

                    case "outArriveTime": OutArriveTime = DateTime.Parse(tmp[1]); break;
                    case "backArriveTime": BackArriveTime = DateTime.Parse(tmp[1]); break;
                    case "outArriveStation": OutArriveStation = tmp[1]; break;
                    case "backArriveStation": BackArriveStation = tmp[1]; break;
                }
            }
        }
    }
    public class Travel
    {
        [DisplayName("Honnan")]
        public string From { get; set; }
        [DisplayName("Hova")]
        public string To { get; set; }
        [DisplayName("Mikor")]
        public DateTime Date { get; set; }
        [DisplayName("Felnőttek")]
        public int Adults { get; set; }
        [DisplayName("Gyerekek")]
        public int Children { get; set; }
        [DisplayName("Csecsmők")]
        public int Infants { get; set; }

        public Travel(string to, string @from, DateTime date, int adults = 1, int children = 0, int infants = 0)
        {
            To = to;
            From = @from;
            Date = date;
            Adults = adults;
            Children = children;
            Infants = infants;
        }
    }
    public class Search: Travel
        {
            public DateTime? RetDate;

            public Search(string to, string @from, DateTime date, DateTime? retDate = null, int adults = 1, int children = 0, int infants = 0)
                : base(to, @from, date, adults, children, infants)
            {
                RetDate = retDate;
            }
        }

    public interface ITravelSearchEngine
    {
        void Initialize();
        void StartSearch(Search inp);

        double GetProgressPercent();
        IEnumerable<Ticket> GetResults();
    }

    public class TravelManager
    {
        public List<Travel> Travels { get; set; }

        private readonly Func<ITravelSearchEngine>[] _engineFactories;

        private Dictionary<Search, ITravelSearchEngine> Searches;

        public TravelManager(Func<ITravelSearchEngine>[] engineFactories)
        {
            _engineFactories = engineFactories;

            Travels = new List<Travel>();
            Searches = new Dictionary<Search, ITravelSearchEngine>();
        }

        public void AddTravel(string to, string @from, DateTime date, int adults = 1, int children = 0, int infants = 0)
        {
            Travels.Add(new Travel(to,from,date,adults,children,infants));
        }

        public void StartSearch()
        {
            if (Travels.Count == 0) return;
            foreach (var engineFactory in _engineFactories) //TODO: Csoportositas...
            {
                var searchEngine = engineFactory();
                searchEngine.Initialize();
                var search = new Search(Travels[0].From,Travels[0].To,Travels[0].Date,null,Travels[0].Adults, Travels[0].Children, Travels[0].Infants);
                search.RetDate = Travels[1].To == search.From && Travels[1].From == search.To
                                        ? (DateTime?) Travels[1].Date
                                        : null;
                searchEngine.StartSearch(search);
                Searches[search] = searchEngine;
            }
        }
        
        public Ticket[] GetResults()
        {
            var retVal = new List<Ticket>();

            foreach (var search in Searches)
                retVal.AddRange(search.Value.GetResults());

            return retVal.Distinct().ToArray();
        }

        public int GetProgress()
        {
            return (int) Math.Floor(Searches.Average(search => search.Value.GetProgressPercent()));
        }
    }
}
