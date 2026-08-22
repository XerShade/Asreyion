using System.Collections.Generic;

namespace Asreyion.Core.Areas.Administration.Models
{
    /// <summary>
    /// View model for the admin sidebar navigation
    /// </summary>
    public class AdminSidebarViewModel
    {
        /// <summary>
        /// Custom navigation sections for modular extensions
        /// </summary>
        public List<AdminNavSection> CustomSections { get; set; } = new();
    }

    /// <summary>
    /// Represents a navigation section in the admin sidebar
    /// </summary>
    public class AdminNavSection
    {
        /// <summary>
        /// Section title
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Navigation items in this section
        /// </summary>
        public List<AdminNavItem> Items { get; set; } = new();
    }

    /// <summary>
    /// Represents a single navigation item
    /// </summary>
    public class AdminNavItem
    {
        /// <summary>
        /// Display label
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Navigation URL
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// SVG icon path content
        /// </summary>
        public string IconSvg { get; set; } = string.Empty;

        /// <summary>
        /// Whether this item is currently active
        /// </summary>
        public bool IsActive { get; set; }
    }
}
