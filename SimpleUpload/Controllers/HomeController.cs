using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SimpleUpload.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SimpleUpload.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration _configuration;

        public HomeController(IConfiguration Configuration)
        {
            _configuration = Configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("UploadFiles")]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            var uploadSuccess = false;

            foreach (var formFile in files)
            {
                if (formFile.Length <=0)
                {
                    continue;
                }
                using (var ms = new MemoryStream())
                {
                    formFile.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    uploadSuccess = await UploadToBlob(formFile.FileName, fileBytes);
                }
            }

            if (uploadSuccess)
                return View("UploadSuccess");
            else
                return View("UploadError");
        }

        private async Task<bool> UploadToBlob(string filename, byte[] imageBuffer)
        {
            CloudStorageAccount storageAccount = null;
            CloudBlobContainer cloudBlobContainer = null;
            string storageConnectionString = _configuration["storageconnectionstring"];

            // Check whether the connection string can be parsed.
            if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
            {
                try
                {
                    // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
                    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                    // Create a container called 'quickstartblobs' and append a GUID value to it to make the name unique. 
                    cloudBlobContainer = cloudBlobClient.GetContainerReference("uploadblob" + Guid.NewGuid().ToString());
                    await cloudBlobContainer.CreateAsync();

                    // Set the permissions so the blobs are public. 
                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };
                    await cloudBlobContainer.SetPermissionsAsync(permissions);
                    
                    // Get a reference to the blob address, then upload the file to the blob.
                    // Use the value of localFileName for the blob name.
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(filename);
                    await cloudBlockBlob.UploadFromByteArrayAsync(imageBuffer, 0, imageBuffer.Length);

                    return true;
                }
                catch (StorageException ex)
                {
                    return false;
                }
                finally
                {
                    // OPTIONAL: Clean up resources, e.g. blob container
                    //if (cloudBlobContainer != null)
                    //{
                    //    await cloudBlobContainer.DeleteIfExistsAsync();
                    //}
                }
            }
            else
            {
                return false;
            }

        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
