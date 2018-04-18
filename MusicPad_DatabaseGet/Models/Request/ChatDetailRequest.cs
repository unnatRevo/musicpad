using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class ChatDetailRequest : BaseRequest
    {
        public int userId { get; set; }
        public int groupId { get; set; }
        public int pageNo { get; set; }
        public string accessToken { get; set; }

        public ChatDetailRequest() : base()
        {

        }
    }
}