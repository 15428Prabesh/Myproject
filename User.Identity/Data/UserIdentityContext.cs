using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using User.Identity.Entities;

namespace User.Identity.Data
{
    public class UserIdentityContext : DbContext
    {
        public UserIdentityContext (DbContextOptions<UserIdentityContext> options)
            : base(options)
        {
        }

        public DbSet<User.Identity.Entities.Account> Accounts { get; set; } = default!;
    }
}
