using MusicEventAPI.Manage;
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

namespace MusicEventAPI.Models
{
    public static class mysqlfirst
    {

        public static void InsertIntoSql(string path = "")
        {
            string URL = "http://54.218.82.234:8080/dotnet/mysqlfirst.php";
            int currentpage = 1;
            string urlParameters = "?page="+currentpage;
        

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            mysqlfirstModel _mysqlfirst = null;


            HttpResponseMessage response = client.GetAsync(urlParameters).Result;
            if (response.IsSuccessStatusCode)
            {
                var responseData = response.Content.ReadAsStringAsync().Result;
                _mysqlfirst = JsonConvert.DeserializeObject<mysqlfirstModel>(responseData);
            }

                  int  totalpagecount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal( _mysqlfirst.total_record) / _mysqlfirst.limit));
            AddEvents(_mysqlfirst.firsttable, path);

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
                    _mysqlfirst = JsonConvert.DeserializeObject<mysqlfirstModel>(responseData);
                }
                AddEvents(_mysqlfirst.firsttable, path);
                currentpage = currentpage + 1;
            }



        }

        public static void AddEvents(List<Firsttable> eventlst, string path)
        {
            for (int i = 0; i < eventlst.Count(); i++)
            {

                try
                {
                    using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                    {
                        if (!string.IsNullOrEmpty(eventlst[i].Day_date))
                        {
                            DateTime date = DateTime.Parse(eventlst[i].Day_date);
                            //TimeSpan starttime = DateTime.Parse("00:00 AM").TimeOfDay; 

                            if (eventlst[i].Time == "Not Known")
                            {
                                eventlst[i].Time = "00:00 AM";
                            }

                            //string filename = Guid.NewGuid().ToString() + ".Jpg";
                            SqlCommand com = new SqlCommand("AddEventsFromMySql", con);
                            com.CommandType = CommandType.StoredProcedure;
                            com.Parameters.AddWithValue("@EventName", eventlst[i].Event_Name);
                            com.Parameters.AddWithValue("@Latitude", eventlst[i].Latitude);
                            com.Parameters.AddWithValue("@Longitude", eventlst[i].Longitude);
                            com.Parameters.AddWithValue("@Location", eventlst[i].Address);
                            com.Parameters.AddWithValue("@Category", "Rock");
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

        private static void AddImage(string ImageUrl, string path, string Filename)
        {

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


    }

    public class Firsttable
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

    public class mysqlfirstModel
    {
        public List<Firsttable> firsttable { get; set; }
        public string total_record { get; set; }
        public int limit { get; set; }
    }
}