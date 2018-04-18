using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class ReGenrateOTPRequest : BaseRequest
    {
        public string countryCode { get; set; }
        public string phoneNumber { get; set; }
        public ReGenrateOTPRequest() : base()
        {

        }
    }
}