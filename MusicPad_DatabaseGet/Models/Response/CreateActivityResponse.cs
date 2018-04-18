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
    public class CreateActivityResponse : BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            DataTable dt = new DataTable();
            try
            {
                DateTime? subfromdates = null, subtodates = null;
                TimeSpan? substarttime = null, subendtime = null;

                var req = (request as MusicEventAPI.Models.Request.CreateActivityRequest);

                if (!string.IsNullOrEmpty(req.startDate))
                {
                    subfromdates = DateTime.ParseExact(req.startDate, "MM-dd-yyyy", CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(req.endDate))
                {
                    subtodates = DateTime.ParseExact(req.endDate, "MM-dd-yyyy", CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(req.startTime))
                {
                    substarttime = TimeSpan.Parse(req.startTime);
                    subfromdates.Value.Add(substarttime.Value);
                }
                if (!string.IsNullOrEmpty(req.endTime))
                {
                    subendtime = TimeSpan.Parse(req.endTime);
                    subtodates.Value.Add(subendtime.Value);
                }
                Security _securityobj = new Security();

                string Imagepath = "";
                if (!string.IsNullOrEmpty(req.FileName))
                    Imagepath = EventImagepath(req.FileName, Convert.ToString(req.eventId));
                else if (!string.IsNullOrEmpty(req.filepath))
                    Imagepath = savefilepath(req.filepath, Convert.ToString(req.eventId));

                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("CreateActivity", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@userID", req.userID);
                    com.Parameters.AddWithValue("@eventId", req.eventId);
                    com.Parameters.AddWithValue("@activityName", req.activityName);
                    com.Parameters.AddWithValue("@location", req.location);
                    com.Parameters.AddWithValue("@startDate", subfromdates);
                    com.Parameters.AddWithValue("@endDate", subtodates);
                    com.Parameters.AddWithValue("@activityDescription", req.activityDescription);
                    if(Imagepath !="")
                    com.Parameters.AddWithValue("@imagepath", Imagepath);
                    com.Parameters.AddWithValue("@starttime", substarttime);
                    com.Parameters.AddWithValue("@endtime", subendtime);
                    com.Parameters.AddWithValue("@latitude", req.latitude);
                    com.Parameters.AddWithValue("@longitude", req.longitude);
                    com.Parameters.AddWithValue("@accesstoken", req.accessToken);

                    if (req.activityId > 0)
                    {
                        com.Parameters.AddWithValue("@activityid", req.activityId);
                    }

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
                        Activity _activity = convertdatatableToObject.DatatabletoActivity(dt);
                        Helper.FillResult(Result, ErrorCode.Success, _activity);
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


            string path = HttpContext.Current.Server.MapPath("~/Content/Image/Activity/") + userid.ToString();
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

            Filepath = "/Content/Image/Activity/" + userid.ToString() + "/" + name;
            return Filepath;

        }

        public string savefilepath(string Filepath, string userid)
        {
            string path = HttpContext.Current.Server.MapPath("~/Content/Image/Activity/") + userid.ToString();
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


            Filepath = "/Content/Image/Activity/" + userid.ToString() + "/" + name;
            return Filepath;
        }
    }
}