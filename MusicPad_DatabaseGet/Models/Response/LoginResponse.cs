
using MusicEventAPI.Manage;
using MusicEventAPI.Models.Request;
using MusicEventAPI.Utility;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web;
using Twilio;


namespace MusicEventAPI.Models.Response
{
    public class LoginResponse : BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            DataTable dt = new DataTable();
            string guid = Guid.NewGuid().ToString();
            try
            {

                Security _securityobj = new Security();
                var req = (request as MusicEventAPI.Models.Request.LoginRequest);
              //  var cmdInfo = HHADataAccess.GetCommandDetails("UserLogin");

                string password = _securityobj.Encrypt(req.password);

                //if (cmdInfo == null)
                //{
                //    throw new ArgumentException("Provider base mapping details or Connection String not found for 'GetLanguages'.");
                //}
                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("Login", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@CountryCode", req.countryCode);
                    com.Parameters.AddWithValue("@username", req.username);
                    com.Parameters.AddWithValue("@Password", password);
                    com.Parameters.AddWithValue("@DeviceToken", req.deviceToken);
                    com.Parameters.AddWithValue("@DeviceType", req.deviceType);
                    com.Parameters.AddWithValue("@guid", guid);
                    if(!string.IsNullOrEmpty(req.mobileContact))
                    com.Parameters.AddWithValue("@mobileContact", req.mobileContact);
                    com.Parameters.AddWithValue("@appname", req.appname);


                    SqlDataAdapter da = new SqlDataAdapter(com);

                    con.Open();
                    da.Fill(dt);
                    con.Close();

                    if (dt.Rows.Count == 0)
                    {
                        Helper.FillResult(Result, ErrorCode.invalidCredential, "");
                    }
                    else
                    {
                        User _user = convertdatatableToObject.DatatabletoUser(dt);
                        Helper.FillResult(Result, ErrorCode.Success, _user);
                    }
                }

                    
            }
            catch (Exception ex)
            {
                Helper.FillResult(Result, ErrorCode. NoAuthentication, "");
                throw;
            }
        }
        //static async Task SendSms()
        //{
        //    // Your Account SID from twilio.com/console
        //    var accountSid = "AC8c161e579684ad28f1bc03221748a4d8";
        //    // Your Auth Token from twilio.com/console
        //    var authToken = "b86ee3b3577a276953c35c5d5352f0d4";

        //    TwilioClient.Init(accountSid, authToken);

        //    var message = await MessageResource.CreateAsync(
        //        to: new PhoneNumber("+919376711466"),
        //        from: new PhoneNumber("+16572163416"),
        //        body: "Hello from C#");

        //    Console.WriteLine(message.Sid);
        //}


    }
}