using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BinarApp.Web.Models
{
    public class EquipmentFileUploadViewModel
    {
        public int EquipmentId { get; set; }

        public bool IsDay { get; set; }

        public HttpPostedFileBase File { get; set; }
    }
}