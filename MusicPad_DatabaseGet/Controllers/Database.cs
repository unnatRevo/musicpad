using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace MusicPad_DatabaseGet.Controllers
{
    public class Database
    {
        static MySqlConnection connection = null;
        public Database()
        {
            string conStirng = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
            connection = new MySqlConnection(conStirng);
        }

        public System.Data.DataTable GetData()
        {
            List<List<string>> masterList = new List<List<string>>();
            var dataTable = new System.Data.DataTable();

            if (this.OpenConnection())
            {
                int skip = 0;
                int pageOffSet = 500;

                try
                {
                    var dataColumn = new System.Data.DataColumn("categoryName"); dataTable.Columns.Add(dataColumn);
                    dataColumn = new System.Data.DataColumn("end_date"); dataTable.Columns.Add(dataColumn);
                    dataColumn = new System.Data.DataColumn("endDate"); dataTable.Columns.Add(dataColumn);
                    dataColumn = new System.Data.DataColumn("endTime"); dataTable.Columns.Add(dataColumn);
                    dataColumn = new System.Data.DataColumn("eventDescription"); dataTable.Columns.Add(dataColumn);
                    dataColumn = new System.Data.DataColumn("eventName"); dataTable.Columns.Add(dataColumn);
                    dataColumn = new System.Data.DataColumn("filepath"); dataTable.Columns.Add(dataColumn);
                    dataColumn = new System.Data.DataColumn("id"); dataTable.Columns.Add(dataColumn);
                    dataColumn = new System.Data.DataColumn("isTmaster"); dataTable.Columns.Add(dataColumn);
                    dataColumn = new System.Data.DataColumn("latitude"); dataTable.Columns.Add(dataColumn);
                    dataColumn = new System.Data.DataColumn("location"); dataTable.Columns.Add(dataColumn);
                    dataColumn = new System.Data.DataColumn("longitude"); dataTable.Columns.Add(dataColumn);
                    dataColumn = new System.Data.DataColumn("price"); dataTable.Columns.Add(dataColumn);
                    dataColumn = new System.Data.DataColumn("scriptName"); dataTable.Columns.Add(dataColumn);
                    dataColumn = new System.Data.DataColumn("start_date"); dataTable.Columns.Add(dataColumn);
                    dataColumn = new System.Data.DataColumn("startDate"); dataTable.Columns.Add(dataColumn);
                    dataColumn = new System.Data.DataColumn("startTime"); dataTable.Columns.Add(dataColumn);
                    dataColumn = new System.Data.DataColumn("ticketsAvailable"); dataTable.Columns.Add(dataColumn);
                    dataColumn = new System.Data.DataColumn("ticketsLeft"); dataTable.Columns.Add(dataColumn);
                    dataColumn = new System.Data.DataColumn("ticketsSold"); dataTable.Columns.Add(dataColumn);
                    dataColumn = new System.Data.DataColumn("Url"); dataTable.Columns.Add(dataColumn);
                    dataColumn = new System.Data.DataColumn("website"); dataTable.Columns.Add(dataColumn);

                    while (skip <= 2000)
                    {
                        var query = $"select * from events limit {skip}, {pageOffSet}";
                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        cmd.CommandTimeout = 12000000;

                        MySqlDataReader dataReader = cmd.ExecuteReader();
                        if (dataReader != null)
                        {
                            while (dataReader.Read())
                            {
                                var list = new List<string>();
                                var dtRow = dataTable.NewRow();

                                dtRow["categoryName"] = dataReader["categoryName"].ToString();
                                dtRow["end_date"] = dataReader["end_date"].ToString();
                                dtRow["endDate"] = dataReader["endDate"].ToString();
                                dtRow["endTime"] = dataReader["endTime"].ToString();
                                dtRow["eventDescription"] = dataReader["eventDescription"].ToString();
                                dtRow["eventName"] = dataReader["eventName"].ToString();
                                dtRow["filepath"] = dataReader["filepath"].ToString();
                                dtRow["id"] = dataReader["id"].ToString();
                                dtRow["isTmaster"] = dataReader["isTmaster"].ToString();
                                dtRow["latitude"] = dataReader["latitude"].ToString();
                                dtRow["location"] = dataReader["location"].ToString();
                                dtRow["longitude"] = dataReader["longitude"].ToString();
                                dtRow["price"] = dataReader["price"].ToString();
                                dtRow["scriptName"] = dataReader["scriptName"].ToString();
                                dtRow["start_date"] = dataReader["start_date"].ToString();
                                dtRow["startDate"] = dataReader["startDate"].ToString();
                                dtRow["startTime"] = dataReader["startTime"].ToString();
                                dtRow["ticketsAvailable"] = dataReader["ticketsAvailable"].ToString();
                                dtRow["ticketsLeft"] = dataReader["ticketsLeft"].ToString();
                                dtRow["ticketsSold"] = dataReader["ticketsSold"].ToString();
                                dtRow["Url"] = dataReader["Url"].ToString();
                                dtRow["website"] = dataReader["website"].ToString();

                                dataTable.Rows.Add(dtRow);
                            }
                        }
                        dataReader.Close();

                        skip = pageOffSet;
                        pageOffSet += 500;
                    }
                }
                catch (Exception ex)
                {
                    
                }
                this.CloseConnection();
            }
            else
            {
                Console.WriteLine("Not Done");
            }
            //return masterList;
            return dataTable;
        }

        public bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        // MessageBox.Show("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        //MessageBox.Show("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        public bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                //MessageBox.Show(ex.Message);
                return false;
            }
        }
    }

    class DirAppend
    {
        public static void WriteLog(string log)
        {
            using (StreamWriter w = File.AppendText("G:\\Logs\\log.txt"))
            {
                Log(log, w);
            }

            using (StreamReader r = File.OpenText("G:\\Logs\\log.txt"))
            {
                DumpLog(r);
            }
        }

        public static void Log(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            w.WriteLine("  :");
            w.WriteLine("  :{0}", logMessage);
            w.WriteLine("-------------------------------");
        }

        public static void DumpLog(StreamReader r)
        {
            string line;
            while ((line = r.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
        }
    }
}