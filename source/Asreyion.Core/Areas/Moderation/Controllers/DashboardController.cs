using Microsoft.AspNetCore.Mvc;
using Asreyion.Core.Areas.Moderation.Models;

namespace Asreyion.Core.Areas.Moderation.Controllers;

[Area("Moderation")]
public class DashboardController : Controller
{
    public async Task<IActionResult> Index()
    {
        var model = new ModerationDashboardViewModel
        {
            // Statistics
            PendingReports = 24,
            ReportsReviewedToday = 47,
            FlaggedUsers = 8,
            ActiveActions = 3,

            // Recent Content Reports
            RecentContentReports = new List<ContentReport>
            {
                new ContentReport
                {
                    Id = 1,
                    ContentType = "Post",
                    ContentId = 1234,
                    ContentPreview = "This is inappropriate content that needs to be reviewed...",
                    ReporterUsername = "user123",
                    AuthorUsername = "problematic_user",
                    Reason = "Spam",
                    Status = "Pending",
                    StatusClass = "moderation-badge-warning",
                    CreatedAt = DateTime.Now.AddMinutes(-15),
                    Priority = "High",
                    PriorityClass = "moderation-badge-danger"
                },
                new ContentReport
                {
                    Id = 2,
                    ContentType = "Comment",
                    ContentId = 5678,
                    ContentPreview = "Offensive language detected in this comment...",
                    ReporterUsername = "moderator_jane",
                    AuthorUsername = "troll_account",
                    Reason = "Harassment",
                    Status = "In Review",
                    StatusClass = "moderation-badge-info",
                    CreatedAt = DateTime.Now.AddMinutes(-45),
                    Priority = "Medium",
                    PriorityClass = "moderation-badge-warning"
                },
                new ContentReport
                {
                    Id = 3,
                    ContentType = "Message",
                    ContentId = 9012,
                    ContentPreview = "Suspicious activity detected in private messages...",
                    ReporterUsername = "system_auto",
                    AuthorUsername = "suspicious_user",
                    Reason = "Phishing Attempt",
                    Status = "Pending",
                    StatusClass = "moderation-badge-warning",
                    CreatedAt = DateTime.Now.AddHours(-1),
                    Priority = "Critical",
                    PriorityClass = "moderation-badge-danger"
                },
                new ContentReport
                {
                    Id = 4,
                    ContentType = "Post",
                    ContentId = 3456,
                    ContentPreview = "Copyright infringement reported for this content...",
                    ReporterUsername = "content_owner",
                    AuthorUsername = "reposter",
                    Reason = "Copyright Violation",
                    Status = "Pending",
                    StatusClass = "moderation-badge-warning",
                    CreatedAt = DateTime.Now.AddHours(-2),
                    Priority = "Low",
                    PriorityClass = "moderation-badge-success"
                },
                new ContentReport
                {
                    Id = 5,
                    ContentType = "Comment",
                    ContentId = 7890,
                    ContentPreview = "Hate speech detected in user comment...",
                    ReporterUsername = "community_member",
                    AuthorUsername = "hate_speaker",
                    Reason = "Hate Speech",
                    Status = "In Review",
                    StatusClass = "moderation-badge-info",
                    CreatedAt = DateTime.Now.AddHours(-3),
                    Priority = "High",
                    PriorityClass = "moderation-badge-danger"
                }
            },

            // Recent User Reports
            RecentUserReports = new List<UserReport>
            {
                new UserReport
                {
                    Id = 1,
                    UserId = 101,
                    Username = "spam_bot_01",
                    ReporterUsername = "multiple_users",
                    Reason = "Spam Bot",
                    Status = "Pending",
                    StatusClass = "moderation-badge-warning",
                    CreatedAt = DateTime.Now.AddMinutes(-30),
                    ReportCount = 15
                },
                new UserReport
                {
                    Id = 2,
                    UserId = 102,
                    Username = "toxic_user",
                    ReporterUsername = "moderator_team",
                    Reason = "Toxic Behavior",
                    Status = "In Review",
                    StatusClass = "moderation-badge-info",
                    CreatedAt = DateTime.Now.AddHours(-1),
                    ReportCount = 8
                },
                new UserReport
                {
                    Id = 3,
                    UserId = 103,
                    Username = "impersonator",
                    ReporterUsername = "verified_user",
                    Reason = "Impersonation",
                    Status = "Pending",
                    StatusClass = "moderation-badge-warning",
                    CreatedAt = DateTime.Now.AddHours(-2),
                    ReportCount = 3
                },
                new UserReport
                {
                    Id = 4,
                    UserId = 104,
                    Username = "scammer_account",
                    ReporterUsername = "victim_01",
                    Reason = "Scamming",
                    Status = "Pending",
                    StatusClass = "moderation-badge-warning",
                    CreatedAt = DateTime.Now.AddHours(-4),
                    ReportCount = 12
                }
            },

            // Moderation Queue
            ModerationQueue = new List<ModerationQueueItem>
            {
                new ModerationQueueItem
                {
                    Id = 1,
                    Type = "Content",
                    Description = "Post #1234 - Potential spam content",
                    Priority = "High",
                    PriorityClass = "moderation-badge-danger",
                    TimeInQueue = "15m",
                    AssignedModerator = null
                },
                new ModerationQueueItem
                {
                    Id = 2,
                    Type = "User",
                    Description = "User @spam_bot_01 - Multiple spam reports",
                    Priority = "Critical",
                    PriorityClass = "moderation-badge-danger",
                    TimeInQueue = "30m",
                    AssignedModerator = "moderator_jane"
                },
                new ModerationQueueItem
                {
                    Id = 3,
                    Type = "Content",
                    Description = "Comment #5678 - Harassment report",
                    Priority = "Medium",
                    PriorityClass = "moderation-badge-warning",
                    TimeInQueue = "45m",
                    AssignedModerator = null
                },
                new ModerationQueueItem
                {
                    Id = 4,
                    Type = "User",
                    Description = "User @toxic_user - Toxic behavior pattern",
                    Priority = "High",
                    PriorityClass = "moderation-badge-danger",
                    TimeInQueue = "1h",
                    AssignedModerator = "moderator_bob"
                }
            },

            // Recent Actions
            RecentActions = new List<ModerationAction>
            {
                new ModerationAction
                {
                    Id = 1,
                    ModeratorUsername = "moderator_jane",
                    ActionType = "Content Removed",
                    Target = "Post #9876 by spammer_account",
                    Notes = "Clear spam violation, removed content",
                    Timestamp = DateTime.Now.AddMinutes(-10)
                },
                new ModerationAction
                {
                    Id = 2,
                    ModeratorUsername = "moderator_bob",
                    ActionType = "User Warned",
                    Target = "User @rude_user",
                    Notes = "First warning for harassment",
                    Timestamp = DateTime.Now.AddMinutes(-25)
                },
                new ModerationAction
                {
                    Id = 3,
                    ModeratorUsername = "moderator_jane",
                    ActionType = "Content Approved",
                    Target = "Post #5432 by legitimate_user",
                    Notes = "False positive, content is appropriate",
                    Timestamp = DateTime.Now.AddMinutes(-40)
                },
                new ModerationAction
                {
                    Id = 4,
                    ModeratorUsername = "admin_mike",
                    ActionType = "User Banned",
                    Target = "User @malicious_actor",
                    Notes = "Permanent ban for phishing attempts",
                    Timestamp = DateTime.Now.AddHours(-1)
                },
                new ModerationAction
                {
                    Id = 5,
                    ModeratorUsername = "moderator_bob",
                    ActionType = "Content Removed",
                    Target = "Comment #3210 by troll_account",
                    Notes = "Hate speech violation",
                    Timestamp = DateTime.Now.AddHours(-2)
                }
            }
        };

        return View(model);
    }
}