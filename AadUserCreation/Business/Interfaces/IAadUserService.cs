using AadUserCreation.Models;
using System.Threading.Tasks;

namespace AadUserCreation.Business.Interfaces
{
    public interface IAadUserService
    {

        Task<string> AddAadUser(User user, string requestByUser, string ActionType);
      
        Task<string> ProcessUserGroupsEdits(GroupUser groupuser, string requestByUser);

        Task<string> RemoveAadUser(string userUPNToDelete, string requestByUser, string actionType);
        Task GetGroupMembershipsForGroups();
    }
}
