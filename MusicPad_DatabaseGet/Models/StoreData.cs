using MusicEventAPI.Manage;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace MusicEventAPI.Models
{
    public class StoreData
    {
        string[] keys = { "AIzaSyAxbXpabERiZmFfjtTLOh77xQimuZM_imQ" };
        public int GetMaxid()
        {
            using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
            {
                DataTable dt = new DataTable();
                SqlCommand com = new SqlCommand("GetMaxCount", con);
                com.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(com);

                con.Open();
                da.Fill(dt);
                con.Close();

                if (dt.Rows.Count == 0)
                {
                    return 0;
                }
                else
                {
                    return Convert.ToInt32(dt.Rows[0]["ID"]);

                }
            }
        }

        public  void GetMysql()
        {
            try
            {
                string str = "server=scrapingdb.cb6jakiryv3m.us-east-2.rds.amazonaws.com;database=scraping;port=3306;username=derrick;password=derrick123321";
                //Display query  
                string Query = "select * from _events;";
                MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString);
                MySqlCommand cmd = new MySqlCommand(Query, con);
                con.Open();
                //For offline connection we weill use  MySqlDataAdapter class.  
                MySqlDataAdapter Adapter = new MySqlDataAdapter();
                Adapter.SelectCommand = cmd;
                DataTable dt = new DataTable();
                Adapter.Fill(dt);
                // here i have assign dTable object to the dataGridView1 object to display data.               
                // MyConn2.Close();  
            }
            catch (Exception ex)
            {
            }
        }


        public void MoveData()
        {
            RootObject ro = new RootObject();
            int totalpagecount = 0, maxid = GetMaxid();
            int currentpage = 1;
            try
            {
                RestClient cc = new RestClient("http://54.218.82.234/api/Move");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", "{\"maxid\":\""+ maxid + "\"}", ParameterType.RequestBody);
                IRestResponse responsess = cc.Execute(request);

                if (Convert.ToString(responsess.StatusCode) == "OK")
                {
                    var responseData = responsess.Content;
                    ro = JsonConvert.DeserializeObject<RootObject>(responseData);
                }
                if (ro != null & ro.Results!= null && ro.Results.Count > 0)
                {
                    AddEvents(ro.Results);
                    totalpagecount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(ro.Results[0].TotalCount) / 100));
                }
                currentpage = currentpage + 1;
                while (currentpage <= totalpagecount)
                {
                    ro = null;
                    maxid = GetMaxid();
                    RestClient client = new RestClient("http://54.218.82.234/api/Move");
                    var requests = new RestRequest(Method.POST);
                    //client.BaseAddress = new Uri("http://localhost:52318/api/Move/");
                    //client.BaseAddress = new Uri("http://54.218.82.234/api/Move/");
                    //client.DefaultRequestHeaders.Accept.Clear();
                    //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                    //HttpResponseMessage response = client.GetAsync("maxid=" + maxid).Result;
                    requests.AddHeader("content-type", "application/json");
                    requests.AddParameter("application/json", "{\"maxid\":\"" + maxid + "\"}", ParameterType.RequestBody);
                    IRestResponse respo = client.Execute(requests);

                    if (Convert.ToString(respo.StatusCode) == "OK")
                    {
                        var responseDatas = respo.Content;
                        ro = JsonConvert.DeserializeObject<RootObject>(responseDatas);
                    }
                    if (ro != null & ro.Results != null && ro.Results.Count > 0)
                    {
                        AddEvents(ro.Results);
                    }
                }
            }
            catch
            {

            }
        }
        public class Result
        {
            public int EventID { get; set; }
            public int UserID { get; set; }
            public string CategoryName { get; set; }
            public string EventName { get; set; }
            public string EventDiscription { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string Starttime { get; set; }
            public string Endtime { get; set; }
            public string Dealtext { get; set; }
            public string Percentage { get; set; }
            public string latitude { get; set; }
            public string longitude { get; set; }
            public string location { get; set; }
            public string Image { get; set; }
            public string TicketUrl { get; set; }
            public bool IsTmaster { get; set; }
            public int TotalCount { get; set; }
        }

        public class Group
        {
        }

        public class RootObject
        {
            public bool Status { get; set; }
            public List<Result> Results { get; set; }
            public Group Group { get; set; }
            public string ErrorMessage { get; set; }
            public int totalPageCount { get; set; }
        }

        public void AddEvents(List<Result> eventlst)
        {
            for (int i = 0; i < eventlst.Count(); i++)
            {

                try
                {
                    using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                    {



                        if ((string.IsNullOrEmpty(eventlst[i].latitude) || string.IsNullOrEmpty(eventlst[i].longitude)) && (!string.IsNullOrEmpty(eventlst[i].location)))
                        {
                            Adress adrs = new Adress();
                            adrs.Key = keys[0];
                            adrs.Address = eventlst[i].location;
                            adrs.GeoCode();
                            eventlst[i].latitude = adrs.Latitude;
                            eventlst[i].longitude = adrs.Longitude;
                        }

                        if ((!string.IsNullOrEmpty(eventlst[i].latitude) && !string.IsNullOrEmpty(eventlst[i].longitude)) && (string.IsNullOrEmpty(eventlst[i].location)))
                        {
                            GetAddress address = new GetAddress();
                            eventlst[i].location = address.RetrieveFormatedAddress(eventlst[i].latitude, eventlst[i].longitude, keys[0]);
                        }


                        SqlCommand com = new SqlCommand("AddEventsFromSql", con);
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@EventName", eventlst[i].EventName);
                        com.Parameters.AddWithValue("@Latitude", eventlst[i].latitude);
                        com.Parameters.AddWithValue("@Longitude", eventlst[i].longitude);
                        com.Parameters.AddWithValue("@Location", eventlst[i].location);
                        com.Parameters.AddWithValue("@Category", eventlst[i].CategoryName);
                        com.Parameters.AddWithValue("@StartDate", eventlst[i].StartDate);
                        com.Parameters.AddWithValue("@StartTime", eventlst[i].Starttime);
                        com.Parameters.AddWithValue("@enddate", eventlst[i].EndDate);
                        com.Parameters.AddWithValue("@endtime", eventlst[i].Endtime);
                        com.Parameters.AddWithValue("@isTmaster", eventlst[i].IsTmaster);
                        if (!string.IsNullOrEmpty(eventlst[i].TicketUrl))
                            com.Parameters.AddWithValue("@ticketUrl", eventlst[i].TicketUrl);
                        com.Parameters.AddWithValue("@masterEventID", eventlst[i].EventID);
                        if (!string.IsNullOrEmpty(eventlst[i].Image))
                        {
                            com.Parameters.AddWithValue("@Image", eventlst[i].Image);
                        }
                        con.Open();

                        SqlDataAdapter da = new SqlDataAdapter(com);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        con.Close();
                    }
                }
                catch (Exception ex)
                {
                    //ErrorLog.AddLog(1, ex.Message, 0);
                }
            }
        }

    }
}