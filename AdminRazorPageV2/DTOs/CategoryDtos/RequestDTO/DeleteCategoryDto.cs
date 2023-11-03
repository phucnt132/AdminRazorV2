namespace AdminRazorPageV2.DTOs.CategoryDtos.RequestDTO
{
    public class DeleteCategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }
    }
}
