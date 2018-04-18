using MusicEventAPI.Manage;
using MusicEventAPI.Models.Request;
using MusicEventAPI.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace MusicEventAPI.Models.Response
{
    public class LoginFacebookResponse : BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            DataTable dt = new DataTable();
            string guid = Guid.NewGuid().ToString();
            try
            {

                var req = (request as MusicEventAPI.Models.Request.LoginFacebookRequest);
                string Imagepath = "";
                if (!string.IsNullOrEmpty(req.FileName))
                    Imagepath = UserImagepath(req.FileName, req.fbid);
                else if (!string.IsNullOrEmpty(req.filepath))
                    Imagepath = savefilepath(req.filepath, req.fbid);

                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("LoginFacebook", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@FbID", req.fbid);
                    if(!string.IsNullOrEmpty(req.emailId))
                    com.Parameters.AddWithValue("@EmailID", req.emailId);
                    com.Parameters.AddWithValue("@Username", req.userName);
                    com.Parameters.AddWithValue("@Name", req.name);
                    com.Parameters.AddWithValue("@DeviceToken", req.deviceToken);
                    com.Parameters.AddWithValue("@DeviceType", req.deviceType);
                    com.Parameters.AddWithValue("@guid", guid);
                    com.Parameters.AddWithValue("@faceBookFriends", req.faceBookFriends);
                    com.Parameters.AddWithValue("@appname", req.appname);
                    if (Imagepath !="")
                    com.Parameters.AddWithValue("@imagePath", Imagepath);
                    if(req.isLogin =="1")
                    com.Parameters.AddWithValue("@isLogin",true);
                    else
                        com.Parameters.AddWithValue("@isLogin", false);


                    SqlDataAdapter da = new SqlDataAdapter(com);

                    con.Open();
                    da.Fill(dt);
                    con.Close();

                    if (dt.Rows.Count > 0)
                    {
                        DataColumnCollection columns = dt.Columns;
                        if (columns.Contains("ErrorMessage"))
                        {
                            if (Convert.ToString(dt.Rows[0]["ErrorMessage"]) == "Invalid")
                                Helper.FillResult(Result, ErrorCode.NOUSER, "");
                            return;
                        }
                    }

                    if (dt.Rows.Count == 0)
                    {
                        Helper.FillResult(Result, ErrorCode.FillResponseFailed, "");
                    }
                    else
                    {
                        User _user = convertdatatableToObject.DatatabletoUser(dt);

                        Helper.FillResult(Result, ErrorCode.Success, _user);
                    }
                }


            }
            catch (Exception ex)
            {
                Helper.FillResult(Result, ErrorCode.NoAuthentication, "");
                throw;
            }
        }


        public string UserImagepath(string Filepath, string fbid)
        {


            string path = HttpContext.Current.Server.MapPath("~/Content/Image/FBUser/") + fbid.ToString();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string name = "ProfilPic.Jpg";

            //var byteArray = ms.ToArray();
            string Filename = Path.Combine(path, "ProfilPic.Jpg");
            //string filePath = "~/Content/Image/FBUser/" + Path.GetFileName("ProfilPic.Png")
            if (Directory.Exists(Filename))
            {
                Directory.Delete(Filename);
            }

            if (File.Exists(Filepath))
            {
                //if (File.Exists(Filename))
                //    File.Delete(Filename);
                File.Copy(Filepath, Filename, true);
            }

            //if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
            //{
            //    string[] testfiles = file.FileName.Split(new char[] { '\\' });
            //    fname = testfiles[testfiles.Length - 1];
            //}
            //else

            //string path = HttpContext.Current.Server.MapPath("~/Content/Image/FBUser/") + fbid.ToString();
            //if (!Directory.Exists(path))
            //{
            //    Directory.CreateDirectory(path);
            //}
            //fname = Path.Combine(path, fname);
            //// string imageFile = Path.Combine(Server.MapPath("~/Uploads"), fname);
            //file.SaveAs(fname);
            Filepath = "/Content/Image/FBUser/" + fbid.ToString() + "/" + name;
            return Filepath;

        }
        public string savefilepath(string Filepath, string fbid)
        {
            string path = HttpContext.Current.Server.MapPath("~/Content/Image/FBUser/") + fbid.ToString();
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


            Filepath = "/Content/Image/FBUser/" + fbid.ToString() + "/" + name;
            return Filepath;
        }
    }
}