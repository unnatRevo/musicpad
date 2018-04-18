using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class CreateEventRequest : BaseRequest
    {
        public int eventID { get; set; }
        public int userID { get; set; }
        public string eventName { get; set; }
        public string location { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string eventDescription { get; set; }
        public int categoryID { get; set; }
        public string filepath { get; set; }
        public string FileName { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }

        public string dealtext { get; set; }
        public string percentage { get; set; }
        public string accessToken { get; set; }

        public CreateEventRequest() : base()
        {

        }
    }
}