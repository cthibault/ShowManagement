using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBlobStorageUploader
{
    class Program
    {
        static void Main(string[] args)
        {
            //RunAsync().Wait();
            bool isSuccess = false;

            if (args.Length > 0)
            {
                var uploadTask = Upload(args[0]);

                uploadTask.Wait();

                isSuccess = uploadTask.Result;
            }

            if (isSuccess) Console.WriteLine("Upload Successful....");
            else Console.WriteLine("Upload Failed....");

            Console.ReadKey();
        }

        private static async Task<bool> Upload(string rootFolderPath)
        {
            string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            string containerName = ConfigurationManager.AppSettings["ContainerName"];

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);

            bool created = await cloudBlobContainer.CreateIfNotExistsAsync();
            if (created)
            {
                var permissions = new BlobContainerPermissions();
                permissions.PublicAccess = BlobContainerPublicAccessType.Blob;
                cloudBlobContainer.SetPermissions(permissions);
            }

            List<string> filePaths = Directory.EnumerateFiles(rootFolderPath, "*.*", SearchOption.AllDirectories).ToList();

            bool uploaded = await UploadFiles(cloudBlobContainer, filePaths, rootFolderPath);

            return uploaded;
        }

        private static async Task<bool> UploadFiles(CloudBlobContainer blobContainer, List<string> filePaths, string folderPath)
        {
            bool isSuccess = false;

            try
            {
                var replacementFolderPath = folderPath;

                if (!replacementFolderPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    replacementFolderPath += Path.DirectorySeparatorChar.ToString();
                }

                foreach (string filePath in filePaths)
                {
                    string filePathReference = filePath.Replace(replacementFolderPath, string.Empty);

                    CloudBlockBlob blob = blobContainer.GetBlockBlobReference(filePathReference);

                    await blob.UploadFromFileAsync(filePath, FileMode.Open);
                }

                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
            }

            return isSuccess;
        }
    }
}
