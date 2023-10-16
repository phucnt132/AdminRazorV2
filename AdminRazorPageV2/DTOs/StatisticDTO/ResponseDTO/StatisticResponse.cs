namespace AdminRazorPageV2.DTOs.StatisticDTO.ResponseDTO
{
    public class StatisticResponse
    {
        public string MovieName { get; set; } = null!;
        public string MovieThumnailImage { get; set; } = null!;
        public string ReleasedYear { get; set; } = null!;
        public int View { get; set; }
    }
}
