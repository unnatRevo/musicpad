using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class RemoveUserFromGroupRequest : BaseRequest
    {
        public int userId { get; set; }
        public int groupId { get; set; }
        public int removeUserId { get; set; }
        public string accessToken { get; set; }

        public RemoveUserFromGroupRequest() : base()
        {

        }
    }
}