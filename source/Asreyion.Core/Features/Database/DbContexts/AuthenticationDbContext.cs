using Asreyion.Core.Features.Database.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Asreyion.Core.Features.Database.DbContexts;

public class AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
{

}