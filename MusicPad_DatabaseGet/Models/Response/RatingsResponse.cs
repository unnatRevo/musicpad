using MusicEventAPI.Manage;
using MusicEventAPI.Models.Request;
using MusicEventAPI.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Response
{
    public class RatingsResponse : BaseResponse
    {

        public override void Fill(BaseRequest request)
        {
            DataTable dt = new DataTable();
            try
            {
                var req = (request as MusicEventAPI.Models.Request.RatingsRequest);


                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("ManageRatings", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@userId", req.userId);
                    com.Parameters.AddWithValue("@eventID", req.eventID);
                    com.Parameters.AddWithValue("@ratings", req.ratings);
                    if (!string.IsNullOrEmpty(req.reviewText))
                        com.Parameters.AddWithValue("@reviewText", req.reviewText);


                    SqlDataAdapter da = new SqlDataAdapter(com);
                    con.Open();
                    da.Fill(dt);
                    con.Close();
                   Helper.FillResult(Result, ErrorCode.Success, "");
                }


            }
            catch (Exception ex)
            {
                Helper.FillResult(Result, ErrorCode.NoAuthentication, "");
                throw;
            }
        }


    }
}