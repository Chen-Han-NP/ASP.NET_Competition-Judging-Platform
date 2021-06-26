using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameTime.Models
{
    public class AreaOfInterest
    {

        [Required]
        [Display(Name = "Area Of Interest ID")]
        public int areaInterestID { get; set; }


        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, ErrorMessage = "Area of Interest name cannot exceed 50 characters.")]
        [Display(Name = "Area of Interest Name")]
        public string Name { get; set; }



    }
}
