using MusicEventAPI.Manage;
using MusicEventAPI.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Xml;

namespace MusicEventAPI.Models
{
    public class bostoncalendar
    {
        string[] keys = { "AIzaSyAxbXpabERiZmFfjtTLOh77xQimuZM_imQ" };

        public void InsertIntoSql(string path = "")
        {
            ErrorLog.AddLog(1, "InsertIntoSql Call", 0);
            string URL = "http://54.218.82.234:8080/dotnet/bostoncalendar.php";
            int currentpage = 1;
            string urlParameters = "?page=" + currentpage;


            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            bostoncalendarModel _TmasterModel = null;


            HttpResponseMessage response = client.GetAsync(urlParameters).Result;
            if (response.IsSuccessStatusCode)
            {
                var responseData = response.Content.ReadAsStringAsync().Result;
                _TmasterModel = JsonConvert.DeserializeObject<bostoncalendarModel>(responseData);
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
                    _TmasterModel = JsonConvert.DeserializeObject<bostoncalendarModel>(responseData);
                }
                if (_TmasterModel != null)
                    AddEvents(_TmasterModel.@event, path);
                else
                    ErrorLog.AddLog(1, Convert.ToString(currentpage), 0);
                currentpage = currentpage + 1;
            }



        }

        public void AddEvents(List<Eventbostoncalendar> eventlst, string path)
        {
            for (int i = 0; i < eventlst.Count(); i++)
            {

                try
                {
                    using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                    {
                        if (!string.IsNullOrEmpty(eventlst[i].event_date))
                        {
                            DateTime date = DateTime.Parse(eventlst[i].event_date);
                            //TimeSpan starttime = DateTime.Parse("00:00 AM").TimeOfDay; 

                            if (date != null)
                            {
                                eventlst[i].start_time = string.Format("{0:hh:mm tt}", date);
                            }

                            if ((string.IsNullOrEmpty(eventlst[i].latitude) || string.IsNullOrEmpty(eventlst[i].longitude)) && (!string.IsNullOrEmpty(eventlst[i].venue_address)))
                            {
                                Adress adrs = new Adress();
                                adrs.Key = keys[0];
                                adrs.Address = eventlst[i].venue_address;
                                adrs.GeoCode();
                                eventlst[i].latitude = adrs.Latitude;
                                eventlst[i].longitude = adrs.Longitude;
                            }

                            if ((!string.IsNullOrEmpty(eventlst[i].latitude) && !string.IsNullOrEmpty(eventlst[i].longitude)) && (string.IsNullOrEmpty(eventlst[i].venue_address)))
                            {
                                GetAddress address = new GetAddress();
                                eventlst[i].venue_address = address.RetrieveFormatedAddress(eventlst[i].latitude, eventlst[i].longitude, keys[0]);
                            }


                            string filename = Guid.NewGuid().ToString() + ".Jpg";
                            SqlCommand com = new SqlCommand("AddEventsFromMySql", con);
                            com.CommandType = CommandType.StoredProcedure;
                            com.Parameters.AddWithValue("@EventName", eventlst[i].title);
                            com.Parameters.AddWithValue("@Latitude", eventlst[i].latitude);
                            com.Parameters.AddWithValue("@Longitude", eventlst[i].longitude);
                            com.Parameters.AddWithValue("@Location", eventlst[i].venue_address);
                            if(!string.IsNullOrEmpty(eventlst[i].category))
                            com.Parameters.AddWithValue("@Category", eventlst[i].category.Split(',')[0]);
                            com.Parameters.AddWithValue("@StartDate", date);
                            com.Parameters.AddWithValue("@StartTime", eventlst[i].start_time);
                            //com.Parameters.AddWithValue("@EndDate", starttime);
                            if (!string.IsNullOrEmpty(eventlst[i].image))
                            {
                                com.Parameters.AddWithValue("@Image", eventlst[i].image);
                            }
                            if (!string.IsNullOrEmpty(eventlst[i].description))
                                com.Parameters.AddWithValue("@description", eventlst[i].description);
                            if (!string.IsNullOrEmpty(eventlst[i].link_url))
                                com.Parameters.AddWithValue("@ticketUrl", eventlst[i].link_url);
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

        private string GetImagePath(string ImageUrl)
        {
            var arr = ImageUrl.Split('|');
            bool image = false;

            foreach (string item in arr)
            {
                if (item.Contains("RETINA_LANDSCAPE_16_9"))
                {
                    ImageUrl = item;
                    image = true;
                    break;
                }
            }

            if (!image)
                ImageUrl = arr[0];
            return ImageUrl;
        }

        private void AddImage(string ImageUrl, string path, string Filename)
        {
            var arr = ImageUrl.Split('|');
            bool image = false;

            foreach (string item in arr)
            {
                if (item.Contains("RETINA_LANDSCAPE_16_9"))
                {
                    ImageUrl = item;
                    image = true;
                    break;
                }
            }

            if (!image)
                ImageUrl = arr[0];

            System.Drawing.Image Image;
            //string Filename = Guid.NewGuid().ToString() + ".Jpg";

            try
            {
                var webClient = new WebClient();
                byte[] imageBytes = webClient.DownloadData(ImageUrl);

                //string[] idx = ImageUrl.Split('.');
                //string Mimetype = "image/" + idx.LastOrDefault();

                using (var ms = new MemoryStream(imageBytes))
                {
                    Image = System.Drawing.Image.FromStream(ms);
                }
                // original image
                using (var originalPic = new Bitmap(Image.Width, Image.Height))
                using (var gr = Graphics.FromImage(originalPic))
                {
                    gr.DrawImage(Image, 0, 0, Image.Width, Image.Height);
                    originalPic.Save(path + Filename, ImageFormat.Jpeg);
                }
            }
            catch (Exception ex)
            {

            }
            //return "/Content/Image/Event/" + Filename;
        }

        public class Eventbostoncalendar
        {
            public string title { get; set; }
            public string image { get; set; }
            public string description { get; set; }
            public string event_date { get; set; }
            public string start_time { get; set; }
            public string end_time { get; set; }
            public string longitude { get; set; }
            public string latitude { get; set; }
            public string venue_name { get; set; }
            public string venue_address { get; set; }
            public string venue_postal_code { get; set; }
            public string price { get; set; }
            public string category { get; set; }
            public string source_url { get; set; }
            public string link_url { get; set; }
        }

        public class bostoncalendarModel
        {
            public List<Eventbostoncalendar> @event { get; set; }
            public string total_record { get; set; }
            public int limit { get; set; }
        }
    }
}