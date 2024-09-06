using CommerceFlow.Protobufs;
using CommerceFlow.Protobufs.Client;
using CSharpFunctionalExtensions;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.MachineLearning;
using Gateway.API.Abstractions;
using Gateway.API.Contracts.Categories;
using Gateway.API.Contracts.Products;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Gateway.API.Services
{
    public class ProductsServiceClient : IProductsService
    {
        private readonly ILogger<ProductsServiceClient> _logger;
        private readonly string address;

        public ProductsServiceClient(
            ILogger<ProductsServiceClient> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            address = $"http://{configuration["PRODUCTS_HOST"]}:{configuration["PRODUCTS_PORT"]}";
        }

        public async Task<Result<int, bool>> CreateProductAsync([FromBody] ProductRequest request)
        {
            var createRequest = new CreateProductRequest()
            {
                Product = new ProductMessage
                {
                    Name = request.Name,
                    Description = request.Description,
                    Stock = request.Stock,
                    Price = request.Price.ToString()
                }
            };

            createRequest.Product.Categories.AddRange(
                request.Categories.Select(c =>
                {
                    return new CategoryMessage()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description
                    };
                })
            );

            using var channel = GrpcChannel.ForAddress(address);
            var _productsService = new ProductsService.ProductsServiceClient(channel);

            var response = await _productsService.CreateProductAsync(createRequest);

            if(!response.IsSuccess)
            {
                return false;
            }

            return response.Id;
        }

        public async Task<bool> UpdateProductAsync(int id, [FromBody] ProductRequest request)
        {
            var updateRequest = new UpdateProductRequest()
            {
                Product = new ProductMessage
                {
                    Id = id,
                    Name = request.Name,
                    Description = request.Description,
                    Stock = request.Stock,
                    Price = request.Price.ToString()
                }
            };

            updateRequest.Product.Categories.AddRange(
                request.Categories.Select(c =>
                {
                    return new CategoryMessage()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description
                    };
                })
            );

            using var channel = GrpcChannel.ForAddress(address);
            var _productsService = new ProductsService.ProductsServiceClient(channel);

            var response = await _productsService.UpdateProductAsync(updateRequest);

            return response.IsSuccess;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var deleteRequest = new DeleteProductRequest() { Id = id };

            using var channel = GrpcChannel.ForAddress(address);
            var _productsService = new ProductsService.ProductsServiceClient(channel);

            var response = await _productsService.DeleteProductAsync(deleteRequest);

            return response.IsSuccess;
        }

        public async Task<Result<ProductMessage,bool>> GetProductAsync(int id)
        {
            var getRequest = new GetProductRequest() { Id = id };

            using var channel = GrpcChannel.ForAddress(address);
            var _productsService = new ProductsService.ProductsServiceClient(channel);

            var response = await _productsService.GetProductAsync(getRequest);

            if(!response.IsSuccess)
            {
                return false;
            }

            return response.Product;
        }

        public async Task<ProductsList> GetAllProductsAsync()
        {
            using var channel = GrpcChannel.ForAddress(address);
            var _productsService = new ProductsService.ProductsServiceClient(channel);

            var response = await _productsService.GetAllProductsAsync(new Empty());

            return response;
        }

        public async Task<Result<int, bool>> CreateCategoryAsync([FromBody] CategoryRequest request)
        {
            var createRequest = new CreateCategoryRequest()
            {
                Category = new()
                {
                    Name = request.Name,
                    Description = request.Description
                }
            };

            using var channel = GrpcChannel.ForAddress(address);
            var _productsService = new ProductsService.ProductsServiceClient(channel);

            var response = await _productsService.CreateCategoryAsync(createRequest);

            if(!response.IsSuccess)
            {
                return false;
            }

            return response.Id;
        }

        public async Task<bool> UpdateCategoryAsync(int id, [FromBody] CategoryRequest request)
        {
            var updateRequest = new UpdateCategoryRequest()
            {
                Category = new()
                {
                    Id = id,
                    Name = request.Name,
                    Description = request.Description
                }
            };

            using var channel = GrpcChannel.ForAddress(address);
            var _productsService = new ProductsService.ProductsServiceClient(channel);

            var updateResponse = await _productsService.UpdateCategoryAsync(updateRequest);

            return updateResponse.IsSuccess;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var deleteRequest = new DeleteCategoryRequest()
            {
                Id = id
            };

            using var channel = GrpcChannel.ForAddress(address);
            var _productsService = new ProductsService.ProductsServiceClient(channel);

            var deleteResponse = await _productsService.DeleteCategoryAsync(deleteRequest);

            return deleteResponse.IsSuccess;
        }

        public async Task<Result<CategoryMessage, bool>> GetCategoryAsync(int id)
        {
            var getRequest = new GetCategoryRequest()
            {
                Id = id
            };


            using var channel = GrpcChannel.ForAddress(address);
            var _productsService = new ProductsService.ProductsServiceClient(channel);

            var updateResponse = await _productsService.GetCategoryAsync(getRequest);

            if(!updateResponse.IsSuccess)
            {
                return false;
            }

            return updateResponse.Category;
        }

        public async Task<CategoriesList> GetAllCategoriesAsync()
        {
            using var channel = GrpcChannel.ForAddress(address);
            var _productsService = new ProductsService.ProductsServiceClient(channel);

            var getAllResponse = await _productsService.GetAllCategoriesAsync(new Empty());

            return getAllResponse;
        }
    }
}
