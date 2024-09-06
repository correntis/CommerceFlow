using CommerceFlow.Persistence.Abstractions;
using CommerceFlow.Persistence.Entities;
using CommerceFlow.Protobufs;
using Grpc.Core;

namespace ProductsService.API.Services
{
    public class ProductsServiceImpl : CommerceFlow.Protobufs.Server.ProductsService.ProductsServiceBase
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

        public override async Task<CreateProductResponse> CreateProduct(CreateProductRequest request, ServerCallContext context)
        {
            var product = new Product()
            {
                Name = request.Product.Name,
                Description = request.Product.Description,
                Stock = request.Product.Stock,
                Price = decimal.Parse(request.Product.Price),
            };

            product.Categories.AddRange(
                request.Product.Categories.Select(c => 
                    new Category()
                    {
                        Id = c.Id
                    }    
                )
            );

            var id = await _productsRepository.AddAsync(product);

            return new CreateProductResponse()
            {
                Id = id
            };
        }
        public override async Task<UpdateProductResponse> UpdateProduct(UpdateProductRequest request, ServerCallContext context)
        {
            var product = new Product()
            {
                Id = request.Product.Id,
                Name = request.Product.Name,
                Description = request.Product.Description,
                Stock = request.Product.Stock,
                Price = decimal.Parse(request.Product.Price),
            };

            product.Categories.AddRange(
                request.Product.Categories.Select(c =>
                    new Category()
                    {
                        Id = c.Id
                    }
                )
            );

            var rowsAffected = await _productsRepository.UpdateAsync(product);

            return new UpdateProductResponse() { IsValid = Convert.ToBoolean(rowsAffected) };
        }
        
        public override async Task<DeleteProductResponse> DeleteProduct(DeleteProductRequest request, ServerCallContext context)
        {
            var isValid = await _productsRepository.DeleteAsync(request.Id);

            return new DeleteProductResponse() { IsValid = Convert.ToBoolean(isValid) };
        }

        public override async Task<GetProductResponse> GetProduct(GetProductRequest request, ServerCallContext context)
        {
            var product = await _productsRepository.GetByIdAsync(request.Id);

            if (product is null)
            {
                return new GetProductResponse()
                {
                    IsValid = false
                };
            }

            var productMessage = new ProductMessage()
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Stock = product.Stock,
                Price = product.Price.ToString()
            };

            productMessage.Categories.AddRange(
                product.Categories.Select(c => new CategoryMessage()
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                })
            );

            return new GetProductResponse()
            {
                IsValid = true,
                Product = productMessage
            };
        }



        public override async Task<ProductsList> GetAllProducts(Empty request, ServerCallContext context)
        {
            var products = await _productsRepository.GetAllAsync();

            var productsList = new ProductsList();

            productsList.Products.AddRange(products.Select(p =>
            { 
                var productMessage = new ProductMessage()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Stock = p.Stock,
                    Price = p.Price.ToString()
                };

                productMessage.Categories.AddRange(
                    p.Categories.Select(c => new CategoryMessage()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description
                    })
                );

                return productMessage;
            }));

            return productsList;
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
