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
    public class InviteGroupResponse : BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            string path = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/");
            DataTable dt = new DataTable();

            try
            {
                var req = (request as MusicEventAPI.Models.Request.InviteGroupRequest);

                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("InviteGroup", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@userID", req.userId);
                    com.Parameters.AddWithValue("@groupID", req.groupId);
                    com.Parameters.AddWithValue("@inviteUserID", req.InviteUserID);
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
                            Group _group = new Group
                            {
                                adminId = Convert.ToInt32(dt.Rows[0]["AdminId"]),
                                groupId = Convert.ToInt32(dt.Rows[0]["UserGroupId"]),
                                groupName = Convert.ToString(dt.Rows[0]["GroupName"]),
                                //image = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/") +( string.IsNullOrEmpty(Convert.ToString(dt.Rows[0]["GroupImage"])) ? "/content/Image/NoImage.png" :  Convert.ToString(dt.Rows[0]["GroupImage"])),
                                image = ImagePath.GetimagePath(Convert.ToString(dt.Rows[0]["GroupImage"]), HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/")),
                                tagName = Convert.ToString(dt.Rows[0]["TagName"]),
                                groupType = Convert.ToString(dt.Rows[0]["GroupType"]),
                                isActive = Convert.ToBoolean(dt.Rows[0]["ISActive"])
                            };
                        sendNotificationThread(Convert.ToString(req.groupId),Convert.ToString(req.userId), path, req.InviteUserID);
                        Helper.FillResult(Result, ErrorCode.Success, _group);
                        }
                }


            }
            catch (Exception ex)
            {
                Helper.FillResult(Result, ErrorCode.NoAuthentication, "");
                throw;
            }
        }

        private void sendNotificationThread(string groupid, string userid, string path,string InviteUserID)
        {

            System.Threading.ThreadStart threadStart = delegate () { SendNotification(groupid, userid, path, InviteUserID); };
            System.Threading.Thread thread = new System.Threading.Thread(threadStart);
            thread.Start();

        }

        public void SendNotification(string groupid, string userid, string path,string InviteUserID)
        {
            List<NewChatPushNotification> list = new List<NewChatPushNotification>();
            DataTable dts = new DataTable();

            #region FachData
            try
            {

                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("Push_joinGroup", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@GroupId", groupid);
                    com.Parameters.AddWithValue("@userid", userid);
                    com.Parameters.AddWithValue("@inviteUserID", InviteUserID);

                    SqlDataAdapter da = new SqlDataAdapter(com);

                    con.Open();
                    da.Fill(dts);
                    con.Close();

                    foreach (DataRow dr in dts.Rows)
                    {

                        list.Add(
                            new NewChatPushNotification
                            {
                                groupId = Convert.ToString(dr["GroupId"]),
                                groupName = Convert.ToString(dr["GroupName"]),
                                DeviceToken = Convert.ToString(dr["DeviceToken"]),
                                DeviceType = Convert.ToString(dr["DeviceType"]),
                                groupType = Convert.ToString(dr["GroupType"]),
                                isadmin = Convert.ToBoolean(dr["IsAdmin"]),
                                tagname = Convert.ToString(dr["TagName"]),
                                //imagepath = path + (string.IsNullOrEmpty(Convert.ToString(dr["GroupImage"])) ? "/content/Image/NoImage.png" : Convert.ToString(dr["GroupImage"]))
                                imagepath = ImagePath.GetimagePath(Convert.ToString(dr["GroupImage"]), path),
                                appname = Convert.ToString(dr["appname"])
                            }
                            );
                    }
                }
            }

            catch (Exception ex)
            {

            }

            #endregion

            foreach (var item in list)
            {
                string alert = item.groupName + ", has added you in this group.";
                if (item.DeviceType == "ios")
                {
                    if (!string.IsNullOrEmpty(item.DeviceToken))
                    {

                        //var messagebody = "{\"aps\":{\"alert\":\"" + item.groupName + "\",\"sound\":\"default\"},\"messageFor\":\"newchat\",\"groupId\":\""+item.groupId +"\"}";
                        //   var messagebody = "{\"aps\":{\"alert\":\"" + item.groupName + "\",\"sound\":\"default\"},\"messageFor\":\"newchat\",\"groupId\":" + item.groupId.ToString() + "}";
                        // var messagebody = "{\"aps\":{\"alert\":\"" + item.groupName + "\",\"sound\":\"default\"},\"messageFor\":\"newchat\",\"groupId\":" + item.groupId.ToString() + "}";
                        // Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(messagebody);
                        //var messagebody = "{\"aps\":{\"alert\":\"" + item.groupName + "\",\"sound\":\"default\",\"groupId\":" + item.groupId + "},\"messageFor\":\"newchat\",\"acme2\":42}";
                        //  var messagebody = "{\"aps\":{\"alert\":\"" + item.groupName + "\",\"badge\":" + "20" + ",\"sound\":\"mailsent.wav\",\"groupId\":" + item.groupId + "},\"acme1\":\"bar\",\"acme2\":42}";
                        // var messagebody = "{\"aps\":{\"alert\":\"" + item.groupName +"New Message" + "\",\"badge\":" + 20 + ",\"sound\":\"mailsent.wav\"},\"NotificationFor\":\"NewChat\",\"groupId\":"+ item.groupId +"}";
                        // var messagebody = "{\"aps\":{\"alert\":\"" + item.groupName + "\",\"type\":" + NotificationFor + ",\"sound\":\"default\",\"groupId\":" + item.groupId + "},\"acme1\":\"bar\",\"acme2\":42}";

                        var messagebody = "{\"aps\":{\"alert\":\"" + item.groupName + "\",\"sound\":\"default\"},\"dictionary\":{\"groupType\":\"" + item.groupType + "\",\"messageFor\":\"joinGroup\",\"isadmin\":\"" + item.isadmin + "\",\"groupName\":\"" + item.groupName + "\",\"alert\":\"" + alert + "\",\"groupId\":" + item.groupId + "}}";
                        // Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(messagebody);
                        PushNotificationIOS.sendMsg(item.DeviceToken, messagebody,item.appname);
                    }
                    //Testmodel.sendMsg();
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
                                body = alert ,
                                title = item.groupName,
                                sound = "Enabled",
                                click_action = "ACTION_OPEN_PUSH"
                            },
                            data = new
                            {
                                groupId = item.groupId,
                                messageFor = "joinGroup",
                                groupType = item.groupType,
                                isadmin = item.isadmin,
                                groupName = item.groupName,
                                tagname = item.tagname,
                                imagepath = item.imagepath
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