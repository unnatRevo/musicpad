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
    public class RemoveUserFromGroupResponse : BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            DataTable dt = new DataTable();
            try
            {
                var req = (request as MusicEventAPI.Models.Request.RemoveUserFromGroupRequest);


                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("RemoveUserFromGroup", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@userId", req.userId);
                    com.Parameters.AddWithValue("@GroupId", req.groupId);
                    com.Parameters.AddWithValue("@removeUserId", req.removeUserId);
                    com.Parameters.AddWithValue("@accesstoken", req.accessToken);


                    SqlDataAdapter da = new SqlDataAdapter(com);

                    con.Open();
                    da.Fill(dt);
                    con.Close();


                    if (dt.Rows.Count > 0)
                    {
                        DataColumnCollection columns = dt.Columns;
                        if (columns.Contains("ErrorMessage"))
                        {
                            if (Convert.ToString(dt.Rows[0]["ErrorMessage"]) == "Logout")
                                Helper.FillResult(Result, ErrorCode.Logout, "");
                            return;
                        }
                    }

                    if (Convert.ToString(dt.Rows[0]["SatusMessage"]) == "success")
                    {
                        Helper.FillResult(Result, ErrorCode.Success, "");
                    }
                   else if (Convert.ToString(dt.Rows[0]["SatusMessage"]) == "Invalid Admin")
                    {
                        Helper.FillResult(Result, ErrorCode.invalidAdmin, "");
                    }
                    else
                    {
                        Helper.FillResult(Result, ErrorCode.NouserWithThisGroup, "");
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