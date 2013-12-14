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

            var symptoms = GetSymptoms(list);

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

        public static List<int> GetSymptoms(List<PatientCase> cases)
        {
            var symptoms = new List<int>();

            foreach (var patientCase in cases)
            {
                var s = patientCase.Symptoms.Split(';');
                foreach (var symptom in s)
                {
                    if (symptom == "") continue;

                    var symptomAsInt = Convert.ToInt32(symptom);
                    var index = symptoms.IndexOf(symptomAsInt);
                    if (index == -1)
                    {
                        symptoms.Add(symptomAsInt);
                    }
                }
            }

            return symptoms;
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
            else if (bmi > 40)
            {
                return 5;
            }
            
            return -9;
        }

        public static List<PatientCase> ClusterCases()
        {
            var vanillaCases = GetAll();

            //var numberOfCases = vanillaCases.Count/2;
           // vanillaCases = vanillaCases.OrderBy(r => Guid.NewGuid()).Take((int) numberOfCases).ToList();
            
            var groups = ClassRepository.GetAll().ToDictionary(diagnose => diagnose.Class, diagnose => new List<PatientCase>());

            foreach (var vanillaCase in vanillaCases)
            {
                groups[vanillaCase.ClassModel.Class].Add(vanillaCase);
            }

            var superCases = (from pair in groups where pair.Value.Count > 0 select GenerateSuperCase(pair.Key, pair.Value)).ToList();

            return superCases;
        }

        public static PatientCase GenerateSuperCase(int classId, List<PatientCase> cases)
        {
            var numOfCases = cases.Count();
            var aggregateAge = 0;
            var aggregateBmi = 0.0;
            var tobaccoUsers = 0;

            //Sex
            var male = 0;
            var female = 0;
            
            //Races
            var white = 0;
            var black = 0;
            var asian = 0;
            var nativeHawaiian = 0;
            var americanIndian = 0;

            var aggregateSymptoms = new int[cases.ElementAt(0).FeatureVector.Length - 5];

            foreach (var patientCase in cases)
            {
                aggregateAge += patientCase.Age;
                aggregateBmi += patientCase.Bmi;

                if (patientCase.TobaccoUse == 2) tobaccoUsers++;

                if (patientCase.Sex == 1) female++;
                else if (patientCase.Sex == 2) male++;

                switch (patientCase.Race)
                {
                    case 1:
                        white++;
                        break;
                    case 2:
                        black++;
                        break;
                    case 3:
                        asian++;
                        break;
                    case 4:
                        nativeHawaiian++;
                        break;
                    case 5:
                        americanIndian++;
                        break;
                }

                for (var i = 5; i < patientCase.FeatureVector.Length; i++)
                {
                    if (patientCase.FeatureVector[i] == 1) aggregateSymptoms[i - 5]++;
                }
            }

            var superAge = GetAgeGroup(Convert.ToInt32(aggregateAge/numOfCases));
            var superSex = female > numOfCases/1.5 ? 1 : male > numOfCases/1.5 ? 2 : -9;
            var superBmi = GetBmiGroup(aggregateBmi/numOfCases);
            var superTobacco = tobaccoUsers > numOfCases/2 ? 2 : -9;
            var superRace = 6;

            if (white > numOfCases/2.0) superRace = 1;
            else if (black > numOfCases/2.0) superRace = 2;
            else if (asian > numOfCases/2.0) superRace = 3;
            else if (nativeHawaiian > numOfCases/2.0) superRace = 4;
            else if (americanIndian > numOfCases/2.0) superRace = 5;

            var superCase = new PatientCase
            {
                Age = superAge,
                Sex = superSex,
                Bmi = superBmi,
                TobaccoUse = superTobacco,
                Race = superRace,
                ClassModel = ClassRepository.GetByClassId(classId),
                FeatureVector = new int[cases.ElementAt(0).FeatureVector.Length]
            };

            superCase.FeatureVector[0] = superAge;
            superCase.FeatureVector[1] = superSex;
            superCase.FeatureVector[2] = superRace;
            superCase.FeatureVector[3] = superTobacco;
            superCase.FeatureVector[4] = superBmi;

            for (var i = 0; i < aggregateSymptoms.Length; i++)
            {
                if (aggregateSymptoms[i] > numOfCases/10.0) superCase.FeatureVector[i + 5] = 1;
                else superCase.FeatureVector[i + 5] = -9;
            }

            return superCase;
        }
    }
}
