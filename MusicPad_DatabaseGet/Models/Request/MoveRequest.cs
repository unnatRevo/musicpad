using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class MoveRequest : BaseRequest
    {
        public int maxid { get; set; }
       
        public MoveRequest() : base()
        {

        }
    }
}