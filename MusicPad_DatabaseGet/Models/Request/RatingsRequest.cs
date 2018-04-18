using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class RatingsRequest : BaseRequest
    {
        public int userId { get; set; }
        public int eventID { get; set; }
        public decimal ratings { get; set; }
        public string reviewText { get; set; }
        public string userName { get; set; }
        public string userImagePath { get; set; }
        public RatingsRequest() : base()
        {

        }
    }

    public class RatingsModel 
    {
        public int userId { get; set; }
        public int eventID { get; set; }
        public string reviewText { get; set; }
        public string userName { get; set; }
        public string userImagePath { get; set; }
        public string ratings { get; set; }
       
    }
}