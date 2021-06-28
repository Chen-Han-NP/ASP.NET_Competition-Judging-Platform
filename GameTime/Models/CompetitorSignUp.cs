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
        [Display(Name = "First name:")]
        [StringLength(50, ErrorMessage = "Cannot Exceed 50 characters.")]
        public string FName { get; set; }

        [Required]
        [Display(Name = "Last name:")]
        [StringLength(50, ErrorMessage = "Cannot Exceed 50 characters.")]
        public string LName { get; set; }

        [Required]
        [Display(Name = "Nickname:")]
        [StringLength(50, ErrorMessage = "Cannot Exceed 50 characters.")]
        public string Nickname { get; set; }

        [Required]
        [Display(Name = "Email Address:")]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        [Display(Name = "Password:")]
        public string Password { get; set; }
    }
}
