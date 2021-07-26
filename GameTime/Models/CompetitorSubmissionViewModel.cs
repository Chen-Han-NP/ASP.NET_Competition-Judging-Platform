using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GameTime.Models
{
    
    public class CompetitorSubmissionViewModel
    {
        [Required]
        public int CompetitionId { get; set; }


        [Required]
        public int CompetitorId { get; set; }


        [StringLength(50, ErrorMessage = "Name cannot excceed 50 letters")]
        public string CompetitorName { get; set; }

        [StringLength(5, ErrorMessage = "Salutation cannot excceed 5 letters")]
        public string Salutation { get; set; }

        //Type will be changed to iFormFile
        public string FileSubmitted { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? DateTimeSubmitted { get; set; }

        public string Appeal { get; set; }

        [Required]
        public int VoteCount { get; set; }

        public int? Ranking { get; set; }

    }
}
