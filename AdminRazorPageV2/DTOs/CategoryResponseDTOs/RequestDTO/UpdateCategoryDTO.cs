using System.ComponentModel.DataAnnotations;

namespace CategoryServices.DTOs.RequestDTO
{
    public class UpdateCategoryDTO
    {
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public string CategoryName { get; set; }
    }
}
