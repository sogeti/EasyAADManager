using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AadUserCreation.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace AadUserCreation.Data.Interfaces
{
    public class AadCollectieFromBlob : IAadCollectieFromBlob
    {
        private readonly AppSettings _appSettings;
        public AadCollectieFromBlob(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public async Task<DateTimeOffset?> GetCollectionBlobState()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_appSettings.AzureStorageConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(_appSettings.BlobContainerName);
            await container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, null, null);

            try
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference("AadCollection");
                await blockBlob.FetchAttributesAsync();
                return blockBlob.Properties.LastModified;
            }
            catch
            {
                return null;
            }
        }

        public async Task<GroupListDetails> GetGroupListDetails()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_appSettings.AzureStorageConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(_appSettings.BlobContainerName);
            await container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, null, null);

            try
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference("AadCollection");
                var aadCollection = await blockBlob.DownloadTextAsync();

                GroupListDetails storedGroupListDetails = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<GroupListDetails>(aadCollection));

                return storedGroupListDetails;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task PutGroupListDetails(GroupListDetails groupDetailsToStore)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_appSettings.AzureStorageConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(_appSettings.BlobContainerName);
            await container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, null, null);

            try
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference("AadCollection");
                var storedGroupListDetails = JsonConvert.SerializeObject(groupDetailsToStore);
                await blockBlob.UploadTextAsync(storedGroupListDetails);


            }
            catch
            {


            }
        }
    }
}

