using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AadUserCreation
{
    public class AppSettings
    {
        public string ClientId { get; set; }
        public string clientSecret { get; set; }

        public string directory { get; set; }

        public string ApplicationName { get; set; }
        public string WebHookUrlCreateUsersRunbook { get; set; }
        public string WebHookUrlEditUsersRunbook { get; set; }
        public string WebHookUrlDeleteUsersRunbook { get; set; }

        public string WebHookUrlUserMembershipGroupRunbook { get; set; }
        public string AzureStorageConnectionString { get; set; }

        public string MailAccountDomain{ get; set; }

        public string BlobContainerName { get; set; }
    }
}
