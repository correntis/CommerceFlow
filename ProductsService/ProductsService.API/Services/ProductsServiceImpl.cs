using CommerceFlow.Persistence.Abstractions;
using CommerceFlow.Persistence.Entities;
using CommerceFlow.Protobufs;
using Grpc.Core;

namespace ProductsService.API.Services
{
    public class ProductsServiceImpl : CommerceFlow.Protobufs.ProductsService.ProductsServiceBase
    {
        private readonly ILogger<ProductsServiceImpl> _logger;
        private readonly IProductsRepository _productsRepository;
        private readonly ICategoriesRepository _categoriesRepository;

        public ProductsServiceImpl(
            ILogger<ProductsServiceImpl> logger,
            IProductsRepository productsRepository,
            ICategoriesRepository categoriesRepository
            )
        {
            _logger = logger;
            _productsRepository = productsRepository;
            _categoriesRepository = categoriesRepository;
        }

        public override async Task<CreateCategoryResponse> CreateCategory(CreateCategoryRequest request, ServerCallContext context)
        {   
            var category = new Category()
            {
                Name = request.Category.Name,
                Description = request.Category.Description,
            };
            
            var id = await _categoriesRepository.AddAsync(category);

            return new CreateCategoryResponse() { Id = id };
        }

        public override async Task<UpdateCategoryResponse> UpdateCategory(UpdateCategoryRequest request, ServerCallContext context)
        {
            var category = new Category()
            {
                Id = request.Category.Id,
                Name = request.Category.Name,
                Description = request.Category.Description,
            };

            var rowsAffected = await _categoriesRepository.UpdateAsync(category);

            return new UpdateCategoryResponse()
            {
                IsValid = Convert.ToBoolean(rowsAffected)
            };
        }

        public override async Task<DeleteCategoryResponse> DeleteCategory(DeleteCategoryRequest request, ServerCallContext context)
        {
            var rowsAffected = await _categoriesRepository.DeleteAsync(request.Id);

            return new DeleteCategoryResponse()
            {
                IsValid = Convert.ToBoolean(rowsAffected)
            };
        }

        public override async Task<GetCategoryResponse> GetCategory(GetCategoryRequest request, ServerCallContext context)
        {
            var category = await _categoriesRepository.GetByIdAsync(request.Id);

            if (category is null)
            {
                return new GetCategoryResponse() { IsValid = false };
            }

            return new GetCategoryResponse()
            {
                IsValid = true,
                Category = new CategoryMessage()
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                }
            };
        }

        public override async Task<CategoriesList> GetAllCategories(Empty request, ServerCallContext context)
        {
            var categories = await _categoriesRepository.GetAllAsync();

            var response = new CategoriesList();

            response.Categories.AddRange(
                categories.Select(c =>
                {
                    return new CategoryMessage()
                    { 
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description
                    };
                })
            );

            return response;
        }
    }
}
