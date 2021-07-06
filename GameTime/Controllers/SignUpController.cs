using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameTime.Models;
using GameTime.DAL;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameTime.Controllers
{
    public class SignUpController : Controller
    {
        private CompetitorDAL competitorContext = new CompetitorDAL();
        private JudgeDAL judgeContext = new JudgeDAL();
        public ActionResult Index()
        {
            return View("Index");
        }

        public ActionResult CompetitorSignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CompetitorSignup(CompetitorSignUp competitor)
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
            ViewData["GetAOI"] = GetAOI();
            return View();
        }

        [HttpPost]
        public ActionResult JudgeSignUp(JudgeSignUp judge)
        {
            if (ModelState.IsValid)
            {
                judge.JudgeID = judgeContext.Add(judge);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View(judge);
            }
        }

        private List<SelectListItem> GetAOI()
        {
            AreaOfInterestDAL AOIcontext = new AreaOfInterestDAL();
            List<AreaOfInterest> AOI = AOIcontext.GetAreaOfInterests();
            List<SelectListItem> SelectList = new List<SelectListItem>();
            for (int i = 0; i < AOI.Count(); i++)
            {
                SelectList.Add(new SelectListItem
                {
                    Value = i.ToString(),
                    Text = AOI[i].ToString()
                });
            }
            ViewData["AOIList"] = SelectList;
            return SelectList;
        }
    }
}
