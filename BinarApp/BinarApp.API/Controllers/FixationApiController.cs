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
    public class FixationApiController : ApiController
    {
        private IBlobStorageProvider _blobStorageProvider;
        

        public FixationApiController(IBlobStorageProvider blobStorageProvider)
        {
            _blobStorageProvider = blobStorageProvider;
        }

        [HttpPost]
        //[Authorize]
        public async Task<IHttpActionResult> Post([FromBody]ICollection<FixationVM> fixationVMList)
        {
            if (fixationVMList == null || !fixationVMList.Any())
                return BadRequest();
            var newFixationList = new List<Fixation>();
            using (var dbCtx = new BinarContext())
            {
                foreach (var fixationVM in fixationVMList)
                {
                    var newFixation = new Fixation()
                    {
                        Description = fixationVM.Description,
                        FixationDate = fixationVM.FixationDate,
                        GRNZ = fixationVM.GRNZ,
                        Speed = fixationVM.Speed,
                    };

                    var intruder = dbCtx.Intruders.FirstOrDefault(x => x.IIN == fixationVM.IntruderIIN);
                    if(intruder == null)
                    {
                        var blobResult = _blobStorageProvider.Upload(fixationVM.ImgBase64, fixationVM.IntruderIIN);
                        intruder = new Intruder()
                        {
                            IIN = fixationVM.IntruderIIN,
                            BirthDate = fixationVM.IntruderBirthDate,
                            FirstName = fixationVM.IntruderFirstName,
                            LastName = fixationVM.IntruderLastName,
                            MiddleName = fixationVM.IntruderMiddleName, 
                            ImageBlobName = blobResult.BlobName,
                            ImageUrl = blobResult.Url
                        };                                                
                    }
                    //newFixation.Intruder = intruder;
                    newFixationList.Add(newFixation);
                }
                dbCtx.Fixations.AddRange(newFixationList);
                await dbCtx.SaveChangesAsync();
            }
            return Ok();            
        }
    }
}
