namespace AdminRazorPageV2.DTOs.UserDTOs.ResponseDTO
{
    public class UserResponse
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        //public DateTime RegistedDate { get; set; }
        public string? Avatar { get; set; }
        public string? Username { get; set; }
        public bool IsActive { get; set; }

    }
}
