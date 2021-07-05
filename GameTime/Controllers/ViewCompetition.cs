using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameTime.Controllers
{
    public class ViewCompetition : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
