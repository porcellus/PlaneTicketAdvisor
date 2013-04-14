using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLogic
{
        public class Ticket
        {
            public long Price { get; set; }
            public long Perprice { get; set; }

            public string OutCompany { get; set; }
            public string OutStartStation { get; set; }
            public DateTime OutStartTime { get; set; }
            public string OutArriveStation { get; set; }
            public DateTime OutArriveTime { get; set; }

            public string RetCompany { get; set; }
            public string BackStartStation { get; set; }
            public DateTime BackStartTime { get; set; }
            public string BackArriveStation { get; set; }
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

    public interface ITravelSearchEngine
    {
        void Initialize();
        void StartSearch(string from, string to, DateTime depDate, DateTime? retDate, int adults = 0,
                                int children = 0, int infants = 0);

        double GetProgressPercent();
        IEnumerable<Ticket> GetResults();
    }

    public class TravelManager
    {
        public struct Travel
        {
            public string To { get; set; }
            public string From { get; set; }
            public DateTime Date { get; set; }
            public int Adults { get; set; }
            public int Children { get; set; }
            public int Infants { get; set; }

            public Travel(string to, string @from, DateTime date, int adults = 1, int children = 0, int infants = 0) : this()
            {
                To = to;
                From = @from;
                Date = date;
                Adults = adults;
                Children = children;
                Infants = infants;
            }
        }

        public List<Travel> Travels { get; set; }

        private readonly ITravelSearchEngine[] _engines;

        public TravelManager(ITravelSearchEngine[] engines)
        {
            _engines = engines;

            Travels = new List<Travel>();

            foreach (var searchEngine in _engines)
                searchEngine.Initialize();
        }

        public void AddTravel(string to, string @from, DateTime date, int adults = 1, int children = 0, int infants = 0)
        {
            Travels.Add(new Travel(to,from,date,adults,children,infants));
        }

        public void StartSearch()
        {
            if (Travels.Count == 0) return;
            foreach (var searchEngine in _engines) //TODO: Csoportositas...
            {
                var travel = Travels.First();
                var retDate = Travels[1].To == travel.From && Travels[1].From == travel.To
                                        ? (DateTime?) Travels[1].Date
                                        : null;
                searchEngine.StartSearch(travel.From, travel.To, travel.Date, retDate, travel.Adults, travel.Children,
                                         travel.Infants);
            }
        }
        
        public Ticket[] GetResults()
        {
            var retVal = new List<Ticket>();

            foreach (var searchEngine in _engines)
                retVal.AddRange(searchEngine.GetResults());

            return retVal.Distinct().ToArray();
        }

        public int GetProgress()
        {
            return (int) Math.Floor(_engines.Average(searchEngine => searchEngine.GetProgressPercent()));
        }
    }
}
