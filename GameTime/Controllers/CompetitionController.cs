using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using GameTime.Models;

namespace GameTime.Controllers
{
    public class CompetitionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
