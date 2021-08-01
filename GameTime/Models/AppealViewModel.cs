using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GameTime.Models
{
    public class AppealViewModel
    {
        [Display(Name = "Competition ID")]
        public int competitionID { get; set; }

        [Display(Name = "Competitor ID")]
        public int competitorID { get; set; }

        [Display(Name = "Content")]
        public string content { get; set; }
    }
}
