using Asreyion.Core.Authentication.Data;

namespace Asreyion.Core.Members.Models;

public class MemberListModel
{
    public const int MembersPerPage = 30;

    public IReadOnlyList<ApplicationUser> Members { get; init; } = [];

    public int Page { get; init; }

    public int TotalPages { get; init; }

    public int TotalMembers { get; init; }
}
