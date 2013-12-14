using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Master.Models;

namespace Master.Implementation
{
    public class NaCoDAE
    {
        public List<PatientCase> CaseBase { get; set; }
        public List<PatientCase> QueryCases { get; set; }
        public int MaxSymptoms { get; set; }
        public PatientCase CurrentQueryCase { get; set; }
        public List<PatientCase> RetrievalSet { get; set; }
        public List<ScoreObject> InitialScores { get; set; }
        public List<ScoreObject> Scores { get; set; }
        public List<int> CurrentQuery { get; set; }

        public NaCoDAE()
        {
            CaseBase = PatientCaseRepository.GetAll();
            QueryCases = PatientCaseRepository.GetAll();
        }

        public void GetMaxSymtoms(int startSymptom = 5)
        {
            var max = 0;

            foreach (var patientCase in CaseBase)
            {
                var numOfSymptoms = 0;
                for (int i = startSymptom; i < patientCase.FeatureVector.Length; i++)
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
                var ageDiff = Math.Abs(patientCase.FeatureVector[0] - CurrentQueryCase.FeatureVector[0]);
                var bmiDiff = Math.Abs(patientCase.FeatureVector[4] - CurrentQueryCase.FeatureVector[4]);
                var sameSex = patientCase.FeatureVector[1] != -9 && CurrentQueryCase.FeatureVector[1] != -9 && patientCase.FeatureVector[1] == CurrentQueryCase.FeatureVector[1] ? 0.2 : 0.0;
                var sameTobacco = patientCase.FeatureVector[3] == CurrentQueryCase.FeatureVector[3] ? 0.2 : 0.0;
                var sameRace = patientCase.FeatureVector[2] == CurrentQueryCase.FeatureVector[2] ? 0.2 : 0.0;

                var score = (0.2 - (ageDiff * 0.04)) + (0.2 - (bmiDiff * 0.04)) + sameSex + sameTobacco + sameRace;

                scores.Add(new ScoreObject { Case = patientCase, Score = score });
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

                var queryScore = (sameAnswers - differentAnswers) / QuestionAnswerPairs(patientCase);
                var unbiasedScore = GetUnbiasedScore(queryScore, 0.5);
                var initialScore = GetInitialScoreForClass(patientCase);

                scores.Add(new ScoreObject { Case = patientCase, Score = initialScore + unbiasedScore });
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
            return (alpha * Math.Max(0, queryScore) + ((1 - alpha) * MaxSymptoms)) / (alpha + ((1 - alpha) * MaxSymptoms));
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

            if (query.Count < numOfSymptoms)
            {
                var symptoms = new int[numOfSymptoms];

                foreach (var patientCase in cases)
                {
                    for (var i = 0; i < numOfSymptoms; i++)
                    {
                        if (patientCase.FeatureVector[i + 5] == 1) symptoms[i]++;
                    }
                }

                var maxValue = 0;
                var maxIndex = 0;
                for (var i = 0; i < symptoms.Length; i++)
                {
                    if (symptoms[i] > maxValue && !query.Contains(i + 5))
                    {
                        maxValue = symptoms[i];
                        maxIndex = i;
                    }
                }

                return maxValue == 0 ? -1 : maxIndex + 5;
            }

            return -1;
        }

        public LeaveOneOutResultModel LeaveOneOutTest(int takeFromScores = 10, int rounds = 1, double treshholdBestScore = 0.0, double treshholdAverageScore = 0.0)
        {
            var correct = 0;
            var numberOfcases = 0;
            var numOfQuestionsAsked = 0.0;
            var caseBaseCount = 0;
            var scoreOfRetrievelSet = 0.0;
            var averageScore = 0.0;
            var score = 0.0;

            for (int i = 0; i < rounds; i++)
            {

                GetMaxSymtoms();

                foreach (var patientCase in QueryCases)
                {
                    CurrentQueryCase = patientCase;
                    InitialSimilarity();

                    RetrievalSet = InitialScores.OrderByDescending(x => x.Score).Take(takeFromScores).Select(x => x.Case).ToList();
                    CurrentQuery = new List<int>();
                    var tempScore = 0.0;
                    var bestTempScore = 0.0;
                    var questionCounter = 0.0;
                    while (true)
                    {
                        var nextQuestion = GetNextQuestionIndex(RetrievalSet, CurrentQuery);
                        if (nextQuestion == -1) break;

                        CurrentQuery.Add(nextQuestion);
                        QuerySimilarity();
                        numOfQuestionsAsked++;
                        var ordered = Scores.OrderByDescending(x => x.Score).Select(x => new { x.Case, x.Score }).ToList();
                        var test2 = Scores.OrderByDescending(x => x.Score).Take(takeFromScores).Select(x => new {x.Case, x.Score}).ToList();
                        RetrievalSet = test2.Select(x => x.Case).ToList();

                        tempScore += test2.Select(s => s.Score).Sum()/test2.Count;
                        bestTempScore += test2.Select(x => x.Score).First();
                        questionCounter++;

                        if (treshholdBestScore != 0.0)
                        {
                            var tt = ordered;
                            if (test2.Select(x => x.Score).First() >= treshholdBestScore) break;
                        }
                        if (treshholdAverageScore != 0.0)
                        {
                            if ((test2.Select(x => x.Score).Sum()/test2.Count) >= treshholdAverageScore) break;
                        }
                    }
                   // var averageScoreCase = (double) tempScore/questionCounter;

                    averageScore = tempScore/questionCounter;
                    score = bestTempScore/questionCounter;

                    numberOfcases++;
                    //if (RetrievalSet.ElementAt(0).ClassModel == CurrentQueryCase.ClassModel) correct++;
                    if (
                        RetrievalSet.Select(x => x.ClassModel)
                            .Take(3).ToList()
                            
                            .Contains(CurrentQueryCase.ClassModel)) correct ++;
                }

            }
            var test = (CaseBase.Count * (CaseBase.ElementAt(0).FeatureVector.Length - 5));


            var effectiveness = numOfQuestionsAsked / (QueryCases.Count * (CaseBase.ElementAt(0).FeatureVector.Length - 5));
            
            return new LeaveOneOutResultModel()
            {
                NumberOfMatchingCases = Math.Round((double)correct/(double)rounds,2),
                CaseBaseCount = Math.Round(CaseBase.Count/(double)rounds,2),
                Effectiveness = Math.Round(effectiveness/(double)rounds,2),
                NumberOfQuestionsAsked = Math.Round((numOfQuestionsAsked/QueryCases.Count)/(double)rounds,2),
                PercentCorrect = Math.Round(((double)correct / (double)QueryCases.Count)/(double)rounds,2),
                TakeFromScores = takeFromScores,
                Score = Math.Round(score,2),
                AverageScore = Math.Round(averageScore,2)

            };

        }
    }

    public class ScoreObject
    {
        public PatientCase Case { get; set; }
        public double Score { get; set; }
    }

    public class LeaveOneOutResultModel
    {
        public double NumberOfMatchingCases { get; set; }
        public double Effectiveness { get; set; }

        public double NumberOfQuestionsAsked { get; set; }
        public double CaseBaseCount { get; set; }
        public double PercentCorrect { get; set; }
        public double TakeFromScores { get; set; }
        public double Score { get; set; }
        public double AverageScore { get; set; }
        public double ScoreOfRetrievelSet { get; set; }

       

    }
    public class HighchartModel
    {
        public string Name { get; set; }
        public object[] Data { get; set; }
    }
}
