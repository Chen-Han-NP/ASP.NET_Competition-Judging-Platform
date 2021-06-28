using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameTime.Models
{
    public class Login
    {
        [Required(ErrorMessage = "Email is required.")]
        [Display(Name = "Email Address:")]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [Display(Name = "Password:")]
        public string Password { get; set; }
    }
}
