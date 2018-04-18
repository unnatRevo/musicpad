using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class CreateEventPython : BaseRequest
    {

        public string eventName { get; set; }
        public string location { get; set; }
        public string startDate { get; set; }
        public string eventDescription { get; set; }
        public string categoryName { get; set; }
        public string filepath { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string startTime { get; set; }
        public bool isTmaster { get; set; }
        public string Url { get; set; }

        public CreateEventPython() : base()
        {

        }
    }
}