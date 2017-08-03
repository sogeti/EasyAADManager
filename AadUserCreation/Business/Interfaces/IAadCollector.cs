using AadUserCreation.Models;
using System.Threading.Tasks;

namespace AadUserCreation.Business.Interfaces
{
    public interface IAadCollector
    {
        Task<GroupListDetails> GetGroupMembershipsForGroups();

        Task<GroupListDetails> LoadGroupMembershipsForGroupsFromBlob();

        Task<GroupListDetails> LoadGroupMembershipsForGroupsFromAad();

        Task<string> CollectionBlobState();

    }
}
