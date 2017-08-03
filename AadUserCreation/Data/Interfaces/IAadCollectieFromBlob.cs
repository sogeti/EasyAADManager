using AadUserCreation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AadUserCreation.Data.Interfaces
{
    public interface IAadCollectieFromBlob
    {
        Task<DateTimeOffset?> GetCollectionBlobState();

        Task<GroupListDetails> GetGroupListDetails();
        Task PutGroupListDetails(GroupListDetails groupDetailsToStore);
    }
}
