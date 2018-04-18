using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace MusicEventAPI.Models
{
    public static class Executeservices
    {
        static string _ScheduledRunningTime = "1:00 AM";
             static   int x = 0;

        public static void StartCheckingLog()
        {

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000;
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        static void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                string _CurrentTime = String.Format("{0:t}", DateTime.Now);

                if (x == 0)
                {
                    nyc nn = new nyc();
                    nn.InsertIntoSql();
                }

                if (_CurrentTime == _ScheduledRunningTime)
                {

                    if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
                    {
                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri("http://54.218.82.234:8091/tmaster");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.GetAsync(client.BaseAddress);

                        ctvisit _ctvisit = new ctvisit();
                        _ctvisit.InsertIntoSql();

                        visitnj _visitnj = new visitnj();
                        _visitnj.InsertIntoSql();

                    }
                 else  if (DateTime.Now.DayOfWeek == DayOfWeek.Tuesday)
                    {
                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri("http://54.218.82.234:8091/nyc");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.GetAsync(client.BaseAddress);

                        HttpClient clients = new HttpClient();
                        clients.BaseAddress = new Uri("http://54.218.82.234:8091/yelp"); // missing class
                        clients.DefaultRequestHeaders.Accept.Clear();
                        clients.GetAsync(clients.BaseAddress);


                        ticketmaster ts = new ticketmaster();
                        ts.InsertIntoSql();
                    }
               else   if (DateTime.Now.DayOfWeek == DayOfWeek.Wednesday)
                    {
                        //HttpClient client = new HttpClient();
                        //client.BaseAddress = new Uri("http://54.218.82.234:8091/eventbrite");
                        //client.DefaultRequestHeaders.Accept.Clear();
                        //client.GetAsync(client.BaseAddress);

                        nyc n = new nyc();
                        n.InsertIntoSql();


                    }
                else  if (DateTime.Now.DayOfWeek == DayOfWeek.Thursday)
                    {
                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri("http://54.218.82.234:8091/lasvegass");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.GetAsync(client.BaseAddress);

                        //eventbrite eb = new eventbrite();
                        //eb.InsertIntoSql();


                    }
                else   if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
                    {
                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri("http://54.218.82.234:8091/bostoncalendar");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.GetAsync(client.BaseAddress);

                        lasvegas ls = new lasvegas();
                        ls.InsertIntoSql();


                    }
              else  if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
                    {
                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri("http://54.218.82.234:8091/ettr");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.GetAsync(client.BaseAddress);

                        bostoncalendar bs = new bostoncalendar();
                        bs.InsertIntoSql();
                    }
                    else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    {
                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri("http://54.218.82.234:8091/ctvisit");
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.GetAsync(client.BaseAddress);

                        HttpClient clients = new HttpClient();
                        clients.BaseAddress = new Uri("http://54.218.82.234:8091/visitnj");
                        clients.DefaultRequestHeaders.Accept.Clear();
                        clients.GetAsync(clients.BaseAddress);

                        ettractions ett = new ettractions();
                        ett.InsertIntoSql();

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}