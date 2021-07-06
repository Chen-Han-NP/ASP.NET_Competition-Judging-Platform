﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using GameTime.DAL;

namespace GameTime.Models
{
    public class ValidateEmailExists : ValidationAttribute
    {
        private CompetitorDAL competitorContext = new CompetitorDAL();
        private JudgeDAL judgeContext = new JudgeDAL();
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string email = Convert.ToString(value);
            CompetitorSignUp competitor = (CompetitorSignUp)validationContext.ObjectInstance;
            int competitorID = competitor.CompetitorID;
            JudgeSignUp judge = (JudgeSignUp)validationContext.ObjectInstance;
            int judgeID = judge.JudgeID;

            if (competitorContext.isEmailExists(email, competitorID))
                // validation failed
                return new ValidationResult
                ("Email address already exists!");
            else if (judgeContext.isEmailExists(email, judgeID))
                // validation failed
                return new ValidationResult
                ("Email address already exists!");
            else
                // validation passed
                return ValidationResult.Success;
        }
    }
}