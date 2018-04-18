using MusicEventAPI.Manage;
using PushSharp;
using PushSharp.Android;
using PushSharp.Apple;
using PushSharp.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace MusicEventAPI.Models
{
    public static class EveryHourEventPushNotification
    {

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
                            DeviceType = Convert.ToString(dr["DeviceType"])
                        }
                        );
                }

                return _model;
            }
        }

        public static void sendNotification()
        {
            int port = 2195;
            String hostname = "gateway.sandbox.push.apple.com";

            //load certificate
            string certificatePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Resources/Certificates.p12");
            string certificatePassword = "";

            X509Certificate2 clientCertificate = new X509Certificate2(certificatePath, certificatePassword);
            X509Certificate2Collection certificatesCollection = new X509Certificate2Collection(clientCertificate);

            TcpClient client = new TcpClient(hostname, port);
            SslStream sslStream = new SslStream(
                                    client.GetStream(),
                                    false,
                                    null,
                                    null
                    );

            try
            {
                sslStream.AuthenticateAsClient(hostname, certificatesCollection, System.Security.Authentication.SslProtocols.Tls, false);
            }
            catch (AuthenticationException ex)
            {
                client.Close();
            }

            // Encode a test message into a byte array.
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(memoryStream);

            writer.Write((byte)0);  //The command
            writer.Write((byte)0);  //The first byte of the deviceId length (big-endian first byte)
            writer.Write((byte)32); //The deviceId length (big-endian second byte)

            String deviceId = "79C84904BB6A82DE7C05D563A953094CC1DFEBC3EC49E6E5D55647337C06E3ED";
            writer.Write((deviceId.ToUpper().ToArray()));

            String payload = "{\"aps\":{\"alert\":\"Test\",\"badge\":14}}";

            writer.Write((byte)0); //First byte of payload length; (big-endian first byte)
            writer.Write((byte)payload.Length);     //payload length (big-endian second byte)

            byte[] b1 = System.Text.Encoding.UTF8.GetBytes(payload);
            writer.Write(b1);
            writer.Flush();

            byte[] array = memoryStream.ToArray();
            sslStream.Write(array);
            sslStream.Flush();

            // Close the client connection.
            client.Close();
        }

        public static void sendNotificationeea()
        {

            int port = 2195;
            String hostname = "gateway.push.apple.com";
            String certificatePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Resources/Certificates.p12");
            X509Certificate2 clientCertificate = new X509Certificate2(System.IO.File.ReadAllBytes(certificatePath), "");
            X509Certificate2Collection certificatesCollection = new X509Certificate2Collection(clientCertificate);

            TcpClient client = new TcpClient(hostname, port);
            SslStream sslStream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);

            try
            {
                sslStream.AuthenticateAsClient(hostname, certificatesCollection, SslProtocols.Tls, false);
                MemoryStream memoryStream = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(memoryStream);
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((byte)32);

                writer.Write(StringToByteArray("79C84904BB6A82DE7C05D563A953094CC1DFEBC3EC49E6E5D55647337C06E3ED"));
                String payload = "{\"aps\":{\"alert\":\"" + "Hi,, This Is a Sample Push Notification For IPhone.." + "\",\"badge\":1,\"sound\":\"default\"}}";
                writer.Write((byte)0);
                writer.Write((byte)payload.Length);
                byte[] b1 = System.Text.Encoding.UTF8.GetBytes(payload);
                writer.Write(b1);
                writer.Flush();
                byte[] array = memoryStream.ToArray();
                sslStream.Write(array);
                sslStream.Flush();
                client.Close();
            }
            catch (System.Security.Authentication.AuthenticationException ex)
            {
                client.Close();
            }
            catch (Exception e)
            {
                client.Close();
            }
        }
        public static bool ValidateServerCertificate(
      object sender,
      X509Certificate certificate,
      X509Chain chain,
      SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }

        public static byte[] StringToByteArray(string hex)
        {
            return System.Linq.Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        public static void sendNotificationaa()
        {
            using (var push = new PushBroker())
            {

                push.OnNotificationSent += NotificationSent;
                push.OnChannelException += ChannelException;
                push.OnServiceException += ServiceException;
                push.OnNotificationFailed += NotificationFailed;
                push.OnDeviceSubscriptionExpired += DeviceSubscriptionExpired;
                push.OnDeviceSubscriptionChanged += DeviceSubscriptionChanged;
                push.OnChannelCreated += ChannelCreated;
                push.OnChannelDestroyed += ChannelDestroyed;
                List<EvntPushNotification> rows = GetEventList();

                rows.Add(
                        new EvntPushNotification
                        {
                            eventId = Convert.ToString("1"),
                            eventName = Convert.ToString("Music Event "),
                            DeviceToken = Convert.ToString("79C84904BB6A82DE7C05D563A953094CC1DFEBC3EC49E6E5D55647337C06E3ED"),
                            DeviceType = Convert.ToString("ios")
                        }
                        );

                foreach (EvntPushNotification row in rows)
                {

                    if (row.DeviceType == "ios")
                    {
                        //-------------------------
                        // APPLE NOTIFICATIONS
                        //-------------------------
                        //Configure and start Apple APNSD:\Bhavik\MusicEventAPI\MusicEventAPI\Resources\Certificates.p12
                        // IMPORTANT: Make sure you use the right Push certificate.  Apple allows you to
                        //generate one for connecting to Sandbox, and one for connecting to Production.  You must
                        // use the right one, to match the provisioning profile you build your
                        //   app with!
                        try
                        {

                            var path = System.Web.Hosting.HostingEnvironment.MapPath("~/Resources/apn_developer_identity.p12");
                            var appleCert = File.ReadAllBytes(path);
                            //IMPORTANT: If you are using a Development provisioning Profile, you must use
                            // the Sandbox push notification server 
                            //  (so you would leave the first arg in the ctor of ApplePushChannelSettings as
                            // 'false')
                            //  If you are using an AdHoc or AppStore provisioning profile, you must use the 
                            //Production push notification server
                            //  (so you would change the first arg in the ctor of ApplePushChannelSettings to 
                            //'true')
                            push.RegisterAppleService(new PushSharp.Apple.ApplePushChannelSettings(false, appleCert,"123456",true));
                            //Extension method
                            //Fluent construction of an iOS notification
                            //IMPORTANT: For iOS you MUST MUST MUST use your own DeviceToken here that gets
                            // generated within your iOS app itself when the Application Delegate
                            //  for registered for remote notifications is called, 
                            // and the device token is passed back to you
                            push.QueueNotification(new AppleNotification()
                                                        .ForDeviceToken(row.DeviceToken)//the recipient device id
                                                        .WithAlert(row.eventName + "Start in one hour")//the message
                                                        .WithBadge(7)
                                                        .WithSound("sound.caf")
                                                        );

                            push.StopAllServices(waitForQueuesToFinish: true);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    else if (row.DeviceType == "android")
                    {
                        //---------------------------
                        // ANDROID GCM NOTIFICATIONS
                        //---------------------------
                        //Configure and start Android GCM
                        //IMPORTANT: The API KEY comes from your Google APIs Console App, 
                        //under the API Access section, 
                        //  by choosing 'Create new Server key...'
                        //  You must ensure the 'Google Cloud Messaging for Android' service is 
                        //enabled in your APIs Console
                        push.RegisterGcmService(new
                         GcmPushChannelSettings("YOUR Google API's Console API Access  API KEY for Server Apps HERE"));
                        //Fluent construction of an Android GCM Notification
                        //IMPORTANT: For Android you MUST use your own RegistrationId 
                        //here that gets generated within your Android app itself!
                        push.QueueNotification(new GcmNotification().ForDeviceRegistrationId(
                        row.DeviceToken)
                         .WithJson("{\"alert\":\"" + row.eventName + "Start in one hour\", \"badge\":" + Convert.ToInt32(row.eventId) + ",\"sound\":\"sound.caf\"}"));
                    }

                }
            }

        }

        public static void StartCheckingLog()
        {

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000;
            timer.Elapsed += timer_Elapsed;
            timer.Start();
            //var timer = new System.Threading.Timer((e) =>
            //{
            //    sendNotification();
            //}, null, 0,1);
        }

        static void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            sendNotification();
        }

        static void DeviceSubscriptionChanged(object sender,
string oldSubscriptionId, string newSubscriptionId, INotification notification)
        {
            Console.WriteLine(notification);
        }

        //this even raised when a notification is successfully sent
        static void NotificationSent(object sender, INotification notification)
        {
            Console.WriteLine(notification);
        }
        //this is raised when a notification is failed due to some reason
        static void NotificationFailed(object sender,
        INotification notification, Exception notificationFailureException)
        {
            Console.WriteLine(notificationFailureException);
        }
        //this is fired when there is exception is raised by the channel
        static void ChannelException
        (object sender, IPushChannel channel, Exception exception)
        {
            Console.WriteLine(exception);
        }
        //this is fired when there is exception is raised by the service
        static void ServiceException(object sender, Exception exception)
        {
            Console.WriteLine(exception);
        }
        //this is raised when the particular device subscription is expired
        static void DeviceSubscriptionExpired(object sender,
        string expiredDeviceSubscriptionId,
        DateTime timestamp, INotification notification)
        {
            Console.WriteLine(notification);
        }
        //this is raised when the channel is destroyed
        static void ChannelDestroyed(object sender)
        {
            Console.WriteLine(sender);
        }
        //this is raised when the channel is created
        static void ChannelCreated(object sender, IPushChannel pushChannel)
        {
            Console.WriteLine(pushChannel);
        }
    }
}