using System;
using System.Collections.Generic;
using System.Linq;
using Master.Models;

namespace Master.Implementation
{
    public class iNNk
    {
        private readonly Random _rnd;
        public List<PatientCase> CaseBase { get; set; }
        public int K { get; set; }
        public List<PatientCase> RetrievalSet { get; set; }
        public string LocalOrGlobal { get; set; }
        public PatientCase CurrentPatientCase { get; set; }
        public List<List<int>> FeaturesWithPossibleValues { get; set; }
        public iNNk(string lOrg = "Global", int k = 1)
        {
            _rnd = new Random();
            K = k;
            LocalOrGlobal = lOrg;
            CaseBase = PatientCaseRepository.GetAll();
            RetrievalSet = new List<PatientCase>();
            CurrentPatientCase = new PatientCase
            {
                FeatureVector = Enumerable.Repeat(-1, CaseBase.First().FeatureVector.Length).ToArray()
            };

            FeaturesWithPossibleValues = new List<List<int>>();

            for (int i = 0; i < CurrentPatientCase.FeatureVector.Length; i++)
            {
                FeaturesWithPossibleValues.Add(new List<int>());
                foreach (var patientCase in CaseBase)
                {
                    var featureValue = patientCase.FeatureVector[i];
                    if (!FeaturesWithPossibleValues.ElementAt(i).Contains(featureValue))
                    {
                        FeaturesWithPossibleValues.ElementAt(i).Add(featureValue);
                    }
                }
            }
        }
        public int FindNextFeatureIndex(List<PatientCase> retrievalSet, ClassModel targetClass)
        {
            //If Local find FeaturesWithPossibleValues for the current retirevalset
            if (LocalOrGlobal == "Local")
            {
                FeaturesWithPossibleValues = new List<List<int>>();

                for (int i = 0; i < CurrentPatientCase.FeatureVector.Length; i++)
                {
                    FeaturesWithPossibleValues.Add(new List<int>());
                    foreach (var patientCase in retrievalSet)
                    {
                        var featureValue = patientCase.FeatureVector[i];
                        if (!FeaturesWithPossibleValues.ElementAt(i).Contains(featureValue))
                        {
                            FeaturesWithPossibleValues.ElementAt(i).Add(featureValue);
                        }
                    }
                }
            }


            var highest = -1;
            var counter = 0;
            var featureIndex = 0;
            foreach (var feature in FeaturesWithPossibleValues)
            {
                foreach (var value in feature)
                {  
                    if (value == -9)
                    {
                        continue;
                    }
                    
                    var valueGivenClass = 0;
                    var classCounter = 0;
                    var notClassCounter = 0;
                    var valueGivenNotClass = 0;
                    foreach (var patientCase in retrievalSet)
                    {
                        var featureValue = patientCase.FeatureVector[counter];
                        if (patientCase.ClassModel.Class == targetClass.Class)
                        {
                            classCounter++;
                            if (featureValue == value)
                            {
                                valueGivenClass++;
                            }
                        }
                        else
                        {
                            notClassCounter++;
                            if (featureValue == value)
                            {
                                valueGivenNotClass++;
                            }
                        }
                    }
                    var d = ((valueGivenClass / classCounter) - (valueGivenNotClass / notClassCounter)) / (feature.Count);
                    if (d <= highest) continue;
                    highest = d;
                    featureIndex = counter;
                }
                counter++;
            }
            return featureIndex;
        }

        public double Similarty(PatientCase storedCase, PatientCase queryCase)
        {
            var sim = queryCase.FeatureVector.Where((t, i) => t != -1 && storedCase.FeatureVector[i] == t).Count();

            return (double)sim / storedCase.FeatureVector.Length;
        }

        public void FindNewRetrievalSet()
        {
            RetrievalSet = new List<PatientCase>();
            var similiartyMap = CaseBase.ToDictionary(patientCase => patientCase, patientCase => Similarty(patientCase, CurrentPatientCase));
            similiartyMap = similiartyMap.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            foreach (var patientCase in similiartyMap)
            {
                var count = similiartyMap.Count(otherCase => otherCase.Value > patientCase.Value);
                if (count <= K)
                {
                    RetrievalSet.Add(patientCase.Key);
                }
            }
        }

        public void LeaveOneOutTest()
        {
            var index = _rnd.Next(CaseBase.Count);
            var leftOut = CaseBase.ElementAt(index);
            CurrentPatientCase = new PatientCase
            {
                FeatureVector = Enumerable.Repeat(-1, CaseBase.First().FeatureVector.Length).ToArray()
            };
            var decided = false;

            RetrievalSet = CaseBase;
            
            while (!decided)
            {
                var targetClass = RetrievalSet.First().ClassModel.Class;
                foreach (var patientCase in RetrievalSet)
                {

                }
                //TODO: FINISH THIS....
                
            }

        }

    }
}
