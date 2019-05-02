using BinarApp.Core.Models;
using BinarApp.Core.POCO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BinarApp.DesktopClient.Managers
{
    public class BinarCameraManager
    {
        private FileManager _fileManager;

        public int SpeedLimit { get; set; }

        public BinarCameraManager(FileManager fileManager)
        {
            _fileManager = fileManager;
            SpeedLimit = Convert.ToInt32(ConfigurationManager.AppSettings["SPEED_LIMIT"].ToString());
        }

        public StoryList ConvertFromStringXml(string xmlData)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(StoryList));
            using (TextReader reader = new StringReader(xmlData))
            {
                var result = (StoryList)serializer.Deserialize(reader);
                return result;
            }
        }

        /// <summary>
        /// Step 1
        /// </summary>
        /// <param name="storyList"></param>
        /// <returns></returns>
        public DateTime GetDateTime(StoryList storyList)
        {
            string dateStr = storyList.Story.StoryInfo.Date.Replace(' ', '-');
            string timeStr = $"{storyList.Story.StoryInfo.Time.Hour}:{storyList.Story.StoryInfo.Time.Minute}:{storyList.Story.StoryInfo.Time.Second}";
            DateTime storyDate = Convert.ToDateTime(dateStr + " " + timeStr);

            return storyDate;
        }

        /// <summary>
        /// Step 2
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="storyList"></param>
        /// <returns></returns>
        public Fixation GetIndexedFixation(DateTime dateTime, StoryList storyList, int? speedLimit = null)
        {
            speedLimit = speedLimit.HasValue ? speedLimit : SpeedLimit;

            var xmlConv = storyList.Story.ImageList
                    .Where(x => Convert.ToInt32(x.ImageInfo.Speed) > 0)
                    .Select(x => new
                    {
                        x,
                        DateTime = Convert.ToDateTime(string.Format("{0} {1}",
                            x.ImageInfo.Date,
                            $"{x.ImageInfo.Time.Hour}:{x.ImageInfo.Time.Minute}:{x.ImageInfo.Time.Second}"))
                    }).ToList();

            var groupedIncidents = xmlConv.GroupBy(x => x.DateTime).ToList();

            var groupedAvg = groupedIncidents.Select(x => new
            {
                x.Key,
                Collection = x.ToList(),
                Avg = x.Average(f => Convert.ToInt32(f.x.ImageInfo.Speed))
            })
            .Where(x => x.Avg > speedLimit)
            .OrderBy(x => x.Avg)
            .ToList();

            int index = Convert.ToInt32(groupedAvg.Count * 0.7) - 1; // - 1 because index in array starts from 0

            var item = groupedAvg[index];

            // Get default image
            var noImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"images/no-image.jpg");
            byte[] imageArray = File.ReadAllBytes(noImagePath);
            string base64 = Convert.ToBase64String(imageArray);

            var result = new Fixation()
            {
                FixationDate = DateTime.SpecifyKind(dateTime, DateTimeKind.Local),
                Speed = (int)item.Avg,
                Image = base64
            };

            return result;
        }

        public void WriteSpeedListToCsv(StoryList storyList, string filePath = "")
        {
            var xmlObj = storyList.Story.ImageList
                .Where(x => Convert.ToInt32(x.ImageInfo.Speed) > 0)
                .Select(x => new
                {
                    StoryImage = x,
                    DateTime = Convert.ToDateTime(string.Format("{0} {1}",
                        x.ImageInfo.Date,
                        $"{x.ImageInfo.Time.Hour}:{x.ImageInfo.Time.Minute}:{x.ImageInfo.Time.Second}"))
                }).ToList();

            var csv = new StringBuilder();

            string newLine = "Initial result";
            csv.AppendLine(newLine);
            foreach (var item in xmlObj)
            {
                newLine = $"{xmlObj.IndexOf(item) + 1}. Date: {item.DateTime}, speed: {item.StoryImage.ImageInfo.Speed}";
                csv.AppendLine(newLine);
            }

            var groupedIncidents = xmlObj.GroupBy(x => x.DateTime).ToList();
            var groupedAvg = groupedIncidents.Select(x => new
            {
                DateTime = x.Key,
                StoryList = x.ToList(),
                AverageSpeed = x.Average(f => Convert.ToInt32(f.StoryImage.ImageInfo.Speed))
            })
            //.Where(x => x.AverageSpeed > SpeedLimit)
            .OrderBy(x => x.AverageSpeed)
            .ToList();

            newLine = "Average by each date result";
            csv.AppendLine(newLine);
            foreach (var item in groupedAvg)
            {
                newLine = $"{groupedAvg.IndexOf(item) + 1}. Date: {item.DateTime}, average speed: {item.AverageSpeed}";
                csv.AppendLine(newLine);
            }

            // 70% item
            int index = Convert.ToInt32(groupedAvg.Count * 0.7) - 1; // - 1 because index in array starts from 0
            var filteredItem = groupedAvg[index];

            newLine = "Filtered by 70% index item";
            csv.AppendLine(newLine);
            newLine = $"Index: {index + 1}, date: {filteredItem.DateTime}, average speed: {filteredItem.AverageSpeed}";
            csv.AppendLine(newLine);

            File.AppendAllText(filePath, csv.ToString());
        }
    }

}
