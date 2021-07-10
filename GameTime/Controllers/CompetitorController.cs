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
        // GET: CompetitorController
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CompetitorViewCompetition()
        {
            List<CompetitionViewModel> competitionList = new List<CompetitionViewModel>();

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

        public ActionResult JoinCompetition()
        {
            return RedirectToAction("CompetitorViewCompetition");
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
