using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameTime.Models
{
    public class Competition
    {
        [Required]
        [Display(Name = "Competition ID")]
        public int CompetitionID { get; set; }

        [Required(ErrorMessage = "Area of Interest ID is required.")]
        [Display(Name = "Area Of Interest ID")]
        public int AreaInterestID { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(255, ErrorMessage = "Competition name cannot exceed 255 characters.")]
        [Display(Name = "Competition Name")]
        public string CompetitionName { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Result Released Date")]
        [DataType(DataType.Date)]
        public DateTime ResultReleasedDate { get; set; }

    }
}
