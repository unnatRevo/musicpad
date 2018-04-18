﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class UpdateProfileRequest : BaseRequest
    {
        public string name { get; set; }
        public string userName { get; set; }
        public string emailId { get; set; }
        public int userID { get; set; }
        public string filepath { get; set; }
        public string FileName { get; set; }
        public string groupType { get; set; }
        public string groupPeople { get; set; }
        public string password { get; set; }
        public string accessToken { get; set; }
        public UpdateProfileRequest() : base()
        {

        }
    }
}