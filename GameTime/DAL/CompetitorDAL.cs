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
                Competitor competitor = new Competitor();
                competitor.CompetitorID = reader.GetInt32(0);
                competitor.CompetitorName = reader.GetString(1);
                if (reader.IsDBNull(2))
                {
                    competitor.Salutation = "NA";
                }
                else
                    competitor.Salutation = reader.GetString(2);
                competitor.EmailAddr = reader.GetString(3);
                competitor.Password = reader.GetString(4);
                competitorList.Add(competitor);
                
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
            //cmd.Parameters.AddWithValue("@salutation", competitorSignUp.Salutation);
            if (competitorSignUp.Salutation == null)
            {
                cmd.Parameters.AddWithValue("@salutation", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@salutation", competitorSignUp.Salutation);
            }
            cmd.Parameters.AddWithValue("@emailAddr", competitorSignUp.EmailAddr);
            cmd.Parameters.AddWithValue("@password", competitorSignUp.Password);

            

            conn.Open();

            competitorSignUp.CompetitorID = (int)cmd.ExecuteScalar();

            conn.Close();

            return competitorSignUp.CompetitorID;
        }

        public List<CompetitionViewModel> GetAllAvailableCompetitions(int competitorId)
        {

            SqlCommand cmd = conn.CreateCommand();

            // SQL 1st subquery checks for previously joined competitions
            // 2nd subquery checks for competitions with 2 or more judges
            // 3rd subquery checks for competitions with 3 or more days left to join
            cmd.CommandText = @"
SELECT c.CompetitionID, c.AreaInterestID, a.Name, c.CompetitionName, c.StartDate, c.EndDate, c.ResultReleasedDate
FROM Competition c
INNER JOIN AreaInterest a
ON c.AreaInterestID = a.AreaInterestID
WHERE c.CompetitionID NOT IN
(SELECT c.CompetitionID
FROM Competition c
INNER JOIN CompetitionSubmission cs
ON c.CompetitionID = cs.CompetitionID
WHERE cs.CompetitorID = @competitorId)
AND c.CompetitionID IN
(SELECT CompetitionID
FROM CompetitionJudge
GROUP BY CompetitionID
HAVING (COUNT(JudgeID) >= 2))
AND c.CompetitionID NOT IN
(SELECT CompetitionID
FROM Competition
WHERE GETDATE() > DATEADD(DAY, -3, StartDate))
AND (
(c.StartDate IS NOT NULL) AND (c.EndDate IS NOT NULL) AND (c.ResultReleasedDate IS NOT NULL))
";

            cmd.Parameters.AddWithValue("@competitorId", competitorId);
            conn.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            List<CompetitionViewModel> competitionList = new List<CompetitionViewModel>();
            while (reader.Read())
            {
                competitionList.Add(
                new CompetitionViewModel
                {
                    CompetitionID = reader.GetInt32(0),
                    AreaInterestID = reader.GetInt32(1),
                    AreaInterestName = reader.GetString(2),
                    CompetitionName = reader.GetString(3),
                    StartDate = !reader.IsDBNull(4) ?
                                  reader.GetDateTime(4) : (DateTime?)null,
                    EndDate = !reader.IsDBNull(5) ?
                                  reader.GetDateTime(5) : (DateTime?)null,
                    ResultReleasedDate = !reader.IsDBNull(6) ?
                                  reader.GetDateTime(6) : (DateTime?)null
                });
            }

            reader.Close();

            conn.Close();
            return competitionList;
        }
        public List<CompetitionViewModel> GetJoinedCompetitions(int competitorId)
        {

            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"SELECT c.CompetitionID, c.AreaInterestID, a.Name, c.CompetitionName, c.StartDate, c.EndDate, c.ResultReleasedDate
FROM Competition c
INNER JOIN AreaInterest a
ON c.AreaInterestID = a.AreaInterestID
INNER JOIN CompetitionSubmission cs
ON cs.CompetitionID = c.CompetitionID
WHERE cs.CompetitorID = @competitorId
";

            cmd.Parameters.AddWithValue("@competitorId", competitorId);
            conn.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            List<CompetitionViewModel> competitionList = new List<CompetitionViewModel>();
            while (reader.Read())
            {
                competitionList.Add(
                new CompetitionViewModel
                {
                    CompetitionID = reader.GetInt32(0),
                    AreaInterestID = reader.GetInt32(1),
                    AreaInterestName = reader.GetString(2),
                    CompetitionName = reader.GetString(3),
                    StartDate = !reader.IsDBNull(4) ?
                                  reader.GetDateTime(4) : (DateTime?)null,
                    EndDate = !reader.IsDBNull(5) ?
                                  reader.GetDateTime(5) : (DateTime?)null,
                    ResultReleasedDate = !reader.IsDBNull(6) ?
                                  reader.GetDateTime(6) : (DateTime?)null
                });
            }

            reader.Close();

            conn.Close();
            return competitionList;
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

        public int UpdateCompetitorFile(CompetitorSubmissionViewModel competitor)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"
UPDATE CompetitionSubmission
SET FileSubmitted = @fileSubmitted, DateTimeFileUpload = @DateTimeFileUpload
WHERE CompetitionID = @CompetitionID AND CompetitorID = @CompetitorID
";
            cmd.Parameters.AddWithValue("@fileSubmitted", competitor.FileSubmitted);
            cmd.Parameters.AddWithValue("@DateTimeFileUpload", competitor.DateTimeSubmitted);
            cmd.Parameters.AddWithValue("@CompetitionID", competitor.CompetitionId);
            cmd.Parameters.AddWithValue("@CompetitorID", competitor.CompetitorId);

            conn.Open();
            int count = cmd.ExecuteNonQuery();
            conn.Close();
            return count;
        }

        public int UpdateCompetitorAppeal(CompetitorSubmissionViewModel competitor)
        {
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"
UPDATE CompetitionSubmission
SET Appeal = @appeal
WHERE CompetitionID = @CompetitionID AND CompetitorID = @CompetitorID
";
            cmd.Parameters.AddWithValue("@appeal", competitor.Appeal);
            cmd.Parameters.AddWithValue("@CompetitionID", competitor.CompetitionId);
            cmd.Parameters.AddWithValue("@CompetitorID", competitor.CompetitorId);

            conn.Open();
            int count = cmd.ExecuteNonQuery();
            conn.Close();
            return count;
        }

        public List<CriteriaScoreAppealViewModel> getAllCriteriaScore(int competitorId, int competitionId)
        {
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"SELECT DISTINCT c.CriteriaID, c.CriteriaName, c.Weightage, cscore.Score
FROM Criteria c
INNER JOIN CompetitionScore cscore
ON c.CriteriaID = cscore.CriteriaID AND c.CompetitionID = cscore.CompetitionID
WHERE cscore.CompetitionID = @competitionID AND cscore.CompetitorID = @competitorID
";

            cmd.Parameters.AddWithValue("@competitorID", competitorId);
            cmd.Parameters.AddWithValue("@competitionID", competitionId);
            conn.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            List<CriteriaScoreAppealViewModel> scoreList = new List<CriteriaScoreAppealViewModel>();
            while (reader.Read())
            {
                scoreList.Add(
                new CriteriaScoreAppealViewModel
                {
                    CriteriaID = reader.GetInt32(0),
                    CriteriaName = reader.GetString(1),
                    Weightage = reader.GetInt32(2),
                    Score = reader.GetInt32(3)
                });
            }

            reader.Close();

            conn.Close();
            return scoreList;
        }

        public string getAppeal(int competitorId, int competitionId)
        {
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"SELECT Appeal
FROM CompetitionSubmission
WHERE CompetitionID = @competitionID AND CompetitorID = @competitorID
";

            cmd.Parameters.AddWithValue("@competitorID", competitorId);
            cmd.Parameters.AddWithValue("@competitionID", competitionId);
            conn.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            string appeal = "";
            while (reader.Read())
            {
                appeal = !reader.IsDBNull(0) ?
                                  reader.GetString(0) : (string)null;
            };

            reader.Close();

            conn.Close();
            return appeal;
        }

    }
}
