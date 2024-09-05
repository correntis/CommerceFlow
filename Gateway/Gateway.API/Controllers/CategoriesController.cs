using CommerceFlow.Protobufs;
using Elastic.Clients.Elasticsearch.Nodes;
using Gateway.API.Contracts.Categories;
using Gateway.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.API.Controllers
{
    [ApiController]
    [Route("/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly ProductsServiceClient _productsService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(
            ProductsServiceClient productsService,
            ILogger<CategoriesController> logger)
        {
            _productsService = productsService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<int> CreateCategory(CategoryRequest request)
        {
            return await _productsService.CreateCategoryAsync(request);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, CategoryRequest request)
        {
            var isValid = await _productsService.UpdateCategoryAsync(id, request);

            if (!isValid)
            {
                return StatusCode(404, "Category Not Found");
            }

            return Ok("Category updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var isValid = await _productsService.DeleteCategoryAsync(id);

            if(!isValid)
            {
                return StatusCode(404, "Category Not Found");
            }

            return Ok("Category deleted");
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryMessage>> GetCategory(int id)
        {
            var response = await _productsService.GetCategoryAsync(id);

            if (response.IsFailure)
            {
                return StatusCode(404, "Category Not Found");
            }

            return Ok(response.Value);
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryMessage>>> GetAllCategories()
        {
            var categories = await _productsService.GetAllCategoriesAsync();

            return Ok(categories);
        }
    }
}
