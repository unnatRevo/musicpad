using MusicEventAPI.Manage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace MusicEventAPI.Models
{
    public class eventbrite
    {

        public void InsertIntoSql(string path = "")
        {
            string URL = "http://54.218.82.234:8080/dotnet/eventbrite.php";
            int currentpage = 1;
            string urlParameters = "?page=" + currentpage;


            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            eventbriteModel _mysqlfirst = null;


            HttpResponseMessage response = client.GetAsync(urlParameters).Result;
            if (response.IsSuccessStatusCode)
            {
                var responseData = response.Content.ReadAsStringAsync().Result;
                _mysqlfirst = JsonConvert.DeserializeObject<eventbriteModel>(responseData);
            }

            int totalpagecount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(_mysqlfirst.total_record) / _mysqlfirst.limit));
            AddEvents(_mysqlfirst.@event, path);

            currentpage = currentpage + 1;
            while (currentpage <= totalpagecount)
            {
                _mysqlfirst = null;
                urlParameters = "?page=" + currentpage;

                HttpClient clients = new HttpClient();
                clients.BaseAddress = new Uri(URL);
                clients.DefaultRequestHeaders.Accept.Clear();
                clients.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                HttpResponseMessage responses = clients.GetAsync(urlParameters).Result;
                if (responses.IsSuccessStatusCode)
                {
                    var responseData = responses.Content.ReadAsStringAsync().Result;
                    _mysqlfirst = JsonConvert.DeserializeObject<eventbriteModel>(responseData);
                }
                if(_mysqlfirst.@event != null)
                AddEvents(_mysqlfirst.@event, path);
                currentpage = currentpage + 1;
            }



        }

        public  void AddEvents(List<eventbriteTable> eventlst, string path)
        {
            DateTime todaydate = DateTime.Now;
            for (int i = 0; i < eventlst.Count(); i++)
            {

                try
                {
                    using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                    {
                        if ((!string.IsNullOrEmpty(eventlst[i].Day_date) && eventlst[i].Day_date != "Not Known") && (eventlst[i].Longitude != "Not Known") && (eventlst[i].Longitude != "Not Known"))
                        {
                            var arr = eventlst[i].Day_date.Split(',');
                            DateTime? date = null;
                            if (arr.Count() == 1)
                                date = DateTime.Parse(arr[0].Trim());
                            else if (arr.Count() == 2 && arr[1].Length <= 8 )
                                date = DateTime.Parse(arr[1].Trim() + " " + todaydate.Year);
                            else if (arr.Count() == 2 && arr[1].Length > 8)
                                date = DateTime.Parse(arr[1].Trim());
                            else
                                date = DateTime.Parse(arr[1].Trim() + " " + arr[2].Trim());

                            if (todaydate > date && todaydate.Month != date.Value.Month )
                                date = date.Value.AddYears(1);


                            if (eventlst[i].Time == "Not Known" || string.IsNullOrEmpty(eventlst[i].Time.Trim()))
                            {
                                eventlst[i].Time = "00:00 AM";
                            }
                            else
                            {
                                TimeSpan starttime = DateTime.Parse(eventlst[i].Time).TimeOfDay;
                                date = date.Value.Add(starttime);
                            }

                            //string filename = Guid.NewGuid().ToString() + ".Jpg";
                            SqlCommand com = new SqlCommand("AddEventsFromMySql", con);
                            com.CommandType = CommandType.StoredProcedure;
                            com.Parameters.AddWithValue("@EventName", eventlst[i].Event_Name);
                            com.Parameters.AddWithValue("@Latitude", eventlst[i].Latitude);
                            com.Parameters.AddWithValue("@Longitude", eventlst[i].Longitude);
                            com.Parameters.AddWithValue("@Location", eventlst[i].Address);
                            com.Parameters.AddWithValue("@Category", "Default");
                            com.Parameters.AddWithValue("@StartDate", date);
                            com.Parameters.AddWithValue("@StartTime", eventlst[i].Time);
                            //com.Parameters.AddWithValue("@EndDate", starttime);
                            //com.Parameters.AddWithValue("@Image", !string.IsNullOrEmpty(eventlst[i].image) ? "/Content/Image/Event/" + filename : eventlst[i].image);
                            con.Open();

                            SqlDataAdapter da = new SqlDataAdapter(com);
                            DataTable dt = new DataTable();
                            //com.ExecuteNonQuery();
                            da.Fill(dt);
                            con.Close();

                            //if (Convert.ToString(dt.Rows[0]["status"]) == "1" && !string.IsNullOrEmpty(eventlst[i].image))
                            //{
                            //    AddImage(eventlst[i].image, path, filename);
                            //}
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }




    }
    public class eventbriteTable
    {
        public string Source_Url { get; set; }
        public string Address { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Event_Name { get; set; }
        public string Genre { get; set; }
        public string Venue { get; set; }
        public string Day_date { get; set; }
        public string Time { get; set; }
        public string City { get; set; }
        public string State { get; set; }
    }

    public class eventbriteModel
    {
        public eventbriteModel()
        {
            @event = new List<eventbriteTable>();
        }
        public List<eventbriteTable> @event { get; set; }
        public string total_record { get; set; }
        public int limit { get; set; }
    }
}