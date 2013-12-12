using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Master.Implementation;
using Master.Models;

namespace WebMaster.Models
{
    public class AboutViewModel
    {
        public string Code { get; set; }
        public List<PatientCase> MatchingCases { get; set; }

        public  List<LeaveOneOutResultModel> LeaveOneOutResultModel { get; set; }
        public int NumberOfRounds { get; set; }

        public int From { get; set; }
        public int To { get; set; }
       

        public double ThBest { get; set; }
        public double ThAverage { get; set; }
    }

    
}