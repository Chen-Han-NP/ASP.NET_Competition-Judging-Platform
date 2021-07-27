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
    public class CompetitionScoreDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;
        //Constructor
        public CompetitionScoreDAL()
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

        public CompetitionScore getCompetitorScore(int competitorID, int competitionID, int criteriaID)
        {
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"SELECT CriteriaID, CompetitorID, CompetitionID, Score
FROM CompetitionScore WHERE (CompetitorID = @competitorID) AND (CompetitionID = @competitionID) AND (CriteriaID = @criteriaID)";

            cmd.Parameters.AddWithValue("@competitorID", competitorID);
            cmd.Parameters.AddWithValue("@competitionID", competitionID);
            cmd.Parameters.AddWithValue("@criteriaID", criteriaID);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            reader.Read();
            CompetitionScore cs = new CompetitionScore
            {
                CriteriaID = reader.GetInt32(0),
                CompetitorID = reader.GetInt32(1),
                CompetitionID = reader.GetInt32(2),
                Score = reader.GetInt32(3)
            };

            reader.Close();

            conn.Close();
            return cs;
        }

        public bool hasScore(int competitorID, int competitionID, int criteriaID)
        {
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"SELECT * FROM CompetitionScore WHERE 
(CompetitorID = @competitorID) AND (CompetitionID = @competitionID) AND (CriteriaID = @criteriaID)";

            cmd.Parameters.AddWithValue("@competitorID", competitorID);
            cmd.Parameters.AddWithValue("@competitionID", competitionID);
            cmd.Parameters.AddWithValue("@criteriaID", criteriaID);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            bool read = reader.Read();

            reader.Close();
            conn.Close();

            return read;
        }

        public void addScore(CompetitionScore cs)
        {
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"INSERT INTO CompetitionScore (CompetitorID, CompetitionID, CriteriaID, Score)
VALUES (@competitorID, @competitionID, @criteriaID, @score)";

            cmd.Parameters.AddWithValue("@competitorID", cs.CompetitorID);
            cmd.Parameters.AddWithValue("@competitionID", cs.CompetitionID);
            cmd.Parameters.AddWithValue("@criteriaID", cs.CriteriaID);
            cmd.Parameters.AddWithValue("@score", cs.Score);

            conn.Open();

            cmd.ExecuteScalar();

            conn.Close();

            return;
        }
        public void updateScore(CompetitionScore cs)
        {
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"UPDATE CompetitionScore
SET Score = @score
WHERE (CompetitorID = @competitorID) AND (CompetitionID = @competitionID) AND (CriteriaID = @criteriaID)";

            cmd.Parameters.AddWithValue("@competitorID", cs.CompetitorID);
            cmd.Parameters.AddWithValue("@competitionID", cs.CompetitionID);
            cmd.Parameters.AddWithValue("@criteriaID", cs.CriteriaID);
            cmd.Parameters.AddWithValue("@score", cs.Score);

            conn.Open();

            cmd.ExecuteScalar();

            conn.Close();

            return;
        }

        public double getFinalScore(int competitorID, int competitionID)
        {
            double Score = 0;
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"SELECT cs.Score, c.Weightage
FROM CompetitionScore cs INNER JOIN Criteria c
ON cs.CriteriaID = c.CriteriaID
WHERE (cs.CompetitorID = @a) AND (cs.CompetitionID = @b)";

            cmd.Parameters.AddWithValue("@a", competitorID);
            cmd.Parameters.AddWithValue("@b", competitionID);

            conn.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                int score = reader.GetInt32(0);
                int weightage = reader.GetInt32(1);
                Score = Score + (Convert.ToDouble(score) / 10) * weightage; //score must be converted to double or the math rounds to int making it always 0
            }

            reader.Close();

            conn.Close();
            return Score;
        }
    }
}
