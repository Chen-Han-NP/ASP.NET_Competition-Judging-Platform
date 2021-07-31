using GameTime.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GameTime.DAL
{
    public class CriteriaDAL
    {
        private IConfiguration Configuration { get; }
        private SqlConnection conn;

        public CriteriaDAL()
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

        public List<Criteria> GetAllCriteria(int competitionID)
        {
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"SELECT CriteriaID, CompetitionID, CriteriaName, Weightage
FROM Criteria
WHERE CompetitionID = @competitionID";
            cmd.Parameters.AddWithValue("@competitionID", competitionID);

            conn.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            List<Criteria> cList = new List<Criteria>();

            while (reader.Read())
            {
                cList.Add(new Criteria
                {
                    CriteriaID = reader.GetInt32(0),
                    CompetitionID = reader.GetInt32(1),
                    CriteriaName = reader.GetString(2),
                    Weightage = reader.GetInt32(3)
                });
            }

            reader.Close();
            conn.Close();

            return cList;
        }

        public Criteria GetCriteria(int competitionID, int criteriaID)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT CriteriaName, Weightage
FROM Criteria
WHERE (CompetitionID = @competitionID) AND (CriteriaID = @criteriaID)";
            cmd.Parameters.AddWithValue("@competitionID", competitionID);
            cmd.Parameters.AddWithValue("@criteriaID", criteriaID);
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();

            reader.Read();
            Criteria criteria = new Criteria {
                CompetitionID = competitionID,
                CriteriaID = criteriaID,
                CriteriaName = reader.GetString(0),
                Weightage = reader.GetInt32(1)
            };

            reader.Close();
            conn.Close();
            return criteria;
        }

        public int GetTotalCriteria(int competitionId)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT CompetitionID, SUM(Weightage)
FROM Criteria
WHERE CompetitionID = @competitionID
GROUP BY CompetitionID";
            cmd.Parameters.AddWithValue("@competitionID", competitionId);
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();

            reader.Read();
            int TotalCriteria = reader.GetInt32(1);

            reader.Close();
            conn.Close();
            return TotalCriteria;
        }

        public int Add(Criteria createCriteria)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"INSERT INTO Criteria (CriteriaName, CompetitionID, Weightage)
                                OUTPUT INSERTED.CriteriaID
                                VALUES(@criteriaName, @competitionID, @weightage)";
            cmd.Parameters.AddWithValue("@criteriaName", createCriteria.CriteriaName);
            cmd.Parameters.AddWithValue("@competitionID", createCriteria.CompetitionID);
            cmd.Parameters.AddWithValue("@weightage", createCriteria.Weightage);

            conn.Open();

            try
            {
                createCriteria.CriteriaID = (int)cmd.ExecuteScalar();
            }
            catch (SqlException)
            {

                return 0;
            }

            conn.Close();

            return createCriteria.CriteriaID;
        }
    }
}
