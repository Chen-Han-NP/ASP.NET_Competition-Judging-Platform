using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameTime.Models
{
    public class Criteria
    {
        [Required]
        [Display(Name = "Criteria ID")]
        public int CriteriaID { get; set; }

        [Required]
        [Display(Name = "Competition ID")]
        public int CompetitionID { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Character name can be up to 50 characters.")]
        [Display(Name = "Criteria Name")]
        public string CriteriaName { get; set; }

        [Required]
        [Range(1,100, ErrorMessage = "Weightage can only be from 1 to 100")]
        public int Weightage { get; set; }

    }
}
