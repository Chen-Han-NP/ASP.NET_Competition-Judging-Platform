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
    public class JudgeDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        public JudgeDAL()
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

        public int Add(JudgeSignUp judgeSignUp)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"INSERT INTO Judge (JudgeName, Salutation, AreaInterestID, EmailAddr, Password)
                                OUTPUT INSERTED.JudgeID
                                VALUES(@judgeName, @salutation, @areaInterestID, @emailAddr, @password)";
            cmd.Parameters.AddWithValue("@judgeName", judgeSignUp.JudgeName);
            cmd.Parameters.AddWithValue("@salutation", judgeSignUp.Salutation);
            cmd.Parameters.AddWithValue("@areaInterestID", judgeSignUp.AreaInterestID);
            cmd.Parameters.AddWithValue("@emailAddr", judgeSignUp.EmailAddr);
            cmd.Parameters.AddWithValue("@password", judgeSignUp.Password);

            conn.Open();

            judgeSignUp.JudgeID = (int)cmd.ExecuteScalar();

            conn.Close();

            return judgeSignUp.JudgeID;
        }
        public bool isEmailExists(string email, int judgeID)
        {
            bool emailFound = false;
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT JudgeID FROM Judge
                                WHERE EmailAddr=@selectedEmail";
            cmd.Parameters.AddWithValue("@selectedEmail", email);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            { //Records found
                while (reader.Read())
                {
                    if (reader.GetInt32(0) != judgeID)
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
