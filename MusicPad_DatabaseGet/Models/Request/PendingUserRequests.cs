using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class PendingUserRequests : BaseRequest
    {
        public int userId { get; set; }
        public string groupId { get; set; }
        public string accessToken { get; set; }

        public PendingUserRequests() : base()
        {

        }
    }
}