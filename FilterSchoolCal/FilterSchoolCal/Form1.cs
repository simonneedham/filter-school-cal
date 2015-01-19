using DDay.iCal;
using DDay.iCal.Serialization.iCalendar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FilterSchoolCal
{
    public partial class Form1 : Form
    {
        const string NEW_FILE = @"C:\Users\simon_000\Documents\GitHub\iCalToolkit\New_Lent_2015_Calendar.ics";
        IICalendar _iCal = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //DTSTART: 20150112T130000
            //SUMMARY: English Department Meeting
            //UID: 040000008200E00074C5B7101A82E0080000000020A2E3B81316D00100000000000000001000000055CE3489AAFE0A4CB7DB3AE01EC59D4D01000000055CE3489AAFE0A4CB7DB3AE01EC59D4D

            //load ical file
            _iCal = iCalendar.LoadFromFile(@"C:\Users\simon_000\Documents\GitHub\iCalToolkit\Lent_2015_Calendar.ics")
                                 .First();

            //populate list with unique Event summaries
            var uniqueItems = _iCal.Events
                                   .Select(evt => evt.Summary)
                                   .Distinct()
                                   .OrderBy(ob => ob)
                                   .ToArray();

            checkedListBox1.Items.AddRange(uniqueItems);

            //Tick off default events
            var defaultEvents = new HashSet<string>(new string[] { "Bacon Butties", "Second-Hand Uniform Shop", "Start of Term", "Term Resumes" });

            for(var i = 0; i<uniqueItems.Length;i++)
                if (defaultEvents.Contains(uniqueItems[i]) || uniqueItems[i].Contains("Pre-Prep") || uniqueItems[i].Contains("Reception"))
                    checkedListBox1.SetItemChecked(i, true);
        }

        private void btnMakeIcsFile_Click(object sender, EventArgs e)
        {
            var checkedItems = new HashSet<string>(checkedListBox1.CheckedItems.Cast<string>());

            _iCal.Events
                 .Where(evt => !checkedItems.Contains(evt.Summary))
                 .ToList()
                 .ForEach(evt => _iCal.Events.Remove(evt));

            if (File.Exists(NEW_FILE))
                File.Delete(NEW_FILE);

            var serializer = new iCalendarSerializer();
            serializer.Serialize(_iCal, @"C:\Users\simon_000\Documents\GitHub\iCalToolkit\New_Lent_2015_Calendar.ics");

        }
    }
}
