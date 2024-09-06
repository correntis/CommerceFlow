using Gateway.API.Contracts.Categories;

namespace Gateway.API.Contracts.Products
{
    public class ProductRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        public List<CategoryRequest> Categories { get; set; }
    }
}
