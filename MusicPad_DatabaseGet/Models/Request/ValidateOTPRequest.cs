using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class ValidateOTPRequest : BaseRequest
    {
        public string code { get; set; }
        public ValidateOTPRequest() : base()
        {

        }
    }
}