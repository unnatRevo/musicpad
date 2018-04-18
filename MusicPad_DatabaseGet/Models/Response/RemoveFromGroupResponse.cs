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
    public class RemoveFromGroupResponse : BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            string path = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/");
            DataTable dt = new DataTable();
            try
            {
                var req = (request as MusicEventAPI.Models.Request.RemoveFromGroupRequest);


                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("RemoveFromGroup", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@userId", req.userId);
                    com.Parameters.AddWithValue("@GroupId", req.groupId);

                    if (req.isadd == "1")
                        com.Parameters.AddWithValue("@isadd", true);
                    else
                        com.Parameters.AddWithValue("@isadd", false);

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

                    if (Convert.ToString(dt.Rows[0]["SatusMessage"]) == "success")
                    {
                        if (req.isadd == "1")
                            sendNotificationDeligate(Convert.ToString(req.groupId), Convert.ToString(req.userId), path);
                        Helper.FillResult(Result, ErrorCode.Success, "");
                    }
                    else
                    {
                        Helper.FillResult(Result, ErrorCode.NouserWithThisGroup, "");
                    }
                }
            }

            catch (Exception ex)
            {
                Helper.FillResult(Result, ErrorCode.NoAuthentication, "");
                throw;
            }
        }

        private void sendNotificationDeligate(string groupid, string userid, string path)
        {

            System.Threading.ThreadStart threadStart = delegate () { SendNotification(groupid, userid, path); };
            System.Threading.Thread thread = new System.Threading.Thread(threadStart);
            thread.Start();

        }

        public void SendNotification(string groupid, string userid, string path)
        {
            NewChatPushNotification item = new NewChatPushNotification();
            DataTable dts = new DataTable();
            string alert = "";
            #region FachData
            try
            {
                

                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("Push_NewRequest", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@GroupId", groupid);
                    com.Parameters.AddWithValue("@userid", userid);

                    SqlDataAdapter da = new SqlDataAdapter(com);

                    con.Open();
                    da.Fill(dts);
                    con.Close();

                    foreach (DataRow dr in dts.Rows)
                    {

                        item = new NewChatPushNotification
                        {
                            groupId = Convert.ToString(dr["GroupId"]),
                            groupName = Convert.ToString(dr["GroupName"]),
                            DeviceToken = Convert.ToString(dr["DeviceToken"]),
                            DeviceType = Convert.ToString(dr["DeviceType"]),
                            groupType = Convert.ToString(dr["GroupType"]),
                            isadmin = Convert.ToBoolean(dr["IsAdmin"]),
                            tagname = Convert.ToString(dr["TagName"]),
                            //imagepath = path + (string.IsNullOrEmpty(Convert.ToString(dr["GroupImage"])) ? "/content/Image/NoImage.png" : Convert.ToString(dr["GroupImage"]))
                            imagepath = ImagePath.GetimagePath(Convert.ToString(dr["Image"]), path),
                            appname = Convert.ToString(dr["appname"]),
                        };
                        alert = Convert.ToString(dr["UserName"]) +", want to join your group (" + item.groupName + ").";

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
                    var messagebody = "{\"aps\":{\"alert\":\"" + item.groupName + "\",\"sound\":\"default\"},\"dictionary\":{\"groupType\":\"" + item.groupType + "\",\"messageFor\":\"groupRequest\",\"isadmin\":\"" + item.isadmin + "\",\"groupName\":\"" + item.groupName + "\",\"alert\":\"" + alert + "\",\"groupId\":" + item.groupId + "}}";
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
                            groupId = item.groupId,
                            messageFor = "groupRequest",
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