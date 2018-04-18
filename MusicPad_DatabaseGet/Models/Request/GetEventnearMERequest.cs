using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class GetEventnearMERequest : BaseRequest
    {
        public int userID { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public int pageNumber { get; set; }
        public int eventId { get; set; }

        public GetEventnearMERequest() : base()
        {

        }
    }
}