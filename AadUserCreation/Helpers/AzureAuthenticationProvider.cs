using Microsoft.Graph;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AadUserCreation.Helpers
{
    public class AzureAuthenticationProvider : IAuthenticationProvider
    {
        private ClientCredential _creds;
        private string _authority;
        public AzureAuthenticationProvider(ClientCredential creds, string authority)
        {
            _creds = creds;
            _authority = authority;

        }
        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {


            AuthenticationContext authContext = new AuthenticationContext(_authority);



            AuthenticationResult authResult = await authContext.AcquireTokenAsync("https://graph.microsoft.com/", _creds);

            request.Headers.Add("Authorization", "Bearer " + authResult.AccessToken);
        }
    }
}
