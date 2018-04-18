using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class RemoveFromGroupRequest : BaseRequest
    {
        public int userId { get; set; }
        public int groupId { get; set; }
        public string isadd { get; set; }
        public string accessToken { get; set; }

        public RemoveFromGroupRequest() : base()
        {

        }
    }
}