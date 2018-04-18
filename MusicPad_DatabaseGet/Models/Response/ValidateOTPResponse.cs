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
    public class ValidateOTPResponse: BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            string password = GenratePassword.NewPassword().ToString();
            DataTable dt = new DataTable();
            try
            {
                var req = (request as MusicEventAPI.Models.Request.ValidateOTPRequest);

                Security _securityobj = new Security();
                string encrippassword = _securityobj.Encrypt(password);
                //  var cmdInfo = HHADataAccess.GetCommandDetails("UserLogin");

                //if (cmdInfo == null)
                //{
                //    throw new ArgumentException("Provider base mapping details or Connection String not found for 'GetLanguages'.");
                //}
                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("VerifyOTP", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@OTP", req.code);
                    com.Parameters.AddWithValue("@password", encrippassword);


                    SqlDataAdapter da = new SqlDataAdapter(com);

                    con.Open();
                    da.Fill(dt);
                    con.Close();

                    if (dt.Rows[0] == null)
                    {
                        Helper.FillResult(Result, ErrorCode.NoAuthentication, "");
                    }
                    else
                    {
                        if (Convert.ToString(dt.Rows[0]["ValidateMessage"]) == "In Valid OTP")
                        {
                            Helper.FillResult(Result, ErrorCode.invalidOTP, null);
                        }
                        else if (Convert.ToString(dt.Rows[0]["ValidateMessage"]) == "OTP Expire")
                        {
                            Helper.FillResult(Result, ErrorCode.OTPExpired, null);
                        }
                        else {
                            User _user = convertdatatableToObject.DatatabletoUser(dt);
                            _user.password = password;
                            Helper.FillResult(Result, ErrorCode.Success, _user);
                        }
                        
                    }
                }


            }
            catch (Exception ex)
            {
                Helper.FillResult(Result, ErrorCode.NoAuthentication, null);
                throw;
            }
        }
    }
}