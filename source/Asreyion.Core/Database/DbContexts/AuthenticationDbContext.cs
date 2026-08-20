using Asreyion.Core.Areas.Account.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Asreyion.Core.Database.DbContexts;

public class AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
{

}