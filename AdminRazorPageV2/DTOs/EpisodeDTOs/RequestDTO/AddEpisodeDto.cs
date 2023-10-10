namespace DTOs.EpisodeDTOs.RequestDTO
{
    public class AddEpisodeDto
    {
        public int MovieId { get; set; }

        public string EpisodeName { get; set; } = null!;

        public string Description { get; set; } = null!;

        // public byte[]? MediaContent { get; set; }

        public bool IsActive { get; set; }

        public string? MediaLink { get; set; }
    }
}
