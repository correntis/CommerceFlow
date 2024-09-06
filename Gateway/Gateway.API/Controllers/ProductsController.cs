using Gateway.API.Contracts.Products;
using Gateway.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.API.Controllers
{
    [ApiController]
    [Route("/products")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly ProductsServiceClient _productsService;

        public ProductsController(
            ILogger<ProductsController> logger,
            ProductsServiceClient productsService
            )
        {
            _logger = logger;
            _productsService = productsService;
        }

        [HttpPost]
        public async Task<ActionResult> CreateProduct(ProductRequest request)
        {
            var id = await _productsService.CreateProductAsync(request);
            
            return Ok(id);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(int id , ProductRequest request)
        {
            if (await _productsService.UpdateProductAsync(id, request))
            {
                return Ok();  
            }

            return StatusCode(404, "Product Not Found");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            if (await _productsService.DeleteProductAsync(id))
            {
                return Ok();
            }
            return StatusCode(404, "Product Not Found");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct(int id)
        {
            var response = await _productsService.GetProductAsync(id);

            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllProducts()
        {
            var response = await _productsService.GetAllProductsAsync();

            return Ok(response);
        }
    }
}
