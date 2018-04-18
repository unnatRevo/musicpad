using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class ForgetPasswordRequest : BaseRequest
    {
        public string countryCode { get; set; }
        public string phoneNo { get; set; }
        public ForgetPasswordRequest() : base()
        {

        }
    }
}