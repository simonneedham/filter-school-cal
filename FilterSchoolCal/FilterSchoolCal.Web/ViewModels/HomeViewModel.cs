using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using FilterSchoolCal.Web.Models;
using FilterSchoolCal.Web.Properties;

namespace FilterSchoolCal.Web.ViewModels
{
    public class HomeViewModel
    {
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

        public string CalendarName
        {
            get
            {
                return Settings.Default.CalendarName;
            }
        }

        public IList<Group> Groups { get; set; }
        public IList<Event> Events { get; set; }
    }
}