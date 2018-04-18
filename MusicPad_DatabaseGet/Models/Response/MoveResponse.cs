using MusicEventAPI.Manage;
using MusicEventAPI.Models.Request;
using MusicEventAPI.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
namespace MusicEventAPI.Models.Response
{
    public class MoveResponse : BaseResponse
    {

        public override void Fill(BaseRequest request)
        {
            
            DataTable dt = new DataTable();
           
            string guid = Guid.NewGuid().ToString();
            try
            {
                
                Security _securityobj = new Security();
                var req = (request as MusicEventAPI.Models.Request.MoveRequest);

                using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                {
                    SqlCommand com = new SqlCommand("MoveData", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@maxid", req.maxid);
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
                        List<MyEvent> my = new List<MyEvent>();
                        foreach (DataRow item in dt.Rows)
                        {
                            my.Add(new MyEvent
                            {
                                EventID = Convert.ToInt32(item["EventID"]),
                                CategoryName = Convert.ToString(item["CategoryName"]),
                                EventName = Convert.ToString(item["EventName"]),
                                EventDiscription = Convert.ToString(item["EventDiscription"]),
                                StartDate = Convert.ToDateTime(item["StartDate"]),
                                EndDate = Convert.ToDateTime(item["EndDate"]),
                                Starttime = Convert.ToString(item["Starttime"]),
                                Endtime = Convert.ToString(item["Endtime"]),
                                Dealtext = Convert.ToString(item["Dealtext"]),
                                Percentage = Convert.ToString(item["Percentage"]),
                                latitude = Convert.ToString(item["latitude"]),
                                longitude = Convert.ToString(item["longitude"]),
                                location = Convert.ToString(item["location"]),
                                Image = Convert.ToString(item["Image"]),
                                TicketUrl = Convert.ToString(item["TicketUrl"]),
                                IsTmaster = Convert.ToBoolean(item["IsTmaster"]),
                                TotalCount = Convert.ToInt32(item["TotalCount"]),

                            });
                        }
                        Helper.FillResult(Result, ErrorCode.Success, my);

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