using MusicEventAPI.Models.Request;
using MusicEventAPI.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Response
{

    public abstract class BaseResponse
    {
        public Result Result { get; set; }
        public abstract void Fill(BaseRequest request);

    }

}