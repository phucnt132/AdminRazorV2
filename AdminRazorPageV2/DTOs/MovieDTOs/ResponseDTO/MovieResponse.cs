namespace DTOs.MovieDTOs.ResponseDTO
{
    public class MovieResponse
    {
        public int MovieId { get; set; }
        public string MovieName { get; set; } = null!;

        public List<string> Categories { get; set; } = null!;

        public string MovieThumnailImage { get; set; } = null!;

        public string MoviePoster { get; set; } = null!;

        public int? TotalEpisodes { get; set; }

        public string Description { get; set; } = null!;

        public string ReleasedYear { get; set; } = null!;

        public string? AliasName { get; set; }

        public string? Director { get; set; }

        public string? MainCharacters { get; set; }

        public string? Trailer { get; set; }

        public string? Comments { get; set; }
    }
}
