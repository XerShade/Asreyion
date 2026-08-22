using Asreyion.Core.Areas.Administration.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Asreyion.Core.Areas.Administration.Controllers;

[Area("Administration"), Authorize(Roles = "Administrator")]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        DashboardViewModel dashboardModel = CreateDashboardViewModel();
        AdminSidebarViewModel sidebarModel = CreateSidebarViewModel();
        AdminHeaderActionsViewModel headerActionsModel = CreateHeaderActionsViewModel();

        this.ViewData["SidebarModel"] = sidebarModel;
        this.ViewData["HeaderActionsModel"] = headerActionsModel;

        return this.View(dashboardModel);
    }

    private static DashboardViewModel CreateDashboardViewModel() => new()
    {
        TotalUsers = 1247,
        UserGrowth = 0.15,
        ActiveSessions = 89,
        SessionGrowth = 0.08,
        TotalPages = 342,
        PageChange = "+12",
        SystemHealth = 95,
        HealthStatus = "Excellent",
        HealthStatusClass = "positive",
        DatabaseStatus = "Connected",
        DatabaseStatusClass = "admin-badge-success",
        CacheStatus = "Active",
        CacheStatusClass = "admin-badge-success",
        StorageStatus = "Healthy",
        StorageStatusClass = "admin-badge-success",
        ApiStatus = "Operational",
        ApiStatusClass = "admin-badge-success",
        Uptime = "45d 12h 34m",
        Version = "1.0.0",
        RecentActivities = new List<RecentActivity>
            {
                new()
                {
                    Timestamp = DateTime.Now.AddMinutes(-5),
                    UserName = "admin",
                    Action = "Updated user permissions",
                    Status = "Success",
                    StatusClass = "admin-badge-success"
                },
                new()
                {
                    Timestamp = DateTime.Now.AddMinutes(-15),
                    UserName = "john.doe",
                    Action = "Created new page",
                    Status = "Success",
                    StatusClass = "admin-badge-success"
                },
                new()
                {
                    Timestamp = DateTime.Now.AddMinutes(-30),
                    UserName = "system",
                    Action = "Database backup completed",
                    Status = "Success",
                    StatusClass = "admin-badge-success"
                },
                new()
                {
                    Timestamp = DateTime.Now.AddHours(-1),
                    UserName = "admin",
                    Action = "Failed login attempt",
                    Status = "Failed",
                    StatusClass = "admin-badge-danger"
                },
                new()
                {
                    Timestamp = DateTime.Now.AddHours(-2),
                    UserName = "jane.smith",
                    Action = "Updated settings",
                    Status = "Success",
                    StatusClass = "admin-badge-success"
                }
            }
    };

    private static AdminSidebarViewModel CreateSidebarViewModel() => new ()
    {
        CustomSections = []
    };

    private static AdminHeaderActionsViewModel CreateHeaderActionsViewModel() => new ()
    {
        CustomActions = []
    };
}