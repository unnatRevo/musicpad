using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class EventStartDateEndDateRequest : BaseRequest
    {
        public int categoryId { get; set; }
        public int userID { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string cityName { get; set; }
        public string accessToken { get; set; }
        public int pageNumber { get; set; }

        public EventStartDateEndDateRequest() : base()
        {

        }
    }
}