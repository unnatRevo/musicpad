using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class EventDateRequest : BaseRequest
    {
        public int userId { get; set; }
        public string date { get; set; }
        public string accessToken { get; set; }

        public EventDateRequest() : base()
        {

        }
    }
}