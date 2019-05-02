using System;
using BinarApp.Core.POCO;
using BinarApp.DesktopClient.Managers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarApp.UnitTests
{
    [TestClass]
    public class TattileTest
    {
        [TestMethod]
        public void GetTattileFixation()
        {
            // Arrange  
            var fileManager = new FileManager();
            var tattileCameraManager = new TattileCameraManager(fileManager);
            DateTime storyDate = new DateTime(2018, 6, 16, 13, 5, 55);

            // Act
            var tattileImageNames = tattileCameraManager.GetImages(storyDate);
        
            var tattileFilteredImages = tattileCameraManager
                .FilterImagesByDateTime(tattileImageNames, storyDate);
   
            string tattileTotalFilteredImage = tattileCameraManager
                .GetFilteredImageName(tattileFilteredImages, storyDate);
        
            Fixation tattileFixation = tattileCameraManager.GetFixation(tattileTotalFilteredImage);

            // Assert  
            Assert.IsNotNull(tattileFixation);
            Assert.AreNotEqual(tattileFixation.GRNZ.Length, 0);
        }
    }
}
