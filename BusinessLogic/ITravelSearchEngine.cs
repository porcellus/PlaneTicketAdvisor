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
    public class Travel : IEquatable<Travel>
    {
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

        public Travel(string @from, string to, DateTime date, int adults = 1, int children = 0, int infants = 0)
        {
            To = to;
            From = @from;
            Date = date;
            Adults = adults;
            Children = children;
            Infants = infants;
        }
    }
    public class Search : Travel, IEquatable<Search>
    {
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

        public DateTime? RetDate;

        public Search(string @from, string to, DateTime date, DateTime? retDate = null, int adults = 1, int children = 0, int infants = 0)
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
}