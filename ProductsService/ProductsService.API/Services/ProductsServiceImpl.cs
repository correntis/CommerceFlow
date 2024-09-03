using Grpc.Core;
using ProductsService.API;

namespace ProductsService.API.Services
{
    public class ProductsServiceImpl : ProductsService.ProductsServiceBase
    {
        private readonly ILogger<ProductsServiceImpl> _logger;
        public ProductsServiceImpl(
            ILogger<ProductsServiceImpl> logger
            )
        {
            _logger = logger;

        }

    }
}
