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
    public class ChatResponse : BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            DataTable dt = new DataTable();
            List<Chat> _chatlist = new List<Chat>();
            try
            {
                var req = (request as MusicEventAPI.Models.Request.ChatRequest);


                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("GetChatList", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@userID", req.userId);
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

                    foreach (DataRow dr in dt.Rows)
                    {

                      Chat c = new Chat
                        {
                            groupId = Convert.ToInt32(dr["UserGroupId"]),
                            request = Convert.ToString(dr["request"]),
                            groupType = Convert.ToString(dr["GroupType"]),
                            isadmin = Convert.ToBoolean(dr["IsAdmin"]),
                            groupName = Convert.ToString(dr["GroupName"]),
                            tagname = Convert.ToString(dr["TagName"]),
                            //imagepath = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/") + (string.IsNullOrEmpty(Convert.ToString(dr["GroupImage"])) ? "/content/Image/NoImage.png" : Convert.ToString(dr["GroupImage"]))
                            imagepath = ImagePath.GetimagePath(Convert.ToString(dr["GroupImage"]), HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/")),
                      };
                        c.GroupUser = GetGroupUser(Convert.ToInt32(dr["UserGroupId"]));
                        _chatlist.Add(c);


                    }
                }
                Helper.FillResult(Result, ErrorCode.Success, _chatlist);
            }

            catch (Exception ex)
            {
                Helper.FillResult(Result, ErrorCode.NoAuthentication, "");
                throw;
            }
        }



        public List<JoinGroupUserList> GetGroupUser(int groupid)
        {
            using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
            {
                DataTable DTS = new DataTable();
                List<JoinGroupUserList> _userlist = new List<JoinGroupUserList>();
                SqlCommand com = new SqlCommand("GroupJoinUserList", con);
                com.CommandType = CommandType.StoredProcedure; ;
                com.Parameters.AddWithValue("@groupid", groupid);

                SqlDataAdapter da = new SqlDataAdapter(com);

                con.Open();
                da.Fill(DTS);
                con.Close();

                foreach (DataRow dr in DTS.Rows)
                {
                    _userlist.Add(
                        new JoinGroupUserList
                        {
                            groupId = Convert.ToInt32(dr["GroupId"]),
                            userId = Convert.ToInt32(dr["UserID"]),
                            userName = Convert.ToString(dr["UserName"]),
                            //imagePath = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/") + (string.IsNullOrEmpty(Convert.ToString(dr["UserImagePath"])) ? "/content/Image/NoImage.png" : Convert.ToString(dr["UserImagePath"])),
                            imagePath = ImagePath.GetimagePath(Convert.ToString(dr["UserImagePath"]), HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/")),
                            ISApprove = Convert.ToBoolean(dr["ISApprove"]),
                            isAdmin = Convert.ToBoolean(dr["isAdmin"])
                        }
                        );
                }

                return _userlist;
            }
        }

    }



}