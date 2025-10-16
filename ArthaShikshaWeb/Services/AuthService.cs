using ArthaShikshaWeb.Shared;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;

namespace ArthaShikshaWeb.Services
{
    public class AuthService : IAuthService
    {
        private readonly ISessionStorageService _sessionStorage;
        private readonly NavigationManager _navigationManager;

        public AuthService(ISessionStorageService sessionStorage, NavigationManager navigationManager)
        {
            _sessionStorage = sessionStorage;
            _navigationManager = navigationManager;
        }

        public async Task<bool> IsAuthenticated()
        {
            try
            {
                var userId = await _sessionStorage.GetItemAsStringAsync("UserSessionUserId");
                return !string.IsNullOrEmpty(userId);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> HasAccess(string menuName)
        {
            try
            {
                var clientId = await _sessionStorage.GetItemAsStringAsync("UserSessionCompanyId");
                var roleId = await _sessionStorage.GetItemAsStringAsync("UserSessionRoleId");

                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(roleId))
                    return false;

                // Define role conditions
                bool isSuperUser = clientId == "1" && roleId == "1";
                bool isClientAdmin = clientId != "1" && roleId == "2";
                bool isHOD = clientId != "1" && roleId == "3";
                bool isFaculty = clientId != "1" && (roleId == "4" || roleId == "5");

                // Menu access based on roles
                return menuName switch
                {
                    // Always accessible
                    "Dashboard" => true,

                    // Super User Only
                    "Create Client" or "Admin Management" or "OS Level Management" 
                        => isSuperUser,

                    // Super User and Client Admin
                    "Program Management" or "Department Management" or 
                    "Hierarchy Management" or "Role Management"
                        => isSuperUser || isClientAdmin,

                    // Super User, Client Admin, and HOD
                    "Faculty Management"
                        => isSuperUser || isClientAdmin || isHOD,

                    // Super User, Client Admin, HOD, and Faculty
                    "Course Management" or "Student Data Management" or 
                    "Batch Management" or "Timetable Management"
                        => isSuperUser || isClientAdmin || isHOD || isFaculty,

                    // Default - no access
                    _ => false
                };
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GetUserRole()
        {
            try
            {
                var clientId = await _sessionStorage.GetItemAsStringAsync("UserSessionCompanyId");
                var roleId = await _sessionStorage.GetItemAsStringAsync("UserSessionRoleId");

                return (clientId, roleId) switch
                {
                    ("1", "1") => "Super User",
                    (_, "2") when clientId != "1" => "Client Admin",
                    (_, "3") when clientId != "1" => "Head of Department",
                    (_, "4") or (_, "5") when clientId != "1" => "Faculty",
                    _ => "Unknown Role"
                };
            }
            catch
            {
                return "Unknown Role";
            }
        }

        public async Task<string> GetUserName()
        {
            try
            {
                var userName = await _sessionStorage.GetItemAsStringAsync("UserSessionUserName");
                return string.IsNullOrEmpty(userName) ? "User" : userName;
            }
            catch
            {
                return "User";
            }
        }

        public async Task Logout()
        {
            try
            {
                await _sessionStorage.ClearAsync();
                _navigationManager.NavigateTo("/login", true);
            }
            catch
            {
                // If clearing session fails, still redirect to login
                _navigationManager.NavigateTo("/login", true);
            }
        }

        // Helper method to get menu items based on user role
        public async Task<List<MenuItemModel>> GetAuthorizedMenuItems()
        {
            var menuItems = new List<MenuItemModel>
            {
                new("Dashboard", "dashboard", "/dashboard"),
                new("Create Client", "business", "/client-management"),
                new("Admin Management", "admin_panel_settings", "/admin-management"),
                new("OS Level Management", "settings_applications", "/os-management"),
                new("Program Management", "school", "/no-programs"),
                new("Department Management", "apartment", "/department-management"),
                new("Hierarchy Management", "account_tree", "/hierarchy-management"),
                new("Role Management", "supervisor_account", "/role-management"),
                new("Faculty Management", "people", "/faculty-management"),
                new("Course Management", "menu_book", "/course-management"),
                new("Student Management", "school", "/student-management"),
                new("Batch Management", "layers", "/batch-management"),
                new("Timetable Management", "event", "/timetable-management")
            };

            return (await Task.WhenAll(menuItems.Select(async item =>
                new { MenuItem = item, HasAccess = await HasAccess(item.Name) })))
                .Where(x => x.HasAccess)
                .Select(x => x.MenuItem)
                .ToList();
        }
    }

    public record MenuItemModel(string Name, string Icon, string Path);
}
