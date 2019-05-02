using BinarApp.Core.POCO;
using BinarApp.DesktopClient.Converters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BinarApp.DesktopClient.Managers
{
    public class TattileCameraManager
    {
        private FileManager _fileManager;

        public string TattileBasePath { get; set; }

        public int DeltaInterval { get; set; }

        public TattileCameraManager(FileManager fileManager)
        {
            _fileManager = fileManager;
            TattileBasePath = ConfigurationManager.AppSettings["TATTILE_FILE_PATH"].ToString();
            DeltaInterval = 5;
        }

        /// <summary>
        /// Step 4
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public List<string> GetImages(DateTime date)
        {
            var res = new List<string>();

            var datePath = string.Format(@"{0}/{1}",
                date.ToString("yyyy-MM-dd"),
                date.Hour);

            var fullPath = Path.Combine(TattileBasePath, datePath);

            if (!Directory.Exists(fullPath))
                return res;

            res = Directory.GetFiles(fullPath, "*.jpg")
                .Select(Path.GetFileName)
                .ToList();

            return res;
        }

        public List<string> FilterImagesByDateTime(List<string> imageNames, DateTime dateTime)
        {
            DateTime from = dateTime.AddSeconds(-DeltaInterval);
            DateTime to = dateTime.AddSeconds(DeltaInterval);

            var result = imageNames.Select(imageName =>
            {
                var nameItem = imageName.Split('_');

                var dateStr = nameItem[0] + " " + nameItem[1];
                return new
                {
                    ImageName = imageName,
                    DateTime = DateTime.ParseExact(dateStr, "yyyy-MM-dd HH-mm-ss-fff",
                                       System.Globalization.CultureInfo.InvariantCulture)
                };
            })
            .Where(x => x.DateTime >= from && x.DateTime < to)
            .Select(x => x.ImageName)
            .ToList();

            return result;
        }

        public List<string> FilterImagesByDateTime(List<string> imageNames, DateTime dateFrom, DateTime dateTo)
        {
            var result = imageNames.Select(imageName =>
            {
                var nameItem = imageName.Split('_');

                var dateStr = nameItem[0] + " " + nameItem[1];
                return new
                {
                    ImageName = imageName,
                    DateTime = DateTime.ParseExact(dateStr, "yyyy-MM-dd HH-mm-ss-fff",
                                       System.Globalization.CultureInfo.InvariantCulture)
                };
            })
            .Where(x => x.DateTime >= dateFrom && x.DateTime < dateTo)
            .Select(x => x.ImageName)
            .ToList();

            return result;
        }

        /// <summary>
        /// Step 5
        /// </summary>
        /// <param name="filteredImages"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public string GetFilteredImageName(List<string> filteredImages, DateTime dateTime)
        {
            var data = filteredImages.Select(imageName =>
            {
                string plateNum = imageName.Split('_')[2].Split('-')[0];

                var dateString = dateTime.ToString("yyyy-MM-dd");

                string path = Path.Combine(TattileBasePath, dateString);
                string excelFilePath = Path.Combine(path, $"{dateString}{plateNum}.csv");

                int cnt = GetTotalRowCount(excelFilePath);

                return new
                {
                    ImageName = imageName,
                    Count = cnt
                };
            })
            .OrderByDescending(x => x.Count)
            .FirstOrDefault();

            return data != null ? data.ImageName : string.Empty;
        }

        public Fixation GetFixation(string imageName)
        {
            Fixation res = null;

            if (!string.IsNullOrWhiteSpace(imageName))
            {
                var data = imageName.Split('_');

                var dateStr = data[0] + " " + data[1];

                var dateTime = DateTime.ParseExact(dateStr, "yyyy-MM-dd HH-mm-ss-fff",
                                       System.Globalization.CultureInfo.InvariantCulture);

                var dateTimePath = string.Format(@"{0}/{1}",
                    dateTime.ToString("yyyy-MM-dd"),
                    dateTime.ToString("HH"));

                var fullPath = Path.Combine(TattileBasePath, dateTimePath, imageName);

                // Reduce image
                string reducedFileName = Guid.NewGuid().ToString();
                string imageSourcePath = "images/";
                string reducedImageFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    imageSourcePath,
                    $"{reducedFileName}.jpg");

                //= Path.Combine(TattileBasePath, dateTimePath, $"{reducedFileName}.jpg");
                //var image = Image.FromFile(fullPath);

                //Bitmap targetImage = null;
                //using (var sourceImage = Image.FromFile(fullPath))
                //{
                //    targetImage = new Bitmap(sourceImage.Width, sourceImage.Height,
                //      PixelFormat.Format32bppArgb);
                //    using (var canvas = Graphics.FromImage(targetImage))
                //    {
                //        canvas.DrawImageUnscaled(sourceImage, 0, 0);
                //    }
                //}

                int tryAttemptsCount = 10;
                for (int i = 0; i < tryAttemptsCount; i++)
                {
                    FileInfo fi = new FileInfo(fullPath);
                    if (!_fileManager.IsFileLocked(fi))
                    {
                        break;
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }

                //Bitmap targetImage = null;
                using (Stream s = File.Open(fullPath, FileMode.Open))
                {
                    using (Image sourceImage = Image.FromStream(s))
                    {
                        FixationImageConverter.SaveJpeg(reducedImageFullPath, sourceImage, 25);

                        byte[] imageArray = File.ReadAllBytes(reducedImageFullPath);
                        string base64 = Convert.ToBase64String(imageArray);

                        if (File.Exists(reducedImageFullPath))
                        {
                            File.Delete(reducedImageFullPath);
                        }

                        res = new Fixation()
                        {
                            GRNZ = data[2].Split('-')[0],
                            FixationDate = DateTime.SpecifyKind(dateTime, DateTimeKind.Local),
                            Image = base64
                        };
                    }

                    //if (s != null)
                    //{
                    //    if (s.Length != 0)
                    //    {
                    //        using (Image sourceImage = Image.FromStream(s))
                    //        {
                    //            targetImage = new Bitmap(sourceImage.Width, sourceImage.Height,
                    //              PixelFormat.Format32bppArgb);
                    //            using (var canvas = Graphics.FromImage(targetImage))
                    //            {
                    //                canvas.DrawImageUnscaled(sourceImage, 0, 0);
                    //            }
                    //        }
                    //    }
                    //}
                }
            }

            return res;
        }

        public int GetTotalRowCount(string filePath = "")
        {
            //DataSet ds = ReadExcelFile(filePath, true);

            int tryAttemptsCount = 10;
            for (int i = 0; i < tryAttemptsCount; i++)
            {
                FileInfo fi = new FileInfo(filePath);
                if (!_fileManager.IsFileLocked(fi))
                {
                    break;
                }
                else
                {
                    Thread.Sleep(300);
                }
            }

            int lineCounter = 0;
            using (var fileStream = new FileStream(filePath, FileMode.Open))
            using (var fileStreamReader = new StreamReader(fileStream))
            {
                while (!fileStreamReader.EndOfStream)
                {
                    var line = fileStreamReader.ReadLine();
                    lineCounter++;

                    //int result = ds.Tables[0].Rows.Count;
                }
            }

            return lineCounter;
        }

    }
}
