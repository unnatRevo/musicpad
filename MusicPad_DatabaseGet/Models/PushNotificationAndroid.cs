using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace MusicEventAPI.Models
{
    public static class PushNotificationAndroid
    {
        public static void SendPushnotification(String json,string appname)
            //  public static void SendPushnotification(string deviceId, string Body, string Title, string GroupId, string MessageFor, string EventId)
        {
            try
            {

                string applicationID = "AIzaSyCkhMRs-1opjMmDNo7lHa_PGH9oavtNP3Y";
                //string applicationID = "AIzaSyAAb1a-A7mm7IJ62S32OTB9PXHrclAGQv8";
                // applicationID = "AAAA9AKvD9c:APA91bFxZMm1bPEyzU4ulOjAXhMymFdjDoEejSn-DAT2RiGGV7bFVr55WOZR6WnZetgpvjD0YUsSSne2u_TEGHjE_W9UUouVF4W4tRM7H8qCwzfGgsireN7ZXdWb-ezaMxyMUVyKJmin";
                string senderId = "1048017047511";
                if (appname == "eventmoon")
                {
                    applicationID = "AIzaSyD4DHAsrATGXObF_uUt3lNjOg2CqcRgqDU";
                    senderId = "849667759725";
                }

                    //  string deviceId = "ewzZ_768Gvc:APA91bFl3MALMvskc23mxnnlz4Ya98j36uUw1LchkhBr_5iL3H4kvRZYR-gzG5eg7jf_givDntEhE1g2wAcK0yn0WwBoeMpYcHCH9ZPlmCo3GqYF8oPUhP2y1LVdq0IsWO3oWblbJv8y";

                    WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                //var data = new
                //{
                //    to = deviceId,
                //    notification = new
                //    {
                //        body = Body,
                //        title = Title,
                //        sound = "Enabled",
                //        groupId = GroupId,
                //        messageFor = MessageFor,
                //        eventId = EventId

                //    }
                //};
                //var serializer = new JavaScriptSerializer();
              //  var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add("Authorization", "key=" + applicationID);
                //   tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                string str = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
        }
    }
}