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
    public class ChatDetailResponse : BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            DataTable dt = new DataTable();
            List<ChatDetail> _chatDetaillist = new List<ChatDetail>();
            try
            {
                var req = (request as MusicEventAPI.Models.Request.ChatDetailRequest);
                int totalpage = 0;
                dynamic obj = new { };
                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("GroupChatDetail", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@userID", req.userId);
                    com.Parameters.AddWithValue("@groupID", req.groupId);
                    com.Parameters.AddWithValue("@PageNumber", req.pageNo == 0 ? 1 : req.pageNo);
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


                    if (dt.Rows.Count > 0)
                    {
                        totalpage = Convert.ToInt32(dt.Rows[0]["TotalRecords"]);
                        totalpage = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(totalpage) / 10));
                    }
                    DataView view = dt.DefaultView;
                    view.Sort = "CreatedDate ASC";
                    DataTable sortedDate = view.ToTable();

                    foreach (DataRow dr in sortedDate.Rows)
                    {
                        _chatDetaillist.Add(
                            new ChatDetail
                            {
                                groupId = Convert.ToInt32(dr["GroupId"]),
                                userId = Convert.ToInt32(dr["UserId"]),
                                chatID = Convert.ToInt32(dr["ChatID"]),
                                chattext = Convert.ToString(dr["Chattext"]),
                                userName = Convert.ToString(dr["UserName"]),
                                createdDate = Convert.ToDateTime(dr["CreatedDate"]).ToString("MM-dd-yyyy"),
                                createdTime = Convert.ToDateTime(dr["CreatedDate"]).ToString("hh:mm"),
                                //imagepath = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/") + (string.IsNullOrEmpty(Convert.ToString(dr["UserImagePath"])) ? "/content/Image/NoImage.png" :  Convert.ToString(dr["UserImagePath"])),
                                imagepath = ImagePath.GetimagePath(Convert.ToString(dr["UserImagePath"]), HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/")),
                                isEvent = Convert.ToString(dr["IsEvent"]) =="False" ?false :true,
                                eventId = Convert.ToInt32(dr["EventID"]),
                                eventName = Convert.ToString(dr["EventName"])
                            }
                            );
                    }
                }



                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("applepushnotificationChat", con);
                    DataTable dats = new DataTable();
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@userID", req.userId);
                    com.Parameters.AddWithValue("@groupid", req.groupId);

                    SqlDataAdapter da = new SqlDataAdapter(com);

                    con.Open();
                    da.Fill(dats);
                    con.Close();

                    foreach (DataRow dr in dats.Rows)
                    {
                        obj = new
                        {
                          
                                newrequest = Convert.ToInt32(dr["pendingrequest"]) == 0 ? false : true,
                                newMessageCount = Convert.ToString(dr["newmessagecoumt"]),
                                groupType = Convert.ToString(dr["GroupType"]),
                                groupId = Convert.ToInt32(dr["UserGroupId"]),
                                isadmin = Convert.ToBoolean(dr["IsAdmin"]),
                                groupName = Convert.ToString(dr["GroupName"]),
                                tagname = Convert.ToString(dr["TagName"]),
                                //imagepath = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/") + (string.IsNullOrEmpty(Convert.ToString(dr["GroupImage"])) ? "/content/Image/NoImage.png" : Convert.ToString(dr["GroupImage"]))
                                ImagePath = ImagePath.GetimagePath(Convert.ToString(dr["GroupImage"]), HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/")),
                        };
                    }
                }


                Helper.FillResult(Result, ErrorCode.Success, _chatDetaillist, totalpage,obj);
            }

            catch (Exception ex)
            {
                Helper.FillResult(Result, ErrorCode.NoAuthentication, "");
                throw;
            }
        }
    }
}