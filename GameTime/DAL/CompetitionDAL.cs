using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using GameTime.Models;
using System.Data.SqlClient;

namespace GameTime.DAL
{
    public class CompetitionDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;
        //Constructor
        public CompetitionDAL()
        {
            //Read ConnectionString from appsettings.json file
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            string strConn = Configuration.GetConnectionString(
            "GameTimeConnectionString");
            //Instantiate a SqlConnection object with the
            //Connection String read.
            conn = new SqlConnection(strConn);
        }

        public int AddComp(Competition comp)
        {
            
            if (comp.StartDate == null)
            {
                comp.StartDate = DateTime.Parse("01/01/1753");
            }
            if (comp.EndDate == null)
            {
                comp.EndDate = DateTime.Parse("01/01/1753");
            }
            if (comp.ResultReleasedDate == null)
            {
                comp.ResultReleasedDate = DateTime.Parse("01/01/1753");
            }
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an INSERT SQL statement which will
            //return the auto-generated StaffID after insertion
            cmd.CommandText = @"INSERT INTO Competition (AreaInterestID, CompetitionName, StartDate, EndDate, ResultReleasedDate)
OUTPUT INSERTED.CompetitionID
VALUES(@AreaInterestID, @CompetitionName, @StartDate, @EndDate, @ResultReleasedDate)";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@AreaInterestID", comp.AreaInterestID);
            cmd.Parameters.AddWithValue("@CompetitionName", comp.CompetitionName); 
            cmd.Parameters.AddWithValue("@StartDate", comp.StartDate);
            cmd.Parameters.AddWithValue("@EndDate", comp.EndDate);
            cmd.Parameters.AddWithValue("@ResultReleasedDate", comp.ResultReleasedDate);
            //A connection to database must be opened before any operations made.
            conn.Open();
            //ExecuteScalar is used to retrieve the auto-generated
            //StaffID after executing the INSERT SQL statement
            comp.CompetitionID = (int)cmd.ExecuteScalar();
            //A connection should be closed after operations.
            conn.Close();
            //Return id when no error occurs.
            return comp.CompetitionID;
        }

        public List<Competition> GetAllComp()
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM Competition ORDER BY CompetitionID";
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<Competition> CompetitionList = new List<Competition>();
            while (reader.Read())
            {
                
                CompetitionList.Add(
                new Competition
                {
                    CompetitionID = reader.GetInt32(0), //0: 1st column
                    AreaInterestID = reader.GetInt32(1), //1: 2nd column
                                                         //Get the first character of a string
                    CompetitionName = reader.GetString(2), //2: 3rd column

                    
                      StartDate = reader.GetDateTime(3), //3: 4th column
                      EndDate = reader.GetDateTime(4),
                      ResultReleasedDate = reader.GetDateTime(5)
                } 
                );
                
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            for (int i = 0; i < CompetitionList.Count; i++)
            {
                if (CompetitionList[i].StartDate == DateTime.Parse("01/01/1753"))
                {
                    CompetitionList[i].StartDate = null;
                }
                if (CompetitionList[i].EndDate == DateTime.Parse("01/01/1753"))
                {
                    CompetitionList[i].EndDate = null;
                }
                if (CompetitionList[i].ResultReleasedDate == DateTime.Parse("01/01/1753"))
                {
                    CompetitionList[i].ResultReleasedDate = null;
                }
            }
            return CompetitionList;
        }
    }
}
