﻿using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.AzureBlobStorageUploader
{
    class Program
    {
        static void Main(string[] args)
        {
            //RunAsync().Wait();
            bool isSuccess = false;

            if (args.Length > 0)
            {
                string cleanupFolderPath = args.Length > 1 ? args[1] : string.Empty;

                var uploadTask = Upload(args[0], cleanupFolderPath);

                uploadTask.Wait();

                isSuccess = uploadTask.Result;

                Console.WriteLine();
            }

            if (isSuccess) Console.WriteLine("UPLOAD SUCCESSFUL....");
            else Console.WriteLine("UPLOAD FAILED....");

            Console.ReadKey();
        }

        private static async Task<bool> Upload(string rootFolderPath, string relativeCleanupFolderPath)
        {
            string connectionString = ConfigurationManager.AppSettings["StorageConnectionString"];
            string containerName = ConfigurationManager.AppSettings["ContainerName"];
            string ignoreSubstring = ConfigurationManager.AppSettings["IgnoreSubstring"];

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

            List<string> filePaths = Directory.EnumerateFiles(rootFolderPath, "*.*", SearchOption.AllDirectories)
                .Where(fp => !fp.Contains(ignoreSubstring))
                .ToList();

            bool uploaded = await UploadFiles(cloudBlobContainer, filePaths, rootFolderPath);

            await CleanupFiles(rootFolderPath, relativeCleanupFolderPath, ignoreSubstring);

            return uploaded;
        }

        private static async Task<bool> UploadFiles(CloudBlobContainer blobContainer, List<string> filePaths, string folderPath)
        {
            bool isSuccess = false;

            try
            {
                Console.WriteLine("==== UPLOADING FILES TO BLOB STORAGE ====");

                var replacementFolderPath = folderPath;

                if (!replacementFolderPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
                {
                    replacementFolderPath += Path.DirectorySeparatorChar.ToString();
                }

                foreach (string filePath in filePaths)
                {
                    string filePathReference = filePath.Replace(replacementFolderPath, string.Empty);

                    CloudBlockBlob blob = blobContainer.GetBlockBlobReference(filePathReference);

                    Console.WriteLine(filePathReference);

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

        private static async Task CleanupFiles(string rootFolderPath, string relativeCleanupFolderPath, string ignoreSubstring)
        {
            if (!string.IsNullOrWhiteSpace(relativeCleanupFolderPath))
            {
                string cleanupFolderPath = Path.Combine(rootFolderPath, relativeCleanupFolderPath);

                List<string> directories = Directory.EnumerateDirectories(cleanupFolderPath, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(dir => !dir.Contains(ignoreSubstring))
                    .ToList();

                foreach (string directoryPath in directories)
                {
                    var di = new DirectoryInfo(directoryPath);

                    string newDirectoryPath = Path.Combine(cleanupFolderPath, ignoreSubstring + di.Name);

                    Directory.Move(directoryPath, newDirectoryPath);
                }
            }
        }
    }
}
