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
        [Display(Name ="Criteria ID")]
        public int CriteriaID { get; set; }

        [Required]
        [Display(Name = "Competitor ID")]
        public int CompetitorID { get; set; }

        [Required]
        [Display(Name = "Competition ID")]
        public int CompetitionID { get; set; }

        [Required]
        [Range(0,10, ErrorMessage = "Score can only range from 0 to 10")]
        public int Score { get; set; }

        [Display(Name = "Criteria Name")]
        public String CriteriaName { get; set; }

        public int? Weightage { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? DateTimeLastEdit { get; set; }
    }
}
