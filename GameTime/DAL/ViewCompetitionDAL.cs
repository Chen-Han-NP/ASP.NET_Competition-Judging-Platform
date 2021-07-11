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

            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"SELECT c.CompetitionID, c.AreaInterestID, a.Name, c.CompetitionName, c.StartDate, c.EndDate, c.ResultReleasedDate
FROM Competition c
INNER JOIN AreaInterest a
ON c.AreaInterestID = a.AreaInterestID";

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

        public List<CompetitionViewModel> GetAllAvailableCompetitions(int competitorId)
        {

            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"SELECT c.CompetitionID, c.AreaInterestID, a.Name, c.CompetitionName, c.StartDate, c.EndDate, c.ResultReleasedDate
FROM Competition c
INNER JOIN AreaInterest a
ON c.AreaInterestID = a.AreaInterestID
where c.CompetitionID not in (select c.CompetitionID 
from Competition c
Inner join CompetitionSubmission cs
on c.CompetitionID = cs.CompetitionID
where cs.CompetitorID = @competitorId
)";

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

        public Competition getCompetitionDetails(int competitionId)
        {

            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"SELECT * FROM Competition
WHERE CompetitionID = @comId";

            conn.Open();

            cmd.Parameters.AddWithValue("@comId", competitionId);
            SqlDataReader reader = cmd.ExecuteReader();

            Competition competition = new Competition();
            while (reader.Read())
            {
                competition = new Competition
                {
                    CompetitionID = reader.GetInt32(0), 
                    AreaInterestID = reader.GetInt32(1), 
                    CompetitionName = reader.GetString(2), 
                    StartDate = !reader.IsDBNull(3) ?
                                  reader.GetDateTime(3) : (DateTime?)null,
                    EndDate = !reader.IsDBNull(4) ?
                                  reader.GetDateTime(4) : (DateTime?)null,
                    ResultReleasedDate = !reader.IsDBNull(5) ?
                                  reader.GetDateTime(5) : (DateTime?)null
                };
            }

            reader.Close();

            conn.Close();
            return competition;
        }



        public List<Criteria> GetAllCriteria()
        {

            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"SELECT * FROM Criteria ORDER BY CriteriaID";


            conn.Open();

            SqlDataReader reader = cmd.ExecuteReader();

            List<Criteria> criteriaList = new List<Criteria>();
            while (reader.Read())
            {
                criteriaList.Add(
                new Criteria
                {
                    CriteriaID = reader.GetInt32(0), 
                    CompetitionID = reader.GetInt32(1), 
                    CriteriaName = reader.GetString(2),
                    Weightage = reader.GetInt32(3)
                });
            }

            reader.Close();

            conn.Close();
            return criteriaList;
        }

        public List<Comment> getAllComments(int competitionId)
        {

            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"SELECT * FROM Comment
WHERE CompetitionID = @comId";

            cmd.Parameters.AddWithValue("@comId", competitionId);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<Comment> commentList = new List<Comment>();

            while (reader.Read())
            {
                commentList.Add(new Comment
                {
                    CommentID = reader.GetInt32(0),
                    CompetitionID = reader.GetInt32(1),
                    Description = !reader.IsDBNull(2) ?
                                  reader.GetString(2) : (string)null,
                    DateTimePosted = reader.GetDateTime(3)
                }); ;
            }

            reader.Close();

            conn.Close();
            return commentList;
        }


        public int AddComment(Comment comment)
        {
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"INSERT INTO Comment(CompetitionID, Description, DateTimePosted)
OUTPUT INSERTED.CommentID
VALUES(@comId, @description, @datetime)";


            cmd.Parameters.AddWithValue("@comId", comment.CompetitionID);
            cmd.Parameters.AddWithValue("@description", comment.Description);
            cmd.Parameters.AddWithValue("@datetime", comment.DateTimePosted);

            conn.Open();

            comment.CommentID = (int)cmd.ExecuteScalar();

            conn.Close();
            return comment.CommentID;
        }





    }
}
