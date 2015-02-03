using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace FilterSchoolCal.Web.Models
{
    public class Group
    {
        string _regExString;
        Regex _regex;

        public string Name { get; set; }
        public bool Selected { get; set; }

        public string RegExString
        {
            get { return _regExString;  }
            set
            {
                _regExString = value;
                _regex = new Regex(_regExString);
            }
        }

        public void SelectEvents(ref List<SchoolEvent> list)
        {
            foreach(var evt in list)
                if (_regex.Match(evt.Summary).Success)
                    evt.Selected = true;
        }
    }
}