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
using System.Text;
using System.Web;
using System.Xml;

namespace MusicEventAPI.Models
{
    public class ticketmaster
    {
        string[] keys = { "AIzaSyAxbXpabERiZmFfjtTLOh77xQimuZM_imQ" };
        static bool isTmaster = true;

        public void InsertIntoSql(string path = "")
        {
            string URL = "http://54.218.82.234:8080/dotnet/ticketmaster.php";
            int currentpage = 1;
            string urlParameters = "?page=" + currentpage;


            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(URL);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            TmasterModel _TmasterModel = null;


            HttpResponseMessage response = client.GetAsync(urlParameters).Result;
            if (response.IsSuccessStatusCode)
            {
                var responseData = response.Content.ReadAsStringAsync().Result;
                _TmasterModel = JsonConvert.DeserializeObject<TmasterModel>(responseData);
            }
            int totalpagecount = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(_TmasterModel.total_record) / _TmasterModel.limit));
            AddEvents(_TmasterModel.tmaster, path);

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
                    _TmasterModel = JsonConvert.DeserializeObject<TmasterModel>(responseData);
                }
                if (_TmasterModel != null)
                    AddEvents(_TmasterModel.tmaster, path);
                else
                    ErrorLog.AddLog(1, Convert.ToString(currentpage), 0);
                currentpage = currentpage + 1;
            }



        }

        public void AddEvents(List<Tmaster> eventlst, string path)
        {
            for (int i = 0; i < eventlst.Count(); i++)
            {

                try
                {
                    using (SqlConnection con = new SqlConnection(DBConnection.MusicEventConnectionString))
                    {
                        if (!string.IsNullOrEmpty(eventlst[i].date))
                        {
                            DateTime date = DateTime.Parse(eventlst[i].date);
                            //TimeSpan starttime = DateTime.Parse("00:00 AM").TimeOfDay; 

                            if (string.IsNullOrEmpty(eventlst[i].time))
                            {
                                eventlst[i].time = "00:00 AM";
                            }

                            if ((string.IsNullOrEmpty(eventlst[i].Latitude) || string.IsNullOrEmpty(eventlst[i].Longitude)) && (!string.IsNullOrEmpty(eventlst[i].Venue)))
                            {
                                Adress adrs = new Adress();
                                adrs.Key = keys[0];
                                adrs.Address = eventlst[i].Venue;
                                adrs.GeoCode();
                                eventlst[i].Latitude = adrs.Latitude;
                                eventlst[i].Longitude = adrs.Longitude;
                            }

                            if ((!string.IsNullOrEmpty(eventlst[i].Latitude) && !string.IsNullOrEmpty(eventlst[i].Longitude)) && (string.IsNullOrEmpty(eventlst[i].Venue)))
                            {
                                GetAddress address = new GetAddress();
                                eventlst[i].Venue = address.RetrieveFormatedAddress(eventlst[i].Latitude, eventlst[i].Longitude, keys[0]);
                            }


                            string filename = Guid.NewGuid().ToString() + ".Jpg";
                            SqlCommand com = new SqlCommand("AddEventsFromMySql", con);
                            com.CommandType = CommandType.StoredProcedure;
                            com.Parameters.AddWithValue("@EventName", eventlst[i].Name);
                            com.Parameters.AddWithValue("@Latitude", eventlst[i].Latitude);
                            com.Parameters.AddWithValue("@Longitude", eventlst[i].Longitude);
                            com.Parameters.AddWithValue("@Location", eventlst[i].Venue);
                            com.Parameters.AddWithValue("@Category", eventlst[i].category);
                            com.Parameters.AddWithValue("@StartDate", date);
                            com.Parameters.AddWithValue("@StartTime", eventlst[i].time);
                            com.Parameters.AddWithValue("@isTmaster", isTmaster);
                            if (!string.IsNullOrEmpty(eventlst[i].source))
                                com.Parameters.AddWithValue("@ticketUrl", eventlst[i].source);
                            //com.Parameters.AddWithValue("@EndDate", starttime);
                            if (!string.IsNullOrEmpty(eventlst[i].image))
                            {
                                com.Parameters.AddWithValue("@Image", GetImagePath(eventlst[i].image));
                            }
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
                    //ErrorLog.AddLog(1, ex.Message, 0);
                }
            }
        }

        private string GetImagePath(string ImageUrl)
        {
            var arr = ImageUrl.Split('|');
            bool image = false;

            foreach (string item in arr)
            {
                if (item.Contains("RETINA_PORTRAIT_16_9"))
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
    }


    public class Tmaster
    {
        public string Name { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Venue { get; set; }
        public string source { get; set; }
        public string type { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public string TimeZone { get; set; }
        public string presale_startdate { get; set; }
        public string presale_enddate { get; set; }
        public string category { get; set; }
        public string image { get; set; }
    }

    public class TmasterModel
    {
        public List<Tmaster> tmaster { get; set; }
        public string total_record { get; set; }
        public int limit { get; set; }
    }

    public class Adress
    {
        public Adress()
        {
        }
        //Properties
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Address { get; set; }
        public string Key { get; set; }

        //The Geocoding here i.e getting the latt/long of address
        public void GeoCode()
        {
            ////to Read the Stream
            //StreamReader sr = null;

            ////The Google Maps API Either return JSON or XML. We are using XML Here
            ////Saving the url of the Google API 
            //string url = String.Format("http://maps.googleapis.com/maps/api/geocode/xml?address=" +
            //this.Address + "&sensor=false&key="+this.Key);

            ////to Send the request to Web Client 
            //WebClient wc = new WebClient();
            //try
            //{
            //    sr = new StreamReader(wc.OpenRead(url));
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("The Error Occured" + ex.Message);
            //}

            //try
            //{
            //    XmlTextReader xmlReader = new XmlTextReader(sr);
            //    bool latread = false;
            //    bool longread = false;

            //    while (xmlReader.Read())
            //    {
            //        xmlReader.MoveToElement();
            //        switch (xmlReader.Name)
            //        {
            //            case "lat":

            //                if (!latread)
            //                {
            //                    xmlReader.Read();
            //                    this.Latitude = xmlReader.Value.ToString();
            //                    latread = true;

            //                }
            //                break;
            //            case "lng":
            //                if (!longread)
            //                {
            //                    xmlReader.Read();
            //                    this.Longitude = xmlReader.Value.ToString();
            //                    longread = true;
            //                }

            //                break;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("An Error Occured" + ex.Message);
            //}

            try
            {
                string url = "http://maps.google.com/maps/api/geocode/xml?address=" + Address + "&sensor=false";
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        DataSet dsResult = new DataSet();
                        dsResult.ReadXml(reader);
                        foreach (DataRow row in dsResult.Tables["result"].Rows)
                        {
                            string geometry_id = dsResult.Tables["geometry"].Select("result_id = " + row["result_id"].ToString())[0]["geometry_id"].ToString();
                            DataRow location = dsResult.Tables["location"].Select("geometry_id = " + geometry_id)[0];
                            Latitude = Convert.ToString(location["lat"]);
                            Longitude = Convert.ToString(location["lng"]);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }


    public class GetAddress
    {

        string baseUri = "http://maps.googleapis.com/maps/api/geocode/xml?latlng={0},{1}&sensor=false&key={2}";
        string location = string.Empty;

        public string RetrieveFormatedAddress(string lat, string lng, string key)
        {
            //string requestUri = string.Format(baseUri, lat, lng,key);

            //using (WebClient wc = new WebClient())
            //{
            //    string result = wc.DownloadString(requestUri);
            //    var xmlElm = System.Xml.Linq.XElement.Parse(result);
            //    var status = (from elm in xmlElm.Descendants()
            //                  where elm.Name == "status"
            //                  select elm).FirstOrDefault();
            //    if (status.Value.ToLower() == "ok")
            //    {
            //        var res = (from elm in xmlElm.Descendants()
            //                   where elm.Name == "formatted_address"
            //                   select elm).FirstOrDefault();
            //        location = res.Value;
            //    }
            //}
            //return location;
            try
            {
                string url = "http://maps.google.com/maps/api/geocode/xml?latlng={0},{1}&sensor=false";
                url = string.Format(url, lat, lng);
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        DataSet dsResult = new DataSet();
                        dsResult.ReadXml(reader);
                        return dsResult.Tables["result"].Rows[0]["formatted_address"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }


}
