using MusicEventAPI.Manage;
using MusicEventAPI.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Web;

namespace MusicEventAPI.Models
{
    public class nyc
    {
        string[] keys = { "AIzaSyAxbXpabERiZmFfjtTLOh77xQimuZM_imQ" };

        public void InsertIntoSql(string path = "")
        {
            List<nycs> model = new List<nycs>();
            try
            {
                ////JObject o1 = JObject.Parse(File.ReadAllText(@"C:\Users\ebiz\Desktop\New folder (4)\nyc\data.json"));
                ////using (StreamReader file = File.OpenText(@"C:\Users\mansoorakram\Downloads\EventsNew\nyc\nyc\data.json"))
                //using (StreamReader file = File.OpenText(@"C:\Users\ebiz\Desktop\Perminder\data.json"))
                //using (JsonTextReader reader = new JsonTextReader(file))
                //{
                //    JObject o2 = (JObject)JToken.ReadFrom(reader);
                //    foreach (var d in o2["disks"].Children())
                //    {
                //        model.Add(new nycs()
                //        {
                //            category = (string)d["category"],
                //            description = (string)d["description"],
                //            hour = (string)d["hour"],
                //            title = (string)d["title"],
                //            url = (string)d["url"],
                //            price = (string)d["price"],
                //            location = (string)d["location"],
                //             date = (string)d["date"],
                //            website_url = (string)d["website_url"]
                //        });
                //    }
                //}

                //string json = File.ReadAllText(@"C:\Users\mansoorakram\Downloads\EventsNew\nyc\nyc\data.json");
                //model = JsonConvert.DeserializeObject<List<nycs>>(json);


                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://54.218.82.234:8080/dotnet/get_json_data.php");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                HttpResponseMessage response = client.GetAsync("").Result;
                if (response.IsSuccessStatusCode)
                {
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    model = JsonConvert.DeserializeObject<List<nycs>>(responseData);
                }

                if (model != null)
                    AddEvents(model, path);
            }
            catch (Exception ex)
            {

            }




        }

        public void AddEvents(List<nycs> eventlst, string path)
        {
            for (int i = 0; i < eventlst.Count(); i++)
            {

                try
                {
                    Thread.Sleep(1000);
                    using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                    {
                        if (!string.IsNullOrEmpty(eventlst[i].event_date))
                        {
                            DateTime date = DateTime.Parse(eventlst[i].event_date);

                            if (string.IsNullOrEmpty(eventlst[i].start_time))
                            {
                                eventlst[i].start_time = "00:00 AM";
                            }
                            string latitude = null, longitude = null;
                            if (!string.IsNullOrEmpty(eventlst[i].venue_address))
                            {
                                Adress adrs = new Adress();
                                adrs.Key = keys[0];
                                adrs.Address = eventlst[i].venue_address;
                                adrs.GeoCode();
                                latitude = adrs.Latitude;
                                longitude = adrs.Longitude;
                            }


                            SqlCommand com = new SqlCommand("AddEventsFromMySql", con);
                            com.CommandType = CommandType.StoredProcedure;
                            com.Parameters.AddWithValue("@EventName", eventlst[i].title);
                            com.Parameters.AddWithValue("@Latitude", latitude);
                            com.Parameters.AddWithValue("@Longitude", longitude);
                            com.Parameters.AddWithValue("@Location", eventlst[i].venue_address);
                            if (!string.IsNullOrEmpty(eventlst[i].category))
                                com.Parameters.AddWithValue("@Category", eventlst[i].category);
                            com.Parameters.AddWithValue("@StartDate", date);
                            com.Parameters.AddWithValue("@StartTime", eventlst[i].start_time);
                            if (!string.IsNullOrEmpty(eventlst[i].description))
                                com.Parameters.AddWithValue("@description", eventlst[i].description);

                            if (!string.IsNullOrEmpty(eventlst[i].image_url))
                            {
                                com.Parameters.AddWithValue("@Image", eventlst[i].image_url);
                            }
                            if (!string.IsNullOrEmpty(eventlst[i].source_url))
                                com.Parameters.AddWithValue("@ticketUrl", eventlst[i].source_url);
                            //com.Parameters.AddWithValue("@EndDate", starttime);
                            //if (!string.IsNullOrEmpty(eventlst[i].image))
                            //{
                            //    com.Parameters.AddWithValue("@Image", GetImagePath(eventlst[i].image));
                            //}
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


    public class nycs
    {
        public string category { get; set; }
        public string description { get; set; }
        public string event_date { get; set; }
        public string event_url { get; set; }
        public string image_url { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string price { get; set; }
        public string source_url { get; set; }
        public string start_time { get; set; }
        public string title { get; set; }
        public string venue_address { get; set; }
        public string venue_name { get; set; }
        public string venue_postal_code { get; set; }
    }

}





