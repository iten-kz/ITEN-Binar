using BinarApp.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BinarApp.Web.Controllers
{
    public class EquipmentsController : Controller
    {
        // GET: Equipments
        public ActionResult Index(int id, EquipmentItemEnum timeFlat)
        {
            var data = new EquipmentFileUploadViewModel()
            {
                EquipmentId = id,
                IsDay = true
            };

            return View();
        }

        public ActionResult Post(EquipmentFileUploadViewModel model)
        {


            return Redirect("/Home/Equipments");
        }

    }
}