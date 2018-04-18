using MusicEventAPI.Manage;
using MusicEventAPI.Models.Request;
using MusicEventAPI.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace MusicEventAPI.Models.Response
{
    public class CreateGroupResponce : BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            DataTable dt = new DataTable();

            try
            {
                var req = (request as MusicEventAPI.Models.Request.CreateGroupRequest);

                string Imagepath = "";
                if (!string.IsNullOrEmpty(req.image))
                    Imagepath = UserImagepath(req.image, Convert.ToString(req.userId));

                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("GroupCreate", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@userID", req.userId);
                    com.Parameters.AddWithValue("@groupid", req.groupId);
                    com.Parameters.AddWithValue("@groupname", req.groupName);
                    if(Imagepath !="")
                    com.Parameters.AddWithValue("@image", Imagepath);
                    com.Parameters.AddWithValue("@tagname", req.tagName);
                    com.Parameters.AddWithValue("@grouptype", req.groupType);
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

                    if (dt.Rows.Count == 0)
                    {
                        Helper.FillResult(Result, ErrorCode.FillResponseFailed, "");
                    }
                    else
                    {
                        if (Convert.ToString(dt.Rows[0]["status"]) == "No Friend")
                        {
                            Helper.FillResult(Result, ErrorCode.noFriend, "");
                        }
                        else if (Convert.ToString(dt.Rows[0]["status"]) != "success")
                        {
                            Helper.FillResult(Result, ErrorCode.GroupalreadyCreated, "");
                        }
                        else
                        {
                            Group _group = new Group
                            {
                                adminId = Convert.ToInt32(dt.Rows[0]["AdminId"]),
                                groupId = Convert.ToInt32(dt.Rows[0]["UserGroupId"]),
                                groupName = Convert.ToString(dt.Rows[0]["GroupName"]),
                                //image = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/") + (string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["GroupImage"])) ? "/content/Image/NoImage.png" : Convert.ToString(dt.Rows[0]["GroupImage"])),
                                image = ImagePath.GetimagePath(Convert.ToString(dt.Rows[0]["GroupImage"]), HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/")),
                                tagName = Convert.ToString(dt.Rows[0]["TagName"]),
                                groupType = Convert.ToString(dt.Rows[0]["GroupType"]),
                                isActive = Convert.ToBoolean(dt.Rows[0]["ISActive"])
                            };
                            Helper.FillResult(Result, ErrorCode.Success, _group);
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


            string path = HttpContext.Current.Server.MapPath("~/Content/Image/Group/") + userid.ToString();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string name = Guid.NewGuid().ToString() + ".Jpg";

            //var byteArray = ms.ToArray();
            string Filename = Path.Combine(path, name);
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

            Filepath = "/Content/Image/Group/" + userid.ToString() + "/" + name;
            return Filepath;

        }

    }
}