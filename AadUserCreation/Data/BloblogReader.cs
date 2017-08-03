using AadUserCreation.Data.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AadUserCreation.Models;

namespace AadUserCreation.Data
{
    public class BloblogReader : IBloblogReader
    {
        private readonly AppSettings _appSettings;
        private readonly ILogTable _logTable;

        public BloblogReader(IOptions<AppSettings> appSettings, ILogTable logTable)
        {
            _appSettings = appSettings.Value;
            _logTable = logTable;
        }


        public async Task<string> GetLog(string jobId)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_appSettings.AzureStorageConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(_appSettings.BlobContainerName);
            await container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, null, null);

            try
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(jobId);
                var logTekst = await blockBlob.DownloadTextAsync();
                return logTekst;
            }
            catch (Exception)
            {
                //var _logData = await AddLogToTable("Log not found ", jobId);
                return null;
            }

            
            
        }
    }
}
