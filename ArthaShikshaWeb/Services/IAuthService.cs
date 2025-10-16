using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using ArthaShikshaWeb.Services;

namespace ArthaShikshaWeb.Shared
{
    public partial class Sidebar : ComponentBase
    {
        [Inject]
        private IAuthService AuthService { get; set; }

        private List<MenuItemModel> authorizedMenuItems = new List<MenuItemModel>();

        protected override async Task OnInitializedAsync()
        {
            authorizedMenuItems = await AuthService.GetAuthorizedMenuItems();
        }
    }
    public interface IAuthService
    {
        Task<List<MenuItemModel>> GetAuthorizedMenuItems();
    }
}