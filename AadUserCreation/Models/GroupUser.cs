using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AadUserCreation.Models
{
    public class GroupUser
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string UPN { get; set; }

        [Required]
        [Display(Name = "Groups for user")]
        public List<GroupList> Groups { get; set; }

        public List<DepartmentList> Departments{ get; set; }

    }
}
