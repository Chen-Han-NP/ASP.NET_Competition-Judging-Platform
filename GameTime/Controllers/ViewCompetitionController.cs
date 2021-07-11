using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameTime.DAL;
using GameTime.Models;

namespace GameTime.Controllers
{
    public class ViewCompetitionController : Controller
    {

        private ViewCompetitionDAL competitionContext = new ViewCompetitionDAL();
        private CompetitorSubmissionDAL competitorContext = new CompetitorSubmissionDAL();

        public ActionResult Index()
        {
            List<CompetitionViewModel> competitionList = new List<CompetitionViewModel>();

            competitionList = competitionContext.GetAllCompetitions();
            ViewData["SuccessfulSearch"] = true;

            return View(competitionList);
        }

        [HttpPost]
        public ActionResult Index(IFormCollection formData)
        {
            string competitionName = formData["CompetitionName"];
            List<CompetitionViewModel> competitionList = new List<CompetitionViewModel>();
            competitionList = competitionContext.GetAllCompetitions();

            foreach (CompetitionViewModel competition in competitionList)
            {
                if (competition.CompetitionName == competitionName)
                {
                    return RedirectToAction("ViewCompetition", new { competitionId = (int)competition.CompetitionID });
                }
            }
            ViewData["SuccessfulSearch"] = false;
            return View(competitionList);
        }


        public ActionResult CriteriaView(int? CompetitionID)
        {
            List<Criteria> criteriaList = new List<Criteria>();
            List<Criteria> showCriteriaList = new List<Criteria>();
            criteriaList = competitionContext.GetAllCriteria();
            for (int i = 0; i < criteriaList.Count(); i++)
            {
               if (criteriaList[i].CompetitionID == CompetitionID)
                {
                    //criteriaVM.CriteriaList.Add(criteriaList[i]);
                    //criteriaVM = new Criteria
                    //{
                    //    CriteriaID = criteriaList[i].CriteriaID,
                    //    CompetitionID = criteriaList[i].CompetitionID,
                    //    CriteriaName = criteriaList[i].CriteriaName,
                    //    Weightage = criteriaList[i].Weightage
                    //};
                    showCriteriaList.Add(criteriaList[i]);
                }
            }
            return View(showCriteriaList);
        }

        public ActionResult JoinCompetition()
        {
            return RedirectToAction("CompetitorViewCompetition");
        }

        public ActionResult ViewCompetition(int? competitionId)
        {

            //mapping competition details, competitor details and comment sections together
            CompetitionCommentViewModel competitionCommentVM = new CompetitionCommentViewModel();
            competitionCommentVM.competitorList = competitorContext.getAllCompetitor((int)competitionId);
            competitionCommentVM.commentList = competitionContext.getAllComments((int)competitionId);
            competitionCommentVM.competition = competitionContext.getCompetitionDetails((int)competitionId);

            //check whether the competition is over or not
            if (DateTime.Compare((DateTime)competitionCommentVM.competition.ResultReleasedDate, DateTime.Now ) > 0 )
            {
                ViewData["Status"] = "on-going";

            }
            else
            {
                ViewData["Status"] = "over";
                //If its over, sort the competitionList according to the ranking, and only pass the first 3 values to the view.
                List<CompetitorSubmissionViewModel> sortedCompetitor = competitionCommentVM.competitorList.OrderBy(x => x.Ranking).ToList();
                competitionCommentVM.competitorList = sortedCompetitor;
            }
            
            //For voting checks
            string sessionCompetitionId = "competition" + competitionId.ToString();

            if (HttpContext.Session.GetString(sessionCompetitionId) == null)
            {
                TempData["hasVoted"] = "false";
                
            }
            else
            {
                TempData["hasVoted"] = "true";
                TempData["votedTo"] = HttpContext.Session.GetString(sessionCompetitionId);
            }



            
            return View(competitionCommentVM);

        }


        public ActionResult CompetitorViewCompetition()
        {
            List<CompetitionViewModel> competitionList = new List<CompetitionViewModel>();

            competitionList = competitionContext.GetAllCompetitions();
            return View(competitionList);
        }


        public ActionResult Vote(int? competitorId, string competitorName, int? competitionId)
        {
            List<CompetitorSubmissionViewModel> competitorList = competitorContext.getAllCompetitor((int)competitionId);

           
            string sessionCompetitionId = "competition" + competitionId.ToString();

            //If have not voted
            if (HttpContext.Session.GetString(sessionCompetitionId) == null)
            {
                HttpContext.Session.SetString(sessionCompetitionId, competitorName);

                CompetitorSubmissionViewModel competitor = competitorList.Find(obj => obj.CompetitorId == (int)competitorId);

                competitorContext.UpdateVoteCount(competitor);
            }


            
            return RedirectToAction("ViewCompetition", new { competitionId = (int)competitionId});
        }



        [HttpPost]
        public ActionResult AddComment(IFormCollection formData)
        {
            int competitionId = Convert.ToInt32(formData["CompetitionID"]);
            DateTime dateTimePosted = Convert.ToDateTime(formData["DateTimePosted"]);
            string description = formData["Description"];

            Comment newCommend = new Comment
            {
                CompetitionID = competitionId,
                Description = description,
                DateTimePosted = dateTimePosted
            };

            newCommend.CommentID = competitionContext.AddComment(newCommend);
            return RedirectToAction("ViewCompetition", new { competitionId = (int)competitionId});
        }

        


        
    }
}
