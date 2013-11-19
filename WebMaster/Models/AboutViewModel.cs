using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Master.Models;

namespace WebMaster.Models
{
    public class AboutViewModel
    {
        public string Code { get; set; }
        public List<PatientCase> MatchingCases { get; set; }
    }
}