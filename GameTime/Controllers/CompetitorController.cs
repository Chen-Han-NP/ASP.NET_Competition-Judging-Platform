using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameTime.DAL;
using GameTime.Models;
using System.IO;

namespace GameTime.Controllers
{
    public class CompetitorController : Controller
    {
        private ViewCompetitionDAL competitionContext = new ViewCompetitionDAL();
        private CompetitorDAL competitorContext = new CompetitorDAL();
        private CompetitorSubmissionDAL competitorSubmissionContext = new CompetitorSubmissionDAL();

        public ActionResult CompetitorViewCompetition()
        {
            List<CompetitionViewModel> competitionList = new List<CompetitionViewModel>();
            if ((HttpContext.Session.GetString("Role") == null) || (HttpContext.Session.GetString("Role") != "Competitor"))
            {
                return RedirectToAction("Index", "Home");
            }
           
            int id = Convert.ToInt32(HttpContext.Session.GetString("CompetitorID"));

            //Check whether the current competitor has alr join a competition
            competitionList = competitorContext.GetAllAvailableCompetitions(id);

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
                    showCriteriaList.Add(criteriaList[i]);
                }
            }
            return View(showCriteriaList);
        }

        public ActionResult JoinCompetition(int? competitionID)
        {
            
            Competition competition = new Competition();
            int id = Convert.ToInt32(HttpContext.Session.GetString("CompetitorID"));
            CompetitorSubmissionViewModel competitorSubmission = new CompetitorSubmissionViewModel
            { 
                CompetitionId = (int) competitionID,
                CompetitorId = id,
                VoteCount = 0
            };
            competitorSubmissionContext.JoinCompetition(competitorSubmission);
            return RedirectToAction("CompetitorViewCompetition", "Competitor");
        }

        public ActionResult JoinedCompetitions()
        {
            List<CompetitionViewModel> competitionList = new List<CompetitionViewModel>();
            if ((HttpContext.Session.GetString("Role") == null) || (HttpContext.Session.GetString("Role") != "Competitor"))
            {
                return RedirectToAction("Index", "Home");
            }

            int id = Convert.ToInt32(HttpContext.Session.GetString("CompetitorID"));

            //Check whether the current competitor has alr join a competition
            competitionList = competitorContext.GetJoinedCompetitions(id);

            return View(competitionList);
        }

        public ActionResult UploadView(int? competitionID)
        {
            List<CompetitionViewModel> competitionList = new List<CompetitionViewModel>();
            if ((HttpContext.Session.GetString("Role") == null) || (HttpContext.Session.GetString("Role") != "Competitor"))
            {
                return RedirectToAction("Index", "Home");
            }
            CompetitorSubmissionViewModel competitor = new CompetitorSubmissionViewModel
            {
                CompetitorId = Convert.ToInt32(HttpContext.Session.GetString("CompetitorID")),
                CompetitionId = (int)competitionID
            };

            return View(competitor);
        }

        public ActionResult Index()
        {
            return RedirectToAction("Competitor", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UploadView(CompetitorSubmissionViewModel competitorVM)
        {
            if ( competitorVM.FileUpload != null &&
 competitorVM.FileUpload.Length > 0)
            {
                try
                {
                    // Find the filename extension of the file to be uploaded.
                    string fileExt = Path.GetExtension(
                     competitorVM.FileUpload.FileName);
                    // Rename the uploaded file with the staff’s name.
                    string uploadedFile = "File_" + competitorVM.CompetitorId + "_" + competitorVM.CompetitionId + fileExt;
                    // Get the complete path to the images folder in server
                    string savePath = Path.Combine(
                     Directory.GetCurrentDirectory(),
                     "wwwroot\\competitorSubmissions", uploadedFile);
                    // Upload the file to server
                    using (var fileSteam = new FileStream(
                     savePath, FileMode.Create))
                    {
                        await competitorVM.FileUpload.CopyToAsync(fileSteam);
                    }
                    competitorVM.FileSubmitted = uploadedFile;
                    ViewData["Message"] = "File uploaded successfully.";
                }
                catch (IOException)
                {
                    //File IO error, could be due to access rights denied
                    ViewData["Message"] = "File uploading fail!";
                }
                catch (Exception ex) //Other type of error
                {
                    ViewData["Message"] = ex.Message;
                }
            }
            return View(competitorVM);

        }

    }
}
