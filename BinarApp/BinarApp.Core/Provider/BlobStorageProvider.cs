using BinarApp.Core.Interfaces;
using BinarApp.Core.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinarApp.Core.Provider
{
    public class BlobStorageProvider : IBlobStorageProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="base64"></param>
        /// <param name="iin"></param>
        /// <returns></returns>
        public BlobFileVM Upload(string base64, string iin)
        {
            var imgByteArr = Convert.FromBase64String(base64);
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["blobStrgConnection"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(ConfigurationManager.AppSettings["blobStrgContainer"]);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(iin + "_img");
            blockBlob.UploadFromByteArray(imgByteArr, 0, imgByteArr.Length);
            return new BlobFileVM()
            {
                BlobName = iin + "_img",
                Url = blockBlob.Uri.AbsoluteUri
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public BlobFileVM GetByBlobName(string blobName)
        {
            byte[] byteArr = null;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(string.Empty);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var filesContainer = blobClient.GetContainerReference("imgcontainer");       
            CloudBlockBlob blockBlob = filesContainer.GetBlockBlobReference(blobName);
            blockBlob.DownloadToByteArray(byteArr, 0);
            return  new BlobFileVM()
            {
                BlobName = blobName,
                Url = blockBlob.Uri.AbsoluteUri,
                ImgArr = byteArr
            };
        }
    }
}
