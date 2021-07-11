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
    public class CompetitorController : Controller
    {
        private ViewCompetitionDAL competitionContext = new ViewCompetitionDAL();
        private CompetitorDAL competitorContext = new CompetitorDAL();
        private CompetitorSubmissionDAL competitorSubmissionContext = new CompetitorSubmissionDAL();

        // GET: CompetitorController
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CompetitorViewCompetition()
        {
            List<CompetitionViewModel> competitionList = new List<CompetitionViewModel>();
            if ((HttpContext.Session.GetString("Role") == null) || (HttpContext.Session.GetString("Role") != "Competitor"))
            {
                return RedirectToAction("Index", "Home");
            }
            competitionList = competitionContext.GetAllCompetitions();
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

        public ActionResult JoinCompetition(int competitionID)
        {
            CompetitorSubmissionViewModel competitorSubmission = new CompetitorSubmissionViewModel();
            Competition competition = new Competition();

            //if (DateTime.Today.AddDays(3) < competition.StartDate)
            //{

            //}


            int id = Convert.ToInt32(HttpContext.Session.GetString("CompetitorID"));
            competitorSubmission.CompetitionId = competitionID;
            competitorSubmission.CompetitorId = id;
            competitorSubmission.FileSubmitted = "replaceThis.pdf";
            competitorSubmission.DateTimeSubmitted = DateTime.Today;
            competitorSubmission.Appeal = "";
            competitorSubmission.VoteCount = 0;
            competitorSubmission.Ranking = -1;
            competitorSubmissionContext.JoinCompetition(competitorSubmission);
            return RedirectToAction("Competitor", "Home");
        }


        // GET: CompetitorController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CompetitorController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CompetitorController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CompetitorController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CompetitorController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CompetitorController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CompetitorController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
