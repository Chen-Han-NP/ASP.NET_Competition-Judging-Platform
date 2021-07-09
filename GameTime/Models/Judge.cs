using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameTime.Models
{
    public class Judge
    {
        [Required]
        public int JudgeID { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Cannot Exceed 50 characters.")]
        public string JudgeName { get; set; }

        [StringLength(5, ErrorMessage = "Cannot Exceed 5 characters.")]
        public string Salutation { get; set; }

        [Required]
        public int AreaInterestID { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Cannot Exceed 50 characters.")]
        [RegularExpression(@".+\@lcu\.edu\.sg", ErrorMessage = "Not Lion City University Staff")]
        public string EmailAddr { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "Cannot Exceed 255 characters.")]
        public string Password { get; set; }
    }
}
