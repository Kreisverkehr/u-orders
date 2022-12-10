using System.Net.Http;
using UOrders.DTOModel.V1;
using UOrders.DTOModel.V1.Authentication;
using UOrders.WebUI.Common;

namespace UOrders.WebUI.Services;

public interface IUOrdersService
{
    #region Public Methods

    Task<int> CreateCategory(MenuCategoryCreateDTO category);

    Task<int> CreateMenuItem(MenuItemCreateDTO menuItem, int categoryId);

    Task<int> CreateMenuItemOption(MenuItemOptionCreateDTO menuItemOption, int itemId);

    Task<int> CreateMenuItemOptionValue(MenuItemOptionValueCreateDTO menuItemOptionValue, int itemId, int optionId);

    Task<int> CreateOrder(OrderCreateDTO order);

    Task DeleteCategory(int categoryId);

    Task DeleteMenuItem(int menuItemId);

    Task DeleteMenuItemOption(int menuItemId, int menuItemOptionId);

    Task DeleteMenuItemOptionValue(int menuItemId, int menuItemOptionId, int menuItemOptionValueId);

    Task<IEnumerable<MenuCategoryDTO>?> GetMenu();

    Task<MenuCategoryDetailedDTO?> GetMenuCategory(int catId);

    Task<MenuItemDetailedDTO?> GetMenuItem(int menuItemId);

    Task<MenuItemOptionDetailedDTO?> GetMenuItemOption(int menuItemId, int menuItemOptionId);

    Task<MenuItemOptionValueDetailedDTO?> GetMenuItemOptionValue(int menuItemId, int menuItemOptionId, int menuItemOptionValueId);

    Task<OrderDTO?> GetOrder(int oderId);

    Task<OrderDTO?> GetMyOrder(int oderId);

    Task<DataPage<OrderOverviewDTO>?> GetOrderOverview(int page, int size);

    Task<DataPage<OrderOverviewDTO>?> GetMyOrderOverview(int page, int size);

    Task<bool> Login(LoginDTO userForAuthentication);

    Task Logout();

    Task<bool> RegisterUser(RegisterDTO userForRegistration);

    Task<bool> IsUserAvailable(string userName, CancellationToken cancellationToken = default);

    Task<UserDTO?> GetCurrentUser();

    Task UpdateCategory(MenuCategoryUpdateDTO category, int catId);

    Task UpdateItemOptionValuePrice(int menuItemId, int menuItemOptionId, int menuItemOptionValueId, decimal price);

    Task UpdateItemPrice(int menuItemId, decimal price);

    Task UpdateMenuItem(MenuItemUpdateDTO menuItem, int menuItemId);

    Task UpdateMenuItemOption(MenuItemOptionUpdateDTO menuItemOption, int menuItemId, int menuItemOptionId);

    Task UpdateMenuItemOptionValue(MenuItemOptionValueUpdateDTO menuItemOptionValue, int menuItemId, int menuItemOptionId, int menuItemOptionValueId);

    Task<string> Echo(string str);

    Task<DataPage<UserDTO>?> GetUsers(int page = 1, int size = 10);

    Task AddUserToRole(UserDTO user, string role);

    Task RemoveUserFromRole(UserDTO user, string role);

    Task CreateReview(int orderId, Guid reviewToken, OrderReviewCreateDTO review);

    Task<IEnumerable<ReviewDTO>> GetReviews();

    Task<UserDTO> GetUser();

    Task<bool> UpdateUser(UserDTO user);

    #endregion Public Methods
}