using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BusinessLogic
{
    public class Ticket
    {
        public string EngineName;

        public long Price { get; set; }
        public long Perprice { get; set; }

        public string OutCompany { get; set; }

        public string OutStartStation { get; set; }

        public DateTime OutStartDate { get; set; }
        public TimeSpan OutStartTime { get; set; }

        public string OutArriveStation { get; set; }

        public TimeSpan OutArriveTime { get; set; }

        public string RetCompany { get; set; }

        public string BackStartStation { get; set; }

        public DateTime BackStartDate { get; set; }
        public TimeSpan BackStartTime { get; set; }

        public string BackArriveStation { get; set; }

        public TimeSpan BackArriveTime { get; set; }

        public int OutStops { get; set; }
        public int BackStops { get; set; }

        public TimeSpan OutTravelTime { get; set; }
        public TimeSpan BackTravelTime { get; set; }

        public Ticket(string engineName,string str="")
        {
            EngineName = engineName;
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
                        
                    case "outStartDate": OutStartDate = DateTime.Parse(tmp[1]); break;
                    case "outStartTime": OutStartTime = TimeSpan.Parse(tmp[1]); break;

                    case "backStartDate": BackStartDate = DateTime.Parse(tmp[1]); break;
                    case "backStartTime": BackStartTime = TimeSpan.Parse(tmp[1]); break;

                    case "outArriveTime": OutArriveTime = TimeSpan.Parse(tmp[1]); break;
                    case "backArriveTime": BackArriveTime = TimeSpan.Parse(tmp[1]); break;

                    case "outArriveStation": OutArriveStation = tmp[1]; break;
                    case "backArriveStation": BackArriveStation = tmp[1]; break;

                    case "outStops ": OutStops = int.Parse(tmp[1]);break;
                    case "backStops ": BackStops = int.Parse(tmp[1]); break;

                    case "outTravelTime": OutTravelTime = new TimeSpan(int.Parse(tmp[1].Split(':')[0]), int.Parse(tmp[1].Split(':')[1]), 0); break;
                    case "backTravelTime": BackTravelTime = new TimeSpan(int.Parse(tmp[1].Split(':')[0]), int.Parse(tmp[1].Split(':')[1]), 0); break;
                }
            }
        }
    }

    public class Travel : IEquatable<Travel>
    {
        #region Equatable
        public bool Equals(Travel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(From, other.From) && string.Equals(To, other.To) && Date.Equals(other.Date) && Adults == other.Adults && Children == other.Children && Infants == other.Infants;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Travel) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (From != null ? From.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (To != null ? To.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ Date.GetHashCode();
                hashCode = (hashCode*397) ^ Adults;
                hashCode = (hashCode*397) ^ Children;
                hashCode = (hashCode*397) ^ Infants;
                return hashCode;
            }
        }
        #endregion

        public string From { get; set; }
        public string To { get; set; }
        public DateTime Date { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public int Infants { get; set; }

        public Travel(string @from, string to, DateTime date, int adults = 1, int children = 0, int infants = 0)
        {
            From = @from;
            To = to;
            Date = date;
            Adults = adults;
            Children = children;
            Infants = infants;
        }
    }
    public class Search : Travel, IEquatable<Search>
    {
        #region Equatable
        public bool Equals(Search other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return base.Equals(other) && RetDate.Equals(other.RetDate);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Search) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode()*397) ^ RetDate.GetHashCode();
            }
        }
        #endregion

        public DateTime? RetDate;

        public Search(string @from, string to, DateTime date, DateTime? retDate = null, int adults = 1, int children = 0, int infants = 0)
            : base(@from, to, date, adults, children, infants)
        {
            RetDate = retDate;
        }
    }

    public struct ResultSet : IEquatable<ResultSet>
    {
        #region IEquatable

        public bool Equals(ResultSet other)
        {
            return Equals(_tickets, other._tickets);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ResultSet && Equals((ResultSet) obj);
        }

        public override int GetHashCode()
        {
            return (_tickets != null ? _tickets.GetHashCode() : 0);
        }

        public static bool operator ==(ResultSet left, ResultSet right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ResultSet left, ResultSet right)
        {
            return !left.Equals(right);
        }

        #endregion

        private Ticket[] _tickets;

        public int TicketCount
        {
            get { return Tickets.Length <= 0 ? 0 : Tickets.Count(); }
        }

        public long SumPrice {
            get { return Tickets.Length <= 0 ? 0 : Tickets.Sum(a => a.Price); }
        }

        public int SumStops
        {
            get { return Tickets.Length <= 0 ? 0 : Tickets.Sum(a => a.OutStops + a.BackStops); }
        }

        public TimeSpan SumTravelTime
        {
            get { return Tickets.Length <= 0 ? new TimeSpan() : Tickets.Aggregate(new TimeSpan(), (cSpan, a) => cSpan + a.OutTravelTime + a.BackTravelTime); }
        }

        public string EngineName
        {
            get { return Tickets.Length <= 0 ? String.Empty : Tickets.Select(a => a.EngineName).Distinct().Aggregate((ws, c) => String.Concat(ws, c)); }
        }

        public Ticket[] Tickets
        {
            get { return _tickets ?? new Ticket[0]; }
            set { _tickets = value; }
        }

        public ResultSet(Ticket[] tickets = null)
        {
            _tickets = tickets;
        }
    }

    public interface ITravelSearchEngine: IDisposable
    {
        void Initialize();
        void StartSearches();
        void CancelSearches();
        void AddSearch(Search nSearch);
        void ClearSearches();

        double GetProgressPercent();
        IDictionary<Search, Ticket[]> GetResults();
    }
}