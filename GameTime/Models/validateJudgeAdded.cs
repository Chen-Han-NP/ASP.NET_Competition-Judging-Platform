using GameTime.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameTime.Models
{
    public class validateJudgeAdded : ValidationAttribute
    {
        private CompetitionDAL compContext = new CompetitionDAL();
        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            // Get the email value to validate
            int judgeID = Convert.ToInt32(value);
            // Casting the validation context to the "Staff" model class
            CompetitionJudge comp = (CompetitionJudge)validationContext.ObjectInstance;
            
            // Get the Staff Id from the staff instance
            int compid = comp.CompetitionID;
            if (compContext.checkJudgeAdded(judgeID, compid))
                // validation failed
                return new ValidationResult
                ("Judge Already Added!");
            else
                // validation passed
                return ValidationResult.Success;



        }

        }
}
