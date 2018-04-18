using MusicEventAPI.Manage;
using MusicEventAPI.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace MusicEventAPI.Models
{
    public class visitnj
    {
        string[] keys = { "AIzaSyAxbXpabERiZmFfjtTLOh77xQimuZM_imQ" };

        public void InsertIntoSql(string path = "")
        {
            string URL = "http://54.218.82.234:8080/dotnet/visitnj.php";
            int currentpage = 1;
            string urlParameters = "?page=" + currentpage;


            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            EventModel _TmasterModel = null;


            HttpResponseMessage response = client.GetAsync(urlParameters).Result;
            if (response.IsSuccessStatusCode)
            {
                var responseData = response.Content.ReadAsStringAsync().Result;
                _TmasterModel = JsonConvert.DeserializeObject<EventModel>(responseData);
            }
            int totalpagecount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(_TmasterModel.total_record) / _TmasterModel.limit));
            AddEvents(_TmasterModel.@event, path);

            currentpage = currentpage + 1;
            while (currentpage <= totalpagecount)
            {
                _TmasterModel = null;
                urlParameters = "?page=" + currentpage;

                HttpClient clients = new HttpClient();
                clients.BaseAddress = new Uri(URL);
                clients.DefaultRequestHeaders.Accept.Clear();
                clients.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                HttpResponseMessage responses = clients.GetAsync(urlParameters).Result;
                if (responses.IsSuccessStatusCode)
                {
                    var responseData = responses.Content.ReadAsStringAsync().Result;
                    _TmasterModel = JsonConvert.DeserializeObject<EventModel>(responseData);
                }
                if (_TmasterModel != null)
                    AddEvents(_TmasterModel.@event, path);
                else
                    ErrorLog.AddLog(1, Convert.ToString(currentpage), 0);
                currentpage = currentpage + 1;
            }



        }

        public void AddEvents(List<Event> eventlst, string path)
        {
            for (int i = 0; i < eventlst.Count(); i++)
            {

                try
                {
                    using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                    {
                        if (!string.IsNullOrEmpty(eventlst[i].event_date))
                        {
                            var arr = eventlst[i].event_date;
                            DateTime date = DateTime.Parse(eventlst[i].event_date.Replace("th,", " ").Replace("st,", " ").Replace("nd,", " ").Replace("rd,", " "));
                            //TimeSpan starttime = DateTime.Parse("00:00 AM").TimeOfDay; 

                            if (string.IsNullOrEmpty(eventlst[i].event_time))
                            {
                                eventlst[i].event_time = "00:00 AM";
                            }
                            else
                            {
                                eventlst[i].event_time = eventlst[i].event_time.Substring(0, 8).Trim();
                            }

                            if ((string.IsNullOrEmpty(eventlst[i].latitude) || string.IsNullOrEmpty(eventlst[i].longitude)) && (!string.IsNullOrEmpty(eventlst[i].venue_name)))
                            {
                                Adress adrs = new Adress();
                                adrs.Key = keys[0];
                                adrs.Address = eventlst[i].venue_name;
                                adrs.GeoCode();
                                eventlst[i].latitude = adrs.Latitude;
                                eventlst[i].longitude = adrs.Longitude;
                            }

                            if ((!string.IsNullOrEmpty(eventlst[i].latitude) && !string.IsNullOrEmpty(eventlst[i].longitude)) && (string.IsNullOrEmpty(eventlst[i].venue_name)))
                            {
                                GetAddress address = new GetAddress();
                                eventlst[i].venue_address = address.RetrieveFormatedAddress(eventlst[i].latitude, eventlst[i].longitude, keys[0]);
                            }


                            SqlCommand com = new SqlCommand("AddEventsFromMySql", con);
                            com.CommandType = CommandType.StoredProcedure;
                            com.Parameters.AddWithValue("@EventName", eventlst[i].title);
                            com.Parameters.AddWithValue("@Latitude", eventlst[i].latitude);
                            com.Parameters.AddWithValue("@Longitude", eventlst[i].longitude);
                            com.Parameters.AddWithValue("@Location", eventlst[i].venue_name);
                            if (!string.IsNullOrEmpty(eventlst[i].description))
                                com.Parameters.AddWithValue("@description", eventlst[i].description);

                            if (!string.IsNullOrEmpty(eventlst[i].category))
                                com.Parameters.AddWithValue("@Category", eventlst[i].category);
                            com.Parameters.AddWithValue("@StartDate", date);
                            com.Parameters.AddWithValue("@StartTime", eventlst[i].event_time);
                            if (!string.IsNullOrEmpty(eventlst[i].image))
                            {
                                com.Parameters.AddWithValue("@Image", eventlst[i].image);
                            }
                            if (!string.IsNullOrEmpty(eventlst[i].link_url))
                                com.Parameters.AddWithValue("@ticketUrl", eventlst[i].link_url);
                            con.Open();

                            SqlDataAdapter da = new SqlDataAdapter(com);
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            con.Close();

                        }
                    }
                }
                catch (Exception ex)
                {
                    //ErrorLog.AddLog(1, ex.Message, 0);
                }
            }
        }


        public class Event
        {
            public string title { get; set; }
            public string image { get; set; }
            public string description { get; set; }
            public string event_date { get; set; }
            public string event_time { get; set; }
            public string longitude { get; set; }
            public string latitude { get; set; }
            public string venue_name { get; set; }
            public string venue_address { get; set; }
            public string price { get; set; }
            public string category { get; set; }
            public string link_url { get; set; }
        }

        public class EventModel
        {
            public List<Event> @event { get; set; }
            public string total_record { get; set; }
            public int limit { get; set; }
        }
    }
}