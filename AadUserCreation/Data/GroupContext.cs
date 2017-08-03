using AadUserCreation.Data.Interfaces;
using AadUserCreation.Helpers;
using AadUserCreation.Models;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AadUserCreation.Data
{
    public class GroupContext : IGroupContext
    {
       
        private readonly AppSettings _appSettings;
        public GroupContext(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
     
        }

        public async Task<bool> AddGroupToTable(GroupList group)
        {
            try
            {
                string clientId = _appSettings.ClientId; 
                string clientSecret = _appSettings.clientSecret; 
                string directory = _appSettings.directory;
                ClientCredential creds = new ClientCredential(clientId, clientSecret);

                GraphServiceClient graphClient = new GraphServiceClient(new AzureAuthenticationProvider(creds, directory));

                var grpToAdd = new Microsoft.Graph.Group();
                grpToAdd.DisplayName = group.Name;
                grpToAdd.MailEnabled = false;
                string withoutSpaces = Regex.Replace(group.Name, @"\s+", "");
                grpToAdd.MailNickname = withoutSpaces;
                grpToAdd.SecurityEnabled = true;
                grpToAdd.MailEnabled = false;

                var addedgroup = await graphClient.Groups.Request().AddAsync(grpToAdd);


                if (addedgroup != null)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {

                throw;
            }

        }

        public async Task<bool> RemoveGroupFromTable(string groupid)
        {
            string clientId = _appSettings.ClientId; 
            string clientSecret = _appSettings.clientSecret;
            string directory = _appSettings.directory;
            ClientCredential creds = new ClientCredential(clientId, clientSecret);

            GraphServiceClient graphClient = new GraphServiceClient(new AzureAuthenticationProvider(creds, directory));
            try
            {
                await graphClient.Groups[groupid].Request().DeleteAsync();
                return true;
            }
            catch (Exception c)
            {
                throw;
            }
            



        }
    }
}
