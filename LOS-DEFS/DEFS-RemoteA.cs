// 2017, 1, 19
// LOS-DEFS:DEFS-RemoteA.cs
// Copyright (c) 2017 PopulationX
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Blob;


namespace LOS_DEFS
{
   public class DefsRemoteAzure
    {
        private static StorageCredentials _credentials = new StorageCredentials();
        // Parse the connection string and return a reference to the storage account.
        private static readonly CloudStorageAccount StorageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
        private static readonly CloudBlobClient BlobClient = StorageAccount.CreateCloudBlobClient();
        private readonly CloudBlobContainer _installerBlobContainer = BlobClient.GetContainerReference("installers"); // Public Read Only Container;

        private string LatestInstallerName = string.Empty;

        public DefsRemoteAzure()
        {
           
        }

        public bool CheckForNewInstaller()
        {
            foreach (IListBlobItem item in _installerBlobContainer.ListBlobs(null, false))
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;

                    if (blob.Name.ToLowerInvariant().EndsWith(".exe"))
                    {
                        // Found an Installer
                        // Check against Latest Installer in C:\LOS\Installers\
                        if (int.Parse(blob.Name.Replace(".exe", "")) > GetLatestLocalInstallerAsInt())
                        {
                            LatestInstallerName = blob.Name;
                            return true;
                        }
                        else
                            return false;
                    }

                }
                else if (item.GetType() == typeof(CloudPageBlob))
                {
                    CloudPageBlob pageBlob = (CloudPageBlob)item;

                    Console.WriteLine("Page blob of length {0}: {1}", pageBlob.Properties.Length, pageBlob.Uri);

                }
                else if (item.GetType() == typeof(CloudBlobDirectory))
                {
                    CloudBlobDirectory directory = (CloudBlobDirectory)item;

                    Console.WriteLine("Directory: {0}", directory.Uri);
                }
            }
            return false;
        }

        private int GetLatestLocalInstallerAsInt()
        {
            if (!Directory.Exists("C:\\Los\\Installers")) Directory.CreateDirectory("C:\\Los\\Installers");
            string[] files = Directory.GetFiles("C:\\Los\\Installers");
            if (files.Length == 0)
            {
                // No installers found return -1
                return -1;
            }

            return int.Parse(files[files.Length].Replace(".exe", ""));
        }

        public string GetLatestInstaller()
        {
            string latestInstallerPath = Path.Combine("C:\\Los\\Installers\\", LatestInstallerName);
            // Retrieve reference to a blob named "photo1.jpg".
            CloudBlockBlob blockBlob = _installerBlobContainer.GetBlockBlobReference(LatestInstallerName);

            // Save blob contents to a file.
            using (var fileStream = System.IO.File.OpenWrite(latestInstallerPath))
            {
                blockBlob.DownloadToStream(fileStream);
            }

            return LatestInstallerName;
        }
    }
}
