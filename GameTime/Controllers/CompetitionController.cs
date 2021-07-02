using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using GameTime.Models;
using GameTime.DAL;

namespace GameTime.Controllers
{
    public class CompetitionController : Controller
    {
        private CompetitionDAL AOIContext = new CompetitionDAL();
        //create form default page
        public ActionResult Createcomp()
        {
            // Stop accessing the action if not logged in
            // or account not in the "Administrator" role
            // ...need to do 


            //create Area of Interest object
            Competition comp = new Competition();

            return View(comp);
        }

        [HttpPost]
        public ActionResult Createcomp(Competition comp)
        {
            // The aoi object contains user inputs from view
            if (ModelState.IsValid)
            {
                //Add staff record to database
                comp.AreaInterestID = AOIContext.AddComp(comp);
                //Redirect user to Staff/Index view
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //Input validation fails, return to the Create view
                //to display error message
                return View(comp);
            }



        }
    }
}
