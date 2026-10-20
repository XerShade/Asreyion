using Asreyion.Core.Authentication.Data;
using Asreyion.Core.Members.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Asreyion.Core.Members.Controllers;

[Area("Members")]
public class MemberListController(
    UserManager<ApplicationUser> userManager) : Controller
{
    private readonly UserManager<ApplicationUser> UserManager = userManager;

    [Route("{Area}")]
    [Route("{Area}/Page/{Page:int:min(1)}")]
    public async Task<IActionResult> Index(int page = 1)
    {
        if (page < 1)
        {
            page = 1;
        }

        IQueryable<ApplicationUser> activeMembers = this.UserManager.Users
            .AsNoTracking()
            .Where(user => user.IsActive);

        int totalMembers = await activeMembers.CountAsync();
        int totalPages = Math.Max(1, (int)Math.Ceiling(totalMembers / (double)MemberListModel.MembersPerPage));
        page = Math.Min(page, totalPages);

        List<ApplicationUser> members = await activeMembers
            .OrderByDescending(user => user.CreatedAt)
            .ThenBy(user => user.Id)
            .Skip((page - 1) * MemberListModel.MembersPerPage)
            .Take(MemberListModel.MembersPerPage)
            .ToListAsync();

        MemberListModel model = new()
        {
            Members = members,
            Page = page,
            TotalPages = totalPages,
            TotalMembers = totalMembers
        };

        if (string.Equals(
            this.Request.Headers["X-Requested-With"],
            "XMLHttpRequest",
            StringComparison.OrdinalIgnoreCase))
        {
            return this.PartialView("_MemberList", model);
        }

        return this.View(model);
    }
}
