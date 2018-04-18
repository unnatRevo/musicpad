using MusicEventAPI.Manage;
using MusicEventAPI.Models.Request;
using MusicEventAPI.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace MusicEventAPI.Models.Response
{
    public class ProfileMissingDataResponse : BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            DataTable dt = new DataTable();
            string guid = Guid.NewGuid().ToString();
            try
            {
                Security _securityobj = new Security();
                var req = (request as MusicEventAPI.Models.Request.ProfileMissingDataRequest);
                string Imagepath = "";
                if (!string.IsNullOrEmpty(req.FileName))
                    Imagepath = UserImagepath(req.FileName, Convert.ToString(req.userID));
                else if (!string.IsNullOrEmpty(req.filepath))
                    Imagepath = savefilepath(req.filepath, Convert.ToString(req.userID));
                string password = _securityobj.Encrypt(req.password);
                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("FillProfileMissingData", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@name", req.name);
                    com.Parameters.AddWithValue("@userName", req.userName);
                    com.Parameters.AddWithValue("@email", req.emailId);
                    com.Parameters.AddWithValue("@password", password);
                    com.Parameters.AddWithValue("@userID", req.userID);
                    com.Parameters.AddWithValue("@accesstoken", req.accessToken);
                    com.Parameters.AddWithValue("@appname", req.appname);
                    

                    if (Imagepath !="")
                    com.Parameters.AddWithValue("@imagePath", Imagepath);

                        com.Parameters.AddWithValue("@isBusinessUser", req.isBusinessUser);
                    if (!string.IsNullOrEmpty(req.businessLocation))
                        com.Parameters.AddWithValue("@businessLocation", req.businessLocation);
                    if (!string.IsNullOrEmpty(req.bussinesslatitude))
                        com.Parameters.AddWithValue("@bussinesslatitude", req.bussinesslatitude);
                    if (!string.IsNullOrEmpty(req.businesslongitude))
                        com.Parameters.AddWithValue("@businesslongitude", req.businesslongitude);



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
                        if (Convert.ToInt32(dt.Rows[0]["UserID"]) == 0)
                        {
                            Helper.FillResult(Result, ErrorCode.EmailUsed, "");
                        }
                        else
                        {
                            User _user = convertdatatableToObject.DatatabletoUser(dt);
                            Helper.FillResult(Result, ErrorCode.Success, _user);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                Helper.FillResult(Result, ErrorCode.NoAuthentication, "");
                throw;
            }
        }


        public string UserImagepath(string Filepath, string userid)
        {


            string path = HttpContext.Current.Server.MapPath("~/Content/Image/User/") + userid.ToString();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string name = "ProfilPic.Jpg";

            //var byteArray = ms.ToArray();
          string  Filename = Path.Combine(path, "ProfilPic.Jpg");
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

            Filepath = "/Content/Image/User/" + userid.ToString() + "/" + name;
            return Filepath;

        }

        public string savefilepath(string Filepath, string userid)
        {
            string path = HttpContext.Current.Server.MapPath("~/Content/Image/User/") + userid.ToString();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string name = "ProfilPic.Jpg";

            //var byteArray = ms.ToArray();
            string Filename = Path.Combine(path, "ProfilPic.Jpg");
            if (Directory.Exists(Filename))
            {
                Directory.Delete(Filename);
            }
                //string filePath = "~/Content/Image/FBUser/" + Path.GetFileName("ProfilPic.Png")
                WebClient webClient = new WebClient();
            webClient.DownloadFile(Filepath, Filename);
            //  File.Copy(Filepath, Filename, true);


            Filepath = "/Content/Image/User/" + userid.ToString() + "/" + name;
            return Filepath;
        }
    }
}