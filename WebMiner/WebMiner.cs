using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Awesomium.Core;
using System.Threading;

namespace WebMiner
{
    public class Ticket
    {
        public long price { get; set; }
        public long perprice { get; set; }

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
    }
    public class WebMiner
    {
        public static Ticket TicketFromString(string str)
        {
            var retVal = new Ticket();
            str=str.Trim(" {}".ToCharArray());
            var props=str.Split(',');

            string[] tmp;

            foreach (var p in props)
            {
                tmp = p.Split('=');
                if (tmp.Length == 2)
                {
                    tmp[0] = tmp[0].Trim();
                    tmp[1] = tmp[1].Trim(" '".ToCharArray());
                    switch (tmp[0])
                    {
                        case "fullprice": retVal.price = long.Parse(tmp[1].Replace(" ", "")); break;
                        case "perprice": retVal.perprice = long.Parse(tmp[1].Replace(" ", "")); break;
        
                        case "outCompany": retVal.OutCompany = tmp[1]; break;
                        case "backCompany": retVal.RetCompany = tmp[1]; break;

                        case "outStartStation": retVal.OutStartStation = tmp[1]; break;
                        case "backStartStation": retVal.BackStartStation = tmp[1]; break;

                        case "outStartTime": retVal.OutStartTime = DateTime.Parse(tmp[1]); break;
                        case "backStartTime": retVal.BackStartTime = DateTime.Parse(tmp[1]); break;

                        case "outArriveTime": retVal.OutArriveTime = DateTime.Parse(tmp[1]); break;
                        case "backArriveTime": retVal.BackArriveTime = DateTime.Parse(tmp[1]); break;
                        case "outArriveStation": retVal.OutArriveStation = tmp[1]; break;
                        case "backArriveStation": retVal.BackArriveStation = tmp[1]; break;
         
                    }
                }
            }

            return retVal;
        }
    }
}
