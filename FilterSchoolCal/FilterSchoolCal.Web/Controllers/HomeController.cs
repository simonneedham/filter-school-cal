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

            var groups = new List<Group>
            {
                new Group { Name = "Common", RegExString = "Bacon Butties|Second-Hand Uniform Shop|Start of Term|Term Resumes|Half-Term|Mufti-Day"},
                new Group { Name = "Reception", RegExString = "^(?!.*Staff Meeting).*Pre-Prep|Reception"},
                new Group { Name = "Yr1", RegExString ="^(?!.*Staff Meeting).*Pre-Prep|Year 1" },
                new Group { Name = "Yr2", RegExString="^(?!.*Staff Meeting).*Pre-Prep|Year 2"},
                new Group { Name = "Yr3", RegExString="Yea(r|rs).*3|3P|3S|Y3|Year 2 – Year 4|No Activities|No Activities or Prep"},
                new Group { Name = "Yr4", RegExString="Yea(r|rs).*4|Years 3 & 4|Y3 – 5|Y3 – 8|No Activities|No Activities or Prep"},
                new Group { Name = "Yr5", RegExString="Yea(r|rs).*5|Y3 – 5|Y3 – 8|5A|5H|Y5|No Activities|No Activities or Prep" },
                new Group { Name = "Yr6", RegExString=@"Yea(r|rs).*6|Y3 – 8|\(Y.*6|Years 5 – 7|No Activities|No Activities or Prep" },
                new Group { Name = "Yr7" , RegExString=@"Yea(r|rs) 7|Y3 – 8|\(Y.*7|Years 5 – 7|No Activities|No Activities or Prep"},
                new Group { Name = "Yr8", RegExString=@"Yea(r|rs) 8|Y3 – 8|\(Y.*8|No Activities|No Activities or Prep" },
                new Group { Name = "DAS", RegExString = "DAS" },
                new Group { Name = "nthXI", RegExString=@"\d(st|nd|rd|th)" },
                new Group { Name = "Swimming", RegExString="Swimming" }
            };

            var qs = this.Request.QueryString;
            foreach(var key in qs.AllKeys.Where(k => k != null).ToList())
            {
                if (Boolean.Parse(qs[key]))
                {
                    var grp = groups.Where(w => w.Name == key).SingleOrDefault();
                    if (grp != null)
                        grp.Selected = true;
                }
            }

            //Get Events
            var events = CacheHelper.Get("events", () =>
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

            groups.Where(w => w.Selected == true)
                  .ToList()
                  .ForEach(grp => grp.SelectEvents(ref events));

            var vm = new HomeViewModel
            {
                Events = events,
                Groups = groups
            };

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