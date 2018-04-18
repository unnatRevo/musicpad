using MusicEventAPI.Manage;
using MusicEventAPI.Models.Request;
using MusicEventAPI.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace MusicEventAPI.Models.Response
{
    public class CreateEventResponse : BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            DataTable dt = new DataTable();
            try
            {
                DateTime? fromdates = null, todates = null;
                TimeSpan? starttime = null, endtime = null;

                var req = (request as MusicEventAPI.Models.Request.CreateEventRequest);

                if (!string.IsNullOrEmpty(req.startDate))
                {
                    fromdates = DateTime.ParseExact(req.startDate, "MM-dd-yyyy", CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(req.endDate))
                {
                    todates = DateTime.ParseExact(req.endDate, "MM-dd-yyyy", CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(req.startTime))
                {
                    starttime = TimeSpan.Parse(req.startTime);
                    fromdates =  fromdates.Value.Add(starttime.Value);
                }
                if (!string.IsNullOrEmpty(req.endTime))
                {
                    endtime = TimeSpan.Parse(req.endTime);
                    todates = todates.Value.Add(endtime.Value);
                }
                Security _securityobj = new Security();

                string Imagepath = "";
                if (!string.IsNullOrEmpty(req.FileName))
                    Imagepath = EventImagepath(req.FileName, Convert.ToString(req.userID));
                else if (!string.IsNullOrEmpty(req.filepath))
                    Imagepath = savefilepath(req.filepath, Convert.ToString(req.userID));

                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("CreateEvent", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@userID", req.userID);
                    com.Parameters.AddWithValue("@category", req.categoryID);
                    com.Parameters.AddWithValue("@eventName", req.eventName);
                    com.Parameters.AddWithValue("@location", req.location);
                    com.Parameters.AddWithValue("@startDate", fromdates);

                    com.Parameters.AddWithValue("@endDate", todates);
                    com.Parameters.AddWithValue("@eventDescription", req.eventDescription);
                    if(Imagepath !="")
                    com.Parameters.AddWithValue("@imagepath", Imagepath);
                    com.Parameters.AddWithValue("@latitude", req.latitude);
                    com.Parameters.AddWithValue("@longitude", req.longitude);

                    com.Parameters.AddWithValue("@starttime", starttime);
                    com.Parameters.AddWithValue("@endtime", endtime);
                    com.Parameters.AddWithValue("@accesstoken", req.accessToken);

                    if (req.eventID > 0)
                    {
                        com.Parameters.AddWithValue("@eventID", req.eventID);
                    }

                    if (!string.IsNullOrEmpty(req.dealtext))
                        com.Parameters.AddWithValue("@dealtext", req.dealtext);

                    if (!string.IsNullOrEmpty(req.percentage))
                        com.Parameters.AddWithValue("@percentage", req.percentage);



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

                    if (dt.Rows.Count == 0)
                    {
                        Helper.FillResult(Result, ErrorCode.FillResponseFailed, "");
                    }
                    else
                    {

                        Event _event = convertdatatableToObject.DatatabletoEvent(dt);
                        Helper.FillResult(Result, ErrorCode.Success, _event);
                    }
                }


            }
            catch (Exception ex)
            {
                Helper.FillResult(Result, ErrorCode.NoAuthentication, "");
                throw;
            }
        }


        public string EventImagepath(string Filepath, string userid)
        {


            string path = HttpContext.Current.Server.MapPath("~/Content/Image/Event/") + userid.ToString();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string name = Guid.NewGuid().ToString() + ".Jpg";

            //var byteArray = ms.ToArray();
            string Filename = Path.Combine(path, name);
            if (Directory.Exists(Filename))
            {
                Directory.Delete(Filename);
            }
            //string filePath = "~/Content/Image/FBUser/" + Path.GetFileName("ProfilPic.Png")


            if (File.Exists(Filepath))
            {
                //if (File.Exists(Filename))
                //    File.Delete(Filename);
                File.Copy(Filepath, Filename, true);
            }

            Filepath = "/Content/Image/Event/" + userid.ToString() + "/" + name;
            return Filepath;

        }

        public string savefilepath(string Filepath, string userid)
        {
            string path = HttpContext.Current.Server.MapPath("~/Content/Image/Event/") + userid.ToString();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string name = Guid.NewGuid().ToString() + ".Jpg";

            //var byteArray = ms.ToArray();
            string Filename = Path.Combine(path, name);
            if (Directory.Exists(Filename))
            {
                Directory.Delete(Filename);
            }
            //string filePath = "~/Content/Image/FBUser/" + Path.GetFileName("ProfilPic.Png")
            WebClient webClient = new WebClient();
            webClient.DownloadFile(Filepath, Filename);
            //  File.Copy(Filepath, Filename, true);


            Filepath = "/Content/Image/Event/" + userid.ToString() + "/" + name;
            return Filepath;
        }
    }
}