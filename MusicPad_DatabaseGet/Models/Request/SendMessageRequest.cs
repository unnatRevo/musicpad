using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class SendMessageRequest : BaseRequest
    {
        public string groupId { get; set; }
        public string userId { get; set; }
        public string chattext { get; set; }
        public string accessToken { get; set; }
        public SendMessageRequest() : base()
        {

        }
    }
}