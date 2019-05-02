using BinarApp.API.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace BinarApp.API.Controllers
{
    public class EquipmentStaffUploadController : ApiController
    {
        private BinarContext _cntx = new BinarContext();

        [HttpPost]
        public async Task<IHttpActionResult> UploadFile(int equipmentId, int timeFlag, FormDataCollection formData)
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                var docfiles = new List<string>();
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];

                    using (var ms = new MemoryStream())
                    {
                        postedFile.InputStream.CopyTo(ms);

                        var fileBytes = ms.ToArray();

                        var fileBase64 = Convert.ToBase64String(fileBytes);

                        var equip = _cntx.Equipments.First(x => x.Id == equipmentId);

                        if (timeFlag == 0)
                            equip.DayImage = fileBase64;
                        else
                            equip.NightImage = fileBase64;

                        await _cntx.SaveChangesAsync();

                    }
                }
                result = Request.CreateResponse(HttpStatusCode.Created, docfiles);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }


            return Ok();
        }
    }
}
