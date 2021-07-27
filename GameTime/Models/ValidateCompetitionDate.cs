using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameTime.Models
{
    public class ValidateCompetitionDate : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            
            Competition comp = (Competition)validationContext.ObjectInstance;

            if (comp.StartDate > comp.EndDate)
                // validation failed
                return new ValidationResult
                ("Competition Start Date cannot be later than Competition End Date.");
            else if (comp.StartDate > comp.ResultReleasedDate)
                return new ValidationResult
                    ("Competition Start Date cannot be later than Competition Results Released Date.");
            else if (comp.EndDate > comp.ResultReleasedDate)
                return new ValidationResult
                    ("Competition End Date cannot be later than Competition Results Released Date.");
            else
                // validation passed
                return ValidationResult.Success;
        }



    }
}
