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
    public class EventDetailResponse : BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            DataTable dt = new DataTable();
            try
            {
                var req = (request as MusicEventAPI.Models.Request.EventDetailRequest);


                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("GetEventDetail", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@eventid", req.eventId);
                    com.Parameters.AddWithValue("@userid", req.userId);
                    com.Parameters.AddWithValue("@latitude", req.latitude);
                    com.Parameters.AddWithValue("@longitude", req.longitude);
                    if(!string.IsNullOrEmpty(req.accessToken))
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


                }

                if (dt.Rows.Count == 0)
                {
                    Helper.FillResult(Result, ErrorCode.FillResponseFailed, "");
                }
                else
                {

                    Event _event = convertdatatableToObject.DatatabletoEvent(dt, req.userId);
                    _event.addToCalender = Convert.ToInt32(dt.Rows[0]["addtoCalander"]) == 0 ? false : true;
                    _event.isCouple = Convert.ToString(dt.Rows[0]["IsCouple"]) ;
                    _event.isFlySolo = Convert.ToString(dt.Rows[0]["IsFlySolo"]);
                    _event.isGroup = Convert.ToString(dt.Rows[0]["ISGroup"]);
                    _event.nearMe = Convert.ToString(dt.Rows[0]["distancemile"]);
                    _event.ratings = Convert.ToString(dt.Rows[0]["Ratings"]);
                    _event.ticketUrl = Convert.ToString(dt.Rows[0]["TicketUrl"]);
                    _event.isTmaster = Convert.ToBoolean(dt.Rows[0]["IsTmaster"]);

                    _event.price = Convert.ToString(dt.Rows[0]["ticketPrice"]);
                    _event.ticketCount = Convert.ToString(dt.Rows[0]["ticketCount"]);
                    _event.ratingCount   = Convert.ToString(dt.Rows[0]["ratingCount"]);
                    _event.nearMeEvents =  EventList(_event.eventId, _event.latitude, _event.longitude);
                    _event.alredyRating = Convert.ToInt32(dt.Rows[0]["alredyRating"]) > 0 ? true : false;
                    Helper.FillResult(Result, ErrorCode.Success, _event);
                }

            }

            catch (Exception ex)
            {
                Helper.FillResult(Result, ErrorCode.NoAuthentication, "");
                throw;
            }
        }


        public List<NearMe> EventList(int eventid, string latitude, string longitude)
        {

            List<NearMe> _event = new List<NearMe>();
            using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
            {
                SqlCommand com = new SqlCommand("EventDetailNearMe", con);
                DataTable DTS = new DataTable();
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@eventId", eventid);
                com.Parameters.AddWithValue("@latitude", latitude);
                com.Parameters.AddWithValue("@longitude", longitude);

                SqlDataAdapter da = new SqlDataAdapter(com);

                con.Open();
                da.Fill(DTS);
                con.Close();


                foreach (DataRow dr in DTS.Rows)
                {

                    _event.Add(
                        new NearMe
                        {
                            eventId = Convert.ToInt32(dr["EventID"]),
                            eventName = Convert.ToString(dr["EventName"]),
                            imagepath = ImagePath.GetimagePath(Convert.ToString(dr["Image"]), HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/")),
                            categoryName = Convert.ToString(dr["CategoryName"]),
                            location = Convert.ToString(dr["location"]),
                            latitude = Convert.ToString(dr["latitude"]),
                            longitude = Convert.ToString(dr["longitude"]
                            )
                        }
                        );
                }

                return _event;
            }
        }
    }
}