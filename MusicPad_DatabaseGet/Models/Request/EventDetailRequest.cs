using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class EventDetailRequest : BaseRequest
    {
        public int userId { get; set; }
        public int eventId { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string accessToken { get; set; }

        public EventDetailRequest() : base()
        {

        }
    }
}