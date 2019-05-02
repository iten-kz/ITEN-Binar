using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BinarApp.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //throw new ArgumentOutOfRangeException(_message);
            return View();
        }

        public ActionResult Equipments()
        {
            return View();
        }

        public ActionResult UploadEquipment(int id, int timeFlag)
        {
            ViewBag.TimeFlag = timeFlag;

            return View(id);
        }

        public ActionResult MatchEquipmentToMap()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult FixationFile(int id)
        {


            return File("", "");
        }

    }
}