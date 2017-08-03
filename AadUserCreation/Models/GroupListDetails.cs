using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AadUserCreation.Models
{

    public class GroupList
    {
        public List<string> Members { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }

        public bool Selected { get; set; }
    }

    public class DepartmentList
    {
        public List<string> Members { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }

        public bool Selected { get; set; }
    }

    public class GroupListDetails
    {

        public List<GroupList> GroupList { get; set; }
        public List<DepartmentList> DepartmentList { get; set; }
    }












    //public class GroupListDetails
    //{

    //    public List<GroupDetails> GroupList{ get; set; }
    //    public List<GroupDetails> DepartmentList { get; set; }
    //}

    //public class GroupDetails
    //{
    //    public List<string> Members{ get; set; }
    //    public string Id { get; internal set; }
    //    public object Name { get; internal set; }
    //}

    //public class GroupMemeber
    //{
    //    public string UPN { get; set; }
    //    public string Mail { get; internal set; }
    //}
}
