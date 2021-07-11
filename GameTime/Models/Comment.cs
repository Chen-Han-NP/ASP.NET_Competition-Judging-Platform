using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GameTime.Models
{
    public class Comment
    {

        public int CommentID { get; set; }

        public int CompetitionID { get; set; }

        [StringLength(255, ErrorMessage = "Your comment must not exceed 255 characters!")]
        public string Description { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateTimePosted { get; set; }
    }
}
