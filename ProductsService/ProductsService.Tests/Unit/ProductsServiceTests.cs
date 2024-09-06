using CommerceFlow.Persistence;
using CommerceFlow.Persistence.Abstractions;
using CommerceFlow.Persistence.Repositories;
using CommerceFlow.Protobufs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ProductsService.API.Services;

namespace ProductsService.Tests
{
    public class ProductsServiceTests
    {
        private readonly ProductsServiceImpl _productsService;
        private readonly CommerceDbContext _context;
        private readonly IProductsRepository _productsRepository;
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly Mock<ILogger<ProductsServiceImpl>> _productsServiceLogger;
        private readonly Mock<ILogger<CategoriesRepository>> _categoriesRepositryLogger;
        private readonly Mock<ILogger<CommerceDbContext>> _contextLogger;

        public ProductsServiceTests()
        {
            _productsServiceLogger = new Mock<ILogger<ProductsServiceImpl>>();
            _categoriesRepositryLogger = new Mock<ILogger<CategoriesRepository>>();
            _contextLogger = new Mock<ILogger<CommerceDbContext>>();

            var contextOptionsBuilder = new DbContextOptionsBuilder<CommerceDbContext>()
                .UseInMemoryDatabase("ProductsServiceTests");

            _context = new CommerceDbContext(contextOptionsBuilder.Options);

            _categoriesRepository = new CategoriesRepository(_context, _categoriesRepositryLogger.Object);
            _productsRepository = new ProductsRepository(_context, _contextLogger.Object);

            _productsService = new ProductsServiceImpl(_productsServiceLogger.Object, _productsRepository, _categoriesRepository);
        }

        [Fact]
        public async Task CreateProductWithCategory()
        {
            var categoryMessage = CreateCategoryMessage(10);
            var createCategoryRequest = new CreateCategoryRequest() { Category = categoryMessage };

            var createCategoryResponse = await _productsService.CreateCategory(createCategoryRequest, null);

            Assert.NotNull(createCategoryResponse);


           var productMessage = CreateProductMessage(1);
            categoryMessage.Id = createCategoryResponse.Id;

            productMessage.Categories.Add(categoryMessage);

            var createProductRequest = new CreateProductRequest() { Product = productMessage };
            var createProductResponse = await _productsService.CreateProduct(createProductRequest, null);

            Assert.NotNull(createProductResponse);
        }


        [Fact]
        public async Task UpdateProductWithCategory()
        {
            var categoryMessage = CreateCategoryMessage(10);
            var createCategoryRequest = new CreateCategoryRequest() { Category = categoryMessage };

            var createCategoryResponse = await _productsService.CreateCategory(createCategoryRequest, null);

            Assert.NotNull(createCategoryResponse);


            var productMessage = CreateProductMessage(1);
            categoryMessage.Id = createCategoryResponse.Id;

            productMessage.Categories.Add(categoryMessage);

            var createProductRequest = new CreateProductRequest() { Product = productMessage };
            var createProductResponse = await _productsService.CreateProduct(createProductRequest, null);

            Assert.NotNull(createProductResponse);

            productMessage.Id = createProductResponse.Id;
            productMessage.Categories.Clear();
            productMessage.Name = "test";
            
            var updateProductRequest = new UpdateProductRequest() { Product = productMessage };
            var updateProductResponse = await _productsService.UpdateProduct(updateProductRequest, null);

            Assert.NotNull(updateProductResponse);
            Assert.True(updateProductResponse.IsValid);
        }

        [Fact]
        public async Task CreateCategory()
        {
            var categoryMessage = CreateCategoryMessage(0);
            var createRequest = new CreateCategoryRequest() { Category =  categoryMessage };

            var response = await _productsService.CreateCategory(createRequest, null);
            var category = await _productsService.GetCategory(new() { Id = response.Id }, null);

            Assert.NotNull(response);
            Assert.NotNull(category);
            Assert.True(category.IsValid);
            Assert.True(category.Category.Id == response.Id);
        }

        [Fact]
        public async Task UpdateCategory()
        {
            var categoryMessage = CreateCategoryMessage(1);
            var createRequest = new CreateCategoryRequest() { Category = categoryMessage };

            var response = await _productsService.CreateCategory(createRequest, null);
            Assert.NotNull(response);

            var newValue = "new";
            var updateRequest = new UpdateCategoryRequest() 
            { 
                Category = new()
                {
                    Id = response.Id,
                    Name = newValue,
                    Description = newValue
                }
            };

            var updateResponse = await _productsService.UpdateCategory(updateRequest, null);
            var categoryResponse = await _productsService.GetCategory(new GetCategoryRequest() { Id = response.Id }, null);

            Assert.NotNull(updateResponse);
            Assert.NotNull(categoryResponse);
            Assert.True(updateResponse.IsValid);
            Assert.True(categoryResponse.IsValid);
            Assert.Equal(newValue, categoryResponse.Category.Name);
            Assert.Equal(newValue, categoryResponse.Category.Description);
        }

        [Fact]
        public async Task DeleteCategory()
        {
            var categoryMessage = CreateCategoryMessage(2);
            var createRequest = new CreateCategoryRequest() { Category = categoryMessage };

            var response = await _productsService.CreateCategory(createRequest, null);
            Assert.NotNull(response);


            var deleteResponse = await _productsService.DeleteCategory(new() { Id = response.Id }, null);
            var categoryResponse = await _productsService.GetCategory(new GetCategoryRequest() { Id = response.Id }, null);

            Assert.NotNull(deleteResponse);
            Assert.True(deleteResponse.IsValid);
            Assert.False(categoryResponse.IsValid);
        }

        [Fact]
        public async Task GetCategory()
        {
            var categoryMessage = CreateCategoryMessage(3);
            var createRequest = new CreateCategoryRequest() { Category = categoryMessage };

            var response = await _productsService.CreateCategory(createRequest, null);
            Assert.NotNull(response);

            var categoryResponse = await _productsService.GetCategory(new GetCategoryRequest() { Id = response.Id }, null);

            Assert.NotNull(categoryResponse);
            Assert.True(categoryResponse.IsValid);
        }

        [Fact]
        public async Task GetAllCategories()
        {
            var categoriesResponse = await _productsService.GetAllCategories(new(), null);

            Assert.NotNull(categoriesResponse);
            Assert.NotNull(categoriesResponse.Categories);
        }

        public CategoryMessage CreateCategoryMessage(int i)
        {
            return new CategoryMessage 
            { 
                Name = "name" + i,
                Description = "description" + i,
            };
        }

        public ProductMessage CreateProductMessage(int i)
        {
            return new ProductMessage
            {
                Name = "name" + i,
                Description = "description" + i,
                Stock = i,
                Price = "123"
            };
        }
    }
}