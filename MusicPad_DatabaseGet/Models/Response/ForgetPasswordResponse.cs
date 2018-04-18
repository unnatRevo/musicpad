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
    public class ForgetPasswordResponse : BaseResponse
    {

        public override void Fill(BaseRequest request)
        {
            DataTable dt = new DataTable();
            string OTP = GenrateOtp.NewOTP();
            try
            {

                Security _securityobj = new Security();
                var req = (request as MusicEventAPI.Models.Request.ForgetPasswordRequest);

                //            string encrippassword = _securityobj.Encrypt(password);

                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("ForgetPassword", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@CountryCode", req.countryCode);
                    com.Parameters.AddWithValue("@Phoneno", req.phoneNo);
                    com.Parameters.AddWithValue("@otp", OTP);
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
                        SendSmsUser.sms(_user.countryCode + _user.phone, "Your OTP is :" + OTP);

                        Helper.FillResult(Result, ErrorCode.Success, OTP);
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