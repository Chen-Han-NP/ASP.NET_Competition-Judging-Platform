using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using GameTime.Models;
using GameTime.DAL;

namespace GameTime.Controllers
{
    public class AreaOfInterestController : Controller
    {
        private AreaOfInterestDAL AOIContext = new AreaOfInterestDAL();
        //create form default page
        public IActionResult CreateAOI()
        {
            // Stop accessing the action if not logged in
            // or account not in the "Administrator" role
            // ...need to do 

            //prepare data for view
            ViewData["ShowResult"] = false;

            //create Area of Interest object
            AreaOfInterest aoi = new AreaOfInterest();
            
            return View(aoi);
        }

        [HttpPost]
        public IActionResult CreateAOI(AreaOfInterest aoi)
        {
            // The aoi object contains user inputs from view
            if (ModelState.IsValid)
            {
                //Add staff record to database
                aoi.AreaInterestID = AOIContext.AddAOI(aoi);
                //Redirect user to Staff/Index view
                return RedirectToAction("Index");
            }
            else
            {
                //Input validation fails, return to the Create view
                //to display error message
                return View(aoi);
            }

            

        }
    }
}
