using MusicEventAPI.Models.Request;
using MusicEventAPI.Utility;
using System;

namespace MusicEventAPI.Models.Response
{
    public class TestResponce : BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            var list = "this is result ";
            try
            {
                Helper.FillResult(Result, ErrorCode.Success, list);
            }
            catch (Exception ex)
            {
                Helper.FillResult(Result, ErrorCode.FillResponseFailed, null);
                throw;
            }
        }

    }
}