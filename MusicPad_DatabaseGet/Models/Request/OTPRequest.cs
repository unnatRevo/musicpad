using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class OTPRequest : BaseRequest
    {
        public string countryCode { get; set; }
        public string phoneNumber { get; set; }
        public OTPRequest() : base()
        {

        }
    }
}