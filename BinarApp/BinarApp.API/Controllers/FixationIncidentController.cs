using BinarApp.API.Models;
using BinarApp.Core.Interfaces;
using BinarApp.Core.Models;
using BinarApp.Core.POCO;
using BinarApp.Core.Provider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace BinarApp.API.Controllers
{
    public class FixationIncidentController : ApiController
    {
        private IBlobStorageProvider _blobStorageProvider = new BlobStorageProvider();

        [HttpGet]
        [Route("api/FixationIncident")]
        public IHttpActionResult Get()
        {
            using (var cntx = new BinarContext())
            {
                var fixations = cntx.Fixations.ToList();
            }

            return Ok("s1");
        }

        [HttpPost]
        [Route("api/FixationIncident")]
        public async Task<IHttpActionResult> Post([FromBody] ICollection<FixationIncidentViewModel> model)
        {
            using (var cntx = new BinarContext())
            {
                var res = new List<Fixation>();
                foreach (var item in model)
                {
                    var nItem = new Fixation()
                    {
                        FixationDate = item.DateStart,
                        PenaltySum = 0,
                        GRNZ = item.PlateNumber,
                        BirthDate = DateTime.Now,
                        EquipmentId = item.EquipmentId,
                        FixationDetails = new List<FixationDetail>()
                        {
                            new FixationDetail() { Image = item.FirstImage, Date = item.DateStart, ImagePlate = item.FirstImagePlate },
                            new FixationDetail() { Image = item.LastImage, Date = item.DateFinish, ImagePlate = item.LastImagePlate },
                        }
                    };
                    res.Add(nItem);
                }

                cntx.Fixations.AddRange(res);

                await cntx.SaveChangesAsync();
            }

            return Ok();
        }
    }
}
