using System;
using System.Threading.Tasks;
using AadUserCreation.Models;
using Newtonsoft.Json;
using AadUserCreation.Business.Interfaces;
using Microsoft.Extensions.Options;
using AadUserCreation.Data.Interfaces;
using System.Linq;

namespace AadUserCreation.Business
{
    /// <summary>
    /// Management of users and groups in AAD, via Webhook calls to Azure runbooks or via the Graph API
    /// </summary>
    public class UserSerivice : IAadUserService
    {
        private readonly AppSettings _appSettings;
        private readonly IDepartmentContext _departmentContext;
        private readonly IGroupContext _groupContext;
        private readonly IWebhookService _webhookService;
        private readonly ILogTable _logTable;
        private readonly IAadCollector _aadCollector;

        public UserSerivice(IOptions<AppSettings> appSettings, IGroupContext groupContext ,IDepartmentContext departmentContext, IWebhookService webhookService, ILogTable logTable, IAadCollector aadCollector)
        {
            _appSettings = appSettings.Value;
            _departmentContext = departmentContext;
            _groupContext = groupContext;
            _webhookService = webhookService;
            _logTable = logTable;
            _aadCollector = aadCollector;
        }

        /// <summary>
        /// Add new AAD User with groups via Azure Runbook webhook call.
        /// 
        /// Fire and forget action, the Azure Runbook will handle the creation and notification.
        /// The Runbook runs in Azure, has the rights to create users, sets some additional properties and sends an email with the temp password.
        ///
        /// </summary>
        /// <param name="user"></param>
        /// <param name="requestByUser"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        public async Task<string> AddAadUser(Models.User user, string requestByUser, string actionType)
        {
            var webhookToCall = _appSettings.WebHookUrlCreateUsersRunbook;

            var userInJson = JsonConvert.SerializeObject(user);
            var jobResult = _webhookService.CallWebHook(userInJson, webhookToCall).Result;
            Job _job = null;

            if (jobResult.Length != 0)
                _job = await Task.Factory.StartNew(()=> JsonConvert.DeserializeObject<Job>(jobResult));

            string jobId = _job.JobIds[0];
            await _logTable.AddLogToTable(jobId, requestByUser, actionType, user.DisplayName);

            return jobId;
        }

        /// <summary>
        /// Add or removes the selected user from AAD groups.
        /// 
        /// Calls SetGroups and SetDepartments (actually the same methods, need to refactor) for updating the users membership via webhooks.
        /// 
        /// </summary>
        /// <param name="groupuser"></param>
        /// <param name="requestByUser"></param>
        /// <returns></returns>
        public async Task<string> ProcessUserGroupsEdits(GroupUser groupuser, string requestByUser)
        {
           var current = await _aadCollector.LoadGroupMembershipsForGroupsFromBlob();
           await SetGroups(groupuser, requestByUser, current);
           await SetDepartments(groupuser, requestByUser, current);
           return null;
        }

