using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Master.Models
{
    public class PatientCase
    {
        [Key]
        public int Id { get; set; }

        public int Age { get; set; }
        public int Sex { get; set; }
        public int Race { get; set; }
        public int TobaccoUse { get; set; }
        public double Bmi { get; set; }
        public string Symptoms { get; set; }
        public string Diagnose { get; set; }
        public virtual ClassModel ClassModel { get; set; }
        public int[] FeatureVector { get; set; }

    }
}
