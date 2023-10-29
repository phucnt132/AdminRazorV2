using AdminRazorPageV2.DTOs.CategoryDtos.RequestDTO;
using AdminRazorPageV2.DTOs.CategoryDtos.ResponseDTO;
using AdminRazorPageV2.Models;
using DTOs.MovieDTOs.RequestDto;
using AutoMapper;
using DTOs.EpisodeDTOs.RequestDTO;
using DTOs.EpisodeDTOs.ResponseDTO;
using DTOs.MovieDTOs.ResponseDTO;
using DTOs.CommentDTOs.ResponseDTO;
using DTOs.CommentDTOs.RequestDTO;

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

            //for category
            CreateMap<CategoryResponse, Category>();
            CreateMap<Category, CategoryResponse>();
            CreateMap<AddCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();
            CreateMap<DeleteCategoryDto, Category>();

            //for comment
            CreateMap<CommentResponse, Comment>();
            CreateMap<Comment, CommentResponse>();
            CreateMap<DeleteCommentDto, Comment>();
        }
    }
}
