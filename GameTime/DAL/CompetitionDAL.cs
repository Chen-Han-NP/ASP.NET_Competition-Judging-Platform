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

            //if (comp.StartDate == null)
            //{
            //    comp.StartDate = DateTime.Parse("01/01/1753");
            //}
            //if (comp.EndDate == null)
            //{
            //    comp.EndDate = DateTime.Parse("01/01/1753");
            //}
            //if (comp.ResultReleasedDate == null)
            //{
            //    comp.ResultReleasedDate = DateTime.Parse("01/01/1753");
            //}
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
            if (comp.StartDate == null)
            {
                cmd.Parameters.AddWithValue("@StartDate", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@StartDate", comp.StartDate);
            }
            if (comp.EndDate == null)
            {
                cmd.Parameters.AddWithValue("@EndDate", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@EndDate", comp.EndDate);
            }
            if (comp.ResultReleasedDate == null)
            {
                cmd.Parameters.AddWithValue("@ResultReleasedDate", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@ResultReleasedDate", comp.ResultReleasedDate);
            }
            //cmd.Parameters.AddWithValue("@StartDate", comp.StartDate);
            //cmd.Parameters.AddWithValue("@EndDate", comp.EndDate);
            //cmd.Parameters.AddWithValue("@ResultReleasedDate", comp.ResultReleasedDate);
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
                Competition competition = new Competition();
                competition.CompetitionID = reader.GetInt32(0);
                competition.AreaInterestID = reader.GetInt32(1);
                competition.CompetitionName = reader.GetString(2);
                if (!reader.IsDBNull(3))
                {
                    competition.StartDate = reader.GetDateTime(3); //3: 4th column
                    //competition.EndDate = reader.GetDateTime(4);
                    //competition.ResultReleasedDate = reader.GetDateTime(5);
                }
                if (!reader.IsDBNull(4))
                {
                    //competition.StartDate = reader.GetDateTime(3); //3: 4th column
                    competition.EndDate = reader.GetDateTime(4);
                    // competition.ResultReleasedDate = reader.GetDateTime(5);
                }
                if (!reader.IsDBNull(5))
                {
                    // competition.StartDate = reader.GetDateTime(3); //3: 4th column
                    // competition.EndDate = reader.GetDateTime(4);
                    competition.ResultReleasedDate = reader.GetDateTime(5);
                }

                CompetitionList.Add(competition);

                //             CompetitionList.Add(
                //new Competition
                //{
                //    CompetitionID = reader.GetInt32(0), //0: 1st column
                //             AreaInterestID = reader.GetInt32(1), //1: 2nd column
                //                                                  //Get the first character of a string
                //             CompetitionName = reader.GetString(2), //2: 3rd column


                //             StartDate = reader.GetDateTime(3), //3: 4th column
                //             EndDate = reader.GetDateTime(4),
                //    ResultReleasedDate = reader.GetDateTime(5)
                //}


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
        public Competition GetDetails(int competitionId)
        {
            Competition Competition = new Competition();
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement that
            //retrieves all attributes of a staff record.
            cmd.CommandText = @"SELECT * FROM Competition WHERE CompetitionID = @selectedCompetitionID";
            //Define the parameter used in SQL statement, value for the
            //parameter is retrieved from the method parameter “staffId”.
            cmd.Parameters.AddWithValue("@selectedCompetitionID", competitionId);
            //Open a database connection
            conn.Open();
            //Execute SELCT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                //Read the record from database
                while (reader.Read())
                {
                    // Fill staff object with values from the data reader
                    Competition.CompetitionID = competitionId;
                    Competition.AreaInterestID = reader.GetInt32(1);
                    Competition.CompetitionName = reader.GetString(2);
                    Competition.StartDate = !reader.IsDBNull(3) ?
                    reader.GetDateTime(3) : (DateTime?)null;
                    Competition.EndDate = !reader.IsDBNull(4) ?
                    reader.GetDateTime(4) : (DateTime?)null;
                    Competition.ResultReleasedDate = !reader.IsDBNull(5) ?
                    reader.GetDateTime(5) : (DateTime?)null;

                }
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return Competition;
        }

        public int Update(Competition competition)
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an UPDATE SQL statement
            cmd.CommandText = @"UPDATE Competition SET AreaInterestID = @AreaInterestID, CompetitionName=@CompetitionName, StartDate=@StartDate, EndDate=@EndDate, ResultReleasedDate=@ResultReleasedDate WHERE CompetitionID = @selectedCompetitionID";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@AreaInterestID", competition.AreaInterestID);
            cmd.Parameters.AddWithValue("@CompetitionName", competition.CompetitionName);
            if (competition.StartDate == null)
            {
                cmd.Parameters.AddWithValue("@StartDate", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@StartDate", competition.StartDate);
            }
            if (competition.EndDate == null)
            {
                cmd.Parameters.AddWithValue("@EndDate", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@EndDate", competition.EndDate);
            }
            if (competition.ResultReleasedDate == null)
            {
                cmd.Parameters.AddWithValue("@ResultReleasedDate", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@ResultReleasedDate", competition.ResultReleasedDate);
            }

            cmd.Parameters.AddWithValue("@selectedCompetitionID", competition.CompetitionID);
            //Open a database connection
            conn.Open();
            //ExecuteNonQuery is used for UPDATE and DELETE
            cmd.ExecuteNonQuery();
            int count = 0;
            //Close the database connection
            conn.Close();
            return count;
        }

        public int Delete(Competition competition)
        {

            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM Competition WHERE CompetitionID = @selectCompetitionID";
            cmd.Parameters.AddWithValue("@selectCompetitionID", competition.CompetitionID);
            //Open a database connection
            conn.Open();
            int rowAffected;
            //Execute the DELETE SQL to remove the staff record
            rowAffected = cmd.ExecuteNonQuery();
            //Close database connection
            conn.Close();
            //Return number of row of staff record updated or deleted
            return rowAffected;
        }


        public void AddJudge(CompetitionJudge judge)
        {

            SqlCommand cmd = conn.CreateCommand();
            //Specify an INSERT SQL statement which will
            //return the auto-generated StaffID after insertion
            cmd.CommandText = @"INSERT INTO CompetitionJudge (CompetitionID, JudgeID)
VALUES(@CompetitionID, @JudgeID)";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@CompetitionID", judge.CompetitionID);
            cmd.Parameters.AddWithValue("@JudgeID", judge.JudgeID);
            //conn.Close();
            conn.Open();
            //ExecuteScalar is used to retrieve the auto-generated
            //StaffID after executing the INSERT SQL statement
            cmd.ExecuteScalar();
            //A connection should be closed after operations.
            conn.Close();
            //Return id when no error occurs.

        }

        public bool checkJudgeAdded(int Judgeid, int competitionid)
        {
            bool judgeFound = false;
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement that
            //retrieves all attributes of a staff record.
            cmd.CommandText = @"SELECT JudgeID FROM CompetitionJudge WHERE CompetitionID = @selectedCompetitionID AND JudgeID = @selectedJudgeID";
            //Define the parameter used in SQL statement, value for the
            //parameter is retrieved from the method parameter “staffId”.
            cmd.Parameters.AddWithValue("@selectedCompetitionID", competitionid);
            cmd.Parameters.AddWithValue("@selectedJudgeID", Judgeid);
            //Open a database connection
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            { //Records found
                while (reader.Read())
                {
                    if (reader.GetInt32(0) == Judgeid)
                        //The email address is used by another staff
                        judgeFound = true;
                }
            }
            else
            { //No record
                judgeFound = false; // The email address given does not exist
            }
            reader.Close();
            conn.Close();

            return judgeFound;

        }
    }
}
