using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Request
{
    public class BaseRequest
    {



        public List<KeyValuePair<string, string>> Options { get; set; }

        internal bool IsValid { get; set; }
        //internal string URI { get; set; }
        internal string UrlReferrer { get; set; }


        //public BaseRequest(string URI)
        //{
        //    //this.URI = URI;
        //}

        private string _cacheKey = string.Empty;

        public virtual string CacheKey
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_cacheKey))
                {
                }
                return _cacheKey;
            }
        }


        //protected void ValidateClientRequest(Result result)
        //{
        //    IsValid = true;
        //}


    }
}