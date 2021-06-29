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
    }
}
