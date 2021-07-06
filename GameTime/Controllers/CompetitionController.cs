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
        private CompetitionDAL compContext = new CompetitionDAL();
        private AreaOfInterestDAL aoiContext = new AreaOfInterestDAL();
        private List<SelectListItem> sList = new List<SelectListItem>();
        public ActionResult Index()
        {
            List<Competition> compList = compContext.GetAllComp();
            return View(compList);
        }
        //create form default page
        public ActionResult Createcomp()
        {
            List<AreaOfInterest> aoiList = aoiContext.GetAreaOfInterests();
            for (int i = 0; i < aoiList.Count; i++)
            {
                sList.Add(
                new SelectListItem
                {
                    Value = (i+1).ToString(),
                    Text = aoiList[i].Name.ToString(),
                });
            }

            ViewData["ShowResult"] = false;
            ViewData["aoiList"] = sList;

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
                comp.CompetitionID = compContext.AddComp(comp);
                //Redirect user to Staff/Index view
                return RedirectToAction("Index");
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
