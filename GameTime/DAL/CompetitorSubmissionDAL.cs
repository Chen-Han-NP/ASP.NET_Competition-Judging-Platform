﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using GameTime.Models;
using System.Data.SqlClient;

namespace GameTime.DAL
{
    public class CompetitorSubmissionDAL
    {
        private IConfiguration Configuration { get; set; }
        private SqlConnection conn;
        //Constructor
        public CompetitorSubmissionDAL()
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

        public List<CompetitorSubmissionViewModel> getAllCompetitor(int competitionId)
        {
            SqlCommand cmd = conn.CreateCommand();

            cmd.CommandText = @"SELECT cs.CompetitionID, cs.CompetitorID, c.CompetitorName, c.Salutation, cs.FileSubmitted, cs.DateTimeFileUpload, cs.VoteCount, cs.Ranking
FROM CompetitionSubmission cs
INNER JOIN Competitor c
ON cs.CompetitorID = c.CompetitorID
WHERE cs.CompetitionID = @comId";

            cmd.Parameters.AddWithValue("@comId", competitionId);

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            List<CompetitorSubmissionViewModel> competitorList = new List<CompetitorSubmissionViewModel>();
            while (reader.Read())
            {
                competitorList.Add(new CompetitorSubmissionViewModel
                {
                    CompetitionId = reader.GetInt32(0),
                    CompetitorId = reader.GetInt32(1),
                    CompetitorName = reader.GetString(2),
                    Salutation = !reader.IsDBNull(3) ?
                                  reader.GetString(3) : (string)null,
                    FileSubmitted = !reader.IsDBNull(4) ?
                                  reader.GetString(4) : (string)null,
                    DateTimeSubmitted = !reader.IsDBNull(5) ?
                                  reader.GetDateTime(5) : (DateTime?)null,
                    VoteCount = reader.GetInt32(6),
                    Ranking = !reader.IsDBNull(7) ?
                                  reader.GetInt32(7) : (int?)null
                });
            }
            //Close DataReader
            reader.Close();
            //Close the database connection
            conn.Close();
            return competitorList;
        }
    }
}