using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GameTime.Models
{
    public class CriteriaScoreAppealViewModel
    {
        

        public int CriteriaID { get; set; }

        public string CriteriaName { get; set; }

        public int Weightage { get; set; }

        public int Score { get; set; }

        public string Appeal { get; set; }

    }
}
