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
        public List<Competitor> GetAllCompetitor()
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM Competitor ORDER BY CompetitorID";
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into a staff list
            List<Competitor> competitorList = new List<Competitor>();
            while (reader.Read())
            {
                competitorList.Add(
                new Competitor
                {
                     CompetitorID = reader.GetInt32(0), 
                     CompetitorName = reader.GetString(1), 
                     Salutation = reader.GetString(2),
                     EmailAddr = reader.GetString(3), 
                     Password = reader.GetString(4)
                }
                );
            }
            reader.Close();
            conn.Close();

            return competitorList;
        }

        public int Add(Competitor competitorSignUp)
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
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT CompetitorID FROM Competitor
                                WHERE EmailAddr=@selectedEmail";
            cmd.Parameters.AddWithValue("@selectedEmail", email);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            { //Records found
                while (reader.Read())
                {
                    if (reader.GetInt32(0) != competitorID)
                        //The email address is used by another staff
                        emailFound = true;
                }
            }
            else
            { //No record
                emailFound = false; // The email address given does not exist
            }
            reader.Close();
            conn.Close();

            return (emailFound);
        }

    }
}
