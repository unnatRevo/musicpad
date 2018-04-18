using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class SearchEventRequest : BaseRequest
    {
        public int userId { get; set; }
        public string eventName { get; set; }
        public string accessToken { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public int pageNumber { get; set; }

        public SearchEventRequest() : base()
        {

        }
    }
}