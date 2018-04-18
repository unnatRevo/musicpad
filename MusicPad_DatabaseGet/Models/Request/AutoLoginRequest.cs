using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class AutoLoginRequest : BaseRequest
    {
        public int userId { get; set; }
        public string mobileContact { get; set; }
        public string faceBookFriends { get; set; }
        public string deviceToken { get; set; }
        public string deviceType { get; set; }
        public string accessToken { get; set; }
        public string appname { get; set; }
        public AutoLoginRequest() : base()
        {

        }
    }
}