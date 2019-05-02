using System;
using System.IO;
using System.Linq;
using BinarApp.Core.Models;
using BinarApp.Core.POCO;
using BinarApp.DesktopClient.Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarApp.UnitTests
{
    [TestClass]
    public class BinarTest
    {
        [TestMethod]
        public void GetBinarFixation()
        {
            // Arrange  
            var fileManager = new FileManager();
            var binarCameraManager = new BinarCameraManager(fileManager);
            string folderPath = @"C:\ProgramData\Simicon\grabberd";
            string cacheFolderPath = @"C:\ProgramData\Simicon\grabberd\cache";
            string filePath = Path.Combine(folderPath, @"18.06.2018_11.59.16_1616603635.xml");
            string cacheFilePath = "";

            // Act
            string xmlData = fileManager.ReadFile(filePath);
            StoryList storyList = binarCameraManager.ConvertFromStringXml(xmlData);
            DateTime storyDate = binarCameraManager.GetDateTime(storyList);
            Fixation binarFixation = binarCameraManager.GetIndexedFixation(storyDate, storyList);

            // Parse xml files
            var res = Directory.GetFiles(folderPath, "*.xml")
               .Select(Path.GetFileName)
               .ToList();

            foreach (var item in res)
            {
                cacheFilePath = Path.Combine(cacheFolderPath, item.Replace(".xml", ".csv"));
                binarCameraManager.WriteSpeedListToCsv(storyList, cacheFilePath);
            }

            // Assert  
            Assert.IsNotNull(binarFixation);
            Assert.AreNotEqual(binarFixation.Speed, 0);
        }
    }
}
