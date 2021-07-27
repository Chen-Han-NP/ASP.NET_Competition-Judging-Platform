using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameTime.Models
{
    public class CompetitorScoreViewModel
    {
        public int CompetitorID { get; set; }

        public int CompetitionID { get; set; }

        public string CompetitorName { get; set; }

        public double Score { get; set; }

        [Display(Name = "Criteria 1")]
        public string C1Score { get; set; }

        [Display(Name = "Criteria 2")]
        public string C2Score { get; set; }

        [Display(Name = "Criteria 3")]
        public string C3Score { get; set; }

        [Display(Name = "Criteria 4")]
        public string C4Score { get; set; }


        public string FileSubmitted { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? DateTimeSubmitted { get; set; }

        public string Appeal { get; set; }

        [Required]
        public int VoteCount { get; set; }

        public int? Ranking { get; set; }

        public IFormFile FileUpload { get; set; }
    }
}
