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
using Microsoft.AspNetCore.Authentication.Cookies;
using Google.Apis.Auth.OAuth2;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Google.Apis.Auth;
using static Google.Apis.Auth.GoogleJsonWebSignature;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;

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
                    int judgeID = judgeContext.GetAllJudge()[i].JudgeID;
                    if (checkEmail == email && checkPassword == password)
                    {
                        HttpContext.Session.SetInt32("JudgeID", judgeID);
                        HttpContext.Session.SetString("Role", "Judge");
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
                        HttpContext.Session.SetString("CompetitorName", competitorContext.GetAllCompetitor()[i].CompetitorName);
                        HttpContext.Session.SetString("CompetitorID", competitorContext.GetAllCompetitor()[i].CompetitorID.ToString());
                        return RedirectToAction("Competitor", "Home");
                    }
                }
            }
            TempData["invalidLogin"] = "Invalid Email or Password.";
            return RedirectToAction("Login", "Home");
            
        }

        [Authorize]
        public async Task<ActionResult> CompetitorLogin()
        {
            // The user is already authenticated, so this call won't
            // trigger login, but it allows us to access token related values.
            AuthenticateResult auth = await HttpContext.AuthenticateAsync();
            string idToken = auth.Properties.GetTokenValue(
             OpenIdConnectParameterNames.IdToken);
            try
            {
                // Verify the current user logging in with Google server
                // if the ID is invalid, an exception is thrown
                Payload currentUser = await
                GoogleJsonWebSignature.ValidateAsync(idToken);
                string userName = currentUser.Name;
                string eMail = currentUser.Email;
                for (int i = 0; i < competitorContext.GetAllCompetitor().Count; i++)
                {
                    if (eMail == competitorContext.GetAllCompetitor()[i].EmailAddr)
                    {
                        HttpContext.Session.SetString("CompetitorID", competitorContext.GetAllCompetitor()[i].CompetitorID.ToString());
                        HttpContext.Session.SetString("Role", "Competitor");
                        return RedirectToAction("Competitor", "Home");
                    }
                    
                }
                TempData["userName"] = userName;
                TempData["eMail"] = eMail;
                return RedirectToAction("GoogleSignUp", "SignUp");
            }
            catch (Exception)
            {
                // Token ID is may be tempered with, force user to logout
                return RedirectToAction("LogOut");
            }
        }

        public async Task<ActionResult> LogOut()
        {
            // Clear authentication cookie
            await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);
            // Clear all key-values pairs stored in session state
            HttpContext.Session.Clear();
            // Call the Index action of Home controller
            return RedirectToAction("Index", "Home");
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
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                TempData["Error"] = "You are not authorised to enter this page.";
                return RedirectToAction("ErrorPage", "Competition");
            }
            return View();
        }
        public ActionResult Judge()
        {
            return RedirectToAction("Index", "Judge");
        }
        public ActionResult Competitor()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Competitor"))
            {
                TempData["NotCompetitor"] = "You haven't logged in yet!";
                return RedirectToAction("ErrorPage", "Competitor");
            }
            for (int i = 0; i < competitorContext.GetAllCompetitor().Count; i++)
            {
                if (HttpContext.Session.GetString("CompetitorID") == competitorContext.GetAllCompetitor()[i].CompetitorID.ToString())
                {
                    ViewData["CompetitorName"] = competitorContext.GetAllCompetitor()[i].CompetitorName;
                }
            }

            return View();
        }

        public ActionResult Privacy()
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
