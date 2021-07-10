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
    }
}
