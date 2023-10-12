using AdminRazorPageV2.Models;
using APIS.DTOs.MovieDTOs.RequestDto;
using AutoMapper;
using DTOs.EpisodeDTOs.RequestDTO;
using DTOs.EpisodeDTOs.ResponseDTO;
using DTOs.MovieDTOs.ResponseDTO;

namespace HighFlixAdmin
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            //for movie
            CreateMap<MovieResponse, Movie>();
            CreateMap<Movie, MovieResponse>();
            CreateMap<AddMovieDto, Movie>();
            CreateMap<UpdateMovieDto, Movie>();

            //for episode
            CreateMap<EpisodeResponse, Episode>();
            CreateMap<Episode, EpisodeResponse>();
            CreateMap<AddEpisodeDto, Episode>();
            CreateMap<UpdateEpisodeDto, Episode>();
            CreateMap<DeleteEpisodeDto, Episode>();
        }
    }
}
