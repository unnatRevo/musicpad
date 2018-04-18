using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class EventListRequest : BaseRequest
    {
        public int userId { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public int categoryId { get; set; }
        public int pageNumber { get; set; }
        public int totalPageCount { get; set; }
        public string accessToken { get; set; }

        public EventListRequest() : base()
        {

        }
    }
}