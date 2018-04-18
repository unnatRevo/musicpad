using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class JoinEventRequest : BaseRequest
    {
        public int userId { get; set; }
        public int eventId { get; set; }
        public string  shareGroupType { get; set; }

        public string accessToken { get; set; }

        public JoinEventRequest() : base()
        {

        }
    }
}