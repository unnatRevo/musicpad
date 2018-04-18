using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class RegisterWithPhoneRequest :BaseRequest
    {
        public string countryCode { get; set; }
        public string phoneNumber { get; set; }
        public string password { get; set; }
        public string deviceToken { get; set; }
        public string deviceType { get; set; }
        public string mobileContact { get; set; }
        public RegisterWithPhoneRequest() : base()
        {

        }
    }
}