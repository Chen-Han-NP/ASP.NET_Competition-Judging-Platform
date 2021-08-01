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
    public class AreaOfInterestDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;
        //Constructor
        public AreaOfInterestDAL()
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

        public int AddAOI(AreaOfInterest aoi)
        {
            
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an INSERT SQL statement which will
            //return the auto-generated AOIID after insertion
            cmd.CommandText = @"INSERT INTO AreaInterest (Name)
OUTPUT INSERTED.AreaInterestID
VALUES(@name)";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@name", aoi.Name);
            //A connection to database must be opened before any operations made.
            conn.Open();
            //ExecuteScalar is used to retrieve the auto-generated
            //AOIID after executing the INSERT SQL statement
            aoi.AreaInterestID = (int)cmd.ExecuteScalar();
            
            //A connection should be closed after operations.
            conn.Close();
            //Return id when no error occurs.
            return aoi.AreaInterestID;
        }

        
        public List<AreaOfInterest> GetAreaOfInterests()
        {
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement
            cmd.CommandText = @"SELECT * FROM AreaInterest ORDER BY AreaInterestID";
            //Open a database connection
            conn.Open();
            //Execute the SELECT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            //Read all records until the end, save data into  interest list
            List<AreaOfInterest> aiList = new List<AreaOfInterest>();
            while (reader.Read())
            {
                aiList.Add(
                new AreaOfInterest
                {
                    AreaInterestID = reader.GetInt32(0), //0: 1st column
                    Name = reader.GetString(1), //1: 2nd column
                                                
                }
                );
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return aiList;
        }
        public int Delete(AreaOfInterest areaOfInterest)
        {

            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandText = @"DELETE FROM AreaInterest WHERE AreaInterestID = @selectAreaInterestID";
            cmd.Parameters.AddWithValue("@selectAreaInterestID", areaOfInterest.AreaInterestID);
            //Open a database connection
            conn.Open();
            int rowAffected;
            //Execute the DELETE SQL to remove the interest record
            rowAffected = cmd.ExecuteNonQuery();
            //Close database connection
            conn.Close();
            //Return number of row of interest record updated or deleted
            return rowAffected;
        }

        public int GetCompetitorCount(int aoiId)
        {
            //Competition Competition = new Competition();
            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify the SELECT SQL statement that retrieves competitions matching the selected area of interest 
            cmd.CommandText = @"SELECT * FROM Competition WHERE AreaInterestID = @selectedaoiId";
            cmd.Parameters.AddWithValue("@selectedaoiId", aoiId);
            //Open a database connection
            conn.Open();
            //Execute SELCT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            List<Competition> compList = new List<Competition>();
            if (reader.HasRows)
            {
                //Read the record from database
                while (reader.Read())
                {
                    Competition Competition = new Competition();
                    // Fill competition object with values from the data reader
                    // if date is null, leave it as null
                    Competition.CompetitionID = aoiId;
                    Competition.AreaInterestID = reader.GetInt32(1);
                    Competition.CompetitionName = reader.GetString(2);
                    Competition.StartDate = !reader.IsDBNull(3) ?
                    reader.GetDateTime(3) : (DateTime?)null;
                    Competition.EndDate = !reader.IsDBNull(4) ?
                    reader.GetDateTime(4) : (DateTime?)null;
                    Competition.ResultReleasedDate = !reader.IsDBNull(5) ?
                    reader.GetDateTime(5) : (DateTime?)null;
                    compList.Add(Competition);

                }
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return compList.Count;
        }

        public AreaOfInterest GetDetails(int aoiID)
        {

            //Create a SqlCommand object from connection object
            SqlCommand cmd = conn.CreateCommand();
            //Specify an INSERT SQL statement which will
            cmd.CommandText = @"SELECT * FROM AreaInterest WHERE AreaInterestID = @selectredaoiid";
            //Define the parameters used in SQL statement, value for each parameter
            //is retrieved from respective class's property.
            cmd.Parameters.AddWithValue("@selectredaoiid", aoiID);
            //Open a database connection
            conn.Open();
            //Execute SELCT SQL through a DataReader
            SqlDataReader reader = cmd.ExecuteReader();
            AreaOfInterest aoi = new AreaOfInterest();
            if (reader.HasRows)
            {
                //Read the record from database
                while (reader.Read())
                {
                    // Fill aoi object with values from the data reader
                    aoi.AreaInterestID = aoiID;
                    aoi.Name = reader.GetString(1);
                    
                }
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return aoi;
        }

    }
}
