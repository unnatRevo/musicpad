using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class CreateGroupRequest : BaseRequest
    {
        public int userId { get; set; }
        public int groupId { get; set; }
        public string groupName { get; set; }
        public string image { get; set; }
        public string tagName  { get; set; }
        public string groupType { get; set; }
        public string accessToken { get; set; }
        public CreateGroupRequest() : base()
        {

        }
    }
}