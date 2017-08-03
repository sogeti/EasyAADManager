using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using AadUserCreation.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AadUserCreation.Models
{
    public class User
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string UPN { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Displayname")]
        public string DisplayName { get; set; }

        [Required]
        [Display(Name = "Department")]
        public string Department { get; set; }

        [Required]
        [Display(Name = "Suffix")]
        public string Suffix { get; set; }
    }
}
