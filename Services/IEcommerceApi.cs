using Ecommerce.Contracts;
using Ecommerce.Client.Models;
using Ecommerce.Contracts.Requests;
using Ecommerce.Contracts.Responses;

using Refit;

namespace Ecommerce.Client.Services;

public interface IEcommerceApi
{
    #region Auth
    [Post(ApiRoutes.Auth.Register)] Task RegisterUser([Body] RegisterRequest request);

    [Post(ApiRoutes.Auth.RegisterAdmin)] Task RegisterAdmin([Body] RegisterRequest request);

    [Post(ApiRoutes.Auth.Login)]
    Task<AuthenticateResponse> LoginUser([Body] LoginRequest request);
    #endregion

    #region Token
    [Post(ApiRoutes.Token.Revoke)]
    Task RevokeToken();

    [Post(ApiRoutes.Token.Refresh)]
    Task<AuthenticateResponse> RefreshToken([Body] RefreshTokenRequest request);
    #endregion

    #region Store
    [Get(ApiRoutes.Store.GetStoreProductsPaginated)]
    Task<PaginatedList<ProductResponse>> GetAllProductsOnStore(
        [Query] int PageNumber,
        [Query] int PageSize,
        [Query] string? NameFilter = null,
        [Query] string? CategoryFilter = null);

    [Put(ApiRoutes.Store.Edit)]
    Task EditStore(int id, [Body] EditStoreRequest request);

    [Post(ApiRoutes.Store.IncreaseProduct)]
    Task IncreaseProductInStore(int id, [Query] int productId);

    [Post(ApiRoutes.Store.DecreaseProduct)]
    Task DecreaseProductInStore(int id, [Query] int productId);
    #endregion

    #region Product
    [Get(ApiRoutes.Product.GetAll)]
    Task<IEnumerable<ProductResponse>> GetAllProducts();

    [Get(ApiRoutes.Product.GetById)]
    Task<ProductResponse> GetProductById(int id);

    [Post(ApiRoutes.Product.Create)]
    Task<ProductResponse> CreateProduct([Body] CreateProductRequest request);

    [Put(ApiRoutes.Product.Edit)]
    Task EditProduct(int id, [Body] EditProductRequest request);

    [Delete(ApiRoutes.Product.Delete)]
    Task DeleteProduct(int id);
    #endregion

    #region Category
    [Get(ApiRoutes.Category.GetAll)]
    Task<IEnumerable<CategoryResponse>> GetAllCategories();

    [Get(ApiRoutes.Category.GetById)]
    Task<CategoryResponse> GetCategoryById(int id);

    [Post(ApiRoutes.Category.Create)]
    Task<CategoryResponse> CreateCategory([Body] CreateCategoryRequest request);

    [Put(ApiRoutes.Category.Edit)]
    Task EditCategory(int id, [Body] EditCategoryRequest request);

    [Delete(ApiRoutes.Category.Delete)]
    Task DeleteCategory(int id);
    #endregion

    #region Brand
    [Get(ApiRoutes.Brand.GetAll)]
    Task<IEnumerable<BrandResponse>> GetAllBrands();

    [Get(ApiRoutes.Brand.GetById)]
    Task<BrandResponse> GetBrandById(int id);

    [Post(ApiRoutes.Brand.Create)]
    Task<BrandResponse> CreateBrand([Body] CreateBrandRequest request);

    [Put(ApiRoutes.Brand.Edit)]
    Task EditBrand(int id, [Body] EditBrandRequest request);

    [Delete(ApiRoutes.Brand.Delete)]
    Task DeleteBrand(int id);
    #endregion

    #region Basket
    [Get(ApiRoutes.Basket.GetProducts)]
    Task<BasketResponse> GetProductsInBasket();

    [Post(ApiRoutes.Basket.AddProduct)]
    Task AddProductToBasket(int productId);

    [Post(ApiRoutes.Basket.IncreaseProduct)]
    Task IncreaseProductInBasket(int productId);

    [Post(ApiRoutes.Basket.DecreaseProduct)]
    Task DecreaseProductInBasket(int productId);

    [Delete(ApiRoutes.Basket.RemoveProduct)]
    Task RemoveProductFromBasket(int productId);

    [Get(ApiRoutes.Basket.GetProductIds)]
    Task<int[]> GetProductIdsInBasket();
    #endregion

    #region Payment
    [Post(ApiRoutes.Payment.Refound)]
    Task<object> RefoundCharge([Query] string chargeToken);

    [Post(ApiRoutes.Payment.Pay)]
    Task<object> Pay([Body] PayRequest request);
    #endregion
}