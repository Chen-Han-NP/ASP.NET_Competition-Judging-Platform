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
        CompetitorSubmissionDAL submissionContext = new CompetitorSubmissionDAL();
        CompetitionScoreDAL scoreContext = new CompetitionScoreDAL();

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

        public ActionResult ViewSubmission()
        {
            List<CompetitorScoreViewModel> Model = new List<CompetitorScoreViewModel>();

            int judgeID = (int)HttpContext.Session.GetInt32("JudgeID");
            foreach (int i in judgeContext.getCompetitions(judgeID))
            {
                List<CompetitorSubmissionViewModel> c = submissionContext.getAllCompetitor(i);
                foreach (CompetitorSubmissionViewModel m in c)
                {
                    CompetitorScoreViewModel s = new CompetitorScoreViewModel();
                    s.CompetitorID = m.CompetitorId;
                    s.CompetitionID = m.CompetitionId;
                    s.CompetitorName = m.CompetitorName;
                    s.FileSubmitted = m.FileSubmitted;
                    s.DateTimeSubmitted = m.DateTimeSubmitted;
                    s.Appeal = m.Appeal;
                    s.VoteCount = m.VoteCount;
                    s.Ranking = m.Ranking;
                    s.FileUpload = m.FileUpload;

                    string n = "N/A";
                    if (scoreContext.hasScore(m.CompetitorId, m.CompetitionId, 1) && 
                        scoreContext.hasScore(m.CompetitorId, m.CompetitionId, 2) &&
                        scoreContext.hasScore(m.CompetitorId, m.CompetitionId, 3) &&
                        scoreContext.hasScore(m.CompetitorId, m.CompetitionId, 4))
                    {
                        s.C1Score = scoreContext.getCompetitorScore(m.CompetitorId, m.CompetitionId, 1).Score.ToString();
                        s.C2Score = scoreContext.getCompetitorScore(m.CompetitorId, m.CompetitionId, 2).Score.ToString();
                        s.C3Score = scoreContext.getCompetitorScore(m.CompetitorId, m.CompetitionId, 3).Score.ToString();
                        s.C4Score = scoreContext.getCompetitorScore(m.CompetitorId, m.CompetitionId, 4).Score.ToString();
                        s.Score = scoreContext.getFinalScore(m.CompetitorId, m.CompetitionId);
                    } 
                    else
                    {
                        s.C1Score = n;
                        s.C2Score = n;
                        s.C3Score = n;
                        s.C4Score = n;
                        if (scoreContext.hasScore(m.CompetitorId, m.CompetitionId, 1))
                            s.C1Score = scoreContext.getCompetitorScore(m.CompetitorId, m.CompetitionId, 1).Score.ToString();
                        if (scoreContext.hasScore(m.CompetitorId, m.CompetitionId, 2))
                            s.C2Score = scoreContext.getCompetitorScore(m.CompetitorId, m.CompetitionId, 2).Score.ToString();
                        if (scoreContext.hasScore(m.CompetitorId, m.CompetitionId, 3))
                            s.C3Score = scoreContext.getCompetitorScore(m.CompetitorId, m.CompetitionId, 3).Score.ToString();
                        if (scoreContext.hasScore(m.CompetitorId, m.CompetitionId, 4))
                            s.C4Score = scoreContext.getCompetitorScore(m.CompetitorId, m.CompetitionId, 4).Score.ToString();
                    }
                    Model.Add(s);
                }

            }
            return View(Model);
        }

        public ActionResult Score(string id)
        {
            CompetitionScore Model;

            string[] idList = id.Split(',');

            if (scoreContext.hasScore(Int32.Parse(idList[0]), Int32.Parse(idList[1]), Int32.Parse(idList[2])))
            {
                Model = scoreContext.getCompetitorScore(Int32.Parse(idList[0]), Int32.Parse(idList[1]), Int32.Parse(idList[2]));
            }
            else
            {
                Model = new CompetitionScore
                {
                    CompetitorID = Int32.Parse(idList[0]),
                    CompetitionID = Int32.Parse(idList[1]),
                    CriteriaID = Int32.Parse(idList[2])
                };
            }

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
