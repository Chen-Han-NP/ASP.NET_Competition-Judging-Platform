using GameTime.DAL;
using GameTime.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameTime.Controllers
{
    public class JudgeController : Controller
    {
        JudgeDAL judgeContext = new JudgeDAL();
        CriteriaDAL criteriaContext = new CriteriaDAL();

        // GET: JudgeController
        public ActionResult Index(Judge judge)
        {
            return View(judge);
        }

        // GET: JudgeController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        public ActionResult CreateCriteria()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCriteria(Criteria criteria)
        {
            if (ModelState.IsValid)
            {
                if (criteriaContext.GetTotalCriteria(criteria.CompetitionID) >= 100) // Check if competition already have 100% weightage
                {
                    ViewBag.Error = "Criteria already has 100% weightage";
                    return View();
                }
                else if (criteria.Weightage + criteriaContext.GetTotalCriteria(criteria.CompetitionID) > 100) // Check if total weightage exceeds 100%
                {
                    ViewBag.Error = "Criteria exceeds 100% weightage";
                    return View();
                }

                criteria.CriteriaID = criteriaContext.Add(criteria);

                if (criteria.CriteriaID == 0)
                {
                    ViewBag.Error = "Competition ID does not exist";
                    return View();
                }

                return RedirectToAction("Index", "Judge");
            }
            else
            {
                return View(criteria);
            }
        }
    }
}