        /// <summary>
        /// User group membership update method. 
        /// 
        /// Fire and forget action, the Azure Runbook will handle the update process.
        /// Based on groupmembership of the user in the stored Blob, it looks for differences in group membership and triggers the webhook.
        /// The methode sends the update calls to an Azure runbook webhook which handles the action.
        /// 
        /// </summary>
        /// <param name="groupuser"></param>
        /// <param name="requestByUser"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        private async Task SetDepartments(GroupUser groupuser, string requestByUser, GroupListDetails current)
        {
            foreach (var department in groupuser.Departments)
            {
                string depName = department.Name;
                DepartmentList dep = current.DepartmentList.FirstOrDefault(n => n.Name == depName);

                if (dep.Members.Contains(groupuser.UPN) & department.Selected == true || !dep.Members.Contains(groupuser.UPN) & department.Selected == false) //staat goed
                {
                    //all good
                }
                else
                {
                    string jobId = "";
                    try
                    {
                        var webhookToCall = _appSettings.WebHookUrlUserMembershipGroupRunbook;

                        var jsonObjectForWebhook = new DepartmentWebhook();
                        jsonObjectForWebhook.UPN = groupuser.UPN;
                        switch (department.Selected)
                        {
                            case false:
                                {
                                    jsonObjectForWebhook.RemoveGroupDep = department.Name;

                                    break;
                                }
                            case true:
                                {
                                    jsonObjectForWebhook.AddGroupDep = department.Name;
                                    break;
                                }
                            default:
                                break;
                        }

                        var userGroupMemebershipInJson = JsonConvert.SerializeObject(jsonObjectForWebhook);
                        var jobResult = _webhookService.CallWebHook(userGroupMemebershipInJson, webhookToCall).Result;
                        Job _job = null;

                        if (jobResult.Length != 0)
                            _job = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<Job>(jobResult));

                        jobId = _job.JobIds[0];

                        try
                        {
                            await _logTable.AddLogToTable(jobId, requestByUser, "Edit", groupuser.UPN);
                        }
                        catch (Exception)
                        {
                            // don't break
                        }

                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                
            }
        }

        /// <summary>
        /// User group membership update method. 
        /// 
        /// Fire and forget action, the Azure Runbook will handle the update process
        /// Based on groupmembership of the user in the stored Blob, it looks for differences in group membership and triggers the webhook.
        /// The methode sends the update calls to an Azure runbook webhook which handles the action.
        /// 
        /// </summary>
        /// <param name="groupuser"></param>
        /// <param name="requestByUser"></param>
        /// <param name="current"></param>
        /// <returns></returns>
        private async Task SetGroups(GroupUser groupuser, string requestByUser, GroupListDetails current)
        {
            foreach (var group in groupuser.Groups)
            {
                string grpName = group.Name;
                GroupList grp = current.GroupList.FirstOrDefault(n => n.Name == grpName);

                if (grp.Members.Contains(groupuser.UPN) & group.Selected == true || !grp.Members.Contains(groupuser.UPN) & group.Selected == false) //staat goed
                {
                    //all good
                }
                else
                {
                    string jobId = "";
                    try
                    {
                        var webhookToCall = _appSettings.WebHookUrlUserMembershipGroupRunbook;

                        var jsonObjectForWebhook = new GroupWebhook();
                        jsonObjectForWebhook.UPN = groupuser.UPN;
                        switch (group.Selected)
                        {
                            case false:
                                {
                                    jsonObjectForWebhook.RemoveGroupApp = group.Name;
                                    break;
                                }
                            case true:
                                {
                                    jsonObjectForWebhook.AddGroupApp = group.Name;
                                    break;
                                }
                            default:
                                break;
                        }

                        var userGroupMemebershipInJson = JsonConvert.SerializeObject(jsonObjectForWebhook);
                        var jobResult = _webhookService.CallWebHook(userGroupMemebershipInJson, webhookToCall).Result;
                        Job _job = null;

                        if (jobResult.Length != 0)
                            _job = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<Job>(jobResult));

                        jobId = _job.JobIds[0];
                        try
                        {
                            await _logTable.AddLogToTable(jobId, requestByUser, "Edit", groupuser.UPN);
                        }
                        catch (Exception)
                        {
                            //never fail on log 
                        }


                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Remove AAD user
        /// 
        /// Fire and forget action, the Azure Runbook will handle the deletion and notification.
        /// The Runbook runs in Azure, has the rights to delete users and makes some additional cleanup actions.
        ///
        /// </summary>
        /// <param name="userUPNToDelete"></param>
        /// <param name="requestByUser"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        public async Task<string> RemoveAadUser(string userUPNToDelete, string requestByUser, string actionType)
        {
            RemovedUser rm = new RemovedUser();
            rm.UPN = userUPNToDelete;

            var webhookToCall = _appSettings.WebHookUrlDeleteUsersRunbook;

            var userInJson = JsonConvert.SerializeObject(rm);
            var jobResult = _webhookService.CallWebHook(userInJson, webhookToCall).Result;
            Job _job = null;

            if (jobResult.Length != 0)
                _job = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<Job>(jobResult));

            string jobId = _job.JobIds[0];

            await _logTable.AddLogToTable(jobId, requestByUser, actionType, userUPNToDelete);

            return jobId;
        }

        public Task GetGroupMembershipsForGroups()
        {
            throw new NotImplementedException();
        }
    }

    public  class RemovedUser
    {
        public string UPN { get; set; }
    }
}