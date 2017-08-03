using AadUserCreation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AadUserCreation.Data.Interfaces
{
    public interface ILogTable
    {
        Task<IEnumerable<LogData>> GetAllLogs();
        Task<LogData> AddLogToTable(string logTekst, string requestByUser, string actionType, string displayname);
    }
}
