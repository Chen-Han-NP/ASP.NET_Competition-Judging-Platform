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

        public ActionResult ViewCompetition(int? competitionId)

        {
            if (competitionId != null)
            {
                List<CompetitorSubmissionViewModel> competitorList = competitorContext.getAllCompetitor((int)competitionId);
                return View(competitorList);
            }
            else
            {
                return View();
            }

        }

        
    }
}
