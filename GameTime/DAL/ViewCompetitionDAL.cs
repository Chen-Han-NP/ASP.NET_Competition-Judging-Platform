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
    public class ViewCompetitionDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;
        //Constructor
        public ViewCompetitionDAL()
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

        public List<CompetitionViewModel> GetAllCompetitions()
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SQL statement that select all branches
            cmd.CommandText = @"SELECT c.CompetitionID, c.AreaInterestID, a.Name, c.CompetitionName, c.StartDate, c.EndDate, c.ResultReleasedDate
FROM Competition c
INNER JOIN AreaInterest a
ON c.AreaInterestID = a.AreaInterestID";

            //Open a database connection
            conn.Open();
            //Execute SELCT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a branch list
            List<CompetitionViewModel> competitionList = new List<CompetitionViewModel>();
            while (reader.Read())
            {
                competitionList.Add(
                new CompetitionViewModel
                {
                    CompetitionID = reader.GetInt32(0), // 0 - 1st column
                    AreaInterestID = reader.GetInt32(1), // 1 - 2nd column
                    AreaInterestName = reader.GetString(2),
                    CompetitionName = reader.GetString(3), // 2 - 3rd column
                    StartDate = reader.GetDateTime(4),
                    EndDate = reader.GetDateTime(5),
                    ResultReleasedDate = reader.GetDateTime(6)
                });
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return competitionList;
        }




    }
}
