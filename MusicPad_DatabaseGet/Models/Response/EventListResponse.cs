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
    public class EventListResponse : BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            DataTable dt = new DataTable();
            List<Event> _event = new List<Event>();

            try
            {
                var req = (request as MusicEventAPI.Models.Request.EventListRequest);

                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("EventList", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@userid", req.userId);
                    com.Parameters.AddWithValue("@latitude", req.latitude);
                    com.Parameters.AddWithValue("@longitude", req.longitude);
                    com.Parameters.AddWithValue("@PageNumber", req.pageNumber == 0 ? 1 : req.pageNumber);
                    com.Parameters.AddWithValue("@categoryId", req.categoryId);
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
                    dynamic obj = new { };
                    if (dt.Rows.Count > 0)
                    {
                        obj = new
                        {
                            //isCouple = string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["IsCouple"]))?"": ImagePath.GetimagePath(Convert.ToString(dt.Rows[0]["IsCouple"]), HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/")),
                            //isFlySolo =string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["IsFlySolo"])) ? "" : ImagePath.GetimagePath(Convert.ToString(dt.Rows[0]["IsFlySolo"]), HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/")),
                            //isGroup = string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["ISGroup"])) ? "" : ImagePath.GetimagePath(Convert.ToString(dt.Rows[0]["ISGroup"]), HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/")),
                            isCouple =Convert.ToString(dt.Rows[0]["IsCouple"]),
                            isFlySolo = Convert.ToString(dt.Rows[0]["IsFlySolo"]),
                            isGroup = Convert.ToString(dt.Rows[0]["ISGroup"]) 
                        };
                    }


                    if (dt.Rows.Count > 0 && req.pageNumber == 1)
                        totalcount = Convert.ToInt32(dt.Rows[0]["TotalRecords"]);
                    else
                        totalcount = req.totalPageCount;
                    foreach (DataRow dr in dt.Rows)
                    {

                        _event.Add(
                            new Event
                            {
                                eventId = Convert.ToInt32(dr["EventID"]),
                                isOwner = Convert.ToInt32(dr["UserID"]) == req.userId && req.userId != 0 ? true : false,
                                eventName = Convert.ToString(dr["EventName"]),
                                location = Convert.ToString(dr["Location"]),
                                startDate = Convert.ToString(dr["StartDate"]),
                                endDate = Convert.ToString(dr["EndDate"]),
                                eventDescription = "",//Convert.ToString(dr["EventDiscription"]),
                                //imagepath = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/") + (string.IsNullOrEmpty(Convert.ToString(dr["Image"])) ? "/content/Image/NoImage.png" : Convert.ToString(dr["Image"])),
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
                    AddAdvertisement((req.pageNumber == 0 ? 1 : req.pageNumber), _event, totalcount, obj, req.latitude, req.longitude);

                }
            }

            catch (Exception ex)
            {
                Helper.FillResult(Result, ErrorCode.NoAuthentication, "");
                throw;
            }
        }


        public void AddAdvertisement(int pageno, List<Event> model, int totalpagecount, dynamic obj,string latitude,string longitude)
        {
            int[] arr = new int[] { 3,6,9,12,15 };
            // int[] arr = new int[] { 1, 2, 3, 4, 5, 6, 7 };
            int addcount = 0;
            try
            {
                DataTable dt = new DataTable();
                List<Advertisement> _advertisement = new List<Advertisement>();
                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("AdvertiseList", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@PageNumber", pageno);
                    com.Parameters.AddWithValue("@latitude", latitude);
                    com.Parameters.AddWithValue("@longitude",longitude);

                    SqlDataAdapter da = new SqlDataAdapter(com);
                    con.Open();
                    da.Fill(dt);
                    con.Close();

                    int totalcount = 0;
                    if (dt.Rows.Count > 0)
                        totalcount = Convert.ToInt32(dt.Rows[0]["TotalRecords"]);
                    foreach (DataRow dr in dt.Rows)
                    {
                        _advertisement.Add(
                            new Advertisement
                            {
                                advertisementId = Convert.ToInt32(dr["Id"]),
                                Name = Convert.ToString(dr["Name"]),
                                Address = Convert.ToString(dr["Address"]),
                                PhoneNo = Convert.ToString(dr["PhoneNo"]),
                                Discription = Convert.ToString(dr["Discription"]),
                                Discount = Convert.ToString(dr["Discount"]),
                                //ImagePath = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/") + (string.IsNullOrEmpty(Convert.ToString(dr["ImagePath"])) ? "/content/Image/NoImage.png" : Convert.ToString(dr["ImagePath"]))
                                ImagePath = ImagePath.GetimagePath(Convert.ToString(dr["ImagePath"]), HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/")),
                            }
                            );
                    }
                    int eventcount = model.Count;
                    int advertisementcount = _advertisement.Count;
                    List<EventAdvertisement> _listmodel = new List<EventAdvertisement>();
                    if (pageno == 1)
                        totalpagecount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalpagecount + totalcount) / 15));
                    if (_advertisement.Count == 0)
                    {
                        for (int i = 1; i <= eventcount; i++)
                        {
                            _listmodel.Add(
                           new EventAdvertisement
                           {
                               eventId = model[i - 1].eventId,
                               eventName = model[i - 1].eventName,
                               location = model[i - 1].location,
                               startDate = model[i - 1].startDate,
                               endDate = model[i - 1].endDate,
                               eventDescription = model[i - 1].eventDescription,
                               imagepath = model[i - 1].imagepath,
                               startTime = model[i - 1].startTime,
                               endTime = model[i - 1].endTime,
                               isadvertisement = false,
                               dealtext = "",
                               percentage = "",
                               PhoneNo = "",
                               categoryName = model[i - 1].categoryName,
                               isOwner = model[i - 1].isOwner,
                               ratings = model[i - 1].ratings,
                               nearMe = model[i - 1].nearMe,
                               latitude = model[i - 1].latitude,
                               longitude = model[i - 1].longitude,
                               addToCalender = model[i - 1].addToCalender
                               
                           }
                           );
                        }
                    }
                    else
                    {

                        for (int i = 1; i <= eventcount; i++)
                        {
                            _listmodel.Add(
                           new EventAdvertisement
                           {
                               eventId = model[i - 1].eventId,
                               eventName = model[i - 1].eventName,
                               location = model[i - 1].location,
                               startDate = model[i - 1].startDate,
                               endDate = model[i - 1].endDate,
                               eventDescription = model[i - 1].eventDescription,
                               imagepath = model[i - 1].imagepath,
                               startTime = model[i - 1].startTime,
                               endTime = model[i - 1].endTime,
                               isadvertisement = false,
                               dealtext = "",
                               percentage = "",
                               PhoneNo = "",
                               categoryName = model[i - 1].categoryName,
                               isOwner = model[i - 1].isOwner,
                               ratings = model[i - 1].ratings,
                               nearMe = model[i - 1].nearMe,
                               latitude = model[i - 1].latitude,
                               longitude = model[i - 1].longitude,
                               addToCalender = model[i - 1].addToCalender
                           }
                           );

                            if (arr.Contains(i + 1) && advertisementcount != addcount)
                            {
                                _listmodel.Add(
                                                   new EventAdvertisement
                                                   {
                                                       eventId = _advertisement[addcount].advertisementId,
                                                       eventName = _advertisement[addcount].Name,
                                                       location = _advertisement[addcount].Address,
                                                       PhoneNo = _advertisement[addcount].PhoneNo,
                                                       eventDescription = _advertisement[addcount].Discription,
                                                       imagepath = _advertisement[addcount].ImagePath,
                                                       percentage = _advertisement[addcount].Discount,
                                                       isadvertisement = true,
                                                       startDate = "",
                                                       endDate = "",
                                                       startTime = "",
                                                       endTime = "",
                                                       dealtext = "",
                                                       categoryName = "",
                                                       isOwner = false,
                                                       ratings = model[i - 1].ratings,
                                                       nearMe = "",
                                                       latitude = "",
                                                       longitude = ""
                                                   }
                                              );
                                addcount++;
                            }

                        }
                        _listmodel = _listmodel.Take(_listmodel.Count > 15 ? 15 : _listmodel.Count).ToList();
                    }

                    Helper.FillResult(Result, ErrorCode.Success, _listmodel, totalpagecount, obj);
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