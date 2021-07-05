using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameTime.Models;

namespace GameTime.Controllers
{
    public class SignUpController : Controller
    {
        public ActionResult Index()
        {
            return View("Index");
        }

        public ActionResult CompetitorSignUp()
        {
            return View();
        }
        public ActionResult JudgeSignUp()
        {
            return View();
        }
    }
}
