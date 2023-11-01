using System.ComponentModel.DataAnnotations;

namespace CategoryServices.DTOs.RequestDTO
{
    public class AddCategoryDTO
    {
        [Required]
        public string CategoryName { get; set; }
    }
}
