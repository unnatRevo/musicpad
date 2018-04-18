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
    public class GroupUserResponse : BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            DataTable dt = new DataTable();
            List<GroupUserList> _userlist = new List<GroupUserList>();
            try
            {
                var req = (request as MusicEventAPI.Models.Request.GroupUserRequest);


                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("GroupUserList", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@userID", req.userId);
                    com.Parameters.AddWithValue("@groupid", req.groupId);
                    com.Parameters.AddWithValue("@accesstoken", req.accessToken);
                    //com.Parameters.AddWithValue("@facebookfriends", req.facebookFriends);
                    //com.Parameters.AddWithValue("@phoneNo", req.phoneNo);

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
                        _userlist.Add(
                            new GroupUserList
                            {
                                groupId = Convert.ToInt32(dr["Groupid"]),
                                userId = Convert.ToInt32(dr["UserID"]),
                                userName = Convert.ToString(dr["UserName"]),
                                //imagePath = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/") +( string.IsNullOrEmpty(Convert.ToString(dr["UserImagePath"])) ? "/content/Image/NoImage.png" :  Convert.ToString(dr["UserImagePath"])),
                                imagePath = ImagePath.GetimagePath(Convert.ToString(dr["UserImagePath"]), HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/")),
                                isnew = Convert.ToBoolean(dr["isnew"]),
                                ISApprove = Convert.ToBoolean(dr["ISApprove"])
                            }
                            );
                    }
                }
                Helper.FillResult(Result, ErrorCode.Success, _userlist);
            }

            catch (Exception ex)
            {
                Helper.FillResult(Result, ErrorCode.NoAuthentication, "");
                throw;
            }
        }
    }
}