namespace DTOs.EpisodeDTOs.ResponseDTO
{
    public class EpisodeResponse
    {
        public int EpisodeId { get; set; }

        public int MovieId { get; set; }

        public string? MovieName { get; set; }

        public string EpisodeName { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string? MediaContent { get; set; }

        public bool IsActive { get; set; }

        public string? MediaLink { get; set; }
    }
}
