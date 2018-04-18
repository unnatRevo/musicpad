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
    public class CreatePasswordResponse : BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            DataTable dt = new DataTable();
            try
            {
                var req = (request as MusicEventAPI.Models.Request.CreatePasswordRequest);

                Security _securityobj = new Security();
                string encrippassword = _securityobj.Encrypt(req.password);

                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("CreateNewPassword", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@userId", req.userId);
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
                        Helper.FillResult(Result, ErrorCode.Success, "");
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