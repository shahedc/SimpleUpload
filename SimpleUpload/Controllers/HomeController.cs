using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SimpleUpload.Models;

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
            long size = files.Sum(f => f.Length);

            // full path to file in temp location
            var filePath = Path.GetTempFileName();

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
                    string s = Convert.ToBase64String(fileBytes);
                    // act on the Base64 data
                    await UploadToBlob(formFile.FileName, fileBytes);
                }
                //using (var stream = new FileStream(filePath, FileMode.Create))
                //{
                //    await formFile.CopyToAsync(stream);
                //    byte[] fileBytes = new byte[stream.Length];
                //    stream.Read(fileBytes, 0, fileBytes.Length);

                //    await UploadToBlob(formFile.FileName, stream, fileBytes, formFile.Length);
                //}
            }

            //

            //return Ok(new { count = files.Count, size, filePath });
            return View("Success");
        }

        //private async Task UploadToBlob(string filename, FileStream stream, byte[] imageBuffer, long imageLength)
        private async Task UploadToBlob(string filename, byte[] imageBuffer)
        {
            // process uploaded files
            // Don't rely or trust the Filename property without validation

            //

            CloudStorageAccount storageAccount = null;
            CloudBlobContainer cloudBlobContainer = null;
            string sourceFile = null;
            string destinationFile = null;

            // Retrieve the connection string for use with the application. The storage connection string is stored
            // in an environment variable on the machine running the application called storageconnectionstring.
            // If the environment variable is created after the application is launched in a console or with Visual
            // Studio, the shell needs to be closed and reloaded to take the environment variable into account.
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
                    Console.WriteLine("Created container '{0}'", cloudBlobContainer.Name);
                    Console.WriteLine();

                    // Set the permissions so the blobs are public. 
                    BlobContainerPermissions permissions = new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    };
                    await cloudBlobContainer.SetPermissionsAsync(permissions);

                    //// Create a file in your local MyDocuments folder to upload to a blob.
                    //string localPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    //string localFileName = "QuickStart_" + Guid.NewGuid().ToString() + ".txt";
                    //sourceFile = Path.Combine(localPath, localFileName);
                    //// Write text to the file.
                    //System.IO.File.WriteAllText(sourceFile, "Hello, World!");

                    //Console.WriteLine("Temp file = {0}", sourceFile);
                    //Console.WriteLine("Uploading to Blob storage as blob '{0}'", localFileName);
                    //Console.WriteLine();

                    // Get a reference to the blob address, then upload the file to the blob.
                    // Use the value of localFileName for the blob name.
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(filename);
                    //await cloudBlockBlob.UploadFromFileAsync(sourceFile);
                    //await cloudBlockBlob.UploadFromStreamAsync(stream);
                    await cloudBlockBlob.UploadFromByteArrayAsync(imageBuffer, 0, imageBuffer.Length);

                    // List the blobs in the container.
                    //Console.WriteLine("Listing blobs in container.");
                    //BlobContinuationToken blobContinuationToken = null;
                    //do
                    //{
                    //    var results = await cloudBlobContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);
                    //    // Get the value of the continuation token returned by the listing call.
                    //    blobContinuationToken = results.ContinuationToken;
                    //    foreach (IListBlobItem item in results.Results)
                    //    {
                    //        Console.WriteLine(item.Uri);
                    //    }
                    //} while (blobContinuationToken != null); // Loop while the continuation token is not null.
                    //Console.WriteLine();

                    // Download the blob to a local file, using the reference created earlier. 
                    // Append the string "_DOWNLOADED" before the .txt extension so that you can see both files in MyDocuments.
                    //destinationFile = sourceFile.Replace(".txt", "_DOWNLOADED.txt");
                    //Console.WriteLine("Downloading blob to {0}", destinationFile);
                    //Console.WriteLine();
                    //await cloudBlockBlob.DownloadToFileAsync(destinationFile, FileMode.Create);
                }
                catch (StorageException ex)
                {
                    Console.WriteLine("Error returned from the service: {0}", ex.Message);
                }
                finally
                {
                    //Console.WriteLine("Press any key to delete the sample files and example container.");
                    //Console.ReadLine();
                    //// Clean up resources. This includes the container and the two temp files.
                    //Console.WriteLine("Deleting the container and any blobs it contains");
                    //if (cloudBlobContainer != null)
                    //{
                    //    await cloudBlobContainer.DeleteIfExistsAsync();
                    //}
                    //Console.WriteLine("Deleting the local source file and local downloaded files");
                    //Console.WriteLine();
                    //System.IO.File.Delete(sourceFile);
                    //System.IO.File.Delete(destinationFile);
                }
            }
            else
            {
                Console.WriteLine(
                    "A connection string has not been defined in the system environment variables. " +
                    "Add a environment variable named 'storageconnectionstring' with your storage " +
                    "connection string as a value.");
            }

        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
