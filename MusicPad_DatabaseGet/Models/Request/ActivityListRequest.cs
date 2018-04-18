using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class ActivityListRequest : BaseRequest
    {
        public int userId { get; set; }
        public int eventId { get; set; }
        public int pageNumber { get; set; }
        public string accessToken { get; set; }

        public ActivityListRequest() : base()
        {

        }
    }
}