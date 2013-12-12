using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using DotNet.Highcharts;
using DotNet.Highcharts.Enums;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;
using Master.Implementation;
using WebMaster.Models;

namespace WebMaster.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            //var test = new iNNk("Global", 1);
            //test.LeaveOneOutTest();

            //var test = new NaCoDAE();
            //test.LeaveOneOutTest();

            return View(new AboutViewModel{From = 1, To = 5,NumberOfRounds = 1, ThAverage = 1.8, ThBest = 1.9});
        }

        [HttpPost]
        public ActionResult About(AboutViewModel model)
        {
           

            var naCoDAE = new NaCoDAE();

            

            var returnList = new List<LeaveOneOutResultModel>();

            for (int i = model.From; i <= model.To; i++)
            {

                returnList.Add(naCoDAE.LeaveOneOutTest(i, model.NumberOfRounds, model.ThBest, model.ThAverage));
            }
           // var matchingCases = PatientCaseRepository.GetAll().Where(x => x.Symptoms.Contains(code)).ToList();
            model.LeaveOneOutResultModel = returnList;
            

            return View(model);
        }


        public ActionResult GetChart(int from, double thBest, double thBestRange, double ticks )
        {
            var highchart = new DotNet.Highcharts.Highcharts("chart")
                .SetXAxis(new XAxis
                {
                    
                    //DateTimeLabelFormats = new DateTimeLabel() { Day = "%d" }

                })
                .SetYAxis(new YAxis()
                {
                    Title = new YAxisTitle() {Text = "Kr"}
                })
                .SetSeries(
                    HighchartCalc.GetDataForFirstComeFirstComeHighchartModel(from,thBest,thBestRange,ticks)
                )
                .SetTitle(new Title() {Text = "tt", VerticalAlign = VerticalAligns.Top})
                .SetPlotOptions(new PlotOptions()
                {
                    Line = new PlotOptionsLine()
                    {
                        Animation = new Animation(new AnimationConfig() {Duration = 2000, Easing = EasingTypes.Swing}),
                        PointStart = new PointStart(thBest),
                        PointInterval = ticks
                    },
                   
                    
                });

            return PartialView(highchart);
        }

        public ActionResult GetChart2()
        {
            Highcharts chart = new Highcharts("chart")
                .InitChart(new Chart
                {
                    DefaultSeriesType = ChartTypes.Line,
                    MarginRight = 130,
                    MarginBottom = 25,
                    ClassName = "chart"
                })
                .SetTitle(new Title
                {
                    Text = "Monthly Average Temperature",
                    X = -20
                })
                .SetSubtitle(new Subtitle
                {
                    Text = "Source: WorldClimate.com",
                    X = -20
                })
                .SetXAxis(new XAxis { Categories = ChartsData.Categories })
                .SetYAxis(new YAxis
                {
                    Title = new YAxisTitle { Text = "Temperature (°C)" },
                    PlotLines = new[]
                            {
                                new YAxisPlotLines
                                    {
                                        Value = 0,
                                        Width = 1,
                                        Color = ColorTranslator.FromHtml("#808080")
                                    }
                            }
                })
                .SetTooltip(new Tooltip
                {
                    Formatter = @"function() {
                                        return '<b>'+ this.series.name +'</b><br/>'+
                                    this.x +': '+ this.y +'°C';
                                }"
                })
                .SetLegend(new Legend
                {
                    Layout = Layouts.Vertical,
                    Align = HorizontalAligns.Right,
                    VerticalAlign = VerticalAligns.Top,
                    X = -10,
                    Y = 100,
                    BorderWidth = 0
                })
                .SetSeries(new[]
                    {
                        new Series { Name = "Tokyo", Data = new Data(ChartsData.TokioData) },
                        new Series { Name = "New York", Data = new Data(ChartsData.NewYorkData) },
                        new Series { Name = "Berlin", Data = new Data(ChartsData.BerlinData) },
                        new Series { Name = "London", Data = new Data(ChartsData.LondonData) }
                    }
                );

            return PartialView(chart);
        }

        public class ChartsData
        {
            public static object[] BerlinData = new object[]
            {-0.9, 0.6, 3.5, 8.4, 13.5, 17.0, 18.6, 17.9, 14.3, 9.0, 3.9, 1.0};

            public static string[] Categories = new[]
            {"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"};

            public static object[] LondonData = new object[]
            {3.9, 4.2, 5.7, 8.5, 11.9, 15.2, 17.0, 16.6, 14.2, 10.3, 6.6, 4.8};

            public static object[] NewYorkData = new object[]
            {-0.2, 0.8, 5.7, 11.3, 17.0, 22.0, 24.8, 24.1, 20.1, 14.1, 8.6, 2.5};

            public static object[] TokioData = new object[]
            {7.0, 6.9, 9.5, 14.5, 18.2, 21.5, 25.2, 26.5, 23.3, 18.3, 13.9, 9.6};
        }

        public void AddNewClassesToModel()
        {
            string[] list = new string[]
            {
                "Intestinal infectious diseases (001–009)", "Tuberculosis (010–018)",
                "Zoonotic bacterial diseases (020–027)", "Other bacterial diseases (030–041)",
                "Human immunodeficiency virus (HIV) infection (042–044)",
                "Poliomyelitis and other non-arthropod-borne viral diseases of central nervous system (045–049)",
                "Viral diseases accompanied by exanthem (050–059)", "Arthropod-borne viral diseases (060–066)",
                "Other diseases due to viruses and chlamydiae (070–079)",
                "Rickettsioses and other arthropod-borne diseases (080–088)",
                "Syphilis and other venereal diseases (090–099)", "Other spirochetal diseases (100–104)",
                "Mycoses (110–118)", "Helminthiases (120–129)", "Other infectious and parasitic diseases (130–136)",
                "Late effects of infectious and parasitic diseases (137–139)",
                "Malignant neoplasm of lip, oral cavity, and pharynx (140–149)",
                "Malignant neoplasm of digestive organs and peritoneum (150–159)",
                "Malignant neoplasm of respiratory and intrathoracic organs (160–165)",
                "Malignant neoplasm of bone, connective tissue, skin, and breast (170–175)",
                "Kaposi's sarcoma (176–176)", "Malignant neoplasm of genitourinary organs (179–189)",
                "Malignant neoplasm of other and unspecified sites (190–199)",
                "Malignant neoplasm of lymphatic and hematopoietic tissue (200–208)", "Neuroendocrine tumors (209–209)",
                "Benign neoplasms (210–229)", "Carcinoma in situ (230–234)", "Neoplasms of uncertain behavior (235–238)",
                "Neoplasms of unspecified nature (239–239)",
                "Disorders of thyroid gland (240–246)", "Diseases of other endocrine glands (249–259)",
                "Nutritional deficiencies (260–269)", "Other metabolic and immunity disorders (270–279)",
                "Diseases of the blood and blood-forming organs (280–289)",
                "Organic psychotic conditions (290–294)", "Other psychoses (295–299)", "Neurotic disorders (300)",
                "Personality disorders (301)", "Psychosexual disorders (302)", "Psychoactive substance (303–305)",
                "Other (primarily adult onset) (306–311)", "Mental disorders diagnosed in childhood (312–316)",
                "Inflammatory diseases of the central nervous system (320–327)",
                "Hereditary and degenerative diseases of the central nervous system (330–337)", "Pain (338–338)",
                "Other headache syndromes (339–339)", "Other disorders of the central nervous system (340–349)",
                "Disorders of the peripheral nervous system (350–359)",
                "Disorders of the eye and adnexa (360–379)", "Diseases of the ear and mastoid process (380–389)",
                "Acute Rheumatic Fever (390–392)", "Chronic rheumatic heart disease (393–398)",
                "Hypertensive disease (401–405)", "Ischemic heart disease (410–414)",
                "Diseases of pulmonary circulation (415–417)", "Other forms of heart disease (420–429)",
                "Cerebrovascular disease (430–438)", "Diseases of arteries, arterioles, and capillaries (440–448)",
                "Diseases of veins and lymphatics, and other diseases of circulatory system (451–459)",
                "Acute respiratory infections (460–466)", "Other diseases of the upper respiratory tract (470–478)",
                "Pneumonia and influenza (480–488)",
                "Chronic obstructive pulmonary disease and allied conditions (490–496)",
                "Pneumoconioses and other lung diseases due to external agents (500–508)",
                "Other diseases of respiratory system (510–519)",
                "Diseases of oral cavity, salivary glands, and jaws (520–529)",
                "Diseases of esophagus, stomach, and duodenum (530–537)", "Appendicitis (540–543)",
                "Hernia of abdominal cavity (550–553)", "Noninfectious enteritis and colitis (555–558)",
                "Other diseases of intestines and peritoneum (560–569)", "Other diseases of digestive system (570–579)",
                "Nephritis, nephrotic syndrome, and nephrosis (580–589)", "Other diseases of urinary system (590–599)",
                "Diseases of male genital organs (600–608)", "Disorders of breast (610–611)",
                "Inflammatory disease of female pelvic organs (614–616)",
                "Other disorders of female genital tract (617–629)",
                "Ectopic and molar pregnancy (630–633)", "Other pregnancy with abortive outcome (634–639)",
                "Complications mainly related to pregnancy (640–649)",
                "Normal delivery, and other indications for care in pregnancy, labor, and delivery (650–659)",
                "Complications occurring mainly in the course of labor and delivery (660–669)",
                "Complications of the puerperium (670–676)", "Other maternal and fetal complications (678–679)",
                "Infections of skin and subcutaneous tissue (680–686)",
                "Other inflammatory conditions of skin and subcutaneous tissue (690–698)",
                "Other diseases of skin and subcutaneous tissue (700–709)",
                "Arthropathies and related disorders (710–719)", "Dorsopathies (720–724)",
                "Rheumatism, excluding the back (725–729)",
                "Osteopathies, chondropathies, and acquired musculoskeletal deformities (730–739)",
            "Congenital anomalies (740–759)",
                "Maternal causes of perinatal morbidity and mortality (760–763)",
                "Other conditions originating in the perinatal period (764–779)",
                "Symptoms (780–789)", "Nonspecific abnormal findings (790–796)",
                "Ill-defined and unknown causes of morbidity and mortality (797–799)",
                "Fracture of skull (800–804)", "Fracture of neck and trunk (805–809)",
                "Fracture of upper limb (810–819)", "Fracture of lower limb (820–829)", "Dislocation (830–839)",
                "Sprains and strains of joints and adjacent muscles (840–848)",
                "Intracranial injury, excluding those with skull fracture (850–854)",
                "Internal injury of thorax, abdomen, and pelvis (860–869)",
                "Open wound of head, neck, and trunk (870–879)", "Open wound of upper limb (880–887)",
                "Open wound of lower limb (890–897)", "Injury to blood vessels (900–904)",
                "Late effects of injuries, poisonings, toxic effects, and other external causes (905–909)",
                "Superficial injury (910–919)", "Contusion with intact skin surface (920–924)",
                "Crushing injury (925–929)", "Effects of foreign body entering through Body orifice (930–939)",
                "Burns (940–949)", "Injury to nerves and spinal cord (950–957)",
                "Certain traumatic complications and unspecified injuries (958–959)",
                "Poisoning by drugs, medicinal and biological substances (960–979)",
                "Toxic effects of substances chiefly nonmedicinal as to source (980–989)",
                "Other and unspecified effects of external causes (990–995)",
                "Complications of surgical and medical care, not elsewhere classified (996–999)"
            };

            //  ClassRepository.AddWithList(list);
            
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            var test = PatientCaseRepository.ClusterCases();

            return View();
        }

        public void ReadFile()
        {
            //RawPatientFiles.ReadFile.Main();

            var test = PatientCaseRepository.GetAll();
        }
    }
}
