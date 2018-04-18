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
    public class GroupJoinUserResponse : BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            DataTable dt = new DataTable();
            List<JoinGroupUserList> _userlist = new List<JoinGroupUserList>();
            try
            {
                var req = (request as MusicEventAPI.Models.Request.GroupJoinUserRequest);


                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("GroupJoinUserList", con);
                    com.CommandType = CommandType.StoredProcedure;;
                    com.Parameters.AddWithValue("@groupid", req.groupId);

                    SqlDataAdapter da = new SqlDataAdapter(com);

                    con.Open();
                    da.Fill(dt);
                    con.Close();

                    foreach (DataRow dr in dt.Rows)
                    {
                        _userlist.Add(
                            new JoinGroupUserList
                            {
                                groupId = Convert.ToInt32(dr["GroupId"]),
                                userId = Convert.ToInt32(dr["UserID"]),
                                userName = Convert.ToString(dr["UserName"]),
                                //imagePath = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/") +( string.IsNullOrEmpty(Convert.ToString(dr["UserImagePath"])) ? "/content/Image/NoImage.png" :  Convert.ToString(dr["UserImagePath"])),
                                imagePath = ImagePath.GetimagePath(Convert.ToString(dr["UserImagePath"]), HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, "/")),
                                ISApprove = Convert.ToBoolean(dr["ISApprove"]),
                                isAdmin = Convert.ToBoolean(dr["isAdmin"])
                            }
                            );
                    }
                }
                Helper.FillResult(Result, ErrorCode.Success, _userlist);
            }

            catch (Exception ex)
            {
                Helper.FillResult(Result, ErrorCode.NoAuthentication, "");
                throw;
            }
        }
    }
}