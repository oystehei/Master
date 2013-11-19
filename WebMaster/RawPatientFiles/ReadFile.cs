using System;
using System.Collections.Generic;
using System.Web;
using Master.Implementation;
using Master.Models;

namespace WebMaster.RawPatientFiles
{
    public class ReadFile
    {
        public static void Main()
        {
            string[] lines = System.IO.File.ReadAllLines(HttpContext.Current.Server.MapPath("~/RawPatientFiles/NAMCS2010.txt"));

            foreach (var line in lines)
            {
                var newPatientCase = new PatientCase
                {
                    Age = Convert.ToInt32(line.Substring(7, 3)),
                    Sex = Convert.ToInt32(line.Substring(10, 1)),
                    Race = Convert.ToInt32(line.Substring(13, 2)),
                    TobaccoUse = Convert.ToInt32(line.Substring(25, 2)),
                    Bmi = Convert.ToDouble(line.Substring(99, 6)),
                    Symptoms = ""
                };

                var symptom1 = line.Substring(30, 5);
                if (symptom1.Substring(0, 2) == "10")
                {
                    newPatientCase.Symptoms += symptom1 + ";";
                }

                var symptom2 = line.Substring(35, 5);
                if (symptom2.Substring(0, 2) == "10")
                {
                    newPatientCase.Symptoms += symptom2 + ";";
                }

                var symptom3 = line.Substring(40, 5);
                if (symptom3.Substring(0, 2) == "10")
                {
                    newPatientCase.Symptoms += symptom3 + ";";
                }

                var diagnose = line.Substring(54, 5);

                if (diagnose.Substring(0, 1).ToLower() != "e" && diagnose.Substring(0, 1).ToLower() != "v")
                {
                    newPatientCase.Diagnose = diagnose;
                }
                else
                {
                    newPatientCase.Diagnose = "";
                }

                var majorReasonForVisit = line.Substring(53, 1);

                if (majorReasonForVisit == "1" && newPatientCase.Symptoms.Length > 0 && newPatientCase.Diagnose.Trim().Length > 0)
                {
                    PatientCaseRepository.Add(newPatientCase);
                }
            }
        }
    }
}