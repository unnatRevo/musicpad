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
    public class RegisterWithPhoneResponse :BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            DataTable dt = new DataTable();
            string guid = Guid.NewGuid().ToString();
            string OTP = GenrateOtp.NewOTP();
            try
            {
                Security _securityobj = new Security();
                var req = (request as MusicEventAPI.Models.Request.RegisterWithPhoneRequest);

                string password = "";
                if (!string.IsNullOrEmpty(req.password))
                    password =  _securityobj.Encrypt(req.password);
                //  var cmdInfo = HHADataAccess.GetCommandDetails("UserLogin");

                //if (cmdInfo == null)
                //{
                //    throw new ArgumentException("Provider base mapping details or Connection String not found for 'GetLanguages'.");
                //}
                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("RegisterWithPhoneRequest", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@CountryCode", req.countryCode);
                    com.Parameters.AddWithValue("@Phoneno", req.phoneNumber);
                    if (!string.IsNullOrEmpty(req.password))
                        com.Parameters.AddWithValue("@password", password);
                    if(!string.IsNullOrEmpty(req.deviceToken))
                    com.Parameters.AddWithValue("@DeviceToken", req.deviceToken);
                    com.Parameters.AddWithValue("@DeviceType", req.deviceType);
                    com.Parameters.AddWithValue("@guid", guid);
                    com.Parameters.AddWithValue("@otp", OTP);
                    if (!string.IsNullOrEmpty(req.mobileContact))
                        com.Parameters.AddWithValue("@mobileContact", req.mobileContact);
                    SqlDataAdapter da = new SqlDataAdapter(com);

                    con.Open();
                    da.Fill(dt);
                    con.Close();

                    if (dt.Rows.Count == 0)
                    {
                        Helper.FillResult(Result, ErrorCode.NoAuthentication, "");
                    }
                    else
                    {
                        if (Convert.ToInt32(dt.Rows[0]["UserID"]) == 0)
                        {
                            Helper.FillResult(Result, ErrorCode.PhoneNoUsed, "");
                        }
                        else
                        {
                            SendSmsUser.sms(req.countryCode + req.phoneNumber, "Your otp is : " + OTP);
                            Helper.FillResult(Result, ErrorCode.Success, OTP);
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
    }
}