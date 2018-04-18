using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class ProfileMissingDataRequest : BaseRequest
    {
        public string name { get; set; }
        public string userName { get; set; }
        public string emailId { get; set; }
        public string password { get; set; }
        public int userID { get; set; }
        public string filepath { get; set; }
        public string FileName { get; set; }
        public bool isBusinessUser { get; set; }
        public string businessLocation { get; set; }
        public string bussinesslatitude { get; set; }
        public string businesslongitude { get; set; }
        public string accessToken { get; set; }
        public string appname { get; set; }
        public ProfileMissingDataRequest() : base()
        {

        }
    }
}