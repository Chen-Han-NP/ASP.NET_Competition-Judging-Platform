using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using GameTime.Models;
using GameTime.SQ

namespace GameTime.Controllers
{
    public class AreaOfInterestController : Controller
    {
        //create form default page
        public IActionResult Create()
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
        public IActionResult Create(AreaOfInterest aoi)
        {
            // The aoi object contains user inputs from view
            if (!ModelState.IsValid) // validation fails
            {
                return View(aoi); // returns the view with errors
            }
            if (ModelState.IsValid)
            {
                //Add staff record to database
                aoi.AreaInterestID = aoi.Add(staff);
                //Redirect user to Staff/Index view
                return RedirectToAction("Index");
            }

            //generate Area of Interest ID

        }
    }
}
