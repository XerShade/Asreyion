using System;
using System.Collections.Generic;

namespace Asreyion.Core.Areas.Moderation.Models
{
    /// <summary>
    /// View model for the moderation dashboard
    /// </summary>
    public class ModerationDashboardViewModel
    {
        // Statistics
        /// <summary>
        /// Total number of pending content reports
        /// </summary>
        public int PendingReports { get; set; }

        /// <summary>
        /// Number of reports reviewed today
        /// </summary>
        public int ReportsReviewedToday { get; set; }

        /// <summary>
        /// Total number of flagged users
        /// </summary>
        public int FlaggedUsers { get; set; }

        /// <summary>
        /// Number of active moderator actions
        /// </summary>
        public int ActiveActions { get; set; }

        // Recent Content Reports
        /// <summary>
        /// List of recent content reports
        /// </summary>
        public List<ContentReport> RecentContentReports { get; set; } = new();

        // Recent User Reports
        /// <summary>
        /// List of recent user reports
        /// </summary>
        public List<UserReport> RecentUserReports { get; set; } = new();

        // Moderation Queue
        /// <summary>
        /// Items in the moderation queue
        /// </summary>
        public List<ModerationQueueItem> ModerationQueue { get; set; } = new();

        // Recent Actions
        /// <summary>
        /// List of recent moderation actions
        /// </summary>
        public List<ModerationAction> RecentActions { get; set; } = new();
    }

    /// <summary>
    /// Represents a content report
    /// </summary>
    public class ContentReport
    {
        /// <summary>
        /// Report ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Content type (e.g., Post, Comment, Message)
        /// </summary>
        public string ContentType { get; set; } = string.Empty;

        /// <summary>
        /// Content ID
        /// </summary>
        public int ContentId { get; set; }

        /// <summary>
        /// Content preview/snippet
        /// </summary>
        public string ContentPreview { get; set; } = string.Empty;

        /// <summary>
        /// Reporter username
        /// </summary>
        public string ReporterUsername { get; set; } = string.Empty;

        /// <summary>
        /// Author username
        /// </summary>
        public string AuthorUsername { get; set; } = string.Empty;

        /// <summary>
        /// Report reason
        /// </summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// Report status
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// CSS class for status badge
        /// </summary>
        public string StatusClass { get; set; } = string.Empty;

        /// <summary>
        /// Report timestamp
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Priority level
        /// </summary>
        public string Priority { get; set; } = string.Empty;

        /// <summary>
        /// CSS class for priority badge
        /// </summary>
        public string PriorityClass { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents a user report
    /// </summary>
    public class UserReport
    {
        /// <summary>
        /// Report ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Reported user ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Reported username
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Reporter username
        /// </summary>
        public string ReporterUsername { get; set; } = string.Empty;

        /// <summary>
        /// Report reason
        /// </summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// Report status
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// CSS class for status badge
        /// </summary>
        public string StatusClass { get; set; } = string.Empty;

        /// <summary>
        /// Report timestamp
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Number of reports against this user
        /// </summary>
        public int ReportCount { get; set; }
    }

    /// <summary>
    /// Represents an item in the moderation queue
    /// </summary>
    public class ModerationQueueItem
    {
        /// <summary>
        /// Queue ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Item type (Content or User)
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Item description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Priority level
        /// </summary>
        public string Priority { get; set; } = string.Empty;

        /// <summary>
        /// CSS class for priority badge
        /// </summary>
        public string PriorityClass { get; set; } = string.Empty;

        /// <summary>
        /// Time in queue
        /// </summary>
        public string TimeInQueue { get; set; } = string.Empty;

        /// <summary>
        /// Assigned moderator (if any)
        /// </summary>
        public string? AssignedModerator { get; set; }
    }

    /// <summary>
    /// Represents a moderation action taken
    /// </summary>
    public class ModerationAction
    {
        /// <summary>
        /// Action ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Moderator username
        /// </summary>
        public string ModeratorUsername { get; set; } = string.Empty;

        /// <summary>
        /// Action type (e.g., Content Removed, User Warned, User Banned)
        /// </summary>
        public string ActionType { get; set; } = string.Empty;

        /// <summary>
        /// Target description
        /// </summary>
        public string Target { get; set; } = string.Empty;

        /// <summary>
        /// Action notes
        /// </summary>
        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// Action timestamp
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
