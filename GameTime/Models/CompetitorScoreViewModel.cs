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
        [Display(Name ="Competitior ID")]
        public int CompetitorID { get; set; }

        [Display(Name ="Competition ID")]
        public int CompetitionID { get; set; }

        [Display(Name ="Competitor Name")]
        public string CompetitorName { get; set; }

        public double Score { get; set; }

        [Display(Name ="File Submitted")]
        public string FileSubmitted { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name ="Date Time Submitted")]
        public DateTime? DateTimeSubmitted { get; set; }

        public string Appeal { get; set; }

        [Required]
        [Display(Name ="Vote Count")]
        public int VoteCount { get; set; }

        public int? Ranking { get; set; }

        [Display(Name ="File Upload")]
        public IFormFile FileUpload { get; set; }
    }
}
