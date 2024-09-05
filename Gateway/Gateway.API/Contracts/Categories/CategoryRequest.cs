using System.ComponentModel.DataAnnotations;

namespace Gateway.API.Contracts.Categories
{
    public class CategoryRequest
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }
    }
}
