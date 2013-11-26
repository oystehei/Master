using System;
using System.Collections.Generic;
using System.Linq;
using Master.Models;

namespace Master.Implementation
{
    public static class PatientCaseRepository
    {
        private static readonly DomainDbContext _db = new DomainDbContext();

        public static PatientCase GetById(int id)
        {
            return _db.PatientCases.FirstOrDefault(x => x.Id == id);
        }

        public static void Add(PatientCase patientCase)
        {
            var classModel = ClassRepository.GetByDiagnose(patientCase.Diagnose);
            patientCase.ClassModel = classModel;
            _db.PatientCases.Add(patientCase);
            _db.SaveChanges();
        }

        public static void Delete(int id)
        {
            var patientCase = GetById(id);
            if (patientCase != null)
            {
                _db.PatientCases.Remove(patientCase);
                _db.SaveChanges();
            }
        }

        public static List<PatientCase> GetAll()
        {
            var list =  _db.PatientCases.ToList();
            
            var symptoms = new List<int>();

            foreach(var patientCase in list)
            {
                var s = patientCase.Symptoms.Split(';');
                foreach (var symptom in s)
                {
                    if(symptom == "") continue;

                    var symptomAsInt = Convert.ToInt32(symptom);
                    var index = symptoms.IndexOf(symptomAsInt);
                    if (index == -1)
                    {
                        symptoms.Add(symptomAsInt);
                    }
                }
            }
            foreach (var patientCase in list)
            {
                patientCase.FeatureVector = new int[5 + symptoms.Count];
                patientCase.FeatureVector[0] = GetAgeGroup(patientCase.Age);
                patientCase.FeatureVector[1] = patientCase.Sex;
                patientCase.FeatureVector[2] = patientCase.Race;
                patientCase.FeatureVector[3] = patientCase.TobaccoUse;
                patientCase.FeatureVector[4] = GetBmiGroup(patientCase.Bmi);
                for (int i = 0; i < symptoms.Count; i++)
                {
                    if (patientCase.Symptoms.Contains(symptoms.ElementAt(i).ToString()))
                    {
                        patientCase.FeatureVector[5 + i] = 1;
                    }
                    else
                    {
                        patientCase.FeatureVector[5 + i] = -9;
                    }
                }
            }
            return list;
        }

        public static int GetAgeGroup(int age)
        {
            if (age == -9)
            {
                return age;
            }
            else if (age <= 2)
            {
                return 0;
            }
            else if (age <= 12)
            {
                return 1;
            }
            else if (age <= 20)
            {
                return 2;
            }
            else if (age <= 40)
            {
                return 3;
            }
            else if (age <= 60)
            {
                return 4;
            }
            else
            {
                return 5;
            }
        }

        public static int GetBmiGroup(double bmi)
        {
            if (bmi > 0 && bmi < 18.5)
            {
                return 0;
            }
            else if (bmi < 25)
            {
                return 1;
            }
            else if (bmi < 30)
            {
                return 2;
            }
            else if (bmi < 35)
            {
                return 3;
            }
            else if (bmi < 40)
            {
                return 4;
            }
            
            return -9;
        }
    }
}
