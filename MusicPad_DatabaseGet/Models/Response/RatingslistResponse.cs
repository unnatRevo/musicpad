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
    public class RatingslistResponse : BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            DataTable dt = new DataTable();
            List<RatingsModel> _catlist = new List<RatingsModel>();
            try
            {
                var req = (request as MusicEventAPI.Models.Request.RatingslistRequest);


                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("GetRatingsList", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@eventId", req.eventId);

                    SqlDataAdapter da = new SqlDataAdapter(com);

                    con.Open();
                    da.Fill(dt);
                    con.Close();

                    foreach (DataRow dr in dt.Rows)
                    {
                        _catlist.Add(
                            new RatingsModel
                            {
                                userId = Convert.ToInt32(dr["UserID"]),
                                eventID = Convert.ToInt32(dr["EventID"]),
                                reviewText = Convert.ToString(dr["ReviewText"]),
                                userName = Convert.ToString(dr["UserName"]),
                                userImagePath = ImagePath.GetimagePath(Convert.ToString(dt.Rows[0]["UserImagePath"]), HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/")),
                                ratings = Convert.ToString(dr["Ratings"])

                            }
                            );
                    }
                    Helper.FillResult(Result, ErrorCode.Success, _catlist);
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