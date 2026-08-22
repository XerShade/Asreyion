using System.Collections.Generic;

namespace Asreyion.Core.Areas.Administration.Models
{
    /// <summary>
    /// View model for admin header actions
    /// </summary>
    public class AdminHeaderActionsViewModel
    {
        /// <summary>
        /// Custom header actions for modular extensions
        /// </summary>
        public List<AdminHeaderAction> CustomActions { get; set; } = new();
    }

    /// <summary>
    /// Represents a header action button
    /// </summary>
    public class AdminHeaderAction
    {
        /// <summary>
        /// Button label
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Action URL
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Tooltip title
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// SVG icon path content
        /// </summary>
        public string IconSvg { get; set; } = string.Empty;
    }
}
