using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;

namespace ArthaShikshaWeb.Services
{
    public interface IMenuService
    {
        Task<List<MenuItem>> GetAuthorizedMenuItemsAsync();
    }

    public class MenuService : IMenuService
    {
        private readonly ISessionStorageService _sessionStorage;
        private readonly NavigationManager _navigationManager;

        public MenuService(ISessionStorageService sessionStorage, NavigationManager navigationManager)
        {
            _sessionStorage = sessionStorage;
            _navigationManager = navigationManager;
        }

        public async Task<List<MenuItem>> GetAuthorizedMenuItemsAsync()
        {
            try
            {
                var clientId = await _sessionStorage.GetItemAsStringAsync("UserSessionCompanyId");
                var roleId = await _sessionStorage.GetItemAsStringAsync("UserSessionRoleId");

                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(roleId))
                {
                    _navigationManager.NavigateTo("/login", true);
                    return new List<MenuItem>();
                }

                // Clean and parse the values
                var cleanClientId = CleanAndParseId(clientId);
                var cleanRoleId = CleanAndParseId(roleId);

                if (!cleanClientId.HasValue || !cleanRoleId.HasValue)
                {
                    Console.WriteLine($"Failed to parse IDs. ClientId: {clientId}, RoleId: {roleId}");
                    return new List<MenuItem>();
                }

                var menuItems = GetAllMenuItems();
                var allowedMenus = GetAllowedMenus(cleanClientId.Value, cleanRoleId.Value);

                return menuItems.Where(item => allowedMenus.Contains(item.Name)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAuthorizedMenuItemsAsync: {ex.Message}");
                return new List<MenuItem>();
            }
        }

        private int? CleanAndParseId(string value)
        {
            try
            {
                // Remove quotes and whitespace
                value = value.Trim().Trim('"', '\'');

                if (int.TryParse(value, out int result))
                {
                    return result;
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

        private List<string> GetAllowedMenus(int clientId, int roleId)
        {
            if (clientId == 1 && roleId == 1)
                return SuperUserMenus;
            else if (clientId != 1 && roleId == 2)
                return ClientAdminMenus;
            else if (clientId != 1 && roleId == 3)
                return HodMenus;
            else if (clientId != 1 && (roleId == 4 || roleId == 5))
                return FacultyMenus;
            return new List<string>();
        }

        private static readonly List<string> SuperUserMenus = new()
        {
            "Dashboard",
            "Create Client",
            "Admin Management",
            "OS Level Management",
            "Program Management",
            "Department Management",
            "Hierarchy Management",
            "Role Management",
            "Faculty Management",
            "Course Management",
            "Student Data Management",
            "Batch Management",
            "Timetable Management"
        };

        private static readonly List<string> ClientAdminMenus = new()
        {
            "Dashboard",
            "Program Management",
            "Department Management",
            "Hierarchy Management",
            "Role Management",
            "Faculty Management",
            "Course Management",
            "Student Data Management",
            "Batch Management",
            "Timetable Management"
        };

        private static readonly List<string> HodMenus = new()
        {
            "Dashboard",
            "Faculty Management",
            "Course Management",
            "Student Data Management",
            "Batch Management",
            "Timetable Management"
        };

        private static readonly List<string> FacultyMenus = new()
        {
            "Dashboard",
            "Course Management",
            "Student Data Management",
            "Batch Management",
            "Timetable Management"
        };

        private static List<MenuItem> GetAllMenuItems() => new()
        {
            new MenuItem("Dashboard", "dashboard", "/dashboard"),
            new MenuItem("Create Client", "business", "/client-management"),
            new MenuItem("Admin Management", "admin_panel_settings", "/admin-management"),
            new MenuItem("OS Level Management", "settings_applications", "/os-management"),
            new MenuItem("Program Management", "school", "/program-management"),
            new MenuItem("Department Management", "apartment", "/department-management"),
            new MenuItem("Hierarchy Management", "account_tree", "/hierarchy-management"),
            new MenuItem("Role Management", "supervisor_account", "/role-management"),
            new MenuItem("Faculty Management", "people", "/faculty-management"),
            new MenuItem("Course Management", "menu_book", "/course-management"),
            new MenuItem("Student Data Management", "school", "/student-management"),
            new MenuItem("Batch Management", "layers", "/batch-management"),
            new MenuItem("Timetable Management", "event", "/timetable-management")
        };
    }

    public record MenuItem(string Name, string Icon, string Path);
}