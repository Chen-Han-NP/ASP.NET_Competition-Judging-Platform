using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using GameTime.Models;
using GameTime.DAL;
using Microsoft.AspNetCore.Http;

namespace GameTime.Controllers
{
    public class CompetitionController : Controller
    {
        private CompetitionDAL compContext = new CompetitionDAL();
        private AreaOfInterestDAL aoiContext = new AreaOfInterestDAL();
        private CompetitorSubmissionDAL competitorContext = new CompetitorSubmissionDAL();
        private List<SelectListItem> sList = new List<SelectListItem>();
        private List<SelectListItem> jList = new List<SelectListItem>();
        JudgeDAL judgeContext = new JudgeDAL();
        public ActionResult Index()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            List<Competition> compList = compContext.GetAllComp();
            return View(compList);
        }
        //create form default page
        public ActionResult Createcomp()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            List<AreaOfInterest> aoiList = aoiContext.GetAreaOfInterests();
            for (int i = 0; i < aoiList.Count; i++)
            {
                sList.Add(
                new SelectListItem
                {
                    Value = aoiList[i].AreaInterestID.ToString(),
                    Text = aoiList[i].Name.ToString(),
                });
            }

            ViewData["ShowResult"] = false;
            ViewData["aoiList"] = sList;

            //create Area of Interest object
            Competition comp = new Competition();

            return View(comp);
        }

        [HttpPost]
        public ActionResult Createcomp(Competition comp)
        {
            // The aoi object contains user inputs from view
            if (ModelState.IsValid)
            {
                //Add staff record to database
                comp.CompetitionID = compContext.AddComp(comp);
                //Redirect user to Staff/Index view
                return RedirectToAction("Index");
            }
            else
            {
                List<AreaOfInterest> aoiList = aoiContext.GetAreaOfInterests();
                for (int i = 0; i < aoiList.Count; i++)
                {
                    sList.Add(
                    new SelectListItem
                    {
                        Value = aoiList[i].AreaInterestID.ToString(),
                        Text = aoiList[i].Name.ToString(),
                    });
                }
                //Input validation fails, return to the Create view
                //to display error message
                ViewData["ShowResult"] = false;
                ViewData["aoiList"] = sList;
                return View(comp);
            }



        }

        public ActionResult Update(int? id)
        {
            // Stop accessing the action if not logged in
            // or account not in the "Staff" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            { //Query string parameter not provided
              //Return to listing page, not allowed to edit
                return RedirectToAction("Index");
            }
            //ViewData["BranchList"] = GetAllBranches();
            
            Competition comp = compContext.GetDetails(id.Value);
            int countCompetitors = competitorContext.getAllCompetitor(id.Value).Count();
            if (comp == null || countCompetitors != 0)
            {
                //Return to listing page, not allowed to edit
                return RedirectToAction("Index");
            }
            return View(comp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(Competition competition)
        {
            //Get branch list for drop-down list
            //in case of the need to return to Edit.cshtml view
            //ViewData["BranchList"] = GetAllBranches();
            if (ModelState.IsValid)
            {
                //Update staff record to database
                compContext.Update(competition);
                return RedirectToAction("Index");
            }
            else
            {
                //Input validation fails, return to the view
                //to display error message
                return View(competition);
            }
        }

        // GET: StaffController/Delete/5
        public ActionResult Delete(int? id)
        {
            // Stop accessing the action if not logged in
            // or account not in the "Staff" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            { //Query string parameter not provided
              //Return to listing page, not allowed to edit
                return RedirectToAction("Index");
            }
            Competition comp = compContext.GetDetails(id.Value);
            int countCompetitors = competitorContext.getAllCompetitor(id.Value).Count();
            if (comp == null || countCompetitors != 0)
            {
                //Return to listing page, not allowed to edit
                return RedirectToAction("Index");
            }
            return View(comp);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(Competition competition)
        {
            // Delete the staff record from database
            compContext.Delete(competition);
            return RedirectToAction("Index");
        }


        public ActionResult AddJudge(int? id)
        {
            List<Judge> judgeList = judgeContext.GetAllJudge();
            // Stop accessing the action if not logged in
            // or account not in the "Administrator" role
            // ...need to do 
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            { //Query string parameter not provided
              //Return to listing page, not allowed to edit
                return RedirectToAction("Index");
            }
            Competition comp = compContext.GetDetails(id.Value);
            if(DateTime.Now > comp.EndDate)
            {
                return RedirectToAction("Index");
            }

           
            for (int i = 0; i < judgeList.Count; i++)
            {
                if (judgeList[i].AreaInterestID == comp.AreaInterestID)
                {
                    jList.Add(
                new SelectListItem
                {
                    Value = judgeList[i].JudgeID.ToString(),
                    Text = judgeList[i].JudgeName.ToString(),
                });
                }

            }

            ViewData["ShowResult"] = false;
            ViewData["judgeList"] = jList;
            CompetitionJudge compjudge = new CompetitionJudge();
            compjudge.CompetitionID = (int)id;

            return View(compjudge);

        }



        [HttpPost]
        public ActionResult AddJudge(CompetitionJudge compJudge)
        {
            List<CompetitionJudge> selectedJudges = compContext.getJudges(compJudge.CompetitionID);
           
            if (selectedJudges.Count == 0)
            {
                compContext.AddJudge(compJudge);
                return RedirectToAction("Index");

            }
            else
            {
                //do validation
                //for (int i = 0; i < selectedJudges.Count; i++)
                //{
                //    if (selectedJudges[i].JudgeID == compJudge.JudgeID)
                //    {
                //        return RedirectToAction("Index");
                       
                //    }
                //    else
                //    {
                //        compContext.AddJudge(compJudge);
                //        break;
                //    }

                //}
            }


            return RedirectToAction("Index");
        }


    }
}
