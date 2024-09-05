using CommerceFlow.Protobufs;
using CommerceFlow.Protobufs.Client;
using CSharpFunctionalExtensions;
using Elastic.Clients.Elasticsearch.MachineLearning;
using Gateway.API.Contracts.Categories;
using Grpc.Net.Client;

namespace Gateway.API.Services
{
    public class ProductsServiceClient
    {
        private readonly ILogger<ProductsServiceClient> _logger;
        private readonly string address;

        public ProductsServiceClient(
            ILogger<ProductsServiceClient> logger,
            IConfiguration configuration)
        {
            logger = _logger;
            address = $"http://{configuration["PRODUCTS_HOST"]}:{configuration["PRODUCTS_PORT"]}";
        }

        public async Task<int> CreateCategoryAsync(CategoryRequest request)
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

            return response.Id;
        }

        public async Task<bool> UpdateCategoryAsync(int id, CategoryRequest request)
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

            return updateResponse.IsValid;
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

            return deleteResponse.IsValid;
        }

        public async Task<Result<CategoryMessage, Error>> GetCategoryAsync(int id)
        {
            var getRequest = new GetCategoryRequest()
            {
                Id = id
            };


            using var channel = GrpcChannel.ForAddress(address);
            var _productsService = new ProductsService.ProductsServiceClient(channel);

            var updateResponse = await _productsService.GetCategoryAsync(getRequest);

            if (!updateResponse.IsValid)
            {
                return new Error() { Code = 404, Message = "Category Not Found" };
            }

            return updateResponse.Category;
        }

        public async Task<List<CategoryMessage>> GetAllCategoriesAsync()
        {
            using var channel = GrpcChannel.ForAddress(address);
            var _productsService = new ProductsService.ProductsServiceClient(channel);

            var getAllResponse = await _productsService.GetAllCategoriesAsync(new Empty());

            return [.. getAllResponse.Categories];
        }
    }
}
