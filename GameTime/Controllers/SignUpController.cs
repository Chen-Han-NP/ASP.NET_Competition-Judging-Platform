using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameTime.Models;
using GameTime.DAL;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace GameTime.Controllers
{
    public class SignUpController : Controller
    {
        private CompetitorDAL competitorContext = new CompetitorDAL();
        private JudgeDAL judgeContext = new JudgeDAL();
       
        public ActionResult Index()
        {
          
            return View("Index", "Home");
        }
        
        public ActionResult GoogleSignUp()
        {
            Competitor competitor = new Competitor();
            competitor.EmailAddr = TempData["eMail"].ToString();
            competitor.CompetitorName = TempData["userName"].ToString();
            competitor.Salutation = null; //cant be set to null because of GetAllCompetitor()
            competitor.Password = "";
            competitor.CompetitorID = competitorContext.Add(competitor);
            HttpContext.Session.SetString("CompetitorID", competitor.CompetitorID.ToString());
            HttpContext.Session.SetString("Role", "Competitor");
            return RedirectToAction("Competitor", "Home");
        }

        public ActionResult CompetitorSignUp()
        {
            Competitor competitor = new Competitor();
            return View(competitor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompetitorSignUp(Competitor competitor)
        {
            if (ModelState.IsValid)
            {
                competitor.CompetitorID = competitorContext.Add(competitor);
                HttpContext.Session.SetString("CompetitorID", competitor.CompetitorID.ToString());
                HttpContext.Session.SetString("Role", "Competitor");
                return RedirectToAction("Competitor", "Home");
            }
            else
            {
                return View(competitor);
            }
        }

        public ActionResult JudgeSignUp()
        {
            Judge judge = new Judge();
            ViewData["GetAOI"] = GetAOI();
            ViewData["GetJudgeID"] = GetJudgeID();
            return View(judge);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult JudgeSignUp(Judge judge)
        {
            if (ModelState.IsValid)
            {
                judge.JudgeID = judgeContext.Add(judge);
                HttpContext.Session.SetInt32("JudgeID", judge.JudgeID);
                HttpContext.Session.SetString("Role", "Judge");
                return RedirectToAction("Index", "Judge");
            }
            else
            {
                ViewData["GetAOI"] = GetAOI();
                ViewData["GetJudgeID"] = GetJudgeID();
                return View(judge);
            }
        }

        private List<SelectListItem> GetAOI()
        {
            AreaOfInterestDAL AOIcontext = new AreaOfInterestDAL();
            List<AreaOfInterest> AOI = AOIcontext.GetAreaOfInterests();
            List<SelectListItem> SelectList = new List<SelectListItem>();

            SelectList.Add(new SelectListItem
            {
                Value = "default",
                Text = "-- select option --",
                Selected = true
            });

            for (int i = 0; i < AOI.Count(); i++)
            {
                SelectList.Add(new SelectListItem
                {
                    Value = (i+1).ToString(),
                    Text = AOI[i].Name.ToString(),
                    Selected = false
                });
            }
            ViewData["AOIList"] = SelectList;
            return SelectList;
        }

        private int GetJudgeID()
        {
            JudgeDAL JudgeContext = new JudgeDAL();
            List<Judge> JudgeList = JudgeContext.GetAllJudge();
            for(int i = 0; i<JudgeList.Count(); i++) // Check if there is space and fills in ID so that judge ID will be filled and not empty
            {
                if (JudgeList[i].JudgeID == i+1)
                {
                    continue;
                } 
                else
                {
                    return i;
                }
            }
            return JudgeList.Count()+1;
        }
    }
}
