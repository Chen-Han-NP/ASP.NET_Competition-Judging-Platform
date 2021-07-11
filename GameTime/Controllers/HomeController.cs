using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GameTime.DAL;
using GameTime.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GameTime.Controllers
{
    public class HomeController : Controller
    {
        private JudgeDAL judgeContext = new JudgeDAL();
        private CompetitorDAL competitorContext = new CompetitorDAL();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(IFormCollection formData)
        {
            // Read inputs from textboxes
            // Email address converted to lowercase
            string email = formData["emailAddr"].ToString();
            string password = formData["txtPassword"].ToString();
            bool isJudge = false;
            string check = "";
            if (email.Length < 10)
            {
                isJudge = false;
            }
            else
            {
                isJudge = true;
                check = email.Substring(email.Length - 10);
            }

            if (email == "admin1@lcu.edu.sg" && password == "p@55Admin")
            {
                HttpContext.Session.SetString("Role", "Admin");
                return RedirectToAction("Admin", "Home");
            }
            else if (check == "lcu.edu.sg" && isJudge == true)
            {
                for (int i = 0; i < judgeContext.GetAllJudge().Count; i++)
                {
                    string checkEmail = judgeContext.GetAllJudge()[i].EmailAddr;
                    string checkPassword = judgeContext.GetAllJudge()[i].Password;
                    if (checkEmail == email && checkPassword == password)
                    {
                        return RedirectToAction("Index", "Judge");
                    }
                }
            }
            else
            {
                for (int i = 0; i < competitorContext.GetAllCompetitor().Count; i++)
                {
                    HttpContext.Session.SetString("Role", "Competitor");
                    string checkEmail = competitorContext.GetAllCompetitor()[i].EmailAddr;
                    string checkPassword = competitorContext.GetAllCompetitor()[i].Password;
                    if (checkEmail == email && checkPassword == password)
                    {
                        HttpContext.Session.SetString("CompetitorID", competitorContext.GetAllCompetitor()[i].CompetitorID.ToString());
                        return RedirectToAction("Competitor", "Home");
                    }
                }
            }
            TempData["invalidLogin"] = "Invalid Email or Password.";
            return RedirectToAction("Login", "Home");
            
        }
        public IActionResult Index()
        {
            
            
            return View();
        }

        public IActionResult Features()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }

        public ActionResult Admin()
        {
            return View();
        }
        public ActionResult Judge()
        {
            return View();
        }
        public ActionResult Competitor()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
