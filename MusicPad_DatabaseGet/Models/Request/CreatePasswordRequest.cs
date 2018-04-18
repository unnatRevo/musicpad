using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class CreatePasswordRequest: BaseRequest
    {
        public string password { get; set; }
        public int userId { get; set; }
        public CreatePasswordRequest() : base()
        {

        }
    }
}