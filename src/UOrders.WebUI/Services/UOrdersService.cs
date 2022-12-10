using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using UOrders.DTOModel.V1;
using UOrders.DTOModel.V1.Authentication;
using UOrders.WebUI.AuthProviders;
using UOrders.WebUI.Common;
using UOrders.WebUI.Properties.Resources;

namespace UOrders.WebUI.Services;

public class UOrdersService : IUOrdersService
{
    #region Private Fields

    private readonly AuthenticationStateProvider _authStateProvider;
    private readonly IStringLocalizer<Resource> _localizer;
    private readonly HttpClient _httpClient;

    #endregion Private Fields

    #region Public Constructors

    public UOrdersService(HttpClient client, AuthenticationStateProvider authStateProvider, IStringLocalizer<Resource> localizer)
    {
        _httpClient = client;
        _authStateProvider = authStateProvider;
        _localizer = localizer;
    }

    #endregion Public Constructors

    #region Public Methods

    public async Task<int> CreateCategory(MenuCategoryCreateDTO category) =>
        await CreateRessource<MenuCategoryCreateDTO, int>("Menu/categories", category);

    public async Task<int> CreateMenuItem(MenuItemCreateDTO menuItem, int categoryId) =>
        await CreateRessource<MenuItemCreateDTO, int>($"Menu/categories/{categoryId}/items", menuItem);

    public async Task<int> CreateMenuItemOption(MenuItemOptionCreateDTO menuItemOption, int itemId) =>
        await CreateRessource<MenuItemOptionCreateDTO, int>($"Menu/items/{itemId}/options", menuItemOption);

    public async Task<int> CreateMenuItemOptionValue(MenuItemOptionValueCreateDTO menuItemOptionValue, int itemId, int optionId) =>
        await CreateRessource<MenuItemOptionValueCreateDTO, int>($"Menu/items/{itemId}/options/{optionId}/values", menuItemOptionValue);

    public async Task<int> CreateOrder(OrderCreateDTO order) => 
        await CreateRessource<OrderCreateDTO, int>("Orders/", order);

    public async Task DeleteCategory(int categoryId) =>
        await DeleteRessource($"Menu/categories/{categoryId}?cascade=true");

    public async Task DeleteMenuItem(int menuItemId) =>
        await DeleteRessource($"Menu/items/{menuItemId}?cascade=true");

    public async Task DeleteMenuItemOption(int menuItemId, int menuItemOptionId) =>
        await DeleteRessource($"Menu/items/{menuItemId}/options/{menuItemOptionId}?cascade=true");

    public async Task DeleteMenuItemOptionValue(int menuItemId, int menuItemOptionId, int menuItemOptionValueId) =>
        await DeleteRessource($"Menu/items/{menuItemId}/options/{menuItemOptionId}/values/{menuItemOptionValueId}");

