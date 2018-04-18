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
    public class ActivityListResponse : BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            DataTable dt = new DataTable();
            List<Activity> _activity = new List<Activity>();
            try
            {
                var req = (request as MusicEventAPI.Models.Request.ActivityListRequest);


                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("ActivityList", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@userid", req.userId);
                    com.Parameters.AddWithValue("@eventid", req.eventId);
                    com.Parameters.AddWithValue("@PageNumber", req.pageNumber);
                    if (!string.IsNullOrEmpty(req.accessToken))
                        com.Parameters.AddWithValue("@accesstoken", req.accessToken);

                    SqlDataAdapter da = new SqlDataAdapter(com);

                    con.Open();
                    da.Fill(dt);
                    con.Close();


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

                    int totalcount = 0;
                        if (dt.Rows.Count > 0)
                        totalcount = Convert.ToInt32(dt.Rows[0]["TotalRecords"]);

                    totalcount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalcount) / 15));
                    foreach (DataRow dr in dt.Rows)
                    {
                        _activity.Add(
                            new Activity
                            {
                                eventId = Convert.ToInt32(dr["EventID"]),
                                isOwner = Convert.ToInt32(dr["UserID"]) == req.userId ?true :false,
                                activityId = Convert.ToInt32(dr["SubEventID"]),
                                activityName = Convert.ToString(dr["SubEventName"]),
                                location = Convert.ToString(dr["Location"]),
                                startDate = Convert.ToString(dr["StartDate"]),
                                endDate = Convert.ToString(dr["EndDate"]),
                                activityDescription = Convert.ToString(dr["SubEventDiscription"]),
                                //imagepath = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/") + (string.IsNullOrEmpty(Convert.ToString(dr["Image"])) ? "/content/Image/NoImage.png" :  Convert.ToString(dr["Image"])),
                                imagepath= ImagePath.GetimagePath(Convert.ToString(dr["Image"]), HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/")),
                                startTime = Convert.ToString(dr["StartTime"]),
                                endTime = Convert.ToString(dr["EndTime"]),
                                latitude = Convert.ToString(dr["latitude"]),
                                longitude = Convert.ToString(dr["longitude"])
                            }
                            );
                    }
                
                Helper.FillResult(Result, ErrorCode.Success, _activity ,totalcount);
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