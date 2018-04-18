using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class AcceptDeclineRequest : BaseRequest
    {
        public int userId { get; set; }
        public int groupId { get; set; }
        public int otherUserID { get; set; }
        public string isapprove { get; set; }
        public string accessToken { get; set; }

        public AcceptDeclineRequest() : base()
        {

        }
    }
}