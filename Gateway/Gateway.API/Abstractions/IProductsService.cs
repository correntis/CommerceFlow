using CommerceFlow.Protobufs;
using CSharpFunctionalExtensions;
using Gateway.API.Contracts.Categories;
using Gateway.API.Contracts.Products;

namespace Gateway.API.Abstractions
{
    public interface IProductsService
    {
        Task<Result<int, bool>> CreateCategoryAsync(CategoryRequest request);
        Task<Result<int, bool>> CreateProductAsync(ProductRequest request);
        Task<bool> DeleteCategoryAsync(int id);
        Task<bool> DeleteProductAsync(int id);
        Task<CategoriesList> GetAllCategoriesAsync();
        Task<ProductsList> GetAllProductsAsync();
        Task<Result<CategoryMessage, bool>> GetCategoryAsync(int id);
        Task<Result<ProductMessage, bool>> GetProductAsync(int id);
        Task<bool> UpdateCategoryAsync(int id, CategoryRequest request);
        Task<bool> UpdateProductAsync(int id, ProductRequest request);
    }
}