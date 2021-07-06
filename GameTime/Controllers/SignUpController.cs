using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameTime.Models;
using GameTime.DAL;

namespace GameTime.Controllers
{
    public class SignUpController : Controller
    {
        private CompetitorDAL competitorContext = new CompetitorDAL();
        public ActionResult Index()
        {
            return View("Index");
        }

        public ActionResult CompetitorSignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateCompetitor(CompetitorSignUp competitor)
        {
            if (ModelState.IsValid)
            {
                competitor.CompetitorID = competitorContext.Add(competitor);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View(competitor);
            }
        }
        public ActionResult JudgeSignUp()
        {
            return View();
        }
    }
}
