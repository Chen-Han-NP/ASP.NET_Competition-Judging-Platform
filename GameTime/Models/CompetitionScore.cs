using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameTime.Models
{
    public class CompetitionScore
    {
        [Required]
        public int CriteriaID { get; set; }

        [Required]
        public int CompetitorID { get; set; }

        [Required]
        public int CompetitionID { get; set; }

        [Required]
        [Range(0,10, ErrorMessage = "Score can only range from 0 to 10")]
        public int Score { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? DateTimeLastEdit { get; set; }
    }
}
