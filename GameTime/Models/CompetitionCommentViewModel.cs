using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameTime.Models
{
    public class CompetitionCommentViewModel
    {

        public List<CompetitorSubmissionViewModel> competitorList { get; set; }
        public List<Comment> commentList { get; set; }

        public Competition competition { get; set; }

        public Comment newComment { get; set; }

        public CompetitionCommentViewModel()
        {
            competitorList = new List<CompetitorSubmissionViewModel>();
            commentList = new List<Comment>();
            competition = new Competition();
            newComment = new Comment();
        }

    }
}
