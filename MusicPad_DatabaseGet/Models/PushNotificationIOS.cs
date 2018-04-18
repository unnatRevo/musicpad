using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace MusicEventAPI.Models
{
    public static class PushNotificationIOS
    {
        public static void sendMsg(string devicetocken,string datastring,string appname)
        {
           // devicetocken = "28DF3D1276CE7E7262EF1872CCF114C8A51BBFDEB5E36015FD0C4D65DADC806F";//  iphone device token
            int port = 2195;
          //  String hostname = "gateway.sandbox.push.apple.com";
            String hostname = "gateway.push.apple.com";

            string certificatePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Resources/MusicPad.p12");
            if(appname == "eventmoon")
                certificatePath = System.Web.Hosting.HostingEnvironment.MapPath("~/Resources/EventMoon.p12");

            string certificatePassword = "";

            X509Certificate2 clientCertificate = new X509Certificate2(certificatePath, certificatePassword, X509KeyStorageFlags.MachineKeySet);
            X509Certificate2Collection certificatesCollection = new X509Certificate2Collection(clientCertificate);

            TcpClient client = new TcpClient(hostname, port);
            SslStream sslStream = new SslStream(
                            client.GetStream(),
                            false,
                            new RemoteCertificateValidationCallback(ValidateServerCertificate),
                            null
            );

            try
            {
                sslStream.AuthenticateAsClient(hostname, certificatesCollection, SslProtocols.Default, false);
            }
            catch (AuthenticationException ex)
            {
                Console.WriteLine("Authentication failed");
                client.Close();
                //  Request.SaveAs(Server.MapPath("Authenticationfailed.txt"), true);
                return;
            }


            //// Encode a test message into a byte array.
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(memoryStream);

            writer.Write((byte)0);  //The command
            writer.Write((byte)0);  //The first byte of the deviceId length (big-endian first byte)
            writer.Write((byte)32); //The deviceId length (big-endian second byte)

            byte[] b0 = HexString2Bytes(devicetocken);
            WriteMultiLineByteArray(b0);

            writer.Write(b0);
          //  String payload;
            //string strmsgbody = "";
            //int totunreadmsg = 20;
            //strmsgbody = "Hey Tester!";

            //Debug.WriteLine("during testing via device!");
            ////   Request.SaveAs(Server.MapPath("APNSduringdevice.txt"), true);

            //payload = "{\"aps\":{\"alert\":\"" + strmsgbody + "\",\"badge\":" + totunreadmsg.ToString() + ",\"sound\":\"mailsent.wav\"},\"acme1\":\"bar\",\"acme2\":42}";

            writer.Write((byte)0); //First byte of payload length; (big-endian first byte)
            writer.Write((byte)datastring.Length);     //payload length (big-endian second byte)

            byte[] b1 = System.Text.Encoding.UTF8.GetBytes(datastring);
            writer.Write(b1);
            writer.Flush();

            byte[] array = memoryStream.ToArray();
          //  Debug.WriteLine("This is being sent...\n\n");
            Debug.WriteLine(array);
            try
            {
                sslStream.Write(array);

                sslStream.Flush();
            }
            catch
            {
                Debug.WriteLine("Write failed buddy!!");
                //  Request.SaveAs(Server.MapPath("Writefailed.txt"), true);
            }

            client.Close();
           // Debug.WriteLine("Client closed.");
            //  Request.SaveAs(Server.MapPath("APNSSuccess.txt"), true);
        }
        private static byte[] HexString2Bytes(string hexString)
        {
            //check for null
            if (hexString == null) return null;
            //get length
            int len = hexString.Length;
            if (len % 2 == 1) return null;
            int len_half = len / 2;
            //create a byte array
            byte[] bs = new byte[len_half];
            try
            {
                //convert the hexstring to bytes
                for (int i = 0; i != len_half; i++)
                {
                    bs[i] = (byte)Int32.Parse(hexString.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Exception : " + ex.Message);
            }
            //return the byte array
            return bs;
        }
        // The following method is invoked by the RemoteCertificateValidationDelegate.
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
        public static void WriteMultiLineByteArray(byte[] bytes)
        {
            const int rowSize = 20;
            int iter;

            Console.WriteLine("initial byte array");
            Console.WriteLine("------------------");

            for (iter = 0; iter < bytes.Length - rowSize; iter += rowSize)
            {
                Console.Write(
                    BitConverter.ToString(bytes, iter, rowSize));
                Console.WriteLine("-");
            }

            Console.WriteLine(BitConverter.ToString(bytes, iter));
            Console.WriteLine();
        }
    }
}