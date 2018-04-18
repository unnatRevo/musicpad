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
    public class ValidateOTPForgotPasswordResponse : BaseResponse
    {
        public override void Fill(BaseRequest request)
        {

            DataTable dt = new DataTable();
            try
            {
                var req = (request as MusicEventAPI.Models.Request.ValidateOTPRequest);

                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("VerifyOTPForgotPassword", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@OTP", req.code);


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
                        else
                        {
                            User _user = convertdatatableToObject.DatatabletoUser(dt);
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