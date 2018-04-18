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
    public class ActivityDetailResponse : BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            DataTable dt = new DataTable();
            List<Category> _catlist = new List<Category>();
            try
            {
                var req = (request as MusicEventAPI.Models.Request.ActivityDetailRequest);


                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("GetActivityDetail", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@eventid", req.eventId);
                    com.Parameters.AddWithValue("@userid", req.userId);
                    com.Parameters.AddWithValue("@activityId", req.activityId);
                    if (!string.IsNullOrEmpty(req.accessToken))
                        com.Parameters.AddWithValue("@accesstoken", req.accessToken);

                    SqlDataAdapter da = new SqlDataAdapter(com);

                    con.Open();
                    da.Fill(dt);
                    con.Close();

                }

                if (dt.Rows.Count > 0)
                {
                    DataColumnCollection columns = dt.Columns;
                    if (columns.Contains("ErrorMessage"))
                    {
                        if (Convert.ToString(dt.Rows[0]["ErrorMessage"]) == "Logout")
                            Helper.FillResult(Result, ErrorCode.Logout, "");
                        return;
                    }
                }

                if (dt.Rows.Count == 0)
                {
                Helper.FillResult(Result, ErrorCode.FillResponseFailed, "");
                }
                else
                {

                    Activity _activity = convertdatatableToObject.DatatabletoActivity(dt, req.userId);
                    Helper.FillResult(Result, ErrorCode.Success, _activity);
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