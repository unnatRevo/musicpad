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
    public class JoinEventResponse : BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            DataTable dt = new DataTable();
            try
            {
                string path = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/");
                var req = (request as MusicEventAPI.Models.Request.JoinEventRequest);
                //  var cmdInfo = HHADataAccess.GetCommandDetails("UserLogin");

                //if (cmdInfo == null)
                //{
                //    throw new ArgumentException("Provider base mapping details or Connection String not found for 'GetLanguages'.");
                //}
                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("JoinEventWithUser", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@userId", req.userId);
                    com.Parameters.AddWithValue("@eventId", req.eventId);
                    if(!string.IsNullOrEmpty(req.shareGroupType))
                    com.Parameters.AddWithValue("@shareGroupType", req.shareGroupType);
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
                        Helper.FillResult(Result, ErrorCode.NoAuthentication, "");
                    }
                    else
                    {
                        if (Convert.ToString(dt.Rows[0]["SatusMessage"]) == "user  already join this event")
                            Helper.FillResult(Result, ErrorCode.alreadyJoinEvent, "");
                        else
                        {
                            sendNotificationDeligate(Convert.ToString(req.eventId), Convert.ToString(req.userId), req.shareGroupType);
                            if (!string.IsNullOrEmpty(req.shareGroupType))
                            {
                                sendNotificationGroup(Convert.ToString(req.eventId), Convert.ToString(req.userId), path, req.shareGroupType, Convert.ToString(dt.Rows[0]["chattetxt"]));
                            }
                            Helper.FillResult(Result, ErrorCode.Success, Convert.ToString(dt.Rows[0]["SatusMessage"]));
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

        private void sendNotificationDeligate(string eventid, string userid,string shareGroupType)
        {

            System.Threading.ThreadStart threadStart = delegate () { SendNotification(eventid, userid,shareGroupType); };
            System.Threading.Thread thread = new System.Threading.Thread(threadStart);
            thread.Start();

        }

        public void SendNotification(string eventID, string userid,string shareGroupType)
        {
            NewChatPushNotification item = new NewChatPushNotification();
            DataTable dts = new DataTable();
            string alert = "";
            #region FachData
            try
            {


                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("Push_AddToCalender", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@eventId", eventID);
                    com.Parameters.AddWithValue("@userId", userid);
                    if(!string.IsNullOrEmpty(shareGroupType))
                        com.Parameters.AddWithValue("@grouptype", shareGroupType);

                    SqlDataAdapter da = new SqlDataAdapter(com);

                    con.Open();
                    da.Fill(dts);
                    con.Close();

                    foreach (DataRow dr in dts.Rows)
                    {

                        item = new NewChatPushNotification
                        {
                            groupId = Convert.ToString(dr["EventID"]),
                            groupName = Convert.ToString(dr["EventName"]),
                            DeviceToken = Convert.ToString(dr["DeviceToken"]),
                            DeviceType = Convert.ToString(dr["DeviceType"]),
                            appname = Convert.ToString(dr["appname"])
                        };
                        alert = Convert.ToString(dr["UserName"]) + ", want to join your Event (" + item.groupName + ").";

                    }
                }
            }

            catch (Exception ex)
            {

            }

            #endregion

            if (item.DeviceType == "ios")
            {
                if (!string.IsNullOrEmpty(item.DeviceToken))
                {
                    var messagebody = "{\"aps\":{\"alert\":\"" + item.groupName + "\",\"sound\":\"default\"},\"dictionary\":{\"messageFor\":\"addtoCalender\",\"eventId\":\"" + item.groupId + "\",\"alert\":\"" + alert + "\"}}";
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
                            body = alert,
                            title = item.groupName,
                            sound = "Enabled",
                            click_action = "ACTION_OPEN_PUSH"
                        },
                        data = new
                        {
                            eventId = item.groupId,
                            messageFor = "addtoCalender"
                        }
                    };
                    var serializer = new JavaScriptSerializer();
                    String json = serializer.Serialize(datas);
                    PushNotificationAndroid.SendPushnotification(json,item.appname);
                    //PushNotificationAndroid.SendPushnotification(item.DeviceToken, item.groupName, item.groupName, item.groupId, "newchat", "0");
                }
            }
        }

        private void sendNotificationGroup(string groupid, string userid, string path, string grouptype,string message)
        {

            System.Threading.ThreadStart threadStart = delegate () { SendNotificationGroup(groupid, userid, path, grouptype, message); };
            System.Threading.Thread thread = new System.Threading.Thread(threadStart);
            thread.Start();

        }

        public void SendNotificationGroup(string enentid, string userid, string path, string grouptype, string text)
        {
            List<NewChatPushNotification> list = new List<NewChatPushNotification>();
            DataTable dts = new DataTable();

            #region FachData
            try
            {

                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("ShareEvent", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@eventid", enentid);
                    com.Parameters.AddWithValue("@userid", userid);
                    com.Parameters.AddWithValue("@grouptype", grouptype);

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
                                groupId = Convert.ToString(dr["GroupID"]),
                                groupName = Convert.ToString(dr["GroupName"]),
                                DeviceToken = Convert.ToString(dr["DeviceToken"]),
                                DeviceType = Convert.ToString(dr["DeviceType"]),
                                groupType = Convert.ToString(dr["GroupType"]),
                                isadmin = Convert.ToBoolean(dr["IsAdmin"]),
                                tagname = Convert.ToString(dr["TagName"]),
                                //imagepath = path + (string.IsNullOrEmpty(Convert.ToString(dr["GroupImage"])) ? "/content/Image/NoImage.png" : Convert.ToString(dr["GroupImage"])),
                                //EventImage = path + (string.IsNullOrEmpty(Convert.ToString(dr["image"])) ? "/content/Image/NoImage.png" : Convert.ToString(dr["image"])),
                                EventName = Convert.ToString(dr["EventName"]),
                                imagepath = ImagePath.GetimagePath(Convert.ToString(dr["GroupImage"]), path),
                                EventImage = ImagePath.GetimagePath(Convert.ToString(dr["image"]), path),
                                chattext = Convert.ToString(dr["GroupChat"]),
                                appname = Convert.ToString(dr["appname"]),
                                isevent = Convert.ToString(dr["isevent"]) =="0" ?false:true 
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


                        //var messagebody = "{\"aps\":{\"alert\":\"" + item.groupName + "\",\"sound\":\"default\"},\"messageFor\":\"newchat\",\"groupId\":\""+item.groupId +"\"}";
                        //   var messagebody = "{\"aps\":{\"alert\":\"" + item.groupName + "\",\"sound\":\"default\"},\"messageFor\":\"newchat\",\"groupId\":" + item.groupId.ToString() + "}";
                        // var messagebody = "{\"aps\":{\"alert\":\"" + item.groupName + "\",\"sound\":\"default\"},\"messageFor\":\"newchat\",\"groupId\":" + item.groupId.ToString() + "}";
                        // Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(messagebody);
                        //var messagebody = "{\"aps\":{\"alert\":\"" + item.groupName + "\",\"sound\":\"default\",\"groupId\":" + item.groupId + "},\"messageFor\":\"newchat\",\"acme2\":42}";
                        //  var messagebody = "{\"aps\":{\"alert\":\"" + item.groupName + "\",\"badge\":" + "20" + ",\"sound\":\"mailsent.wav\",\"groupId\":" + item.groupId + "},\"acme1\":\"bar\",\"acme2\":42}";
                        // var messagebody = "{\"aps\":{\"alert\":\"" + item.groupName +"New Message" + "\",\"badge\":" + 20 + ",\"sound\":\"mailsent.wav\"},\"NotificationFor\":\"NewChat\",\"groupId\":"+ item.groupId +"}";
                        // var messagebody = "{\"aps\":{\"alert\":\"" + item.groupName + "\",\"type\":" + NotificationFor + ",\"sound\":\"default\",\"groupId\":" + item.groupId + "},\"acme1\":\"bar\",\"acme2\":42}";
                        //final working  var messagebody = "{\"aps\":{\"alert\":\"" + item.groupName + "\",\"sound\":\"default\"},\"dictionary\":{\"groupType\":\"" + item.groupType + "\",\"messageFor\":\"newchat\",\"isadmin\":\"" + item.isadmin + "\",\"groupName\":\"" + item.groupName + "\",\"tagname\":\"" + item.tagname + "\",\"groupId\":" + item.groupId + "}}";
                        var messagebody = "{\"aps\":{\"alert\":\"" + item.tagname + "\",\"sound\":\"default\"},\"dictionary\":{\"groupType\":\"" + item.groupType + "\",\"newrequest\":\"" + item.newrequest + "\",\"messageFor\":\"ShareEvent\",\"isadmin\":\"" + item.isadmin + "\",\"alert\":\"" + "" + "\",\"groupName\":\"" + item.groupName + "\",\"groupId\":" + item.groupId + ",\"eventId\":" + enentid + "}}";
                        // var messagebody = "{\"dictionary\":{\"groupType\":\"" + item.groupType + "\",\"messageFor\":\"newchat\",\"isadmin\":\"" + item.isadmin + "\",\"groupimagepath\":\"" + item.imagepath + "\",\"userId\":\"" + detail.userId + "\",\"chatID \":\"" + detail.chatID + "\",\"chattext\":\"" + detail.chattext + "\",\"userName\":\"" + detail.userName + "\",\"createdDate\":\"" + detail.createdDate + "\",\"createdTime\":\"" + detail.createdTime + "\",\"imagepath\":\"" + detail.imagepath + "\",\"groupName\":\"" + item.groupName + "\",\"tagname\":\"" + item.tagname + "\",\"groupId\":" + item.groupId + "}}";
                        Newtonsoft.Json.Linq.JObject json = Newtonsoft.Json.Linq.JObject.Parse(messagebody);
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
                                body = item.chattext,
                                title = item.groupName,
                                sound = "Enabled",
                                click_action = "ACTION_OPEN_PUSH"
                            },
                            data = new
                            {
                                groupId = item.groupId,
                                messageFor = "ShareEvent",
                                groupType = item.groupType,
                                isadmin = item.isadmin,
                                groupName = item.groupName,
                                tagname = item.tagname,
                                groupimagepath = item.imagepath,
                                eventId = enentid,
                                isEvent = item.isevent,
                                chattext = item.chattext,
                                imagepath = item.EventImage,
                                eventName = item.EventName,
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