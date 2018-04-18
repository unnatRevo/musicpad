using MusicEventAPI.Manage;
using MusicEventAPI.Models.Request;
using MusicEventAPI.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace MusicEventAPI.Models.Response
{
    public class SendMessageResponse : BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            string path = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/");
            DataTable dt = new DataTable();
            string OTP = GenrateOtp.NewOTP();
            try
            {
                var req = (request as MusicEventAPI.Models.Request.SendMessageRequest);

                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("SendMessage", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@groupId", req.groupId);
                    com.Parameters.AddWithValue("@userId", req.userId);
                    com.Parameters.AddWithValue("@chattext", req.chattext);
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

                    ChatDetail _chatdetail = new ChatDetail
                    {
                        groupId = Convert.ToInt32(dt.Rows[0]["GroupId"]),
                        userId = Convert.ToInt32(dt.Rows[0]["UserId"]),
                        chatID = Convert.ToInt32(dt.Rows[0]["ChatID"]),
                        chattext = Convert.ToString(dt.Rows[0]["Chattext"]),
                        userName = Convert.ToString(dt.Rows[0]["UserName"]),
                        createdDate = Convert.ToDateTime(dt.Rows[0]["CreatedDate"]).ToString("MM-dd-yyyy"),
                        createdTime = Convert.ToDateTime(dt.Rows[0]["CreatedDate"]).ToString("hh:mm"),
                        //imagepath = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/") + (string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["UserImagePath"])) ? "/content/Image/NoImage.png" : Convert.ToString(dt.Rows[0]["UserImagePath"])),
                        imagepath = ImagePath.GetimagePath(Convert.ToString(dt.Rows[0]["UserImagePath"]), HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/")),
                    };
                    sendNotificationABC(req.groupId, req.userId, path, _chatdetail);

                    Helper.FillResult(Result, ErrorCode.Success, _chatdetail);

                }


            }
            catch (Exception ex)
            {
                Helper.FillResult(Result, ErrorCode.NoAuthentication, "");
                throw;
            }
        }


        private void sendNotificationABC(string groupid, string userid,string path, ChatDetail detail)
        {

            System.Threading.ThreadStart threadStart = delegate () { SendNotification(groupid, userid, path, detail); };
            System.Threading.Thread thread = new System.Threading.Thread(threadStart);
            thread.Start();

        }

        public void SendNotification(string groupid, string userid, string path, ChatDetail detail)
        {
            List<NewChatPushNotification> list = new List<NewChatPushNotification>();
            DataTable dts = new DataTable();

            #region FachData
            try
            {

                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("NewChatPushNotification", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@GroupId", groupid);
                    com.Parameters.AddWithValue("@userid", userid);

                    SqlDataAdapter da = new SqlDataAdapter(com);

                    con.Open();
                    da.Fill(dts);
                    con.Close();

                    foreach (DataRow dr in dts.Rows)
                    {
                        NewChatPushNotification push = new NewChatPushNotification();
                        list.Add(
                            new NewChatPushNotification
                            {
                                newrequest = Convert.ToInt32(dr["pendingrequest"]) == 0 ? false : true,
                                groupId = Convert.ToString(dr["GroupId"]),
                                groupName = Convert.ToString(dr["GroupName"]),
                                DeviceToken = Convert.ToString(dr["DeviceToken"]),
                                DeviceType = Convert.ToString(dr["DeviceType"]),
                                groupType = Convert.ToString(dr["GroupType"]),
                                isadmin = Convert.ToBoolean(dr["IsAdmin"]),
                                tagname = Convert.ToString(dr["TagName"]),
                                appname = Convert.ToString(dr["appname"]),
                                //imagepath = path + (string.IsNullOrEmpty(Convert.ToString(dr["GroupImage"])) ? "/content/Image/NoImage.png" : Convert.ToString(dr["GroupImage"]))
                                imagepath = ImagePath.GetimagePath(Convert.ToString(dr["GroupImage"]), path),
                            }
                            );
                    }
                }
            }

            catch (Exception ex)
            {

            }

            #endregion
            string message = null;
            foreach (var item in list)
            {
                if (item.DeviceType == "ios")
                {
                    if (!string.IsNullOrEmpty(item.DeviceToken))
                    {
                        message = detail.chattext != null ?
                   detail.chattext.Substring(0, detail.chattext.Length >= 20 ? 20 : detail.chattext.Length) :
                   null;

                       var messagebody = "{\"aps\":{\"alert\":\"" + detail.chattext + "\",\"sound\":\"default\"},\"dictionary\":{\"groupType\":\"" + item.groupType + "\",\"messageFor\":\"newchat\",\"isadmin\":\"" + item.isadmin + "\",\"alert\":\"" + "" + "\",\"groupName\":\"" + item.groupName + "\",\"newrequest\":\"" + item.newrequest + "\",\"groupId\":" + item.groupId + "}}";
                        // var messagebody = "{\"dictionary\":{\"groupType\":\"" + item.groupType + "\",\"messageFor\":\"newchat\",\"isadmin\":\"" + item.isadmin + "\",\"groupimagepath\":\"" + item.imagepath + "\",\"userId\":\"" + detail.userId + "\",\"chatID \":\"" + detail.chatID + "\",\"chattext\":\"" + detail.chattext + "\",\"userName\":\"" + detail.userName + "\",\"createdDate\":\"" + detail.createdDate + "\",\"createdTime\":\"" + detail.createdTime + "\",\"imagepath\":\"" + detail.imagepath + "\",\"groupName\":\"" + item.groupName + "\",\"tagname\":\"" + item.tagname + "\",\"groupId\":" + item.groupId + "}}";
                          Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(messagebody);
                        PushNotificationIOS.sendMsg(item.DeviceToken, messagebody,item.appname);
                    }
                }
                else if (item.DeviceType == "Android")
                {
                    if (!string.IsNullOrEmpty(item.DeviceToken))
                    {

                        var datas = new
                        {
                            to = item.DeviceToken,
                            notification = new
                            {
                                body = detail.chattext,
                                title = item.groupName,
                                sound = "Enabled",
                                click_action ="ACTION_OPEN_PUSH"
                            },
                            data = new
                            {
                                groupId = item.groupId,
                                messageFor = "newchat",
                                groupType = item.groupType,
                                isadmin = item.isadmin,
                                groupName = item.groupName,
                                tagname = item.tagname,
                                groupimagepath = item.imagepath,
                                userId = detail.userId,
                                chatID = detail.chatID,
                                chattext =detail.chattext,
                                userName =detail.userName,
                                createdDate = detail.createdDate,
                                createdTime=detail.createdTime,
                                imagepath =detail.imagepath,
                                newrequest = item.newrequest
                            }
                    };
                        var serializer = new JavaScriptSerializer();
                        String json = serializer.Serialize(datas);
                        PushNotificationAndroid.SendPushnotification(json,item.appname);
                        //PushNotificationAndroid.SendPushnotification(item.DeviceToken, item.groupName, item.groupName, item.groupId, "newchat", "0");
                    }
                }
            }
        }
    }
}