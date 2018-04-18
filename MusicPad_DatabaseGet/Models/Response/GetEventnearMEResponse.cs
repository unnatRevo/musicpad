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
    public class GetEventnearMEResponse : BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            DataTable dt = new DataTable();
            List<Event> _event = new List<Event>();
            try
            {
                var req = (request as MusicEventAPI.Models.Request.GetEventnearMERequest);
              
                using (SqlConnection con = new SqlConnection(Manage.DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("GetEventnearME", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@eventid", req.eventId);
                    com.Parameters.AddWithValue("@userid", req.userID);
                    com.Parameters.AddWithValue("@latitude", req.latitude);
                    com.Parameters.AddWithValue("@longitude", req.longitude);
                    com.Parameters.AddWithValue("@PageNumber", req.pageNumber == 0 ? 1 : req.pageNumber);

                    SqlDataAdapter da = new SqlDataAdapter(com);

                    con.Open();
                    da.Fill(dt);
                    con.Close();

                    int totalcount = 0;
                    if (dt.Rows.Count > 0)
                        totalcount = Convert.ToInt32(dt.Rows[0]["TotalRecords"]);

                    dynamic obj = new { };
                    if (dt.Rows.Count > 0)
                    {
                        obj = new
                        {
                            //isCouple = string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["IsCouple"]))?"": ImagePath.GetimagePath(Convert.ToString(dt.Rows[0]["IsCouple"]), HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/")),
                            //isFlySolo =string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["IsFlySolo"])) ? "" : ImagePath.GetimagePath(Convert.ToString(dt.Rows[0]["IsFlySolo"]), HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/")),
                            //isGroup = string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["ISGroup"])) ? "" : ImagePath.GetimagePath(Convert.ToString(dt.Rows[0]["ISGroup"]), HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/")),
                            isCouple = Convert.ToString(dt.Rows[0]["IsCouple"]),
                            isFlySolo = Convert.ToString(dt.Rows[0]["IsFlySolo"]),
                            isGroup = Convert.ToString(dt.Rows[0]["ISGroup"])
                        };
                    }

                    totalcount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalcount) / 25));
                    foreach (DataRow dr in dt.Rows)
                    {

                        _event.Add(
                            new Event
                            {
                                eventId = Convert.ToInt32(dr["EventID"]),
                                isOwner = Convert.ToInt32(dr["UserID"]) == req.userID && req.userID != 0 ? true : false,
                                eventName = Convert.ToString(dr["EventName"]),
                                location = Convert.ToString(dr["Location"]),
                                startDate = Convert.ToString(dr["StartDate"]),
                                endDate = Convert.ToString(dr["EndDate"]),
                                eventDescription = "",
                                imagepath = ImagePath.GetimagePath(Convert.ToString(dr["Image"]), HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/")),
                                startTime = Convert.ToString(dr["StartTime"]),
                                endTime = Convert.ToString(dr["EndTime"]),
                                categoryName = Convert.ToString(dr["CategoryName"]),
                                latitude = Convert.ToString(dr["latitude"]),
                                longitude = Convert.ToString(dr["longitude"]),
                                addToCalender = Convert.ToInt32(dr["addtoCalander"]) == 0 ? false : true,
                                nearMe = Convert.ToString(dr["distancemile"]),
                                ratings = Convert.ToString(dr["Ratings"])
                            }
                            );
                    }

                    Helper.FillResult(Result, ErrorCode.Success, _event, totalcount,obj);
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