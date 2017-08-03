using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AadUserCreation.Models;

namespace AadUserCreation.Data.Interfaces
{
    public interface IBloblogReader
    {
        Task<string> GetLog(string jobId);
     
    }
}
