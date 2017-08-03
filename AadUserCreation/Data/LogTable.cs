using AadUserCreation.Data.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AadUserCreation.Models;

namespace AadUserCreation.Data
{
    public class LogTable : ILogTable
    {
        private CloudTable _table { get; set; }
        private readonly AppSettings _appSettings;

        public LogTable(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            _table = GetTable().Result;
        }

        private async Task<CloudTable> GetTable()
        {
            var _azureStorageConnectionString = _appSettings.AzureStorageConnectionString;
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_azureStorageConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("LogTable");

            await table.CreateIfNotExistsAsync();

            return table;
        }

        public async Task<LogData> AddLogToTable(string jobId, string requestByUser, string actionType, string displayname)
        {
            try
            {
                LogData logToInsert = new LogData()
                {
                    PartitionKey = requestByUser,
                    RowKey = jobId
                };
                logToInsert.ActionType = actionType;
                logToInsert.DisplayName = displayname;

                TableOperation insertOperation = TableOperation.Insert(logToInsert);
                TableResult result = await _table.ExecuteAsync(insertOperation);
                if (result != null)
                    return logToInsert;
                else
                    return null;
            }
            catch (Exception e)
            {

                throw e;
            }
          
        }

        public async Task<IEnumerable<LogData>> GetAllLogs()
        {
            TableQuery<LogData> query = new TableQuery<LogData>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.NotEqual, "###"));
            TableQuerySegment<LogData> resultSegment = await _table.ExecuteQuerySegmentedAsync(query, null);

            return resultSegment.Results.OrderByDescending(d=> d.Timestamp);
        }


    }
}
