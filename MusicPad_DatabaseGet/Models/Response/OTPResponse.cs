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
    public class OTPResponse : BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            DataTable dt = new DataTable();
            string OTP = GenrateOtp.NewOTP();
            try
            {
                var req = (request as MusicEventAPI.Models.Request.OTPRequest);
                //  var cmdInfo = HHADataAccess.GetCommandDetails("UserLogin");

                //if (cmdInfo == null)
                //{
                //    throw new ArgumentException("Provider base mapping details or Connection String not found for 'GetLanguages'.");
                //}
                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("GenrateOTP", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@CountryCode", req.countryCode);
                    com.Parameters.AddWithValue("@Phoneno", req.phoneNumber);
                    com.Parameters.AddWithValue("@otp", OTP);


                    SqlDataAdapter da = new SqlDataAdapter(com);

                    con.Open();
                    da.Fill(dt);
                    con.Close();

                    if (dt.Rows.Count == 0)
                    {
                        Helper.FillResult(Result, ErrorCode.InvalidMobile, "");
                    }
                    else
                    {
                        if (Convert.ToInt32(dt.Rows[0]["UserID"]) > 0)
                        {
                            SendSmsUser.sms(req.countryCode + req.phoneNumber, "Your Otp is : " + OTP);
                            Helper.FillResult(Result, ErrorCode.Success, "Otp Send Successfully");
                        }
                        else
                        {
                            Helper.FillResult(Result, ErrorCode.InvalidMobile, null);
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