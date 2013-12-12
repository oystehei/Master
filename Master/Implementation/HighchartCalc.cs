using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;

namespace Master.Implementation
{
    public static class HighchartCalc
    {

        public static DotNet.Highcharts.Options.Series[] GetDataForFirstComeFirstComeHighchartModel(int from, double thBest, double thBestRange,
            double thBestTicks, bool bestAverage = false)
        {
            if (thBest > thBestRange) return null;
            var naCoDAE = new NaCoDAE();

          

            var numberOfTicks = 0;
            var tempTheBest = thBest;
            while (tempTheBest <= thBestRange)
            {
                numberOfTicks++;
                tempTheBest += thBestTicks;
            }
            var correctPercentage = new object[numberOfTicks];
            var efficiant = new object[numberOfTicks];
         
            numberOfTicks = 0;
            while (thBest<=thBestRange)
            {
                var tt = bestAverage ? 0.0 : thBest;
                var ta = bestAverage ? thBest : 0.0;
                var model = naCoDAE.LeaveOneOutTest(from, 1, tt, ta);
                correctPercentage[numberOfTicks] = model.PercentCorrect;
                efficiant[numberOfTicks] = model.Effectiveness;
                numberOfTicks++;
                thBest += thBestTicks;
            }
            var rr = new DotNet.Highcharts.Options.Series[2];
            var percentCorrect = new Series()
            {
                Name = "PercentCorrect",
                Data = new Data(correctPercentage)
            };
            var effectiveness = new Series()
            {
                Name = "Effectiveness",
                Data = new Data(efficiant)
            };
            rr[0] = percentCorrect;
            rr[1] = effectiveness;

            return rr;
        }
    }
}
