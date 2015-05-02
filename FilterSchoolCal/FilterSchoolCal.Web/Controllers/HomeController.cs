using DDay.iCal;
using DDay.iCal.Serialization.iCalendar;
using FilterSchoolCal.Web.Models;
using FilterSchoolCal.Web.Properties;
using FilterSchoolCal.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
                Groups = GetGroups()
            };

            return View(vm);
        }

        public ActionResult Events()
        {
            var groups = GetGroups();

            var qs = this.Request.QueryString;
            foreach (var key in qs.AllKeys.Where(k => k != null).ToList())
            {
                if (Boolean.Parse(qs[key]))
                {
                    var grp = groups.Where(w => w.Name == key).SingleOrDefault();
                    if (grp != null)
                        grp.Selected = true;
                }
            }

            //Get Events
            var events = GetUniqueEvents();

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
        public void Events(HomeViewModel vm)
        {
            string queryString = String.Empty;
            foreach (var grp in vm.Groups.Where(w => w.Selected == true).ToList())
                queryString += "&" + grp.Name + "=true";

            if (queryString.Length > 0)
                queryString = "?" + queryString;

            this.Response.Redirect("~/Home/Events" + queryString);
        }

        [HttpPost]
        public FileStreamResult MakeICal(HomeViewModel vm)
        {
            var checkedItems = new HashSet<string>(vm.Events.Where(w => w.Selected == true).Select(s => s.Summary).ToList());

            var iCal = GetICal();

            iCal.Events
                .Where(evt => !checkedItems.Contains(evt.Summary))
                .ToList()
                .ForEach(evt => iCal.Events.Remove(evt));

            var ms = new MemoryStream();
            var serializer = new iCalendarSerializer();
            serializer.Serialize(iCal, ms, Encoding.UTF8);
            ms.Seek(0, 0);

            return File(ms, "text/Calendar", "calendar.ical");
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact";
            return View();
        }

        public ActionResult FAQ()
        {
            ViewBag.Message = "FAQ";
            return View();
        }

        private static List<Group> GetGroups()
        {
            var groups = new List<Group>
            {
                //new Group { Name = "Reception", RegExString = "^(?!.*Staff Meeting).*Pre-Prep|Reception|Pre-Prep"},
                //new Group { Name = "Yr1", RegExString ="^(?!.*Staff Meeting).*Pre-Prep|Year 1" },
                //new Group { Name = "Yr2", RegExString="^(?!.*Staff Meeting).*Pre-Prep|Year 2"},

                new Group { Name = "Common", RegExString = "Bacon Butties|Second-Hand Uniform Shop|Start of Term|Term Resumes|Half-Term|Mufti-Day|End of School|Bike & Scoot|Cirque|Whole School|Walk-to-School|Speech Day", Selected = true},
                new Group { Name = "Reception", RegExString = "Pre-Prep|Reception|Flop Club|EYFS"},
                new Group { Name = "Yr1", RegExString ="Pre-Prep|Year 1|1C|1M|Flop Club|EYFS" },
                new Group { Name = "Yr2", RegExString="Pre-Prep|Year 2|Flop Club|EYFS"},
                new Group { Name = "Yr3", RegExString="Yea(r|rs).*3|3P|3S|Y3|Year 2 – Year 4|Junior School|Activities|Prep School Parent|U’8"},
                new Group { Name = "Yr4", RegExString="Yea(r|rs).*4|Years 3 & 4|Y3 – 5|Y3 – 8|Years 3 – 5|Years 3 – 6|Junior School|Activities|Prep School Parent|U’9"},
                new Group { Name = "Yr5", RegExString="Yea(r|rs).*5|Y3 – 5|Y3 – 8|5A|5H|Y5|Years 4 – 6|Activities|Middle School|Prep School Parent|U’10" },
                new Group { Name = "Yr6", RegExString=@"Yea(r|rs).*6|Y3 – 8|\(Y.*6|6B|6T|Years 5 – 7|Years 4 – 6|Years 4 – 8|Activities|Middle School|Prep School Parent|U’11" }, //U'11
                new Group { Name = "Yr7" , RegExString=@"Yea(r|rs) 7|Y3 – 8|\(Y.*7|Years 5 – 7|Years 4 – 8|Activities|Senior|Prep School Parent|U’12"},
                new Group { Name = "Yr8", RegExString=@"Yea(r|rs) 8|Y3 – 8|\(Y.*8|Years 4 – 8|Activities|Senior|Prep School Parent|U’13" },
                new Group { Name = "DAS", RegExString = "DAS" },
                new Group { Name = "nthXI", RegExString=@"\d(st|nd|rd|th)" },
                new Group { Name = "Swimming", RegExString="Swimming" }
            };
            return groups;
        }

        private List<SchoolEvent> GetUniqueEvents()
        {
            var events = CacheHelper.Get("events", () =>
            {
                var iCal = GetICal();

                //populate list with unique Event summaries
                var uniqueItems = iCal.Events
                                      .Select(evt => evt.Summary)
                                      .Distinct()
                                      .OrderBy(ob => ob)
                                      .Select(s => new SchoolEvent { Summary = s })
                                      .ToList();


                uniqueItems.ForEach(ui => System.Diagnostics.Debug.WriteLine(ui.Summary));

                return uniqueItems;
            }
            );
            return events;
        }

        private IICalendar GetICal()
        {
            var ical = CacheHelper.Get("ical", () =>
            {
                var iCalObj = iCalendar.LoadFromFile(HttpContext.Server.MapPath("~/App_Data/" + Settings.Default.CalendarFileName))
                                    .First();

                return iCalObj;
            }
            );
            return ical;
        }
    }
}