using MusicEventAPI.Manage;
using MusicEventAPI.Models.Request;
using MusicEventAPI.Utility;
using System;
using System.Data;
using System.Data.SqlClient;


namespace MusicEventAPI.Models.Response
{
    public class AutoLoginResponse : BaseResponse
    {

        public override void Fill(BaseRequest request)
        {
            DataTable dt = new DataTable();
            string guid = Guid.NewGuid().ToString();
            try
            {

                Security _securityobj = new Security();
                var req = (request as MusicEventAPI.Models.Request.AutoLoginRequest);

                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("AutoLogin", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@userId", req.userId);
                    if (!string.IsNullOrEmpty(req.mobileContact))
                        com.Parameters.AddWithValue("@mobileContact", req.mobileContact);
                    if (!string.IsNullOrEmpty(req.faceBookFriends))
                        com.Parameters.AddWithValue("@faceBookFriends", req.faceBookFriends);
                    if (!string.IsNullOrEmpty(req.deviceToken))
                        com.Parameters.AddWithValue("@deviceToken", req.deviceToken);
                    if (!string.IsNullOrEmpty(req.deviceType))
                        com.Parameters.AddWithValue("@deviceType", req.deviceType);
                    com.Parameters.AddWithValue("@guid", guid);
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
                Helper.FillResult(Result, ErrorCode.NoAuthentication, "");
                throw;
            }
        }



    }
}