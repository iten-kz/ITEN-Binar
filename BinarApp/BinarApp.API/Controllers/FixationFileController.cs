using BinarApp.API.Models;
using BinarApp.Core.Export;
using BinarApp.Core.POCO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web.Http;

namespace BinarApp.API.Controllers
{
    public class FixationFileController : ApiController
    {
        private DatConverter _datConverter = new DatConverter();

        public HttpResponseMessage Get(int id)
        {
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);

            var sb = new StringBuilder();

            try
            {
                var fileName = string.Empty;

                using (var cntx = new BinarContext())
                {
                    var fds = cntx.Fixations
                        .First(x => x.Id == id);

                    sb.Append("1;");

                    fileName = GetFileName(fds.FixationDate, fds.GRNZ);

                    sb.Append("2;");

                    var path = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/" + fileName);

                    var folder = System.Web.Hosting.HostingEnvironment.MapPath("~/App_Data/");

                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }

                    sb.Append("3;");

                    if (!File.Exists(path))
                    {
                        sb.Append("4;");

                        var files = cntx.FixationDetails
                        .Where(x => x.FixationId == fds.Id)
                        .ToList();

                        var plates = new List<PlateModel>();

                        files.ForEach(item => 
                        {
                            plates.Add(new PlateModel()
                            {
                                ImageBase64 = item.Image,
                                ImagePlateBase64 = item.ImagePlate
                            });
                        });

                        var symbolBase64 = string.Empty;

                        var fixationHour = fds.FixationDate.Hour;

                        if (9 < fixationHour && fixationHour < 20)
                            symbolBase64 = fds.Equipment.DayImage;
                        else
                            symbolBase64 = fds.Equipment.NightImage;

                        var datBytes = _datConverter.GetDatFileAsByteArray(fds.FixationDate, fds.GRNZ, symbolBase64, fds.Equipment.Description, plates);

                        //var total = new List<byte>();

                        //var fileHeader = GetFileHeader(fds.FixationDate, fds.GRNZ);

                        sb.Append("5;");

                        //var fileBody = GetFileBody(files);

                        sb.Append("6;");

                        //total.AddRange(fileHeader);
                        //total.AddRange(fileBody);

                        var fs = File.Create(path);

                        sb.Append("7;");

                        fs.Close();
                        File.WriteAllBytes(path, datBytes);

                        sb.Append("8;");
                    }

                    var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                    result.Content = new StreamContent(stream);
                    result.Content.Headers.ContentType =
                        new MediaTypeHeaderValue("application/octet-stream");
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = fileName
                    };

                    //stream.Close();
                    //File.Delete(path);

                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                }
                
            }
            catch (Exception ex)
            {
                result.Content = new StringContent(sb.ToString() + "|" + ex.Message);
            }

            return result;
        }

        private string GetFileName(DateTime fixationDate, string GRNZ)
        {
            return string.Format("{0}_{1}_{2}_{4}_{5}_{6}_{7}.{8}",
                    fixationDate.ToString("yyyyMMddHHmmssfff"),
                    "I0001", //"localNo",
                    1, //"Lane",
                    1, //"LaserNum",
                    ConvertLenghtToString(GRNZ, 12, " "),
                    "000", // speed
                    5, //"ViolationCode",
                    1, //"ViolationSignal",
                    "dat");
        }

        private string ConvertLenghtToString(string lenght, int countStr, string symbol)
        {
            var sb = new StringBuilder();
            
            var needSybmolCount = countStr - lenght.Length - 1;
            
            for (int i = 0; i <= needSybmolCount; i++)
            {
                sb.Append(symbol);
            }
            
            sb.Append(lenght);
            
            return sb.ToString();
        }

    }
}
