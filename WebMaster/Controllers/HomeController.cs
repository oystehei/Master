using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
            var test = new iNNk("Local", 1);
            test.LeaveOneOutTest();


            return View();
        }

        [HttpPost]
        public ActionResult About(string code)
        {
            if (code == null) return View();

            var matchingCases = PatientCaseRepository.GetAll().Where(x => x.Symptoms.Contains(code)).ToList();
            var model = new AboutViewModel {Code = code, MatchingCases = matchingCases};

            return View(model);
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

            var list = PatientCaseRepository.GetAll();
            foreach (var patientCase in list.Where(x => x.ClassModel.Class == 251))
            {
                PatientCaseRepository.Delete(patientCase.Id);
            }
           

            return View();
        }

        public void ReadFile()
        {
            //RawPatientFiles.ReadFile.Main();

            var test = PatientCaseRepository.GetAll();
        }
    }
}
