using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AadUserCreation.Models
{

    public class LogData : TableEntity
    {


        public LogData(string CreatorName, string JobId)
        {
            PartitionKey = CreatorName;
            RowKey = JobId;
        }
        public LogData()
        {

        }
        public string LogText;
        public string ActionType { get; set; }
        public string DisplayName { get; set; }
    }
}

