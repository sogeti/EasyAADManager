using AadUserCreation.Data.Interfaces;
using AadUserCreation.Helpers;
using AadUserCreation.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AadUserCreation.Data
{
    public class AadCollectieFromAad: IAadCollectieFromAad
    {
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;

        public AadCollectieFromAad(IOptions<AppSettings> appSettings, ILogger<AadCollectieFromAad> logger)
        {
            _appSettings = appSettings.Value;
            _logger = logger;
        }

        public async Task<GroupListDetails> GetGroupListDetails()
        {
            return await GetGroupMembershipsForGroups();
        }

        private async Task<GroupListDetails> GetGroupMembershipsForGroups()
        {
            var groupslistdetails = new GroupListDetails();
            List<GroupList> groupList = new List<GroupList>();
            List<DepartmentList> departmentList = new List<DepartmentList>();
            try
            {
                string clientId = _appSettings.ClientId; 
                string clientSecret = _appSettings.clientSecret; 
                string directory = _appSettings.directory;
                ClientCredential creds = new ClientCredential(clientId, clientSecret);

                GraphServiceClient graphClient = new GraphServiceClient(new AzureAuthenticationProvider(creds, directory));

                var result = await graphClient.Groups.Request().GetAsync();
                var foundGroups = result.CurrentPage.ToList();
                if (foundGroups != null && foundGroups.Count > 0)
                {
                    do
                    {
                        foundGroups = result.CurrentPage.ToList();
                        foreach (var item in foundGroups)
                        {
                            _logger.LogInformation(8, item.DisplayName, "Group");

                            if (item.DisplayName.ToString().ToLower().StartsWith("dep") || item.DisplayName.ToString().ToLower().StartsWith("az") || item.DisplayName.ToString().ToLower().StartsWith("app "))
                            {
                                string groupname = item.DisplayName.ToString().ToLower();
                                if (groupname.StartsWith("dep"))
                                {
                                    var groupdetails = new DepartmentList()
                                    {
                                        Id = item.Id,
                                        Name = item.DisplayName
                                    };
                                    var t = await GetGroupFrom(graphClient, item.Id);
                                    groupdetails.Members = t;
                                    departmentList.Add(groupdetails);
                                }
                                if (groupname.StartsWith("az") || groupname.StartsWith("app "))
                                {
                                    var groupdetails = new GroupList()
                                    {
                                        Id = item.Id,
                                        Name = item.DisplayName
                                    };
                                    var t = await GetGroupFrom(graphClient, item.Id);
                                    groupdetails.Members = t;
                                    groupList.Add(groupdetails);
                                }
                            }
                        }
                        if (result.NextPageRequest !=null)
                        {
                            result = await result.NextPageRequest.GetAsync();
                        }
                        else
                        {
                            result = null;
                        }
                    } while (result != null && result.Count > 0);
                }
                groupslistdetails.GroupList = groupList;
                groupslistdetails.DepartmentList = departmentList;
                return groupslistdetails;
            }
            catch (Exception e)
            {
                _logger.LogError(8, e, "GetGroupMembershipsForGroups");
                throw e;
            }
        }


        private async Task<List<string>> GetGroupFrom(GraphServiceClient graphClient, string id)
        {
            List<string> list = new List<string>();
            try
            {
                Microsoft.Graph.Group group = await graphClient.Groups[id].Request().GetAsync();

                if (group != null)
                {
                    IGroupMembersCollectionWithReferencesPage members = await graphClient.Groups[id].Members.Request().GetAsync();
                    var foundMembers = members.CurrentPage.ToList();
                    if (foundMembers != null && foundMembers.Count > 0)
                    {
                        do
                        {
                            foundMembers = members.CurrentPage.ToList();
                            foreach (var member in foundMembers)
                            {
                                try
                                {
                                    var graphMember = await graphClient.Users[member.Id].Request().GetAsync();
                                    list.Add(graphMember.UserPrincipalName.ToLower());

                                    _logger.LogInformation(8, id + "--> " + graphMember.UserPrincipalName.ToLower());
                                }
                                catch (Exception e)
                                {
                                    _logger.LogWarning(8, e, "member: " + member.Id + "|| group: " + group.DisplayName);
                                    // don't crash    
                                }

                            }


                            if (members.NextPageRequest != null)
                            {
                                members = await members.NextPageRequest.GetAsync();
                            }
                            else
                            {
                                members = null;
                            }
                        } while (members != null && members.Count > 0);

                    }
                     
                }
                return list;
            }
            catch (Exception e)
            {
                _logger.LogError(8, e, "GetGroupFrom " + id.ToString());
                return list;
            }
        }
    }
}
//https://stackoverflow.com/questions/44115248/ms-graph-api-c-sharp-add-user-to-group
