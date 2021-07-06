using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameTime.Models
{
    public class Competitor
    {
        [Required]
        public int CompetitorID { get; set; }

        [Required(ErrorMessage ="Please enter your name.")]
        [Display(Name = "Your Name:")]
        [StringLength(50, ErrorMessage = "Cannot Exceed 50 characters.")]
        public string CompetitorName { get; set; }

        [Required]
        [Display(Name = "Salutation:")]
        public string Salutation { get; set; }

        [Required(ErrorMessage = "Please enter your Email.")]
        [Display(Name = "Email Address:")]
        [EmailAddress]
        [ValidateEmailExists] //Custom Validation Attribute for emails that exists
        public string EmailAddr { get; set; }

        
        [Required(ErrorMessage ="Please enter a Password.")]
        [Display(Name = "Password:")]
        public string Password { get; set; }
    }
}
