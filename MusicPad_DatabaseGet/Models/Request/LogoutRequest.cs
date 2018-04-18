using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class LogoutRequest : BaseRequest
    {
        public string userId { get; set; }
        public string accessToken { get; set; }

        public LogoutRequest() : base()
        {

        }
    }
}