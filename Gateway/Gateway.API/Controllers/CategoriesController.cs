using CommerceFlow.Protobufs;
using CSharpFunctionalExtensions;
using Elastic.Clients.Elasticsearch.Core.Bulk;
using Elastic.Clients.Elasticsearch.Nodes;
using Gateway.API.Abstractions;
using Gateway.API.Contracts.Categories;
using Gateway.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.API.Controllers
{
    [ApiController]
    [Route("/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly IProductsService _productsService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(
            IProductsService productsService,
            ILogger<CategoriesController> logger)
        {
            _productsService = productsService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryRequest request)
        {
            var response =  await _productsService.CreateCategoryAsync(request);

            if (response.IsFailure)
            {
                return StatusCode(500, "Internal Server Error");
            }

            return Ok(response.Value);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryRequest request)
        {
            var isSuccess = await _productsService.UpdateCategoryAsync(id, request);

            if (!isSuccess)
            {
                return StatusCode(404, "Category Not Found");
            }

            return Ok("Category updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var isSuccess = await _productsService.DeleteCategoryAsync(id);

            if(!isSuccess)
            {
                return StatusCode(404, "Category Not Found");
            }

            return Ok("Category deleted");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var response = await _productsService.GetCategoryAsync(id);

            if (response.IsFailure)
            {
                return StatusCode(404, "Category Not Found");
            }

            return Ok(response.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _productsService.GetAllCategoriesAsync();

            return Ok(categories);
        }
    }
}
