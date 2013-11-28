using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Master.Models;

namespace Master.Implementation
{
    public class NaCoDAE
    {
        public List<PatientCase> CaseBase { get; set; }
        public int MaxSymptoms { get; set; }
        public PatientCase CurrentQueryCase { get; set; }
        public List<PatientCase> RetrievalSet { get; set; }
        public List<ScoreObject> InitialScores { get; set; }
        public List<ScoreObject> Scores { get; set; }
        public List<int> CurrentQuery { get; set; } 

        public NaCoDAE()
        {
            CaseBase = PatientCaseRepository.ClusterCases();
        }

        public void GetMaxSymtoms()
        {
            var max = 0;

            foreach (var patientCase in CaseBase)
            {
                var numOfSymptoms = 0;
                for (int i = 5; i < patientCase.FeatureVector.Length; i++)
                {
                    if (patientCase.FeatureVector[i] == 1) numOfSymptoms++;
                }

                if (numOfSymptoms > max) max = numOfSymptoms;
            }

            MaxSymptoms = max;
        }

        public void InitialSimilarity()
        {
            var scores = new List<ScoreObject>();

            foreach (var patientCase in CaseBase)
            {
                //if (patientCase != CurrentQueryCase)
                //{
                    var ageDiff = Math.Abs(patientCase.Age - CurrentQueryCase.Age);
                    var bmiDiff = Math.Abs(patientCase.Bmi - CurrentQueryCase.Bmi);
                    var sameSex = patientCase.Sex != -9 && CurrentQueryCase.Sex != -9 && patientCase.Sex == CurrentQueryCase.Sex ? 0.2 : 0.0;
                    var sameTobacco = patientCase.TobaccoUse == CurrentQueryCase.TobaccoUse ? 0.2 : 0.0;
                    var sameRace = patientCase.Race == CurrentQueryCase.Race ? 0.2 : 0.0;

                    var score = (0.2 - (ageDiff*0.04)) + (0.2 - (bmiDiff*0.05)) + sameSex + sameTobacco + sameRace;

                    scores.Add(new ScoreObject{Case = patientCase, Score = score}); 
                //}
            }

            InitialScores = scores;
        }

        

        public void QuerySimilarity()
        {
            var scores = new List<ScoreObject>();

            foreach (var patientCase in CaseBase)
            {
                //if (patientCase != CurrentQueryCase)
                //{
                    var sameAnswers = 0.0;
                    var differentAnswers = 0.0;

                    foreach (var i in CurrentQuery)
                    {
                        if (patientCase.FeatureVector[i] != -9 && CurrentQueryCase.FeatureVector[i] != -9)
                        {
                            if (patientCase.FeatureVector[i] == CurrentQueryCase.FeatureVector[i]) sameAnswers++;
                            else differentAnswers++;
                        }
                    }

                    var queryScore = (sameAnswers - differentAnswers)/QuestionAnswerPairs(patientCase);
                    var unbiasedScore = GetUnbiasedScore(queryScore, 0.5);
                    var initialScore = GetInitialScoreForClass(patientCase);

                    scores.Add(new ScoreObject { Case = patientCase, Score =  initialScore + unbiasedScore});
                //}
            }

            Scores = scores;
        }
        
        public double GetInitialScoreForClass(PatientCase diagnoseClass)
        {
            return InitialScores.FirstOrDefault(x => x.Case == diagnoseClass).Score;
        }

        public double GetUnbiasedScore(double queryScore, double alpha)
        {
            return (alpha*Math.Max(0, queryScore) + ((1 - alpha)*MaxSymptoms))/(alpha + ((1 - alpha)*MaxSymptoms));
        }

        public int QuestionAnswerPairs(PatientCase patientCase)
        {
            var pairs = 0;

            for (var i = 5; i < patientCase.FeatureVector.Length; i++)
            {
                if (patientCase.FeatureVector[i] == 1) pairs++;
            }

            return pairs;
        }

        public int GetNextQuestionIndex(List<PatientCase> cases, List<int> query)
        {
            var numOfSymptoms = cases.ElementAt(0).FeatureVector.Length - 5;

            if (query.Count < numOfSymptoms) { 
                var symptoms = new int[numOfSymptoms];

                foreach (var patientCase in cases)
                {
                    for (var i = 0; i < numOfSymptoms; i++)
                    {
                        if (patientCase.FeatureVector[i+5] == 1) symptoms[i]++;
                    }
                }

                var maxValue = -1;
                var maxIndex = 0;
                for (var i=0; i < symptoms.Length; i++)
                {
                    if (symptoms[i] > maxValue && !query.Contains(i+5))
                    {
                        maxValue = symptoms[i];
                        maxIndex = i;
                    }
                }

                return maxValue == -1 ? -1 : maxIndex + 5;
            }

            return -1;
        }

        public void LeaveOneOutTest()
        {
            var correct = 0;
            var numOfQuestionsAsked = 0.0;
            
            GetMaxSymtoms();

            foreach (var patientCase in CaseBase)
            {
                CurrentQueryCase = patientCase;
                InitialSimilarity();

                RetrievalSet = InitialScores.OrderByDescending(x => x.Score).Take(10).Select(x => x.Case).ToList();
                CurrentQuery = new List<int>();

                while (true)
                {
                    var nextQuestion = GetNextQuestionIndex(RetrievalSet, CurrentQuery);
                    if (nextQuestion == -1) break;
                    numOfQuestionsAsked++;

                    CurrentQuery.Add(nextQuestion);
                    QuerySimilarity();

                    RetrievalSet = Scores.OrderByDescending(x => x.Score).Take(10).Select(x => x.Case).ToList();
                }

                if (RetrievalSet.ElementAt(0).ClassModel == CurrentQueryCase.ClassModel) correct++;
            }

            var test = (CaseBase.Count*(CaseBase.ElementAt(0).FeatureVector.Length - 5));

            var effectiveness = numOfQuestionsAsked/(CaseBase.Count*(CaseBase.ElementAt(0).FeatureVector.Length - 5));

        }
    }

    public class ScoreObject
    {
        public PatientCase Case { get; set; }
        public double Score { get; set; }
    }
}
