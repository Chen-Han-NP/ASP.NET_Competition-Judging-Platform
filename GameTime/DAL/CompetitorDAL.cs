using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data.SqlClient;
using GameTime.Models;

namespace GameTime.DAL
{
    public class CompetitorDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;
        //Constructor
        public CompetitorDAL()
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

        public int Add(CompetitorSignUp competitorSignUp)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"INSERT INTO Competitor (CompetitorName, Salutation, EmailAddr, Password)
                                OUTPUT INSERTED.CompetitorID
                                VALUES(@competitorName, @salutation, @emailAddr, @password)";
            cmd.Parameters.AddWithValue("@competitorName", competitorSignUp.CompetitorName);
            cmd.Parameters.AddWithValue("@salutation", competitorSignUp.Salutation);
            cmd.Parameters.AddWithValue("@emailAddr", competitorSignUp.EmailAddr);
            cmd.Parameters.AddWithValue("@password", competitorSignUp.Password);

            conn.Open();

            competitorSignUp.CompetitorID = (int)cmd.ExecuteScalar();

            conn.Close();

            return competitorSignUp.CompetitorID;
        }

        //Add Checks for unique Email
        public bool isEmailExists(string email, int competitorID)
        {
            bool emailFound = false;
            return (emailFound);
        }

    }
}
