using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Master.Implementation;

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
            ViewBag.Message = "Your application description page.";

            ReadFile();

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public void ReadFile()
        {
            //RawPatientFiles.ReadFile.Main();

            var test = PatientCaseRepository.GetAll();

            var test2 = test.Where(x => x.Symptoms.Contains("10120")).ToList();

            var test3 = test2.Count;
        }
    }
}