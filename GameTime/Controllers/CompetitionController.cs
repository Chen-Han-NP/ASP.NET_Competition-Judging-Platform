using GameTime.DAL;
using GameTime.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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
                TempData["NotAdmin"] = "YOU IMPOSTOR, YOU AINT A ADMIN";
                return RedirectToAction("ErrorPage");
            }
            List<Competition> compList = compContext.GetAllComp();
            return View(compList);
        }
        public ActionResult ErrorPage()
        {
            ViewData["Error"] = TempData["Error"].ToString();
            return View();
        }
        
        

        //create form default page
        public ActionResult Createcomp()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                TempData["Error"] = "You are not authorised to enter this page.";
                return RedirectToAction("ErrorPage");
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
        [ValidateAntiForgeryToken]
        public ActionResult Createcomp(Competition comp)
        {
            ValidateCompetitionDate validateCompetitionDate = new ValidateCompetitionDate();
            // The aoi object contains user inputs from view
            //if (ModelState.IsValid)
            //{
            //    //Add staff record to database
            //    comp.CompetitionID = compContext.AddComp(comp);
            //    //Redirect user to Staff/Index view
            //    return RedirectToAction("Index");
            //}
            //else if (!validateCompetitionDate.IsValid(comp))
            //{
            //    List<AreaOfInterest> aoiList = aoiContext.GetAreaOfInterests();
            //    for (int i = 0; i < aoiList.Count; i++)
            //    {
            //        sList.Add(
            //        new SelectListItem
            //        {
            //            Value = aoiList[i].AreaInterestID.ToString(),
            //            Text = aoiList[i].Name.ToString(),
            //        });
            //    }
            //    //Input validation fails, return to the Create view
            //    //to display error message
            //    ViewData["ShowResult"] = false;
            //    ViewData["aoiList"] = sList;
            //    return View(comp);
            //}


            if (validateCompetitionDate.GetValidationResult(comp, new ValidationContext(comp)) != ValidationResult.Success)
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
            else if (ModelState.IsValid)
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
                TempData["Error"] = "You are not authorised to enter this page.";
                return RedirectToAction("ErrorPage", "Competition");
            }
            Competition comp = compContext.GetDetails(id.Value);
            if (id == null || comp.CompetitionID == 0)
            { //Query string parameter not provided
              //Return to listing page, not allowed to edit
                TempData["Error"] = "Invalid Competition ID";
                return RedirectToAction("ErrorPage");
            }
            //ViewData["BranchList"] = GetAllBranches();

            
            int countCompetitors = competitorContext.getAllCompetitor(id.Value).Count();
            if (countCompetitors != 0)
            {
                TempData["Error"] = "You are not allowed to delete the competition, As there are already competitors in it.";
                //Return to listing page, not allowed to edit
                return RedirectToAction("ErrorPage");
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
                TempData["Error"] = "You are not authorised to enter this page.";
                return RedirectToAction("ErrorPage", "Competition");
            }
            Competition comp = compContext.GetDetails(id.Value);
            if (id == null || comp.CompetitionID == 0)
            { //Query string parameter not provided
              //Return to listing page, not allowed to edit
                TempData["Error"] = "Invalid Competition ID";
                return RedirectToAction("ErrorPage");
            }
            
            int countCompetitors = competitorContext.getAllCompetitor(id.Value).Count();
            if (comp == null || countCompetitors != 0 || compContext.getJudgesinCompetition(id.Value).Count != 0)
            {
                TempData["Error"] = "Not allowed to delete competition. Competitors and Judges have already joined.";
                return RedirectToAction("ErrorPage");
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

        //List<Judge> judgeList = judgeContext.GetAllJudge();
        public ActionResult AddJudge(int? id)
        {
            List<Judge> judgeList = judgeContext.GetAllJudge();
            // Stop accessing the action if not logged in
            // or account not in the "Administrator" role
            // ...need to do 
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                TempData["Error"] = "You are not authorised to enter this page.";
                return RedirectToAction("ErrorPage", "Competition");
            }
            Competition comp = compContext.GetDetails(id.Value);
            if (id == null || comp.CompetitionID == 0)
            { //Query string parameter not provided
              //Return to listing page, not allowed to edit
                TempData["Error"] = "Invalid Competition ID";
                return RedirectToAction("ErrorPage"); 
            }
           
            if(DateTime.Now > comp.EndDate)
            {
                TempData["Error"] = "Not allowed to add judges. Competition has ended.";
                return RedirectToAction("ErrorPage");
            }

           
            for (int i = 0; i < judgeList.Count; i++)
            {
                if (judgeList[i].AreaInterestID == comp.AreaInterestID)
                {
                   
                    if (compContext.getJudgeCompetition(judgeList[i].JudgeID) == 0)
                    {
                        jList.Add(
                        new SelectListItem
                        {
                            Value = judgeList[i].JudgeID.ToString(),
                            Text = judgeList[i].JudgeName.ToString(),
                        });
                    }
                    if (jList.Count() == 0)
                    {
                        ViewData["noJudgesAvailable"] = "No Judges is available.";
                    }
                    else
                    {
                        ViewData["noJudgesAvailable"] = "";
                    }

                }

            }

            ViewData["ShowResult"] = false;
            ViewData["judgeList"] = jList;
            CompetitionJudge compjudge = new CompetitionJudge();
            compjudge.CompetitionID = (int)id;

            return View(compjudge);

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddJudge(CompetitionJudge compJudge)
        {
            //List<CompetitionJudge> selectedJudges = compContext.getJudges(compJudge.CompetitionID);
           

            if (ModelState.IsValid)
            {
                compContext.AddJudge(compJudge);
                return RedirectToAction("Index");
            }
            else
            {
                List<Judge> judgeList = judgeContext.GetAllJudge();
                Competition comp = compContext.GetDetails(compJudge.CompetitionID);

                for (int i = 0; i < judgeList.Count; i++)
                {
                   
                    if (judgeList[i].AreaInterestID == comp.AreaInterestID && compContext.getJudgeCompetition(judgeList[i].JudgeID) == 0)
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
                //Input validation fails, return to the Create view
                //to display error message
                return View(compJudge);
            }
        }

        public ActionResult RemoveJudge(int? id) 
        {
            List<Judge> judgeList = judgeContext.GetAllJudge();
            // Stop accessing the action if not logged in
            // or account not in the "Administrator" role
            // ...need to do 
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                TempData["Error"] = "You are not authorised to enter this page.";
                return RedirectToAction("ErrorPage", "Competition");
            }
            Competition comp = compContext.GetDetails(id.Value);
            if (id == null || comp.CompetitionID == 0)
            { //Query string parameter not provided
              //Return to listing page, not allowed to edit
                TempData["ErrorID"] = "Invalid Competition ID";
                return RedirectToAction("ErrorPage");
            }
            
            if (DateTime.Now > comp.EndDate)
            {
                TempData["ErrorCompetitionStart"] = "Not allowed to remove judges. Competition has ended.";
                return RedirectToAction("ErrorPage");
            }

            List<int> judgeidList = compContext.getJudgesinCompetition(id.Value);
            if (judgeidList.Count == 0)
            { //Query string parameter not provided
              //Return to listing page, not allowed to edit
                TempData["ErrorNoJudges"] = "There are no judges to remove.";
                return RedirectToAction("ErrorPage");
            }
            for (int i = 0; i < judgeidList.Count; i++)
            {
                   jList.Add(
                new SelectListItem
                {
                    Value = judgeidList[i].ToString(),
                    Text = judgeContext.GetJudge(judgeidList[i]).JudgeName,
                });
               
            }
            ViewData["ShowResult"] = false;
            ViewData["jidList"] = jList;
            CompetitionJudge compjudge = new CompetitionJudge();
            compjudge.CompetitionID = (int)id;
            return View(compjudge);
        }
        [HttpPost]
        public ActionResult RemoveJudge(CompetitionJudge competitionJudge)
        {
            compContext.RemoveJudge(competitionJudge);
            return RedirectToAction("Index");
        }
    }
}
