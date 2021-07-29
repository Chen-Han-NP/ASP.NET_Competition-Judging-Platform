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
        public ActionResult Index()
        {
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            List<AreaOfInterest> aiList = AOIContext.GetAreaOfInterests();
            return View(aiList);
        }
        public ActionResult ErrorPage()
        {
            ViewData["Error"] = TempData["Error"].ToString();
            return View();
        }
        //create form default page
        public ActionResult CreateAOI()
        {
            // Stop accessing the action if not logged in
            // or account not in the "Administrator" role
            // ...need to do 
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                TempData["Error"] = "YOU IMPOSTOR, YOU AINT A ADMIN";
                return RedirectToAction("ErrorPage");
            }

            //create Area of Interest object
            AreaOfInterest aoi = new AreaOfInterest();
            
            return View(aoi);
        }

        [HttpPost]
        public ActionResult CreateAOI(AreaOfInterest aoi)
        {
            // The aoi object contains user inputs from view
            if (ModelState.IsValid)
            {
                //Add aoi record to database
                aoi.AreaInterestID = AOIContext.AddAOI(aoi);
                //ViewData["ShowResult"] = true;
                //Redirect user to AOI/Index view
                return RedirectToAction("Index");
            }
            else
            {
                //Input validation fails, return to the Create view
                //to display error message
                return View(aoi);
            }

            

        }

        public ActionResult Delete(int? id)
        {
            // Stop accessing the action if not logged in
            // or account not in the "Staff" role
            if ((HttpContext.Session.GetString("Role") == null) ||
            (HttpContext.Session.GetString("Role") != "Admin"))
            {
                TempData["Error"] = "YOU IMPOSTOR, YOU AINT A ADMIN";
                return RedirectToAction("ErrorPage");
            }
            AreaOfInterest aoi = AOIContext.GetDetails(id.Value);
            if (aoi.AreaInterestID == 0 || aoi == null)
            { //Query string parameter not provided
              //Return to listing page, not allowed to edit
                TempData["Error"] = "Invalid Area of Interest ID.";
                return RedirectToAction("ErrorPage");
            }
            

            if (AOIContext.GetCompetitorCount(id.Value) != 0)
            {
                //Return to listing page, not allowed to edit
                TempData["Error"] = "You are not allowed to delete this Area of Interest as competitors exist in this Area of Interest.";
                return RedirectToAction("ErrorPage");
            }
            return View(aoi);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(AreaOfInterest aoi)
        {
            // Delete the staff record from database
            AOIContext.Delete(aoi);
            return RedirectToAction("Index");
        }


      

    }
}
