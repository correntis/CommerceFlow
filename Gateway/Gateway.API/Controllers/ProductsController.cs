using Gateway.API.Abstractions;
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
        private readonly IProductsService _productsService;

        public ProductsController(
            ILogger<ProductsController> logger,
            IProductsService productsService
            )
        {
            _logger = logger;
            _productsService = productsService;
        }

        [HttpPost]
        public async Task<ActionResult> CreateProduct(ProductRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _productsService.CreateProductAsync(request);

            if (response.IsFailure)
            {
                return StatusCode(500, "Internal Server Error");
            }

            return Ok(response.Value);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(int id , ProductRequest request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

            if(response.IsFailure)
            {
                return StatusCode(404, "Product Not Found");
            }

            return Ok(response.Value);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllProducts()
        {
            var response = await _productsService.GetAllProductsAsync();

            return Ok(response);
        }
    }
}
