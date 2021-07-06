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

            return View(competitionList);
        }

        public ActionResult ViewCompetition(int? competitionId, string competitionName = "")

        {
            List<CompetitorSubmissionViewModel> competitorList = competitorContext.getAllCompetitor((int)competitionId);
            ViewData["CompetitionName"] = competitionName;
            return View(competitorList);

        }

        public ActionResult Vote(int? competitorId, int? competitionId)
        {

            return RedirectToAction("ViewCompetition", new { competitionId = (int)competitionId});
        }

        
    }
}
