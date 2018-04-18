using MusicEventAPI.Models.Request;
using MusicEventAPI.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Response
{
    public class MyEvenResponse : BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            DataTable dt = new DataTable();
            List<Event> _event = new List<Event>();
            try
            {
                var req = (request as MusicEventAPI.Models.Request.MyEventRequest);
               
                using (SqlConnection con = new SqlConnection(Manage.DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("MyEvent", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@userid", req.userId);
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

                    foreach (DataRow dr in dt.Rows)
                    {
                        _event.Add(
                            new Event
                            {
                                eventId = Convert.ToInt32(dr["EventID"]),
                                // userId = Convert.ToInt32(dr["UserID"]),
                                categoryId = Convert.ToInt32(dr["CategoryID"]),
                                eventName = Convert.ToString(dr["EventName"]),
                                location = Convert.ToString(dr["location"]),
                                startDate = Convert.ToDateTime(dr["StartDate"]).ToString("MM-dd-yyyy"),
                                endDate = Convert.ToDateTime(dr["EndDate"]).ToString("MM-dd-yyyy"),
                                eventDescription = "",//Convert.ToString(dr["EventDiscription"]),
                                //imagepath = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/") + (string.IsNullOrEmpty(Convert.ToString(dr["Image"])) ? "/content/Image/NoImage.png" : Convert.ToString(dr["Image"])),
                                imagepath = ImagePath.GetimagePath(Convert.ToString(dr["Image"]), HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/")),
                                latitude = Convert.ToString(dr["latitude"]),
                                longitude = Convert.ToString(dr["longitude"]),
                                activityCount = Convert.ToInt32(dr["SubEventCount"]),
                                startTime = Convert.ToString(dr["EventStartTime"]),
                                endTime = Convert.ToString(dr["EventEndTime"]),
                                dealtext = Convert.ToString(dr["Dealtext"]),
                                percentage = Convert.ToString(dr["Percentage"]),
                                categoryName = Convert.ToString(dr["CategoryName"])
                            }
                            );
                    }

                    Helper.FillResult(Result, ErrorCode.Success, _event);
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