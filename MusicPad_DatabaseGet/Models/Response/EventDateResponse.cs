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
    public class EventDateResponse : BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            List<Event> _event = new List<Event>();
            List<EventUser> eventUserss = new List<EventUser>();
            try
            {
                var req = (request as MusicEventAPI.Models.Request.EventDateRequest);
                DateTime? fromdates = null;
                if (!string.IsNullOrEmpty(req.date))
                {
                    fromdates = DateTime.ParseExact(req.date, "MM-dd-yyyy", CultureInfo.InvariantCulture);
                }
                DateTime startDate = new DateTime(fromdates.Value.Year, fromdates.Value.Month, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);


                using (SqlConnection con = new SqlConnection(Manage.DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("EventUserList", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@userid", req.userId);
                    com.Parameters.AddWithValue("@startdate", startDate);
                    com.Parameters.AddWithValue("@enddate", endDate);
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
                        eventUserss.Add(
                            new EventUser
                            {
                                eventID = Convert.ToInt32(dr["EventID"]),
                                userId = Convert.ToString(dr["UserID"]),
                                //imagePath = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/") + ( string.IsNullOrEmpty(Convert.ToString(dr["UserImagePath"])) ? "/content/Image/NoImage.png" : Convert.ToString(dr["UserImagePath"])),
                                imagePath = ImagePath.GetimagePath(Convert.ToString(dr["UserImagePath"]), HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/")),
                                userName = Convert.ToString(dr["UserName"])
                            }
                            );
                    }
                }

                using (SqlConnection con = new SqlConnection(Manage.DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("GetEventDatewise", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@userid", req.userId);
                    com.Parameters.AddWithValue("@startdate", startDate);
                    com.Parameters.AddWithValue("@enddate", endDate);

                    SqlDataAdapter da = new SqlDataAdapter(com);

                    con.Open();
                    da.Fill(dt1);
                    con.Close();



                    foreach (DataRow dr in dt1.Rows)
                    {
                        Event E = null;
                        E = new Event
                        {
                            eventId = Convert.ToInt32(dr["EventID"]),
                            isOwner = string.IsNullOrEmpty(Convert.ToString(dr["UserID"])) ?false :Convert.ToInt32(dr["UserID"]) == req.userId && req.userId != 0 ? true : false,
                            categoryId = Convert.ToInt32(dr["CategoryID"]),
                            eventName = Convert.ToString(dr["EventName"]),
                            location = Convert.ToString(dr["location"]),
                            startDate = Convert.ToDateTime(dr["StartDate"]).ToString("MM-dd-yyyy"),
                            endDate = Convert.ToDateTime(dr["EndDate"]).ToString("MM-dd-yyyy"),
                            eventDescription = "",//Convert.ToString(dr["EventDiscription"]),
                            //imagepath = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/") + (string.IsNullOrEmpty(Convert.ToString(dr["Image"])) ? "/content/Image/NoImage.png" :  Convert.ToString(dr["Image"])),
                            imagepath = ImagePath.GetimagePath(Convert.ToString(dr["Image"]), HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/")),
                            latitude = Convert.ToString(dr["latitude"]),
                            longitude = Convert.ToString(dr["longitude"]),
                            activityCount = Convert.ToInt32(dr["SubEventCount"]),
                            startTime = Convert.ToString(dr["EventStartTime"]),
                            endTime = Convert.ToString(dr["EventEndTime"]),
                            dealtext = Convert.ToString(dr["Dealtext"]),
                            percentage = Convert.ToString(dr["Percentage"]),
                            categoryName = Convert.ToString(dr["CategoryName"]),
                            ratings = Convert.ToString(dr["Ratings"])

                        };
                         E.eventUser = eventUserss.Where(x => x.eventID == E.eventId).ToList();
                        _event.Add(E);
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