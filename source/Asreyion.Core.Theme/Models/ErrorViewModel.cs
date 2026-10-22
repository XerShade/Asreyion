namespace Asreyion.Core.Theme.Models;

/// <summary>
/// Information displayed on the server error page.
/// </summary>
public class ErrorViewModel
{
    /// <summary>
    /// Gets or sets the request identifier associated with the error.
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Gets whether the request identifier should be shown.
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(this.RequestId);
}
