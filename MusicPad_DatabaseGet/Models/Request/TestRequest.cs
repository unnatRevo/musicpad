using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class TestRequest : BaseRequest
    {
        public int intUserID { get; set; }
        public string Caller { get; set; }
        public TestRequest() : base()
        {

        }

    }
}