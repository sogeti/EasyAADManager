using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AadUserCreation.Models
{
    public class GroupWebhook
    {
        public string UPN { get; set; }

        public string AddGroupApp { get; set; }
        public string RemoveGroupApp { get; set; }
    }
}
