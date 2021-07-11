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
                criteria.CriteriaID = criteriaContext.Add(criteria);

                return RedirectToAction("Index", "Judge");
            }
            else
            {
                return View(criteria);
            }
        }
    }
}
