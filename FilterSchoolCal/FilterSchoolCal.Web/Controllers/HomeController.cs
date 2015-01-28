using DDay.iCal;
using FilterSchoolCal.Web.Models;
using FilterSchoolCal.Web.Properties;
using FilterSchoolCal.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FilterSchoolCal.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var vm = new HomeViewModel
            {
                Groups = new List<Group>
                {
                    new Group { Name = "Common" },
                    new Group { Name = "Reception" },
                    new Group { Name = "Yr1" },
                    new Group { Name = "Yr2" },
                    new Group { Name = "Yr3" },
                    new Group { Name = "Yr4" },
                    new Group { Name = "Yr5" },
                    new Group { Name = "Yr6" },
                    new Group { Name = "Yr7" },
                    new Group { Name = "Yr8" },
                    new Group { Name = "DAS" },
                    new Group { Name = "nthXI" },
                    new Group { Name = "Swimming" }
                }
            };

            //public bool Common { get; set; }
            //public bool Reception { get; set; }
            //public bool Yr1 { get; set; }
            //public bool Yr2 { get; set; }
            //public bool Yr3 { get; set; }
            //public bool Yr4 { get; set; }
            //public bool Yr5 { get; set; }
            //public bool Yr6 { get; set; }
            //public bool Yr7 { get; set; }
            //public bool Yr8 { get; set; }
            //public bool nthXI { get; set; }
            //public bool Swimming { get; set; }

            var qs = this.Request.QueryString;

            foreach(var key in qs.AllKeys.Where(k => k != null).ToList())
            {
                if (Boolean.Parse(qs[key]))
                {
                    var grp = vm.Groups.Where(w => w.Name == key).SingleOrDefault();
                    if (grp != null)
                        grp.Selected = true;
                }
            }

            //Get Events
            vm.Events = CacheHelper.Get("events", () =>
                {
                    var iCal = iCalendar.LoadFromFile(HttpContext.Server.MapPath("~/App_Data/" + Settings.Default.CalendarFileName))
                                        .First();

                    //populate list with unique Event summaries
                    var uniqueItems = iCal.Events
                                          .Select(evt => evt.Summary)
                                          .Distinct()
                                          .OrderBy(ob => ob)
                                          .Select(s => new SchoolEvent { Summary = s})
                                          .ToList();

                    return uniqueItems;
                }
            );



            return View(vm);
        }

        [HttpPost]
        public void Filter(HomeViewModel vm)
        {
            string queryString = String.Empty;
            foreach (var grp in vm.Groups.Where(w => w.Selected == true).ToList())
                queryString += "&" + grp.Name + "=true";

            if (queryString.Length > 0)
                queryString = "?" + queryString;

            this.Response.Redirect("~/" + queryString);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}