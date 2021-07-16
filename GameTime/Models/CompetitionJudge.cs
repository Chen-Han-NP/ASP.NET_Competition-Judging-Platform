using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameTime.Models
{
    public class CompetitionJudge
    {
        [Required]
        [validateJudgeAdded]
        public int JudgeID { get; set; }

        [Required]
        public int CompetitionID { get; set; }
    }
}
