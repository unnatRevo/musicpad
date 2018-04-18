using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class LoginRequest : BaseRequest
    {
        public string countryCode { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string deviceToken { get; set; }
        public string deviceType { get; set; }
        public string mobileContact { get; set; }
        public string appname { get; set; }
        public LoginRequest() : base()
        {

        }
    }
}