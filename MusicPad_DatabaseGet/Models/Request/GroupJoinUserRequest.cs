using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class GroupJoinUserRequest : BaseRequest
    {
        public string groupId { get; set; }


        public GroupJoinUserRequest() : base()
        {

        }
    }
}