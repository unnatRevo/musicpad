using MusicEventAPI.Manage;
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
    public class CreateEventPythonResponse: BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            DataTable dt = new DataTable();
            try
            {
                DateTime? fromdates = null;

                var req = (request as MusicEventAPI.Models.Request.CreateEventPython);

                if (!string.IsNullOrEmpty(req.startDate))
                {
                    fromdates = DateTime.ParseExact(req.startDate, "MM-dd-yyyy", CultureInfo.InvariantCulture);
                }

                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    if (!string.IsNullOrEmpty(req.startDate))
                    {
                        //TimeSpan starttime = DateTime.Parse("00:00 AM").TimeOfDay; 
                        if (string.IsNullOrEmpty(req.startTime))
                        {
                            req.startTime = "00:00 AM";
                        }

                            if ((string.IsNullOrEmpty(req.latitude) || string.IsNullOrEmpty(req.longitude)) && (!string.IsNullOrEmpty(req.location)))
                        {
                            Adress adrs = new Adress();
                            adrs.Key = "";
                            adrs.Address = req.location;
                            adrs.GeoCode();
                            req.latitude = adrs.Latitude;
                            req.longitude = adrs.Longitude;
                        }

                        if ((!string.IsNullOrEmpty(req.latitude) && !string.IsNullOrEmpty(req.longitude)) && (string.IsNullOrEmpty(req.location)))
                        {
                            GetAddress address = new GetAddress();
                            req.location = address.RetrieveFormatedAddress(req.latitude, req.longitude,"");
                        }

                        SqlCommand com = new SqlCommand("AddEventsFromMySql", con);
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@EventName", req.eventName);
                        com.Parameters.AddWithValue("@Latitude", req.latitude);
                        com.Parameters.AddWithValue("@Longitude", req.longitude);
                        com.Parameters.AddWithValue("@Location", req.location);
                        if (!string.IsNullOrEmpty(req.categoryName))
                            com.Parameters.AddWithValue("@Category", req.categoryName);
                        com.Parameters.AddWithValue("@StartDate", fromdates);
                        com.Parameters.AddWithValue("@StartTime", req.startTime);
                        com.Parameters.AddWithValue("@isTmaster", req.isTmaster);
                        if (!string.IsNullOrEmpty(req.eventDescription))
                            com.Parameters.AddWithValue("@description", req.eventDescription);
                        if (!string.IsNullOrEmpty(req.Url))
                            com.Parameters.AddWithValue("@ticketUrl", req.Url);
                        //com.Parameters.AddWithValue("@EndDate", starttime);
                        if (!string.IsNullOrEmpty(req.filepath))
                        {
                            com.Parameters.AddWithValue("@Image", req.filepath);
                        }
                        con.Open();

                        SqlDataAdapter da = new SqlDataAdapter(com);
                        //com.ExecuteNonQuery();
                        da.Fill(dt);
                        con.Close();

                        //if (Convert.ToString(dt.Rows[0]["status"]) == "1" && !string.IsNullOrEmpty(eventlst[i].image))
                        //{
                        //    AddImage(eventlst[i].image, path, filename);
                        //}
                    }

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