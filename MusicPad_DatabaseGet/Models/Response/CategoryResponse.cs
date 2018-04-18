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
    public class CategoryResponse :BaseResponse
    {
        public override void Fill(BaseRequest request)
        {
            DataTable dt = new DataTable();
            List<Category> _catlist = new List<Category>();
            try
            {
                var req = (request as MusicEventAPI.Models.Request.CategoryRequest);
  
              
                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("GetCategoryList", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@UserID", req.userId);
                    if (!string.IsNullOrEmpty(req.accessToken))
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
                            if(Convert.ToString(dt.Rows[0]["ErrorMessage"]) == "Logout")
                            Helper.FillResult(Result, ErrorCode.Logout,"");
                            return;
                        }
                    }

                    foreach (DataRow dr in dt.Rows)
                    {
                        _catlist.Add(
                            new Category
                            {
                                Categoryid = Convert.ToInt32(dr["MainCatID"]),
                                CategoryName = Convert.ToString(dr["MainCategoryName"])
                            }
                            );
                    }   
                    }
                Helper.FillResult(Result, ErrorCode.Success, _catlist);
            }

            catch (Exception ex)
            {
                Helper.FillResult(Result, ErrorCode.NoAuthentication, "");
                throw;
            }
        }
    }
}