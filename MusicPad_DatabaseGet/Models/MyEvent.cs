using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models
{
    public class MyEvent
    {
        public int EventID { get; set; }
        public int UserID { get; set; }
        public string CategoryName { get; set; }
        public string EventName { get; set; }
        public string EventDiscription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Starttime { get; set; }
        public string Endtime { get; set; }
        public string Dealtext { get; set; }
        public string Percentage { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string location { get; set; }
        public string Image { get; set; }
        public string TicketUrl { get; set; }
        public bool IsTmaster { get; set; }
        public int  TotalCount { get; set; }
    }
}