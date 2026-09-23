using Asreyion.Core.Features.Authentication.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Asreyion.Core.Features.Database.DbContexts;

public class AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
{

}