using MusicEventAPI.Manage;
using MusicEventAPI.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace MusicEventAPI.Models
{
    public static class PushNotificationEveryHour
    {
        static int x = 0;
        static string path = HttpContext.Current.Server.MapPath("~/Content/Image/Event/");
        public static List<EvntPushNotification> GetEventList()
        {
            DataTable dt = new DataTable();
            List<EvntPushNotification> _model = new List<EvntPushNotification>();

            using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
            {
                SqlCommand com = new SqlCommand("GetEventListLast1hour", con);
                com.CommandType = CommandType.StoredProcedure;

                SqlDataAdapter da = new SqlDataAdapter(com);

                con.Open();
                da.Fill(dt);
                con.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    _model.Add(
                        new EvntPushNotification
                        {
                            eventId = Convert.ToString(dr["EventID"]),
                            eventName = Convert.ToString(dr["EventName"]),
                            DeviceToken = Convert.ToString(dr["DeviceToken"]),
                            DeviceType = Convert.ToString(dr["DeviceType"]),
                            appname = Convert.ToString(dr["appname"])
                        }
                        );
                }

                return _model;
            }
        }
        public static void StartCheckingLog()
        {

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000;
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        static void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
         {
            try
            {
                if (x == 0)
                {
                    x = x + 1;
                    //nyc n = new nyc();
                    //n.InsertIntoSql();
                }
                SendNotification();
            }
            catch (Exception ex)
            {
               
            }
         
           
        }

        public static void SendNotification()
        {
            List<EvntPushNotification> list = GetEventList();
       
            string message = null;
            foreach (var item in list)
            {
                message = "Event " + item.eventName + "start within 1 hour";
                if (item.DeviceType == "ios")
                {
                    
                    if (!string.IsNullOrEmpty(item.DeviceToken))
                    {

                        var messagebody = "{\"aps\":{\"alert\":\"" + message + "\",\"sound\":\"default\"},\"dictionary\":{\"messageFor\":\"startinhour\",\"alert\":\"" + "" + "\",\"eventName\":\"" + item.eventName + "\",\"eventId\":" + item.eventId + "}}";
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
                                body = message,
                                title = item.eventName,
                                sound = "Enabled",
                                click_action = "ACTION_OPEN_PUSH"
                            },
                            data = new
                            {
                                eventId = item.eventId,
                                messageFor = "startinhour"
                            }
                        };
                        var serializer = new JavaScriptSerializer();
                        String json = serializer.Serialize(datas);
                        PushNotificationAndroid.SendPushnotification(json, item.appname);
                    }
                }
            }
        }


    }
}