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
            //check if user is an authorised admin from the login page
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                TempData["Error"] = "You are not authorised to enter this page.";
                return RedirectToAction("ErrorPage");
            }
            //display list of competitions
            List<Competition> compList = compContext.GetAllComp();
            return View(compList);
        }
        public ActionResult ErrorPage()
        {
            //return an error page with error message when user is not allowed to execute an certain action.
            ViewData["Error"] = TempData["Error"].ToString();
            return View();
        }
        
        

        //create form default page
        public ActionResult Createcomp()
        {
            //check if user is an authorised admin from the login page
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                TempData["Error"] = "You are not authorised to enter this page.";
                return RedirectToAction("ErrorPage");
            }
            //get list of interests and add them to selectlistitem list for the dropdown list in view
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
            ViewData["aoiList"] = sList;

            //create competition object
            Competition comp = new Competition();

            return View(comp);
        }
        
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Createcomp(Competition comp)
        {
            //execute validate competition date to check if competition date is later than start or end date 
            ValidateCompetitionDate validateCompetitionDate = new ValidateCompetitionDate();
            if (validateCompetitionDate.GetValidationResult(comp, new ValidationContext(comp)) != ValidationResult.Success)
            {
                //if validation is not successful bring user back with the interests list and competition object
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
                ViewData["aoiList"] = sList;
                return View(comp);
            }
            else if (ModelState.IsValid)
            {
                //if validation successful 
                //Add competition record to database
                comp.CompetitionID = compContext.AddComp(comp);
                //Redirect user to index
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
            // or account not in the "Admin" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                TempData["Error"] = "You are not authorised to enter this page.";
                return RedirectToAction("ErrorPage", "Competition");
            }
            //get competition object from the competition ID
            Competition comp = compContext.GetDetails(id.Value);

            //if ID is null or competitionID = 0 return to error page
            if (id == null || comp.CompetitionID == 0)
            { 
                TempData["Error"] = "Invalid Competition ID";
                return RedirectToAction("ErrorPage");
            }
            
           //get count of competitiors, if competitors is not equal to 0, admin is not allowed to edit the competition details 
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
            //if validation is successful update competition to database.
            if (ModelState.IsValid)
            {
                //Update competition record to database
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
            // or account not in the "Admin" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                TempData["Error"] = "You are not authorised to enter this page.";
                return RedirectToAction("ErrorPage", "Competition");
            }

            //get competition object from the ID
            Competition comp = compContext.GetDetails(id.Value);

            //if id == to null or competition ID equals to 0 bring user to error page
            if (id == null || comp.CompetitionID == 0)
            { 
                TempData["Error"] = "Invalid Competition ID";
                return RedirectToAction("ErrorPage");
            }
            
            //count the number of competitors who joined the competition. 
            //if count does not equals to 0, admin is not allowed to delete the competition
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
            // Delete the competition record from database
            compContext.Delete(competition);
            return RedirectToAction("Index");
        }

        public ActionResult AddJudge(int? id)
        {
            List<Judge> judgeList = judgeContext.GetAllJudge();
            // Stop accessing the action if not logged in
            // or account not in the "Administrator" role
            // ...need to do 
            Competition comp = compContext.GetDetails(id.Value);
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                TempData["Error"] = "You are not authorised to enter this page.";
                return RedirectToAction("ErrorPage", "Competition");
            }

            //if competitionid equates to null or 0, admin will be redirected to the error page
            else if (id == null || comp.CompetitionID == 0)
            { 
                TempData["Error"] = "Invalid Competition ID";
                return RedirectToAction("ErrorPage"); 
            }

            //if competition has ended, the admin is not allowed to add judges
            else if(DateTime.Now > comp.EndDate)
            {
                TempData["Error"] = "Not allowed to add judges. Competition has ended.";
                return RedirectToAction("ErrorPage");
            }
            
            //if start and end dates are null, the competition is not allowed to add judges 
            else if(comp.StartDate == null || comp.EndDate == null)
            {
                TempData["Error"] = "Not allowed to add judges. Competition dates have not been finalised.";
                return RedirectToAction("ErrorPage");
            }

            //check if the areainterest for judges match the competition area of interest 
            for (int i = 0; i < judgeList.Count; i++)
            {
                if (judgeList[i].AreaInterestID == comp.AreaInterestID)
                {
                    //check if the judge has joined any competition, if have not joined any current or future competitions,
                    //they are added to the list to let admin to assign them to a competition
                    if (compContext.getJudgeCompetition(judgeList[i].JudgeID) == 0)
                    {
                        jList.Add(
                        new SelectListItem
                        {
                            Value = judgeList[i].JudgeID.ToString(),
                            Text = judgeList[i].JudgeName.ToString(),
                        });
                    }
                    //if list is 0 priint message no judges is available
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

            //show judge list and create competitionjudge object and set competitionid to id.
            ViewData["judgeList"] = jList;
            CompetitionJudge compjudge = new CompetitionJudge();
            compjudge.CompetitionID = (int)id;

            return View(compjudge);

        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddJudge(CompetitionJudge compJudge)
        {
            //if validation succeessful, add judge
            if (ModelState.IsValid)
            {
                compContext.AddJudge(compJudge);
                return RedirectToAction("Index");
            }
            else
            {
                //return the add judge view with the options to add judge
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
            // get competition object 
            Competition comp = compContext.GetDetails(id.Value);
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                TempData["Error"] = "You are not authorised to enter this page.";
                return RedirectToAction("ErrorPage", "Competition");
            }
            //if id or competition id is null/ 0 direct user to error page
            else if (id == null || comp.CompetitionID == 0)
            {
                TempData["Error"] = "Invalid Competition ID";
                return RedirectToAction("ErrorPage");
            }
            //if competition is over admin is not allowed to remove judges
            else if (DateTime.Now > comp.EndDate)
            {
                TempData["Error"] = "Not allowed to remove judges. Competition has ended.";
                return RedirectToAction("ErrorPage");
            }
            //admin cannot remove judges as dates has not been finalised. judges count should be 0 either ways.
            else if (comp.StartDate == null || comp.EndDate == null)
            {
                TempData["Error"] = "Not allowed to remove judges. Competition dates have not been finalised.";
                return RedirectToAction("ErrorPage");
            }

            //get judges that are assigned the competition
            List<int> judgeidList = compContext.getJudgesinCompetition(id.Value);
            if (judgeidList.Count == 0)
            { 
                //if there are not judges, direct admin to error page
                TempData["Error"] = "There are no judges to remove.";
                return RedirectToAction("ErrorPage");
            }
            // add judges to select list item for the drop down list
            for (int i = 0; i < judgeidList.Count; i++)
            {
                   jList.Add(
                new SelectListItem
                {
                    Value = judgeidList[i].ToString(),
                    Text = judgeContext.GetJudge(judgeidList[i]).JudgeName,
                });
               
            }
            //set viewdata to the removejudge view with the judgelist
            ViewData["jidList"] = jList;
            //create competition judge object and set id
            CompetitionJudge compjudge = new CompetitionJudge();
            compjudge.CompetitionID = (int)id;
            return View(compjudge);
        }
        [HttpPost]
        public ActionResult RemoveJudge(CompetitionJudge competitionJudge)
        {
            //remove judge and redirect user to index
            compContext.RemoveJudge(competitionJudge);
            return RedirectToAction("Index");
        }

        
    }
}
