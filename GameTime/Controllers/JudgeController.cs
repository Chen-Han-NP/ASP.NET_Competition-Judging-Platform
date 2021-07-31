using GameTime.DAL;
using GameTime.Models;
using Microsoft.AspNetCore.Authorization;
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
        CompetitorSubmissionDAL submissionContext = new CompetitorSubmissionDAL();
        CompetitionScoreDAL scoreContext = new CompetitionScoreDAL();
        CompetitionDAL compContext = new CompetitionDAL();

        // GET: JudgeController
        public ActionResult Index()
        {
            if (!isJudge()) return RedirectToAction("Login", "Home"); // Validate if user has logged in

            int judgeId = HttpContext.Session.GetInt32("JudgeID") ?? default(int);
            Judge judge = judgeContext.GetJudge(judgeId);
            return View(judge);
        }

        // GET: JudgeController/Details/5
        public ActionResult Details(int id)
        {
            if (!isJudge()) return RedirectToAction("Login", "Home"); // Validate if user has logged in

            return View();
        }

        public ActionResult CreateCriteria()
        {
            if (!isJudge()) return RedirectToAction("Login", "Home"); // Validate if user has logged in

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCriteria(Criteria criteria)
        {
            if (ModelState.IsValid)
            {
                criteria.CriteriaID = criteriaContext.Add(criteria);

                if (criteria.CriteriaID == 0)
                {
                    ViewBag.Error = "Competition ID does not exist";
                    return View();
                }
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

                return RedirectToAction("Index", "Judge");
            }
            else
            {
                return View(criteria);
            }
        }

        public ActionResult ViewSubmission()
        {
            if (!isJudge()) return RedirectToAction("Login", "Home"); // Validate if user has logged in

            List<CompetitorScoreViewModel> Model = new List<CompetitorScoreViewModel>();

            int judgeID = (int)HttpContext.Session.GetInt32("JudgeID");
            foreach (int i in judgeContext.getCompetitions(judgeID))
            {
                List<CompetitorSubmissionViewModel> c = submissionContext.getAllCompetitorForJudge(i);
                foreach (CompetitorSubmissionViewModel m in c)
                {
                    if (compContext.GetDetails(m.CompetitionId).EndDate <= DateTime.Now) break; // If competition ended, don't display

                    CompetitorScoreViewModel s = new CompetitorScoreViewModel();
                    s.CompetitorID = m.CompetitorId;
                    s.CompetitionID = m.CompetitionId;
                    s.CompetitorName = m.CompetitorName;
                    s.FileSubmitted = m.FileSubmitted;
                    s.DateTimeSubmitted = m.DateTimeSubmitted;
                    s.VoteCount = m.VoteCount;
                    s.Ranking = m.Ranking;
                    s.FileUpload = m.FileUpload;

                    if (m.Appeal != null)   // Allow Judges to view if appeal exists
                    {
                        s.Appeal = "View";
                    }
                    else
                    {
                        s.Appeal = "";  // Set to empty string so it doesn't return exception
                    }

                    string n = "N/A";   // default text for each criteria score

                    s.C1Score = n;
                    s.C2Score = n;
                    s.C3Score = n;
                    s.C4Score = n;

                    List<Criteria> cList = criteriaContext.GetAllCriteria(m.CompetitionId);

                    if (scoreContext.hasScore(m.CompetitorId, m.CompetitionId, cList[0].CriteriaID))
                        s.C1Score = scoreContext.getCompetitorScore(m.CompetitorId, m.CompetitionId, cList[0].CriteriaID).Score.ToString();
                    if (scoreContext.hasScore(m.CompetitorId, m.CompetitionId, cList[1].CriteriaID))
                        s.C2Score = scoreContext.getCompetitorScore(m.CompetitorId, m.CompetitionId, cList[1].CriteriaID).Score.ToString();
                    if (scoreContext.hasScore(m.CompetitorId, m.CompetitionId, cList[2].CriteriaID))
                        s.C3Score = scoreContext.getCompetitorScore(m.CompetitorId, m.CompetitionId, cList[2].CriteriaID).Score.ToString();
                    if (scoreContext.hasScore(m.CompetitorId, m.CompetitionId, cList[3].CriteriaID))
                        s.C4Score = scoreContext.getCompetitorScore(m.CompetitorId, m.CompetitionId, cList[3].CriteriaID).Score.ToString();
                    s.Score = scoreContext.getFinalScore(m.CompetitorId, m.CompetitionId);
                    Model.Add(s);
                }

            }
            return View(Model);
        }

        public ActionResult Score(string id) // gets CompetitorID,CompetitionID,CriteriaID
        {
            if (!isJudge()) return RedirectToAction("Login", "Home"); // Validate if user has logged in

            CompetitionScore Model;

            string[] idList = id.Split(','); // ["CompetitorID","CompetitionID","CriteriaID"]

            int competitorID = Int32.Parse(idList[0]);
            int competitionID = Int32.Parse(idList[1]);
            int criteriaID = criteriaContext.GetAllCriteria(competitionID)[Int32.Parse(idList[2]) - 1].CriteriaID;

            if (scoreContext.hasScore(competitorID, competitionID, criteriaID)) // Check if Criteria previously has score to get
            {
                Model = scoreContext.getCompetitorScore(competitorID, competitionID, criteriaID); // Yes : get Score to Model
            }
            else // No : create new Score
            {
                Model = new CompetitionScore
                {
                    CompetitorID = competitorID,
                    CompetitionID = competitionID,
                    CriteriaID = criteriaID
            };
            }

            Criteria criteria = criteriaContext.GetCriteria(Model.CompetitionID, Model.CriteriaID);
            TempData["CriteriaName"] = criteria.CriteriaName;
            TempData["CriteriaWeightage"] = criteria.Weightage;

            return View(Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Score(CompetitionScore cs)
        {
            if (ModelState.IsValid)
            {
                if (scoreContext.hasScore(cs.CompetitorID, cs.CompetitionID, cs.CriteriaID))
                {
                    scoreContext.updateScore(cs);
                    return RedirectToAction("ViewSubmission", "Judge");
                }
                else
                {
                    scoreContext.addScore(cs);
                    return RedirectToAction("ViewSubmission", "Judge");
                }
            }
            else
            {
                return View(cs);
            }
        }

        public ActionResult Rank(string id) // gets CompetitorID,CompetitionID
        {
            if (!isJudge()) return RedirectToAction("Login", "Home"); // Validate if user has logged in

            string[] idList = id.Split(','); // ["CompetitorID","CompetitionID"]

            CompetitorSubmissionViewModel submission = submissionContext.getCompetitorSubmission(Int32.Parse(idList[0]), Int32.Parse(idList[1])); // .getCompetitorSubmission(CompetitorID,CompetitionID)
            return View(submission);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Rank(CompetitorSubmissionViewModel submission)
        {
            if (ModelState.IsValid)
            {
                if(submissionContext.UpdateRank(submission))
                {
                    List<CompetitorSubmissionViewModel> submissionList = submissionContext.getAllCompetitor(submission.CompetitionId); // Get all competitors in competition that the competitor was in
                    int? rankToPush = submission.Ranking; // Other Competitors of this ranking will be pushed up by 1 Rank
                    CompetitorSubmissionViewModel check = submission; // Original competitor's submission to check

                    for (int i = 0; i < submissionList.Count(); i++)
                    {
                        if (submissionList[i].Ranking == rankToPush && submissionList[i].CompetitorId != check.CompetitorId) // If Rank is same as rank to push and is not original competitor, update new ranking
                        {
                            submissionList[i].Ranking += 1; // Update new ranking
                            submissionContext.UpdateRank(submissionList[i]); // Upload new ranking
                            rankToPush += 1;    // Check new ranking for duplicates
                            check = submissionList[i];  // Set check to new submission
                            i = 0;  // Reset loop
                        }
                    }

                    return RedirectToAction("ViewSubmission", "Judge");
                }
                else
                {
                    ViewBag.Error = "Please put a Ranking";
                    return View(submission);
                }
            }
            else
            {
                return View(submission);
            }
        }

        public ActionResult Appeal(string id)
        {
            string[] idList = id.Split(','); // ["CompetitorID","CompetitionID","CriteriaID"]

            int competitorID = Int32.Parse(idList[0]);
            int competitionID = Int32.Parse(idList[1]);

            AppealViewModel appeal = submissionContext.GetAppeal(competitorID, competitionID);

            return View(appeal);
        }

        public bool isJudge()
        {
            int? judgeId = HttpContext.Session.GetInt32("JudgeID");
            if (judgeId != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
