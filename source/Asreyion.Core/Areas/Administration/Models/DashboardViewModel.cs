using System;
using System.Collections.Generic;

namespace Asreyion.Core.Areas.Administration.Models
{
    /// <summary>
    /// View model for the admin dashboard
    /// </summary>
    public class DashboardViewModel
    {
        // Statistics
        /// <summary>
        /// Total number of users
        /// </summary>
        public int TotalUsers { get; set; }

        /// <summary>
        /// User growth percentage from last month
        /// </summary>
        public double UserGrowth { get; set; }

        /// <summary>
        /// Number of active sessions
        /// </summary>
        public int ActiveSessions { get; set; }

        /// <summary>
        /// Session growth percentage from last hour
        /// </summary>
        public double SessionGrowth { get; set; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Page change from last week (e.g., "+12", "-5")
        /// </summary>
        public string PageChange { get; set; } = string.Empty;

        /// <summary>
        /// System health score (0-100)
        /// </summary>
        public int SystemHealth { get; set; }

        /// <summary>
        /// Health status text
        /// </summary>
        public string HealthStatus { get; set; } = string.Empty;

        /// <summary>
        /// CSS class for health status
        /// </summary>
        public string HealthStatusClass { get; set; } = string.Empty;

        // System Status
        /// <summary>
        /// Database connection status
        /// </summary>
        public string DatabaseStatus { get; set; } = string.Empty;

        /// <summary>
        /// CSS class for database status
        /// </summary>
        public string DatabaseStatusClass { get; set; } = string.Empty;

        /// <summary>
        /// Cache server status
        /// </summary>
        public string CacheStatus { get; set; } = string.Empty;

        /// <summary>
        /// CSS class for cache status
        /// </summary>
        public string CacheStatusClass { get; set; } = string.Empty;

        /// <summary>
        /// File storage status
        /// </summary>
        public string StorageStatus { get; set; } = string.Empty;

        /// <summary>
        /// CSS class for storage status
        /// </summary>
        public string StorageStatusClass { get; set; } = string.Empty;

        /// <summary>
        /// API gateway status
        /// </summary>
        public string ApiStatus { get; set; } = string.Empty;

        /// <summary>
        /// CSS class for API status
        /// </summary>
        public string ApiStatusClass { get; set; } = string.Empty;

        /// <summary>
        /// System uptime
        /// </summary>
        public string Uptime { get; set; } = string.Empty;

        /// <summary>
        /// Application version
        /// </summary>
        public string Version { get; set; } = string.Empty;

        // Recent Activity
        /// <summary>
        /// List of recent activities
        /// </summary>
        public List<RecentActivity> RecentActivities { get; set; } = new();

        // Custom Widgets
        /// <summary>
        /// Custom dashboard widgets for modular extensions
        /// </summary>
        public List<DashboardWidget> CustomWidgets { get; set; } = new();
    }

    /// <summary>
    /// Represents a recent activity entry
    /// </summary>
    public class RecentActivity
    {
        /// <summary>
        /// Timestamp of the activity
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Username who performed the action
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Action description
        /// </summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// Status text
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// CSS class for status badge
        /// </summary>
        public string StatusClass { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents a custom dashboard widget
    /// </summary>
    public class DashboardWidget
    {
        /// <summary>
        /// Widget title
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Widget HTML content
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Widget actions
        /// </summary>
        public List<DashboardWidgetAction> Actions { get; set; } = new();
    }

    /// <summary>
    /// Represents an action button for a dashboard widget
    /// </summary>
    public class DashboardWidgetAction
    {
        /// <summary>
        /// Button label
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Action URL
        /// </summary>
        public string Url { get; set; } = string.Empty;
    }
}
