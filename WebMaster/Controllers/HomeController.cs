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

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public void ReadFile()
        {
            //RawPatientFiles.ReadFile.Main();

            var test = PatientCaseRepository.GetAll();
        }
    }
}