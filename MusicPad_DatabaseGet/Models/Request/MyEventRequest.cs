using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class MyEventRequest : BaseRequest
    {
        public int userId { get; set; }
        public string accessToken { get; set; }

        public MyEventRequest() : base()
        {

        }
    }
}