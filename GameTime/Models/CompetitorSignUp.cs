using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameTime.Models
{
    public class CompetitorSignUp
    {
        [Required]
        public int CompetitorID { get; set; }

        [Required]
        [Display(Name = "Your Name:")]
        [StringLength(50, ErrorMessage = "Cannot Exceed 50 characters.")]
        public string CompetitorName { get; set; }

        [Required]
        [Display(Name = "Salutation:")]
        public string Salutation { get; set; }

        [Required]
        [Display(Name = "Email Address:")]
        [EmailAddress]
        public string EmailAddr { get; set; }

        
        [Required]
        [Display(Name = "Password:")]
        public string Password { get; set; }
    }
}