    public async Task<string> Echo(string str)
    {
        try
        {
            var result = await _httpClient.PostAsJsonAsync("System/echo", str);
            if (!result.IsSuccessStatusCode)
                return string.Empty;
            return await result.Content.ReadAsStringAsync();
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }

    public async Task<IEnumerable<MenuCategoryDTO>?> GetMenu() =>
        await GetRessourceAsync<MenuCategoryDTO[]>($"Menu/categories?lang={_localizer["str_sys_api_lang"]}");

    public async Task<MenuCategoryDetailedDTO?> GetMenuCategory(int catId) =>
        await GetRessourceAsync<MenuCategoryDetailedDTO>($"Menu/categories/{catId}");

    public async Task<MenuItemDetailedDTO?> GetMenuItem(int menuItemId) =>
        await GetRessourceAsync<MenuItemDetailedDTO>($"Menu/items/{menuItemId}");

    public async Task<MenuItemOptionDetailedDTO?> GetMenuItemOption(int menuItemId, int menuItemOptionId) =>
        await GetRessourceAsync<MenuItemOptionDetailedDTO>($"Menu/items/{menuItemId}/options/{menuItemOptionId}");

    public async Task<MenuItemOptionValueDetailedDTO?> GetMenuItemOptionValue(int menuItemId, int menuItemOptionId, int menuItemOptionValueId) =>
        await GetRessourceAsync<MenuItemOptionValueDetailedDTO>($"Menu/items/{menuItemId}/options/{menuItemOptionId}/values/{menuItemOptionValueId}");

    public async Task<OrderDTO?> GetOrder(int orderId) =>
        await GetRessourceAsync<OrderDTO>($"Orders/{orderId}?lang={_localizer["str_sys_api_lang"]}");

    public async Task<OrderDTO?> GetMyOrder(int orderId) =>
        await GetRessourceAsync<OrderDTO>($"My/orders/{orderId}?lang={_localizer["str_sys_api_lang"]}");

    public async Task<DataPage<OrderOverviewDTO>?> GetOrderOverview(int page, int size) =>
        await GetPagedRessource<OrderOverviewDTO>("Orders", page, size);

    public async Task<DataPage<OrderOverviewDTO>?> GetMyOrderOverview(int page, int size) =>
        await GetPagedRessource<OrderOverviewDTO>("My/orders", page, size);

    public async Task<bool> Login(LoginDTO userForAuthentication)
    {
        var authResult = await _httpClient.PostAsJsonAsync("Authenticate/login", userForAuthentication);

        if (!authResult.IsSuccessStatusCode)
            return false;

        var token = await authResult.Content.ReadAsStringAsync();
        await ((AuthStateProvider)_authStateProvider).Login(token);

        return true;
    }

    public async Task Logout()
    {
        await ((AuthStateProvider)_authStateProvider).Logout();
    }

    public async Task<bool> RegisterUser(RegisterDTO userForRegistration)
    {
        var authResult = await _httpClient.PostAsJsonAsync("Authenticate/register", userForRegistration);
        
        return authResult.IsSuccessStatusCode;
    }

    public async Task<bool> IsUserAvailable(string userName, CancellationToken cancellationToken = default) =>
        await GetRessourceAsync<bool>($"Authenticate/userAvailable/{userName}");

    public async Task<UserDTO?> GetCurrentUser() =>
        await GetRessourceAsync<UserDTO>("Authenticate/me");

    public async Task UpdateCategory(MenuCategoryUpdateDTO category, int catId) =>
        await UpdateResource($"Menu/categories/{catId}", category);

    public async Task UpdateItemOptionValuePrice(int menuItemId, int menuItemOptionId, int menuItemOptionValueId, decimal price)
    {
        await _httpClient.PatchAsync($"Menu/items/{menuItemId}/options/{menuItemOptionId}/values/{menuItemOptionValueId}/priceChangeToBase",
            new StringContent(price.ToString(CultureInfo.InvariantCulture), Encoding.UTF8, "text/json"));
    }

    public async Task UpdateItemPrice(int menuItemId, decimal price)
    {
        await _httpClient.PatchAsync($"Menu/items/{menuItemId}/currentPrice",
            new StringContent(price.ToString(CultureInfo.InvariantCulture), Encoding.UTF8, "text/json"));
    }

    public async Task UpdateMenuItem(MenuItemUpdateDTO menuItem, int menuItemId) =>
        await UpdateResource($"Menu/items/{menuItemId}", menuItem);

    public async Task UpdateMenuItemOption(MenuItemOptionUpdateDTO menuItemOption, int menuItemId, int menuItemOptionId) =>
        await UpdateResource($"Menu/items/{menuItemId}/options/{menuItemOptionId}", menuItemOption);

    public async Task UpdateMenuItemOptionValue(MenuItemOptionValueUpdateDTO menuItemOptionValue, int menuItemId, int menuItemOptionId, int menuItemOptionValueId) =>
        await UpdateResource($"Menu/items/{menuItemId}/options/{menuItemOptionId}/values/{menuItemOptionValueId}", menuItemOptionValue);

    public async Task<DataPage<UserDTO>?> GetUsers(int page = 1, int size = 10) =>
        await GetPagedRessource<UserDTO>("Authenticate/users", page, size);

    public async Task AddUserToRole(UserDTO user, string role) =>
        await _httpClient.PostAsync($"Authenticate/users/{user.Id}/roles/{role}", null);

    public async Task RemoveUserFromRole(UserDTO user, string role) =>
        await _httpClient.DeleteAsync($"Authenticate/users/{user.Id}/roles/{role}");

    public async Task<IEnumerable<ReviewDTO>> GetReviews() =>
        await GetRessourceAsync<IEnumerable<ReviewDTO>>("Reviews/latest") ?? Enumerable.Empty<ReviewDTO>();

    #endregion Public Methods

    #region Private Methods

    private async Task<TResponse?> CreateRessource<T, TResponse>(string ressource, T payload) where T : notnull
    {
        TResponse? result = default;

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(ressource, payload);
        if (response != null && response.IsSuccessStatusCode)
            result = await response.Content.ReadAsAsync<TResponse>();

        return result;
    }

    private async Task<bool> DeleteRessource(string ressource)
    {
        var response = await _httpClient.DeleteAsync(ressource);
        return response.IsSuccessStatusCode;
    }

    private async Task<DataPage<T>?> GetPagedRessource<T>(string ressource, int page, int size)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri((_httpClient.BaseAddress?.ToString() ?? "") + ressource)
        };
        request.Headers.Add("X-Pagination-PageIdx", page.ToString());
        request.Headers.Add("X-Pagination-PageSize", size.ToString());
        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return new DataPage<T>(await response.Content.ReadAsAsync<IEnumerable<T>>(),
                size: Convert.ToInt32(response.Headers.GetValues("X-Pagination-PageSize").Single()),
                index: Convert.ToInt32(response.Headers.GetValues("X-Pagination-PageIdx").Single()),
                totalPages: Convert.ToInt32(response.Headers.GetValues("X-Pagination-TotalPages").Single()),
                totalRecords: Convert.ToInt32(response.Headers.GetValues("X-Pagination-TotalRecords").Single())
                );
        }
        else return null;
    }

    private async Task<T?> GetRessourceAsync<T>(string ressource, CancellationToken cancellationToken = default) where T : notnull
    {
        T? result = default;

        HttpResponseMessage response = await _httpClient.GetAsync(ressource, cancellationToken);
        if (response != null && response.IsSuccessStatusCode)
            result = await response.Content.ReadAsAsync<T>(cancellationToken);

        return result;
    }

    private async Task<bool> UpdateResource<T>(string resource, T entity)
    {
        var response = await _httpClient.PutAsJsonAsync(resource, entity);
        return response.IsSuccessStatusCode;
    }

    public async Task CreateReview(int orderId, Guid reviewToken, OrderReviewCreateDTO review) =>
        await CreateRessource<OrderReviewCreateDTO, object>($"Orders/{orderId}/reviews?reviewToken={reviewToken}", review);

    public async Task<UserDTO> GetUser() =>
        await GetRessourceAsync<UserDTO>("My") ?? new();

    public async Task<bool> UpdateUser(UserDTO user) =>
        await UpdateResource("My", user);

    #endregion Private Methods
}