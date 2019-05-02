using System;
using System.Drawing;
using System.IO;
using BinarApp.Core.Models;
using BinarApp.Core.POCO;
using BinarApp.DesktopClient.Converters;
using BinarApp.DesktopClient.Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarApp.UnitTests
{
    [TestClass]
    public class FullLogicTest
    {
        [TestMethod]
        public void GetMergedFixation()
        {
            // Arrange
            var fileManager = new FileManager();
            var binarCameraManager = new BinarCameraManager(fileManager);
            string filePath = @"C:\ProgramData\Simicon\grabberd\15.03.2018_12.10.56_2924966151.xml";
            string xmlData = fileManager.ReadFile(filePath);

            var tattileCameraManager = new TattileCameraManager(fileManager);
            DateTime storyDate = new DateTime(2018, 6, 16, 13, 5, 55);

            // Act
            // Binar logic
            StoryList storyList = binarCameraManager.ConvertFromStringXml(xmlData);
            DateTime storyDate1 = binarCameraManager.GetDateTime(storyList);

            Fixation binarFixation = binarCameraManager.GetIndexedFixation(storyDate, storyList);

            // Tattile logic
            var tattileImageNames = tattileCameraManager.GetImages(storyDate);

            var tattileFilteredImages = tattileCameraManager
                .FilterImagesByDateTime(tattileImageNames, storyDate);

            string tattileTotalFilteredImage = tattileCameraManager
                .GetFilteredImageName(tattileFilteredImages, storyDate);

            Fixation tattileFixation = tattileCameraManager.GetFixation(tattileTotalFilteredImage);

            // Merge
            Fixation mergedFixation = new Fixation
            {
                Speed = binarFixation.Speed,
                FixationDate = binarFixation.FixationDate
            };

            if (tattileFixation != null)
            {
                mergedFixation.GRNZ = tattileFixation.GRNZ;
                mergedFixation.Image = tattileFixation.Image;
            }

            // Assert
            Assert.IsNotNull(mergedFixation);
            Assert.AreNotEqual(mergedFixation.Speed, 0);
            Assert.AreNotEqual(mergedFixation.GRNZ.Length, 0);
        }

        [TestMethod]
        public void ReduceImageSize()
        {
            // Arrange
            string fullPath = @"C:\camera\2018-06-16\13\2018-06-16_13-00-25-604_353SYA05-KAZ.jpg";
            string newFullPath = $@"C:\camera\2018-06-16\13\{Guid.NewGuid().ToString()}.jpg";

            // Act
            var image = Image.FromFile(fullPath);
            FixationImageConverter.SaveJpeg(newFullPath, image, 25);

            byte[] imageArray = File.ReadAllBytes(newFullPath);
            string base64 = Convert.ToBase64String(imageArray);

            if (File.Exists(newFullPath))
            {
                File.Delete(newFullPath);
            }

            // Assert
            Assert.IsNotNull(image);
        }
            
    }
}
