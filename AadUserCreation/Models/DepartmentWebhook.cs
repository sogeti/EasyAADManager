using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AadUserCreation.Models
{
    public class DepartmentWebhook
    {
        public string UPN { get; set; }

        public string AddGroupDep{ get; set; }
        public string RemoveGroupDep     { get; set; }
    }
}
