using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameTime.Controllers
{
    public class AreaOfInterestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
