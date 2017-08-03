using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AadUserCreation.Models;

namespace AadUserCreation.Models
{
    public class AadUserCreationContext : DbContext
    {
        public AadUserCreationContext (DbContextOptions<AadUserCreationContext> options)
            : base(options)
        {
        }

        public DbSet<AadUserCreation.Models.User> User { get; set; }

        public DbSet<AadUserCreation.Models.GroupUser> GroupUser { get; set; }
    }
}
