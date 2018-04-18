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
    public class MyChatResponse : BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            DataTable dt = new DataTable();
            List<Chat> _chatlist = new List<Chat>();
            try
            {
                var req = (request as MusicEventAPI.Models.Request.MyChatRequest);


                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("GetMyChatList", con);
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
                        _chatlist.Add(
                            new Chat
                            {
                                newrequest = Convert.ToInt32(dr["pendingrequest"]) == 0 ?false : true,
                                newMessageCount = Convert.ToString(dr["newmessagecoumt"]),
                                groupType = Convert.ToString(dr["GroupType"]),
                                groupId = Convert.ToInt32(dr["UserGroupId"]),
                                isadmin = Convert.ToBoolean(dr["IsAdmin"]),
                                groupName = Convert.ToString(dr["GroupName"]),
                                tagname = Convert.ToString(dr["TagName"]),
                                //imagepath = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/") +( string.IsNullOrEmpty(Convert.ToString(dr["GroupImage"])) ? "/content/Image/NoImage.png" :  Convert.ToString(dr["GroupImage"]))
                                imagepath = ImagePath.GetimagePath(Convert.ToString(dr["GroupImage"]), HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/")),
                            }
                            );
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
    }
}