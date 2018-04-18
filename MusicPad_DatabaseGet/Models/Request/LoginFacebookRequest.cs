using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class LoginFacebookRequest : BaseRequest
    {
        public string fbid { get; set; }
        public string emailId { get; set; }
        public string userName { get; set; }
        public string name { get; set; }
        public string deviceToken { get; set; }
        public string deviceType { get; set; }
        public string filepath { get; set; }
        public string FileName { get; set; }
        public string faceBookFriends { get; set; }
        public string isLogin { get; set; }
        public string appname { get; set; }
        //public HttpPostedFileBase Files { get; set; }
        // public MultipartMemoryStreamProvider ImageCode { get; set; }
        //public HttpPostedFileBase Image { get; set; }
        public LoginFacebookRequest() : base()
        {

        }
    }
}