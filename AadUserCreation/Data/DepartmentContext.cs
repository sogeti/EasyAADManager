using AadUserCreation.Data.Interfaces;
using AadUserCreation.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Auth;

using Microsoft.WindowsAzure.Storage;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Graph;
using AadUserCreation.Helpers;
using System.Text.RegularExpressions;

namespace AadUserCreation.Data
{
    public class DepartmentContext : IDepartmentContext
    {
  
        private readonly AppSettings _appSettings;
        public DepartmentContext(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;

        }

        public async Task<bool> AddDepartmentToTable(DepartmentList department)
        {
            string clientId = _appSettings.ClientId; 
            string clientSecret = _appSettings.clientSecret; 
            string directory = _appSettings.directory;
            ClientCredential creds = new ClientCredential(clientId, clientSecret);

            GraphServiceClient graphClient = new GraphServiceClient(new AzureAuthenticationProvider(creds, directory));

            var grpToAdd = new Microsoft.Graph.Group();
            grpToAdd.DisplayName = department.Name;
            grpToAdd.MailEnabled = false;
            string withoutSpaces = Regex.Replace(department.Name, @"\s+", "");
            grpToAdd.MailNickname = withoutSpaces;
            grpToAdd.SecurityEnabled = true;
            grpToAdd.MailEnabled = false;

            var addedgroup = await graphClient.Groups.Request().AddAsync(grpToAdd);


            if (addedgroup != null)
                return true;
            else
                return false;
        }

      
    }
}
