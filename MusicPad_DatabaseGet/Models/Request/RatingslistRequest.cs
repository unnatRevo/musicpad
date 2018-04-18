using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class RatingslistRequest : BaseRequest
    {
        public int eventId { get; set; }
        public RatingslistRequest() : base()
        {

        }
    }
}